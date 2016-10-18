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
namespace RoadRaskEvaltionSystem
{
    public partial class ConfigForm : Form
    {
        private IRoadRiskConfig roadRisk = ServerLocator.GetRoadRiskConfig();
        public ConfigForm()
        {
            InitializeComponent();
            UISet();
        }


        private void UISet()
        {
            this.panelControl1.VerticalScroll.Visible = true;
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
                roadRangeControl = new RoadRangeControl(this.panelControl1.Controls.Count+1,roadRange);
            }
            else
            {
                roadRangeControl = new RoadRangeControl(this.panelControl1.Controls.Count + 1);
            }
        //    roadRangeControl.Width = 43;
        //    roadRangeControl.Height = 18;
            roadRangeControl.Location = new Point(5, (this.panelControl1.Controls.Count) * (roadRangeControl.Height+5));
            this.panelControl1.Controls.Add(roadRangeControl);
        }
        private void UpdateConfig()
        {
            IDictionary<int,RoadRange> roadRanges=new Dictionary<int,RoadRange>();
            for(int i=0;i<this.panelControl1.Controls.Count;i++)
            {
                RoadRangeControl rangeControl = (RoadRangeControl)panelControl1.Controls[i];
                roadRanges.Add(i+1,rangeControl.RoadRange);
            }
            roadRisk.UpdateRoadRiskLevelToConfig(roadRanges);
        }

        private void comboBoxEdit1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int countValue =int.Parse(comboBoxEdit1.SelectedItem.ToString());
            //加控件
            if(countValue>=this.panelControl1.Controls.Count)
            {
                for (int i = this.panelControl1.Controls.Count; i < countValue; i++)
                {
                  AddArange(null);
                }
            }
             //减控件
            else
            {
                List<Control> removeControls = new List<Control>();
                for (int i = this.panelControl1.Controls.Count; i >countValue; i--)
                {
                    removeControls.Add(this.panelControl1.Controls[i - 1]);
                }
                foreach(var value in removeControls)
                {
                    this.panelControl1.Controls.Remove(value);
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

    }
}
