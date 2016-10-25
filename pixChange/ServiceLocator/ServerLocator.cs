using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoadRaskEvaltionSystem.RasterAnalysis;
using RoadRaskEvaltionSystem.WeatherHander;
using HtmlAgilityPackDemo1;
using RoadRaskEvaltionSystem.RouteAnalysis;
namespace RoadRaskEvaltionSystem.ServiceLocator
{
    /// <summary>
    /// 采用工厂和单例实现的注入类
    /// </summary>
    class ServerLocator
    {
        private static IRoadRaskCaculate m_RoadRaskCacluate = null;
        private static IRoadRiskConfig m_RoadConfig = null;
        private static ISaveWeather m_SaverWeather = null;
        private static IGetWeather m_GetWeather = null;
        private static IRouteConfig routeConfig = null;
        private static IRouteDecide routeDecide = null;
        public static IRoadRaskCaculate GetIRoadRaskCaculate()
        {
            if(m_RoadRaskCacluate==null)
            {
                m_RoadRaskCacluate = new ToRasterControl(ServerLocator.GetRoadRiskConfig());
            }
            return m_RoadRaskCacluate;
        }
        public static IRoadRiskConfig GetRoadRiskConfig()
        {

            if (m_RoadConfig == null)
            {
                m_RoadConfig = new RoadConfigClass();
            }
            return m_RoadConfig;
        }
        public static IGetWeather GetWeather()
        {
            if(m_GetWeather==null)
            {
                m_GetWeather = new GetWeatherMessage();
            }
            return m_GetWeather;
        }
        public static ISaveWeather GetSaveWeather()
        {
            if(m_SaverWeather==null)
            {
                m_SaverWeather = new SaveWeatherMsg(GetWeather());
            }
            return m_SaverWeather;
        }
        public static IRouteConfig GetRoutConfig()
        {
            if (routeConfig == null)
            {
                routeConfig = new RouteConfigClass();
            }
            return routeConfig;
        }
        public static IRouteDecide GetRouteDecide()
        {
            if (routeDecide == null)
            {
                routeDecide = new RouteDecideClass(GetRoutConfig());
            }
            return routeDecide;
        }
    }
}
