using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadRaskEvaltionSystem.RasterAnalysis
{
    public interface IRoadRaskCaculate
    {
        /// <summary>
        /// 要素转栅格
        /// </summary>
        /// <param name="inputFeature"></param>
        /// <param name="outRaster"></param>
        /// <param name="fieldName"></param>
        /// <param name="cellSize"></param>
        /// <returns></returns>
         bool FeatureToRaster(object inputFeature, object outRaster, object fieldName, object cellSize);
        /// <summary>
        /// 道路风险计算 
        /// </summary>
        /// <param name="roadEvalName"></param>
        /// <param name="roadRainsName"></param>
        /// <param name="saveWorkspace"></param>
        /// <returns></returns>
          bool RoadRaskCaulte(string roadEvalName, string roadRainsName, string saveWorkspace);
           /// <summary>
       /// 生成风险等级栅格
       /// 是否和上面冲突？？
       /// </summary>
       /// <param name="roadEvalName"></param>
       /// <param name="rains"></param>
       /// <param name="saveWorkspace"></param>
       /// <returns></returns>
          bool RoadRaskCaulte(string roadEvalName, int rains, string saveWorkspace);
        /// <summary>
        /// 打开栅格数据集
        /// </summary>
        /// <param name="rasterWorkspace"></param>
        /// <param name="name"></param>
        /// <returns></returns>
          IRasterDataset OpenRasterDataSet(IRasterWorkspace rasterWorkspace, string name);
        /// <summary>
        /// 打开栅格数据集
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
         IRasterDataset OpenRasterDataSet(string name);
        /// <summary>
        /// 打开矢量数据集
        /// </summary>
        /// <param name="featureWorkspace"></param>
        /// <param name="name"></param>
        /// <returns></returns>
          IFeatureClass OpenFeatureClass(IFeatureWorkspace featureWorkspace,string name);
        //打开矢量数据集
          IFeatureClass OpenFeatureClass(string name);
    }
}
