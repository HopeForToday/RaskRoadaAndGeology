using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadRaskEvaltionSystem.RouteAnalysis
{
    class RouteDecideClass:IRouteDecide
    {
        IRouteConfig routeConfig = null;
        public RouteDecideClass(IRouteConfig config)
        {
            this.routeConfig = config;
        }
        //根据点查询缓冲区以内的要素
        private IFeature QuerySingleFeatureByPoint(IPoint  point,IMap map,IFeatureLayer layer,double buffer_distance)
        {
          //为了安全在此判断是否map中是否已经包括layer
            bool isContain=false;
            for(int i=0;i<map.LayerCount;i++)
            {
                if(map.get_Layer(i)==layer)
                {
                    isContain=true;
                    break;
                }
            }
            if(!isContain) 
            {
                throw new Exception("Map中不包括该图层");
            }
         //   map.SelectByShape()
            IFeatureClass pFeatureClass = layer.FeatureClass;
            ITopologicalOperator pTopOperator = point as ITopologicalOperator;
            IGeometry pGeometry = pTopOperator.Buffer(buffer_distance);
            //进行选取
            map.SelectByShape(pGeometry,null,true);
            map.ClearSelection();
            //空间过滤运算
            ISpatialFilter pSpatialFilter = new SpatialFilterClass();
            pSpatialFilter.Geometry = pGeometry;
            //设置选取点与待选取要素之间的空间关系
            switch (pFeatureClass.ShapeType)
            {
                case esriGeometryType.esriGeometryPoint:
                    pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;
                    break;
                case esriGeometryType.esriGeometryPolyline:
                    pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelCrosses;
                    break;
                case esriGeometryType.esriGeometryPolygon:
                    pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                    break;

            }
            pSpatialFilter.GeometryField = pFeatureClass.ShapeFieldName;
            //利用指针进行遍历 不过我们查询单个元素 只需要对第一个进行返回
            IFeatureCursor pFeatureCursor;
            pFeatureCursor = pFeatureClass.Search(pSpatialFilter, false);
            return pFeatureCursor.NextFeature();
        }
      //查询绕行路线 0代表没有查询到
        public int QueryTheRoute(IPoint point,IMap map,IFeatureLayer featureLayer)
        {
            //查询所点击的要素
            IFeature feature = QuerySingleFeatureByPoint(point, map, featureLayer, 2);
            if (feature == null)
            {
                return 0;
            }
            int objectID = feature.Fields.FindField("ObjectID");
            return routeConfig.QueryGoodRouteIndex(objectID);
        }
    }
}
