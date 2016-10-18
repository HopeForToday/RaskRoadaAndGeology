using HtmlAgilityPackDemo1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadRaskEvaltionSystem.WeatherHander
{
    public interface IGetWeather
    {
        /// <summary>
        /// 获取预报信息
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        List<forecastWeatherMesg> getforcastMessage(string url);
        /// <summary>
        /// 获取过去24小时的天气详细信息
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
         object Get24HourWeather(string url);
    }
}
