using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoadRaskEvaltionSystem.HelperClass
{
 public  class ShapeHelper
    {
        //加载矢量图层
        public static void addShapfileLayer(IMapControl3 map, string shapefilePath)
        {
            string fullPath = shapefilePath;
            //利用"\\"将文件路径分成两部分 
            int Position = fullPath.LastIndexOf("\\");
            //文件目录
            string FilePath = fullPath.Substring(0, Position);
            //
            string ShpName = fullPath.Substring(Position + 1);
            IWorkspaceFactory pWF;
            pWF = new ESRI.ArcGIS.DataSourcesFile.ShapefileWorkspaceFactory();
            IFeatureWorkspace pFWS;
            pFWS = (IFeatureWorkspace)pWF.OpenFromFile(FilePath, 0);
            IFeatureClass pFClass;
            pFClass = pFWS.OpenFeatureClass(ShpName);

            IFeatureLayer pFLayer;
            pFLayer = new FeatureLayer();
            pFLayer.FeatureClass = pFClass;
            pFLayer.Name = pFClass.AliasName;

            map.AddLayer(pFLayer, 0);
            //   MainFrom.m_mapControl.Refresh(esriViewDrawPhase.esriViewGeography, null, null);
            //选择数据源

            //  MainFrom.m_pTocControl.Update();
        }
        public static void ReadAttribute(ILayer layer, string attributeName,ref Dictionary<string, List<string>> aredata)
        {


            IFeatureLayer pFeatureLayer = layer as IFeatureLayer;//定义矢量图层      
          //  string fileName = "NAME";
            IFields fields = pFeatureLayer.FeatureClass.Fields;
            int filedIndex = fields.FindField(attributeName);
            IFeatureCursor pFeatureCursor = pFeatureLayer.Search(null, false);
            IFeature pFeature = pFeatureCursor.NextFeature();
            string[] nn = { "111", "222", "333" };
            while (pFeature!=null)
           {
                string name=pFeature.get_Value(filedIndex).ToString();
                try
                {
                    aredata.Add(name, nn.ToList());
                }
                catch
                {
                    //为了解决县名称出现相同时出现的问题（忽略了） 
                    pFeature = pFeatureCursor.NextFeature();
                    continue;
                }
              
                pFeature = pFeatureCursor.NextFeature();
           }


            //IQueryFilter pQueryFilter = new QueryFilter();//实例化一个查询条件对象            

            // pQueryFilter.WhereClause = "NAME";//将查询条件赋值            

            //IFeatureCursor pFeatureCursor = pFeatureLayer.Search(pQueryFilter, false);//进行查询            

            //IFeature pFeature;

            //pFeature = pFeatureCursor.NextFeature();//此步是将游标中的第一个交给pFeature            

            //if (pFeature == null)//判断是否查到结果            
            //{//如果没有查到报错并结束                

            //    MessageBox.Show("没有查询到地物！", "查询提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

            //    return;

            //}

            //axMapControl1.Map.SelectFeature(pLayer, pFeature);//将查询到的地物作为选择对象高亮显示在地图上           

            //axMapControl1.CenterAt(pFeature.Shape as ESRI.ArcGIS.Geometry.IPoint);//设置当前查询到的要素为地图的中心           

            //axMapControl1.MapScale = pLayer.MinimumScale;//将当前地图的比例尺设置为ILayer的最小显示比例尺           

            //axMapControl1.ActiveView.Refresh();//刷新地图，这样才能显示出地物  

        }


       
    }
}
