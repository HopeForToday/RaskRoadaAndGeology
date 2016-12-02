using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using RoadRaskEvaltionSystem.HelperClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadRaskEvaltionSystem.RouteAnalysis
{
    class RouteDecideNew:IRouteDecide
    {

        public string QueryTheRoute(ESRI.ArcGIS.Geometry.IPoint point, ESRI.ArcGIS.Carto.IFeatureLayer featureLayerref, ref ESRI.ArcGIS.Geometry.IPoint rightPoint)
        {
            throw new NotImplementedException();
        }
        public bool QueryTheRoue(IPoint breakPoint, AxMapControl mapControl, IFeatureLayer featureLayer, string dbPath, string featureSetName, string ndsName, ref IPoint rightPoint)
        {
            IFeature feature = null;
            double distance = 0;
            int distNum = 0;
           rightPoint= DistanceUtil.GetNearestLineInFeatureLayer(featureLayer, breakPoint, ref feature, ref distance, ref distNum);
           if (rightPoint == null)
            {
                return false;
            }
            //获取线要素的点集合
            IPointCollection lineCollection = feature.Shape as IPointCollection;
            //实例化站点和障碍点要素
            IFeatureClass stopFeatureClass =
                FeatureClassUtil.CreateMemorySimpleFeatureClass(esriGeometryType.esriGeometryPoint, mapControl.SpatialReference, "stops");
            IFeatureClass barriesFeatureClass =
                FeatureClassUtil.CreateMemorySimpleFeatureClass(esriGeometryType.esriGeometryPoint, mapControl.SpatialReference, "barries");
            //添加站点
            FeatureClassUtil.InsertSimpleFeature(lineCollection.get_Point(0), stopFeatureClass);
            FeatureClassUtil.InsertSimpleFeature(lineCollection.get_Point(lineCollection.PointCount - 1), stopFeatureClass);
            //添加障碍
            FeatureClassUtil.InsertSimpleFeature(rightPoint, barriesFeatureClass);
            //组装站点和障碍点要素
            IDictionary<string, DecorateRouteFeatureClass> featureClasses = new Dictionary<string, DecorateRouteFeatureClass>();
            featureClasses.Add("Stops", new DecorateRouteFeatureClass(0.2, stopFeatureClass));
            featureClasses.Add("Barriers", new DecorateRouteFeatureClass(0.2, barriesFeatureClass));
            //最短路径分析
            return NormalNetworkUtil.Short_Path(mapControl, dbPath, featureSetName, ndsName, featureClasses);
        }
        public IPolyline QueryTheRoue2(IPoint breakPoint, IMap pMap, IFeatureLayer featureLayer, string dbPath, ref IPoint rightPoint)
        {
            
            IFeature feature = null;
            double distance = 0;
            int distNum = 0;
             rightPoint = DistanceUtil.GetNearestLineInFeatureLayer(featureLayer, breakPoint, ref feature, ref distance, ref distNum);
             if (rightPoint == null)
            {
                return null;
            }
            //获取线要素的点集合
            IPointCollection lineCollection = feature.Shape as IPointCollection;
            //将线要素的起点和终点加入路线点集合中
            IPointCollection routePointCollection = new MultipointClass();
            routePointCollection.AddPoint(lineCollection.get_Point(0));
            routePointCollection.AddPoint(lineCollection.get_Point(lineCollection.PointCount-1));
            //查询最短路径
            IPolyline polyline = UtilityNetWorkUtil.DistanceFun(pMap, dbPath, "roads",1, routePointCollection, "length", 50);
            return polyline;
             
        }
    }
}
