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
        public PropertyQueryForm(string selectLayerName, AxMapControl mapControl)
        {
            InitializeComponent();
            this.mapControl = mapControl;
           layers = MapUtil.GetAllLayers(mapControl);
           this.selectLayerName = selectLayerName;
        }
        private void PropertyQueryForm_Load(object sender, EventArgs e)
        {
            layers.ForEach(p =>
            {
                layerCombobox.Properties.Items.Add(p.Name);
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
                UpdateListBox(selectLayer);
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
        #region 操作符事件
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DealOpreate("=");
        }
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            DealOpreate("<>");
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            DealOpreate("like");
        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            DealOpreate(">");
        }

        private void simpleButton5_Click(object sender, EventArgs e)
        {
            DealOpreate(">=");
        }

        private void simpleButton6_Click(object sender, EventArgs e)
        {
            DealOpreate("and");
        }

        private void simpleButton8_Click(object sender, EventArgs e)
        {
            DealOpreate("<");
        }

        private void simpleButton7_Click(object sender, EventArgs e)
        {
            DealOpreate("<=");
        }

        private void simpleButton9_Click(object sender, EventArgs e)
        {
            DealOpreate("or");
        }

        private void simpleButton12_Click(object sender, EventArgs e)
        {
            DealOpreate("%");
        }

        private void simpleButton10_Click(object sender, EventArgs e)
        {
            DealOpreate("()");
        }

        private void simpleButton11_Click(object sender, EventArgs e)
        {
            DealOpreate("not");
        }

        private void simpleButton13_Click(object sender, EventArgs e)
        {
            DealOpreate("is");
        }

        #endregion
        private void UpdateListBox(ILayer layer)
        {
            this.listBoxControl1.Items.Clear();
            List<IField> fields = LayerUtil.GetLayerFields(selectLayer as IFeatureLayer);
            fields.ForEach(p => this.listBoxControl1.Items.Add(p.Name));
        }
        private void DealOpreate(string opreate)
        {

            this.queryTxtBox.Text = string.Format("{0} {1} ",this.queryTxtBox.Text,opreate);
        }
        private void DealField(string fieldName)
        {
            this.queryTxtBox.Text = string.Format("{0} {1} ", this.queryTxtBox.Text, fieldName);
        }
        private void DealNumber(string number)
        {
            this.queryTxtBox.Text = string.Format("{0}{1} ", this.queryTxtBox.Text, number);
        }
        private void listBoxControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listBoxControl1.SelectedIndex > -1)
            {
                string fieldName = this.listBoxControl1.SelectedItem as string;
                DealField(fieldName);
                this.listBoxControl1.SelectedIndex=-1;
            }
        }

        private void simpleButton14_Click(object sender, EventArgs e)
        {
            DealNumber("1");
        }

        private void simpleButton15_Click(object sender, EventArgs e)
        {
            DealNumber("2");
        }

        private void simpleButton16_Click(object sender, EventArgs e)
        {
            DealNumber("3");
        }

        private void simpleButton19_Click(object sender, EventArgs e)
        {
            DealNumber("4");
        }

        private void simpleButton18_Click(object sender, EventArgs e)
        {
            DealNumber("5");
        }

        private void simpleButton17_Click(object sender, EventArgs e)
        {
            DealNumber("6");
        }

        private void simpleButton22_Click(object sender, EventArgs e)
        {
            DealNumber("7");
        }

        private void simpleButton21_Click(object sender, EventArgs e)
        {
            DealNumber("8");
        }

        private void simpleButton20_Click(object sender, EventArgs e)
        {
            DealNumber("9");
        }

        private void simpleButton23_Click(object sender, EventArgs e)
        {
            DealNumber(".");
        }

        private void simpleButton24_Click(object sender, EventArgs e)
        {
            DealNumber("0");
        }
    }
}
