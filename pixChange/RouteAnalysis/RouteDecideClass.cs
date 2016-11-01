using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
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
        public IFeature QuerySingleFeatureByPoint(IPoint  point,IMap map,IFeatureLayer layer,double buffer_distance)
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
            ITopologicalOperator pTopOperator = point as ITopologicalOperator;
            IGeometry pGeometry = pTopOperator.Buffer(buffer_distance);
            IIdentify pIdentity = layer as IIdentify;
           IArray pArray= pIdentity.Identify(pGeometry);
          //  IArray pArray = pIdentity.Identify(point);
            IFeature pFeature = null;
            if(pArray!=null)
            {
                 pFeature = (pArray.get_Element(0) as IRowIdentifyObject).Row as IFeature;
            }
            return pFeature;
        }

        //查询绕行路线 0代表没有查询到
        public string QueryTheRoute(IPoint point,IFeatureLayer featureLayer,ref IPoint rightPoint)
        {
            //查询离所点击的点最近的元素
            IFeature feature = QueryTheRightFeatureByPoint(point, featureLayer, ref rightPoint);
            if (feature == null)
            {
                return null;
            }
            int index = feature.Fields.FindField("OBJECTID");
           int objectID=(int) feature.get_Value(index);
            return routeConfig.QueryGoodRouteIndex(objectID);
        }
        /// <summary>
        /// 查询配置文件中线要素集合中离点最近的元素
        /// </summary>
        /// <param name="point"></param>
        /// <param name="map"></param>
        /// <param name="featureLayer"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        private IFeature QueryTheRightFeatureByPoint(IPoint point, IFeatureLayer featureLayer, ref IPoint rightPoint)
        {
            List<IFeature> featuers = QueryAllFeatureInConfig(featureLayer);
            IFeature feature = null;
            double resultDistance = 9999999999;
            ILine resultLine = null;
            foreach (IFeature value in featuers)
            {
                double tempValue=0;
                int disNum=0;
                ILine line=DistanceUtil.GetNearestLine(value.Shape as  IPolyline, point, ref tempValue, ref disNum);
                if(tempValue<resultDistance)
                {
                    resultDistance = tempValue;
                    feature = value;
                    resultLine = line;
                }
            }
            rightPoint = new PointClass();
            rightPoint.X = resultLine.FromPoint.X;
            rightPoint.Y = resultLine.FromPoint.Y;
            return feature;
        }
        //查询所有配置文件中的要素
        public List<IFeature> QueryAllFeatureInConfig(IFeatureLayer layer)
        {
            List<IFeature> queryFeaturers = new List<IFeature>();
            IFeatureClass featureClass=layer.FeatureClass;
            Dictionary<int, string> queryObjectIDS=routeConfig.QueryIndexs;
            foreach(var v in queryObjectIDS)
            {
                IFeature feature = QuerySingleFeature(featureClass, v.Key);
                if(feature!=null)
                {
                    queryFeaturers.Add(feature);
                }
            }
            return queryFeaturers;
        }
        //根据OBJECTID查询单个要素
        public IFeature QuerySingleFeature(IFeatureClass featureClass, int objecID)
        {
            IQueryFilter2 queryFilter2 = new QueryFilterClass();
            queryFilter2.WhereClause = "OBJECTID = " + objecID.ToString();
            //Using a query filter to search a feature class:
            IFeatureCursor featureCursor = featureClass.Search(queryFilter2, false);
            return featureCursor.NextFeature();
        }
    }
}
