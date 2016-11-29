using RoadRaskEvaltionSystem.RasterAnalysis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RoadRaskEvaltionSystem.ServiceLocator;
using System.Data.OleDb;
using System.Text.RegularExpressions;
namespace RoadRaskEvaltionSystem
{
    public partial class ConfigForm : Form
    {
        String day1, day2;
        double _24hAgoRain=-1, todayRain=-1, tomorrowRain=-1;
        DateTime NowTime;
        DataTable st = new DataTable ();
        int areaID = 1;//记录地区id 1为芦山县，2为宝兴县
        private IRoadRiskConfig roadRisk = ServerLocator.GetRoadRiskConfig();
        //栅格接口类
        IRoadRaskCaculate roadRaskCaculate = ServerLocator.GetIRoadRaskCaculate();
        string strFilePath = "Provider=Microsoft.Jet.OLEDB.4.0;Data source=" + Application.StartupPath + "\\Rources\\雨量信息.mdb";
        public ConfigForm()
        {
            InitializeComponent();
            UISet();
            getDays();
            this.comboBoxEdit3.SelectedIndex = 0;
            this.comboBoxEdit4.Enabled = false;
            day1 = String.Format("{0}日", DateTime.Now.AddDays(1).ToString("dd"));
            day2 = String.Format("{0}日", DateTime.Now.AddDays(2).ToString("dd"));
        }


        private void UISet()
        {
            this.panel1.VerticalScroll.Visible = true;
            IDictionary<int, RoadRange> roadRanges = roadRisk.GetRoadRiskLevelFromConfig();
            foreach(var v in roadRanges)
            {
                AddArange(v.Value);
            }
            this.comboBoxEdit1.Text = roadRanges.Count.ToString();
        }
        private void AddArange(RoadRange roadRange)
        {
            RoadRangeControl roadRangeControl = null;
            if (roadRange != null)
            {
                roadRangeControl = new RoadRangeControl(this.panel1.Controls.Count + 1, roadRange);
            }
            else
            {
                roadRangeControl = new RoadRangeControl(this.panel1.Controls.Count + 1);
            }
        //    roadRangeControl.Width = 43;
        //    roadRangeControl.Height = 18;
            roadRangeControl.Location = new Point(5, (this.panel1.Controls.Count) * (roadRangeControl.Height + 5));
            this.panel1.Controls.Add(roadRangeControl);
        }
        private void UpdateConfig()
        {
            IDictionary<int,RoadRange> roadRanges=new Dictionary<int,RoadRange>();
            for (int i = 0; i < this.panel1.Controls.Count; i++)
            {
                RoadRangeControl rangeControl = (RoadRangeControl)panel1.Controls[i];
                roadRanges.Add(i+1,rangeControl.RoadRange);
            }
            roadRisk.UpdateRoadRiskLevelToConfig(roadRanges);
        }

        private void comboBoxEdit1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int countValue =int.Parse(comboBoxEdit1.SelectedItem.ToString());
            //加控件
            if (countValue >= this.panel1.Controls.Count)
            {
                for (int i = this.panel1.Controls.Count; i < countValue; i++)
                {
                  AddArange(null);
                }
            }
             //减控件
            else
            {
                List<Control> removeControls = new List<Control>();
                for (int i = this.panel1.Controls.Count; i > countValue; i--)
                {
                    removeControls.Add(this.panel1.Controls[i - 1]);
                }
                foreach(var value in removeControls)
                {
                    this.panel1.Controls.Remove(value);
                }
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            double rain = 0;//记录雨量
            string hour = null;
            string sqlString = null;
            string _24hAgoStr=null;//24小时前降雨量
            UpdateConfig();
            if (_24hAgoRain<0)
            {
                _24hAgoStr = String.Format("SELECT TOP 1 rain24h FROM OneHourWeather WHERE AreaID = {0} order by ID", areaID);
                _24hAgoRain = AddRain(_24hAgoStr, "rain24h"); 
            }
            if (todayRain<0)
            {
                sqlString = String.Format("SELECT rains FROM ForecastWeather WHERE AreaID = {0} AND ID<(SELECT ID FROM ForecastWeather WHERE ForecastWeather.dtime3hour LIKE '{1}%' AND ForecastWeather.AreaID={0})", areaID, day1);
                todayRain = AddRain(sqlString, "rains"); 
            }
            if(this.comboBoxEdit2.SelectedItem.ToString()=="时刻"){
                hour = this.comboBoxEdit4.SelectedItem.ToString();
                if (this.comboBoxEdit3.SelectedItem.ToString() == DateTime.Now.ToString("yyyy/MM/dd"))
                {
                    rain = _24hAgoRain * 0.8 + todayRain;
                }
                else
                {
                    rain = _24hAgoRain * 0.64 + todayRain * 0.8+tomorrowRain;
                }
            }
            else
            {
                if (this.comboBoxEdit3.SelectedItem.ToString()==DateTime.Now.ToString("yyyy/MM/dd"))
                {
                    rain = _24hAgoRain * 0.8 + todayRain;
                }
                else
                {
                    sqlString = String.Format("SELECT rains FROM ForecastWeather WHERE AreaID = {0} AND ID>=(SELECT ID FROM ForecastWeather WHERE ForecastWeather.dtime3hour LIKE '{1}%' AND ForecastWeather.AreaID={0}) AND ID<(SELECT ID FROM ForecastWeather WHERE ForecastWeather.dtime3hour LIKE '{2}%' AND ForecastWeather.AreaID={0})", areaID,day1,day2);
                    tomorrowRain = AddRain(sqlString, "rains");
                    rain = _24hAgoRain * 0.64 + todayRain * 0.8 + tomorrowRain;
                }
            }
            try
            {
                roadRaskCaculate.RoadRaskCaulte(@"风险评价1.tif", rain, @"..\..\Rources\RoadData\CheckedRoad");
            }
            catch (Exception)
            {
                
                throw;
            }
            this.Close();
        }

