using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using RoadRaskEvaltionSystem.WeatherHander;
using System.Threading;

namespace HtmlAgilityPackDemo1
{
    public class GetWeatherMessage : IGetWeather
    {
       
        //获取预报信息
        public List<forecastWeatherMesg> getforcastMessage(string url)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = web.Load(url);
            HtmlNode rootnode = doc.DocumentNode;
            var htmls = doc.DocumentNode.SelectNodes("//*[@id='hour3']/div[@class='hour3']");

            do
            {
                doc = web.Load(url);
                htmls = doc.DocumentNode.SelectNodes("//*[@id='hour3']/div[@class='hour3']");
            } while (htmls == null);

            List<forecastWeatherMesg> forWeMsgList = new List<forecastWeatherMesg>();
            for (int k = 1; k <= htmls.Count; k++)
            {
                var item = htmls[k - 1];
                var xpath = item.XPath;
                //时间段
                var datetimes = item.SelectNodes(item.XPath + "/div[@class='row first']/div");
                //降水信息
                var rains = item.SelectNodes(item.XPath + "/div[@class='row js']/div");
                //温度信息
                var temps = item.SelectNodes(item.XPath + "/div[@class='row wd']/div");
                //风速
                var winds = item.SelectNodes(item.XPath + "/div[@class='row winds']/div");
                //风向
                var windd = item.SelectNodes(item.XPath + "/div[@class='row windd']/div");
                //气压
                var qy = item.SelectNodes(item.XPath + "/div[@class='row qy']/div");
                //相对湿度
                var xdsd = item.SelectNodes(item.XPath + "/div[@class='row xdsd']/div");
                //云量
                var yl = item.SelectNodes(item.XPath + "/div[@class='row yl']/div");
                //能见度
                var njd = item.SelectNodes(item.XPath + "/div[@class='row njd']/div");

                for (int i = 1; i < datetimes.Count; i++)
                {
                    forecastWeatherMesg fm = new forecastWeatherMesg();
                    fm.dateTime = datetimes[i].InnerText.Replace("\n", "").Trim();
                    forWeMsgList.Add(fm);
                }

                for (int j = 1; j < datetimes.Count; j++)
                {
                    string oritime = datetimes[j].InnerText.Replace("\n", "").Trim();
                    string alltime = "";
                    if (k==1)
                    {
                        if (j==1)
                        {
                            if (oritime.Contains("日"))
                            {
                                alltime = DateTime.Today.ToString("yyyy-MM-") + oritime.Replace("日", " ");
                                forWeMsgList[j - 1].timedate7 = alltime.Split(' ')[0].ToString();
                                forWeMsgList[j - 1].timehour7 = alltime.Split(' ')[1].ToString();
                            }
                            else
                            {
                                if (oritime.Equals("02:00"))
                                {
                                    if (int.Parse(DateTime.Now.ToString("HH")) == 23)
                                    {
                                        alltime = (DateTime.Today.AddDays(k).ToString("yyyy-MM-dd") + " " + oritime).ToString();
                                        forWeMsgList[j - 1].timedate7 = alltime.Split(' ')[0];
                                        forWeMsgList[j - 1].timehour7 = alltime.Split(' ')[1];
                                    }
                                    else
                                    {
                                        alltime = (DateTime.Today.AddDays(k - 1).ToString("yyyy-MM-dd") + " " + oritime).ToString();
                                        forWeMsgList[j - 1].timedate7 = alltime.Split(' ')[0];
                                        forWeMsgList[j - 1].timehour7 = alltime.Split(' ')[1];
                                    }
                                }
                                else
                                {
                                    alltime = (DateTime.Today.AddDays(k - 1).ToString("yyyy-MM-dd") + " " + oritime).ToString();
                                    forWeMsgList[j - 1].timedate7 = alltime.Split(' ')[0];
                                    forWeMsgList[j - 1].timehour7 = alltime.Split(' ')[1];
                                }
                            } 
                        }
                        else
                        {
                            if (oritime.Contains("日"))
                            {
                                if (forWeMsgList[j - 2].timedate7.Split('-')[1].Split('-')[0].Equals(Convert.ToDateTime(forWeMsgList[j - 2].timedate7).AddDays(1).ToString("yyyy-MM-dd").Split('-')[1].Split('-')[0]))
                                {
                                    alltime = Convert.ToDateTime(forWeMsgList[j - 2].timedate7).ToString("yyyy-MM-") + oritime.Replace("日", " ");
                                    forWeMsgList[j - 1].timedate7 = alltime.Split(' ')[0].ToString();
                                    forWeMsgList[j - 1].timehour7 = alltime.Split(' ')[1].ToString(); 
                                }
                                else
                                {
                                    alltime = Convert.ToDateTime(forWeMsgList[j - 2].timedate7).AddDays(1).ToString("yyyy-MM-") + oritime.Replace("日", " ");
                                    forWeMsgList[j - 1].timedate7 = alltime.Split(' ')[0].ToString();
                                    forWeMsgList[j - 1].timehour7 = alltime.Split(' ')[1].ToString(); 
                                }
                            }
                            else
                            {
                                alltime = (Convert.ToDateTime(forWeMsgList[j - 2].timedate7).ToString("yyyy-MM-dd") + " " + oritime).ToString();
                                forWeMsgList[j - 1].timedate7 = alltime.Split(' ')[0].ToString();
                                forWeMsgList[j - 1].timehour7 = alltime.Split(' ')[1].ToString();
                            } 
                        }
                    }
                    else if(k>1)
                    {
                        if (j==1)
	                    {
		                    if (oritime.Contains("日"))
                            {
                                alltime = Convert.ToDateTime(forWeMsgList[8 * (k - 1) - 1 + (j - 1)].timedate7).ToString("yyyy-MM-") + oritime.Replace("日", " ");
                                forWeMsgList[8 * (k - 1)].timedate7 = alltime.Split(' ')[0].ToString();
                                forWeMsgList[8 * (k - 1)].timehour7 = alltime.Split(' ')[1].ToString();
                            }
                            else
                            {
                                alltime = (Convert.ToDateTime(forWeMsgList[8 * (k - 1) - 1 + (j - 1)].timedate7).ToString("yyyy-MM-dd") + " " + oritime).ToString();
                                forWeMsgList[8 * (k - 1) - 1 + j].timedate7 = alltime.Split(' ')[0];
                                forWeMsgList[8 * (k - 1) - 1 + j].timehour7 = alltime.Split(' ')[1];
                            } 
	                    }   
                        else
	                    {
                            if (oritime.Contains("日"))
                            {
                                alltime = Convert.ToDateTime(forWeMsgList[8 * (k - 1) - 1 + (j - 1)].timedate7).ToString("yyyy-MM-") + oritime.Replace("日", " ");
                                forWeMsgList[8 * (k - 1) - 1 + j].timedate7 = alltime.Split(' ')[0].ToString();
                                forWeMsgList[8 * (k - 1) - 1 + j].timehour7 = alltime.Split(' ')[1].ToString();
                            }
                            else
                            {
                                alltime = (Convert.ToDateTime(forWeMsgList[8 * (k - 1) - 1 + (j - 1)].timedate7).ToString("yyyy-MM-dd") + " " + oritime).ToString();
                                forWeMsgList[8 * (k - 1) - 1 + j].timedate7 = alltime.Split(' ')[0];
                                forWeMsgList[8 * (k - 1) - 1 + j].timehour7 = alltime.Split(' ')[1];
                            } 
	                    }
                    }
                    //if (j==1)
                    //{
                    //    if (oritime.Contains("日"))
                    //    {
                    //        alltime = DateTime.Today.ToString("yyyy-MM-") + oritime.Replace("日", " ");
                    //        //alltime = oritime.Insert(3, " ").ToString();
                    //        forWeMsgList[8 * k + j - 9].timedate7 = alltime.Split(' ')[0].ToString();
                    //        forWeMsgList[8 * k + j - 9].timehour7 = alltime.Split(' ')[1].ToString();
                    //    }
                    //    else
                    //    {
                    //        if (oritime.Equals("02:00"))
                    //        {
                    //            if (int.Parse(DateTime.Now.ToString("HH"))==23)
                    //            {
                    //                alltime = (DateTime.Today.AddDays(k).ToString("yyyy-MM-dd") + " " + oritime).ToString();
                    //                forWeMsgList[8 * k + j - 9].timedate7 = alltime.Split(' ')[0];
                    //                forWeMsgList[8 * k + j - 9].timehour7 = alltime.Split(' ')[1]; 
                    //            }
                    //            else
                    //            {
                    //                alltime = (DateTime.Today.AddDays(k - 1).ToString("yyyy-MM-dd") + " " + oritime).ToString();
                    //                forWeMsgList[8 * k + j - 9].timedate7 = alltime.Split(' ')[0];
                    //                forWeMsgList[8 * k + j - 9].timehour7 = alltime.Split(' ')[1]; 
                    //            }
                    //        }
                    //        else
                    //        {
                    //            alltime = (DateTime.Today.AddDays(k - 1).ToString("yyyy-MM-dd") + " " + oritime).ToString();
                    //            forWeMsgList[8 * k + j - 9].timedate7 = alltime.Split(' ')[0];
                    //            forWeMsgList[8 * k + j - 9].timehour7 = alltime.Split(' ')[1];
                    //        }

                    //    }
                    //}
                    //else if (j>1)
                    //{
                    //    if (oritime.Contains("日"))
                    //    {
                    //        alltime = DateTime.Today.ToString("yyyy-MM-") + oritime.Replace("日", " ");
                    //        //alltime = oritime.Insert(3, " ").ToString();
                    //        forWeMsgList[8 * k + j - 9].timedate7 = alltime.Split(' ')[0];
                    //        forWeMsgList[8 * k + j - 9].timehour7 = alltime.Split(' ')[1];
                    //    }
                    //    else
                    //    {

                    //        if (k>2)
                    //        {
                    //            alltime = (forWeMsgList[8 * (k - 1) - 1].timedate7 + " " + oritime).ToString();
                    //            forWeMsgList[8 * k + j - 9].timedate7 = alltime.Split(' ')[0];
                    //            forWeMsgList[8 * k + j - 9].timehour7 = alltime.Split(' ')[1]; 
                    //        }
                    //        else
                    //        {
                    //            alltime = (forWeMsgList[8 * k + (j - 1) - 9].timedate7 + " " + oritime).ToString();
                    //            forWeMsgList[8 * k + j - 9].timedate7 = alltime.Split(' ')[0];
                    //            forWeMsgList[8 * k + j - 9].timehour7 = alltime.Split(' ')[1]; 
                    //        }
                    //    } 
                    //}
                }

                for (int j = 1; j < rains.Count; j++)
                {
                    if (rains[j].InnerText.Replace("\n", "").Trim().Equals("无降水"))
                    {
                        forWeMsgList[8 * k + j - 9].rains = 0.0f;
                    }
                    else
                    {
                        forWeMsgList[8 * k + j - 9].rains = float.Parse(rains[j].InnerText.Replace("\n", "").Trim().Split('毫')[0].ToString());
                    }
                }
                for (int j = 1; j < temps.Count; j++)
                {
                    forWeMsgList[8 * k + j - 9].temperature = float.Parse(temps[j].InnerText.Replace("\n", "").Trim().Split('℃')[0].ToString());
                }
                for (int j = 1; j < winds.Count; j++)
                {
                    forWeMsgList[8 * k + j - 9].wind = float.Parse(winds[j].InnerText.Replace("\n", "").Trim().Split('米')[0].ToString());
                }
                for (int j = 1; j < windd.Count; j++)
                {
                    forWeMsgList[8 * k + j - 9].windd = windd[j].InnerText.Replace("\n", "").Trim();
                }

                for (int j = 1; j < qy.Count; j++)
                {
                    forWeMsgList[8 * k + j - 9].qy = qy[j].InnerText.Replace("\n", "").Trim();
                }
                for (int j = 1; j < xdsd.Count; j++)
                {
                    forWeMsgList[8 * k + j - 9].xdsd = float.Parse(xdsd[j].InnerText.Replace("\n", "").Trim().Split('%')[0].ToString());
                }
                for (int j = 1; j < yl.Count; j++)
                {
                    forWeMsgList[8 * k + j - 9].yl = float.Parse(yl[j].InnerText.Replace("\n", "").Trim().Split('%')[0].ToString());
                }
                for (int j = 1; j < njd.Count; j++)
                {
                    forWeMsgList[8 * k + j - 9].njd = njd[j].InnerText.Replace("\n", "").Trim();
                }

            }


