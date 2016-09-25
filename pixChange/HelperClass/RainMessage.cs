using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
namespace RoadRaskEvaltionSystem.HelperClass
{
  public class RainMessage
    {
      public string   initByAreaNameAndDate(string AreaName,DateTime date)
      {
          //string AreaID=
          string AreaID = Common.DBHander.ReturnDataSet("select AreaID from Area where AreaName='" + AreaName+"'").Tables[0].Rows[0]["AreaID"].ToString();
          int count = Common.DBHander.ReturnSqlResultCount("select ReacordID from AllDayRanis where AreaID='" + AreaID + "' and " + "TheDay='" + date+"'");
          if (count == 0)
          {
              string insertString = "insert into AllDayRanis (AreaID,TheDay)" + "vaules (\"" + AreaID + "\"" + ",\"" + date + "\")";
              Common.DBHander.ExeSQL(insertString);
          }
          return AreaID;
      }
      /// <summary>
      /// 雨量信息录入
      /// </summary>
      /// <param name="AreaName"></param>
      /// <param name="date"></param>
      /// <param name="rains">雨量</param>      
      public bool EnterRanisMessage(string AreaName, DateTime date,int rains,int timeHour)
      {
          string dateString=date.ToString("yyyy/MM/dd");
          string AreaID = Common.DBHander.ReturnDataSet("select AreaID from Area where AreaName='" + AreaName + "'").Tables[0].Rows[0]["AreaID"].ToString();
          //  string getReacordID = "select ReacordID from AllDayRanis where AreaID='" + AreaID + "' and " + "TheDay='" + dateString + "'";//遇到找不到表AllDayRanis或查询的错误 
          string getReacordID = string.Format("SELECT ReacordID FROM AllDayRains  where AreaID='{0}' and TheDay=#{1}#", AreaID, dateString);//access sql 中#时间# 才能表示时间
          DataTable dt = Common.DBHander.ReturnDataSet(getReacordID).Tables[0];
          string updateString = "";
          int ReacordID=0;
          if(dt==null||dt.Rows.Count==0)
          {
              string insertString = "insert into AllDayRains (AreaID,TheDay)" + "values (\"" + AreaID + "\"" + ",\"" + dateString + "\")";
              Common.DBHander.ExeSQL(insertString);
              updateString = string.Format("update AllDayRains set V{0}='{1}' where AreaID='{2}' and  TheDay=#{3}#", timeHour, rains, AreaID, dateString);
          }
          else
          {
              ReacordID = Convert.ToInt32(Common.DBHander.ReturnDataSet(getReacordID).Tables[0].Rows[0]["ReacordID"]);
              updateString = string.Format("update AllDayRains V{0}='{1}' where ReacordID='{2}'", timeHour, rains, ReacordID);
            
          }
         return  Common.DBHander.ExeSQL(updateString);
      }
    }
}
