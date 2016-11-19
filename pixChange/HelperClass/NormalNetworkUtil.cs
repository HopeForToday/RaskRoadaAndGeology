using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.NetworkAnalyst;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RoadRaskEvaltionSystem.HelperClass
{
    public class NormalNetworkUtil
    {
        /// <summary>
        /// 打开工作空间
        /// </summary>
        /// <param name="strMDBName"></param>
        /// <returns></returns>
        public static IWorkspace OpenWorkspace(string strMDBName)
        {
            IWorkspaceFactory pWorkspaceFactory = new AccessWorkspaceFactoryClass();
            return pWorkspaceFactory.OpenFromFile(strMDBName, 0);
        }
        public static INetworkDataset OpenNetworkDataset(IWorkspace pWorkspace, string featureDatasetName, string sNDSName)
        {
            try
            {
                IFeatureWorkspace pFeatureWorkspace = pWorkspace as IFeatureWorkspace;
                if (pFeatureWorkspace == null) return null;
                IFeatureDatasetExtensionContainer pFeatureDatasetExtensionContainer = pFeatureWorkspace.OpenFeatureDataset(featureDatasetName) as IFeatureDatasetExtensionContainer;
                if (pFeatureDatasetExtensionContainer == null) return null;
                IDatasetContainer2 pDatasetContainer2 = pFeatureDatasetExtensionContainer.FindExtension(esriDatasetType.esriDTNetworkDataset) as IDatasetContainer2;
                INetworkDataset pNetworkDataset = pDatasetContainer2.get_DatasetByName(esriDatasetType.esriDTAny, sNDSName) as INetworkDataset;
                return pNetworkDataset;
            }
            catch(Exception e)
            {
                Debug.Print(e.Message);
                return null;
            }
        }
        /// <summary>
        /// 打开网络数据集
        /// </summary>
        /// <param name="GDBfileName"></param>
        /// <param name="featureDatasetName"></param>
        /// <param name="sNDSName"></param>
        /// <returns></returns>
        public static INetworkDataset OpenNetworkDataset(string GDBfileName, string featureDatasetName, string sNDSName)
        {
            try
            {
                IWorkspace pWorkspace = OpenWorkspace(GDBfileName);
                IFeatureWorkspace pFeatureWorkspace = pWorkspace as IFeatureWorkspace;
                if (pFeatureWorkspace == null) return null;
                IFeatureDatasetExtensionContainer pFeatureDatasetExtensionContainer = pFeatureWorkspace.OpenFeatureDataset(featureDatasetName) as IFeatureDatasetExtensionContainer;
                if (pFeatureDatasetExtensionContainer == null) return null;
                IDatasetContainer2 pDatasetContainer2 = pFeatureDatasetExtensionContainer.FindExtension(esriDatasetType.esriDTNetworkDataset) as IDatasetContainer2;
                INetworkDataset pNetworkDataset = pDatasetContainer2.get_DatasetByName(esriDatasetType.esriDTAny, sNDSName) as INetworkDataset;
                return pNetworkDataset;
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                return null;
            }
        }
        /// <summary>
        /// 初始化地图和网络数据集
        /// </summary>
        /// <param name="mapControl"></param>
        /// <param name="gdbfileName"></param>
        /// <param name="featureDatasetName"></param>
        /// <param name="ndsName"></param>
        public static void Initial(AxMapControl mapControl, string gdbfileName, string featureDatasetName, string ndsName)
        {
            mapControl.ActiveView.Clear();
            mapControl.ActiveView.Refresh();
            IWorkspace pWorkspace = OpenWorkspace(gdbfileName);
            IFeatureWorkspace pFeatureWorkspace = pWorkspace as IFeatureWorkspace;
            INetworkDataset pNetworkDataset = OpenNetworkDataset(gdbfileName, featureDatasetName, ndsName);

            INAContext pNAContext = CreateNAContext(pNetworkDataset);

            IFeatureClass pInputFC = pFeatureWorkspace.OpenFeatureClass("stop");

            IFeatureClass pVertexFC = pFeatureWorkspace.OpenFeatureClass("TestNet_ND_Junctions");

            IFeatureLayer pVertexFL = new FeatureLayerClass();
            pVertexFL.FeatureClass = pVertexFC;
            pVertexFL.Name = pVertexFL.FeatureClass.AliasName;
            IFeatureLayer pRoadFL = new FeatureLayerClass();
            pRoadFL.FeatureClass = pFeatureWorkspace.OpenFeatureClass("道路中心线");
            pRoadFL.Name = pRoadFL.FeatureClass.AliasName;
            mapControl.AddLayer(pRoadFL, 0);

            ILayer pLayer;
            INetworkLayer pNetworkLayer = new NetworkLayerClass();
            pNetworkLayer.NetworkDataset = pNetworkDataset;
            pLayer = pNetworkLayer as ILayer;
            pLayer.Name = "Network Dataset";
            mapControl.AddLayer(pLayer, 0);

            //Create a Network Analysis Layer and add to ArcMap
            INALayer naLayer = pNAContext.Solver.CreateLayer(pNAContext);
            pLayer = naLayer as ILayer;
            pLayer.Name = pNAContext.Solver.DisplayName;
            mapControl.AddLayer(pLayer, 0);

            IActiveView pActiveView = mapControl.ActiveView;
            IMap pMap = pActiveView.FocusMap;
            IGraphicsContainer pGraphicsContainer = pMap as IGraphicsContainer;
        }

        /// <summary>
        ///  根据网络数据集创建网络分析上下文
        /// </summary>
        /// <param name="networkDataset"></param>
        /// <returns></returns>
        public static INAContext CreateNAContext(INetworkDataset networkDataset)
        {
            IDENetworkDataset pDENetworkDataset = GetDENetworkDataset(networkDataset);
            INASolver pNASolver = new NARouteSolverClass();
            INAContextEdit pNAContextEdit = pNASolver.CreateContext(pDENetworkDataset, pNASolver.Name) as INAContextEdit;
            pNAContextEdit.Bind(networkDataset, new GPMessagesClass());
            return pNAContextEdit as INAContext;
        }
        /// <summary>
        /// 获取IDENetworkDataset
        /// </summary>
        /// <param name="networkDataset"></param>
        /// <returns></returns>
        public static IDENetworkDataset GetDENetworkDataset(INetworkDataset networkDataset)
        {
            IDatasetComponent dsComponent = networkDataset as IDatasetComponent;
            return dsComponent.DataElement as IDENetworkDataset;
        }

       /// <summary>
       /// 插入相关元素 如经过站点、障碍点等
       /// </summary>
       /// <param name="pNAContext"></param>
       /// <param name="strNAClassName"></param>
       /// <param name="inputFC"></param>
       /// <param name="dSnapTolerance"></param>
        public static void LoadNANetWorkLocations(INAContext pNAContext, string strNAClassName, IFeatureClass inputFC, double dSnapTolerance)
        {
            INAClass pNAClass;
            INamedSet pNamedSet;
            pNamedSet = pNAContext.NAClasses;
            pNAClass = pNamedSet.get_ItemByName(strNAClassName) as INAClass;
            for (int i = 0; i < pNamedSet.Count; i++)
            {
                INAClass temp = pNamedSet.get_Item(i) as INAClass;
                string name=pNamedSet.get_Name(i);
            }
            ISpatialFilter filer = new SpatialFilterClass();
            filer.WhereClause = "";
            int count = inputFC.FeatureCount(filer as IQueryFilter);
             //删除已存在的元素
            pNAClass.DeleteAllRows();
            //创建NAClassLoader，设置捕捉容限值
            INAClassLoader pNAClassLoader = new NAClassLoaderClass();
            pNAClassLoader.Locator = pNAContext.Locator;
            if (dSnapTolerance > 0)
            {
                pNAClassLoader.Locator.SnapTolerance = dSnapTolerance;
            }
            pNAClassLoader.NAClass = pNAClass;
            //字段匹配
            INAClassFieldMap pNAClassFieldMap = new NAClassFieldMapClass();
            pNAClassFieldMap.CreateMapping(pNAClass.ClassDefinition, inputFC.Fields);
            pNAClassLoader.FieldMap = pNAClassFieldMap;
            //加载要素类数据
            int iRows = 0;
            int iRowsLocated = 0;
            IFeatureCursor pFeatureCursor = inputFC.Search(null, true);
            pNAClassLoader.Load((ICursor)pFeatureCursor, null, ref iRows, ref iRowsLocated);
            ((INAContextEdit)pNAContext).ContextChanged();
        }
        //路径分析
        public static void Short_Path(AxMapControl mapControl, string gdbfileName, string featureDatasetName, string ndsName, IFeatureClass stopFeatureClass, IFeatureClass barriesFeatureClass, double dSnapTolerance)
        {
            mapControl.ClearLayers();
            //打开工作空间
            IFeatureWorkspace pFeatureWorkspace = OpenWorkspace(gdbfileName) as IFeatureWorkspace;
            //获取网络数据集
            INetworkDataset pNetworkDataset = OpenNetworkDataset(pFeatureWorkspace as IWorkspace, featureDatasetName, ndsName);
           //获取网络分析上下文
            INAContext pNAContext = CreateNAContext(pNetworkDataset);
            //Initial(mapControl, gdbfileName, featureDatasetName, ndsName);
           // IFeatureClass pInputFC = pFeatureWorkspace.OpenFeatureClass("stop");
            //打开节点图层 一般和网络数据集放置在一起 名称是xxx_Junctions
            IFeatureClass pVertexFC = pFeatureWorkspace.OpenFeatureClass(ndsName + "_Junctions");
            // 显示网络数据集图层 
            INetworkLayer pNetworkLayer = new NetworkLayerClass();
            pNetworkLayer.NetworkDataset = pNetworkDataset;
            ILayer pLayer = pNetworkLayer as ILayer;
            pLayer.Name = "Network Dataset";
            mapControl.AddLayer(pLayer, 0);

            //Create a Network Analysis Layer and add to ArcMap
            INALayer naLayer = pNAContext.Solver.CreateLayer(pNAContext);
             pLayer = naLayer as ILayer;
            pLayer.Name = pNAContext.Solver.DisplayName;
            pLayer.SpatialReference = mapControl.SpatialReference;
            mapControl.AddLayer(pLayer, 0);
            IActiveView pActiveView = mapControl.ActiveView;
            IMap pMap = pActiveView.FocusMap;
            IGraphicsContainer pGraphicsContainer = pMap as IGraphicsContainer;
            INASolver naSolver = pNAContext.Solver;
            //插入经过点
            LoadNANetWorkLocations(pNAContext,"Stops", stopFeatureClass, 30);
            //插入障碍点
            LoadNANetWorkLocations(pNAContext, "Barriers", barriesFeatureClass, 30);
            IGPMessages gpMessages = new GPMessagesClass();
         //   INASolver naSolver = pNAContext.Solver;
            // naSolver.
           //  INARouteSolver pRouteSolver = naSolver as INARouteSolver;
           //   pRouteSolver.
         //  SetSolverSettings(pNAContext);
          //  pNAContext.Solver.
            //寻找最短路径
            mapControl.Refresh();
            return;
            pNAContext.Solver.Solve(pNAContext, gpMessages, new CancelTrackerClass());
            mapControl.Refresh();
            if (gpMessages != null)
            {
                for (int i = 0; i < gpMessages.Count; i++)
                {
                    switch (gpMessages.GetMessage(i).Type)
                    {
                        case esriGPMessageType.esriGPMessageTypeError:
                            MessageBox.Show("错误 " + gpMessages.GetMessage(i).ErrorCode.ToString() + " " + gpMessages.GetMessage(i).Description);
                            break;
                        case esriGPMessageType.esriGPMessageTypeWarning:
                            MessageBox.Show("警告 " + gpMessages.GetMessage(i).Description);
                            break;
                        default:
                            MessageBox.Show("信息 " + gpMessages.GetMessage(i).Description);
                            break;
                    }
                }
            }
        }

        private static void SetSolverSettings(INAContext pNAContext)
        {
            if (pNAContext.Solver.CanAccumulateAttributes)
            {
                INASolver naSolver = pNAContext.Solver;
                INARouteSolver naRouteSolver = naSolver as INARouteSolver;
                naRouteSolver.FindBestSequence = true;
                naRouteSolver.PreserveFirstStop = true;
                naRouteSolver.PreserveLastStop = false;
                naRouteSolver.UseTimeWindows = false;
                naRouteSolver.OutputLines = esriNAOutputLineType.esriNAOutputLineTrueShapeWithMeasure;
                INASolverSettings2 naSolverSettings = naSolver as INASolverSettings2;
                IStringArray restrictions = naSolverSettings.RestrictionAttributeNames;
                restrictions.Add("Oneway");
                naSolverSettings.RestrictionAttributeNames = restrictions;
                //基于上述设置更新Solver  
                naSolver.UpdateContext(pNAContext, GetDENetworkDataset(pNAContext.NetworkDataset), new GPMessagesClass()); 
                
            }
        }

    }
}
