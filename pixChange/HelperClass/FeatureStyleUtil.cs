using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadRaskEvaltionSystem.HelperClass
{
    //要素样式修改
    public  class FeatureStyleUtil
    {
        //设置线要素的样式并刷新
        public static void SetFetureLineStyle(int red, int green, int blue, int width, IFeatureLayer featureLayer, AxMapControl mapControl)
        {
            IRgbColor pRgbColor = new RgbColor(); ;
            pRgbColor.Red = red;
            pRgbColor.Green = green;
            pRgbColor.Blue = blue;
            ILineSymbol lineSymbol = new SimpleLineSymbol();
            lineSymbol.Width = width;
            ISimpleRenderer simpleRender = new SimpleRendererClass();
            simpleRender.Symbol = lineSymbol as ISymbol;
            IGeoFeatureLayer geoLayer = featureLayer as IGeoFeatureLayer;
            geoLayer.Renderer = simpleRender as IFeatureRenderer;
            mapControl.ActiveView.ContentsChanged();
            mapControl.Refresh(esriViewDrawPhase.esriViewGeography, null, null);
        }
        /// <summary>
        /// 设置线要素的样式
        /// </summary>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
        /// <param name="width"></param>
        /// <param name="featureLayer"></param>
        public static void SetFetureLineStyle(int red, int green, int blue, int width, IFeatureLayer featureLayer)
        {
            IRgbColor pRgbColor = new RgbColor(); ;
            pRgbColor.Red = red;
            pRgbColor.Green = green;
            pRgbColor.Blue = blue;
            ILineSymbol lineSymbol = new SimpleLineSymbol();
            lineSymbol.Width = width;
            ISimpleRenderer simpleRender = new SimpleRendererClass();
            simpleRender.Symbol = lineSymbol as ISymbol;
            IGeoFeatureLayer geoLayer = featureLayer as IGeoFeatureLayer;
            geoLayer.Renderer = simpleRender as IFeatureRenderer;
        }
    }
}
