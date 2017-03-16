using DevExpress.XtraEditors;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using RoadRaskEvaltionSystem.HelperClass;
using RoadRaskEvaltionSystem.QueryAndUIDeal;
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
    public partial class PropertyQueryForm : Form
    {
        private ISpatialQueryUI spatialQuery = ServiceLocator.SpatialQueryUI;
        public PropertyQueryForm()
        {
            InitializeComponent();
        }
        private string selectLayerName;
        private ILayer selectLayer;
        private AxMapControl mapControl;
        List<ILayer> layers;
        private List<IField> fields { get; set; }
        private EventHandler ListBox2ItemHandler = null;
        #region 基本事件
        public PropertyQueryForm(string selectLayerName, AxMapControl mapControl)
        {
            InitializeComponent();
            EventDeal();
            this.mapControl = mapControl;
           layers = MapUtil.GetAllLayers(mapControl);
           this.selectLayerName = selectLayerName;
          // ListBox2ItemHandler = new EventHandler(listBoxControl2_SelectedIndexChanged);
        }

        private void PropertyQueryForm_Load(object sender, EventArgs e)
        {
            layers.ForEach(p =>
            {
                if (p is FeatureLayer)
                {
                    layerCombobox.Properties.Items.Add(p.Name);
                }
            });
            if (!string.IsNullOrEmpty(selectLayerName))
            {
                this.layerCombobox.Text = selectLayerName;
            }
        }
        private void layerCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (layerCombobox.SelectedIndex > -1)
            {
                IGroupLayer gLayer = null;
                int layerIndex;
                int gIndex;
                selectLayer = LayerUtil.QueryLayerInMap(mapControl, layerCombobox.Text, ref gLayer, out layerIndex, out gIndex);
                this.labelControl2.Text = "select * from " + selectLayer.Name + " where ";
                UpdateFieldBox(selectLayer);
                this.queryTxtBox .Text= "";
            }
        }

        private void okBtt_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.queryTxtBox.Text))
            {
                return;
            }
            try
            {
                spatialQuery.DealFeatureQuery(mapControl, null, this.queryTxtBox.Text, this.layerCombobox.Text);
                mapControl.Refresh(esriViewDrawPhase.esriViewGeography, null, null);
                this.Close();
            }
            catch (Exception er)
            {
                MessageBox.Show("请检查查询语句");
                this.queryTxtBox.Focus();
            }
        }

        private void resetBtt_Click(object sender, EventArgs e)
        {
            this.queryTxtBox.Text = "";
        }

        private void cancelBtt_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //获取唯一值事件
        private void getValueBtt_Click(object sender, EventArgs e)
        {
            int selectIndex = this.listBoxControl1.SelectedIndex;
            if (selectIndex < 0)
            {
                return;
            }
            this.valueslistBoxControl.SelectedIndexChanged -= this.ListBox2ItemHandler;
            UpdateFieldUniqueValueListbox(this.fields[this.listBoxControl1.SelectedIndex]);
            this.valueslistBoxControl.SelectedIndexChanged += this.ListBox2ItemHandler;
        }
        private void listBoxControl1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = listBoxControl1.IndexFromPoint(new Point(e.X, e.Y));
            listBoxControl1.SelectedIndex = index;
            if (listBoxControl1.SelectedIndex > -1)
            {
                string fieldName = this.listBoxControl1.SelectedItem as string;
                DealField(fieldName);
            }
        }

        private void valueslistBoxControl_MouseClick(object sender, MouseEventArgs e)
        {
            int index = valueslistBoxControl.IndexFromPoint(new Point(e.X, e.Y));
            valueslistBoxControl.SelectedIndex = index;
            if (this.valueslistBoxControl.SelectedIndex > -1)
            {
                string value = this.valueslistBoxControl.SelectedItem as string;
                DealFieldValue(value);
                this.valueslistBoxControl.SelectedIndex = -1;
            }
        }
        #endregion  
        private void EventDeal()
        {
            var numberClickHandler = new EventHandler((p, q) =>
            {
                SimpleButton button = p as SimpleButton;
                DealNumber(button.Text);
            });

            var opreateClickHandler = new EventHandler((p, q) =>
            {
                SimpleButton button = p as SimpleButton;
                DealOpreate(button.Text);
            });

            var fuhaoClickHandler = new EventHandler((p, q) =>
            {
                SimpleButton button = p as SimpleButton;
                DealFuhao(button.Text);
            });
            foreach (var control in opreateGroupControl.Controls)
            {
                if (control is SimpleButton)
                {
                    SimpleButton button = control as SimpleButton;
                    button .Click+= opreateClickHandler;
                }
            }
            foreach (var control in groupControl3.Controls)
            {
                if (control is SimpleButton)
                {
                    SimpleButton button = control as SimpleButton;
                    button.Click += numberClickHandler;
                }
            }
            foreach (var control in panel1.Controls)
            {
                if (control is SimpleButton)
                {
                    SimpleButton button = control as SimpleButton;
                    button.Click += fuhaoClickHandler;
                }
            }
        }
        private void UpdateFieldBox(ILayer layer)
        {
            this.listBoxControl1.Items.Clear();
            fields = LayerUtil.GetLayerFields(selectLayer as IFeatureLayer);
            fields.ForEach(p => this.listBoxControl1.Items.Add(String.Format("\"{0}\"", p.Name)));
        }
        private void AddQueryText(string str)
        {
            int index = this.queryTxtBox.SelectionStart;
            string text = string.Empty;
            if (index >0)
            {
                text = this.queryTxtBox.Text.Insert(index, str);
            }
            else
            {
                text =this.queryTxtBox.Text+ str;
            }
            this.queryTxtBox.Text = text;
        }
        private void DealOpreate(string opreate)
        {
            AddQueryText(" "+opreate+" ");
        }
        
        private void DealField(string fieldName)
        {
            AddQueryText(" " + fieldName + " ");
        }
        private void DealNumber(string number)
        {
            AddQueryText(number);
        }

        private void DealFuhao(string value)
        {
            AddQueryText(value);
        }
        private void DealFieldValue(string value)
        {
            AddQueryText(value);
        }
       
        /// <summary>
        /// 更新valueslistBoxControl元素
        /// </summary>
        /// <param name="field"></param>
        void UpdateFieldUniqueValueListbox(IField field)
        {
            this.valueslistBoxControl.Items.Clear();
            IList<object> values = FeatureDealUtil.GetUnikeValues(selectLayer as IFeatureLayer, field.Name, 200);
            if (field.Type == esriFieldType.esriFieldTypeString)
            {
                foreach (var value in values)
                {
                    this.valueslistBoxControl.Items.Add(String.Format("'{0}'",value));
                }
            }
            else
            {
                foreach (var value in values)
                {
                    this.valueslistBoxControl.Items.Add(value);
                }
            }
            this.valueslistBoxControl.SelectedIndex = -1;
        }
       
    }
}
