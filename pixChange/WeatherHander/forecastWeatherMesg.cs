using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlAgilityPackDemo1
{
    //提交测试1
    //提交测试2肥嘟嘟
    public class forecastWeatherMesg
    {
       //每个3小时的整点数  存在 16日02:00 这样的数据，可以作为日期划分的依据
        public string dateTime { get; set; }
       //气温
        public float temperature { get; set; }
        //雨量
        public float rains { get; set; }
        /// <summary>
        /// 风速
        /// </summary>
        public float wind { get; set; }
        //风速
        public string windd { get; set; }
        //气压
        public string qy { get; set; }
        //云量
        public float yl { get; set; }
        //能见度
        public string njd { get; set; }
        //相对湿度
        public float xdsd { get; set; }

        public string timedate7 { get; set; }
        public string timehour7 { get; set; }
    }
   
    
}

