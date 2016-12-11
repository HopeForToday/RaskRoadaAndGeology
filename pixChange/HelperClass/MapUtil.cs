using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadRaskEvaltionSystem.HelperClass
{
    /// <summary>
    /// 地图帮助类
    /// 2016/12/11 fhr 
    /// </summary>
    class MapUtil
    {
        /// <summary>
        /// 高亮要素
        /// </summary>
        /// <param name="mapControl"></param>
        /// <param name="pFeatures"></param>
        /// <param name="symbol"></param>
        public static void FlashFeature(AxMapControl mapControl, IList<IFeature> pFeatures, ISymbol symbol)
        {
           foreach(var value in pFeatures)
           {
               FlashFeature(mapControl, value, symbol);
           }
        }
        /// <summary>
        /// 高亮要素
        /// </summary>
        /// <param name="mapControl"></param>
        /// <param name="pFeature"></param>
        /// <param name="symbol"></param>
        public static void FlashFeature(AxMapControl mapControl,IFeature pFeature,ISymbol symbol)
        {
            if (symbol != null)
            {
                mapControl.FlashShape(pFeature.Shape, 5, 0, symbol);
            }
            else
            {
                mapControl.FlashShape(pFeature.Shape);
            }
       // mapControl.Map.SelectFeature(pFeature);
        }
        /// <summary>
        /// 高亮要素
        /// </summary>
        /// <param name="mapControl"></param>
        /// <param name="pFeatureCursor"></param>
        /// <param name="symbol"></param>
        public static void FlashFeature(AxMapControl mapControl, IFeatureCursor pFeatureCursor, ISymbol symbol)
        {
            IFeature pFeature = pFeatureCursor.NextFeature();
            while (pFeature != null)
            {
                FlashFeature(mapControl, pFeature, symbol);
                pFeature = pFeatureCursor.NextFeature();
            }
        }
        /// <summary>
        /// 加载地图mxd
        /// </summary>
        /// <param name="mapControl"></param>
        /// <param name="mapFileName"></param>
        /// <returns></returns>
        public static bool LoadMxd(AxMapControl mapControl, string mapFileName)
        {
            if (mapControl.CheckMxFile(mapFileName))
            {
                mapControl.LoadMxFile(mapFileName);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 保存地图mxd
        /// </summary>
        /// <param name="mapControl"></param>
        /// <returns></returns>
        public static bool SaveMxd(AxMapControl mapControl)
        {
            if (mapControl.DocumentFilename != null)
            {
                IMapDocument mapDoc = new MapDocumentClass();
                mapDoc.Open(mapControl.DocumentFilename, string.Empty);
                mapDoc.ReplaceContents((IMxdContents)mapControl.Map);
                mapDoc.Save(mapDoc.UsesRelativePaths,false);
                mapDoc.Close();
                return true;
            }
            return false;
        }
        /// <summary>
        /// 打开本地地图  
        /// </summary>
        /// <param name="mapFileName"></param>
        /// <returns></returns>
        public static  IMap OpenMap(string mapFileName)
        {
            IMapDocument mapDoc = new MapDocumentClass();
            mapDoc.Open(mapFileName, "");
            return mapDoc.get_Map(0);
        }
        /// <summary>
        /// 保存地图
        /// </summary>
        /// <param name="mapFileName"></param>
        /// <param name="map"></param>
        public static void SaveMap(string mapFileName,IMap map)
        {
            try
            {
                IMapDocument mapDoc = new MapDocumentClass();
                mapDoc.Open(mapFileName, "");
                mapDoc.ReplaceContents(map as IMxdContents);
                mapDoc.Save(mapDoc.UsesRelativePaths, false);
                mapDoc.Close();
            }
            catch (Exception e)
            {

            }
        }
      /// <summary>
     /// 获取所有图层 不包括组合图层
      /// </summary>
      /// <param name="mapControl"></param>
      /// <returns></returns>
        public static List<ILayer> GetAllLayers(AxMapControl mapControl)
        {
            List<ILayer> layers = new List<ILayer>();
            for (int i = 0; i < mapControl.LayerCount; i++)
            {
                ILayer layer = mapControl.get_Layer(i);
                if (layer == null) continue;
                IGroupLayer groupLayer = layer as IGroupLayer;
                if (groupLayer != null)
                {
                    List<ILayer> tempLayers = GetGroupLayers(groupLayer);
                    tempLayers.ForEach(p => layers.Add(p));
                }
                else
                {
                    layers.Add(layer);
                }
            }
            return layers;
        }
        /// <summary>
        /// 获取组合图层的子图层
        /// </summary>
        /// <param name="groupLayer"></param>
        /// <returns></returns>
        private static List<ILayer>  GetGroupLayers(IGroupLayer groupLayer)
        {
            List<ILayer> layers=new List<ILayer>();
            ICompositeLayer compositeLayer = groupLayer as ICompositeLayer;
            for (int j = 0; j < compositeLayer.Count; j++)
            {
                ILayer childLayer = compositeLayer.get_Layer(j);
                if (childLayer != null)
                {
                    layers.Add(childLayer);
                }
            }
            return layers;
        }
    }
}
