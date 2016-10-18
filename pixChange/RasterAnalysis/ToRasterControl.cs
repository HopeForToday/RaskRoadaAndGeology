using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.ConversionTools;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.GeoAnalyst;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.SpatialAnalyst;
using ESRI.ArcGIS.SpatialAnalystTools;
using pixChange;
using pixChange.HelperClass;
using ESRI.ArcGIS.Geometry;
using RoadRaskEvaltionSystem.HelperClass;

namespace RoadRaskEvaltionSystem.RasterAnalysis
{
   public class ToRasterControl
    {
       /// <summary>
       /// 使用GP工具和转化工具进行要素转栅格
       /// </summary>
       /// <param name="inputFeature"></param>
       /// <param name="outRaster"></param>
       /// <param name="fieldName"></param>
       /// <param name="cellSize"></param>
       public static bool Rasterize(object inputFeature, object outRaster, object fieldName, object cellSize)
       {
           //Runtime manager to find the ESRI product installed in the system  
           //ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.Desktop);  

           Geoprocessor geoprocessor = new Geoprocessor();
           geoprocessor.OverwriteOutput = true;
          // geoprocessor.GetEnvironmentValue();
           FeatureToRaster featureToRaster = new FeatureToRaster();
           featureToRaster.cell_size = cellSize;
           featureToRaster.in_features = inputFeature;
           featureToRaster.out_raster = outRaster;
           featureToRaster.field = fieldName;
           try
           {
               geoprocessor.Execute(featureToRaster, null);
               return true;
           }
           catch (Exception)
           {

               return false;
           }
          
       }
       /// <summary>
       /// 计算雨量系数
       /// </summary>
       /// <returns></returns>
       public static float RainsCoffcient()
       {
           float coffcient=0;



           return coffcient;
       }
       /// <summary>
       /// 道路风险计算
       /// </summary>
       /// <param name="workPath">存储路径</param>
       /// <param name="roadEvalPath">道路评价结果 </param>
       /// <param name="roadRainsShpPath">加了雨量字段的道路缓冲区</param>
       /// <returns></returns>
       public static bool RoadRaskCaulte(string roadEvalName, string roadRainsName, string saveWorkspace)
       {
           //读取 道路评价结果栅格数据的信息 
           RasterHelper rh=new RasterHelper();
           //IRasterWorkspace rasterWorkspace =  new RasterLayer();
           IWorkspaceFactory rWorkspaceFactory = new RasterWorkspaceFactory();
           IWorkspace SWorkspace = rWorkspaceFactory.OpenFromFile(saveWorkspace, 0);
           IRasterWorkspace rasterWorkspace = SWorkspace as IRasterWorkspace;

           IRasterDataset rasterDt = rasterWorkspace.OpenRasterDataset(roadEvalName);
         
           //  var t = rh.GetRasterProps(rasterDt); 
           IRasterLayer rasterLayer = new RasterLayer();

           rasterLayer.CreateFromFilePath(saveWorkspace + "\\" + roadEvalName);
           IRaster pRaster = rasterLayer.Raster;
       
           IRasterProps rasterProps = (IRasterProps)pRaster; ;//存储了栅格信息
           
           //初始化GP工具
           Geoprocessor gp = new Geoprocessor();
           gp.OverwriteOutput = true;
           //string path = @"D:\GISTest";
           //gp.SetEnvironmentValue("workspace", path);
           //道路缓冲区 根据雨量转栅格
           FeatureToRaster featureToRaster = new FeatureToRaster();
           featureToRaster.cell_size = rasterProps.MeanCellSize().X;//这里可以提前规定一个值，而不是每次去读取
           featureToRaster.in_features = OpenFeatureClass(saveWorkspace+"\\"+roadRainsName);
           featureToRaster.out_raster = saveWorkspace + "\\roadGrid";
           featureToRaster.field = "RAINS";//这个字段需要矢量图层中加上
           try
           {
               gp.Execute(featureToRaster, null);

           }
           catch (Exception ex)
           {
               Console.WriteLine("矢量转栅格失败！");
               return false;
           }

           //栅格计算器 计算风险级数
           IMapAlgebraOp mapAlgebra = new RasterMapAlgebraOpClass();
           IRasterDataset roadEvalRaster = OpenRasterDataSet(rasterWorkspace, roadEvalName);
           IRasterDataset roadGridRaster = OpenRasterDataSet(rasterWorkspace, saveWorkspace + @"\roadGrid");      
           IGeoDataset geo1 = roadEvalRaster as IGeoDataset;
           IGeoDataset geo2 = roadGridRaster as IGeoDataset;
           mapAlgebra.BindRaster(geo1, "EvalRaster");
           mapAlgebra.BindRaster(geo2, "RoadRains");
           IGeoDataset raskDataset = mapAlgebra.Execute("[EvalRaster] * [RoadRains] / 25");//然后存储  表达式必须间隔开
           ISaveAs saveAs = raskDataset as ISaveAs;
           saveAs.SaveAs("roadPre", SWorkspace, "");
           //加入图层
           IRasterLayer rasterLayer2 = new RasterLayer();

           rasterLayer2.CreateFromFilePath(saveWorkspace + "\\" + "roadPre");
          
           MainFrom.m_mapControl.AddLayer(rasterLayer2, 0);
           //MainFrom.m_mapControl.Refresh(esriViewDrawPhase.esriViewGeography, null, null);
           MainFrom.m_pTocControl.Update();
           //将生成的风险栅格重分类
           //<0.2	一级：可能性小
           //0.2-0.4	二级：可
           //能性较小
           //0.4-0.6	三级：可能性较大
           //0.6-0.8	四级：可能性大
           //>0.8	五级：可能性很大
           // 输入：raskDataset

           // 输出：geoDataset_result
           IReclassOp pReclassOp = new RasterReclassOpClass();
           INumberRemap pNumRemap = new NumberRemapClass();              
           pNumRemap.MapRange(0,0.2,1);
           pNumRemap.MapRange(0.2, 0.4, 2);
           pNumRemap.MapRange(0.4, 0.6, 3);
           pNumRemap.MapRange(0.6, 0.8, 4);
           pNumRemap.MapRange(0.8,1000,5);
           //pNumRemap.MapRangeToNoData(-1000,0);
           //pNumRemap.MapRangeToNoData(1000, 20000);
           IRemap pRemap = pNumRemap as IRemap; 
        // 重分类
        // geoDataset为上一步得到的栅格
        //     IGeoDataset geoDataset_result = pReclassOp.ReclassByRemap(raskDataset, pRemap, true);//还没有测试成功 


          // RasterCalculator rasterCalculator = new RasterCalculator("[EvalRaster]*[RoadRains]/25", saveWorkspace + @"\RainEval.tif");
           try
           {
              // gp.Execute(rasterCalculator, null);
           }
           catch (Exception ex)
           {
               Debug.Print(ex.Message);
               for (int i = 0; i < gp.MessageCount; i++)
               {
                   Debug.Print(gp.GetMessage(i));
               }
               Console.WriteLine("栅格计算失败！");
               return false;
           }                        
           return true;
       }
       /// <summary>
       /// 生成风险等级栅格
       /// </summary>
       /// <param name="roadEvalName"></param>
       /// <param name="rains"></param>
       /// <param name="saveWorkspace"></param>
       /// <returns></returns>
       public static bool RoadRaskCaulte(string roadEvalName,int rains,string saveWorkspace)
       {
           IWorkspaceFactory rWorkspaceFactory = new RasterWorkspaceFactory();
           IWorkspace SWorkspace = rWorkspaceFactory.OpenFromFile(saveWorkspace, 0);
           IRasterWorkspace rasterWorkspace = SWorkspace as IRasterWorkspace;
           //栅格计算器 计算风险级数  先不要签你的三方协议 然后存储  表达式必须隔开
           IMapAlgebraOp mapAlgebra = new RasterMapAlgebraOpClass();
           IRasterDataset roadEvalRaster = OpenRasterDataSet(rasterWorkspace, roadEvalName);
     
           IGeoDataset geo1 = roadEvalRaster as IGeoDataset;
        
           mapAlgebra.BindRaster(geo1, "EvalRaster");    
           IGeoDataset raskDataset = mapAlgebra.Execute("[EvalRaster] * 10 / 25");//然后存储  表达式必须间隔开
           //将生成的风险栅格重分类
           //<0.2	一级：可能性小
           //0.2-0.4	二级：可
           //能性较小
           //0.4-0.6	三级：可能性较大
           //0.6-0.8	四级：可能性大
           //>0.8	五级：可能性很大
           // 输入：raskDataset
         //   输出：geoDataset_result
           IRasterBandCollection pRsBandCol = raskDataset as IRasterBandCollection;
           IRasterBand pRasterBand = pRsBandCol.Item(0);
           pRasterBand.ComputeStatsAndHist();
           IRasterStatistics pRasterStatistic = pRasterBand.Statistics;
           double dMaxValue = pRasterStatistic.Maximum;
           double dMinValue = pRasterStatistic.Minimum;
           IReclassOp pReclassOp = new RasterReclassOpClass();
           INumberRemap pNumRemap = new NumberRemapClass();
           //pNumRemap.MapRange(dMinValue, 0.2, 1);
           //pNumRemap.MapRange(0.2, 0.4, 2);
           //pNumRemap.MapRange(0.4, 0.6, 3);
           //pNumRemap.MapRange(0.6, dMaxValue, 4);
           //pNumRemap.MapRangeToNoData(-1000, 0);
           //pNumRemap.MapRange(0.8, 1000, 5);
           pNumRemap.MapRange(dMinValue,0.2, 0);
           pNumRemap.MapRange(0.2, 0.4, 1);
           pNumRemap.MapRange(0.4, 0.6, 2);
           pNumRemap.MapRange(0.6, 0.8, 3);
           pNumRemap.MapRange(0.8, dMaxValue, 4);
           IRemap pRemap = pNumRemap as IRemap;
           //IGeoDataset geoDataset_result = pReclassOp.ReclassByRemap(raskDataset, pRemap, true);
           IRaster pOutRaster = pReclassOp.ReclassByRemap(raskDataset, pRemap, false) as IRaster;
           IRasterLayer rasterLayer = new RasterLayerClass();
           rasterLayer.CreateFromRaster(pOutRaster);
           if (rasterLayer != null)
           {
               //string fullPath = Common.RoadshapePath+"道路.shp";
               rasterLayer.Name = "公路风险";
               Boolean IsEqual = false;
               //int Position = fullPath.LastIndexOf("\\");
               ////文件目录
               //string FilePath = fullPath.Substring(0, Position);
               //string ShpName = fullPath.Substring(Position + 1);
               //IWorkspaceFactory pWF;
               //pWF = new ShapefileWorkspaceFactory();
               //IFeatureWorkspace pFWS;
               //pFWS = (IFeatureWorkspace)pWF.OpenFromFile(FilePath, 0);
               //IFeatureClass pFClass;
               //pFClass = pFWS.OpenFeatureClass(ShpName);
               //IFeatureLayer pFLayer = new FeatureLayer();
               //pFLayer.FeatureClass = pFClass;

               for (int i = 0; i < MainFrom.m_mapControl.LayerCount;i++ )
               {
                   ILayer ComLayer=MainFrom.m_mapControl.get_Layer(i);
                   if (rasterLayer.Name == ComLayer.Name)
                   {
                       IsEqual = true;
                   }
               }
               if (!IsEqual)
               {
                   //MainFrom.m_mapControl.AddLayer((ILayer)pFLayer);
                   MainFrom.m_mapControl.AddLayer(rasterLayer);
                   IEnvelope envelope = rasterLayer.AreaOfInterest;
                   MainFrom.m_mapControl.ActiveView.Extent = envelope;//缩放至图层 
               }
           }
           return true;
       }
     

       //打开栅格数据集
       public static IRasterDataset OpenRasterDataSet(IRasterWorkspace rasterWorkspace, string name)
       {
        
        //   name = name.Substring(0, name.LastIndexOf("."));
           IRasterDataset dataSet = rasterWorkspace.OpenRasterDataset(name);
           return dataSet;
       }
       //打开要素文件
       public static IFeatureClass OpenFeatureClass(string name)
       {            
           //利用"\\"将文件路径分成两部分 
           int Position = name.LastIndexOf("\\");
           //文件目录
           string FilePath = name.Substring(0, Position);
           //
           string ShpName = name.Substring(Position + 1);
           IWorkspaceFactory pWF;
           pWF = new ShapefileWorkspaceFactory();
           IFeatureWorkspace pFWS;
           pFWS = (IFeatureWorkspace)pWF.OpenFromFile(FilePath, 0);
           IFeatureClass pFClass;
           pFClass = pFWS.OpenFeatureClass(ShpName);
           return pFClass;
       }

    }


   
}
