using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
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
    /// 要素类简单帮助类
    /// 2016//11/20 FHR
    /// </summary>
    class FeatureClassUtil
    {
        /// <summary>
        /// 根据字段集合创建一个内存要素类
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="shapeFiledName"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public static IFeatureClass CreateMemoryFeatureClass(IFields fields,string shapeFiledName,string className)
        {
            // 创建内存工作空间
            IWorkspaceFactory pWSF = new InMemoryWorkspaceFactoryClass();
            IWorkspaceName pWSName = pWSF.Create("", "Temp", null, 0);
            IName pName = (IName)pWSName;
            IWorkspace memoryWS = (IWorkspace)pName.Open();
            //创建要素类
            IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)memoryWS;
            IFeatureClass featureClass = featureWorkspace.CreateFeatureClass(
                className, fields, null, null, esriFeatureType.esriFTSimple, shapeFiledName, "");
            return featureClass;
        }
        /// <summary>
        /// 创建一个只包含几何字段的简单内存要素类
        /// </summary>
        /// <param name="geometryType"></param>
        /// <param name="spatialReference"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public static IFeatureClass CreateMemorySimpleFeatureClass(esriGeometryType geometryType,ISpatialReference spatialReference, string className)
        {
            // 创建内存工作空间
            IWorkspaceFactory pWSF = new InMemoryWorkspaceFactoryClass();
            IWorkspaceName pWSName = pWSF.Create("", "Temp", null, 0);
            IName pName = (IName)pWSName;
            IWorkspace memoryWS = (IWorkspace)pName.Open();
            //创建字段集
            IField field = new FieldClass();
            IFields fields = new FieldsClass();
            IFieldsEdit fieldsEdit = fields as IFieldsEdit;
            IFieldEdit fieldEdit = field as IFieldEdit;
            //创建图形字段
            field = new FieldClass();
            fieldEdit = field as IFieldEdit;
            IGeometryDef geoDef = new GeometryDefClass();
            IGeometryDefEdit geoDefEdit = (IGeometryDefEdit)geoDef;
            geoDefEdit.AvgNumPoints_2 = 5;
            geoDefEdit.GeometryType_2 = geometryType;
            geoDefEdit.GridCount_2 = 1;
            geoDefEdit.HasM_2 = false;
            geoDefEdit.HasZ_2 = false;
            geoDefEdit.SpatialReference_2 = spatialReference;
            fieldEdit.Name_2 = "SHAPE";
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeGeometry;
            fieldEdit.GeometryDef_2 = geoDef;
            fieldEdit.IsNullable_2 = true;
            fieldEdit.Required_2 = true;
            fieldsEdit.AddField(field);
            //创建要素类
            IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)memoryWS;
            IFeatureClass featureClass = featureWorkspace.CreateFeatureClass(
                className, fields, null, null, esriFeatureType.esriFTSimple, "SHAPE", "");
            return featureClass;
        }
      /// <summary>
      /// 复制字段
      /// </summary>
      /// <param name="originField"></param>
      /// <returns></returns>
        public static IField CloneField(IField originField)
        {
            IField field = new FieldClass();
            IFieldEdit fieldEdit = field as IFieldEdit;
            fieldEdit.AliasName_2 = originField.AliasName;
            fieldEdit.DefaultValue_2 = originField.DefaultValue;
            fieldEdit.Domain_2 = originField.Domain;
            fieldEdit.DomainFixed_2 = originField.DomainFixed;
            fieldEdit.Editable_2 = originField.Editable;
            fieldEdit.GeometryDef_2 = originField.GeometryDef;
            fieldEdit.Type_2 = originField.Type;
            fieldEdit.Scale_2 = originField.Scale;
            fieldEdit.Name_2 = originField.Name;
          //  fieldEdit.
            return field;
        }
        /// <summary>
        /// 简单的插入只包含几何信息的要素
        /// 注意在适当情况使用
        /// </summary>
        /// <param name="pFeature"></param>
        /// <param name="pFeatureClass"></param>
        public static void InsertSimpleFeature(IGeometry shape,IFeatureClass pFeatureClass)
        {
            IFeature newLineFeature = pFeatureClass.CreateFeature();
            newLineFeature.Shape = shape;
            newLineFeature.Store();
        }
        /// <summary>
        /// 删除要素类中符合条件的元素
        /// </summary>
        /// <param name="pFeatureClass"></param>
        /// <param name="whereClause"></param>
        /// <param name="pGeometry"></param>
        public static void DeleteAllFeature(IFeatureClass pFeatureClass,string whereClause,IGeometry pGeometry)
        {
            ISpatialFilter pSpatialFilter = new SpatialFilterClass();
            pSpatialFilter.WhereClause = whereClause;
            if(pGeometry!=null)
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
    }
}
