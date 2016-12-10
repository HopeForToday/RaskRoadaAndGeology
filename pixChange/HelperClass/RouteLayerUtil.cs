using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using pixChange.HelperClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            //四级公路符号
            ILineSymbol pOutline4 = new SimpleLineSymbolClass();
            //三级公路符号
            ILineSymbol pOutline3 = new SimpleLineSymbolClass();
            //等外公路符号
            ILineSymbol pOutlineEqual = new SimpleLineSymbolClass();
            //其它公路符号
            ILineSymbol pOutlineOther = new SimpleLineSymbolClass();
            pOutline3.Color = SymbolUtil.GetColor(0, 0, 0);
            pOutline3.Width = 2.5;
            pOutline4.Color = SymbolUtil.GetColor(0, 0, 0);
            pOutline4.Width = 2;
            pOutlineEqual.Color = SymbolUtil.GetColor(0, 0, 0);
            pOutlineEqual.Width = 1.5;
            pOutlineOther.Color = SymbolUtil.GetColor(0, 0, 0);
            pOutlineOther.Width =1;
            IDictionary<string, ISymbol> symbolDic = new Dictionary<string, ISymbol>();
            symbolDic.Add("四级", pOutline3 as ISymbol);
            symbolDic.Add("三级", pOutline4 as ISymbol);
            symbolDic.Add("等外", pOutlineEqual as ISymbol);
            symbolDic.Add("其他", pOutlineOther as ISymbol);
            LayerManager.SetLayerGraderSymbol(layer, "RTEG", symbolDic);
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
