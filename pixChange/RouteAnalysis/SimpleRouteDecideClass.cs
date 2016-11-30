using ESRI.ArcGIS.Carto;
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
    /// <summary>
    /// 最短路径实现类  2016/11/26 fhr
    /// </summary>
    class SimpleRouteDecideClass :ISimpleRouteDecide
    {
        /// <summary>
        /// 根据障碍点和经过点求解最短路径
        /// </summary>
        /// <param name="mapControl"></param>
        /// <param name="featureLayer"></param>
        /// <param name="dbPath"></param>
        /// <param name="featureSetName"></param>
        /// <param name="ndsName"></param>
        /// <param name="stopPoints"></param>
        /// <param name="barryPoints"></param>
        /// <returns></returns>
        public bool QueryTheRoue(ESRI.ArcGIS.Controls.AxMapControl mapControl, ESRI.ArcGIS.Carto.IFeatureLayer featureLayer, string dbPath, string featureSetName, string ndsName, List<ESRI.ArcGIS.Geometry.IPoint> stopPoints, List<ESRI.ArcGIS.Geometry.IPoint> barryPoints)
        {
           // List<IPoint>   newStopPoints;
          //  List<IPoint>  newBarryPoints;
        //    UpdatePointsToRouteCore(featureLayer, stopPoints, barryPoints, out newStopPoints, out newBarryPoints);
            //实例化站点和障碍点要素
            IFeatureClass stopFeatureClass =
                FeatureClassUtil.CreateMemorySimpleFeatureClass(esriGeometryType.esriGeometryPoint, mapControl.SpatialReference, "stops");
            IFeatureClass barriesFeatureClass =
                FeatureClassUtil.CreateMemorySimpleFeatureClass(esriGeometryType.esriGeometryPoint, mapControl.SpatialReference, "barries");
            //添加站点
            foreach (var value in stopPoints)
            {
                FeatureClassUtil.InsertSimpleFeature(value, stopFeatureClass);
            }
            //添加障碍
            foreach (var value in barryPoints)
            {
                FeatureClassUtil.InsertSimpleFeature(value, barriesFeatureClass);
            }
            //组装站点和障碍点要素
            IDictionary<string, DecorateRouteFeatureClass> featureClasses = new Dictionary<string, DecorateRouteFeatureClass>();
            featureClasses.Add("Stops", new DecorateRouteFeatureClass(0.2,stopFeatureClass));
            featureClasses.Add("Barriers", new DecorateRouteFeatureClass(0.2,barriesFeatureClass));
            //最短路径分析
            return NormalNetworkUtil.Short_Path(mapControl, dbPath, featureSetName, ndsName, featureClasses);
        }
    }
}
