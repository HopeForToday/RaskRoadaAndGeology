using DevExpress.Utils;
using DevExpress.XtraCharts;
using RoadRaskEvaltionSystem.HelperClass;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WinInrush.Class;

namespace RoadRaskEvaltionSystem
{
    public partial class ForecastDisplay : Form
    {
        string strFilePath = "Provider=Microsoft.Jet.OLEDB.4.0;Data source=" + Application.StartupPath + "\\Rources\\雨量信息.mdb";
        string ChartSqlStr, GridSqlStr, WeekSqlStr;
        Series newRains, newTemperature, newWindspeed;//未来天气预报Series
        Series oldRain1h, oldTemperature, oldHumidity, oldWindspeed;//过去24小时天气数据Series
        Series series;//全局Series变量
        SecondaryAxisY myAxis;//第二坐标轴
        List<SecondaryAxisY> oldAxisYList, newAxisYList;
        List<Series> oldlist, newlist;
        DataTable dt_Chart, dt_Grid, dt_Week;
        DateTime NowTime;//当前时间
        String day1, day2;
        int AreaId = 1; //1为芦山县，2为宝兴县
        public ForecastDisplay()
        {
            InitializeComponent();
            int AreaId = 1;
        }

        private void ForecastDisplay_Load(object sender, EventArgs e)
        {
            //为Combox控件添加数据
            comboBoxEdit_Area.Properties.Items.Add("芦山县");
            comboBoxEdit_Area.Properties.Items.Add("宝兴县");
            comboBoxEdit_Area.SelectedItem = "芦山县";
            NowTime = DateTime.Now;
            for (int i = 1; i < 6; i++)
            {
                comboBoxEdit_Date.Properties.Items.Add(NowTime.AddDays(i).ToString("yyyy-MM-dd"));
            }
            comboBoxEdit_Date.SelectedItem = NowTime.AddDays(1).ToString("yyyy-MM-dd");

            oldAxisYList = new List<SecondaryAxisY>();
            newAxisYList = new List<SecondaryAxisY>();
            //day1 = (Convert.ToDateTime(comboBoxEdit_Date.Text)).AddDays(1).ToString("yyyy-MM-dd");
            day1 = NowTime.ToString("yyyy-MM-dd");
            day2 = NowTime.AddDays(6).ToString("yyyy-MM-dd");
            //数据库连接字符串
            ChartSqlStr = String.Format("SELECT ID,dtime3hour,temperature,rains,wind FROM ForecastWeather WHERE AreaID = {0} AND timedate7 = #{1}#", AreaId, comboBoxEdit_Date.Text);
            GridSqlStr = String.Format("SELECT ID,dtime3hour,temperature,rains,wind,windd,yl,xdsd FROM ForecastWeather WHERE AreaID = {0} AND timedate7 BETWEEN #{1}# AND #{2}# ORDER BY ID ASC", AreaId, day1, day2);
            WeekSqlStr = String.Format("SELECT ID,Hourtime,rain1h,temperature,humidity,windSpeed FROM OneHourWeather WHERE Hourtime BETWEEN #{1}# AND #{2}# AND AreaID = {0} ORDER BY Hourtime ASC", AreaId, NowTime.ToString("yyyy-MM-dd HH:mm"), NowTime.AddDays(-1).ToString("yyyy-MM-dd HH:mm"));
            //获取数据并创建图表 
            dt_Chart = getDataTable(ChartSqlStr);
            CreateNewChart(dt_Chart);
            dt_Grid = getDataTable(GridSqlStr);
            CreateTable(dt_Grid);
            dt_Week = getDataTable(WeekSqlStr);
            CreateOldChart(dt_Week);
        }

        /// <summary>
        /// 表格数据建立
        /// </summary>
        /// <param name="dt">数据源</param>
        private void CreateTable(DataTable dt)
        {
            //让各列头禁止移动
            gridView1.OptionsCustomization.AllowColumnMoving = false;
            //让各列头禁止排序
            gridView1.OptionsCustomization.AllowSort = false;
            //去掉上面的筛选条
            gridView1.OptionsView.ShowGroupPanel = false;
            //禁止各列头改变列宽
            gridView1.OptionsCustomization.AllowColumnResizing = false;
            this.gridView1.IndicatorWidth = 100;
            //不显示内置的导航条。
            gridControl1.UseEmbeddedNavigator = false;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (String.IsNullOrEmpty(dt.Rows[i]["dtime3hour"].ToString()))
                {
                    dt.Rows.RemoveAt(i);
                    i--;
                }
            }

