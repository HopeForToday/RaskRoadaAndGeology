using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SystemUI;

using pixChange.HelperClass;
using RoadRaskEvaltionSystem.RasterAnalysis;
using RoadRaskEvaltionSystem;
using RoadRaskEvaltionSystem.HelperClass;
using RoadRaskEvaltionSystem.RouteAnalysis;
using ESRI.ArcGIS.NetworkAnalyst;
using System.Threading.Tasks;
using System.Diagnostics;
using RoadRaskEvaltionSystem.RouteUIDeal;
using RoadRaskEvaltionSystem.QueryAndUIDeal;
using System.Threading;
using ESRI.ArcGIS.ADF.BaseClasses;
using RoadRaskEvaltionSystem.ComTools;

namespace pixChange
{
    public partial class MainFrom : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        //0为不插入 1为插入经过点 2为插入断点
        private int insertFlag = 0;
        //路线操作接口字段
        private IRouteDecide routeDecide = ServiceLocator.GetRouteDecide();
        //路线操作
        private IRouteUI routeUI = ServiceLocator.RouteUI;
        //空间查询操作
        private ISpatialQueryUI spatiallUI=ServiceLocator.SpatialQueryUI;
        //公路网图层
        private ILayer routeNetLayer = null;
        ////栅格接口类
        //IRoadRaskCaculate roadRaskCaculate = ServerLocator.GetIRoadRaskCaculate();
        //提交测试
        //公共变量用于表示整个系统都能访问的图层控件和环境变量
        //  public static SpatialAnalysisOption SAoption;
        public static IMapControl3 m_mapControl = null;
        public static ITOCControl2 m_pTocControl = null;
        public static ToolStripComboBox toolComboBox = null;
        public static IGroupLayer groupLayer = null;//数据分组
        public static string groupLayerName = null;
        public static int WhichChecked = 0;//记录哪一个模块被点击 1:基础数据 2:地质数据 3:公路数据 4:生态数据
        //用于判断当前鼠标点击的菜单命令,以备在地图控件中判断操作
        static public CustomTool m_cTool;
        IScreenDisplay m_focusScreenDisplay;// For 平移
        //For 放大,缩小，平移
        INewEnvelopeFeedback m_feedBack;//  '拉框
        IPoint m_mouseDownPoint;
        bool m_isMouseDown;
        public bool frmAttriQueryisOpen = false;


        //当前窗体实例
        public MainFrom pCurrentWin = null;
        //当前主地图控件实例
        public AxMapControl pCurrentMap = null;
        //当前鹰眼控件实例
        public AxMapControl pCurrentSmallMap = null;
        //当前TOC控件实例
        public AxTOCControl pCurrentTOC = null;
        #region ToolbarMenus
        private IToolbarMenu mapMenu;//toc控件右键地图菜单
        private IToolbarMenu layerMenu;//toc控件右键图层菜单
        #endregion

        #region Tools
        private StopsInsertTool stopsInsertTool = new StopsInsertTool();
        private BarrysInsertTool barrysInsertTool = new BarrysInsertTool();
        private StopsRemoveTool stopRemTool = new StopsRemoveTool();
        private BarrysRemoveTool barryRemTool = new BarrysRemoveTool();
        #endregion

        private ProInfoWindow infoWindow;
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
        public MainFrom()
        {
            InitializeComponent();
        }
        private void 图层管理_Click(object sender, EventArgs e)
        {
            //List<string>LayerPathList=new List<string>();
            LayerMangerView lm = new LayerMangerView();
            lm.Show();
        }

