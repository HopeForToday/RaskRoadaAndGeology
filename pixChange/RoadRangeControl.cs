using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RoadRaskEvaltionSystem.RasterAnalysis;

namespace RoadRaskEvaltionSystem
{
    public partial class RoadRangeControl : UserControl
    {
        public RoadRangeControl(int level)
        {
            InitializeComponent();
            this.labelControl1.Text = "第" + level.ToString() + "级";
        }
        public RoadRange RoadRange
        {
            get
            {
                double minValue = double.Parse(this.textBox1.Text);
                double maxValue = double.Parse(this.textBox2.Text);
                return new RoadRange(minValue, maxValue);
            }
        }
        public RoadRangeControl(int level,RoadRange roadRange)
        {
            InitializeComponent();
            this.textBox1.Text = roadRange.MinValue.ToString();
            this.textBox2.Text = roadRange.MaxValue.ToString();
            this.labelControl1.Text = "第" + level.ToString() + "级";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
