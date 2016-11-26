using RoadRaskEvaltionSystem.RasterAnalysis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Text.RegularExpressions;
namespace RoadRaskEvaltionSystem
{
    public partial class ConfigForm : Form
    {
        int areaID = 1;//记录地区id 1为芦山县，2为宝兴县
        private IRoadRiskConfig roadRisk = ServiceLocator.GetRoadRiskConfig();
        public ConfigForm()
        {
            InitializeComponent();
            UISet();
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
            UpdateConfig();
            this.Close();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.Close();
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
    }
}
