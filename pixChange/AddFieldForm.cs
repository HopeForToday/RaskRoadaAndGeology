using ESRI.ArcGIS.Geodatabase;
using RoadRaskEvaltionSystem.HelperClass;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RoadRaskEvaltionSystem
{
    public partial class AddFieldForm : Form
    {
        public DataColumn AddDataColumn { get; set; }
        private IFeatureClass pFeatureClass = null;
        public AddFieldForm(IFeatureClass pFeatureClass)
        {
            InitializeComponent();
            InitialUI();
            this.pFeatureClass = pFeatureClass;
        }
        private static readonly Dictionary<string, string> TYPES = new Dictionary<string, string>()
        {
            {"整型","System.Int32"},
            {"浮点型","System.Single"},
            {"双精度浮点型","System.Double"},
            {"字符串","System.String"},
            {"日期","System.DateTime"}
        };
        private void InitialUI()
        {
            foreach (var single in TYPES)
            {
                this.typeComboBox.Items.Add(single.Key);
            }
            this.typeComboBox.SelectedIndex = 0;
        }
        public AddFieldForm()
        {
            InitializeComponent();
        }

        private void Okbutton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(nameTbb.Text))
            {
                MessageBox.Show("请输入字段名称");
                this.nameTbb.Focus();
                return;
            }
            if (string.IsNullOrEmpty(aliasTbb.Text))
            {
                MessageBox.Show("请输入字段别名");
                this.aliasTbb.Focus();
                return;
            }
            if (this.typeComboBox.SelectedIndex < 0)
            {
                MessageBox.Show("请选择数据类型");
                this.typeComboBox.Focus();
                return;
            }
            var type = AtrributeUtil.ConvertToEsriFiled(this.typeComboBox.SelectedItem.ToString());
            if (FeatureClassUtil.AddField(pFeatureClass, nameTbb.Text, aliasTbb.Text, type))
            {
                this.DialogResult = DialogResult.OK;
                CreateDataColumn();
                MessageBox.Show("添加成功");
                this.Close();
            }
            else
            {
                MessageBox.Show("添加失败，请检查字段名称是否重复");
            }
        }

        private void CreateDataColumn()
        {
            var columnType = Type.GetType(TYPES[this.typeComboBox.SelectedItem.ToString()].ToString());
            AddDataColumn = new DataColumn(nameTbb.Text, columnType);
        }

        private void Cancelbutton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
