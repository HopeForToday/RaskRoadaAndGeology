using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace RoadRaskEvaltionSystem.HelperClass
{
    class TOCControlUtil
    {
        /// <summary>
        /// 移动图层 通用方法
        /// </summary>
        /// <param name="mapControl"></param>
        /// <param name="removedLayer"></param>
        /// <param name="fromGroupLayer"></param>
        /// <param name="toGroupLayer"></param>
        /// <param name="toIndexObj"></param>
        public static void RemoveLayer(AxMapControl mapControl, ILayer removedLayer, IGroupLayer fromGroupLayer, IGroupLayer toGroupLayer, object toIndexObj)
        {
            if (removedLayer == null)
            {
                return;
            }
            IMap pMap = mapControl.Map;
            //如果是在一个组合图层中进行移动的情况
            if (fromGroupLayer == toGroupLayer && fromGroupLayer != null)
            {
                int fromIndex = -1;
                if (LayerUtil.QueryLayerInGroupLayer(removedLayer, fromGroupLayer, ref fromIndex))
                {
                    MoveInSameGroupLayer(removedLayer, fromGroupLayer, (object)fromIndex, toIndexObj);
                }
                return;
            }
            //如果含有父图层
            //则父图层中移除组合图层
            if (fromGroupLayer != null)
            {
                fromGroupLayer.Delete(removedLayer);
            }
            else
            {
                pMap.DeleteLayer(removedLayer);
            }
            //如果不移动到组合图层
            if (toGroupLayer == null)
            {
                MoveLayerToMapIndex(pMap, removedLayer, toIndexObj);
            }
            //如果是移动到某个组合图层
            else
            {
                if (toIndexObj == null)
                {
                    toGroupLayer.Add(removedLayer);
                    return;
                }
                MoveLayerToGroupIndex(removedLayer, toGroupLayer, (int)toIndexObj);
            }
        }
        /// <summary>
        /// 在组合图层内部进行移动
        /// </summary>
        /// <param name="removedLayer"></param>
        /// <param name="groupLayer"></param>
        /// <param name="fromIndexObj"></param>
        /// <param name="toIndexObj"></param>
        private static void MoveInSameGroupLayer(ILayer removedLayer, IGroupLayer groupLayer, object fromIndexObj, object toIndexObj)
        {
            if (toIndexObj == null)
            {
                groupLayer.Delete(removedLayer);
                AddMyLayer(groupLayer, removedLayer);
                return;
            }
            int fromIndex = (int)fromIndexObj;
            int toIndex = (int)toIndexObj;
            if (fromIndex == toIndex)
            {
                return;
            }
            if (fromIndex < toIndex)
            {
                groupLayer.Delete(removedLayer);
              //  toIndex--;
            }
            //else
            //{
            //    toIndex--;
            //    groupLayer.Delete(removedLayer);
            //}
            MoveLayerToGroupIndex(removedLayer, groupLayer, toIndex);
        }
        /// <summary>
        /// 移动图层到组合图层的对应位置 单纯移除
        /// </summary>
        /// <param name="removedLayer"></param>
        /// <param name="toGroupLayer"></param>
        /// <param name="toIndex"></param>
        private static void MoveLayerToGroupIndex(ILayer removedLayer, IGroupLayer toGroupLayer, int toIndex)
        {
            ICompositeLayer pComposite = toGroupLayer as ICompositeLayer;
            IList<ILayer> tempLayers = new List<ILayer>();
            //移除toIndex后面的所有图层
            for (int i = toIndex; i <pComposite.Count; i++)
            {
                ILayer tempLayer = pComposite.get_Layer(i);
                tempLayers.Add(tempLayer);
            }
            foreach (var layer in tempLayers)
            {
                toGroupLayer.Delete(layer);
            }
            //添加待移除图层
            toGroupLayer.Add(removedLayer);
            //添加图层
            foreach (var layer in tempLayers)
            {
                if (layer != removedLayer)
                {
                    toGroupLayer.Add(layer);
                }
            }
        }
        /// <summary>
        /// 将图层移动到地图控件的某个位置
        /// </summary>
        /// <param name="pMap"></param>
        /// <param name="removedLayer"></param>
        /// <param name="toIndex"></param>
        private static void MoveLayerToMapIndex(IMap pMap, ILayer removedLayer, object toIndex)
        {
            if (toIndex == null)
            {
                pMap.AddLayer(removedLayer);
                return;
            }
            int index = (int)toIndex;
            try
            {
                pMap.MoveLayer(removedLayer, index);
            }
            catch (Exception e)
            {
                IList<ILayer> tempLayers = new List<ILayer>();
                for (int i = 0; i < index; i++)
                {
                    tempLayers.Add(pMap.get_Layer(i));
                }
                foreach (var layer in tempLayers)
                {
                    pMap.DeleteLayer(layer);
                }
                pMap.AddLayer(removedLayer);
                foreach (var layer in tempLayers)
                {
                    pMap.AddLayer(layer);
                }
            }
        }
        public static void AddMyLayer(IGroupLayer groupLayer, ILayer layer)
        {
            IList<ILayer> layers = ClearAllLayer(groupLayer);
            groupLayer.Add(layer);
            foreach (var value in layers)
            {
                groupLayer.Add(value);
            }
        }
        public static IList<ILayer> ClearAllLayer(IGroupLayer groupLayer)
        {
            IList<ILayer> layers = new List<ILayer>();
            ICompositeLayer pCompositeLayer = groupLayer as ICompositeLayer;
            for (int i = 0; i < pCompositeLayer.Count; i++)
            {
                layers.Add(pCompositeLayer.get_Layer(i));
            }
            groupLayer.Clear();
            return layers;
        }
    }
}