        private void MainFrom_Load(object sender, EventArgs e)
        {
            //将地图控件赋给变量，这样就可以使用接口所暴露的属性和方法了
            //axMapControl1属于主框架的私有控件，外部不能访问，所以采用这种模式可以通过公共变量的形式操作
            m_mapControl = (IMapControl3)axMapControl1.Object;
            m_pTocControl = (ITOCControl2)axTOCControl1.Object;

            toolComboBox = this.toolStripComboBox2;
            //TOC控件绑定地图控件
            m_pTocControl.SetBuddyControl(m_mapControl);
            //pCurrentTOC = m_pTocControl;
            //构造地图右键菜单
            mapMenu = new ToolbarMenuClass();
            mapMenu.AddItem(new LayerVisibility(), 1, 0, false, esriCommandStyles.esriCommandStyleIconAndText);
            mapMenu.AddItem(new LayerVisibility(), 2, 1, false, esriCommandStyles.esriCommandStyleIconAndText);
            //构造图层右键菜单
            layerMenu = new ToolbarMenuClass();
            //添加“移除图层”菜单项
            layerMenu.AddItem(new RemoveLayer(), -1, 0, false, esriCommandStyles.esriCommandStyleTextOnly);
            //添加“放大到整个图层”菜单项
            layerMenu.AddItem(new ZoomToLayer(), -1, 1, true, esriCommandStyles.esriCommandStyleTextOnly);
            //右键菜单绑定
            mapMenu.SetHook(m_mapControl);
            layerMenu.SetHook(m_mapControl);
           // IMap map = MapUtil.OpenMap(Common.MapPath);
            MapUtil.LoadMxd(this.axMapControl1,Common.MapPath);
            ClearNoData();
        }
        //删除不存在的图层
        private void ClearNoData()
        {
            IMap map = this.axMapControl1.Map;
            List<int> removeIndexs = new List<int>();
            for (int i = 0; i < map.LayerCount; i++)
            {
                if (map.get_Layer(i) == null)
                {
                    removeIndexs.Add(i);
                }
            }
            //倒着删除 切记
            for (int i = removeIndexs.Count - 1; i > -1; i--)
            {
                int index = removeIndexs[i];
                this.axMapControl1.DeleteLayer(index);
            }
            this.axMapControl1.Refresh();
        }
        /// <summary>
        /// 在TocControl的鼠标事件中实现右键菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axTOCControl1_OnMouseDown(object sender, ITOCControlEvents_OnMouseDownEvent e)
        {
            esriTOCControlItem item = esriTOCControlItem.esriTOCControlItemNone;
            IBasicMap map = null;
            ILayer layer = null;
            object other = null;
            object index = null;
            m_pTocControl.HitTest(e.x, e.y, ref item, ref map, ref layer, ref other, ref index);
            //右键命令
            if (e.button == 2)
            {
                m_mapControl.CustomProperty = layer;
                if (item == esriTOCControlItem.esriTOCControlItemMap)//点击的是地图
                {
                    m_pTocControl.SelectItem(map, null);
                    mapMenu.PopupMenu(e.x, e.y, m_pTocControl.hWnd);
                }

                if (item == esriTOCControlItem.esriTOCControlItemLayer)//点击的是图层
                {
                    m_pTocControl.SelectItem(layer, null);
                    //setSecAndEdit(layer.Name);
                    layerMenu.PopupMenu(e.x, e.y, m_pTocControl.hWnd);
                }
            }
            /*
            //左键移动
            if(e.button==1)
            {
                if (item == esriTOCControlItem.esriTOCControlItemLayer)
                {
                    //如果是注记图层则返回
                    if (layer is IAnnotationSublayer)
                    {
                        return;
                    }
                    //如何是组合图层的子图层
                    if (index == null)
                    {
                        int removedIndex = -1;
                        removedGroupLayer = QueryGroupLayer(layer, ref removedIndex);
                    }
                    removedLayer = layer;
                }
            }
             * */
        }
        private IGroupLayer QueryGroupLayer(ILayer layer,ref int index)
        {
            IGroupLayer gLayer = null;
            int layerIndex;
            int groupIndex;
            if (LayerUtil.QueryLayerInMap(this.axMapControl1, layer, ref gLayer, out layerIndex, out groupIndex))
            {
                index = layerIndex;
            }
            return gLayer;
        }
        private void axTOCControl1_OnMouseUp(object sender, ITOCControlEvents_OnMouseUpEvent e)
        {
            /*
            if (e.button == 1)
            {
            //   int toIndex = -1;
                esriTOCControlItem pItem = esriTOCControlItem.esriTOCControlItemNone;
                IBasicMap pBasMap = null;
                ILayer pLayer = null;
                object pOther = null;
                object pIndex = null;
                this.axTOCControl1.HitTest(e.x, e.y, ref pItem, ref pBasMap, ref pLayer, ref pOther, ref pIndex);
                if (pItem == esriTOCControlItem.esriTOCControlItemLayer)
                {
                    if (removedLayer != pLayer)//如果是原图层则不用操作
                    {
                        //IMap pMap = axMapControl1.Map;
                        //ILayer pTempLayer;
                        //for (int i = 0; i < pMap.LayerCount; i++)
                        //{
                        //    pTempLayer = pMap.get_Layer(i);
                        //    if (pTempLayer == pLayer)//获取移动后的图层索引
                        //        toIndex = i;
                        //}
                        //pMap.MoveLayer(removedLayer, toIndex);
                        IGroupLayer gLayer = null;
                        int toIndex2 = -1;
                        //如何是组合图层的子图层
                        if (pIndex == null)
                        {
                            gLayer = QueryGroupLayer(pLayer, ref toIndex2);
                        }
                        object toObj = null;
                        if (toIndex2 != -1)
                        {
                            toObj = (object)toIndex2;
                        }
                        TOCControlUtil.RemoveLayer(this.axMapControl1, removedLayer, removedGroupLayer, gLayer, toObj);
                        axMapControl1.ActiveView.Refresh();
                        this.axTOCControl1.Update();
                    }
                }
            }
             * */
        }

