using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using DevExpress.XtraBars.ViewInfo;
using ESRI.ArcGIS.Geoprocessing;
using HtmlAgilityPackDemo1;
using RoadRaskEvaltionSystem.HelperClass;

namespace RoadRaskEvaltionSystem.WeatherHander
{
    public class SaveWeatherMsg:ISaveWeather
    {
        private IGetWeather getWeatherObj = null;
        public SaveWeatherMsg(IGetWeather getWeatherObj)
        {
            this.getWeatherObj = getWeatherObj;
        }
        public  void SaveForeacastWerherMsg(string url,int AreaID)
        {
            List<forecastWeatherMesg> WeatherList = getWeatherObj.getforcastMessage(url);
            if (WeatherList.Count == 0)
            {
                Console.WriteLine("天气预报提取失败!");
                return;
            }
            DataTable idS = Common.DBHander.ReturnDataSet("select ID from ForecastWeather where AreaID=" + AreaID ).Tables[0];
            List<string> sqllist = new List<string>();
            if (idS.Rows.Count == 0 || idS == null)
            {
               
                foreach (var r in WeatherList)
                {

                     string sql =
                        string.Format(
                            "INSERT INTO  ForecastWeather (AreaID,dtime3hour,temperature,rains,wind,windd,qy,yl,njd,xdsd) VALUES ({0},'{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}') ",
                            AreaID, r.dateTime, r.temperature, r.rains, r.wind, r.windd, r.qy, r.yl, r.njd, r.xdsd);//datetime 不能用在sql语句中 是关键字
                 // Common.DBHander.ExeSQL(sql);
                    sqllist.Add(sql);
                }              
            }
            else
            {
                List<int> IDs=new List<int>();
                foreach (DataRow dr in idS.Rows)
                {
                    IDs.Add(Convert.ToInt32(dr[0]));
                }
                for (int i = 0; i < IDs.Count; i++)
                {
                    var r = WeatherList[i];
                    sqllist.Add(string.Format("update ForecastWeather set dtime3hour='{0}' , temperature='{1}',temperature='{2}',wind='{3}',windd='{4}',qy='{5}',yl='{6}',njd='{7}',xdsd='{8}'  where ID='{9}' ", r.dateTime, r.temperature, r.rains, r.wind, r.windd, r.qy, r.yl, r.njd, r.xdsd, IDs[i]));
                }
               
            }
            Common.DBHander.insertToAccessByBatch(sqllist);
          
        }
        //实际上只有最近23个小时的数据
        public  void Savelast24hMsg(string url, int AreaID)
        {
            List<OneHourWeather> Last23h = (List<OneHourWeather>)getWeatherObj.Get24HourWeather(url);
            foreach (var r in Last23h)
            {
                DataTable dt = Common.DBHander.ReturnDataSet("select ID from OneHourWeather where Hourtime=#" + r.time + "# and AreaID=" + AreaID).Tables[0];
                if (dt.Rows.Count == 0)
                {
                    //插入这个时间的数据
                    string sql =
                        string.Format(
                            "insert into OneHourWeather (Hourtime,rain1h,rain12h,rain6h,rain24h,temperature,pressure,humidity,windDirection,windSpeed,AreaID) values(#{0}#,'{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',{10}) ",
                            r.time, r.rain1h, r.rain12h, r.rain6h, r.rain24h, r.temperature, r.pressure, r.humidity,
                            r.windDirection, r.windSpeed, AreaID);
                  bool succes=  Common.DBHander.ExeSQL(sql);
                    if (!succes)
                    {
                        Console.WriteLine(r.time + " 的天气信息存入失败!___" + DateTime.Now);
                    }
                    else
                    {
                        Console.WriteLine(AreaID+"---"+  r.time + " 的天气信息存入成功!___" + DateTime.Now);
                    }
                }
                else
                {
                    //更新  可以不写
                }
               

            }

  
        }
    }
}
