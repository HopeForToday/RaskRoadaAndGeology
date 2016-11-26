using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadRaskEvaltionSystem.HelperClass
{
    /// <summary>
    /// 带阈值的装饰要素类 2016/11/26 fhr
    /// </summary>
    public class DecorateRouteFeatureClass
    {
        /// <summary>
        /// 阈值
        /// </summary>
        public double SnapTolerance { get; set; }
        /// <summary>
        /// 要素类
        /// </summary>
        public IFeatureClass FeatureClass { get; set; }
        public DecorateRouteFeatureClass(double snap, IFeatureClass featureClass)
        {
            this.SnapTolerance = snap;
            this.FeatureClass = featureClass;
        }
    }
}
