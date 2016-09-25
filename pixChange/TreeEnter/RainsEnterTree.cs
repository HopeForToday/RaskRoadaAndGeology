using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace RoadRaskEvaltionSystem.TreeEnter
{
    public class RainsEnterTree
    {
        //名称字段变量  
        private string areaName = string.Empty;
        //选择字段变量
        private bool m_bIsChecked = false;
        //子Node节点ID变量
        private int areaID = -1;
        //父Node节点ID变量
        private int fatherAreaID = -1;
        //开始日期
        private DateTime formDate;
        //截止日期
        private DateTime toDate;
        //雨量
        private int vol;
        public int AreaID
        {
            get
            {
                return areaID;
            }
            set
            {
                areaID = value;
            }
        }
        public int FatherAreaID
        {

            get
            {

                return fatherAreaID;

            }

            set
            {

                fatherAreaID = value;

            }

        }
        public string AreaName
        {

            get
            {

                return areaName;

            }

            set
            {

                areaName = value;

            }

        }
        //起始整点数
        public int fromHour;
        //截至整点数
        public int toHour;
        public bool IsChecked
        {

            get
            {

                return m_bIsChecked;

            }

            set
            {

                m_bIsChecked = value;

            }

        }
        public DateTime FormDate { get { return formDate; }set { formDate = value; } }
        public DateTime ToDate { get { return toDate; } set { toDate = value; } }
        public int Vol { get { return vol; } set { vol = value; } }
        public int FromHour { get { return fromHour; } set { fromHour = value; } }
        public int ToHour { get { return toHour; } set { toHour = value; } }
        public RainsEnterTree(DataRow dr)
        {
            if (dr == null) return;
            AreaID = dr["AreaID"] is DBNull ? -1 : Convert.ToInt32(dr["AreaID"]);
            fatherAreaID = dr["fatherID"] is DBNull ? -1 : Convert.ToInt32(dr["fatherID"]);
            AreaName = dr["AreaName"] is DBNull ? string.Empty : Convert.ToString(dr["AreaName"]);
            formDate = DateTime.Today.AddDays(-1);
            toDate = DateTime.Today;


        }
        //测试
        public RainsEnterTree(int q,int fa)
        {
            AreaID = q;
            fatherAreaID = fa;
            AreaName = q.ToString() + fa.ToString() + "Name";
            formDate = DateTime.Today.AddDays(-1);
            toDate = DateTime.Today;
        }

        public RainsEnterTree()
        {
            
        }
    }
}
