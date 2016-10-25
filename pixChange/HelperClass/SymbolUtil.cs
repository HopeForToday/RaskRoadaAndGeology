using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadRaskEvaltionSystem.HelperClass
{
    class SymbolUtil
    {
        //画带图片的地图注记
        public static void DrawSymbolWithPicture(IPoint point, AxMapControl mapControl, string pictureUri)
        {
            IMap map = mapControl.Map;
            //实例化注记
            IPictureElement pictureElement = new PictureElementClass()
            {
                Path = pictureUri,
                Geometry=point,
            };
            //添加地图注记
            IGraphicsContainer graphicsContainer = map as IGraphicsContainer;
            graphicsContainer.AddElement(pictureElement as IElement, 0);
            (map as IActiveView).PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, mapControl.Extent);
        }
    }
}
