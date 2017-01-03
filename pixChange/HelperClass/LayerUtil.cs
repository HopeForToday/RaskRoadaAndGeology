using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadRaskEvaltionSystem.HelperClass
{
   public class LayerUtil
    {
        /// <summary>
        /// 图层查找 确定图层的存在性 不包括组合图层
        /// </summary>
        /// <param name="layerName"></param>
        /// <returns></returns>
        public static ILayer QueryLayerInMap(AxMapControl mapControl,string layerName)
        {
            ILayer queryLayer = null;
            int layerCount = mapControl.Map.LayerCount;
            for (int i = 0; i < layerCount; i++)
            {
                ILayer tempLayer = mapControl.Map.get_Layer(i);
                if (tempLayer.Name == layerName)
                {
                    queryLayer = tempLayer;
                    break;
                }
            }
            return queryLayer;
        }
        /// <summary>
        /// 图层查找 确定图层的存在性 包括组合图层
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="layerName"></param>
        /// <param name="gLayer"></param>
        /// <returns></returns>
        public static ILayer QueryLayerInMap(AxMapControl mapControl, string layerName, ref IGroupLayer gLayer, out int layerIndex, out int groupIndex)
        {
            layerIndex = -1;
            groupIndex = -1;
            ILayer queryLayer = null;
            for (int i = 0; i < mapControl.Map.LayerCount; i++)
            {
                ILayer tempLayer = mapControl.Map.get_Layer(i);
                if (tempLayer is IGroupLayer)
                {
                    IGroupLayer tempGlayer = tempLayer as IGroupLayer;
                    ILayer querylayer2 = QueryLayerInGroupLayer(layerName, tempGlayer, ref layerIndex);
                    if (querylayer2 != null)
                    {
                        queryLayer = querylayer2;
                        gLayer = tempGlayer;
                        groupIndex = i;
                        break;
                    }
                }
                if (tempLayer.Name == layerName)
                {
                    queryLayer = tempLayer;
                    layerIndex = i;
                    break;
                }
            }
            return queryLayer;
        }
      /// <summary>
      /// 在组合图层中查找对应图层
      /// </summary>
      /// <param name="layerName"></param>
      /// <param name="groupLayer"></param>
      /// <param name="index"></param>
      /// <returns></returns>
         public static ILayer QueryLayerInGroupLayer(string layerName, IGroupLayer groupLayer,ref int index)
        {
             index=-1;
            ILayer queryLayer = null;
            ICompositeLayer compositeLayer=groupLayer as ICompositeLayer;
            for (int i = 0; i < compositeLayer.Count; i++)
            {
                ILayer tempLayer = compositeLayer.get_Layer(i);
                if (tempLayer.Name == layerName)
                {
                    queryLayer = tempLayer;
                    index=i;
                    break;
                }
            }
            return queryLayer;
        }
       /// <summary>
       /// 获取图层所有字段
       /// </summary>
       /// <param name="layer"></param>
       /// <returns></returns>
         public static List<IField> GetLayerFields(IFeatureLayer layer)
         {
             List<IField> fields = new List<IField>();
             for (int i = 0; i < layer.FeatureClass.Fields.FieldCount; i++)
             {
                 fields.Add(layer.FeatureClass.Fields.get_Field(i));
             }
             return fields;
         }
         /// <summary>
         /// 查找图层 包括组合图层
         /// </summary>
         /// <param name="groupName"></param>
         /// <param name="layerName"></param>
         /// <param name="gLayer"></param>
         /// <returns></returns>
         public static bool QueryLayerInMap(AxMapControl mapControl, ILayer searchLayer , ref IGroupLayer gLayer, out int layerIndex, out int groupIndex)
         {
             layerIndex = -1;
             groupIndex = -1;
             ILayer queryLayer = null;
             for (int i = 0; i < mapControl.Map.LayerCount; i++)
             {
                 ILayer tempLayer = mapControl.Map.get_Layer(i);
                 if (tempLayer is IGroupLayer)
                 {
                     IGroupLayer tempGlayer = tempLayer as IGroupLayer;
                     if (QueryLayerInGroupLayer(searchLayer, tempGlayer, ref layerIndex))
                     {
                         gLayer = tempGlayer;
                         groupIndex = i;
                         return true;
                     }
                 }
                 if (tempLayer == searchLayer)
                 {
                     queryLayer = tempLayer;
                     layerIndex = i;
                     return true;
                 }
             }
             return false;
         }
         /// <summary>
         /// 查找图层 包括组合图层
         /// </summary>
         /// <param name="groupName"></param>
         /// <param name="layerName"></param>
         /// <param name="gLayer"></param>
         /// <returns></returns>
         public static bool QueryLayerInMap(IMapControl3 mapControl, ILayer searchLayer, ref IGroupLayer gLayer, out int layerIndex, out int groupIndex)
         {
             layerIndex = -1;
             groupIndex = -1;
             ILayer queryLayer = null;
             for (int i = 0; i < mapControl.Map.LayerCount; i++)
             {
                 ILayer tempLayer = mapControl.Map.get_Layer(i);
                 if (tempLayer is IGroupLayer)
                 {
                     IGroupLayer tempGlayer = tempLayer as IGroupLayer;
                     if (QueryLayerInGroupLayer(searchLayer, tempGlayer, ref layerIndex))
                     {
                         gLayer = tempGlayer;
                         groupIndex = i;
                         return true;
                     }
                 }
                 if (tempLayer == searchLayer)
                 {
                     queryLayer = tempLayer;
                     layerIndex = i;
                     return true;
                 }
             }
             return false;
         }
         /// <summary>
         /// 在组合图层中查找图层
         /// </summary>
         /// <param name="layerName"></param>
         /// <param name="groupLayer"></param>
         /// <param name="index"></param>
         /// <returns></returns>
         public static bool QueryLayerInGroupLayer(ILayer searchLayer, IGroupLayer groupLayer, ref int index)
         {
             index = -1;
             ILayer queryLayer = null;
             ICompositeLayer compositeLayer = groupLayer as ICompositeLayer;
             for (int i = 0; i < compositeLayer.Count; i++)
             {
                 ILayer tempLayer = compositeLayer.get_Layer(i);
                 if (tempLayer == searchLayer)
                 {
                     queryLayer = tempLayer;
                     index = i;
                     return true;
                 }
             }
             return false;
         }
    }
}