        private double AddRain(String sql,string FildName)
        {
            DataTable rain = RainData(sql);
            double rains=0;
            foreach(DataRow v in rain.Rows){
                if (v[FildName].ToString().IndexOf("毫米")>0)
                {
                    string raindata=v[FildName].ToString();
                    string result = raindata.Substring(0,raindata.Length-2);
                    rains = rains + Convert.ToDouble(result);
                }
                else if (v[FildName].ToString() == "无降水")
                {
                    rains = rains + 0;
                }
                else
                {
                    rains = Convert.ToDouble(v[FildName].ToString());
                }
            }
            return rains;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboBoxEdit2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string chosedValue = comboBoxEdit2.SelectedItem.ToString();
            if(chosedValue=="时刻"){
                this.comboBoxEdit4.Enabled = true;
                string sql=null;
                if (this.comboBoxEdit3.SelectedItem.ToString() == DateTime.Now.ToString("yyyy/MM/dd"))
                {
                    if (st.Rows.Count==0)
                    {
                        sql = String.Format("SELECT dtime3hour FROM ForecastWeather WHERE AreaID = {0} AND ID<(SELECT ID FROM ForecastWeather WHERE ForecastWeather.dtime3hour LIKE '{1}%' AND ForecastWeather.AreaID={0})", areaID, day1);
                        try
                        {
                            st = RainData(sql);
                            this.comboBoxEdit4.Properties.Items.Clear();
                            if(st.Rows.Count>=8){
                                SetItem();
                            }
                            else
                            {
                                foreach (DataRow v in st.Rows)
                                {
                                    this.comboBoxEdit4.Properties.Items.Add(v["dtime3hour"].ToString());
                                } 
                            }
                        }
                        catch (Exception)
                        {
                            throw;
                        } 
                    }
                    else
                    {
                        this.comboBoxEdit4.Properties.Items.Clear();
                        if (st.Rows.Count >= 8)
                        {
                            SetItem();
                        }
                        else
                        {
                            foreach (DataRow v in st.Rows)
                            {
                                this.comboBoxEdit4.Properties.Items.Add(v["dtime3hour"].ToString());
                            }
                        }
                    }
                }
                else{
                    SetItem();
                }
                this.comboBoxEdit4.SelectedIndex = 0;
            }
            else
            {
                this.comboBoxEdit4.Enabled = false;
            }
        }

        private void SetItem()
        {
            this.comboBoxEdit4.Properties.Items.Clear();
            this.comboBoxEdit4.Properties.Items.Add("02:00");
            this.comboBoxEdit4.Properties.Items.Add("05:00");
            this.comboBoxEdit4.Properties.Items.Add("08:00");
            this.comboBoxEdit4.Properties.Items.Add("11:00");
            this.comboBoxEdit4.Properties.Items.Add("14:00");
            this.comboBoxEdit4.Properties.Items.Add("17:00");
            this.comboBoxEdit4.Properties.Items.Add("20:00");
            this.comboBoxEdit4.Properties.Items.Add("23:00");
        }
        private void getDays()
        {
            NowTime = DateTime.Now;
            for (int i = 0; i < 2;i++ )
            {
                this.comboBoxEdit3.Properties.Items.Add(NowTime.AddDays(i).ToString("yyyy/MM/dd"));
            }
        }

        private void comboBoxEdit5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(this.comboBoxEdit5.SelectedItem.ToString()=="庐山县"){
                areaID = 1;
            }
            else
            {
                areaID = 2;
            }
        }
        private DataTable RainData(string sqlStr)
        {
            OleDbConnection con = new OleDbConnection(strFilePath);
            con.Open();
            OleDbDataAdapter data = new OleDbDataAdapter(sqlStr, con);
            DataTable dt = new DataTable();
            try
            {
                data.Fill(dt);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                con.Close();
                con.Dispose();
                data.Dispose();
            }
            return dt;
        }

        private void comboBoxEdit3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxEdit2.SelectedItem.ToString()=="时刻")
            {
                if (this.comboBoxEdit3.SelectedItem.ToString() != DateTime.Now.ToString("yyyy/MM/dd"))
                {
                    SetItem();
                }
                else
                {
                    this.comboBoxEdit4.Properties.Items.Clear();
                    if (st.Rows.Count>0&&st.Rows.Count<8)
                    {
                        foreach (DataRow v in st.Rows)
                        {
                            this.comboBoxEdit4.Properties.Items.Add(v["dtime3hour"].ToString());
                        } 
                    }
                    else
                    {
                       string sql = String.Format("SELECT dtime3hour FROM ForecastWeather WHERE AreaID = {0} AND ID<(SELECT ID FROM ForecastWeather WHERE ForecastWeather.dtime3hour LIKE '{1}%' AND ForecastWeather.AreaID={0})", areaID, day1);
                        try
                        {
                            st = RainData(sql);
                            this.comboBoxEdit4.Properties.Items.Clear();
                            if (st.Rows.Count >= 8)
                            {
                                SetItem();
                            }
                            else
                            {
                                foreach (DataRow v in st.Rows)
                                {
                                    this.comboBoxEdit4.Properties.Items.Add(v["dtime3hour"].ToString());
                                }
                            }
                        }
                        catch (Exception)
                        {
                            throw;
                        } 
                    }
                }
                this.comboBoxEdit4.SelectedIndex = 0;
            }
        }
    }
}
