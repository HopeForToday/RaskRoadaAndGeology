using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadRaskEvaltionSystem.HelperClass
{
    class MapUtil
    {
        //加载地图mxd
        public static bool LoadMxd(AxMapControl mapControl, string mapFileName)
        {
            if (mapControl.CheckMxFile(mapFileName))
            {
                mapControl.LoadMxFile(mapFileName);
                return true;
            }
            return false;
        }
        public static bool SaveMxd(AxMapControl mapControl)
        {
            if (mapControl.DocumentFilename != null)
            {
                IMapDocument mapDoc = new MapDocumentClass();
                mapDoc.Open(mapControl.DocumentFilename, string.Empty);
                mapDoc.ReplaceContents((IMxdContents)mapControl.Map);
                mapDoc.Save(mapDoc.UsesRelativePaths,false);
                mapDoc.Close();
                return true;
            }
            return false;
        }
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
            try
            {
                IMapDocument mapDoc = new MapDocumentClass();
                mapDoc.Open(mapFileName, "");
                mapDoc.ReplaceContents(map as IMxdContents);
                mapDoc.Save(mapDoc.UsesRelativePaths, false);
                mapDoc.Close();
            }
            catch (Exception e)
            {

            }
        }
    }
}
