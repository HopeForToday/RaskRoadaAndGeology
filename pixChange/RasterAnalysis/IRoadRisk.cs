using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoadRaskEvaltionSystem.RasterAnalysis
{
    //获取和修改道路风险配置
    interface IRoadRisk
    {
        IDictionary<int, RoadRange> GetRoadRiskLevelFromConfig();
        void UpdateRoadRiskLevelToConfig(IDictionary<int, RoadRange> roadRanges);
    }
}
