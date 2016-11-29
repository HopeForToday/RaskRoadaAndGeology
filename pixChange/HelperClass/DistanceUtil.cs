using ESRI.ArcGIS.Carto;
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
    public class DistanceUtil
    {

        /// <summary>
        /// 获取点到线要素图层中的最短线
        /// </summary>
        /// <param name="featureLayer">要素图层</param>
        /// <param name="point">被查询点</param>
        /// <param name="thefeature">离查询点最近要素</param>
        /// <param name="distance">最短距离</param>
        /// <param name="disNum">在线段上的位置,1在A点,2在B点，0在中间</param>
        /// <returns>离点最近的单线</returns>
        public static ILine GetNearestLineInFeature(IFeatureLayer featureLayer, IPoint point, ref IFeature thefeature, ref double distance, ref int disNum)
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
            c = GetPointDistance(PA, PB);
            if (c <= 0.00001)
                return a;//如果PA和PB坐标相同，则退出函数，并返回距离   
            //------------------------------   


            if (a * a >= b * b + c * c)//--------图3--------   
            {
                disNum = 1;
                return b;
            }
            if (b * b >= a * a + c * c)//--------图4-------   
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
