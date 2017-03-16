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
            this.pfeatuers = features;
            dataTable = AtrributeUtil.GetDataTable(layer, features);
            this.dataGridView.DataSource = dataTable;
        }
        //刷新要素 在增加、删除字段后必须进行调用
        public void RefreshFeaturesByFids()
        {
           var fids = new List<int>();
            pfeatuers.ForEach(p => {
                var value = p.get_Value(p.Fields.FindField("FID"));
                fids.Add((int)value);
            });
            pfeatuers = FeatureDealUtil.FindFeatures(layer, fids);
        }
        #region 删除字段
        private int toDeleteColumnIndex = -1;
        private void dataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                toDeleteColumnIndex = e.ColumnIndex;
                //如果是无法删除的字段则根本就不弹出菜单
                if(!couldDeleteFiled(e.ColumnIndex))
                {
                    return;
                }
                System.Drawing.Point point = dataGridView.PointToScreen(new System.Drawing.Point(0, 0));
                int x = 0;
                DataGridViewColumnCollection columns = dataGridView.Columns;
                for (int i = 0; i < e.ColumnIndex; i++)
                {
                    if (columns[i].Visible)
                    {
                        x += columns[i].Width;
                    }
                }
                contextMenuStrip1.Show(dataGridView.PointToScreen(new System.Drawing.Point(x + e.X, e.Y+5)));
            }
        }
        /// <summary>
        /// 检查字段是否可以修改
        /// 一般FID字段和Shape字段不可以修改
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        private bool couldDeleteFiled(int columnIndex)
        {
             if (columnIndex < 1)
            {
                return false;
            }
            var columnName = this.dataGridView.Columns[columnIndex].Name;
            for(int i=0;i<this.layer.FeatureClass.Fields.FieldCount;i++){
                IField tempFiled=this.layer.FeatureClass.Fields.get_Field(i);
                if (tempFiled.Name == columnName)
                {
                    return !tempFiled.Required;
                }
            }
           return false;
        }
        private void removeColumnMenuItem_Click(object sender, EventArgs e)
        {
            if (toDeleteColumnIndex < 1)
            {
                return;
            }
            var columnName = this.dataGridView.Columns[toDeleteColumnIndex].Name;
            RemoveColumn(columnName);
            toDeleteColumnIndex = -1;
        }
        //删除字段
        private void RemoveColumn(string columnName)
        {
            FeatureClassUtil.DeleteField(layer.FeatureClass, columnName);
            dataTable.Columns.Remove(columnName);
            RefreshFeaturesByFids();
        }
        #endregion
        #region 添加字段
        //添加字段
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            var addForm = new AddFieldForm(this.layer.FeatureClass);

            if (addForm.ShowDialog() == DialogResult.OK)
            {
                var dataColumn = addForm.AddDataColumn;
                dataTable.Columns.Add(dataColumn);
                RefreshFeaturesByFids();
            }
        }
        #endregion
        #region 删除选中行
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
                deFeatures.ForEach(feature =>this.pfeatuers.Remove(feature));
                this.countLabel.Text = dataTable.Rows.Count.ToString();
            }
            else{
                MessageBox.Show("删除错误");
            }
        }
        #endregion
        #region 更新数据行
        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                this.dataGridView.EndEdit();
                var dRow = dataTable.Rows[e.RowIndex];
                var pFeature = this.pfeatuers[e.RowIndex];
                FeatureDealUtil.UpdateFeature(pFeature, dRow);
            }
        }
        private void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("输入数据不合法", "错误");
        }
        #endregion
        private void cancelBtt_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void ProListView_Load(object sender, EventArgs e)
        {
            this.countLabel.Text = dataTable.Rows.Count.ToString();
        }
    }
}
