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
using System.Data;
using System.Collections;
using System.Diagnostics;

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
            pColor.Transparency = color.A;
            return pColor;
        }  

        /// <summary>
        /// 根据图层唯一值渲染图层,行政区
        /// </summary>
        /// <param name="R_pFeatureLayer"></param>
        /// <param name="sFieldName"></param>
        public static void UniqueValueRenderer(IFeatureLayer R_pFeatureLayer, string sFieldName)
        {

            IGeoFeatureLayer geoLayer = R_pFeatureLayer as IGeoFeatureLayer;
            ITable pTable = geoLayer.FeatureClass as ITable;
            ICursor pCursor;
            IQueryFilter pQueryFilter = new QueryFilter();
            pQueryFilter.AddField(sFieldName);              //以唯一值作为条件
            pCursor = pTable.Search(pQueryFilter,true);
            IUniqueValueRenderer pUniqueValueR = new UniqueValueRendererClass();
            pUniqueValueR.FieldCount = 1;                   //单值渲染
            pUniqueValueR.set_Field(0, sFieldName);         //渲染字段
            IFeatureCursor pFeatureCursor=R_pFeatureLayer.Search(pQueryFilter,false);
            IFeature pFeature = pFeatureCursor.NextFeature();
            int index = R_pFeatureLayer.FeatureClass.FindField("Name");
            
            #region
            List<Color> colors = new List<Color>();         //存储行政区连续颜色带,以要素数量为上限
            colors = createcolor(colors, R_pFeatureLayer.FeatureClass.FeatureCount(pQueryFilter));
            int i = 15; 
            while (pFeature != null)
            {
                string value=pFeature.get_Value(index).ToString();
                pFeature = pFeatureCursor.NextFeature();
                ISimpleFillSymbol symd = new SimpleFillSymbolClass();
                symd.Style = esriSimpleFillStyle.esriSFSSolid;
                symd.Outline.Width = 1;
                if (value.Equals("飞仙关镇"))                       //此处用于渲染原始图层
                    symd.Color = ConvertColorToIColor(colors[0]);
                else if(value.Equals("凤禾乡"))
                    symd.Color = ConvertColorToIColor(colors[1]);
                else if(value.Equals("芦阳镇"))
                    symd.Color = ConvertColorToIColor(colors[2]);
                else if(value.Equals("沫东镇"))
                    symd.Color = ConvertColorToIColor(colors[3]);
                else if(value.Equals("思延乡"))
                    symd.Color = ConvertColorToIColor(colors[4]);
                else if(value.Equals("升隆乡"))
                    symd.Color = ConvertColorToIColor(colors[5]);
                else if(value.Equals("清源乡"))
                    symd.Color = ConvertColorToIColor(colors[6]);
                else if(value.Equals("隆兴乡"))
                    symd.Color = ConvertColorToIColor(colors[7]);
                else if(value.Equals("仁加乡"))
                    symd.Color = ConvertColorToIColor(colors[8]);
                else if(value.Equals("龙门乡"))
                    symd.Color = ConvertColorToIColor(colors[9]);
                else if(value.Equals("双石镇"))
                    symd.Color = ConvertColorToIColor(colors[10]);
                else if(value.Equals("太平镇"))
                    symd.Color = ConvertColorToIColor(colors[11]);
                else if(value.Equals("宝盛乡"))
                    symd.Color = ConvertColorToIColor(colors[12]);
                else if(value.Equals("中林乡"))
                    symd.Color = ConvertColorToIColor(colors[13]);
                else if(value.Equals("大川镇"))
                    symd.Color = ConvertColorToIColor(colors[14]);
                else
                {
                    symd.Color = ConvertColorToIColor(colors[i]);  //i后移一位，以便于对应name字段
                    i++; 
                }
                pUniqueValueR.AddValue(value, "", symd as ISymbol);
                
            }
            geoLayer.Renderer = pUniqueValueR as IFeatureRenderer;
            MainFrom.m_mapControl.Refresh();
            #endregion
            //IEnumerator pEnumreator;
            ////获取字段中各要素属性唯一值
            //IDataStatistics pDataStatistics = new DataStatisticsClass();
            //pDataStatistics.Field = sFieldName;//获取统计字段
            //pDataStatistics.Cursor = pCursor;
            //pEnumreator = pDataStatistics.UniqueValues;
            //int fieldcount = pDataStatistics.UniqueValueCount;//唯一值个数，以此确定颜色带范围
            //IUniqueValueRenderer pUniqueValueR = new UniqueValueRendererClass();
            //pUniqueValueR.FieldCount = 1;//单值渲染
            //pUniqueValueR.set_Field(0, sFieldName);//渲染字段
            //IEnumColors pEnumColor = GetColorRamp(fieldcount).Colors;
            //pEnumColor.Reset();
            //createcolor();int i=0;              //产生颜色带,
            //while (pEnumreator.MoveNext())
            //{
            //    string value = pEnumreator.Current.ToString();
            //    if (value != null)
            //    {
            //        //IColor pColor = pEnumColor.Next();
            //        ISimpleFillSymbol symd = new SimpleFillSymbolClass();
            //        symd.Style = esriSimpleFillStyle.esriSFSSolid;
            //        symd.Outline.Width = 1;
            //        symd.Color = ConvertColorToIColor(colors[i]);i++;   //i后移一位，以便于对应name字段
            //        pUniqueValueR.AddValue(value, "", symd as ISymbol);
            //    }
            //}
        }
        /// <summary>
        /// 根据图层唯一值渲染图层,地震烈度
        /// </summary>
        /// <param name="R_pFeatureLayer"></param>
        /// <param name="sFieldName"></param>
        public static void UniqueValueRendererEarthquake(IFeatureLayer R_pFeatureLayer, string sFieldName)
        {

            IGeoFeatureLayer geoLayer = R_pFeatureLayer as IGeoFeatureLayer;
            ITable pTable = geoLayer.FeatureClass as ITable;
            ICursor pCursor;
            IQueryFilter pQueryFilter = new QueryFilter();
            pQueryFilter.AddField(sFieldName);              //以唯一值作为条件
            pCursor = pTable.Search(pQueryFilter, true);
            IUniqueValueRenderer pUniqueValueR = new UniqueValueRendererClass();
            pUniqueValueR.FieldCount = 1;                   //单值渲染
            pUniqueValueR.set_Field(0, sFieldName);         //渲染字段
            IFeatureCursor pFeatureCursor = R_pFeatureLayer.Search(pQueryFilter, false);
            IFeature pFeature = pFeatureCursor.NextFeature();
            int index = R_pFeatureLayer.FeatureClass.FindField(sFieldName);

            List<Color> colors = new List<Color>();         //存储地震烈度连续颜色带,以要素数量为上限
            colors = createEarthquakecolor(colors, R_pFeatureLayer.FeatureClass.FeatureCount(pQueryFilter));
            int i = 5;
            while (pFeature != null)
            {
                string value = pFeature.get_Value(index).ToString();
                pFeature = pFeatureCursor.NextFeature();
                ISimpleFillSymbol symd = new SimpleFillSymbolClass();
                symd.Style = esriSimpleFillStyle.esriSFSSolid;
                symd.Outline.Width = 1;
                if (value.Equals("Ⅸ度"))                       //此处用于渲染原始图层
                    symd.Color = ConvertColorToIColor(colors[4]);
                else if (value.Equals("Ⅷ度"))
                    symd.Color = ConvertColorToIColor(colors[3]);
                else if (value.Equals("Ⅶ度"))
                    symd.Color = ConvertColorToIColor(colors[2]);
                else if (value.Equals("Ⅵ度"))
                    symd.Color = ConvertColorToIColor(colors[1]);
                else if (value.Equals("V度"))
                    symd.Color = ConvertColorToIColor(colors[0]);
                else
                {
                    symd.Color = ConvertColorToIColor(colors[i]);  //i后移一位，以便于对应name字段
                    i++;
                }
                pUniqueValueR.AddValue(value, "", symd as ISymbol);

            }
            geoLayer.Renderer = pUniqueValueR as IFeatureRenderer;
            MainFrom.m_mapControl.Refresh();
        }
        public static void RiverRender(IFeatureLayer R_pFeatureLayer, string sFieldName)
        {
            IGeoFeatureLayer geoLayer = R_pFeatureLayer as IGeoFeatureLayer;
            ITable pTable = geoLayer.FeatureClass as ITable;
            ICursor pCursor;
            IQueryFilter pQueryFilter = new QueryFilter();
            pQueryFilter.AddField(sFieldName);              //以唯一值作为条件
            pCursor = pTable.Search(pQueryFilter, true);
            IUniqueValueRenderer pUniqueValueR = new UniqueValueRendererClass();
            pUniqueValueR.FieldCount = 1;                   //单值渲染
            pUniqueValueR.set_Field(0, sFieldName);         //渲染字段
        }

        private static IRandomColorRamp GetColorRamp(int size)
        {
            IRandomColorRamp pRandomColorRamp = new RandomColorRampClass();
            pRandomColorRamp.StartHue = 10;
            pRandomColorRamp.EndHue = 300;
            pRandomColorRamp.MaxSaturation = 100;
            pRandomColorRamp.MinSaturation = 0;
            pRandomColorRamp.MaxValue = 100;
            pRandomColorRamp.MinValue = 0;
            pRandomColorRamp.Size = size;
            bool ok = true;
            pRandomColorRamp.CreateRamp(out ok);
            return pRandomColorRamp;
        }

        /// <summary>
        /// 产生行政区的渐变颜色带
        /// </summary>
        /// <param name="colors"></param>
        /// <param name="colornum"></param>
        /// <returns></returns>
        private static List<Color> createcolor(List<Color> colors,int colornum)
        {
            colors.Add(Color.FromArgb(100, 77, 117, 180));   //飞仙关
            colors.Add(Color.FromArgb(100, 159, 169, 228));  //凤禾乡
            colors.Add(Color.FromArgb(100, 107, 134, 202));  //芦阳镇
            colors.Add(Color.FromArgb(100, 171, 178, 233));  //沫东镇
            colors.Add(Color.FromArgb(100, 83, 121, 186));  //思延乡
            colors.Add(Color.FromArgb(100, 83, 121, 237));  //升隆乡
            colors.Add(Color.FromArgb(100, 184, 189, 239));  //清源乡
            colors.Add(Color.FromArgb(100, 126, 146, 212));  //隆兴乡
            colors.Add(Color.FromArgb(100, 212, 213, 249));  //仁加乡
            colors.Add(Color.FromArgb(100, 147, 159, 223));  //龙门乡
            colors.Add(Color.FromArgb(100, 116, 138, 207));  //双石镇
            colors.Add(Color.FromArgb(100, 136, 152, 217));  //太平镇
            colors.Add(Color.FromArgb(100, 198, 201, 245));  //宝盛乡
            colors.Add(Color.FromArgb(100, 99, 129, 196));  //中林乡
            colors.Add(Color.FromArgb(100, 226, 226, 255));  //大川镇
            for (int i = 15; i < colornum+15; i++)         //产生随机颜色,用于多余要素处理
            {
                Random ran1 = new Random(i);
                Random ran2 = new Random(2 * i);
                Random ran3 = new Random(3 * i);
                Random ran0=new Random(4*i);
                colors.Add(Color.FromArgb(ran0.Next(0,255),ran1.Next(0, 255), ran2.Next(0, 255), ran3.Next(0, 255)));
            }
                return colors;
        }
        /// <summary>
        /// 产生地震烈度的渐变颜色带
        /// </summary>
        /// <param name="colors"></param>
        /// <param name="colornum"></param>
        /// <returns></returns>
        private static List<Color> createEarthquakecolor(List<Color> colors, int colornum)
        {
            colors.Add(Color.FromArgb(100, 222, 222, 253));   //V度
            colors.Add(Color.FromArgb(100, 161, 167, 231));  //VI度
            colors.Add(Color.FromArgb(100, 110, 131, 209));  //VII度
            colors.Add(Color.FromArgb(100, 68, 104, 187));  //VIII度
            colors.Add(Color.FromArgb(100, 38, 86, 165));  //IX度
            for (int i = 5; i < colornum + 5; i++)         //产生随机颜色,用于多余要素处理
            {
                Random ran1 = new Random(i);
                Random ran2 = new Random(2 * i);
                Random ran3 = new Random(3 * i);
                Random ran0 = new Random(4 * i);
                colors.Add(Color.FromArgb(ran0.Next(0, 255), ran1.Next(0, 255), ran2.Next(0, 255), ran3.Next(0, 255)));
            }
            return colors;
        }
        /// <summary>
        /// 岩性图层单值渲染
        /// </summary>
        /// <param name="R_pFeatureLayer"></param>
        /// <param name="sFieldName"></param>
        public static void UniqueValueRendererLithology(IFeatureLayer R_pFeatureLayer, string sFieldName)
        {
            IGeoFeatureLayer geoLayer = R_pFeatureLayer as IGeoFeatureLayer;
            ITable pTable = geoLayer.FeatureClass as ITable;
            ICursor pCursor;
            IQueryFilter pQueryFilter = new QueryFilter();
            pQueryFilter.AddField(sFieldName);              //以唯一值作为条件
            pCursor = pTable.Search(pQueryFilter, true);
            IUniqueValueRenderer pUniqueValueR = new UniqueValueRendererClass();
            pUniqueValueR.FieldCount = 1;                   //单值渲染
            pUniqueValueR.set_Field(0, sFieldName);         //渲染字段
            IFeatureCursor pFeatureCursor = R_pFeatureLayer.Search(pQueryFilter, false);
            IFeature pFeature = pFeatureCursor.NextFeature();
            int index = R_pFeatureLayer.FeatureClass.FindField(sFieldName);

            int i = 20;
            while (pFeature != null)
            {
                string value = pFeature.get_Value(index).ToString();
                pFeature = pFeatureCursor.NextFeature();
                IPictureFillSymbol pFillSymbol = new PictureFillSymbolClass();
                pFillSymbol.Outline.Width = 1;
                if (value.Equals("上三叠统"))
                {
                    pFillSymbol.CreateFillSymbolFromFile(esriIPictureType.esriIPicturePNG, LithologyImageUtils.getImagePath("上三叠统"));
                }
                else if (value.Equals("上古生界"))
                    pFillSymbol.CreateFillSymbolFromFile(esriIPictureType.esriIPicturePNG, LithologyImageUtils.getImagePath("上古生界"));
                else if (value.Equals("上白垩统"))
                    pFillSymbol.CreateFillSymbolFromFile(esriIPictureType.esriIPicturePNG, LithologyImageUtils.getImagePath("上白垩统"));
                else if (value.Equals("上震旦统"))
                    pFillSymbol.CreateFillSymbolFromFile(esriIPictureType.esriIPicturePNG, LithologyImageUtils.getImagePath("上震旦统"));
                else if (value.Equals("下三叠统"))
                    pFillSymbol.CreateFillSymbolFromFile(esriIPictureType.esriIPicturePNG, LithologyImageUtils.getImagePath("下三叠统"));
                else if (value.Equals("下白垩统"))
                    pFillSymbol.CreateFillSymbolFromFile(esriIPictureType.esriIPicturePNG, LithologyImageUtils.getImagePath("下白垩统"));
                else if (value.Equals("下第三系"))
                    pFillSymbol.CreateFillSymbolFromFile(esriIPictureType.esriIPicturePNG, LithologyImageUtils.getImagePath("下第三系"));
                else if (value.Equals("下远古界"))
                    pFillSymbol.CreateFillSymbolFromFile(esriIPictureType.esriIPicturePNG, LithologyImageUtils.getImagePath("下远古界"));
                else if (value.Equals("下震旦统"))
                    pFillSymbol.CreateFillSymbolFromFile(esriIPictureType.esriIPicturePNG, LithologyImageUtils.getImagePath("下震旦统"));
                else if (value.Equals("中远古界"))
                    pFillSymbol.CreateFillSymbolFromFile(esriIPictureType.esriIPicturePNG, LithologyImageUtils.getImagePath("中远古界"));
                else if (value.Equals("二叠系"))
                    pFillSymbol.CreateFillSymbolFromFile(esriIPictureType.esriIPicturePNG, LithologyImageUtils.getImagePath("二叠系"));
                else if (value.Equals("二叠系  三叠系"))
                    pFillSymbol.CreateFillSymbolFromFile(esriIPictureType.esriIPicturePNG, LithologyImageUtils.getImagePath("二叠系  三叠系"));
                else if (value.Equals("二叠系上统"))
                    pFillSymbol.CreateFillSymbolFromFile(esriIPictureType.esriIPicturePNG, LithologyImageUtils.getImagePath("二叠系上统"));
                else if (value.Equals("侏罗纪"))
                    pFillSymbol.CreateFillSymbolFromFile(esriIPictureType.esriIPicturePNG, LithologyImageUtils.getImagePath("侏罗纪"));
                else if (value.Equals("奥陶系  志留系"))
                    pFillSymbol.CreateFillSymbolFromFile(esriIPictureType.esriIPicturePNG, LithologyImageUtils.getImagePath("奥陶系  志留系"));
                else if (value.Equals("志留系 泥盆系"))
                    pFillSymbol.CreateFillSymbolFromFile(esriIPictureType.esriIPicturePNG, LithologyImageUtils.getImagePath("志留系 泥盆系"));
                else if (value.Equals("新近系"))
                    pFillSymbol.CreateFillSymbolFromFile(esriIPictureType.esriIPicturePNG, LithologyImageUtils.getImagePath("新近系"));
                else if (value.Equals("泥盆系"))
                    pFillSymbol.CreateFillSymbolFromFile(esriIPictureType.esriIPicturePNG, LithologyImageUtils.getImagePath("泥盆系"));
                else if (value.Equals("石炭系  二叠系"))
                    pFillSymbol.CreateFillSymbolFromFile(esriIPictureType.esriIPicturePNG, LithologyImageUtils.getImagePath("石炭系  二叠系"));
                else
                {
                    pFillSymbol.CreateFillSymbolFromFile(esriIPictureType.esriIPicturePNG, LithologyImageUtils.getImagePath("其他岩性")); //i后移一位，以便于对应name字段
                    i++;
                } 
                pUniqueValueR.AddValue(value, "", pFillSymbol as ISymbol);

            }
            geoLayer.Renderer = pUniqueValueR as IFeatureRenderer;
            MainFrom.m_mapControl.Refresh();
        }
        /// <summary>
        /// 设置要素图片填充
        /// </summary>
        /// <param name="featureLayer"></param>
        /// <param name="pictureName"></param>
        public static void SetFeaturePictureFillSymbol(IFeatureLayer featureLayer, string pictureName)
        {
            IGeoFeatureLayer geoLayer = featureLayer as IGeoFeatureLayer;
            IPictureFillSymbol pFillSymbol = new PictureFillSymbolClass();
            pFillSymbol.CreateFillSymbolFromFile(esriIPictureType.esriIPicturePNG, pictureName);
            ISimpleRenderer simpleRender = geoLayer.Renderer as ISimpleRenderer;
            if (simpleRender==null)
            {
                simpleRender = new SimpleRendererClass();
            }
            simpleRender.Symbol = pFillSymbol as ISymbol;
            geoLayer.Renderer = simpleRender as IFeatureRenderer;
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
            IPictureMarkerSymbol pPicturemksb = CreatePictureSymbol(pictureName, size);
            ISimpleRenderer simpleRender = geoLayer.Renderer as ISimpleRenderer;
            if (simpleRender == null)
            {
                simpleRender = new SimpleRendererClass();
            }
            simpleRender.Symbol = pPicturemksb as ISymbol;
            geoLayer.Renderer = simpleRender as IFeatureRenderer;
        }

        private static IPictureMarkerSymbol CreatePictureSymbol(string pictureName, int size)
        {
            IPictureMarkerSymbol pPicturemksb = new PictureMarkerSymbolClass();
            pPicturemksb.Size = size;
            string extension = System.IO.Path.GetExtension(pictureName);
            switch (extension)
            {
                case ".png":
                    pPicturemksb.CreateMarkerSymbolFromFile(esriIPictureType.esriIPicturePNG, pictureName);
                    break;
                case ".jpg":
                    pPicturemksb.CreateMarkerSymbolFromFile(esriIPictureType.esriIPictureJPG, pictureName);
                    break;
                case ".gif":
                    pPicturemksb.CreateMarkerSymbolFromFile(esriIPictureType.esriIPictureGIF, pictureName);
                    break;
                case ".emf":
                    pPicturemksb.CreateMarkerSymbolFromFile(esriIPictureType.esriIPictureEMF, pictureName);
                    break;
            }
            return pPicturemksb;
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
