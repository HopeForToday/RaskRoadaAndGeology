using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlAgilityPackDemo1
{
    public class forecastWeatherMesg
    {
       //每个3小时的整点数  存在 16日02:00 这样的数据，可以作为日期划分的依据
        public string dateTime { get; set; }
       //气温
        public string temperature { get; set; }
        //雨量
        public string rains { get; set; }
        /// <summary>
        /// 风速
        /// </summary>
        public string wind { get; set; }
        //风速
        public string windd { get; set; }
        //气压
        public string qy { get; set; }
        //云量
        public string yl { get; set; }
        //能见度
        public string njd { get; set; }
        //相对湿度
        public string xdsd { get; set; }
    }
   
    
}

