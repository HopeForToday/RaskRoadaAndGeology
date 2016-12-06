using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadRaskEvaltionSystem.RouteUIDeal
{
    /// <summary>
    /// 点位远离公路异常类
    /// 2016/12/6 fhr
    /// </summary>
    class PointIsFarException : Exception
    {
        public PointIsFarException(string message)
            : base(message)
        {

        }
    }
}
