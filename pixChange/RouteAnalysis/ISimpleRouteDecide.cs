using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadRaskEvaltionSystem.RouteAnalysis
{
    /// <summary>
    /// 简单最短路径接口  2016/11/26 fhr
    /// </summary>
    interface ISimpleRouteDecide
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
        /// <param name="routeLayer"></param>
        /// <returns></returns>
        bool QueryTheRoue(ESRI.ArcGIS.Controls.AxMapControl mapControl, ESRI.ArcGIS.Carto.IFeatureLayer featureLayer, string dbPath, string featureSetName, string ndsName, List<ESRI.ArcGIS.Geometry.IPoint> stopPoints, List<ESRI.ArcGIS.Geometry.IPoint> barryPoints, ref ILayer routeLayer);
    }
}