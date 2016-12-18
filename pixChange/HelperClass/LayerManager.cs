using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using RoadRaskEvaltionSystem.HelperClass;
using ESRI.ArcGIS.Display;

namespace pixChange.HelperClass
{
    public class LayerManager
    {
        /// <summary>
        /// 设置要素图片显示样式
        /// </summary>
        /// <param name="featureLayer"></param>
        /// <param name="pictureName"></param>
        /// <param name="size"></param>
        public static void SetFeaturePictureSymbol(IFeatureLayer featureLayer, string pictureName, int size)
        {
            IGeoFeatureLayer geoLayer = featureLayer as IGeoFeatureLayer;
            IPictureMarkerSymbol pPicturemksb = new PictureMarkerSymbolClass();
            pPicturemksb.Size = 20;
            pPicturemksb.CreateMarkerSymbolFromFile(esriIPictureType.esriIPictureJPG, pictureName);
            ISimpleRenderer simpleRender = geoLayer.Renderer as ISimpleRenderer;
            if (simpleRender == null)
            {
                simpleRender = new SimpleRendererClass();
            }
            simpleRender.Symbol = pPicturemksb as ISymbol;
            geoLayer.Renderer = simpleRender as IFeatureRenderer;
        }
        /// <summary>
        /// 设置图层根据某字段的值进行显示
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="filedName"></param>
        /// <param name="symbolDic"></param>
        public static void  SetLayerGraderSymbol(ILayer layer,string filedName,IDictionary<string,ISymbol> symbolDic)
        {
            IFeatureLayer featureLayer = layer as IFeatureLayer;
            IUniqueValueRenderer pRender = new UniqueValueRendererClass();
            pRender.FieldCount = 1;
            pRender.set_Field(0, filedName);
            foreach(var dicValue in symbolDic)
            {
             //   pRender.AddValue(dicValue.Key, dicValue.Key, dicValue.Value);
                pRender.AddValue(dicValue.Key, "", dicValue.Value);
            }
            IGeoFeatureLayer geoLayer = layer as IGeoFeatureLayer;
            geoLayer.Renderer = pRender as IFeatureRenderer;
        }
        /// <summary>
        /// 设置图层文字注记显示
        /// </summary>
        /// <param name="pFeaturelayer"></param>
        /// <param name="sLableField"></param>
        /// <param name="pRGB"></param>
        /// <param name="size"></param>
        /// <param name="angleField">旋转角度</param>
        public static void EnableFeatureLayerLabel(IFeatureLayer pFeaturelayer, string sLableField, IRgbColor pRGB, int size, string angleField)
        {
            if (pFeaturelayer == null)
            {
                return;
            }
            IGeoFeatureLayer pGeoFeaturelayer = (IGeoFeatureLayer)pFeaturelayer;
            IAnnotateLayerPropertiesCollection pAnnoLayerPropsCollection;
            pAnnoLayerPropsCollection = pGeoFeaturelayer.AnnotationProperties;
            pAnnoLayerPropsCollection.Clear();
            //stdole.IFontDisp pFont; //字体  
            //pFont.Bold = true;
            //pFont.Name = "新宋体";  
            //pFont.Size = 9;  
            //未指定字体颜色则默认为黑色  
            ITextSymbol pTextSymbol;
            if (pRGB == null)
            {
                pRGB = new RgbColorClass();
                pRGB.Red = 0;
                pRGB.Green = 0;
                pRGB.Blue = 0;
            }
            pTextSymbol = new TextSymbolClass();
            pTextSymbol.Color = (IColor)pRGB;
            pTextSymbol.Size = size; //标注大小  
            pTextSymbol.Font.Bold=true;//加粗
            IBasicOverposterLayerProperties4 pBasicOverposterlayerProps4 = new BasicOverposterLayerPropertiesClass();
            switch (pFeaturelayer.FeatureClass.ShapeType)//判断图层类型  
            {
                case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon:
                    pBasicOverposterlayerProps4.FeatureType = esriBasicOverposterFeatureType.esriOverposterPolygon;
                    break;
                case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint:
                    pBasicOverposterlayerProps4.FeatureType = esriBasicOverposterFeatureType.esriOverposterPoint;
                    break;
                case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline:
                    pBasicOverposterlayerProps4.FeatureType = esriBasicOverposterFeatureType.esriOverposterPolyline;
                    break;
            }
            pBasicOverposterlayerProps4.PointPlacementMethod = esriOverposterPointPlacementMethod.esriRotationField;
            pBasicOverposterlayerProps4.RotationField = angleField;
            ILabelEngineLayerProperties pLabelEnginelayerProps = new LabelEngineLayerPropertiesClass();
            pLabelEnginelayerProps.Expression = "[" + sLableField + "]";
            pLabelEnginelayerProps.Symbol = pTextSymbol;
            pLabelEnginelayerProps.BasicOverposterLayerProperties = pBasicOverposterlayerProps4 as IBasicOverposterLayerProperties;
            pAnnoLayerPropsCollection.Add((IAnnotateLayerProperties)pLabelEnginelayerProps);
            pGeoFeaturelayer.DisplayAnnotation = true;//很重要，必须设置   
            //axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewBackground, null, null); }  
        }  
        /// <summary>
        /// 设置图层透明度
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="tran"></param>
        public static void SetLayerTranspan(ILayer layer, short tran)
        {
            ILayerEffects layerEffects = layer as ILayerEffects;
            layerEffects.Transparency = tran;
        }
        /// <summary>
        /// 设置图层颜色
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="red"></param>
        /// <param name="blue"></param>
        /// <param name="green"></param>
        public static void SetLayerColor(ILayer layer,int red,int blue,int green)
        {
            IGeoFeatureLayer geoLayer=layer as IGeoFeatureLayer;
            IFeatureRenderer featureRender = geoLayer.Renderer;
            ILineSymbol pOutline = new SimpleLineSymbolClass();
            pOutline.Color=SymbolUtil.GetColor(red,green,blue);
           // ISimpleRenderer pSimpleRenderer = new SimpleRendererClass()
            ISimpleRenderer pSimpleRenderer = featureRender as ISimpleRenderer;
            pSimpleRenderer.Label = "NAME";
            pSimpleRenderer.Description = "ss";
            pSimpleRenderer.Symbol =(ISymbol) pOutline;
           // pSimpleRenderer.a
            geoLayer.Renderer = pSimpleRenderer as IFeatureRenderer;
        }



