using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadRaskEvaltionSystem.HelperClass
{
    /// <summary>
    /// 空间查询帮助类
    /// 2015/12/11 fhr
    /// </summary>
    class FeatureQueryUtil
    {
         /// <summary>
        /// 利用语句进行查询 返回游标和查询过滤类
         /// </summary>
         /// <param name="featureLayer"></param>
         /// <param name="whereClause"></param>
         /// <param name="queryFilterCopy"></param>
         /// <returns></returns>
        public static IFeatureCursor QueryFeatureInLayer(IFeatureLayer featureLayer, String whereClause,ref IQueryFilter queryFilterCopy)
        {
            IFeatureCursor featureCursor = null;
            IQueryFilter2 queryFilter = new QueryFilterClass();
            queryFilter.WhereClause = whereClause;
            IFeatureClass featureClass = featureLayer.FeatureClass;
            featureCursor=featureClass.Search(queryFilter, false);
            queryFilterCopy = queryFilter;
            return featureCursor;
        }
        /// <summary>
        /// 利用空间位置进行查询 返回游标和查询过滤类
        /// </summary>
        /// <param name="featureLayer"></param>
        /// <param name="geometry"></param>
        /// <param name="queryFilterCopy"></param>
        /// <returns></returns>
        public static IFeatureCursor QueryFeatureInLayer(IFeatureLayer featureLayer, IGeometry geometry, ref IQueryFilter queryFilterCopy)
        {
            IFeatureCursor featureCursor = null;
            ISpatialFilter spatialFilter = new SpatialFilter();
            switch (featureLayer.FeatureClass.ShapeType)
            {
                case esriGeometryType.esriGeometryPoint:
                    spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;
                    break;
                case esriGeometryType.esriGeometryPolyline:
                    spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelEnvelopeIntersects;
                    break;
                case esriGeometryType.esriGeometryPolygon:
                    spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                    break;
            }
            spatialFilter.Geometry = geometry;
            IFeatureClass featureClass = featureLayer.FeatureClass;
            featureCursor = featureClass.Search(spatialFilter, false);
            queryFilterCopy = spatialFilter;
            return featureCursor;
        }
    }
}
