using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RoadRaskEvaltionSystem.HelperClass
{
    public sealed class IPictureConverter : AxHost
    {
        private IPictureConverter() : base("") { }

        #region IPictureDisp
        public static stdole.IPictureDisp ImageToIPictureDisp(Image image)
        {
            return (stdole.IPictureDisp)GetIPictureDispFromPicture(image);
        }

        public static Image IPictureDispToImage(stdole.IPictureDisp pictureDisp)
        {
            return GetPictureFromIPictureDisp(pictureDisp);
        }
        #endregion

        #region IPicture
        public static stdole.IPicture ImageToIPicture(Image image)
        {
            return (stdole.IPicture)GetIPictureFromPicture(image);
        }

        public static Image IPictureToImage(stdole.IPicture picture)
        {
            return GetPictureFromIPicture(picture);
        }
        #endregion
    }
}
