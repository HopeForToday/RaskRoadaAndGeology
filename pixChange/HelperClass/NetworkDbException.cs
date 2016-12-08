using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadRaskEvaltionSystem.HelperClass
{
    /// <summary>
    /// 网络数据集异常类
    /// 2016/12/6 fhr 
    /// </summary>
    class NetworkDbException:Exception
    {
        public NetworkDbException(string message)
            : base(message)
        {
        }
    }
}
