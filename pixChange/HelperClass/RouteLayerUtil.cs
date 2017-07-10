using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using pixChange.HelperClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RoadRaskEvaltionSystem.HelperClass
{
    /// <summary>
    /// 公路网图层帮助类
    /// </summary>
    class RouteLayerUtil
    {
        /// <summary>
        /// 设置公路网图层样式
        /// </summary>
        /// <param name="?"></param>
        public static void SetRouteLayerStyle(ILayer layer)
        {
            SetRouteLayerSymbolGrade(layer);
            ShowRouteName(layer);
           // SetLayerTran(layer);
        }
        /// <summary>
        /// 设置公路网图层按照公路等级分级显示
        /// </summary>
        /// <param name="layer"></param>
        public  static void SetRouteLayerSymbolGrade(ILayer layer)
        {
            var symbolDic = GetSymbolsFromConfig();
            if (symbolDic == null)
            {
                symbolDic=GetDefaultSymbols();
            }
            LayerManager.SetLayerGraderSymbol(layer, "RTEG", symbolDic);
        }
        /// <summary>
        /// 缺省样式
        /// </summary>
        /// <returns></returns>
        private static IDictionary<string, ISymbol> GetDefaultSymbols()
        {
            //一级公路符号
            ILineSymbol pOutline1 = new SimpleLineSymbolClass();
            //二级公路符号
            ILineSymbol pOutline2 = new SimpleLineSymbolClass();
            //三级公路符号
            ILineSymbol pOutline3 = new SimpleLineSymbolClass();
            //四级公路符号
            ILineSymbol pOutline4 = new SimpleLineSymbolClass();
            //等外公路符号
            ILineSymbol pOutlineEqual = new SimpleLineSymbolClass();
            //其它公路符号
            ILineSymbol pOutlineOther = new SimpleLineSymbolClass();
            pOutline1.Color = SymbolUtil.GetColor(255, 0, 0);
            pOutline1.Width = 3.0;
            pOutline2.Color = SymbolUtil.GetColor(255, 125, 0);
            pOutline2.Width = 2.5;
            pOutline3.Color = SymbolUtil.GetColor(255, 220, 0);
            pOutline3.Width = 2.0;
            pOutline4.Color = SymbolUtil.GetColor(0, 255, 0);
            pOutline4.Width = 1.5;
            pOutlineEqual.Color = SymbolUtil.GetColor(0, 0, 255);
            pOutlineEqual.Width = 1.2;
            pOutlineOther.Color = SymbolUtil.GetColor(0, 255, 255);
            pOutlineOther.Width = 0.8;
            IDictionary<string, ISymbol> symbolDic = new Dictionary<string, ISymbol>();
            symbolDic.Add("一级", pOutline1 as ISymbol);
            symbolDic.Add("二级", pOutline2 as ISymbol);
            symbolDic.Add("三级", pOutline3 as ISymbol);
            symbolDic.Add("四级", pOutline4 as ISymbol);
            symbolDic.Add("等外", pOutlineEqual as ISymbol);
            symbolDic.Add("其他", pOutlineOther as ISymbol);
            return symbolDic;
        }

        /// <summary>
        /// 从配置文件中读取样式
        /// </summary>
        /// <returns></returns>
        private  static IDictionary<string, ISymbol> GetSymbolsFromConfig()
        {
            try
            {
                var symbolDic = new Dictionary<string, ISymbol>();
                var routeValues = ConfigHelper.ReadAppConfig("routenetvalues").Split(',');
                foreach (var routeValue in routeValues)
                {
                    var configParams = ConfigHelper.ReadAppConfig(string.Format("route_{0}", routeValue)).Split(',');
                    var red = int.Parse(configParams[0]);
                    var green = int.Parse(configParams[1]);
                    var blue = int.Parse(configParams[2]);
                    var width = double.Parse(configParams[3]);
                    var lineSymbol = new SimpleLineSymbolClass();
                    lineSymbol.Color = SymbolUtil.GetColor(red, green, blue);
                    lineSymbol.Width = width;
                    symbolDic.Add(routeValue, lineSymbol);
                }
                return symbolDic;
            }
            catch (Exception e)
            {
                MessageBox.Show("公路网样式配置错误");
                return null;
            }
          
        }

        /// <summary>
        /// 显示公路网名字
        /// </summary>
        /// <param name="layer"></param>
        public static void ShowRouteName(ILayer layer)
        {
            IFeatureLayer pFeatureLayer = layer as IFeatureLayer;
            LayerManager.EnableFeatureLayerLabel(pFeatureLayer,"NAME", SymbolUtil.GetColor(50, 50, 0), 10, "");
        }
        //设置公路网透明度
        public static void SetLayerTran(ILayer layer)
        {
            LayerManager.SetLayerTranspan(layer, 40);
        }
    }
}
