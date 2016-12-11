using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadRaskEvaltionSystem.QueryAndUIDeal
{
    /// <summary>
    /// 空间查询接口
    /// 2016/12/11 fhr
    /// </summary>
    public interface ISpatialQueryUI
    {
        /// <summary>
        /// 处理空间查询
        /// </summary>
        /// <param name="mapControl"></param>
        /// <param name="pGeometry"></param>
        /// <param name="layerName"></param>
         void DealFeatureQuery(AxMapControl mapControl, IGeometry pGeometry, string layerName);
    }
}
