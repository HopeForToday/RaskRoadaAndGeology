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
        void showRouteShape(IFeatureLayer featureLayer, AxMapControl mapControl);
        ILayer DealRoutenetLayer(AxMapControl mapControl);
        bool FindTheShortRoute(AxMapControl mapControl, IFeatureLayer routeNetLayer, ref ILayer layer);
        void UpdateSymbol(AxMapControl mapControl);
        void ClearRouteAnalyst(AxMapControl mapControl, ref int insertFlag);
        void ResetStopPointSymbols(AxMapControl mapControl);
        void ResetBarryPointSymbols(AxMapControl mapControl);
        void UndoStopPointSymbols(AxMapControl mapControl);
        void UndoBarryPointSymbols(AxMapControl mapControl);
        void InsertPoint(int insertFlag, AxMapControl mapControl, IPoint point);
        void InsertBarryPoint(AxMapControl mapControl, IPoint point);
        void InsertStopPoint(AxMapControl mapControl, IPoint point);
        List<IPoint> BarryPoints { get; }
        List<IPoint> StopPoints { get; }
        List<IElement> BarryElements { get; }
        List<IElement> StopElements { get; }
    }
}
