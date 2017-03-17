using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.NetworkAnalyst;
using log4net;
using pixChange.HelperClass;
using RoadRaskEvaltionSystem.HelperClass;
using RoadRaskEvaltionSystem.RouteAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RoadRaskEvaltionSystem.RouteUIDeal
{
    /// <summary>
    /// 路线分析UI帮助类
    /// 2016/12/6 fhr
    /// </summary>
    class RouteUIHelp : IRouteUI
    {
        private static readonly log4net.ILog logger = LogManager.GetLogger(typeof(RouteUIHelp));
        private List<IPoint> barryPoints = new List<IPoint>();
        private List<IPoint> stopPoints = new List<IPoint>();
        private List<IElement> barryElements = new List<IElement>();
        private List<IElement> stopElements = new List<IElement>();
        //路线分析类
        private ISimpleRouteDecide simpleRrouteDecide;
        public RouteUIHelp(ISimpleRouteDecide simpleRrouteDecide)
        {
            this.simpleRrouteDecide = simpleRrouteDecide;
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
        bool UpdatePointsToRouteCore(IFeatureLayer featureLayer)
        {
           List<IPoint> newStopPoints = new List<IPoint>();
           List<IPoint> newBarryPoints = new List<IPoint>();
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
            this.stopPoints = newStopPoints;
            this.barryPoints = newBarryPoints;
            return true;
        }
        /// <summary>
        /// 对公路网图层进行处理 线程安全
        /// 如果有则将其提取到第一个位置
        /// 如果没有直接加载
        /// </summary>
        /// <param name="mapControl"></param>
        public ILayer DealRoutenetLayer(AxMapControl mapControl)
        {
            IGroupLayer myGroupLayer = null;
            int gIndex;
            int layerIndex;
            ILayer routeNetLayer = LayerUtil.QueryLayerInMap(mapControl, "公路网", ref myGroupLayer,out layerIndex,out gIndex);
            //如果公路网的数据没有加载，则直接加载
            if (routeNetLayer == null)
            {
                routeNetLayer = ShapeSimpleHelper.OpenFile(Common.RouteNetFeaturePath);
                RouteLayerUtil.SetRouteLayerStyle(routeNetLayer);
                mapControl.AddLayer(routeNetLayer);
            }
            //否则 保证在第一个位置 也就是图层最上面
            if(gIndex<1&&layerIndex==0)
            {
                routeNetLayer.Visible=true;
            }
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
        /// 路线结果显示方法 线程安全
        /// </summary>
        /// <param name="featureLayer"></param>
        /// <param name="mapControl"></param>
        public void showRouteShape(IFeatureLayer featureLayer, AxMapControl mapControl)
        {
            IQueryFilter pQueryFilter = new QueryFilter();
            pQueryFilter.WhereClause = "";
            IFeatureCursor pCursor = featureLayer.FeatureClass.Search(pQueryFilter, false);
            mapControl.Map.AreaOfInterest = featureLayer.AreaOfInterest;
            //实际上路线分析结果只有一个要素 只是为了保险
            IFeature pFeature = pCursor.NextFeature();
            while (pFeature != null)
            {
                #region 更新地图显示范围为要素范围
                    ShowRouteMethod(mapControl, pFeature);
                #endregion
                SymbolUtil.DrawLineSymbol(mapControl, pFeature.Shape as IGeometry);
                pFeature = pCursor.NextFeature();
            }
        }

        private static void ShowRouteMethod(AxMapControl mapControl, IFeature pFeature)
        {
            IEnvelope pEnvelope = pFeature.Extent;
            double width = pEnvelope.Width;
            double height = pEnvelope.Height;
            pEnvelope.XMax += width * 1.5;
            pEnvelope.XMin -= width * 1.5;
            pEnvelope.YMax += height * 1.5;
            pEnvelope.YMin -= height * 1.5;
            mapControl.Extent = pEnvelope;
            SymbolUtil.DrawLineSymbol(mapControl, pFeature.Shape as IGeometry);
        }
        public bool FindTheShortRoute(AxMapControl mapControl, IFeatureLayer routeNetLayer, ref ILayer layer)
        {
            bool pointIsRight = false;
            TimeSpan ts1 = new TimeSpan(DateTime.Now.Ticks);
            pointIsRight = UpdatePointsToRouteCore(routeNetLayer as IFeatureLayer);
            TimeSpan ts2 = new TimeSpan(DateTime.Now.Ticks);
            TimeSpan ts = ts2.Subtract(ts1).Duration(); //时间差的绝对值  
            Debug.Print("运行时间：" + ts.TotalSeconds.ToString());
            if (!pointIsRight)
            {
                logger.Error("点位太过远离图层");
                throw new PointIsFarException("请检查点位是否太过远离图层");
            }
            return simpleRrouteDecide.QueryTheRoue(mapControl, routeNetLayer as IFeatureLayer, Common.NetWorkPath, "roads", "roads_ND", this.stopPoints, this.barryPoints, ref layer);
        }

        public void UpdateSymbol(AxMapControl mapControl)
        {
            SymbolUtil.ClearElement(mapControl);
            this.stopPoints.ForEach(point=>SymbolUtil.DrawSymbolWithPicture(point, mapControl, Common.StopImagePath));
            this.barryPoints.ForEach(point => SymbolUtil.DrawSymbolWithPicture(point, mapControl, Common.RouteBeakImggePath));
        }

        public void ClearRouteAnalyst(AxMapControl mapControl)
        {
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
            this.stopPoints.Clear();
            this.barryPoints.Clear();
        }


        public void ResetStopPointSymbols(AxMapControl mapControl)
        {
            this.stopPoints.Clear();
            this.stopElements.ForEach(element => SymbolUtil.ClearElement(mapControl, element));
            this.stopElements.Clear();
        }

        public void ResetBarryPointSymbols(AxMapControl mapControl)
        {
            this.barryPoints.Clear();
            this.barryElements.ForEach(element => SymbolUtil.ClearElement(mapControl, element));
            this.barryElements.Clear();
        }

        public void UndoStopPointSymbols(AxMapControl mapControl)
        {
            if (this.stopPoints.Count > 0)
            {
                this.stopPoints.RemoveAt(this.stopPoints.Count - 1);
                SymbolUtil.ClearElement(mapControl, this.stopElements[this.stopElements.Count - 1]);
                this.stopElements.RemoveAt(this.stopElements.Count - 1);
            }
        }

        public void UndoBarryPointSymbols(AxMapControl mapControl)
        {
            if (this.barryPoints.Count > 0)
            {
                this.barryPoints.RemoveAt(this.barryPoints.Count - 1);
                SymbolUtil.ClearElement(mapControl, this.barryElements[this.barryElements.Count - 1]);
                this.barryElements.RemoveAt(this.barryElements.Count - 1);
            }
        }
        public void InsertBarryPoint(AxMapControl mapControl, IPoint point)
        {
            this.barryElements.Add(SymbolUtil.DrawSymbolWithPicture(point, mapControl, Common.RouteBeakImggePath));
            this.barryPoints.Add(point);
        }
        public void InsertStopPoint(AxMapControl mapControl, IPoint point)
        {
            this.stopElements.Add(SymbolUtil.DrawSymbolWithPicture(point, mapControl, Common.StopImagePath));
            this.stopPoints.Add(point);
        }
        public void RemoveStopPoint(AxMapControl mapControl, IPoint point)
        {
            if (this.stopPoints.Count > 0)
            {
                int index = JudgePointIsInCircle(this.stopPoints, point);
                if (index != -1)
                {
                    this.stopPoints.RemoveAt(index);
                    SymbolUtil.ClearElement(mapControl, this.stopElements[index]);
                    this.stopElements.RemoveAt(index);
                }
            }
        }
        public void RemoveBarryPoint(AxMapControl mapControl, IPoint point)
        {
            if (this.BarryPoints.Count > 0)
            {
                int index = JudgePointIsInCircle(this.barryPoints, point);
                if (index != -1)
                {
                    this.barryPoints.RemoveAt(index);
                    SymbolUtil.ClearElement(mapControl, this.barryElements[index]);
                    this.barryElements.RemoveAt(index);
                }
            }
        }
        /// <summary>
        /// 判断点是否在点集合中任意一点的半径之内
        /// 返回求得点索引 -1则没有取到
        /// </summary>
        /// <param name="sourcePoints"></param>
        /// <param name="targetPoint"></param>
        /// <returns></returns>
        private int  JudgePointIsInCircle(List<IPoint> sourcePoints,IPoint targetPoint)
        {
            int index=-1;
            for (int i = 0; i < sourcePoints.Count; i++)
            {
                IPoint sourcePoint=sourcePoints[i];
                if (isInPointCircle(sourcePoint, targetPoint, 0.02))
                {
                    index = i;
                    break;
                }
            }
            return index;
        }
        /// <summary>
        /// 判断两点是否在指定距离之内
        /// </summary>
        /// <param name="sourcePoint"></param>
        /// <param name="targetPoint"></param>
        /// <param name="radii"></param>
        /// <returns></returns>
        private Boolean isInPointCircle(IPoint sourcePoint, IPoint targetPoint,double radii)
        {
            double xStance = Math.Abs(sourcePoint.X - targetPoint.X);
            double yStance = Math.Abs(sourcePoint.Y - targetPoint.Y);
            return Math.Pow(radii, 2) > (Math.Pow(xStance, 2) + Math.Pow(yStance, 2)) ? true : false;
        }
        #region getters
        public List<IPoint> BarryPoints
        {
            get { return this.barryPoints; }
        }

        public List<IPoint> StopPoints
        {
            get { return this.stopPoints; }
        }


        public List<IElement> BarryElements
        {
            get { return this.barryElements; }
        }

        public List<IElement> StopElements
        {
            get { return this.stopElements; }
        }
        #endregion
    }
}
