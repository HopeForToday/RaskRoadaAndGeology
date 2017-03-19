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
using Ling.cnzhnet;
using System.Diagnostics;
using log4net;
namespace pixChange
{
     class Program
    {
        private static readonly log4net.ILog logger = LogManager.GetLogger(typeof(Program));
        private static readonly AboutDevCompanion devCompanion=new  AboutDevCompanion(1, false);
        private static LicenseInitializer m_AOLicenseInitializer = new pixChange.LicenseInitializer();
        private static readonly ISaveWeather saveWeather = ServiceLocator.GetSaveWeather();
     
         /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            logger.Info("程序启动");
            #region fhr 通过毒手关闭Dev
            devCompanion.Run();
            #endregion
            //ESRI License Initializer generated code.
            m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeEngine },
            new esriLicenseExtensionCode[] { esriLicenseExtensionCode.esriLicenseExtensionCode3DAnalyst, esriLicenseExtensionCode.esriLicenseExtensionCodeNetwork, esriLicenseExtensionCode.esriLicenseExtensionCodeSpatialAnalyst, esriLicenseExtensionCode.esriLicenseExtensionCodeSchematics, esriLicenseExtensionCode.esriLicenseExtensionCodeMLE, esriLicenseExtensionCode.esriLicenseExtensionCodeDataInteroperability, esriLicenseExtensionCode.esriLicenseExtensionCodeTracking });
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            #region fhr  异步处理天气获取
            Thread getWeatherThread = new Thread(() =>
            {
                if (!getWeatherData())
                {
                    MessageBox.Show("天气信息获取异常,请检查网络连接或者联系管理员", "警告", MessageBoxButtons.OK);
                }
            });
            getWeatherThread.Start();
            #endregion
            Application.Run(new MainFrom());
            //ESRI License Initializer generated code.
            //Do not make any call to ArcObjects after ShutDownApplication()
            //http://www.nmc.cn/publish/forecast/ASC/lushan.html   芦山  
            
            //http://www.nmc.cn/f/rest/passed/56279
            //http://www.nmc.cn/publish/forecast/ASC/baoxing.html 宝兴  http://www.nmc.cn/f/rest/passed/56273
            m_AOLicenseInitializer.ShutdownApplication();

            // 关闭Dev检测程序
            devCompanion.Stop();
        }

        public static  bool getWeatherData()
        {
            bool isError = true;
            try
            {
                bool is7days_1, is7days_2, is24hours_1, is24hours_2;
                //更新庐山地区
                saveWeather.SaveForeacastWerherMsg("http://www.nmc.cn/publish/forecast/ASC/lushan.html", 1, out is7days_1);
                Common.DBHander.coloseCon(); //来不及数据库连接
               //更新宝新
                saveWeather.SaveForeacastWerherMsg("http://www.nmc.cn/publish/forecast/ASC/baoxing.html", 2, out is7days_2);
                Common.DBHander.coloseCon();
                saveWeather.Savelast24hMsg("http://www.nmc.cn/f/rest/passed/56279", 1, out is24hours_1);
                Common.DBHander.coloseCon();
                saveWeather.Savelast24hMsg("http://www.nmc.cn/f/rest/passed/56273", 2, out is24hours_2);
                Common.DBHander.coloseCon();
                if (!is7days_1 || !is7days_2 || !is24hours_1 || !is24hours_2)
                {
                    isError = false;
                }
            }
            catch (Exception ex)
            {
               logger.Error("天气信息提取异常",ex);
               Debug.Print("天气信息提取异常！");
               isError = false;
            }
            return isError;
        }
    }
}