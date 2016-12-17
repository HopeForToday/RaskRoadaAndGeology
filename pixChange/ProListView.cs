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
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System.IO;
using ESRI.ArcGIS.DataSourcesGDB;
using RoadRaskEvaltionSystem.HelperClass;
using System.Diagnostics;

namespace pixChange
{
    public partial class ProListView : Form
    {
        //private IList<IFeature> features;
        private DataTable dataTable = null;
        private IList<IFeature> pfeatuers = null;
        public ProListView()
        {
            InitializeComponent();
        }
        public ProListView(IFeatureLayer layer, IList<IFeature> features)
        {
            InitializeComponent();
            dataTable = AtrributeUtil.GetDataTable(layer, features);
            this.pfeatuers = features;
            this.dataGridView.DataSource = dataTable;
            SetDataGridViewStyle();
        }
        private void SetDataGridViewStyle()
        {
            for(int i=0;i<dataTable.Columns.Count;i++)
            {
               DataColumn column = dataTable.Columns[i];
                this.dataGridView.Columns[i].ReadOnly = column.ReadOnly;
            }
        }
        private void ProListView_Load(object sender, EventArgs e)
        {
            this.countLabel.Text = dataTable.Rows.Count.ToString();
        }

        private void oKbtt_Click(object sender, EventArgs e)
        {
            if (FeatureDealUtil.UpdateFeature(pfeatuers, dataTable))
            {
                this.Close();
            }
        }

        private void cancelBtt_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DataGridView_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                object prevIndex =  this.dataGridView.Tag;
                if (prevIndex == null || !prevIndex.Equals(e.RowIndex))
                {
                    dataGridView.Tag = e.RowIndex;
                    dataGridView.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red;
                    dataGridView.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Blue;
                }
            }
        }

        private void DataGridView_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                dataGridView.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                dataGridView.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(0, 64, 64);
            }
        }

    }
}
