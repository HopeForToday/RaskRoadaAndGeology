using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
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
        public static ILayer QueryLayerInMap(AxMapControl mapControl, string layerName, ref IGroupLayer gLayer)
        {
            ILayer queryLayer = null;
            for (int i = 0; i < mapControl.Map.LayerCount; i++)
            {
                ILayer tempLayer = mapControl.Map.get_Layer(i);
                if (tempLayer is IGroupLayer)
                {
                    IGroupLayer tempGlayer = tempLayer as IGroupLayer;
                    ILayer querylayer2 = QueryLayerInGroupLayer(layerName, tempGlayer);
                    if (querylayer2 != null)
                    {
                        queryLayer = querylayer2;
                        gLayer = tempGlayer;
                        break;
                    }
                }
                else
                {
                    if (tempLayer.Name == layerName)
                    {
                        queryLayer = tempLayer;
                        break;
                    }
                }
            }
            return queryLayer;
        }
        /// <summary>
        /// 在组合图层中查找对应图层
        /// </summary>
        /// <param name="layerName"></param>
        /// <param name="groupLayer"></param>
        /// <returns></returns>
         public static ILayer QueryLayerInGroupLayer(string layerName, IGroupLayer groupLayer)
        {
            ILayer queryLayer = null;
            ICompositeLayer compositeLayer=groupLayer as ICompositeLayer;
            for (int i = 0; i < compositeLayer.Count; i++)
            {
                ILayer tempLayer = compositeLayer.get_Layer(i);
                if (tempLayer.Name == layerName)
                {
                    queryLayer = tempLayer;
                    break;
                }
            }
            return queryLayer;
        }

    }
}
