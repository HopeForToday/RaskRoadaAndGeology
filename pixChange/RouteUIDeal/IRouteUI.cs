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
        
        ILayer DealRoutenetLayer(AxMapControl mapControl);

        void ClearRouteAnalyst(AxMapControl mapControl, ref int insertFlag, List<IPoint> stopPoints, List<IPoint> barryPoints);

        bool FindTheShortRoute(AxMapControl mapControl, List<IPoint> stopPoints, List<IPoint> barryPoints, IFeatureLayer routeNetLayer);
    }
}
