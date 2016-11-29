using ESRI.ArcGIS.Carto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadRaskEvaltionSystem.HelperClass
{
    class MapUtil
    {
        //打开地图  
        public static  IMap OpenMap(string mapFileName)
        {
            IMapDocument mapDoc = new MapDocumentClass();
            mapDoc.Open(mapFileName, "");
            return mapDoc.get_Map(0);
        }
        //保存地图
        public static void SaveMap(string mapFileName,IMap map)
        {
            IMapDocument mapDoc = new MapDocumentClass();
            mapDoc.Open(mapFileName, "");
            mapDoc.ReplaceContents(map as IMxdContents);
            mapDoc.Save(mapDoc.UsesRelativePaths, false);
            mapDoc.Save();
        }
    }
}
