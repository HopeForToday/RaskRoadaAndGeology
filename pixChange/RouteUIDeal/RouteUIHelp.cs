using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.NetworkAnalyst;
using pixChange.HelperClass;
using RoadRaskEvaltionSystem.HelperClass;
using RoadRaskEvaltionSystem.RouteAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadRaskEvaltionSystem.RouteUIDeal
{
    /// <summary>
    /// 路线分析UI帮助类
    /// 2016/12/6 fhr
    /// </summary>
    class RouteUIHelp : IRouteUI
    {
        //路线分析类
        private ISimpleRouteDecide simpleRrouteDecide;
        public RouteUIHelp(ISimpleRouteDecide simpleRrouteDecide)
        {
            this.simpleRrouteDecide = simpleRrouteDecide;
        }
        //设置公路分级显示
        public void SetRoutesGrade(ILayer layer)
        {
            //四级公路符号
            ILineSymbol pOutline4 = new SimpleLineSymbolClass();
            //三级公路符号
            ILineSymbol pOutline3 = new SimpleLineSymbolClass();
            //等外公路符号
            ILineSymbol pOutlineEqual = new SimpleLineSymbolClass();
            //其它公路符号
            ILineSymbol pOutlineOther = new SimpleLineSymbolClass();
            pOutline4.Color = SymbolUtil.GetColor(255, 0, 0);
            pOutline4.Width = 2;
            pOutline3.Color = SymbolUtil.GetColor(0, 255, 0);
            pOutline3.Width = 1.5;
            pOutlineEqual.Color = SymbolUtil.GetColor(0, 255, 255);
            pOutlineEqual.Width = 1.2;
            pOutlineOther.Color = SymbolUtil.GetColor(0, 0, 255);
            pOutlineOther.Width = 1;
            IDictionary<string, ISymbol> symbolDic = new Dictionary<string, ISymbol>();
            symbolDic.Add("四级", pOutline4 as ISymbol);
            symbolDic.Add("三级", pOutline3 as ISymbol);
            symbolDic.Add("等外", pOutlineEqual as ISymbol);
            symbolDic.Add("其他", pOutlineOther as ISymbol);
            LayerManager.SetLayerGraderSymbol(layer, "RTEG", symbolDic);
        }

        /// <summary>
        /// 更新经过点和障碍点标志
        /// </summary>
        /// <param name="mapControl"></param>
        /// <param name="newStopPoints"></param>
        /// <param name="newBarryPoints"></param>
        void UpdateSymbol(AxMapControl mapControl, List<IPoint> newStopPoints, List<IPoint> newBarryPoints)
        {
            SymbolUtil.ClearElement(mapControl);
            foreach (var point in newStopPoints)
            {
                SymbolUtil.DrawSymbolWithPicture(point, mapControl, Common.StopImagePath);
            }
            foreach (var point in newBarryPoints)
            {
                SymbolUtil.DrawSymbolWithPicture(point, mapControl, Common.RouteBeakImggePath);
            }
        }
        /// <summary>
        /// 求出点到公路网的对应点
        /// </summary>
        /// <param name="featureLayer"></param>
        /// <param name="stopPoints"></param>
        /// <param name="barryPoints"></param>
        /// <param name="newStopPoints"></param>
        /// <param name="newBarryPoints"></param>
        /// <returns></returns>
        bool UpdatePointsToRouteCore(IFeatureLayer featureLayer, List<IPoint> stopPoints, List<IPoint> barryPoints, ref List<IPoint> newStopPoints, ref List<IPoint> newBarryPoints)
        {
            #region 注释
            /*
            newStopPoints = new List<IPoint>();
            newBarryPoints = new List<IPoint>();
            IEnumerable<IPoint> allPoints = stopPoints.Concat(newBarryPoints);
            List<IFeature> features;
            List<double> distances;
            List<int> disNums;
            List<ILine> lines = DistanceUtil.GetNearestLineInFeatureLayer(featureLayer, allPoints.ToList<IPoint>(), out features, out distances, out disNums);
            for (int i = 0; i < lines.Count; i++)
            {
                if (i >= stopPoints.Count)
                {
                    newBarryPoints.Add(lines[i].FromPoint);
                    continue;
                }
                newStopPoints.Add(lines[i].FromPoint);
            }
             */
            #endregion
            newStopPoints = new List<IPoint>();
            newBarryPoints = new List<IPoint>();
            foreach (var point in stopPoints)
            {
                double distance = 0;
                int disNum = 0;
                IFeature feature = null;
                IPoint rightPoint = DistanceUtil.GetNearestLineInFeatureLayer(featureLayer, point, ref feature, ref distance, ref disNum, 0.15);
                if (rightPoint == null)
                {
                    return false;
                }
                newStopPoints.Add(rightPoint);
            }
            foreach (var point in barryPoints)
            {
                double distance = 0;
                int disNum = 0;
                IFeature feature = null;
                IPoint rightPoint = DistanceUtil.GetNearestLineInFeatureLayer(featureLayer, point, ref feature, ref distance, ref disNum, 0.1);
                if (rightPoint == null)
                {
                    return false;
                }
                newBarryPoints.Add(rightPoint);
            }
            return true;
        }
        /// <summary>
        /// 对公路网图层进行处理
        /// 如果有则将其提取到第一个位置
        /// 如果没有直接加载
        /// </summary>
        /// <param name="mapControl"></param>
        public ILayer DealRoutenetLayer(AxMapControl mapControl)
        {
            IGroupLayer myGroupLayer = null;
            ILayer routeNetLayer = LayerUtil.QueryLayerInMap(mapControl, "公路网", ref myGroupLayer);
            //如果公路网的数据没有加载，则直接加载
            if (routeNetLayer == null)
            {
                routeNetLayer = ShapeSimpleHelper.OpenFile(Common.RouteNetFeaturePath);
                RouteLayerUtil.SetRouteLayerStyle(routeNetLayer);
                mapControl.AddLayer(routeNetLayer);
            }
            //否则 先移除 再加载 保证在第一个位置 也就是图层最上面
            else
            {
                if (myGroupLayer != null)
                {
                    myGroupLayer.Delete(routeNetLayer);
                }
                else
                {
                    mapControl.Map.DeleteLayer(routeNetLayer);
                }
                mapControl.AddLayer(routeNetLayer);
            }
            return routeNetLayer;
        }
        /// <summary>
        /// 清除路线分析相关数据
        /// </summary>
        /// <param name="mapControl"></param>
        /// <param name="insertFlag"></param>
        /// <param name="stopPoints"></param>
        /// <param name="barryPoints"></param>
        public void ClearRouteAnalyst(AxMapControl mapControl, ref int insertFlag, List<IPoint> stopPoints, List<IPoint> barryPoints)
        {
            //标志初始化
            insertFlag = 0;
            //清除所有图标
            SymbolUtil.ClearElement(mapControl);
            stopPoints.Clear();
            barryPoints.Clear();
            //清除网络分析图层
            for (int i = 0; i < mapControl.LayerCount; i++)
            {
                ILayer layer = mapControl.get_Layer(i);
                INetworkLayer networkLayer = layer as INetworkLayer;
                INALayer naLayer = layer as INALayer;
                if (networkLayer != null || naLayer != null)
                {
                    mapControl.DeleteLayer(i);
                }
            }
            //清除网络数据集
            ILayer datalayer = LayerUtil.QueryLayerInMap(mapControl, "网络数据集");
            if (datalayer != null)
            {
                mapControl.Map.DeleteLayer(datalayer);
            }
            IActiveView pActiveView = mapControl.ActiveView;
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
            mapControl.Refresh();
        }

        public bool FindTheShortRoute(AxMapControl mapControl, List<IPoint> stopPoints, List<IPoint> barryPoints, IFeatureLayer routeNetLayer)
        {
            List<IPoint> newStopPoints = null;
            List<IPoint> newBarryPoints = null;
            bool pointIsRight = false;
            TimeSpan ts1 = new TimeSpan(DateTime.Now.Ticks);
            pointIsRight = UpdatePointsToRouteCore(routeNetLayer as IFeatureLayer, stopPoints, barryPoints, ref newStopPoints, ref newBarryPoints);
            TimeSpan ts2 = new TimeSpan(DateTime.Now.Ticks);
            TimeSpan ts = ts2.Subtract(ts1).Duration(); //时间差的绝对值  
            Debug.Print("运行时间：" + ts.TotalSeconds.ToString());
            if (!pointIsRight)
            {
                throw new PointIsFarException("请检查点位是否太过远离图层");
            }
            UpdateSymbol(mapControl, newStopPoints, newBarryPoints);
            bool result = simpleRrouteDecide.QueryTheRoue(mapControl, routeNetLayer as IFeatureLayer, Common.NetWorkPath, "roads", "roads_ND", newStopPoints, newBarryPoints);
            return result;

        }

    }
}
