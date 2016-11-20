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
    public interface IRouteDecide
    {
        string QueryTheRoute(IPoint point,IFeatureLayer featureLayerref,ref IPoint rightPoint);
        bool QueryTheRoue(IPoint breakPoint, AxMapControl mapControl, IFeatureLayer featureLayer, string dbPath, string featureSetName, string ndsName, ref IPoint rightPoint);
    }
}
