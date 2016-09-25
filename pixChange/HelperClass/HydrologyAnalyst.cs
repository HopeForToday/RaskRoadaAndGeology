using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.SpatialAnalyst;
using ESRI.ArcGIS.SpatialAnalystTools;

namespace RoadRaskEvaltionSystem.HelperClass
{
    class HydrologyAnalyst
    {
        public static void test()
        {
            Console.WriteLine("s调用了水文分析的类！！");
        }
        //使用GP工具完成相关分析
     //水文分析工具大部分在空间分析下面
        public static void HydrologyAnalys(string workSpacePath,
                                                             object demSource,
                                                             object flowDir,
                                                             object sinkOutRaster,
                                                               object watershSink,
                                                                object zonelField,
                                                                object zonalMin,
                                                                object zonalMax,
                                                                object sinkdep,
                                                                object filldem,
                                                                object fdirfill,
                                                                object flowAcc,
                                                                object flowlendown,
                                                                object Flowlenup,
                                                                object streamnet,
                                                                object streamfea,
                                                                object streamlinkResult)
        {
            //初始化GP工具
            Geoprocessor gp = new Geoprocessor();
            try
            {
                 gp.OverwriteOutput = true;
                //提取水流方向
                FlowDirection flowDirection = new FlowDirection(demSource, flowDir);
              //  flowDirection.force_flow = "FORCE";
              
                gp.Execute(flowDirection, null);
                //进行洼地计算
                Sink sink = new Sink(flowDir, sinkOutRaster);
                gp.Execute(sink, null);
                //洼地深度计算
                Watershed watershed = new Watershed(flowDir, sinkOutRaster, watershSink);
                watershed.pour_point_field = zonelField;
                gp.Execute(watershed, null);
                //计算每个洼地所形成的贡献区域的最低高程
                ZonalStatistics zonalSta = new ZonalStatistics(watershSink, zonelField, demSource, zonalMin);
                zonalSta.statistics_type = "MINIMUM";
                gp.Execute(zonalSta, null);
                //计算每个洼地贡献区域出口的最低高程即洼地出水口高程
                ZonalFill zonalFill = new ZonalFill(watershSink, demSource, zonalMax);
                gp.Execute(zonalFill, null);
                //计算洼地深度
                IWorkspaceFactory rWorkspaceFactory = new RasterWorkspaceFactory();
                IWorkspace myWorkspace = rWorkspaceFactory.OpenFromFile(workSpacePath, 0);
                IRasterWorkspace rasterWorkspace = myWorkspace as IRasterWorkspace;
                IRasterDataset rasterds1 = OpenRasterDataSet(rasterWorkspace,zonalMin.ToString(),true);
                IRasterDataset rasterds2 = OpenRasterDataSet(rasterWorkspace,zonalMax.ToString(),true);
                IMapAlgebraOp mapAlgebra = new RasterMapAlgebraOpClass();
                IGeoDataset geo1=rasterds1 as IGeoDataset;
                 IGeoDataset geo2=rasterds1 as IGeoDataset;
                mapAlgebra.BindRaster(geo1,"raster1");
                mapAlgebra.BindRaster(geo2,"raster2");
                IGeoDataset pOutGeoDT = mapAlgebra.Execute("[raster1] - [raster2]");
              //  "\"rasterds1\"*\"rasterds2\"";
                //RasterCalculatorFunction(expression, sinkdep, rasterds1, rasterds2);
                //洼地填充
                Fill fill = new Fill(demSource, filldem);
                gp.Execute(fill, null);
                ////////汇流累积量
                //基于无洼地DEM的水流方向的计算
                flowDirection = new FlowDirection(filldem, fdirfill);
                gp.Execute(flowDirection, null);
                //汇流累积量的计算
                FlowAccumulation flowAccu = new FlowAccumulation(fdirfill, flowAcc);
                gp.Execute(flowAccu, null);
                //////水流长度
                //顺流计算和逆流计算
                FlowLength flowLength = new FlowLength(fdirfill, flowlendown);
                flowLength.direction_measurement = "DOWNSTREAM";
                gp.Execute(flowLength, null);
                flowLength.out_raster = Flowlenup;
                flowLength.direction_measurement = "UPSTREAM";
                gp.Execute(flowLength, null);
                ////河网的提取
                //设定阈值
                int value = 500; //表达式暂时不写
                RasterCalculator rasCalculator = new RasterCalculator(null, streamnet);
                gp.Execute(flowLength, null);

                //栅格河网矢量化
                StreamToFeature stream_toFature = new StreamToFeature(streamnet, fdirfill, streamfea);
                gp.Execute(stream_toFature, null);

                ////stream link的生成
                StreamLink streamLink = new StreamLink(streamnet, fdirfill, streamlinkResult);

                /**分级暂时不做*/
            }
            catch(Exception e)
            {
                Debug.Print(e.Message);
                for (int i = 0; i < gp.MessageCount; i++)
                {
                    Debug.Print(gp.GetMessage(i));
                }
            }

        }
        //缓冲区分析
        public static void BufferSpatialAnalyst(object inPutFeature,object outPutFeature,object bufferCondition)
        {
            //初始化GP工具
            Geoprocessor gp = new Geoprocessor();
            gp.OverwriteOutput = true;
            //调用缓冲区工具
            ESRI.ArcGIS.AnalysisTools.Buffer buffer = new ESRI.ArcGIS.AnalysisTools.Buffer(inPutFeature, outPutFeature, bufferCondition);
            buffer.dissolve_option = "ALL";//这个要设成ALL,否则相交部分不会融合
            buffer.line_side = "FULL";//默认是"FULL",最好不要改否则出错
            buffer.line_end_type = "ROUND";//默认是"ROUND",最好不要改否则出错
            gp.Execute(buffer, null);
        }
        //栅格计算器
        public static void RasterCalculatorFunction(object expression, object outPutRaster,params object[] source)
        {
            Geoprocessor gp = new Geoprocessor();
            gp.OverwriteOutput = true;
            RasterCalculator rasterCalculator = new RasterCalculator(expression, outPutRaster);
            gp.Execute(rasterCalculator, null);
        }
        //打开栅格数据集
        public static IRasterDataset OpenRasterDataSet(IRasterWorkspace rasterWorkspace,string name,bool isFullName)
        {
            if(isFullName)
            {
                string []arrays = name.Split('\\');
                name = arrays[arrays.Length-1];
            }
            IRasterDataset dataSet = rasterWorkspace.OpenRasterDataset(name);
            return dataSet;
        }
    }
}
