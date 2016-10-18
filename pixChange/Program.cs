using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Data.XtraReports.Wizard.Native;
using ESRI.ArcGIS.esriSystem;
using RoadRaskEvaltionSystem;
using RoadRaskEvaltionSystem.HelperClass;
using RoadRaskEvaltionSystem.WeatherHander;
using RoadRaskEvaltionSystem.ServiceLocator;

namespace pixChange
{
    delegate bool GetWeatherHandler(); //申明一个委托，表明需要在子线程上执行的方法的函数签名
     class Program
    {
        private static LicenseInitializer m_AOLicenseInitializer = new pixChange.LicenseInitializer();
        //把委托和具体的方法关联起来
        private  static GetWeatherHandler calcMethod = new GetWeatherHandler(getWeatherData);
         //获取保存天气的依赖字段
        private static  ISaveWeather saveWeather = ServerLocator.GetSaveWeather();
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //ESRI License Initializer generated code.
            m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeEngine },
            new esriLicenseExtensionCode[] { esriLicenseExtensionCode.esriLicenseExtensionCode3DAnalyst, esriLicenseExtensionCode.esriLicenseExtensionCodeNetwork, esriLicenseExtensionCode.esriLicenseExtensionCodeSpatialAnalyst, esriLicenseExtensionCode.esriLicenseExtensionCodeSchematics, esriLicenseExtensionCode.esriLicenseExtensionCodeMLE, esriLicenseExtensionCode.esriLicenseExtensionCodeDataInteroperability, esriLicenseExtensionCode.esriLicenseExtensionCodeTracking });
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            getWeatherData();//这个地方要改成异步的
            Application.Run(new MainFrom());
            //ESRI License Initializer generated code.
            //Do not make any call to ArcObjects after ShutDownApplication()
            //http://www.nmc.cn/publish/forecast/ASC/lushan.html   芦山  
            
            //http://www.nmc.cn/f/rest/passed/56279
            //http://www.nmc.cn/publish/forecast/ASC/baoxing.html 宝兴  http://www.nmc.cn/f/rest/passed/56273
            m_AOLicenseInitializer.ShutdownApplication();
        }

        public static  bool getWeatherData()
        {
            try
            {
                saveWeather.SaveForeacastWerherMsg("http://www.nmc.cn/publish/forecast/ASC/lushan.html", 1);
                Common.DBHander.coloseCon(); //来不及数据库连接
                saveWeather.SaveForeacastWerherMsg("http://www.nmc.cn/publish/forecast/ASC/baoxing.html", 2);
                Common.DBHander.coloseCon();
                saveWeather.Savelast24hMsg("http://www.nmc.cn/f/rest/passed/56279", 1);
                Common.DBHander.coloseCon();
                saveWeather.Savelast24hMsg("http://www.nmc.cn/f/rest/passed/56273", 2);
                Common.DBHander.coloseCon();
            }
            catch (Exception ex)
            {
               Console.WriteLine("天气信息提取异常！");
                return false;
            }
            return true;
        }
    }
}