            gridControl1.DataSource = dt;
        }

        /// <summary>
        /// 未来天气预报
        /// </summary>
        /// <param name="dt">数据源</param>
        private void CreateNewChart(DataTable dt)
        {
            chartControl1.DataSource = dt;
            chartControl2.Series.Clear();
            #region Series
            //创建几个图形的对象
            newRains = CreateSeries("降雨量(mm)", ViewType.Bar, dt, "dtime3hour", "rains");
            newTemperature = CreateSeries("温度(℃)", ViewType.Line, dt, "dtime3hour", "temperature");
            newWindspeed = CreateSeries("风速(m/s)", ViewType.Line, dt, "dtime3hour", "wind");
            #endregion

            newRains.View.Color = Color.GreenYellow;
            newRains.ArgumentScaleType = ScaleType.Qualitative;

            chartControl1.Series.Add(newRains);
            ((BarSeriesView)newRains.View).BarWidth = 0.2;
            XYDiagram diagram = (XYDiagram)chartControl1.Diagram;
            diagram.AxisY.Title.Visible = true;
            diagram.AxisY.Title.Alignment = StringAlignment.Far;
            diagram.AxisY.Title.Text = newRains.Name;
            diagram.AxisY.Title.Antialiasing = true;
            diagram.AxisY.Title.Font = new Font("宋体", 9.0f);
            diagram.AxisY.Title.TextColor = Color.Green;
            diagram.AxisY.Label.Visible = true;
            diagram.AxisY.Label.TextColor = Color.Green;
            diagram.AxisY.Color = Color.Green;
            ((XYDiagram)chartControl1.Diagram).EnableAxisXZooming = true;

            newlist = new List<Series>() { newTemperature, newWindspeed };
            chartControl1.Series.AddRange(newlist.ToArray());
            chartControl1.Legend.Visibility = DefaultBoolean.False;

            //十字线参数设置
            chartControl1.CrosshairOptions.ShowArgumentLabels = false;//移动鼠标时动态显示Y轴数据标签
            chartControl1.CrosshairOptions.ShowArgumentLine = true;//显示Y轴线
            chartControl1.CrosshairOptions.ShowValueLabels = false;//移动鼠标时动态显示X轴数据标签
            chartControl1.CrosshairOptions.ShowValueLine = false;//显示X轴线

            List<Color> colorList = new List<Color> { Color.Blue, Color.Green, Color.Red };
            for (int i = 0; i < newlist.Count; i++)
            {
                newlist[i].View.Color = colorList[i];
                CreateAxisY(newlist[i], chartControl1, MarkerKind.Circle, true, i);
            }
        }

