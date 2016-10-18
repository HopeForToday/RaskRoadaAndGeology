using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadRaskEvaltionSystem.WeatherHander
{
    public interface ISaveWeather
    {
        /// <summary>
        /// 保存24小时数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="AreaID"></param>
          void Savelast24hMsg(string url, int AreaID);
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="AreaID"></param>
          void SaveForeacastWerherMsg(string url, int AreaID);

    }
}
