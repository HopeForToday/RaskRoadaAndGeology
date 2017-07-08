using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadRaskEvaltionSystem
{
    /// <summary>
    /// 自定义工具类型枚举 用于MapControl
    /// </summary>
    public enum CustomTool
    {
        None = 0,
        RectSelect = 7,
        PolygonSelect = 8,
        CircleSelect = 9,
        StopInsert=11,
        BarryInsert=12,
        StopRemove=13,
        BarryRemove=14
    };
}
