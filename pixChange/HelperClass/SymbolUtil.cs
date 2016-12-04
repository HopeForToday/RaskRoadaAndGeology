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
    /// <summary>
    /// 注记操作类 fhr 2016/12/4 修改
    /// </summary>
    class SymbolUtil
    {
        /// <summary>
        /// 清除所有图形元素
        /// </summary>
        /// <param name="mapControl"></param>
        public static void ClearElement(AxMapControl mapControl)
        {
            IMap map = mapControl.Map;
            IActiveView pActiveView = map as IActiveView;
            IGraphicsContainer pGraphicsContainer = pActiveView.GraphicsContainer;
            pGraphicsContainer.DeleteAllElements();
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }
        /// <summary>
        /// 清除元素
        /// </summary>
        /// <param name="mapControl"></param>
        /// <param name="elements"></param>
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
        /// <summary>
        /// 清除元素
        /// </summary>
        /// <param name="mapControl"></param>
        /// <param name="element"></param>

        public static void ClearElement(AxMapControl mapControl, IElement element)
        {
            try
            {
                IMap map = mapControl.Map;
                IActiveView pActiveView = map as IActiveView;
                IGraphicsContainer pGraphicsContainer = pActiveView.GraphicsContainer;
                pGraphicsContainer.DeleteElement(element);
                pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
            }
            catch (Exception)
            {
                
            }
        }
        /// <summary>
        /// 插入带图片的地图注记
        /// </summary>
        /// <param name="point"></param>
        /// <param name="mapControl"></param>
        /// <param name="pictureUri"></param>
        /// <returns></returns>

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
            pPicturemksb.CreateMarkerSymbolFromFile(esriIPictureType.esriIPictureJPG, pictureUri);
          //  Image image=Image.FromFile(pictureUri);
           // IPictureDisp pictureDisp= IPictureConverter.ImageToIPictureDisp(image);
          //  pPicturemksb.Picture = pictureDisp;
            IMarkerElement pMarkerEle = new MarkerElement() as IMarkerElement;
            //将注记添加到元素中
            pMarkerEle.Symbol = pPicturemksb as IMarkerSymbol;
            //添加元素到对应位置
            IElement pEle = (IElement)pMarkerEle;
            pEle.Geometry = point;
            //添加标注
            InsertElement(mapControl, pEle, 1);
            return pEle;
        }
        /// <summary>
        /// 插入带文字的注记
        /// </summary>
        /// <param name="point"></param>
        /// <param name="mapControl"></param>
        /// <param name="text"></param>
        /// <returns></returns>

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
            InsertElement(mapControl, pEle, 1);
            return pEle;
        }
        /// <summary>
        /// 构造RGB颜色    
        /// </summary>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
        /// <returns></returns>
        public static IRgbColor GetColor(int red, int green, int blue)
        {
            return new RgbColorClass()
                    {
                        Red = red,
                        Blue = green,
                        Green = blue,
                    };
        }
        /// <summary>
        /// 插入线标记 并且将线标记移动到后面
        /// </summary>
        /// <param name="mapControl"></param>
        /// <param name="pGeometry"></param>
        /// <returns></returns>
        public static IElement DrawLineSymbol(AxMapControl mapControl, IGeometry pGeometry)
        {
            IRgbColor pColor = GetColor(0, 255, 255);
            pColor.Transparency = 255;
            //产生一个线符号对象   
            ILineSymbol pOutline = new SimpleLineSymbolClass();
            pOutline.Width = 4;
            pOutline.Color = pColor;
            ILineElement pLineElement =new LineElementClass();
            pLineElement.Symbol= pOutline;
            IElement pElement = pLineElement as IElement;
            pElement.Geometry = pGeometry;
            //添加标注
            InsertElement(mapControl, pElement, 0);
            IGraphicsContainerSelect tmpGSelect = (IGraphicsContainerSelect)mapControl.Map;
            //将元素移动到后面
            tmpGSelect.SelectElement(pElement);
            IGraphicsContainer pGraphicsContainer = mapControl.Map as IGraphicsContainer;
            pGraphicsContainer.SendToBack(tmpGSelect.SelectedElements);
            tmpGSelect.UnselectAllElements();  
            return pElement;
        }
        /// <summary>
        /// 插入元素
        /// </summary>
        /// <param name="mapControl"></param>
        /// <param name="pElement"></param>
        /// <param name="zorder"></param>
        private static void InsertElement(AxMapControl mapControl, IElement pElement, int zorder)
        {
            IMap map = mapControl.Map;
            IActiveView pActiveView = map as IActiveView;
            IGraphicsContainer pGraphicsContainer = pActiveView.GraphicsContainer;
            pGraphicsContainer.AddElement(pElement, zorder);
            //刷新
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }
    }
}
