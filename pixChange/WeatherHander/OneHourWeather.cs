using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoadRaskEvaltionSystem.WeatherHander
{
  public class OneHourWeather
    {
        //为什么不是"rain1h":"0.0",这种形式？
        //{"rain1h":0.0,"rain24h":0.0,"rain12h":0.0,"rain6h":0.0,"temperature":17.0,"humidity":39.0,"pressure":672.0,"windDirection":48.0,"windSpeed":2.8,"time":"2016-09-16 14:00"}
      public string rain1h { get; set; }
      public string rain24h { get; set; }
      public string rain12h { get; set; }
      public string rain6h { get; set; }
      public string temperature { get; set; }
      public string humidity { get; set; }
      public string pressure { get; set; }
      public string windDirection { get; set; }
      public string windSpeed { get; set; }  
      public string time { get; set; }
     

    }
}