        //放大
        private void ToolButtonZoomIn_Click(object sender, EventArgs e)
        {
            ICommand zoomIn;
            zoomIn = new ControlsMapZoomInTool();
            zoomIn.OnCreate(m_mapControl);
            m_mapControl.CurrentTool = zoomIn as ITool;
            m_cTool = CustomTool.ZoomIn;
        }
        //缩小
        private void ToolButtonZoomOut_Click(object sender, EventArgs e)
        {
            ICommand zoomOut;
            zoomOut = new ControlsMapZoomOutTool();
            zoomOut.OnCreate(m_mapControl);
            m_cTool = CustomTool.ZoomOut;
            m_mapControl.CurrentTool = zoomOut as ITool;
        }
        //平移
        private void ToolButtonPan_Click(object sender, EventArgs e)
        {
            ICommand pan;
            pan = new ControlsMapPanTool();
            pan.OnCreate(m_mapControl);
            m_cTool = CustomTool.Pan;
            m_mapControl.CurrentTool = pan as ITool;
        }
        /// <summary>
        /// 全景 获取图层中最大的包围壳进行显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void ToolButtonFull_Click(object sender, EventArgs e)
        {
            MapAreaUtil.ZoomToByMaxLayer(this.axMapControl1);
        }

        private void axMapControl1_OnKeyUp(object sender, IMapControlEvents2_OnKeyUpEvent e)
        {
            //鼠标未落下,退出
            if (m_isMouseDown == false) return;

            IActiveView pActiveView = m_mapControl.ActiveView;
            IEnvelope pEnvelope;

            switch (m_cTool)
            {
                case CustomTool.ZoomIn:

                    ////If an envelope has not been tracked

                    if (m_feedBack == null)
                    {
                        //Zoom in from mouse click
                        pEnvelope = pActiveView.Extent;
                        pEnvelope.CenterAt(m_mouseDownPoint);
                        pEnvelope.Expand(0.5, 0.5, true);


                    }
                    else
                    {
                        //Stop the envelope feedback
                        pEnvelope = m_feedBack.Stop();

                        //Exit if the envelope height or width is 0
                        if (pEnvelope.Width == 0 || pEnvelope.Height == 0)
                        {
                            m_feedBack = null;
                            m_isMouseDown = false;
                        }
                    }
                    //Set the new extent
                    pActiveView.Extent = pEnvelope;
                    break;

                case CustomTool.ZoomOut:
                    IEnvelope pFeedEnvelope;

                    double NewWidth, NewHeight;

                    //If an envelope has not been tracked
                    if (m_feedBack == null)
                    {
                        //Zoom out from the mouse click
                        pEnvelope = pActiveView.Extent;
                        pEnvelope.Expand(2, 2, true);
                        pEnvelope.CenterAt(m_mouseDownPoint);
                    }
                    else
                    {
                        //Stop the envelope feedback
                        pFeedEnvelope = m_feedBack.Stop();

                        //Exit if the envelope height or width is 0
                        if (pFeedEnvelope.Width == 0 || pFeedEnvelope.Height == 0)
                        {
                            m_feedBack = null;
                            m_isMouseDown = false;

                        }

                        NewWidth = pActiveView.Extent.Width * (pActiveView.Extent.Width / pFeedEnvelope.Width);
                        NewHeight = pActiveView.Extent.Height * (pActiveView.Extent.Height / pFeedEnvelope.Height);

                        //Set the new extent coordinates
                        pEnvelope = new Envelope() as IEnvelope;
                        pEnvelope.PutCoords(
                            pActiveView.Extent.XMin -
                            ((pFeedEnvelope.XMin - pActiveView.Extent.XMin) *
                             (pActiveView.Extent.Width / pFeedEnvelope.Width)),
                            pActiveView.Extent.YMin -
                            ((pFeedEnvelope.YMin - pActiveView.Extent.YMin) *
                             (pActiveView.Extent.Height / pFeedEnvelope.Height)),
                            (pActiveView.Extent.XMin -
                             ((pFeedEnvelope.XMin - pActiveView.Extent.XMin) *
                              (pActiveView.Extent.Width / pFeedEnvelope.Width))) + NewWidth,
                            (pActiveView.Extent.YMin -
                             ((pFeedEnvelope.YMin - pActiveView.Extent.YMin) *
                              (pActiveView.Extent.Height / pFeedEnvelope.Height))) + NewHeight);

                    }
                    //Set the new extent
                    pActiveView.Extent = pEnvelope;
                    break;

                case CustomTool.Pan:
                    pEnvelope = m_focusScreenDisplay.PanStop();

                    //Check if user dragged a rectangle or just clicked.
                    //If a rectangle was dragged, m_ipFeedback in non-NULL
                    if (pEnvelope != null)
                    {
                        m_focusScreenDisplay.DisplayTransformation.VisibleBounds = pEnvelope;
                        m_focusScreenDisplay.Invalidate(null, true, (short)esriScreenCache.esriAllScreenCaches);

                    }
                    m_cTool = CustomTool.None;
                    break;
            }
        }
      
