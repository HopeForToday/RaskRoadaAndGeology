﻿using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadRaskEvaltionSystem.HelperClass
{
    /// <summary>
    /// 属性表帮助类
    /// 2016/12/11 fhr
    /// </summary>
    class AtrributeUtil
    {
        /// <summary>
        /// 将要素集合转换为datable
        /// </summary>
        /// <param name="?"></param>
        /// <param name="featurers"></param>
        /// <returns></returns>
        public static DataTable GetDataTable(IFeatureLayer featureLayer, IList<IFeature> featurers)
        {
            if (featurers == null || featurers.Count < 1)
            {
                return null;
            }
            DataTable dataTable = GetDataTableStyle(featurers[0]);
            foreach (var pfeature in featurers)
            {
                AddNewRowByFeature(featureLayer, dataTable, pfeature);
            }
            return dataTable;
        }
        /// <summary>
        /// 根据图层获取所有要素 返回一个DataTable
        /// </summary>
        /// <param name="featureLayer"></param>
        /// <returns></returns>
        public static DataTable GetDataTable(IFeatureLayer featureLayer)
        {
            DataTable dataTable = GetDataTableStyle(featureLayer);
            IFeatureCursor cursor = featureLayer.Search(null, false);
            IFeature pFeature = cursor.NextFeature();
            while (pFeature != null)
            {
                AddNewRowByFeature(featureLayer, dataTable, pFeature);
                pFeature = cursor.NextFeature();
            }
            return dataTable;
        }
        /// <summary>
        /// 根据图层和游标获取要素 返回一个DataTable
        /// </summary>
        /// <param name="featureLayer"></param>
        /// <param name="cursor"></param>
        /// <returns></returns>
        public static DataTable GetDataTable(IFeatureLayer featureLayer, IFeatureCursor cursor)
        {
            DataTable dataTable = GetDataTableStyle(featureLayer);
            IFeature feature = cursor.NextFeature();
            while (feature != null)
            {
                AddNewRowByFeature(featureLayer, dataTable, feature);
                feature = cursor.NextFeature();
            }
            return dataTable;
        }
        /// <summary>
        /// 添加一行
        /// </summary>
        /// <param name="featureLayer"></param>
        /// <param name="dataTable"></param>
        /// <param name="feature"></param>
        private static void AddNewRowByFeature(IFeatureLayer featureLayer, DataTable dataTable, IFeature feature)
        {
            DataRow dRow = dataTable.NewRow();
            for (int i = 0; i < feature.Fields.FieldCount; i++)
            {
                IField field = feature.Fields.get_Field(i);
                if (field.Type == esriFieldType.esriFieldTypeGeometry)
                {
                    dRow[i] = GetShapeType(featureLayer.FeatureClass);
                }
                else if (field.Type == esriFieldType.esriFieldTypeBlob)
                {
                    dRow[i] = "Element";
                }
                else
                {
                    dRow[i] = feature.get_Value(i);
                }
            }
            dataTable.Rows.Add(dRow);
        }
        /// <summary>
        /// 根据图层设置Datable样式
        /// </summary>
        /// <param name="featureLayer"></param>
        /// <returns></returns>
        public static DataTable GetDataTableStyle(IFeatureLayer featureLayer)
        {
            DataTable dataTable = new DataTable(featureLayer.Name);
            ITable table = featureLayer.FeatureClass as ITable;
            for (int i = 0; i < table.Fields.FieldCount; i++)
            {
                DataColumn column = new DataColumn(table.Fields.get_Field(i).Name);
                dataTable.Columns.Add(column);
            }
            return dataTable;
        }
        /// <summary>
        /// 根据要素设置Datable样式
        /// </summary>
        /// <param name="featureLayer"></param>
        /// <returns></returns>
        public static DataTable GetDataTableStyle(IFeature feature)
        {
            DataTable dataTable = new DataTable("属性表");
            for (int i = 0; i < feature.Fields.FieldCount; i++)
            {
                DataColumn column = new DataColumn(feature.Fields.get_Field(i).Name);
                dataTable.Columns.Add(column);
            }
            return dataTable;
        }
        /// <summary>
        /// 获取要素类的几何
        /// </summary>
        /// <param name="featureClass"></param>
        /// <returns></returns>
        public static String GetShapeType(IFeatureClass featureClass)
        {
            esriGeometryType type = featureClass.ShapeType;
            switch (type)
            {
                case esriGeometryType.esriGeometryPolyline:
                    return "Polyline";
                case esriGeometryType.esriGeometryPoint:
                    return "Point";
                case esriGeometryType.esriGeometryPolygon:
                    return "Polygon";
                default:
                    return "Geometry";
            }
        }
        /// <summary>
        /// 转换字段
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static String ParseEsriField(esriFieldType type)
        {
            switch (type)
            {
                case esriFieldType.esriFieldTypeBlob:
                    return "System.String";
                case esriFieldType.esriFieldTypeDouble:
                    return "System.Double";
                case esriFieldType.esriFieldTypeDate:
                    return "System.Date";
                case esriFieldType.esriFieldTypeGeometry:
                    return "System.String";
                case esriFieldType.esriFieldTypeInteger:
                    return "System.Int32";
                case esriFieldType.esriFieldTypeSmallInteger:
                    return "System.Int32";
                case esriFieldType.esriFieldTypeSingle:
                    return "System.Int32";
                default:
                    return "System.String";
            }
        }
    }
}