        /// <summary>
        /// 过去24小时天气展示
        /// </summary>
        /// <param name="dt">数据源</param>
        private void CreateOldChart(DataTable dt)
        {
            chartControl2.DataSource = dt;
            chartControl2.Series.Clear();
            #region Series
            //创建几个图形的对象
            oldRain1h = CreateSeries("降雨量(mm)", ViewType.Bar, dt, "Hourtime", "rain1h");
            oldTemperature = CreateSeries("温度(℃)", ViewType.Spline, dt, "Hourtime", "temperature");
            oldHumidity = CreateSeries("湿度(%)", ViewType.Spline, dt, "Hourtime", "humidity");
            oldWindspeed = CreateSeries("风速(m/s)", ViewType.Line, dt, "Hourtime", "windSpeed");
            #endregion

            oldRain1h.View.Color = Color.GreenYellow;
            oldRain1h.ArgumentScaleType = ScaleType.Qualitative;
            ((BarSeriesView)oldRain1h.View).BarWidth = 0.8;
            chartControl2.Series.Add(oldRain1h);
            XYDiagram diagram = (XYDiagram)chartControl2.Diagram;
            diagram.AxisY.Title.Visible = true;
            diagram.AxisY.Title.Alignment = StringAlignment.Far;
            diagram.AxisY.Title.Text = oldRain1h.Name;
            diagram.AxisY.Title.Antialiasing = true;
            diagram.AxisY.Title.Font = new Font("宋体", 9.0f);
            diagram.AxisY.Title.TextColor = Color.Green;
            diagram.AxisY.Label.Visible = true;
            diagram.AxisY.Label.TextColor = Color.Green;
            diagram.AxisY.Color = Color.Green;
            ((XYDiagram)chartControl2.Diagram).EnableAxisXZooming = true;

            oldlist = new List<Series>() { oldTemperature, oldHumidity, oldWindspeed };
            chartControl2.Series.AddRange(oldlist.ToArray());
            chartControl2.Legend.Visibility = DefaultBoolean.False;

            //十字线参数设置
            chartControl2.CrosshairOptions.ShowArgumentLabels = false;//移动鼠标时动态显示Y轴数据标签
            chartControl2.CrosshairOptions.ShowArgumentLine = true;//显示Y轴线
            chartControl2.CrosshairOptions.ShowValueLabels = false;//移动鼠标时动态显示X轴数据标签
            chartControl2.CrosshairOptions.ShowValueLine = false;//显示X轴线

            List<MarkerKind> markerList = new List<MarkerKind> { MarkerKind.Circle, MarkerKind.Diamond, MarkerKind.Triangle };
            List<bool> boolList = new List<bool> { true, true, true };
            List<Color> colorList = new List<Color> { Color.Blue, Color.Green, Color.Red };

            for (int i = 0; i < oldlist.Count; i++)
            {
                oldlist[i].View.Color = colorList[i];
                CreateAxisY(oldlist[i], chartControl2, markerList[i], boolList[i], i);
            }
        }

