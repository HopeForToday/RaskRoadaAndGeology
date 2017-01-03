using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadRaskEvaltionSystem.HelperClass
{
    /// <summary>
    /// 空间查询与操作帮助类
    /// 2015/12/11 fhr
    /// </summary>
    class FeatureDealUtil
    {
        public static IList<object> GetUniqueValues(IFeatureLayer layer, string fieldName)
        {
            IList<object> values = new List<object>();
            IFeatureCursor pFeatureCursor = QueryFeatureInLayer(layer, "");
            IDataStatistics dataStatistics = new DataStatisticsClass();
            dataStatistics.Cursor = pFeatureCursor as ICursor;
            dataStatistics.Field = fieldName;
            //  IStatisticsResults result = dataStatistics.Statistics;
            try
            {
                IEnumerator myEnumerator = dataStatistics.UniqueValues;
                List<string> myValueList = new List<string>();
                myEnumerator.Reset();
                while (myEnumerator.MoveNext())
                {
                    if (myEnumerator.Current != null)
                    {
                        values.Add(myEnumerator.Current.ToString());
                    }
                }
            }
            catch (Exception e)
            {

            }
            //     IStatisticsResults  result= dataStatistics.Statistics;
            return values;
        }

        private static IFeatureCursor QueryFeatureInLayer(IFeatureLayer layer, string whereClause)
        {
            IFeatureCursor featureCursor = null;
            IQueryFilter2 queryFilter = new QueryFilterClass();
            queryFilter.WhereClause = whereClause;
            IFeatureClass featureClass = layer.FeatureClass;
            featureCursor = featureClass.Search(queryFilter, false);
            return featureCursor;
        }
        public static bool UpdateFeature(IList<IFeature> pfeatuers, DataTable dataTable)
        {
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                IFeature pFeature = pfeatuers[i];
                DataRow dRow = dataTable.Rows[i];
                bool isUpdate = false;
                for (int j = 0; j < dRow.ItemArray.Count(); j++)
                {
                    IField pField = pFeature.Fields.get_Field(j);
                    if (!pField.Editable)
                    {
                        continue;
                    }
                    if (pField.Type == esriFieldType.esriFieldTypeBlob || pField.Type == esriFieldType.esriFieldTypeRaster || pField.Type == esriFieldType.esriFieldTypeGeometry)
                    {
                        continue;
                    }
                    if (pFeature.get_Value(j) != dRow[j])
                    {
                        isUpdate = true;
                        object value=dRow[j];
                        if (pFeature.Fields.get_Field(j).CheckValue(value))
                        {
                            pFeature.set_Value(j, value);
                        }
                    }
                }
                if (isUpdate)
                {
                    pFeature.Store();
                }
            }
            return true;
        }
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
                    spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
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
