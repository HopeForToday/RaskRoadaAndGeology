using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using stdole;
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
        //清除所有图形元素
        public static void ClearElement(AxMapControl mapControl)
        {
            IMap map = mapControl.Map;
            IActiveView pActiveView = map as IActiveView;
            IGraphicsContainer pGraphicsContainer = pActiveView.GraphicsContainer;
            pGraphicsContainer.DeleteAllElements();
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }
        //清除元素
        public static void ClearElement(AxMapControl mapControl, List<IElement> elements)
        {
            IMap map = mapControl.Map;
            IActiveView pActiveView = map as IActiveView;
            IGraphicsContainer pGraphicsContainer = pActiveView.GraphicsContainer;
            foreach(var element in elements)
            {
                pGraphicsContainer.DeleteElement(element);
            }
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }
        //清除元素
        public static void ClearElement(AxMapControl mapControl, IElement element)
        {
            IMap map = mapControl.Map;
            IActiveView pActiveView = map as IActiveView;
            IGraphicsContainer pGraphicsContainer = pActiveView.GraphicsContainer;
            pGraphicsContainer.DeleteElement(element);
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }
        //画带图片的地图注记
        public static IElement DrawSymbolWithPicture(IPoint point, AxMapControl mapControl, string pictureUri)
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
           // pPicturemksb.CreateMarkerSymbolFromFile(esriIPictureType.esriIPictureJPG, pictureUri);
            Image image=Image.FromFile(pictureUri);
            IPictureDisp pictureDisp= IPictureConverter.ImageToIPictureDisp(image);
            pPicturemksb.Picture = pictureDisp;
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
            return pEle;
        }
        //画带文字的注记
        public static IElement DrawSymbolWithText(IPoint point, AxMapControl mapControl, string text)
        {
            #region 设置样式
            IFontDisp pFont = new StdFont()
             {
                 Name = "宋体",
                 Size = 5
             } as IFontDisp;

            ITextSymbol pTextSymbol = new TextSymbolClass()
           {
               Color = GetColor(255, 0, 0),
               Font = pFont,
               Size = 11
           };
            #endregion
            //设置带注记的元素
            ITextElement pTextElment = null;
            pTextElment = new TextElementClass()
            {
                Symbol = pTextSymbol,
                ScaleText = true,
                Text = text
            };
            IElement pEle = pTextElment as IElement;
            pEle.Geometry = point;
            //添加标注
            IMap map = mapControl.Map;
            IActiveView pActiveView = map as IActiveView;
            IGraphicsContainer pGraphicsContainer = pActiveView.GraphicsContainer;
            pGraphicsContainer.AddElement(pEle, 1);
            //刷新
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
            return pEle;
        }
        //构造RGB颜色     
        public static IRgbColor GetColor(int red, int green, int blue)
        {
            return new RgbColorClass()
                    {
                        Red = red,
                        Blue = green,
                        Green = blue,
                    };
        }
    }
}
