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

namespace pixChange
{
    public partial class MainFrom : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private List<IPoint> barryPoints = new List<IPoint>();
        private List<IPoint> stopPoints = new List<IPoint>();
        //0为不插入 1为插入经过点 2为插入断点
        private int insertFlag = 0;
        //路线操作接口字段
        private IRouteDecide routeDecide = ServiceLocator.GetRouteDecide();
        //路线操作
        private IRouteUI routeUI = ServiceLocator.RouteUI;
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

        private IToolbarMenu mapMenu;//toc控件右键地图菜单
        private IToolbarMenu layerMenu;//toc控件右键图层菜单
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
        //初始化障碍点图层和经过点图层
        private void InitialAboutNetLayer()
        {

        }
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
            if (e.button == 2)//右键
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
        private void axTOCControl1_OnMouseMove(object sender, ITOCControlEvents_OnMouseMoveEvent e)
        {
            //鼠标未落下,退出
            IPoint pt = new ESRI.ArcGIS.Geometry.Point();
            pt.PutCoords(e.x, e.y);

            switch (m_cTool)
            {
                case CustomTool.ZoomIn:
                case CustomTool.ZoomOut:
                    //'Get 


                    break;
                case CustomTool.Pan:
                    m_focusScreenDisplay = m_mapControl.ActiveView.ScreenDisplay;
                    m_focusScreenDisplay.PanMoveTo(pt);
                    break;



            }
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
            IFeatureLayer pFeatureLayer;
            IFeatureClass pFeatureClass;
            //新建一个空间过滤器
            ISpatialFilter pSpatialFilter;
            IQueryFilter pFilter;

            IFeatureCursor pCursor;

            //定义各种空间类型数据的符号
            ISimpleMarkerSymbol simplePointSymbol;
            ISimpleFillSymbol simpleFillSymbol;
            ISimpleLineSymbol simpleLineSymbol;
            //用于闪烁的符号
            ISymbol symbol;

            IFeature pFeature;
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
                    //    pFeatureLayer = (IFeatureLayer)m_mapControl.get_Layer(toolComboBox.SelectedIndex);
                    pFeatureLayer = (IFeatureLayer)LayerManager.RetuenLayerByLayerNameLayer(m_mapControl, toolComboBox.SelectedItem.ToString());
                    pFeatureClass = pFeatureLayer.FeatureClass;
                    IEnvelope pRect = new Envelope() as IEnvelope;
                    pRect = m_mapControl.TrackRectangle();

                    //新建一个空间过滤器
                    pSpatialFilter = new SpatialFilter();
                    pSpatialFilter.Geometry = pRect;
                    //依据被选择的要素类的类型不同，设置不同的空间过滤关系
                    switch (pFeatureClass.ShapeType)
                    {
                        case esriGeometryType.esriGeometryPoint:
                        case esriGeometryType.esriGeometryMultipoint:
                            pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;
                            break;
                        case esriGeometryType.esriGeometryPolyline:
                        case esriGeometryType.esriGeometryLine:
                            pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelCrosses;
                            break;
                        case esriGeometryType.esriGeometryPolygon:
                        case esriGeometryType.esriGeometryEnvelope:
                            pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                            break;
                    }


                    pSpatialFilter.GeometryField = pFeatureClass.ShapeFieldName;
                    pFilter = pSpatialFilter;
                    //通过空间关系查询
                    pCursor = pFeatureLayer.Search(pFilter, false);

                    //定义各种空间类型数据的符号
                    simplePointSymbol = new SimpleMarkerSymbol();
                    simpleFillSymbol = new SimpleFillSymbol();
                    simpleLineSymbol = new SimpleLineSymbol();
                    simplePointSymbol.Style = esriSimpleMarkerStyle.esriSMSCircle;
                    simplePointSymbol.Size = 5;
                    simplePointSymbol.Color = ColorHelper.GetRGBColor(255, 0, 0);

                    simpleLineSymbol.Width = 2;
                    simpleLineSymbol.Color = ColorHelper.GetRGBColor(255, 0, 99);
                    simpleFillSymbol.Outline = simpleLineSymbol;
                    simpleFillSymbol.Color = ColorHelper.GetRGBColor(222, 222, 222);
                    //用于闪烁的符号
                    pFeature = pCursor.NextFeature();
                    DataTable pDataTable = createDataTableByLayer(pFeatureLayer as ILayer);
                    while (pFeature != null)
                    {
                        axMapControl1.Map.SelectFeature(pFeatureLayer, pFeature);  //高亮显示
                        IGeometry pShape;
                        pShape = pFeature.Shape;
                        ITable pTable = pFeature as ITable;
                        DataRow pDataRow = pDataTable.NewRow();
                        for (int i = 0; i < pFeature.Fields.FieldCount; i++)
                        {

                            if (pFeature.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeGeometry)
                            {
                                pDataRow[i] = getShapeType(pFeatureLayer as ILayer);
                            }
                            else if (pFeature.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeBlob)
                            {
                                pDataRow[i] = "Element";
                            }
                            else
                            {
                                pDataRow[i] = pFeature.get_Value(i);
                            }

                        }
                        pDataTable.Rows.Add(pDataRow);
                        switch (pFeatureClass.ShapeType)
                        {
                            case esriGeometryType.esriGeometryPoint:
                            case esriGeometryType.esriGeometryMultipoint:
                                symbol = (ISymbol)simplePointSymbol;
                                m_mapControl.FlashShape(pShape, 5, 100, symbol);
                                break;
                            case esriGeometryType.esriGeometryPolyline:
                            case esriGeometryType.esriGeometryLine:
                                symbol = (ISymbol)simpleLineSymbol;
                                m_mapControl.FlashShape(pShape, 5, 100, symbol);
                                break;
                            case esriGeometryType.esriGeometryPolygon:
                            case esriGeometryType.esriGeometryEnvelope:
                                symbol = (ISymbol)simpleFillSymbol;
                                m_mapControl.FlashShape(pShape, 5, 100, symbol);
                                break;
                        }

                        pFeature = pCursor.NextFeature();
                    }
                    m_cTool = CustomTool.None;

                    ProListView result = new ProListView();
                    result.showTable(pDataTable);

                    result.getLayerName(pFeatureLayer.Name);
                    result.Show();
                    //可以让选中的区域立即显示出来
                    MainFrom.m_mapControl.Refresh(esriViewDrawPhase.esriViewGeography, null, null);
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
                InsertPoint(e);
            }
        }

        #region get dataTable by Ilayer
        private DataTable createDataTableByLayer(ILayer player)
        {
            DataTable dataTable = new DataTable();
            ITable ptable = player as ITable;
            IField pField;
            DataColumn pDataColumn;
            for (int i = 0; i < ptable.Fields.FieldCount; i++)
            {
                pField = ptable.Fields.get_Field(i);
                pDataColumn = new DataColumn(pField.Name);
                dataTable.Columns.Add(pDataColumn);
                pField = null;
                pDataColumn = null;
            }
            return dataTable;
        }
        private DataTable createDataTable()
        {
            DataTable dataTable = new DataTable();
            //ITable ptable = player as ITable;
            //IField pField;
            //DataRow pDataRow;
            DataColumn pColumn1 = new DataColumn("Feilds");
            DataColumn pColumn2 = new DataColumn("Value");
            dataTable.Columns.Add(pColumn1);
            dataTable.Columns.Add(pColumn2);
            /*
            for (int i = 0; i < ptable.Fields.FieldCount; i++)
               {
                    pField = ptable.Fields.get_Field(i);
                    pDataRow = new DataRow();
                }
            */
            return dataTable;
        }
        private string getShapeType(ILayer player)
        {
            IFeatureLayer pFlayer = (IFeatureLayer)player;
            switch (pFlayer.FeatureClass.ShapeType)
            {
                case esriGeometryType.esriGeometryPoint:
                    return "Point";
                case esriGeometryType.esriGeometryPolyline:
                    return "Polyline";
                case esriGeometryType.esriGeometryPolygon:
                    return "Polygon";
                default:
                    return "";
            }
        }
        #endregion

        //操作图层选择
        private void LayerSelect_Click(object sender, EventArgs e)
        {
            if (toolComboBox.SelectedIndex < 0)
            {
                MessageBox.Show("请先选择操作图层");
            }
            //改变鼠标形状
            m_mapControl.MousePointer = esriControlsMousePointer.esriPointerArrow;
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


        private static void IsCheck(ILayer layer)//判断IGroupLayer中所有图层的visible状态
        {
            for (int count = 0; count < MainFrom.m_mapControl.LayerCount; count++)
            {
                bool IsEqual = false;
                bool IsEmpty = false;
                ILayer pGL = MainFrom.m_mapControl.get_Layer(count);
                if (pGL is IGroupLayer)
                {
                    ICompositeLayer pGroupLayer = pGL as ICompositeLayer;
                    for (int j = 0; j < pGroupLayer.Count; j++)
                    {
                        ILayer pCompositeLayer;
                        pCompositeLayer = pGroupLayer.get_Layer(j);
                        if (layer == pCompositeLayer)
                        {
                            IsEqual = true;
                        }
                        if (pCompositeLayer.Visible == true)
                        {
                            IsEmpty = true;
                        }
                    }
                }
                if (IsEqual)
                {
                    if (IsEmpty)
                    {
                        pGL.Visible = true;
                    }
                    else
                    {
                        pGL.Visible = false;
                    }
                }
            }
        }

        private void barButtonItem14_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            new ConfigForm().ShowDialog();
        }
        //开启编辑公路断点模式
        private void barButtonItem15_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            #region 注释
            ////如果处于正在编辑状态 则清除断路点和标识
            //if (isInserting)
            //{
            //    SymbolUtil.ClearElement(this.axMapControl1);
            //    this.breakPoint = null;
            //    this.pixtureElement = null;
            //    isInserting = false;
            //    this.barButtonItem15.Caption = "设置断点";
            //}
            //else
            //{
            //    isInserting = true;
            //  //  this.barButtonItem15.Caption = "取消断点";
            //}
            //if(this.pixtureElement!=null)
            //{
            //    SymbolUtil.ClearElement(this.axMapControl1,this.pixtureElement as IElement);
            //}
            //MainFrom.m_mapControl.Refresh();
            #endregion
            if (this.insertFlag == 2)
            {
                this.insertFlag = 0;
                return;
            }
            this.insertFlag = 2;
            routeUI.DealRoutenetLayer(this.axMapControl1);
        }
        private void barButtonItem22_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (this.insertFlag == 1)
            {
                this.insertFlag = 0;
                return;
            }
            this.insertFlag = 1;
            //处理公路网图层
            routeUI.DealRoutenetLayer(this.axMapControl1);
        }
        //插入公路断点
        private void InsertPoint(IMapControlEvents2_OnMouseDownEvent e)
        {
            /*
            if(this.pixtureElement!=null)
            {
                SymbolUtil.ClearElement(this.axMapControl1, this.pixtureElement as IElement);
            }
            IPoint point = new PointClass();
            point.X = e.mapX;
            point.Y = e.mapY;
            this.pixtureElement = SymbolUtil.DrawSymbolWithPicture(point, this.axMapControl1, Common.RouteBeakImggePath);
            this.breakPoint = point;
            this.barButtonItem15.Caption = "取消断点";
              */
            IPoint point = new PointClass();
            point.X = e.mapX;
            point.Y = e.mapY;
            if (this.insertFlag == 1)
            {
               SymbolUtil.DrawSymbolWithPicture(point, this.axMapControl1, Common.StopImagePath);
               this.stopPoints.Add(point);
            }
            else if(this.insertFlag==2)
            {
                SymbolUtil.DrawSymbolWithPicture(point, this.axMapControl1, Common.RouteBeakImggePath);
                this.barryPoints.Add(point);
            }
        }
 
        //计算最优路线
        //最后可以考虑使用async进行异步查询
        private void barButtonItem16_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            #region 注释
            /*
            this.isInserting = false;
            if (breakPoint == null)
            {
                MessageBox.Show("尚未设置公路断点");
                return;
            }
            this.barButtonItem16.Caption = "正在查询";
            //图标修正点
            IPoint rightPoint = null;
            bool result= routeDecide.QueryTheRoue(breakPoint, this.axMapControl1, routeNetLayer as IFeatureLayer, Common.NetWorkPath, "roads", "roads_ND", ref rightPoint);
            if(result)
            {
                MessageBox.Show("查询成功");
                //修正短路点的坐标
                SymbolUtil.ClearElement(this.axMapControl1, this.pixtureElement as IElement);
                // this.breakPoint = rightPoint;
                // this.pixtureElement = SymbolUtil.DrawSymbolWithPicture(breakPoint, this.axMapControl1, Common.RouteBeakImggePath);
            }
            else
            {
                MessageBox.Show("查询失败");
            }
            this.barButtonItem16.Caption = "绕行方案";
          */
            #endregion
            if (this.stopPoints.Count < 2)
            {
                MessageBox.Show("流程经过点少于一个");
                return;
            }
            this.insertFlag = 0;
            ILayer layer = routeUI.DealRoutenetLayer(this.axMapControl1);
            if (layer == null)
            {
                return;
            }
            try
            {
                bool result = routeUI.FindTheShortRoute(this.axMapControl1, stopPoints, barryPoints, layer as IFeatureLayer);
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
        }
     
        //指针按钮事件  去除其它操作鼠标命令
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            this.axMapControl1.CurrentTool = null;
        }

        private void MainFrom_FormClosing(object sender, FormClosingEventArgs e)
        {
            routeUI.ClearRouteAnalyst(this.axMapControl1,ref this.insertFlag,stopPoints,barryPoints);
           // SymbolUtil.ClearElement(this.axMapControl1);
         //   MapUtil.SaveMap(Common.MapPath, this.axMapControl1.Map);
            MapUtil.SaveMxd(this.axMapControl1);
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
        //显示绕行路线 现在不采用该方案
        private void ShowRoute(string routeName)
        {
            ILayer rightLayer = ShapeSimpleHelper.OpenFile(Common.BetterRoutesPath, routeName);
            FeatureStyleUtil.SetFetureLineStyle(255, 255, 0, 3, rightLayer as IFeatureLayer);
            this.axMapControl1.AddLayer(rightLayer);
        }

       

        private void barButtonItem23_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            routeUI.ClearRouteAnalyst(this.axMapControl1, ref this.insertFlag, stopPoints, barryPoints);
        }
    }
}
