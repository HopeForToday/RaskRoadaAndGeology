using ESRI.ArcGIS.Carto;
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
        string QueryTheRoute(IPoint point,IMap map,IFeatureLayer featureLayer);
    }
}
