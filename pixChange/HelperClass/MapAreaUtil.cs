using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadRaskEvaltionSystem.HelperClass
{
   public   class MapAreaUtil
    {
       /// <summary>
       /// 利用最大感兴趣缩放至图层
       /// </summary>
       /// <param name="mapControl"></param>
       public static  void ZoomToByMaxLayer(AxMapControl mapControl)
       {
           mapControl.Extent = GetMaxEnvelope(mapControl);
           mapControl.Refresh();
       }
       /// <summary>
       /// 获取地图中最大感兴趣面积
       /// </summary>
       /// <param name="mapControl"></param>
       /// <returns></returns>
       public static IEnvelope GetMaxEnvelope(AxMapControl mapControl)
       {
           double maxArea = -1;
           IEnvelope pEnvelope = null;
           for (int i = 0; i < mapControl.LayerCount; i++)
           {
               ILayer layer = mapControl.get_Layer(i);
               if (layer == null || layer.AreaOfInterest == null)
               {
                   continue;
               }
               double area = getLayerAreaEnvelop(layer);
               if (maxArea < area)
               {
                   pEnvelope = layer.AreaOfInterest;
                   maxArea = area;
               }
           }
           return pEnvelope;
       }
       /// <summary>
       /// 获取图层感兴趣区的面积
       /// </summary>
       /// <param name="layer"></param>
       /// <returns></returns>
       public static double getLayerAreaEnvelop(ILayer layer)
       {
           IEnvelope pEnvelope = layer.AreaOfInterest;
           return pEnvelope.Height * pEnvelope.Width;
       }
    }
}
