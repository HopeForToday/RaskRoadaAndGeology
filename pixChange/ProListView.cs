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
using RoadRaskEvaltionSystem;

namespace pixChange
{
    public partial class ProListView : Form
    {
        //private IList<IFeature> features;
        private DataTable dataTable = null;
        private List<IFeature> pfeatuers = null;
        private IFeatureLayer layer = null;
        public ProListView()
        {
            InitializeComponent();
        }
        public ProListView(IFeatureLayer layer, List<IFeature> features)
        {
            InitializeComponent();
            this.layer = layer;
            dataTable = AtrributeUtil.GetDataTable(layer, features);
            this.pfeatuers = features;
            this.dataGridView.DataSource = dataTable;
            SetDataGridViewStyle();
        }
        private void SetDataGridViewStyle()
        {
            for(int i=1;i<dataTable.Columns.Count;i++)
            {
               DataColumn column = dataTable.Columns[i];
               // this.dataGridView.Columns[i].ReadOnly = column.ReadOnly;
                BindContextMenu(this.dataGridView.Columns[i]);
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
        private void BindContextMenu(DataGridViewColumn column)
        {
            column.HeaderCell.ContextMenuStrip = contextMenuStrip1;
        }

        private void removeColumnMenuItem_Click(object sender, EventArgs e)
        {
         //  string columnName= dataGridView..Value.ToString();
            string columnName = "RTEG";
           RemoveColumn(columnName);
        }
        //删除字段
        private void RemoveColumn(string columnName)
        {
            FeatureClassUtil.DeleteField(layer.FeatureClass, columnName);
            dataTable.Columns.Remove(columnName);
        }
        //添加字段
        private void AddColumnMenuItem_Click(object sender, EventArgs e)
        {
           var addForm=new AddFieldForm(this.layer.FeatureClass);
           if (addForm.DialogResult == DialogResult.OK)
           {
               var dataColumn = addForm.AddDataColumn;
               dataTable.Columns.Add(dataColumn);
           }
        }
        private List<IFeature> GetIsGoingDeletedRows(out  List<DataRow> rows)
        {
            var deFeatures = new List<IFeature>();
            rows = new List<DataRow>();
            var count = this.dataGridView.Rows.Count;
            for (int i = 0; i < count; i++)
            {
                //如果DataGridView是可编辑的，将数据提交，否则处于编辑状态的行无法取到  
                this.dataGridView.EndEdit();
                var checkCell = (DataGridViewCheckBoxCell)dataGridView.Rows[i].Cells[0];
                var flag = Convert.ToBoolean(checkCell.Value);
                if (flag == true)     //查找被选择的数据行  
                {
                    rows.Add(this.dataTable.Rows[i]);
                    deFeatures.Add(this.pfeatuers[i]);
                }
            }
            return deFeatures;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            List<DataRow> rows;
            var deFeatures = GetIsGoingDeletedRows(out rows);
            if (rows.Count == 0)
            {
                MessageBox.Show("尚未选择任何行，无法删除");
                return;
            }
            if (FeatureDealUtil.DeleteFeatures(deFeatures))
            {
                rows.ForEach(row =>
                {
                    this.dataTable.Rows.Remove(row);
                });
                deFeatures.ForEach(feature =>
                   this.pfeatuers.Remove(feature));
                this.countLabel.Text = dataTable.Rows.Count.ToString();
            }
            else{
                MessageBox.Show("删除错误");
            }
        }
        /*
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
        */
    }
}
