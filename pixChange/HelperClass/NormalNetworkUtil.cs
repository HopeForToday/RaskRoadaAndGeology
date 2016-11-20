using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.NetworkAnalyst;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RoadRaskEvaltionSystem.HelperClass
{
    /// <summary>
    /// 传输网络分析封装类 
    /// 2016//11/20 FHR
    /// </summary>
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
            IWorkspace pWorkspace=null;
            try
            {
                pWorkspace = pWorkspaceFactory.OpenFromFile(strMDBName, 0);
            }
            catch (Exception e)
            {
                Debug.Print("打开工作空间出错:"+e.Message);
            }
            return pWorkspace;
        }
        /// <summary>
        /// 打开网络数据集
        /// </summary>
        /// <param name="pWorkspace"></param>
        /// <param name="featureDatasetName"></param>
        /// <param name="sNDSName"></param>
        /// <returns></returns>
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
        ///  根据网络数据集创建网络分析上下文
        /// </summary>
        /// <param name="networkDataset"></param>
        /// <returns></returns>
        private static INAContext CreateNAContext(INetworkDataset networkDataset)
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
        private static IDENetworkDataset GetDENetworkDataset(INetworkDataset networkDataset)
        {
            IDatasetComponent dsComponent = networkDataset as IDatasetComponent;
            return dsComponent.DataElement as IDENetworkDataset;
        }

       /// <summary>
       /// 以图层的方法插入相关元素 如经过站点、障碍点、障碍线、障碍多边形等
       /// </summary>
       /// <param name="pNAContext">网络分析上下文</param>
        /// <param name="strNAClassName">Stops、Barriers、PolylineBarriers\PolygonBarriers</param>
       /// <param name="inputFeatuerClass"></param>
       /// <param name="dSnapTolerance"></param>
        public static void LoadNANetWorkLocations(INAContext pNAContext, string strNAClassName, IFeatureClass inputFeatuerClass, double dSnapTolerance)
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
            int count = inputFeatuerClass.FeatureCount(filer as IQueryFilter);
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
            pNAClassFieldMap.CreateMapping(pNAClass.ClassDefinition, inputFeatuerClass.Fields);
            pNAClassLoader.FieldMap = pNAClassFieldMap;
            //加载要素类数据
            int iRows = 0;
            int iRowsLocated = 0;
            IFeatureCursor pFeatureCursor = inputFeatuerClass.Search(null, true);
            pNAClassLoader.Load((ICursor)pFeatureCursor, null, ref iRows, ref iRowsLocated);
            ((INAContextEdit)pNAContext).ContextChanged();
            
        }
        /// <summary>
        /// 最短路径分析
        /// </summary>
        /// <param name="mapControl">地图控件</param>
        /// <param name="gdbfileName">数据库文件</param>
        /// <param name="featureDatasetName">要素集名字</param>
        /// <param name="ndsName">网络数据集名字</param>
        /// <param name="featureClasses">参数要素类字段,键值包括:Stops(路线经过结点),Barriers,PolylineBarriers,PolygonBarriers</param>
        /// <param name="dSnapTolerance"></param>
        public static bool Short_Path(AxMapControl mapControl, string gdbfileName, string featureDatasetName, string ndsName, IDictionary<string,IFeatureClass> featureClasses, double dSnapTolerance)
        {
            //首先判断输入的参数要素类是否合法
            if (!FeatureClassKeyIsRight(featureClasses))
            {
                throw new Exception("参数字典错误");
            }
           // mapControl.ClearLayers();
            //打开工作空间
            IFeatureWorkspace pFeatureWorkspace = OpenWorkspace(gdbfileName) as IFeatureWorkspace;
            if (pFeatureWorkspace == null) { return false; }
            //获取网络数据集
            INetworkDataset pNetworkDataset = OpenNetworkDataset(pFeatureWorkspace as IWorkspace, featureDatasetName, ndsName);
            if (pNetworkDataset == null)
            {
                Debug.Print("无法获取网络数据集"); 
                return false;
            }
            //获取网络分析上下文
            INAContext pNAContext = CreateNAContext(pNetworkDataset);
            //打开节点图层 一般和网络数据集放置在一起 名称是xxx_Junctions
            IFeatureClass pVertexFC = pFeatureWorkspace.OpenFeatureClass(ndsName + "_Junctions");
            // 显示网络数据集图层 
            INetworkLayer pNetworkLayer = new NetworkLayerClass();
            pNetworkLayer.NetworkDataset = pNetworkDataset;
            ILayer pLayer = pNetworkLayer as ILayer;
            pLayer.Name = "Network Dataset";
            mapControl.AddLayer(pLayer, 0);
            //创建网络分析图层
            INALayer naLayer = pNAContext.Solver.CreateLayer(pNAContext);
             pLayer = naLayer as ILayer;
            pLayer.Name = pNAContext.Solver.DisplayName;
            pLayer.SpatialReference = mapControl.SpatialReference;
            mapControl.AddLayer(pLayer, 0);
            IActiveView pActiveView = mapControl.ActiveView;
            IMap pMap = pActiveView.FocusMap;
            IGraphicsContainer pGraphicsContainer = pMap as IGraphicsContainer;
            mapControl.Refresh();
            INASolver naSolver = pNAContext.Solver;
            //插入相关数据
            foreach (var value in featureClasses)
            {
                LoadNANetWorkLocations(pNAContext, value.Key, value.Value, dSnapTolerance);
            }
            //插入经过点
            //LoadNANetWorkLocations(pNAContext,"Stops", stopFeatureClass, 30);
            //插入障碍点
          //    LoadNANetWorkLocations(pNAContext, "Barriers", barriesFeatureClass, 30);
            IGPMessages gpMessages = new GPMessagesClass();
         //  SetSolverSettings(pNAContext);
            //寻找最短路径
            try
            {
                pNAContext.Solver.Solve(pNAContext, gpMessages, new CancelTrackerClass());
            }
            catch (Exception e)
            {
                Debug.Print("无法找到最短路径:" + e.Message);
                return false;
            }
             mapControl.Refresh();
                if (gpMessages != null)
                {
                    for (int i = 0; i < gpMessages.Count; i++)
                    {
                        switch (gpMessages.GetMessage(i).Type)
                        {
                            case esriGPMessageType.esriGPMessageTypeError:
                                Debug.Print("错误 " + gpMessages.GetMessage(i).ErrorCode.ToString() + " " + gpMessages.GetMessage(i).Description);
                                break;
                            case esriGPMessageType.esriGPMessageTypeWarning:
                                Debug.Print("警告 " + gpMessages.GetMessage(i).Description);
                                break;
                            default:
                                Debug.Print("信息 " + gpMessages.GetMessage(i).Description);
                                break;
                        }
                    }
                }
                return true;
        }
        /// <summary>
        /// 判断参数要素类字典是否包括意外值
        /// </summary>
        /// <param name="featureClasses"></param>
        /// <returns></returns>
        private static bool FeatureClassKeyIsRight(IDictionary<string, IFeatureClass> featureClasses)
        {
            List<string> tempArray=new List<string> (){"Stops","Barriers","PolylineBarriers","PolygonBarriers"};
            foreach(var value in featureClasses)
            {
                if (!tempArray.Contains(value.Key))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 设置解决器 暂时没有使用到 
        /// 有问题待修正
        /// </summary>
        /// <param name="pNAContext"></param>
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