        /// <summary>
        /// 根据图层名找到图层索引
        /// </summary>
        /// <param name="mainMap"></param>
        /// <param name="layerName"></param>
        /// <returns></returns>

        public static ILayer returnIndexByLayerName(IMapControl3 mainMap, string layerName)
        {
            ILayer index = null;
            ILayer temp = null;
            int layerCount = mainMap.LayerCount;
            for (int i = 0; i < layerCount; i++)
            {
                temp = mainMap.get_Layer(i);
                if (temp is GroupLayer)
                {
                    ILayer LayerItem = temp;
                    ICompositeLayer pGroupLayer = LayerItem as ICompositeLayer;//获取子图层
                    for (int j = 0; j < pGroupLayer.Count; j++)
                    {
                        ILayer pCompositeLayer;
                        pCompositeLayer = pGroupLayer.get_Layer(j);
                        if (pCompositeLayer.Name == layerName)
                        {
                            index = pCompositeLayer;
                            IGroupLayer pGPLayer = LayerItem as IGroupLayer;
                            pGPLayer.Delete(pCompositeLayer);
                        }
                    }
                    if (pGroupLayer.Count == 0)
                    {
                        MainFrom.m_mapControl.Map.DeleteLayer(LayerItem);
                    }
                }
                else if (temp.Name == layerName)
                {
                    index = temp;
                }
            }
            return index;

        }
      

        //从groupLayer中查找FeatureLayer

        public static IFeatureLayer getSubLayer(ILayer layers, string layerName)
        {
            IFeatureLayer l = null;
            ICompositeLayer compositeLayer = layers as ICompositeLayer;
            for (int i = 0; i < compositeLayer.Count; i++)
            {
                ILayer layer = compositeLayer.Layer[i];   //递归
                if (layer is GroupLayer || layer is ICompositeLayer)
                {
                    l = getSubLayer(layer, layerName);
                    if (l != null)
                    {
                        break;
                    }
                }
                else
                {
                    while (layer.Name.Equals(layerName))
                    {
                        l = layer as IFeatureLayer;
                        break;
                    }
                }
            }
            return l;
        }


    }
}
