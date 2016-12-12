using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadRaskEvaltionSystem.RouteUIDeal
{
    /// <summary>
    /// 路线分析UI帮助接口
    /// 2016/12/6 fhr
    /// </summary>
    public interface IRouteUI
    {
         bool FindTheShortRoute(AxMapControl mapControl, List<IPoint> stopPoints, List<IPoint> barryPoints, IFeatureLayer routeNetLayer, ref ILayer layer, ref List<IPoint> newStopPoints, ref List<IPoint> newBarryPoints);
        void showRouteShape(IFeatureLayer featureLayer, AxMapControl mapControl);
        ILayer DealRoutenetLayer(AxMapControl mapControl);
        void UpdateSymbol(AxMapControl mapControl, List<IPoint> newStopPoints, List<IPoint> newBarryPoints);
        void ClearRouteAnalyst(AxMapControl mapControl, ref int insertFlag, List<IPoint> stopPoints, List<IPoint> barryPoints);
    }
}
