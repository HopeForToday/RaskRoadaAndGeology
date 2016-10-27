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
            DataTable idS = Common.DBHander.ReturnDataSet("select ID from ForecastWeather where AreaID=" + AreaID).Tables[0];
            List<string> sqllist = new List<string>();
            // var dd=WeatherList.Find(t=>t.dateTime.Contains("日"));var index1=WeatherList.IndexOf(dd);  下面的方式效率会高一些
            int index = 0, count = WeatherList.Count;
            for (int k = 0; k < count; k++)
            {
                if (WeatherList[k].dateTime.Contains("日"))
                {
                    index = k;
                    break;
                }
            }
            WeatherList.RemoveRange(index + 2, 8 - (index + 2));
            //第一次录入数据 
            if (idS.Rows.Count == 0 || idS == null)
            {
                for (int i = 0; i < 56; i++)
                {
                    string sql = "INSERT INTO  ForecastWeather (AreaID) values (" + AreaID + ")";
                    sqllist.Add(sql);
                }
                Common.DBHander.insertToAccessByBatch(sqllist);
                idS = Common.DBHander.ReturnDataSet("select ID from ForecastWeather where AreaID=" + AreaID).Tables[0];
            }

            List<int> IDs = new List<int>();
            foreach (DataRow dr in idS.Rows)
            {
                IDs.Add(Convert.ToInt32(dr[0]));
            }
            //var kk = WeatherList.Find(t=>t.dateTime.Contains("日"));
            //var dd = WeatherList.IndexOf(kk);   下面的代码效率要高一些

            int start = 8 - index - 1;//开始存数据的时间段
            int timeD = IDs.Count - start;//需要录入数据的时间段数
            //WeatherList.RemoveRange(index + 2, 8 - (index+2) );
            sqllist.Clear();
            for (int i = 0; i < WeatherList.Count; i++)
            {
                var r = WeatherList[i];
                sqllist.Add(string.Format("update ForecastWeather set dtime3hour='{0}' , temperature='{1}',rains='{2}',wind='{3}',windd='{4}',qy='{5}',yl='{6}',njd='{7}',xdsd='{8}'  where ID={9} ",
                    r.dateTime, r.temperature, r.rains, r.wind, r.windd, r.qy, r.yl, r.njd, r.xdsd, IDs[8 - (index + 2) + i]));
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
