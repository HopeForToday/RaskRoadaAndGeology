using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
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
            #region 注释
          //  IMap map = mapControl.Map;
          //  IEnvelope envelop = new EnvelopeClass();
          ////  point.QueryEnvelope(envelop);
          //   envelop.PutCoords(point.X - 50, point.Y - 50, point.X + 50, point.Y + 50);
          //  //实例化注记
          //  IPictureElement pictureElement = new PictureElementClass();
          //  pictureElement.MaintainAspectRatio = true;
          //  pictureElement.ImportPictureFromFile(pictureUri);
          //  IElement elment = pictureElement as IElement;
          //  elment.Geometry = envelop as IGeometry;
          //  //添加地图注记
          //  IGraphicsContainer graphicsContainer = map as IGraphicsContainer;
          //  graphicsContainer.AddElement(pictureElement as IElement, 0);
          //  (map as IActiveView).PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, mapControl.Extent);
            #endregion
            IMap map = mapControl.Map;
            //实例化图片注记
            IPictureMarkerSymbol pPicturemksb = new PictureMarkerSymbolClass();
            pPicturemksb.Size = 20;
            pPicturemksb.CreateMarkerSymbolFromFile(esriIPictureType.esriIPictureBitmap, pictureUri);
            IMarkerElement pMarkerEle = new MarkerElement() as IMarkerElement;
          //将注记添加到元素中
            pMarkerEle.Symbol = pPicturemksb as IMarkerSymbol;
            //添加元素到对应位置
            IElement pEle = (IElement)pMarkerEle;
            pEle.Geometry = point;
            IActiveView pActiveView = map as IActiveView;
            IGraphicsContainer pGraphicsContainer = pActiveView.GraphicsContainer;
            pGraphicsContainer.AddElement(pEle, 0);
            //刷新
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }
    }
}
