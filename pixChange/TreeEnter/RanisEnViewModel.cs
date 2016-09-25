using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using DevExpress.Data.XtraReports.Wizard.Native;
using RoadRaskEvaltionSystem.HelperClass;

namespace RoadRaskEvaltionSystem.TreeEnter
{
   public class RanisEnViewModel
   {
       public ObservableCollection<RainsEnterTree> RainsEnterTreeList;
    
       public RanisEnViewModel()
       {
           RainsEnterTreeList=new ObservableCollection<RainsEnterTree>();
          GetDataFormmdb();
         //  test();
       }
       //取出区域信息
       public void GetDataFormmdb()
       {
           string sql = "select AreaID,fatherID, AreaName from Area ";
           DataTable dt = Common.DBHander.ReturnDataSet(sql).Tables[0];
           foreach (DataRow dr in dt.Rows)
           {
               RainsEnterTreeList.Add(new RainsEnterTree(dr)); 
           }
           
       }
       //批量录入雨量信息
       public void EnterRains(ObservableCollection<RainsEnterTree> rainsList)
       {
         
           int i = 0;
           // string[] sqllist;
           List<string> sqllist = new List<string>();
           foreach (var r in rainsList)
           {
              
           //    string sql = "INSERT INTO  ManRains(AreaID,FromDate,FromHour,ToDate,ToHour,Vol)" + "VALUES (\"" + i + "\"" + ",\"" +"fff\"" + "\")";
               string sql = string.Format("INSERT INTO  ManRains(AreaID,FromDate,FromHour,ToDate,ToHour,Vol)VALUES({0},#{1}#,{2},#{3}#,{4},{5}) ",r.AreaID,r.FormDate,r.FromHour,r.ToDate,r.ToHour,r.Vol);
               sqllist.Add(sql);
              //  Common.DBHander.ExeSQL(sql);
           }
           Common.DBHander.insertToAccessByBatch(sqllist);
       }
       //添加地图
       public void LoadMap()
       {
           
       }

       public void test()
       {
           for (int i = 1; i < 100; i++)
           {
             
                   RainsEnterTreeList.Add(new RainsEnterTree(i, i - 5));
           
              
           }
       }
   }
}
