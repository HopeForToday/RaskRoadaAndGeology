using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoadRaskEvaltionSystem.RasterAnalysis
{
    public class RoadRange
    {
   
            public double MinValue { get; set; }
            public double MaxValue { get; set; }
            public RoadRange(double minValue, double maxValue)
            {
                this.MinValue = minValue;
                this.MaxValue = maxValue;
            }
    }
}
