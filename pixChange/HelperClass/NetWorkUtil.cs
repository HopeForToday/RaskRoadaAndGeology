using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.NetworkAnalysis;
using ESRI.ArcGIS.DataSourcesGDB;
namespace RoadRaskEvaltionSystem.HelperClass
{
    /// <summary>
    /// 网络分析操作帮助类 FHR 2016/11/14 23:30
    /// </summary>
   public class NetWorkUtil
    {
       /// <summary>
        /// 获取网络数据集
       /// </summary>
       /// <param name="dbFileName">文件路径</param>
       /// <returns></returns>
       public static INetworkCollection OpenNetworkCollection(string dbFileName)
       {
           string workPath=System.IO.Path.GetDirectoryName(dbFileName);
           string dbName = System.IO.Path.GetFileNameWithoutExtension(dbFileName);
           IWorkspaceFactory pWF = new FileGDBWorkspaceFactoryClass();
           IWorkspace pWorkspace = pWF.OpenFromFile(workPath,0);
           IFeatureWorkspace pFW=pWorkspace as IFeatureWorkspace;
           IFeatureDataset pFeatureDataset = pFW.OpenFeatureDataset(dbName);
           INetworkCollection pNetColl = pFeatureDataset as INetworkCollection;
           return pNetColl;
       }
       /// <summary>
       /// 获取网络数据
       /// </summary>
       /// <param name="netCollection"></param>
       /// <param name="index"></param>
       /// <returns></returns>
       public static IGeometricNetwork GetGeometricNetWork(INetworkCollection netCollection,int index)
       {
           if(index<0||index>=netCollection.GeometricNetworkCount)
           {
               throw new Exception("索引异常");
           }
           IGeometricNetwork pGeoNet = netCollection.get_GeometricNetwork(index);
           return pGeoNet;
       }
       /// <summary>
       /// 获取网络数据
       /// </summary>
       /// <param name="netCollection"></param>
       /// <param name="index"></param>
       /// <returns></returns>
       public static IGeometricNetwork GetGeometricNetWork(string dbFileName, int index)
       {
           INetworkCollection netCollection = OpenNetworkCollection(dbFileName);
           if (index < 0 || index >= netCollection.GeometricNetworkCount)
           {
               throw new Exception("索引异常");
           }
           IGeometricNetwork pGeoNet = netCollection.get_GeometricNetwork(index);
           return pGeoNet;
       }
       public static void NetWorkFun(IGeometricNetwork pGeometricNet,IPointCollection pCollection,string weightName,double pDisc)
       {
           //几何网络分析接口
           ITraceFlowSolverGEN pTraceFlowGen = new TraceFlowSolverClass() as ITraceFlowSolverGEN;
           INetSolver pNetSolver = pTraceFlowGen as INetSolver;
           //获取网络
           INetwork pNetwork = pGeometricNet.Network;
           //设置该网络为几何分析处理网络
           pNetSolver.SourceNetwork = pNetwork;
           //网络元素
           INetElements pNetElements = pNetwork as INetElements;
           //根据输入点建立旗帜数组（也就是最短路径所要经过的节点）
           IJunctionFlag[] pJunctionFlags = GetJunctionFlags(pGeometricNet, pCollection, pDisc);
           //将旗帜数组添加到处理类中
           pTraceFlowGen.PutJunctionOrigins(ref pJunctionFlags);
           //设置边线的权重 可以考虑使用点权重
           SetWeight(weightName, pTraceFlowGen, pNetwork);
           //获取边线和交汇点的集合
           IEnumNetEID junctionEIDs;
           IEnumNetEID netEIDS;
           object []pRec=new object[100];
           pTraceFlowGen.FindPath(esriFlowMethod.esriFMConnected, esriShortestPathObjFn.esriSPObjFnMinSum, out IEnumNetEID,out netEIDS, pCollection.PointCount - 1, ref pRec);
          //获取最短路径
          // IGeometryCollection pGeometryCollection=
       }
       /// <summary>
       /// 根据输入点建立旗帜数组
       /// </summary>
       /// <param name="pGeometricNet"></param>
       /// <param name="pCollection"></param>
       /// <param name="pDisc"></param>
       /// <param name="pNetElements"></param>
       /// <returns></returns>
       private static IJunctionFlag[] GetJunctionFlags(IGeometricNetwork pGeometricNet, IPointCollection pCollection, double pDisc)
       {
           INetElements pNetElements = pGeometricNet.Network as INetElements;
           int pCount = pCollection.PointCount;
           IJunctionFlag[] pJunctionFlags = new JunctionFlagClass[pCount];
           IPointToEID pPointToEID = new PointToEIDClass();
           pPointToEID.GeometricNetwork = pGeometricNet;
           //从输入点找最近节点的距离阈值
           pPointToEID.SnapTolerance = pDisc;
           for (int i = 0; i < pCount; i++)
           {
               INetFlag pNetFlag = new JunctionFlagClass();
               IPoint pEdgePoint = pCollection.get_Point(i);
               #region 查找输入点最近的节点的ID
               int nearestJunctionID;
               IPoint locationPoint;
               int userClassID;
               int userID;
               int userSubID;
               pPointToEID.GetNearestJunction(pEdgePoint, out nearestJunctionID, out locationPoint);
               pNetElements.QueryIDs(nearestJunctionID, esriElementType.esriETJunction, out userClassID, out userID, out userSubID);
               #endregion
               //设置网络旗帜的节点的ID
               pNetFlag.UserClassID = userClassID;
               pNetFlag.UserID = userID;
               pNetFlag.UserSubID = userSubID;
               //节点旗帜
               IJunctionFlag pJuncF = pNetFlag as IJunctionFlag;
               //添加
               pJunctionFlags[i] = pJuncF;
           }
           return pJunctionFlags;
       }
       /// <summary>
       /// 设置边线权重
       /// </summary>
       /// <param name="weightName"></param>
       /// <param name="pTraceFlowGen"></param>
       /// <param name="pNetwork"></param>
       private static void SetWeight(string weightName, ITraceFlowSolverGEN pTraceFlowGen, INetwork pNetwork)
       {
           INetSchema pNetSchema = pNetwork as INetSchema;
           //根据名字获取权重
           INetWeight pNetWeight = pNetSchema.get_WeightByName(weightName);
           INetSolverWeightsGEN pNetSolverWeightGen = pTraceFlowGen as INetSolverWeightsGEN;
           //开始边线权重
           pNetSolverWeightGen.FromToEdgeWeight = pNetWeight;
           //终止边线权重
           pNetSolverWeightGen.ToFromEdgeWeight = pNetWeight;
       }
       /// <summary>
       /// 设置障碍点
       /// </summary>
       /// <param name="pNetSolver"></param>
       /// <param name="breakPoints"></param>
       /// <param name="pGeometricNet"></param>
       /// <param name="pDisc"></param>
       private void SetBarries(INetSolver pNetSolver,List<IPoint> breakPoints,IGeometricNetwork pGeometricNet,double pDisc)
       {
           INetwork pNetwork = pGeometricNet.Network;
           INetElements pNetElements = pNetwork as INetElements;
           IPointToEID pPointToEID = new PointToEIDClass();
           pPointToEID.GeometricNetwork = pGeometricNet;
           pPointToEID.SnapTolerance = pDisc;
           ISelectionSetBarriers barriers = new SelectionSetBarriersClass();
           foreach(var point in breakPoints)
           {
               //寻找障碍点最近的线要素，从而添加爆管线
               int nearEdgeID;
               IPoint outIpoint;
               double precent;
               pPointToEID.GetNearestEdge(point,out nearEdgeID,out outIpoint,out precent);
               int userClassID;
               int userID;
               int userSubID;
               //查询相关ID
                pNetElements.QueryIDs(nearEdgeID, esriElementType.esriETJunction, out userClassID, out userID, out userSubID);
              //添加障碍点
                barriers.Add(userClassID, userID);
           }
           pNetSolver.SelectionSetBarriers = barriers;
       }
    }
}
