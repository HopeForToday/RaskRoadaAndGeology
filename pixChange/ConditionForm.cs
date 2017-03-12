using RoadRaskEvaltionSystem.RasterAnalysis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RoadRaskEvaltionSystem
{
    public partial class ConditionForm : Form
    {
        DataTable st = new DataTable();
        String day1,NowDate;
        double _24hAgoRain = -1, todayRain = -1, tomorrowRain = -1;
        DateTime NowTime;
        int areaID = 1;//记录地区id 1为芦山县，2为宝兴县
        //栅格接口类
        IRoadRaskCaculate roadRaskCaculate = ServiceLocator.GetIRoadRaskCaculate();
        string strFilePath = "Provider=Microsoft.Jet.OLEDB.4.0;Data source=" + Application.StartupPath + "\\Rources\\雨量信息.mdb";
        public ConditionForm()
        {
            InitializeComponent();
            getDays();
            this.comboBoxEdit3.SelectedIndex = 0;
            this.comboBoxEdit4.Enabled = false;
            NowDate = String.Format("{0}", DateTime.Now.ToString("yyyy/MM/dd"));
            day1 = String.Format("{0}", DateTime.Now.AddDays(1).ToString("yyyy/MM/dd"));
        }

        private void comboBoxEdit2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string chosedValue = comboBoxEdit2.SelectedItem.ToString();
            if (chosedValue == "时刻")
            {
                this.comboBoxEdit4.Enabled = true;
                string sql = null;
                if (this.comboBoxEdit3.SelectedItem.ToString() == DateTime.Now.ToString("yyyy/MM/dd"))
                {
                    if (st.Rows.Count == 0)
                    {
                        sql = String.Format("SELECT timehour7 FROM ForecastWeather WHERE AreaID={0} AND ForecastWeather.timedate7 =#{1}#", areaID, NowDate);
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
                                    if (v["timehour7"].ToString() != "")
                                    {
                                        DateTime time = DateTime.Parse(v["timehour7"].ToString());
                                        this.comboBoxEdit4.Properties.Items.Add(time.ToString("HH:mm")); 
                                    }
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
                                if (v["timehour7"].ToString() != "")
                                {
                                    DateTime time = DateTime.Parse(v["timehour7"].ToString());
                                    this.comboBoxEdit4.Properties.Items.Add(time.ToString("HH:mm")); 
                                }
                            }
                        }
                    }
                }
                else
                {
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
            for (int i = 0; i < 2; i++)
            {
                this.comboBoxEdit3.Properties.Items.Add(NowTime.AddDays(i).ToString("yyyy/MM/dd"));
            }
        }
        private void comboBoxEdit3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxEdit2.SelectedItem.ToString() == "时刻")
            {
                if (this.comboBoxEdit3.SelectedItem.ToString() != DateTime.Now.ToString("yyyy/MM/dd"))
                {
                    SetItem();
                }
                else
                {
                    this.comboBoxEdit4.Properties.Items.Clear();
                    if (st.Rows.Count > 0 && st.Rows.Count < 8)
                    {
                        foreach (DataRow v in st.Rows)
                        {
                            if (v["timehour7"].ToString() != "")
                            {
                                DateTime time = DateTime.Parse(v["timehour7"].ToString());
                                this.comboBoxEdit4.Properties.Items.Add(time.ToString("HH:mm")); 
                            }
                        }
                    }
                    else
                    {
                        string sql = String.Format("SELECT timehour7 FROM ForecastWeather WHERE AreaID={0} AND ForecastWeather.timedate7 =#{1}#", areaID, NowDate);
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
                                    if (v["timehour7"].ToString() != "")
                                    {
                                        DateTime time = DateTime.Parse(v["timehour7"].ToString());
                                        this.comboBoxEdit4.Properties.Items.Add(time.ToString("HH:mm")); 
                                    }
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

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            double rain = 0;//记录雨量
            string hour = null;
            string sqlString = null;
            string _24hAgoStr = null;//24小时前降雨量
            
            if (_24hAgoRain < 0)
            {
                _24hAgoStr = String.Format("SELECT TOP 1 rain24h FROM OneHourWeather WHERE AreaID = {0} AND OneHourWeather.timedate24 = #{1}# order by ID", areaID, NowDate);
                _24hAgoRain = AddRain(_24hAgoStr, "rain24h");
            }
            if (todayRain < 0)
            {
                sqlString = String.Format("SELECT rains FROM ForecastWeather WHERE AreaID = {0} AND ForecastWeather.timedate7 =#{1}#", areaID, NowDate);
                todayRain = AddRain(sqlString, "rains");
            }
            if (this.comboBoxEdit2.SelectedItem.ToString() == "时刻")
            {
                hour = this.comboBoxEdit4.SelectedItem.ToString();
                if (this.comboBoxEdit3.SelectedItem.ToString() == DateTime.Now.ToString("yyyy/MM/dd"))
                {
                    rain = _24hAgoRain * 0.8 + todayRain;
                }
                else
                {
                    sqlString = String.Format("SELECT rains FROM ForecastWeather WHERE AreaID = {0} AND ForecastWeather.timedate7 =#{1}#", areaID, day1);
                    tomorrowRain = AddRain(sqlString, "rains");
                    rain = _24hAgoRain * 0.64 + todayRain * 0.8 + tomorrowRain;
                }
            }
            else
            {
                if (this.comboBoxEdit3.SelectedItem.ToString() == DateTime.Now.ToString("yyyy/MM/dd"))
                {
                    rain = _24hAgoRain * 0.8 + todayRain;
                }
                else
                {
                    sqlString = String.Format("SELECT rains FROM ForecastWeather WHERE AreaID = {0} AND ForecastWeather.timedate7 =#{1}#", areaID, day1);
                    tomorrowRain = AddRain(sqlString, "rains");
                    rain = _24hAgoRain * 0.64 + todayRain * 0.8 + tomorrowRain;
                }
            }
            try
            {
                roadRaskCaculate.RoadRaskCaulte(@"风险评价1.tif", rain,  Application.StartupPath+@"\Rources\RoadData\CheckedRoad");
            }
            catch (Exception)
            {
                throw;
            }
            this.Close();
        }
        private double AddRain(String sql, string FildName)
        {
            DataTable rain = RainData(sql);
            double rains = 0;
            foreach (DataRow v in rain.Rows)
            {
                rains =rains+ Convert.ToDouble(v[FildName].ToString());
            }
            return rains;
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void simpleButton2_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
