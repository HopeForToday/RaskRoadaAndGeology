using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadRaskEvaltionSystem.RouteAnalysis
{
    interface IRouteConfig
    {
        //根据objectID查询绕行路线的编号 
        bool QueryGoodRouteIndex(int objectID);
    }
}
