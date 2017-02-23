using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using pixChange;
using RoadRaskEvaltionSystem.HelperClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadRaskEvaltionSystem.QueryAndUIDeal
{
    /// <summary>
    /// 空间查询处理类
    /// 2016/12/11 fhr
    /// </summary>
    public class SpatialQueryUIClass : ISpatialQueryUI
    {
       /// <summary>
        ///  处理空间统计
       /// </summary>
       /// <param name="mapControl"></param>
       /// <param name="pGeometry"></param>
       /// <param name="layerName"></param>
        public void DealFeatureQuery(AxMapControl mapControl, IGeometry pGeometry, string queryStr, string layerName)
        {
            IGroupLayer gLayer = null;
            if (string.IsNullOrEmpty(layerName))
            {
                return;
            }
            int gIndex;
            int layerIndex;
            IFeatureLayer pFeatureLayer = LayerUtil.QueryLayerInMap(mapControl, layerName, ref gLayer,out layerIndex,out gIndex) as IFeatureLayer;
            if (pFeatureLayer == null)
            {
                return;
            }
            IQueryFilter queryFilter=null;
            IFeatureCursor pFeatureCursor = null;
            if (string.IsNullOrEmpty(queryStr))
            {
                 pFeatureCursor = FeatureDealUtil.QueryFeatureInLayer(pFeatureLayer, pGeometry, ref queryFilter);
            } 
            else
            {
                pFeatureCursor = FeatureDealUtil.QueryFeatureInLayer(pFeatureLayer,queryStr, ref queryFilter);
            }
            IList<IFeature> features = FlashFeatureShape(mapControl, pFeatureLayer, pFeatureCursor);
            ShowFeatureDetail(pFeatureLayer, features, queryFilter);
        }
        /// <summary>
        /// 高亮显示要素并且返回一个list
        /// </summary>
        /// <param name="mapControl"></param>
        /// <param name="pFeatureLayer"></param>
        /// <param name="pFeatureCursor"></param>
        private IList<IFeature> FlashFeatureShape(AxMapControl mapControl, IFeatureLayer pFeatureLayer, IFeatureCursor pFeatureCursor)
        {
            IList<IFeature> featurers = new List<IFeature>();
            ISymbol pSymbol = GetSymbolByShapeType(pFeatureLayer);
            IFeature pFeature = pFeatureCursor.NextFeature();
            while(pFeature!=null)
            {
                featurers.Add(pFeature);
          //      MapUtil.FlashFeature(mapControl, pFeature, pSymbol);
                mapControl.Map.SelectFeature(pFeatureLayer,pFeature);
                pFeature = pFeatureCursor.NextFeature();
            }
       //     mapControl.Refresh(esriViewDrawPhase.esriViewGeography, null, null);
            return featurers;
        }
        /// <summary>
        /// 设置要素高亮显示的符号
        /// </summary>
        /// <param name="pFeatureLayer"></param>
        /// <returns></returns>
        private ISymbol GetSymbolByShapeType(IFeatureLayer pFeatureLayer)
        {
            ISymbol symbol = null;
            switch (pFeatureLayer.FeatureClass.ShapeType)
            {
                case esriGeometryType.esriGeometryPoint:
                    symbol = new SimpleMarkerSymbolClass();
                    SetPointMarkerSymbol(symbol);
                    break;
                case esriGeometryType.esriGeometryPolyline:
                    symbol = new SimpleLineSymbolClass();
                    SetPolylineMarkerSymbol(symbol);
                    break;
                case esriGeometryType.esriGeometryPolygon:
                    symbol = new SimpleFillSymbolClass();
                    SetPolygonMarkerSymbol(symbol);
                    break;
                default:
                    break;
            }
            return symbol;
        }
        private void ShowFeatureDetail(IFeatureLayer pFeatureLayer, IList<IFeature> featurers,IQueryFilter pQueryFilter)
        {
            if (featurers.Count != 0)
            {
                new ProListView(pFeatureLayer,(List<IFeature>)featurers).ShowDialog();
            }
        }
        private void SetPointMarkerSymbol(ISymbol symbol)
        {
            SimpleMarkerSymbolClass simplePointSymbol = symbol as SimpleMarkerSymbolClass;
            simplePointSymbol.Style = esriSimpleMarkerStyle.esriSMSCircle;
            simplePointSymbol.Size = 5;
            simplePointSymbol.Color = SymbolUtil.GetColor(255, 0, 0);
        }

        private void SetPolylineMarkerSymbol(ISymbol symbol)
        {
            SimpleLineSymbolClass simpleLineSymbol = symbol as SimpleLineSymbolClass;
            simpleLineSymbol.Width = 2;
            simpleLineSymbol.Color = SymbolUtil.GetColor(255, 0, 99);
       
        }
        private void SetPolygonMarkerSymbol(ISymbol symbol)
        {
            SimpleFillSymbolClass simpleFillSymbol = symbol as SimpleFillSymbolClass;
            SimpleLineSymbolClass simpleLineSymbol = new SimpleLineSymbolClass();
            SetPolylineMarkerSymbol(simpleLineSymbol as ISymbol);
            simpleFillSymbol.Outline = simpleLineSymbol;
            simpleFillSymbol.Color = SymbolUtil.GetColor(222, 222, 222);
        }
    }
}
