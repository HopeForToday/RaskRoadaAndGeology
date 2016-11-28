using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoadRaskEvaltionSystem.HelperClass
{
    class RasterSimpleHelper
    {
        public static IWorkspaceFactory rWorkspaceFactory = new RasterWorkspaceFactory();
        public static ILayer OpenRasterFile(string fullFilePath)
        {
            String path = System.IO.Path.GetDirectoryName(fullFilePath);//路径
            String _name = System.IO.Path.GetFileName(fullFilePath);//文件名
            IWorkspace myWorkspace = rWorkspaceFactory.OpenFromFile(path, 0);
            IRasterWorkspace rasterWorkspace = myWorkspace as IRasterWorkspace;
            IRasterDataset rasterDataset = new RasterDatasetClass();
            IRasterLayer rasterLayer = new RasterLayerClass();
            rasterDataset = rasterWorkspace.OpenRasterDataset(_name);
            rasterLayer.CreateFromDataset(rasterDataset);
            return rasterLayer;
        }
        public static ILayer OpenRasterFile(string spacePath,string fileName)
        {
            IWorkspace myWorkspace = rWorkspaceFactory.OpenFromFile(spacePath, 0);
            IRasterWorkspace rasterWorkspace = myWorkspace as IRasterWorkspace;
            IRasterDataset rasterDataset = new RasterDatasetClass();
            IRasterLayer rasterLayer = new RasterLayerClass();
            rasterDataset = rasterWorkspace.OpenRasterDataset(fileName);
            rasterLayer.CreateFromDataset(rasterDataset);
            return rasterLayer;
        }
    }
}
