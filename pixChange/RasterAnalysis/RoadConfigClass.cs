using RoadRaskEvaltionSystem.HelperClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoadRaskEvaltionSystem.RasterAnalysis
{
    class RoadConfigClass:IRoadRisk
    {
        public IDictionary<int, RoadRange> GetRoadRiskLevelFromConfig()
        {
            IDictionary<int, RoadRange> roadRanges = new Dictionary<int, RoadRange>();
            string configStr = ConfigHelper.ReadAppConfig("RoadRiskLevel");
            if(!String.IsNullOrEmpty(configStr))
            {
                string[] strArray = configStr.Split(',');
                List<double> numbers = new List<double>();
                foreach(var v in strArray)
                {
                    double temp = double.Parse(v);
                    numbers.Add(temp);
                }
                for (int i = 0; i < numbers.Count - 1; i+=2)
                {
                    RoadRange roadRange = new RoadRange(numbers[i], numbers[i + 1]);
                    roadRanges.Add(i + 1, roadRange);
                }
            }
            return roadRanges;
        }


        public void UpdateRoadRiskLevelToConfig(IDictionary<int, RoadRange> roadRanges)
        {
            int[] keys = roadRanges.Keys.ToArray();
            Array.Sort(keys);
            StringBuilder builder = new StringBuilder();
            for(int i=0;i<keys.Length;i++)
            {
                RoadRange range = roadRanges[keys[i]];
                builder.Append(range.MinValue.ToString()+','+range.MaxValue);
                if(i!=(keys.Length-1))
                {
                    builder.Append(',');
                }
            }
            ConfigHelper.UpdateAppConfig("RoadRiskLevel", builder.ToString());
        }
    }
}
