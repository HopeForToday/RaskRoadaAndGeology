using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RoadRaskEvaltionSystem.HelperClass
{
    /// <summary>
    /// 岩性图片工具类
    /// </summary>
    class LithologyImageUtils
    {
        public static string getImagePath(string pictureName)
        {
            try
            {
                return Application.StartupPath + @"\Images\" + pictureName + ".png";
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
