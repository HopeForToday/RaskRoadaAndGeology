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
    public class SaveWeatherMsg : ISaveWeather
    {
        private IGetWeather getWeatherObj = null;
        public SaveWeatherMsg(IGetWeather getWeatherObj)
        {
            this.getWeatherObj = getWeatherObj;
        }
        public void SaveForeacastWerherMsg(string url, int AreaID, out bool isSaved7days)
        {
            isSaved7days = true;
            List<forecastWeatherMesg> WeatherList = getWeatherObj.getforcastMessage(url);
            if (WeatherList.Count == 0)
            {
                Console.WriteLine("SaveForeacastWerherMsg天气预报提取失败!");
                isSaved7days = false;
                return;
            }
            DataTable idS = Common.DBHander.ReturnDataSet("select ID from ForecastWeather where AreaID=" + AreaID + " order by ID asc").Tables[0];
            List<string> sqllist = new List<string>();
            
            int index = 0, count = WeatherList.Count;
            for (int k = 0; k < count; k++)
            {
                if (WeatherList[k].dateTime.Contains("日"))
                {
                    index = k;
                    break;
                }
            }
            if (index < 8)
            {
                WeatherList.RemoveRange(index + 2, 8 - (index + 2));
            }
            if (index == 14)
            {
                WeatherList[0].dateTime = DateTime.Today.AddDays(1).ToString("dd日") + WeatherList[0].dateTime;
                WeatherList.RemoveRange(8, 6);
            }

            //第一次录入数据 
            if (idS.Rows.Count == 0 || idS == null)
            {
                for (int i = 0; i < 56; i++)
                {
                    string sql = "INSERT INTO  ForecastWeather (AreaID) values (" + AreaID + ")";
                    sqllist.Add(sql);
                }
                Common.DBHander.insertToAccessByBatch(sqllist);
                string sq = "select ID from ForecastWeather where  AreaID=" + AreaID + " order by ID asc";
                idS = Common.DBHander.ReturnDataSet(sq).Tables[0];
            }
            if (idS.Rows.Count < 56)
            {
                string sqlStr = "DELETE FROM ForecastWeather WHERE AreaID = "+AreaID;
                Common.DBHander.deleteDt(sqlStr);
                for (int i = 0; i < 56; i++)
                {
                    string sql = "INSERT INTO  ForecastWeather (AreaID) values (" + AreaID + ")";
                    sqllist.Add(sql);
                }
                Common.DBHander.insertToAccessByBatch(sqllist);
                string sq = "select ID from ForecastWeather where  AreaID=" + AreaID + " order by ID asc";
                idS = Common.DBHander.ReturnDataSet(sq).Tables[0];
            }
            List<int> IDs = new List<int>();
            foreach (DataRow dr in idS.Rows)
            {
                IDs.Add(Convert.ToInt32(dr[0]));
            }
            sqllist.Clear();
            
            for (int i = 0; i < WeatherList.Count; i++)
            {
                var r = WeatherList[i];
                sqllist.Add(
                    string.Format(
                        "update ForecastWeather set dtime3hour='{0}' , temperature='{1}',rains='{2}',wind='{3}',windd='{4}',qy='{5}',yl='{6}',njd='{7}',xdsd='{8}',timedate7='{10}',timehour7='{11}' where ID={9} ",
                        r.dateTime, r.temperature, r.rains, r.wind, r.windd, r.qy, r.yl, r.njd, r.xdsd, IDs[i], r.timedate7, r.timehour7));
            }
            Common.DBHander.insertToAccessByBatch(sqllist);
        }

        //实际上只有最近23个小时的数据
        public void Savelast24hMsg(string url, int AreaID, out bool isSaved24hours)
        {
            isSaved24hours = true;
            List<OneHourWeather> Last23h = (List<OneHourWeather>)getWeatherObj.Get24HourWeather(url);
            foreach (var r in Last23h)
            {
                DataTable dt = Common.DBHander.ReturnDataSet("select ID from OneHourWeather where Hourtime=#" + r.time + "# and AreaID=" + AreaID).Tables[0];
                r.timedate24 = r.time.Split(' ')[0].ToString();
                r.timehour24 = r.time.Split(' ')[1].ToString();

                if (dt.Rows.Count == 0)
                {
                    //插入这个时间的数据
                    string sql =
                        string.Format(
                            "insert into OneHourWeather (Hourtime,rain1h,rain12h,rain6h,rain24h,temperature,pressure,humidity,windDirection,windSpeed,AreaID,timedate24,timehour24) values(#{0}#,'{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}',#{11}#,#{12}#) ",
                            r.time, r.rain1h, r.rain12h, r.rain6h, r.rain24h, r.temperature, r.pressure, r.humidity,
                            r.windDirection, r.windSpeed, AreaID, r.timedate24, r.timehour24);
                    bool succes = Common.DBHander.ExeSQL(sql);
                    if (!succes)
                    {
                        Console.WriteLine(r.time + " 的天气信息存入失败!___" + DateTime.Now);
                        isSaved24hours = false;
                    }
                    else
                    {
                        Console.WriteLine(AreaID + "---" + r.time + " 的天气信息存入成功!___" + DateTime.Now);
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