        /// <summary>
        /// 根据数据创建一个图形展现
        /// </summary>
        /// <param name="caption">图形标题</param>
        /// <param name="viewType">图形类型</param>
        /// <param name="dt">数据DataTable</param>
        /// <param name="xBindName">x轴绑定数据</param>
        /// <param name="yBindName">y轴绑定数据</param>
        /// <returns></returns>
        private Series CreateSeries(string caption, ViewType viewType, DataTable dt, string xBindName, string yBindName)
        {
            series = new Series(caption, viewType);
            series.ArgumentScaleType = ScaleType.Qualitative;
            series.LabelsVisibility = DefaultBoolean.True;//显示标注标签
            double value = 0.0;//参数值
            string argument = "";//参数名称
            int length = dt.Rows.Count;
            for (int i = 0; i < length; i++)
            {
                try
                {
                    switch (xBindName)
                    {
                        //OneHourWeather
                        case "Hourtime":
                            if (dt.Rows[i][xBindName].ToString().Contains("0:00:00") || dt.Rows[i][xBindName].ToString().Contains("00:00:00"))
                            {
                                argument = dt.Rows[i][xBindName].ToString().Split(' ')[0].ToString();
                            }
                            else
                            {
                                argument = String.Format("{0}点", dt.Rows[i][xBindName].ToString().Split(' ')[1].Split(':')[0].ToString());
                            }
                            break;
                        //ForecastWeather
                        case "dtime3hour":
                            argument = dt.Rows[i][xBindName].ToString();
                            break;
                        default:
                            break;
                    }

                    switch (yBindName)
                    {
                        //ForecastWeather
                        case "temperature":
                            if (dt.Rows[i][yBindName].ToString().Contains('℃'))
                            {
                                value = Convert.ToDouble(dt.Rows[i][yBindName].ToString().Split('℃')[0]);
                            }
                            else
                            {
                                if (dt.Rows[i][yBindName].ToString().Equals("9999.0"))
                                {
                                    dt.Rows.RemoveAt(i);
                                    length--;
                                }
                                else
                                {
                                    value = Convert.ToDouble(dt.Rows[i][yBindName].ToString());
                                }
                            }
                            break;
                        case "rains":
                            value = dt.Rows[i][yBindName].Equals("无降水") ? 0.0 : Convert.ToDouble(dt.Rows[i][yBindName].ToString().Split('毫')[0]);
                            break;
                        case "wind":
                            value = Convert.ToDouble(dt.Rows[i][yBindName].ToString().Split('米')[0]);
                            break;
                        //OneHourWeather
                        case "rain1h":
                            if (dt.Rows[i][yBindName].ToString().Equals("9999.0"))
                            {
                                value = Convert.ToDouble(dt.Rows[i - 1][yBindName].ToString());
                            }
                            else
                            {
                                value = Convert.ToDouble(dt.Rows[i][yBindName].ToString());
                            }
                            break;
                        case "humidity":
                            if (dt.Rows[i][yBindName].ToString().Equals("9999.0"))
                            {
                                value = Convert.ToDouble(dt.Rows[i - 1][yBindName].ToString());
                            }
                            else
                            {
                                value = Convert.ToDouble(dt.Rows[i][yBindName].ToString());
                            }
                            break;
                        case "windSpeed":
                            if (dt.Rows[i][yBindName].ToString().Equals("9999.0"))
                            {
                                value = Convert.ToDouble(dt.Rows[i - 1][yBindName].ToString());
                            }
                            else
                            {
                                value = Convert.ToDouble(dt.Rows[i][yBindName].ToString());
                            }
                            break;
                        default:
                            break;
                    }
                    series.Points.Add(new SeriesPoint(argument, value));
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.ToString());
                }

            }
            return series;
        }

        /// <summary>
        /// 创建图表的第二坐标系
        /// </summary>
        /// <param name="series">Series对象</param>
        /// <returns></returns>
        private SecondaryAxisY CreateAxisY(Series series, ChartControl chartcontrol, MarkerKind type, bool label, int i)
        {
            if (newAxisYList.Count < 3 && oldAxisYList.Count < 3)
            {
                myAxis = new SecondaryAxisY(series.Name);
                ((XYDiagram)chartcontrol.Diagram).SecondaryAxesY.Add(myAxis);
                ((XYDiagram)chartcontrol.Diagram).EnableAxisXZooming = true;
                ((LineSeriesView)series.View).AxisY = myAxis;
                //定义线条上点的标识形状是否需要
                ((LineSeriesView)series.View).LineMarkerOptions.Visible = true;
                //定义线条上点的标识形状
                ((LineSeriesView)series.View).LineMarkerOptions.Kind = type;
                //显示X、Y轴上面的交点的值
                ((PointSeriesLabel)series.Label).Visible = label;
                //线条的类型，虚线，实线
                ((LineSeriesView)series.View).LineStyle.DashStyle = DashStyle.Solid;

                myAxis.Title.Text = series.Name;
                myAxis.Title.Alignment = StringAlignment.Far; //顶部对齐
                myAxis.Title.Visible = true; //显示标题
                myAxis.Title.Font = new Font("宋体", 9.0f);

                Color color = series.View.Color;//设置坐标的颜色和图表线条颜色一致

                myAxis.Title.TextColor = color;
                myAxis.Label.TextColor = color;
                myAxis.Color = color;

                switch (chartcontrol.Name.ToString())
                {
                    case "chartControl1":
                        newAxisYList.Add(myAxis);
                        break;
                    case "chartControl2":
                        oldAxisYList.Add(myAxis);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (chartcontrol.Name.ToString())
                {
                    case "chartControl1":
                        ((LineSeriesView)series.View).AxisY = newAxisYList[i];
                        //定义线条上点的标识形状是否需要
                        ((LineSeriesView)series.View).LineMarkerOptions.Visible = true;
                        //定义线条上点的标识形状
                        ((LineSeriesView)series.View).LineMarkerOptions.Kind = MarkerKind.Circle;
                        //显示X、Y轴上面的交点的值
                        ((PointSeriesLabel)series.Label).Visible = true;
                        //线条的类型，虚线，实线
                        ((LineSeriesView)series.View).LineStyle.DashStyle = DashStyle.Solid;
                        break;
                    case "chartControl2":
                        ((LineSeriesView)series.View).AxisY = oldAxisYList[i];
                        //定义线条上点的标识形状是否需要
                        ((LineSeriesView)series.View).LineMarkerOptions.Visible = true;
                        //定义线条上点的标识形状
                        ((LineSeriesView)series.View).LineMarkerOptions.Kind = MarkerKind.Circle;
                        //显示X、Y轴上面的交点的值
                        ((PointSeriesLabel)series.Label).Visible = true;
                        //线条的类型，虚线，实线
                        ((LineSeriesView)series.View).LineStyle.DashStyle = DashStyle.Solid;
                        break;
                    default:
                        break;
                }
            }



            return myAxis;
        }

        /// <summary>
        /// 读取数据存为table
        /// </summary>
        /// <param name="sqlStr">SQL语句</param>
        /// <returns></returns>
        private DataTable getDataTable(string sqlStr)
        {
            OleDbConnection con = new OleDbConnection(strFilePath);
            con.Open();
            OleDbDataAdapter da = new OleDbDataAdapter(sqlStr, con);
            DataTable _dt = new DataTable();
            try
            {
                da.Fill(_dt);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                con.Close();
                con.Dispose();
                da.Dispose();
            }

            return _dt;
        }

        /// <summary>
        /// 为表格添加行标题
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            e.Appearance.Font = new Font("宋体", 10, FontStyle.Bold);
            if (e.Info.IsRowIndicator)
            {
                if (e.RowHandle >= 0)
                {
                    e.Info.DisplayText = dt_Grid.Rows[e.RowHandle]["dtime3hour"].ToString();
                }
                else if (e.RowHandle < 0 && e.RowHandle > -1000)
                {
                    e.Info.Appearance.BackColor = System.Drawing.Color.AntiqueWhite;
                    e.Info.DisplayText = "G" + e.RowHandle.ToString();
                }
            }
        }

        private void comboBoxEdit_Area_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxEdit_Area.Text.Equals("芦山县"))
            {
                AreaId = 1;
            }
            else
            {
                AreaId = 2;
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            //day1 = String.Format("{0}日", comboBoxEdit_Date.Text.Split('-')[2].ToString());
            //day2 = String.Format("{0}日", (Convert.ToDateTime(comboBoxEdit_Date.Text.ToString()).AddDays(1).ToString("yyyy-mm-dd").Split('-')[2].ToString()));
            //day1 = (Convert.ToDateTime(comboBoxEdit_Date.Text)).AddDays(1).ToString("yyyy-MM-dd");
            //day2 = (Convert.ToDateTime(comboBoxEdit_Date.Text)).AddDays(5).ToString("yyyy-MM-dd");
            day1 = NowTime.ToString("yyyy-MM-dd");
            day2 = NowTime.AddDays(6).ToString("yyyy-MM-dd");

            ChartSqlStr = String.Format("SELECT ID,dtime3hour,temperature,rains,wind FROM ForecastWeather WHERE AreaID = {0} AND timedate7 = #{1}#", AreaId, comboBoxEdit_Date.Text);
            //GridSqlStr = String.Format("SELECT dtime3hour,temperature,rains,wind,windd,yl,xdsd FROM ForecastWeather WHERE AreaID = {0} ORDER BY ID", AreaId);
            GridSqlStr = String.Format("SELECT ID,dtime3hour,temperature,rains,wind,windd,yl,xdsd FROM ForecastWeather WHERE AreaID = {0} AND timedate7 BETWEEN #{1}# AND #{2}# ORDER BY ID ASC", AreaId, day1, day2);
            WeekSqlStr = String.Format("SELECT ID,Hourtime,rain1h,temperature,humidity,windSpeed FROM OneHourWeather WHERE Hourtime BETWEEN #{1}# AND #{2}# AND AreaID = {0} ORDER BY Hourtime ASC", AreaId, NowTime.ToString("yyyy-MM-dd HH:mm"), NowTime.AddDays(-1).ToString("yyyy-MM-dd HH:mm"));

            initPoints();   // 初始化图表的点位信息

            dt_Chart = getDataTable(ChartSqlStr);
            CreateNewChart(dt_Chart);
            dt_Grid = getDataTable(GridSqlStr);
            CreateTable(dt_Grid);
            dt_Week = getDataTable(WeekSqlStr);
            CreateOldChart(dt_Week);
        }

        /// <summary>
        /// 初始化图表的点位信息
        /// </summary>
        private void initPoints()
        {
            oldRain1h.Points.Clear();
            oldTemperature.Points.Clear();
            oldHumidity.Points.Clear();
            oldWindspeed.Points.Clear();
            newRains.Points.Clear();
            newTemperature.Points.Clear();
            newWindspeed.Points.Clear();
        }

        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (xtraTabControl1.SelectedTabPage == xtraTabPage1)
            {
                timegroupControl.Enabled = true;
            }
            else
            {
                timegroupControl.Enabled = false;
            }
        }
    }
}