        private void axMapControl1_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            this.axMapControl1.Focus();
            this.axMapControl1.Map.ClearSelection();
            switch (m_cTool)
            {
                case CustomTool.ZoomIn:
                case CustomTool.ZoomOut:
                    //case CustomTool.Pan:
                    //Create a point in map coordinates
                    IPoint pPoint = new ESRI.ArcGIS.Geometry.Point();
                    m_mouseDownPoint = new ESRI.ArcGIS.Geometry.Point();
                    pPoint.X = e.mapX;
                    pPoint.Y = e.mapY;
                    m_mouseDownPoint = pPoint;
                    m_isMouseDown = true;
                    break;
                case CustomTool.RectSelect:
                 //  toolStripComboBox2.ComboBox.
                    if (this.toolStripComboBox2.SelectedIndex < 0)
                    {
                        MessageBox.Show("尚未选择任何查询图层");
                        return;
                    }
                    IEnvelope pRect = m_mapControl.TrackRectangle();
                    spatiallUI.DealFeatureQuery(this.axMapControl1,pRect as IGeometry,null,this.toolStripComboBox2.Text);
                    this.axMapControl1.Refresh(esriViewDrawPhase.esriViewGeography, null, null);
                    break;

                case CustomTool.Pan:
                    pPoint = new ESRI.ArcGIS.Geometry.Point();
                    pPoint.X = e.mapX;
                    pPoint.Y = e.mapY;
                    m_mouseDownPoint = pPoint;
                    m_isMouseDown = true;
                    m_focusScreenDisplay = m_mapControl.ActiveView.ScreenDisplay;
                    m_focusScreenDisplay.PanStart(m_mouseDownPoint);
                    break;
            }
            if (this.insertFlag!=0&&this.axMapControl1.CurrentTool==null)
            {
                InsertStopOrBarryPoint(e);
            }
        }

        //操作图层选择
        private void LayerSelect_Click(object sender, EventArgs e)
        {
            if( m_cTool == CustomTool.RectSelect)
            {
                this.toolStripButton9.Text = "矩形框查询";
                m_cTool = CustomTool.None;
                this.axMapControl1.Map.ClearSelection();
                this.axMapControl1.Refresh(esriViewDrawPhase.esriViewGeography, null, null);
                return;
            }
            //if (string.IsNullOrEmpty(toolComboBox.Text))
            //{
            //    MessageBox.Show("请先选择操作图层");
            //    return;
            //}
            //改变鼠标形状
            m_mapControl.MousePointer = esriControlsMousePointer.esriPointerArrow;
            this.toolStripButton9.Text = "停止查询";
            // //将mapcontrol的tool设为nothing，不然会影响效果
            m_mapControl.CurrentTool = null;
            m_cTool = CustomTool.RectSelect;
        }
        private void LayerMange_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MainFrom.WhichChecked = 1;
            MainFrom.groupLayer = new GroupLayerClass();
            MainFrom.groupLayer.Name = "基础数据";
            LayerMangerView lm = new LayerMangerView();
            lm.Show();
        }

        private void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            new ForecastDisplay().Show();
        }

        private void barButtonItem11_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //添加雨量字段并赋值 测试
            ConditionForm condtion = new ConditionForm();
            condtion.Show();
            //addRains();  
            //roadRaskCaculate.RoadRaskCaulte(@"w001001.adf", 20, @"..\..\Rources\RoadData\RoadRasterData");
        }

        private void barButtonItem14_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            new ConfigForm().ShowDialog();
        }
     
        //指针按钮事件  去除其它操作鼠标命令
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            this.axMapControl1.CurrentTool = null;
        }

        private void MainFrom_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.axMapControl1.Map.ClearSelection();
            routeUI.ClearRouteAnalyst(this.axMapControl1,ref this.insertFlag);
           // SymbolUtil.ClearElement(this.axMapControl1);
         //   MapUtil.SaveMap(Common.MapPath, this.axMapControl1.Map);
            MapUtil.SaveMxd(this.axMapControl1);
            Application.Exit();
        }

        private void barButtonItem9_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MainFrom.WhichChecked = 2;
            MainFrom.groupLayer = new GroupLayerClass();
            MainFrom.groupLayer.Name = "风险因素数据";
            LayerMangerView lm = new LayerMangerView();
            lm.Show();
        }

        private void barButtonItem10_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MainFrom.WhichChecked = 3;
            MainFrom.groupLayer = new GroupLayerClass();
            MainFrom.groupLayer.Name = "风险综合数据";
            LayerMangerView lm = new LayerMangerView();
            lm.Show();
        }

        private void barButtonItem17_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            new ConfigForm().ShowDialog();
        }
        /*
        /// <summary>
        /// 地图视图刷新事件
        /// 往图层下拉框中添加数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axMapControl1_OnViewRefreshed(object sender, IMapControlEvents2_OnViewRefreshedEvent e)
        {
            UpdateLayerCombox();
        }
        */
        private void toolStripComboBox2_Enter(object sender, EventArgs e)
        {
            toolStripComboBox2.SelectedText = "";
            UpdateLayerCombox();
        }
        private void UpdateLayerCombox()
        {
            toolStripComboBox2.Items.Clear();
            List<ILayer> layers = MapUtil.GetAllLayers(this.axMapControl1);
            layers.ForEach(p =>
            {
                if (p is FeatureLayer)
                {
                    toolStripComboBox2.Items.Add(p.Name);
                }
            });
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            new PropertyQueryForm(this.toolStripComboBox2.Text, this.axMapControl1).ShowDialog();
        }
        #region 最短路径计算
        //计算最优路线
        //最后可以考虑使用async进行异步查询
        private void barButtonItem16_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (routeUI.StopPoints.Count < 2)
            {
                MessageBox.Show("路线经过点少于一个");
                return;
            }
            this.insertFlag = 0;
            this.routeNetLayer = routeUI.DealRoutenetLayer(this.axMapControl1);
            if (routeNetLayer == null)
            {
                MessageBox.Show("未找到公路网图层");
                return;
            }
            infoWindow = new ProInfoWindow();
            infoWindow.Show();
            timer1.Enabled = true;
        }
        private void barButtonItem23_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            routeUI.ClearRouteAnalyst(this.axMapControl1, ref this.insertFlag);
        }
        //插入公路点
        private void InsertStopOrBarryPoint(IMapControlEvents2_OnMouseDownEvent e)
        {
            IPoint point = new PointClass();
            point.X = e.mapX;
            point.Y = e.mapY;
            routeUI.InsertPoint(this.insertFlag, this.axMapControl1, point);
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            this.timer1.Enabled = false;
            try
            {
              bool result=  FindRouteMethod();
              if (!result)
              {
                  MessageBox.Show("查询失败");
              }
            }
            catch (PointIsFarException e1)
            {
                MessageBox.Show(e1.Message);
            }
            catch (NetworkDbException e2)
            {
                MessageBox.Show(e2.Message);
            }
            infoWindow.Close();
        }
        //显示路线查找结果
        public bool FindRouteMethod()
        {
            #region 同步方法
            ILayer routeLayer = null;
            Debug.Print("当前运行线程：" + Thread.CurrentThread.ManagedThreadId);
            bool result = routeUI.FindTheShortRoute(this.axMapControl1,routeNetLayer as IFeatureLayer, ref routeLayer);
            Debug.Print("当前运行线程：" + Thread.CurrentThread.ManagedThreadId);
            //更新点位标志
            routeUI.UpdateSymbol(this.axMapControl1);
            if (!result)
            {
                return result;
            }
            //显示路线
            routeUI.showRouteShape(routeLayer as IFeatureLayer, this.axMapControl1);
            IActiveView pActiveView = this.axMapControl1.Map as IActiveView;
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
            return true;
            #endregion
        }

        #endregion
        #region 公路经过点相关命令事件
        //激活或者关闭公路经过点插入工具
        private void barButtonItem26_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (this.axMapControl1.CurrentTool == stopsInsertTool)
            {
                this.axMapControl1.CurrentTool = null;
                return;
            }
            stopsInsertTool.OnCreate(this.axMapControl1);//绑定到mapcontrol
            stopsInsertTool.OnClick();//执行itool的click事件
            this.axMapControl1.CurrentTool = stopsInsertTool;//设置当前工具
        }
        //撤销上一个公路经过点
        private void barButtonItem25_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            routeUI.UndoStopPointSymbols(this.axMapControl1);
        }
        //清空公路经过点
        private void barButtonItem29_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            routeUI.ResetStopPointSymbols(this.axMapControl1);
        }

        #endregion
        #region 公路断点相关命令事件
        //激活或者关闭公路断点插入工具
        private void barButtonItem27_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (this.axMapControl1.CurrentTool == barrysInsertTool)
            {
                this.axMapControl1.CurrentTool = null;
                return;
            }
            barrysInsertTool.OnCreate(this.axMapControl1);//绑定到mapcontrol
            barrysInsertTool.OnClick();//执行itool的click事件
            this.axMapControl1.CurrentTool = barrysInsertTool;//设置当前工具
        }
        //撤销上一个断点
        private void barButtonItem28_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            routeUI.UndoBarryPointSymbols(this.axMapControl1);
        }
        //清空断点
        private void barButtonItem30_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            routeUI.ResetBarryPointSymbols(this.axMapControl1);
        }
        #endregion

        private void barButtonItem15_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.axMapControl1.CurrentTool = stopRemTool;
        }
    }
}