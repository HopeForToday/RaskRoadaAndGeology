using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RoadRaskEvaltionSystem
{
    /// <summary>
    /// 点线关系判断类
    /// 2016/11/30 FHR
    /// </summary>
    public class DistanceUtil
    {
        /// <summary>
        /// 通过梯级缓冲区递推获取点到线要素中的最短距离
        /// </summary>
        /// <param name="featureLayer">要素图层</param>
        /// <param name="point">被查询点</param>
        /// <param name="thefeature">缓冲区中第一个要素</param>
        /// <param name="distance">最短距离</param>
        /// <param name="disNum">在线段上的位置,0在中间</param>
        /// <param name="buffer_Span">缓冲区梯度</param>
        /// <param name="initialBuffer">初始缓冲区距离</param>
        /// <param name="maxBuffer">最大缓冲区距离</param>
        /// <returns>离点最近的单线</returns>
        public static ILine GetNearestLineInFeatureLayerByBufferRecur(IFeatureLayer featureLayer, IPoint point, ref IFeature thefeature, ref double distance, ref int disNum, double buffer_Span, double initialBuffer, double maxBuffer)
        {
            IQueryFilter pQueryFilter = new QueryFilter();
            pQueryFilter.WhereClause = "";
            //要素个数为0则直接返回null
            if (featureLayer.FeatureClass.FeatureCount(pQueryFilter) < 0)
            {
                return null;
            }
            ILine line = null;
            double buffer_distance = initialBuffer;
            IGeometry pGeometry = point as IGeometry;
            while (true)
            {
                if (buffer_distance > maxBuffer)
                {
                    break;
                }
                ITopologicalOperator pTopOperator = pGeometry as ITopologicalOperator;
                if (buffer_distance == initialBuffer)
                {
                    pGeometry = pTopOperator.Buffer(buffer_distance);
                }
                else
                {
                    pGeometry = pTopOperator.Buffer(buffer_Span);
                }
                ILayer layer = featureLayer as ILayer;
                IIdentify pIdentity = layer as IIdentify;
                IArray pArray = pIdentity.Identify(pGeometry);
                if (pArray == null)
                {
                    return null;
                }
                if (pArray.Count == 0)
                {
                    continue;
                }
                distance = 9999999;
                for (int i = 0; i < pArray.Count; i++)
                {
                    IFeature feature = (pArray.get_Element(i) as IRowIdentifyObject).Row as IFeature;
                    double dis = -1;
                    int dnum = -1;
                    ILine L = GetNearestLine(feature.Shape as IPolyline, point, ref dis, ref dnum);
                    if (distance > dis)
                    {
                        disNum = dnum;
                        distance = dis;
                        line = L;
                        thefeature = feature;
                    }
                }
                break;
            }
            return line;
        }
        /// <summary>
        /// 通过梯级缓冲区递推获取点到线要素中的最短距离
        /// </summary>
        /// <param name="featureLayer">要素图层</param>
        /// <param name="point">被查询点</param>
        /// <param name="thefeature">缓冲区中第一个要素</param>
        /// <param name="distance">最短距离</param>
        /// <param name="disNum">在线段上的位置,0在中间</param>
        /// <param name="buffer_Span">缓冲区梯度</param>
        /// <returns>离点最近的单线</returns>
        public static ILine GetNearestLineInFeatureLayerByBufferRecur(IFeatureLayer featureLayer, IPoint point, ref IFeature thefeature, ref double distance, ref int disNum, double buffer_Span)
        {
            IQueryFilter pQueryFilter = new QueryFilter();
            pQueryFilter.WhereClause = "";
            //要素个数为0则直接返回null
            if (featureLayer.FeatureClass.FeatureCount(pQueryFilter) < 0)
            {
                return null;
            }
            ILine line = null;
            IGeometry pGeometry = point as IGeometry;
            while (true)
            {
                ITopologicalOperator pTopOperator = pGeometry as ITopologicalOperator;
                pGeometry = pTopOperator.Buffer(buffer_Span);
                ILayer layer = featureLayer as ILayer;
                IIdentify pIdentity = layer as IIdentify;
                IArray pArray = pIdentity.Identify(pGeometry);
                if (pArray == null)
                {
                    return null;
                }
                if (pArray.Count == 0)
                {
                    continue;
                }
                distance = 9999999;
                for (int i = 0; i < pArray.Count; i++)
                {
                    IFeature feature = (pArray.get_Element(i) as IRowIdentifyObject).Row as IFeature;
                    double dis = -1;
                    int dnum = -1;
                    ILine L = GetNearestLine(feature.Shape as IPolyline, point, ref dis, ref dnum);
                    if (distance > dis)
                    {
                        disNum = dnum;
                        distance = dis;
                        line = L;
                        thefeature = feature;
                    }
                }
                break;
            }
            return line;
        }
        /// <summary>
        /// 通过梯级缓冲区获取点到线要素中的最短距离
        /// </summary>
        /// <param name="featureLayer">要素图层</param>
        /// <param name="point">被查询点</param>
        /// <param name="thefeature">缓冲区中第一个要素</param>
        /// <param name="distance">最短距离</param>
        /// <param name="disNum">在线段上的位置,0在中间</param>
        /// <param name="buffer_Span">缓冲区梯度</param>
        /// <param name="initialBuffer">初始缓冲区距离</param>
        /// <param name="maxBuffer">最大缓冲区距离</param>
        /// <returns>离点最近的单线</returns>
        public static ILine GetNearestLineInFeatureLayerByBuffer(IFeatureLayer featureLayer, IPoint point, ref IFeature thefeature, ref double distance, ref int disNum, double buffer_Span,double initialBuffer,double maxBuffer)
        {
            IQueryFilter pQueryFilter = new QueryFilter();
            pQueryFilter.WhereClause = "";
            //要素个数为0则直接返回null
            if (featureLayer.FeatureClass.FeatureCount(pQueryFilter) < 0)
            {
                return null;
            }
            ILine line = null;
            double buffer_distance = initialBuffer;
            while (true)
            {
                buffer_distance += buffer_Span;
                if (maxBuffer < buffer_distance)
                {
                    break;
                }
                ITopologicalOperator pTopOperator = point as ITopologicalOperator;
                IGeometry pGeometry = pTopOperator.Buffer(buffer_distance);
                ILayer layer = featureLayer as ILayer;
                IIdentify pIdentity = layer as IIdentify;
                IArray pArray = pIdentity.Identify(pGeometry);
                if (pArray == null)
                {
                    return null;
                }
                if (pArray.Count == 0)
                {
                    continue;
                }
                distance = 9999999;
                for (int i = 0; i < pArray.Count; i++)
                {
                    IFeature feature = (pArray.get_Element(i) as IRowIdentifyObject).Row as IFeature;
                    double dis = -1;
                    int dnum = -1;
                    ILine L = GetNearestLine(feature.Shape as IPolyline, point, ref dis, ref dnum);
                    if (distance > dis)
                    {
                        disNum = dnum;
                        distance = dis;
                        line = L;
                        thefeature = feature;
                    }
                }
                break;
            }
            return line;
        }
        /// <summary>
        /// 通过梯级缓冲区获取点到线要素中的最短距离
        /// </summary>
        /// <param name="featureLayer">要素图层</param>
        /// <param name="point">被查询点</param>
        /// <param name="thefeature">缓冲区中第一个要素</param>
        /// <param name="distance">最短距离</param>
        /// <param name="disNum">在线段上的位置,0在中间</param>
        /// <param name="buffer_Span">缓冲区梯度</param>
        /// <returns>离点最近的单线</returns>
        public static ILine GetNearestLineInFeatureLayerByBuffer(IFeatureLayer featureLayer, IPoint point, ref IFeature thefeature, ref double distance, ref int disNum, double buffer_Span)
        {
             IQueryFilter pQueryFilter = new QueryFilter();  
            pQueryFilter.WhereClause = "";     
            //要素个数为0则直接返回null
            if(featureLayer.FeatureClass.FeatureCount(pQueryFilter)<0){
                return null;
            }
            ILine line = null;
            double buffer_distance=0;
            while (true)
            {
                buffer_distance += buffer_Span;
                ITopologicalOperator pTopOperator = point as ITopologicalOperator;
                IGeometry pGeometry = pTopOperator.Buffer(buffer_distance);
                ILayer layer = featureLayer as ILayer;
                IIdentify pIdentity = layer as IIdentify;
                IArray pArray = pIdentity.Identify(pGeometry);
                if (pArray == null)
                {
                    return null;
                }
                if (pArray.Count == 0)
                {
                    continue;
                }
                distance = 9999999;
                for (int i = 0; i < pArray.Count; i++)
                {
                    IFeature feature = (pArray.get_Element(i) as IRowIdentifyObject).Row as IFeature;
                    double dis = -1;
                    int dnum = -1;
                    ILine L = GetNearestLine(feature.Shape as IPolyline, point, ref dis, ref dnum);
                    if (distance > dis)
                    {
                        disNum = dnum;
                        distance = dis;
                        line = L;
                        thefeature = feature;
                    }
                }
                break;
            }
            return line;
        }
         /// <summary>
        /// 获取点到其缓冲区中的最短距离
        /// </summary>
        /// <param name="featureLayer">要素图层</param>
        /// <param name="point">被查询点</param>
        /// <param name="thefeature">缓冲区中第一个要素</param>
        /// <param name="distance">最短距离</param>
        /// <param name="disNum">在线段上的位置,1在A点,2在B点，0在中间</param>
        /// <param name="buffer_distance">缓冲区距离</param>
        /// <returns>离点最近的单线</returns>
        public static ILine GetNearestLineInFeatureLayer(IFeatureLayer featureLayer, IPoint point, ref IFeature thefeature, ref double distance, ref int disNum, double buffer_distance)
        {
            ITopologicalOperator pTopOperator = point as ITopologicalOperator;
            IGeometry pGeometry = pTopOperator.Buffer(buffer_distance);
            ILayer layer = featureLayer as ILayer;
            IIdentify pIdentity = layer as IIdentify;
            IArray pArray = pIdentity.Identify(pGeometry);
            if(pArray ==null)
            {
                return null;
            }
            distance = 9999999;
            ILine line = null;
            for (int i = 0; i < pArray.Count;i++)
            {
                IFeature feature = (pArray.get_Element(i) as IRowIdentifyObject).Row as IFeature;
                double dis = -1;
                int dnum = -1;
                ILine L = GetNearestLine(feature.Shape as IPolyline, point, ref dis, ref dnum);
                if (distance > dis)
                {
                    disNum = dnum;
                    distance = dis;
                    line = L;
                    thefeature = feature;
                }
            }
            return line;
        }
        /// <summary>
        /// 暴力获取点到线要素图层中的最短线
        /// </summary>
        /// <param name="featureLayer">要素图层</param>
        /// <param name="point">被查询点</param>
        /// <param name="thefeature">离查询点最近要素</param>
        /// <param name="distance">最短距离</param>
        /// <param name="disNum">在线段上的位置,1在A点,2在B点，0在中间</param>
        /// <returns>离点最近的单线</returns>
        public static ILine GetNearestLineInFeatureLayer(IFeatureLayer featureLayer, IPoint point, ref IFeature thefeature, ref double distance, ref int disNum)
        {
            List<IFeature> features = new List<IFeature>();
            IQueryFilter pQueryFilter = new QueryFilter();//实例化一个查询条件对象            
            pQueryFilter.WhereClause = "";//将查询条件赋值            
            IFeatureCursor pFeatureCursor = featureLayer.Search(pQueryFilter, false);//进行查询            
            IFeature pFeature = pFeatureCursor.NextFeature();
            while(pFeature!=null)
            {
                features.Add(pFeature);
                pFeature = pFeatureCursor.NextFeature();
            }
            ILine line = null;
             distance = 99999999;
            foreach (IFeature feature in features)
            {
                double dis = -1;
                int dnum = -1;
                ILine L = GetNearestLine(feature.Shape as IPolyline, point, ref dis, ref dnum);
                if (distance > dis)
                {
                    disNum = dnum;
                    distance = dis;
                    line = L;
                    thefeature = feature;
                }
            }
            return line;
        }
        /// <summary>
        /// 暴力获取点到线要素图层中的最短线
        /// </summary>
        /// <param name="featureLayer">要素图层</param>
        /// <param name="points">被查询点集合</param>
        /// <param name="thefeatures">离查询点最近要素集合</param>
        /// <param name="distances">最短距离集合</param>
        /// <param name="disNums">在线段上的位置j集合,1在A点,2在B点，0在中间</param>
        /// <returns>离点最近的单线集合</returns>
        public static List<ILine> GetNearestLineInFeatureLayer(IFeatureLayer featureLayer, List<IPoint> points, out List<IFeature> thefeatures,out  List<double> distances,out List<int>disNums)
        {
            List<IFeature> features = new List<IFeature>();
            IQueryFilter pQueryFilter = new QueryFilter();//实例化一个查询条件对象            
            pQueryFilter.WhereClause = "";//将查询条件赋值            
            IFeatureCursor pFeatureCursor = featureLayer.Search(pQueryFilter, false);//进行查询            
            IFeature pFeature = pFeatureCursor.NextFeature();
            while (pFeature != null)
            {
                features.Add(pFeature);
                pFeature = pFeatureCursor.NextFeature();
            }
            #region 初始化操作
            List<ILine> lines = new List<ILine>();
            thefeatures = new List<IFeature>();
            distances = new List<double>();
            disNums = new List<int>();
            foreach (var point in points)
            {
                thefeatures.Add(null);
                lines.Add(null);
                distances.Add(99999999);
                disNums.Add(0);
            }
            #endregion
            foreach (IFeature feature in features)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    IPoint point = points[i];
                    double dis = -1;
                    int dnum = -1;
                    ILine L = GetNearestLine(feature.Shape as IPolyline, point, ref dis, ref dnum);
                    if (distances[i] > dis)
                    {
                        disNums[i] = dnum;
                        distances[i] = dis;
                        lines[i] = L;
                        thefeatures[i] = feature;
                    }
                }
            }
            return lines;
        }
        /// <summary>
        /// 获取点到线的最短距离
        /// </summary>
        /// <param name="pline">线图形元素</param>
        /// <param name="point">点</param>
        /// <param name="dis">最短距离</param>
        /// <param name="disNum">在线段上的位置,1在A点,2在B点，0在中间</param>
        /// <returns>离点最近的单线</returns>
        public static ILine GetNearestLine(IPolyline pLine, IPoint point, ref double dis, ref int disNum)
        {
            ILine line = null;
            IPointCollection pcl = pLine as IPointCollection;
            dis = 999999999;
            for (int i = 1; i < pcl.PointCount; i++)
            {
                ILine l = new LineClass();
                l.FromPoint = pcl.get_Point(i - 1);
                l.ToPoint = pcl.get_Point(i);
                int dnum = -1;
                double d = getPointDistance(l, point, ref dnum);
                if (dis > d)
                {
                    disNum = dnum;
                    dis = d;
                    line = l;
                }
            }
            return line;
        }

        /// <summary>
        /// 获取点到线的最近距离
        /// </summary>
        /// <param name="line"></param>
        /// <param name="pnt"></param>
        /// <param name="disNum">在线段上的位置,1在A点,2在B点，0在中间</param>
        /// <returns>最近距离</returns>
        private static double getPointDistance(ILine line, IPoint pnt, ref int disNum)
        {
            return GetNearestDistance(line.FromPoint, line.ToPoint, pnt, ref disNum);
        }
        /// <summary>
        /// 求p3到AB线段距离
        /// </summary>
        /// <param name="PA"></param>
        /// <param name="PB"></param>
        /// <param name="P3"></param>
        /// <param name="disNum">在线段上的位置,1在A点,2在B点，0在中间</param>
        /// <returns>距离</returns>
        private static double GetNearestDistance(IPoint PA, IPoint PB, IPoint P3, ref int disNum)
        {
            double a, b, c;
            a = GetPointDistance(PB, P3);
            if (a <= 0.00001)
            {
                disNum = 2;//在线段的B点
                return 0.0f;
            }
            b = GetPointDistance(PA, P3);
            if (b <= 0.00001)
            {
                disNum = 1;//在线段的A点
                return 0.0f;
            }
            //如果PA和PB坐标相同，则退出函数，并返回距离   
            c = GetPointDistance(PA, PB);
            if (c <= 0.00001)
            {
                return a;
            }
            if (a * a >= b * b + c * c)
            {
                disNum = 1;
                return b;
            }
            if (b * b >= a * a + c * c) 
            {
                disNum = 2;
                return a;
            }
            double l = (a + b + c) / 2;     //周长的一半   
            double s = Math.Sqrt(l * (l - a) * (l - b) * (l - c));  //海伦公式求面积   
            disNum = 0;//在线段中间
            return 2 * s / c;
        }
        /// <summary>
        /// 获取点到单线的垂足
        /// </summary>
        /// <param name="line"></param>
        /// <param name="pnt"></param>
        /// <returns>垂足</returns>
        public static IPoint GetCrossPnt(ILine line, IPoint pnt)
        {
            IPoint pt1 = line.FromPoint;
            IPoint pt2 = line.ToPoint;
            double A = (pt1.Y - pt2.Y) / (pt1.X - pt2.X);
            double B = (pt1.Y - A * pt1.X);
            double m = pnt.X + A * pnt.Y;
            /// 求两直线交点坐标
            IPoint ptCross = new PointClass();
            ptCross.X = ((m - A * B) / (A * A + 1));
            ptCross.Y = (A * ptCross.X + B);
            return ptCross;
        }
        /// <summary>
        /// 获取两点距离
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns>距离</returns>
        private static double GetPointDistance(IPoint p1, IPoint p2)
        {
            return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
        }
    }
}
