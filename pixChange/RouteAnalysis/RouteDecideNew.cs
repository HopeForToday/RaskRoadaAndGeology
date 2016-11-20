﻿using ESRI.ArcGIS.Carto;
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
            int distNum = 0;
            ILine breakLine = DistanceUtil.GetNearestLineInFeature(featureLayer, breakPoint, ref feature, ref distNum);
            if (breakLine == null)
            {
                return false;
            }
            rightPoint = breakLine.FromPoint;
            //获取线要素的点集合
            IPointCollection lineCollection = feature.Shape as IPointCollection;
            //实例化站点图层和障碍点图层
            IFeatureClass stopFeatureClass =
                FeatureClassUtil.CreateMemorySimpleFeatureClass(esriGeometryType.esriGeometryPoint, mapControl.SpatialReference, "stops");
            IFeatureClass barriesFeatureClass =
                FeatureClassUtil.CreateMemorySimpleFeatureClass(esriGeometryType.esriGeometryPoint, mapControl.SpatialReference, "barries");
            //添加站点
            FeatureClassUtil.InsertSimpleFeature(lineCollection.get_Point(0), stopFeatureClass);
            FeatureClassUtil.InsertSimpleFeature(lineCollection.get_Point(lineCollection.PointCount - 1), stopFeatureClass);
            //添加障碍
            FeatureClassUtil.InsertSimpleFeature(rightPoint, barriesFeatureClass);
            IDictionary<string, IFeatureClass> featureClasses = new Dictionary<string, IFeatureClass>();
            featureClasses.Add("Stops", stopFeatureClass);
            featureClasses.Add("Barriers", barriesFeatureClass);
            //最短路径分析
            return NormalNetworkUtil.Short_Path(mapControl, dbPath, featureSetName, ndsName, featureClasses, 0.2);
        }
        public IPolyline QueryTheRoue2(IPoint breakPoint, IMap pMap, IFeatureLayer featureLayer, string dbPath, ref IPoint rightPoint)
        {
            
            IFeature feature = null;
            int distNum = 0;
            ILine breakLine = DistanceUtil.GetNearestLineInFeature(featureLayer, breakPoint, ref feature, ref distNum);
            if(breakLine==null)
            {
                return null;
            }
            rightPoint = breakLine.FromPoint;
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
