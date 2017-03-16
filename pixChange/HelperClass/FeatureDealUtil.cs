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

        public static List<IFeature> FindFeatures(IFeatureLayer layer, List<int> fIds)
        {
            var newFeatures = new List<IFeature>();
            fIds.ForEach(p =>
                {
                    IFeature pFeature = GetFeatureByFID(layer, p);
                    newFeatures.Add(pFeature);
                });
            return newFeatures;
        }
        /// <summary>
        /// 查询图层某一字段的所有唯一值
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static IList<object> GetUnikeValues(IFeatureLayer layer, string fieldName)
        {
            IList<object> values = new List<object>();
            IFeatureCursor pFeatureCursor = QueryFeatureInLayer(layer, "");
            IDataStatistics dataStatistics = new DataStatisticsClass();
            dataStatistics.Cursor = pFeatureCursor as ICursor;
            dataStatistics.Field = fieldName;
            //  IStatisticsResults result = dataStatistics.Statistics;
            IEnumerator myEnumerator = dataStatistics.UniqueValues;
            List<string> myValueList = new List<string>();
            myEnumerator.Reset();
            try
            {
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
      /// <summary>
        ///  查询图层某一字段的所有唯一值 带最多个数限制
      /// </summary>
      /// <param name="layer"></param>
      /// <param name="fieldName"></param>
      /// <param name="maxCount"></param>
      /// <returns></returns>
        public static IList<object> GetUnikeValues(IFeatureLayer layer, string fieldName,int maxCount)
        {
            IList<object> values = new List<object>();
            IFeatureCursor pFeatureCursor = QueryFeatureInLayer(layer, "");
            IDataStatistics dataStatistics = new DataStatisticsClass();
            dataStatistics.Cursor = pFeatureCursor as ICursor;
            dataStatistics.Field = fieldName;
            //  IStatisticsResults result = dataStatistics.Statistics;
            IEnumerator myEnumerator = dataStatistics.UniqueValues;
            List<string> myValueList = new List<string>();
            int count = 1;
            myEnumerator.Reset();
            try
            {
                while (myEnumerator.MoveNext())
                {
                    count++;
                    if (count > maxCount)
                    {
                        break;
                    }
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
        public static IFeature GetFeatureByFID(IFeatureLayer layer, int id)
        {
            IFeatureCursor featureCursor = null;
            IQueryFilter2 queryFilter = new QueryFilterClass();
            queryFilter.WhereClause = string.Format("FID = {0}", id);
            featureCursor = layer.Search(queryFilter, false);
            IFeature pFeature = featureCursor.NextFeature();
            return pFeature;
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
        /// <summary>
        /// 更新要素
        /// </summary>
        /// <param name="pfeatuers"></param>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static bool UpdateFeature(IFeature pFeature, DataRow dRow)
        {
            bool isUpdate = false;
            for (int j = 0; j < pFeature.Fields.FieldCount; j++)
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
                object value = dRow[pField.Name];
                if (pFeature.get_Value(j) != value)
                {
                    isUpdate = true;
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
            return true;
        }
        /// <summary>
        /// 更新要素
        /// </summary>
        /// <param name="pfeatuers"></param>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static bool UpdateFeature(IList<IFeature> pfeatuers, DataTable dataTable)
        {
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                IFeature pFeature = pfeatuers[i];
                DataRow dRow = dataTable.Rows[i];
                bool isUpdate = false;
                for (int j = 0; j < pFeature.Fields.FieldCount; j++)
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
                    object value = dRow[pField.Name];
                    if (pFeature.get_Value(j) != value)
                    {
                        isUpdate = true;
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
        /// 删除要素
        /// </summary>
        /// <param name="features"></param>
        /// <returns></returns>
        public static bool DeleteFeatures(List<IFeature> features)
        {
            features.ForEach(p =>
                p.Delete());
            return true;
        }
        /// <summary>
        /// 删除要素类中符合条件的元素
        /// </summary>
        /// <param name="pFeatureClass"></param>
        /// <param name="whereClause"></param>
        /// <param name="pGeometry"></param>
        public static void DeleteAllFeature(IFeatureClass pFeatureClass, string whereClause, IGeometry pGeometry)
        {
            ISpatialFilter pSpatialFilter = new SpatialFilterClass();
            pSpatialFilter.WhereClause = whereClause;
            if (pGeometry != null)
            {
                pSpatialFilter.Geometry = pGeometry;
            }
            IFeatureCursor pCursor = pFeatureClass.Update(pSpatialFilter as IQueryFilter, false);
            IFeature pFeature = pCursor.NextFeature();
            while (pFeature != null)
            {
                pCursor.DeleteFeature();
                pFeature = pCursor.NextFeature();
            }
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
