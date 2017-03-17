using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadRaskEvaltionSystem
{
    public enum CustomTool
    {
        None = 0,
        ZoomIn = 1,
        ZoomOut = 2,
        Pan = 3,
        RuleMeasure = 4,
        AreaMeasure = 5,
        PointSelect = 6,
        RectSelect = 7,
        PolygonSelect = 8,
        CircleSelect = 9,
        NAanalysis = 10,
        StartEditing = 11,
        SelectFeature = 12,
        MoveFeature = 13,
        EditVertex = 14,
        EditUndo = 15,
        EditRedo = 16,
        EditDeleteFeature = 17,
        EditAttribute = 18
    };
}
