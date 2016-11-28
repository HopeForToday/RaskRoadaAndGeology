using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoadRaskEvaltionSystem
{
    class ShapeSimpleHelper
    {
        public static IWorkspaceFactory pWorkspaceFactory = new ShapefileWorkspaceFactory();
        public static ILayer OpenFile(string fileName)
        {
            IFeatureClass pFeatureClass = OpenFeature(fileName);
            IFeatureLayer pFeatureLayer = new FeatureLayerClass();
            pFeatureLayer.FeatureClass = pFeatureClass;
            pFeatureLayer.Name = pFeatureClass.AliasName;
            ILayer pLayer = pFeatureLayer as ILayer;
            return pLayer;
        }
        public static ILayer OpenFile(string workPath,string fileName)
        {
            IWorkspace pWorkspace = pWorkspaceFactory.OpenFromFile(workPath, 0);
            IFeatureWorkspace pFeatureWorkspace = pWorkspace as IFeatureWorkspace;
            IFeatureClass pFeatureClass = pFeatureWorkspace.OpenFeatureClass(fileName);
            IFeatureLayer pFeatureLayer = new FeatureLayerClass();
            pFeatureLayer.FeatureClass = pFeatureClass;
            pFeatureLayer.Name = pFeatureClass.AliasName;
            ILayer pLayer = pFeatureLayer as ILayer;
            return pLayer;
        }
        public static IFeatureClass OpenFeature(string fileName)
        {
            string path = System.IO.Path.GetDirectoryName(fileName);//路径
            string _name = System.IO.Path.GetFileNameWithoutExtension(fileName);//文件名
            IWorkspace pWorkspace = pWorkspaceFactory.OpenFromFile(path, 0);
            IFeatureWorkspace pFeatureWorkspace = pWorkspace as IFeatureWorkspace;
            IFeatureClass pFeatureClass = pFeatureWorkspace.OpenFeatureClass(_name);
            return pFeatureClass;
        }
    }
}
