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
    /// 2016/12/2 FHR
    /// </summary>
    public class DistanceUtil
    {
        /// <summary>
        /// 获取点到包围壳边缘的最大距离
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static double GetPointToTheEnvelopMax(IFeatureLayer layer,IPoint point)
        {
            IEnvelope pEnvelope = layer.AreaOfInterest;
            double xMax = pEnvelope.XMax;
            double xMin = pEnvelope.XMin;
            double yMax = pEnvelope.YMax;
            double yMin = pEnvelope.YMin;
            IPoint point1 = new PointClass();
            point1.X = xMax;
            point1.Y = yMax;
            IPoint point2= new PointClass();
            point1.X = xMax;
            point1.Y = yMin;
            IPoint point3 = new PointClass();
            point1.X = xMin;
            point1.Y = yMax;
            IPoint point4 = new PointClass();
            point1.X = xMin;
            point1.Y = yMin;
            List<IPoint> points = new List<IPoint>() { point2,point3,point4};
            double distance=GetPointDistance(point, point1);
            points.ForEach(p =>
            {
                double tempDistance = GetPointDistance(point, p);
                if (distance > tempDistance)
                {
                    distance = tempDistance;
                }
            });
            return distance;
        }
        /// <summary>
        /// 通过缓冲区二分查找获取点到线要素图层中的最短距离
        /// 基于二分查找和贪心算法
        /// </summary>
        /// <param name="featureLayer">要素图层</param>
        /// <param name="point">被查询点</param>
        /// <param name="thefeature">缓冲区中最近要素</param>
        /// <param name="distance">最短距离</param>
        /// <param name="disNum">在线段上的位置集合,1在右方,2在左方，0在线上</param>
        /// <param name="minSpan">最小间距</param>
        /// <returns>离点最近的单线</returns>
        public static IPoint GetNearestLineInFeatureLayerByBufferBinary(IFeatureLayer featureLayer, IPoint point, ref IFeature thefeature, ref double distance, ref int disNum, double minSpan)
        {
            #region 初始化操作
            IQueryFilter pQueryFilter = new QueryFilter();
            pQueryFilter.WhereClause = "";
            //要素个数为0则直接返回null
            int count = featureLayer.FeatureClass.FeatureCount(pQueryFilter);
            if (count == 0)
            {
                return null;
            }
            //个数为1则直接计算
            else if (count == 1)
            {
                return GetNearestLineInFeatureLayer(featureLayer, point, ref  thefeature, ref  distance, ref  disNum);
            }
            #endregion
            double low = 0;
            double high = GetPointToTheEnvelopMax(featureLayer,point);
            IArray pArray = null;
            ////求最大缓冲区内的要素个数
            ITopologicalOperator pTopOperator = point as ITopologicalOperator;
            IGeometry pGeometry = pTopOperator.Buffer(high);
            ILayer layer = featureLayer as ILayer;
            IIdentify pIdentity = layer as IIdentify;
            pArray = pIdentity.Identify(pGeometry);
            // 如果为0或者计算错误直接返回null
            if (pArray.Count == 0 || pArray==null)
            {
                return null;
            }
              //如果只有一个要素 则直接计算返回
            else if (pArray.Count==1)
            {
                return GetPointToIArrayMin(point, ref thefeature, ref distance, ref disNum, pArray);
            }
            ///////////////////////////////////////////////
            while ((high - low) < minSpan)
            {
                double buffer_distance = (low + high) / 2;
                pGeometry = pTopOperator.Buffer(buffer_distance);
                pArray = pIdentity.Identify(pGeometry);
                if (pArray.Count == 0)
                {
                    low = buffer_distance;
                }
                else
                {
                    high = buffer_distance;
                }
            }
            ////此时high可以认为是最低缓冲区距离 直接求解最短距离
            pGeometry = pTopOperator.Buffer(high);
            pArray = pIdentity.Identify(pGeometry);
            return GetPointToIArrayMin(point, ref thefeature, ref distance, ref disNum, pArray);
        }
        /// <summary>
        /// 通过缓冲区二分查找获取点到缓冲区内线要素中的最短距离
        /// 基于二分查找和贪心算法
        /// </summary>
        /// <param name="featureLayer">要素图层</param>
        /// <param name="point">被查询点</param>
        /// <param name="thefeature">缓冲区中最近要素</param>
        /// <param name="distance">最短距离</param>
        /// <param name="disNum">在线段上的位置集合,1在右方,2在左方，0在线上</param>
        /// <param name="buffer_Span">缓冲区梯度</param>
        /// <param name="maxBuffer">最大缓冲区距离</param>
        /// <param name="minSpan">最小间距</param>
        /// <returns>离点最近的单线</returns>
        public static IPoint GetNearestLineInFeatureLayerByBufferBinary(IFeatureLayer featureLayer, IPoint point, ref IFeature thefeature, ref double distance, ref int disNum, double maxBuffer,double minSpan)
        {
            IQueryFilter pQueryFilter = new QueryFilter();
            pQueryFilter.WhereClause = "";
            //要素个数为0则直接返回null
            if (featureLayer.FeatureClass.FeatureCount(pQueryFilter) ==0)
            {
                return null;
            }
            double low=0;
            double high=maxBuffer;
            IArray pArray=null;
            ////求最大缓冲区内的要素个数 如果为0则直接返回null 为1则直接计算返回
           ITopologicalOperator pTopOperator = point as ITopologicalOperator;
            IGeometry pGeometry=pTopOperator.Buffer(high);
            ILayer layer = featureLayer as ILayer;
            IIdentify pIdentity = layer as IIdentify;
            pArray = pIdentity.Identify(pGeometry);
            if(pArray.Count==0)
            {
              return null;
            }
            else if (pArray.Count == 1)
            {
                return GetPointToIArrayMin(point, ref thefeature, ref distance, ref disNum, pArray);
            }
            ///////////////////////////////////////////////
            while((high-low)<minSpan)
            {
                  double buffer_distance=(low+high)/2;
                 pGeometry=pTopOperator.Buffer(buffer_distance);
                  pArray = pIdentity.Identify(pGeometry);
                  if(pArray.Count==0)
                  {
                      low=buffer_distance;
                  }
                  else
                  {
                      high=buffer_distance;
                  }
            }
              ////此时high可以认为是最低缓冲区距离 直接求解最短距离
              pGeometry=pTopOperator.Buffer(high);
              pArray = pIdentity.Identify(pGeometry);
              return GetPointToIArrayMin(point, ref thefeature, ref distance, ref disNum, pArray);
        }
       
        /// <summary>
        /// 通过梯级缓冲区递推获取点到线要素图层中的最短距离
        /// </summary>
        /// <param name="featureLayer">要素图层</param>
        /// <param name="point">被查询点</param>
        /// <param name="thefeature">缓冲区中最近要素</param>
        /// <param name="distance">最短距离</param>
        /// <param name="disNum">在线段上的位置集合,1在右方,2在左方，0在线上</param>
        /// <param name="buffer_Span">缓冲区梯度</param>
        /// <returns>离点最近的单线</returns>
        public static IPoint GetNearestLineInFeatureLayerByBufferRecur(IFeatureLayer featureLayer, IPoint point, ref IFeature thefeature, ref double distance, ref int disNum, double buffer_Span)
        {
            #region 初始化操作
            IQueryFilter pQueryFilter = new QueryFilter();
            pQueryFilter.WhereClause = "";
            //要素个数为0则直接返回null
            int count = featureLayer.FeatureClass.FeatureCount(pQueryFilter);
            if (count == 0)
            {
                return null;
            }
            //个数为1则直接计算
            else if (count == 1)
            {
                return GetNearestLineInFeatureLayer(featureLayer, point, ref  thefeature, ref  distance, ref  disNum);
            }
            #endregion
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
                return GetPointToIArrayMin(point, ref thefeature, ref distance, ref disNum, pArray);
            }
        }
        /// <summary>
        /// 通过梯级缓冲区递推获取点到缓冲区中线要素中的最短距离
        /// </summary>
        /// <param name="featureLayer">要素图层</param>
        /// <param name="point">被查询点</param>
        /// <param name="thefeature">缓冲区中第一个要素</param>
        /// <param name="distance">最短距离</param>
        /// <param name="disNum">在线段上的位置集合,1在右方,2在左方，0在线上</param>
        /// <param name="buffer_Span">缓冲区梯度</param>
        /// <param name="maxBuffer">最大缓冲区距离</param>
        /// <returns>离点最近的单线</returns>
        public static IPoint GetNearestLineInFeatureLayerByBuffer(IFeatureLayer featureLayer, IPoint point, ref IFeature thefeature, ref double distance, ref int disNum, double buffer_Span,double maxBuffer)
        {
            IQueryFilter pQueryFilter = new QueryFilter();
            pQueryFilter.WhereClause = "";
            //要素个数为0则直接返回null
            if (featureLayer.FeatureClass.FeatureCount(pQueryFilter) < 0)
            {
                return null;
            }
            double buffer_distance = 0;
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
                return GetPointToIArrayMin(point, ref thefeature, ref distance, ref disNum, pArray);
            }
            return null;
        }
        /// <summary>
        /// 通过梯级缓冲区获取点到线要素图层中的最短距离
        /// </summary>
        /// <param name="featureLayer">要素图层</param>
        /// <param name="point">被查询点</param>
        /// <param name="thefeature">缓冲区中第一个要素</param>
        /// <param name="distance">最短距离</param>
        /// <param name="disNum">在线段上的位置集合,1在右方,2在左方，0在线上</param>
        /// <param name="buffer_Span">缓冲区梯度</param>
        /// <returns>离点最近的单线</returns>
        public static IPoint GetNearestLineInFeatureLayerByBuffer(IFeatureLayer featureLayer, IPoint point, ref IFeature thefeature, ref double distance, ref int disNum, double buffer_Span)
        {
            #region 初始化操作
            IQueryFilter pQueryFilter = new QueryFilter();
            pQueryFilter.WhereClause = "";
            //要素个数为0则直接返回null
            int count = featureLayer.FeatureClass.FeatureCount(pQueryFilter);
            if (count == 0)
            {
                return null;
            }
            //个数为1则直接计算
            else if (count == 1)
            {
                return GetNearestLineInFeatureLayer(featureLayer, point, ref  thefeature, ref  distance, ref  disNum);
            }
            #endregion
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
                return GetPointToIArrayMin(point, ref thefeature, ref distance, ref disNum, pArray);
            }
        }
         /// <summary>
        /// 暴力获取点到其缓冲区中的最短距离
        /// </summary>
        /// <param name="featureLayer">要素图层</param>
        /// <param name="point">被查询点</param>
        /// <param name="thefeature">缓冲区中第一个要素</param>
        /// <param name="distance">最短距离</param>
        /// <param name="disNum">在线段上的位置集合,1在右方,2在左方，0在线上</param>
        /// <param name="buffer_distance">缓冲区距离</param>
        /// <returns>离点最近的单线</returns>
        public static IPoint GetNearestLineInFeatureLayer(IFeatureLayer featureLayer, IPoint point, ref IFeature thefeature, ref double distance, ref int disNum, double buffer_distance)
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
            return GetPointToIArrayMin(point, ref thefeature, ref distance, ref disNum, pArray);
        }
    
        /// <summary>
        /// 暴力获取点到线要素图层中的最短线
        /// </summary>
        /// <param name="featureLayer">要素图层</param>
        /// <param name="point">被查询点</param>
        /// <param name="thefeature">离查询点最近要素</param>
        /// <param name="distance">最短距离</param>
        /// <param name="disNum">在线段上的位置集合,1在右方,2在左方，0在线上</param>
        /// <returns>离点最近的单线</returns>
        public static IPoint GetNearestLineInFeatureLayer(IFeatureLayer featureLayer, IPoint point, ref IFeature thefeature, ref double distance, ref int disNum)
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
            IPoint outPoint = GetPointToFeaturesMin(point, ref thefeature, ref distance, ref disNum, features);
            return outPoint;
        }
        
        /// <summary>
        /// 暴力获取点集合到线要素图层中的最短线
        /// </summary>
        /// <param name="featureLayer">要素图层</param>
        /// <param name="points">被查询点集合</param>
        /// <param name="thefeatures">离查询点最近要素集合</param>
        /// <param name="distances">最短距离集合</param>
        /// <param name="disNums">在线段上的位置集合,1在右方,2在左方，0在线上</param>
        /// <returns>最近点集合</returns>
        public static List<IPoint> GetNearestLineInFeatureLayer(IFeatureLayer featureLayer, List<IPoint> points, out List<IFeature> thefeatures,out  List<double> distances,out List<int>disNums)
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
            return GetPointsToFeatures(points, features, out thefeatures, out distances, out disNums);
        }
        /// <summary>
        /// 获取点到IArray的最短距离
        /// </summary>
        /// <param name="point"></param>
        /// <param name="thefeature"></param>
        /// <param name="distance"></param>
        /// <param name="disNum"></param>
        /// <param name="pArray"></param>
        /// <returns></returns>
        private static IPoint GetPointToIArrayMin(IPoint point, ref IFeature thefeature, ref double distance, ref int disNum, IArray pArray)
        {
            distance = 9999999;
            IPoint outPoint = null;
            for (int i = 0; i < pArray.Count; i++)
            {
                IFeature feature = (pArray.get_Element(i) as IRowIdentifyObject).Row as IFeature;
                double dis = -1;
                int dnum = -1;
                IPoint tempPoint = GetNearestLine(feature.Shape as IPolyline, point, ref dis, ref dnum);
                if (distance > dis)
                {
                    disNum = dnum;
                    distance = dis;
                    outPoint = tempPoint;
                    thefeature = feature;
                }
            }
            return outPoint;
        }
        /// <summary>
        /// 获取点到要素集合的最近距离
        /// </summary>
        /// <param name="point"></param>
        /// <param name="thefeature"></param>
        /// <param name="distance"></param>
        /// <param name="disNum"></param>
        /// <param name="features"></param>
        /// <returns></returns>
        private static IPoint GetPointToFeaturesMin(IPoint point, ref IFeature thefeature, ref double distance, ref int disNum, List<IFeature> features)
        {
            IPoint outPoint = null;
            distance = 99999999;
            foreach (IFeature feature in features)
            {
                double dis = -1;
                int dnum = -1;
                IPoint tempPoint = GetNearestLine(feature.Shape as IPolyline, point, ref dis, ref dnum);
                if (distance > dis)
                {
                    disNum = dnum;
                    distance = dis;
                    outPoint = tempPoint;
                    thefeature = feature;
                }
            }
            return outPoint;
        }
        /// <summary>
        /// 获取点集合到要素的集合的最短距离
        /// </summary>
        /// <param name="points"></param>
        /// <param name="features"></param>
        /// <param name="thefeatures"></param>
        /// <param name="distances"></param>
        /// <param name="disNums"></param>
        /// <returns></returns>
        private static List<IPoint> GetPointsToFeatures(List<IPoint> points, List<IFeature> features, out List<IFeature> thefeatures, out List<double> distances, out List<int> disNums)
        {
            #region 初始化操作
            List<IPoint> outPoints = new List<IPoint>();
            thefeatures = new List<IFeature>();
            distances = new List<double>();
            disNums = new List<int>();
            foreach (var point in points)
            {
                thefeatures.Add(null);
                outPoints.Add(null);
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
                    IPoint outPoint = GetNearestLine(feature.Shape as IPolyline, point, ref dis, ref dnum);
                    if (distances[i] > dis)
                    {
                        disNums[i] = dnum;
                        distances[i] = dis;
                        outPoints[i] = outPoint;
                        thefeatures[i] = feature;
                    }
                }
            }
            return outPoints;
        }
        /// <summary>
        /// 通过IPolyline.QueryPointAndDistance获取最近点和距离
        /// </summary>
        /// <param name="pLine">线</param>
        /// <param name="point">输入点</param>
        /// <param name="disAloneFrom">最近点到线段起始点的距离</param>
        /// <param name="dis">最近距离</param>
        /// <param name="disNum">在线段上的位置集合,1在右方,2在左方，0在线上</param>
        /// <returns>线上的最近点</returns>
        public static IPoint GetNearestLine(IPolyline pLine, IPoint point, ref double disAloneFrom, ref double dis, ref int disNum)
        {
            IPoint outPoint = new PointClass();
            bool isRightSide = false;
            pLine.QueryPointAndDistance(esriSegmentExtension.esriNoExtension, point, false, outPoint, ref disAloneFrom, ref dis, ref isRightSide);
            if (isRightSide)
            {
                disNum = 1;
            }
            else
            {
                if (dis == 0)
                {
                    disNum = 0;
                }
                else
                {
                    disNum = 2;
                }
            }
            return outPoint;
        }
        /// <summary>
        /// 通过IPolyline.QueryPointAndDistance获取最近点和距离
        /// </summary>
        /// <param name="pLine">线</param>
        /// <param name="point">输入点</param>
        /// <param name="dis">最近距离</param>
        /// <param name="disNum">在线段上的位置集合,1在右方,2在左方，0在线上</param>
        /// <returns>线上的最近点</returns>
        public static IPoint GetNearestLine(IPolyline pLine, IPoint point, ref double dis, ref int disNum)
        {
            IPoint outPoint = new PointClass();
            bool isRightSide = false;
            double disAloneFrom = -1;
            pLine.QueryPointAndDistance(esriSegmentExtension.esriNoExtension, point, false, outPoint, ref disAloneFrom, ref dis, ref isRightSide);
            if (isRightSide)
            {
                disNum = 1;
            }
            else
            {
                if (dis == 0)
                {
                    disNum = 0;
                }
                else
                {
                    disNum = 2;
                }
            }
            return outPoint;
        }
        /// <summary>
        /// 获取点到线的最短距离
        /// </summary>
        /// <param name="pline">线图形元素</param>
        /// <param name="point">点</param>
        /// <param name="dis">最短距离</param>
        /// <param name="disNum">在线段上的位置,1在A点,2在B点，0在中间</param>
        /// <returns>离点最近的单线</returns>
        public static ILine GetNearestLine2(IPolyline pLine, IPoint point, ref double dis, ref int disNum)
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
