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
      public float rain1h { get; set; }
      public float rain24h { get; set; }
      public float rain12h { get; set; }
      public float rain6h { get; set; }
      public float temperature { get; set; }
      public float humidity { get; set; }
      public float pressure { get; set; }
      public float windDirection { get; set; }
      public float windSpeed { get; set; }  
      public string time { get; set; }
      public string timedate24 { get; set; }
      public string timehour24 { get; set; }
    }
}
