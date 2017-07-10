using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SystemUI;
using pixChange;
using pixChange.HelperClass;
using ESRI.ArcGIS.DataSourcesRaster;

namespace RoadRaskEvaltionSystem.HelperClass
{
   /// <summary>
   /// 为了防止程序 的工作目录发生变化
   /// 下面最好全部采用绝对路径
   /// </summary>
   public class Common
    {
       private static string afterDisaterImagePath = Application.StartupPath + @"\Images\震后灾害点.png";
       public static string AfterDisaterImagePath
       {
           get
           {
               return afterDisaterImagePath;
           }
       }
       private static string beforeDisaterImagePath = Application.StartupPath + @"\Images\震前灾害点.png";
       public static string BeforeDisaterImagePath
       {
           get
           {
               return beforeDisaterImagePath;
           }
       }
       private static string conuntryPointImagePath = Application.StartupPath + @"\Images\乡村点.png";
       public static string CountryPointImagePath
       {
           get
           {
               return conuntryPointImagePath;
           }
       }
       private static string stopImagePath = Application.StartupPath + @"\Images\stop.jpg";
       public static string StopImagePath
       {
           get { return stopImagePath; }
       }
       private static string routeBreakImagePath = Application.StartupPath + @"\Images\routebreak.jpg";
       public static string  RouteBeakImggePath
       {
           get { return routeBreakImagePath; }
       }
       private static string mapPath = Application.StartupPath+@"\Resource\地图文档\ourMap.mxd";
        
       public static string MapPath
       {
           get { return mapPath; }
       }
       private static string betterRoutesPath = Application.StartupPath + @"\Resource\绕行线路";
       /// <summary>
       /// 绕行数据存放路径
       /// </summary>
       public static string BetterRoutesPath
       {
           get { return betterRoutesPath; }
       }
       private static string routeNetFeaturePath = Application.StartupPath + @"\Resource\公路\公路网";
       /// <summary>
       /// 公路网数据路径
       /// </summary>
       public static string RouteNetFeaturePath
       {
           get { return routeNetFeaturePath; }
       }
       private static string riskDataPath = Application.StartupPath + @"\Resource\风险评价";
       public static string RiskDataPath
       {
           get { return riskDataPath; }
       }
       private static string netWorkPath = Application.StartupPath + @"\Resource\网络数据集\roadnetworks.mdb";
       /// <summary>
       /// 网络数据集
       /// </summary>
       public static string NetWorkPath
       {
           get { return netWorkPath; }
       }
       /// <summary>
       /// 栅格底图的存储路径
       /// </summary>
        //基础数据
        public static string BaserasterPath = Application.StartupPath+@"\Rources\BaseData\BaseRasterData\";
       //风险因素数据
        public static string conditionRaster = Application.StartupPath + @"\Rources\ConditionData\ConditionRasterData\";
       //风险综合数据
        public static string conditionAllRaster = Application.StartupPath + @"\Rources\ConditionAllData\ConditionAllRasterData\";
        ////生态数据
        //public static string EcologyrasterPath = @"..\..\Rources\EcologyData\EcologyRasterData\";
        ////地质灾害数据
        //public static string GeoDisasterrasterPath = @"..\..\Rources\GeoDisasterData\GeoDisasterRasterData\";
        ////公路数据
        //public static string RoadrasterPath = @"..\..\Rources\RoadData\RoadRasterData\";
        //public static string rasterPath = @"..\..\Rources\RasterData\";
        /// <summary>
        /// 矢量底图的存储路径
        /// </summary>
        /// //基础数据
        public static string BaseshapePath = Application.StartupPath + @"\Rources\BaseData\BaseShapeData\";
        //风险因素数据
        public static string conditionShape = Application.StartupPath + @"\Rources\ConditionData\ConditionShapeData\";
        //风险综合数据
        public static string conditionAllShape = Application.StartupPath + @"\Rources\ConditionAllData\ConditionAllShapeData\";
        //生态数据
        //public static string EcologyshapePath = @"..\..\Rources\EcologyData\EcologyShapeData\";
        ////地质灾害数据
        //public static string GeoDisastershapePath = @"..\..\Rources\GeoDisasterData\GeoDisasterShapeData\";
        ////公路数据
        //public static string RoadshapePath = @"..\..\Rources\RoadData\RoadShapeData\";
        //public static string shapPath = @"..\..\Rources\ShapeData\";
       /// <summary>
       /// areaXML保存路径
       /// </summary>
       public static string arexmlPath = Application.StartupPath + @"\Rources\xmlData\AreaXML.xml";
       public  static string[] hour = { "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22","23"};

       public static AccessDataBase DBHander = new AccessDataBase();
       public static bool InsertShapeLayer(Boolean IsEqual, ILayer pFlayer)//插入矢量图
       {
           int count = MainFrom.m_mapControl.LayerCount;
           if (count == 0)
           {
               MainFrom.groupLayer.Add(pFlayer);
               MainFrom.m_mapControl.AddLayer(MainFrom.groupLayer);
           }
           else
           {
               for (int m = count - 1; m >= 0; m--)
               {
                   IMapLayers pLayers = MainFrom.m_mapControl.Map as IMapLayers;
                   ILayer pGL = MainFrom.m_mapControl.get_Layer(m);
                   if (pGL.Name == MainFrom.groupLayer.Name)
                   {
                       IsEqual = true;
                       if (pGL is IGroupLayer)
                       {
                           pLayers.InsertLayerInGroup((IGroupLayer)pGL, pFlayer, false, 0);
                       }
                   }
               }
               if (!IsEqual)
               {
                   MainFrom.groupLayer.Add(pFlayer);
                   MainFrom.m_mapControl.AddLayer(MainFrom.groupLayer);

               }
           }
           return IsEqual;
       }
       public static void funColorForRaster_Classify(IRasterLayer pRasterLayer,int ClassNum)
       {
           IRasterClassifyColorRampRenderer pRClassRend = new RasterClassifyColorRampRendererClass();
           IRasterRenderer pRRend = pRClassRend as IRasterRenderer;
           IRaster pRaster = pRasterLayer.Raster;
           IRasterBandCollection pRBandCol = pRaster as IRasterBandCollection;
           IRasterBand pRBand = pRBandCol.Item(0);
           if (pRBand.Histogram == null)
           {
               pRBand.ComputeStatsAndHist();
           }
           pRRend.Raster = pRaster;
           pRClassRend.ClassCount = ClassNum;
           pRRend.Update();
           IRgbColor pFromColor = new RgbColorClass();
           pFromColor.Red = 0;//绿  
           pFromColor.Green = 255;
           pFromColor.Blue = 0;
           IRgbColor pToColor = new RgbColorClass();
           pToColor.Red = 255;//红
           pToColor.Green = 0;
           pToColor.Blue = 0;
           IAlgorithmicColorRamp colorRamp = new AlgorithmicColorRampClass();
           colorRamp.Size = ClassNum;
           colorRamp.FromColor = pFromColor;
           colorRamp.ToColor = pToColor;
           bool createColorRamp;
           colorRamp.CreateRamp(out createColorRamp);
           IFillSymbol fillSymbol = new SimpleFillSymbolClass();
           for (int i = 0; i < pRClassRend.ClassCount; i++)
           {
               fillSymbol.Color = colorRamp.get_Color(i);
               pRClassRend.set_Symbol(i, fillSymbol as ISymbol);
               pRClassRend.set_Label(i,(i+1).ToString());
           }
           pRasterLayer.Renderer = pRRend;
           MainFrom.m_mapControl.AddLayer(pRasterLayer);
       }
      
    }
}
