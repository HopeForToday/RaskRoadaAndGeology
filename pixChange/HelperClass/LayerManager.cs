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
using System.Drawing;

namespace pixChange.HelperClass
{
    public class LayerManager
    {
        /// <summary>
        /// 专题图渲染（多级颜色）。参数为需要渲染的字段（数值类型）,分级数目
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="strField"></param>
        /// <param name="numDesiredClasses"></param>
        public static void ClassBreaksMap(IFeatureLayer layer,string strField, int numDesiredClasses)
        {
            double[] classes = { 147.744196, 213.149274, 506.677245, 742.234523, 1096.249126, 4864.483353 };//断点值，两端分别是渲染字段的最小值和最大值
            IEnumColors pEnumColors;//颜色带
            Color startColor = Color.FromArgb(100,100,190, 150);//低值颜色
            Color endColor = Color.FromArgb(30, 0, 200, 0);//高值颜色  
            IGeoFeatureLayer pGeoFeatureLayer;
            ITable pTable;
            IClassifyGEN pClassify;
            ITableHistogram pTableHistogram;
            IBasicHistogram pBasicHistogram;
            object dataFrequency;
            object dataValues;
            int classesCount;
            IClassBreaksRenderer pClassBreaksRenderer;
            IColor pColor;
            ISimpleFillSymbol pSimpleFillSymbol;
            int breakIndex;
            pGeoFeatureLayer = (IGeoFeatureLayer)layer;
            pTable = (ITable)pGeoFeatureLayer.FeatureClass;
            pTableHistogram = new BasicTableHistogramClass();
            pBasicHistogram = (IBasicHistogram)pTableHistogram;
            pTableHistogram.Field = strField;
            pTableHistogram.Table = pTable;
            pBasicHistogram.GetHistogram(out dataValues, out dataFrequency);
            pClassify = new EqualIntervalClass();
            try
            {
                pClassify.Classify(dataValues, dataFrequency, ref numDesiredClasses);
            }
            catch (Exception ee)
            {
                //MessageBox.Show(ee.Message);
            }
            //classes = (double[])pClassify.ClassBreaks;
            //classes[0] = 147.744196;
            //classes[1]=213.149274;
            //classes[2]=506.677245;
            //classes[3]=742.234523;
            //classes[4]=1096.249126;
            //classes[5] = 4864.483353;
            classesCount = classes.GetUpperBound(0);
            pClassBreaksRenderer = new ClassBreaksRendererClass();
            pClassBreaksRenderer.Field = strField;
            pClassBreaksRenderer.BreakCount = classesCount;
            pClassBreaksRenderer.SortClassesAscending = true;
            pEnumColors = ProduceEnumColors(startColor, endColor, classesCount);//产生色带  
            for (breakIndex = 0; breakIndex < classesCount; breakIndex++)
            {
                pColor = pEnumColors.Next();
                pSimpleFillSymbol = new SimpleFillSymbolClass();
                pSimpleFillSymbol.Color = pColor;
                pSimpleFillSymbol.Style = esriSimpleFillStyle.esriSFSSolid;
                pClassBreaksRenderer.set_Symbol(breakIndex, pSimpleFillSymbol as ISymbol);
                pClassBreaksRenderer.set_Break(breakIndex, classes[breakIndex + 1]);
            }
            pGeoFeatureLayer.Renderer = (IFeatureRenderer)pClassBreaksRenderer;
            MainFrom.m_mapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
        }
        /// <summary>
        /// 根据起点颜色、终点颜色和级别数目，产生色带
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="gradecount"></param>
        /// <returns></returns>
        private static IEnumColors ProduceEnumColors(Color start, Color end, int gradecount)
        {
            //创建一个新AlgorithmicColorRampClass对象  
            IAlgorithmicColorRamp algColorRamp = new AlgorithmicColorRampClass();
            algColorRamp.ToColor = ConvertColorToIColor(end);//从.net的颜色转换  
            algColorRamp.FromColor =ConvertColorToIColor(start);
            //设置梯度类型  
            algColorRamp.Algorithm = esriColorRampAlgorithm.esriCIELabAlgorithm;
            //设置颜色带颜色数量  
            algColorRamp.Size = gradecount;
            //创建颜色带  
            bool bture = true;
            algColorRamp.CreateRamp(out bture);
            //使用IEnumColors获取颜色带  
            return algColorRamp.Colors;
        }
        
        /// <summary>
        /// 将.net颜色转变为ESRI的颜色
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static IColor ConvertColorToIColor(Color color)
        {
            IColor pColor = new RgbColorClass();
            pColor.RGB = color.B * 65536 + color.G * 256 + color.R;
            return pColor;
        }  
        /// <summary>
        /// 根据图层唯一值渲染图层
        /// </summary>
        /// <param name="R_pFeatureLayer"></param>
        /// <param name="sFieldName"></param>
        public static void UniqueValueRenderer(IFeatureLayer R_pFeatureLayer, string sFieldName)
        {
            IFeatureSelection R_pFeatureSelection = R_pFeatureLayer as IFeatureSelection;
            IFeature R_pFeature;
            IFeatureCursor R_FeatureCursor;
            R_pFeatureSelection = R_pFeatureLayer as IFeatureSelection;
            R_pFeatureSelection.Clear();
            ISelectionSet R_pSelectionSet = R_pFeatureSelection.SelectionSet;
            IFeatureClass R_pFeatureClass = R_pFeatureLayer.FeatureClass;
            IQueryFilter R_pQueryFilter = new QueryFilterClass();
            R_pQueryFilter.WhereClause = null;
            R_FeatureCursor = R_pFeatureClass.Search(R_pQueryFilter, true);
            R_pFeature = R_FeatureCursor.NextFeature();
            IUniqueValueRenderer renderer = new UniqueValueRendererClass();
            renderer.FieldCount = 1;
            renderer.set_Field(0, sFieldName);
            int index = R_pFeatureLayer.FeatureClass.Fields.FindField(sFieldName);
            IRandomColorRamp rx = new RandomColorRampClass();
            rx.MinSaturation = 15;
            rx.MaxSaturation = 30;
            rx.MinValue = 85;
            rx.MaxValue = 100;
            rx.StartHue = 0;
            rx.EndHue = 360;
            rx.Size = 100;
            bool ok=true;
            rx.CreateRamp(out ok);
            IEnumColors RColors = rx.Colors;
            RColors.Reset();
            while (R_pFeature != null)
            {
                ISimpleFillSymbol symd = new SimpleFillSymbolClass();
                symd.Style = esriSimpleFillStyle.esriSFSSolid;
                symd.Outline.Width = 1;
                symd.Color = RColors.Next();
                string valuestr = R_pFeature.get_Value(index).ToString();
                renderer.AddValue(valuestr,"", symd as ISymbol);
                R_pFeature = R_FeatureCursor.NextFeature();
            }
            IGeoFeatureLayer geoLayer = R_pFeatureLayer as IGeoFeatureLayer;
            geoLayer.Renderer = renderer as IFeatureRenderer;
            MainFrom.m_mapControl.Refresh();  
        }
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
            pPicturemksb.CreateMarkerSymbolFromFile(esriIPictureType.esriIPicturePNG, pictureName);
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