            return forWeMsgList;
        }

        //获取过去24小时的天气详细信息
        public object Get24HourWeather(string url)
        {
            Uri uil = new Uri(url);
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(uil);
            System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
            System.IO.Stream responseStream = response.GetResponseStream();
            System.IO.StreamReader sr = new System.IO.StreamReader(responseStream, System.Text.Encoding.GetEncoding("utf-8"));
            string responseText = sr.ReadToEnd();
            sr.Close();
            sr.Dispose();
            responseStream.Close();
            string jsonData = responseText;
            var list = JsonUtility.Instance.JsonToObjectList<OneHourWeather>(jsonData);
            return list;
        }

        public static void test()
        {
            //Uri url =new Uri("http://www.nmc.cn/f/rest/passed/56079");  http://www.nmc.cn/publish/forecast/ASC/ruoergai.html
            Uri url = new Uri("http://www.nmc.cn/publish/forecast/ASC/ruoergai.html");
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(url);
            System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
            System.IO.Stream responseStream = response.GetResponseStream();
            System.IO.StreamReader sr = new System.IO.StreamReader(responseStream, System.Text.Encoding.GetEncoding("utf-8"));
            string responseText = sr.ReadToEnd();
            sr.Close();
            sr.Dispose();
            responseStream.Close();
            string jsonData = responseText;
        }
        static CookieContainer GetCookie(string postString, string postUrl)
        {

            CookieContainer cookie = new CookieContainer();

            HttpWebRequest httpRequset = (HttpWebRequest)HttpWebRequest.Create(postUrl);//创建http 请求
            httpRequset.CookieContainer = cookie;//设置cookie
            httpRequset.Method = "POST";//POST 提交
            httpRequset.KeepAlive = true;
            httpRequset.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko";
            httpRequset.Accept = "text/html, application/xhtml+xml, */*";
            httpRequset.ContentType = "application/x-www-form-urlencoded";//以上信息在监听请求的时候都有的直接复制过来
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(postString);
            httpRequset.ContentLength = bytes.Length;
            Stream stream = httpRequset.GetRequestStream();
            stream.Write(bytes, 0, bytes.Length);
            stream.Close();//以上是POST数据的写入

            HttpWebResponse httpResponse = (HttpWebResponse)httpRequset.GetResponse();//获得 服务端响应
            return cookie;//拿到cookie
        }
        static string GetContent(CookieContainer cookie, string url)
        {
            string content;
            HttpWebRequest httpRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            httpRequest.CookieContainer = cookie;
            httpRequest.Referer = url;
            httpRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko";
            httpRequest.Accept = "text/html, application/xhtml+xml, */*";
            httpRequest.ContentType = "application/x-www-form-urlencoded";
            httpRequest.Method = "GET";

            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();

            using (Stream responsestream = httpResponse.GetResponseStream())
            {

                using (StreamReader sr = new StreamReader(responsestream, System.Text.Encoding.UTF8))
                {
                    content = sr.ReadToEnd();
                }
            }

            return content;
        }
    }
}
