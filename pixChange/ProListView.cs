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

namespace pixChange
{
    public partial class ProListView : Form
    {
        //private IList<IFeature> features;
        private DataTable dataTable = null;
        public ProListView()
        {
            InitializeComponent();
        }
        public ProListView(IFeatureLayer layer, IList<IFeature> features)
        {
            InitializeComponent();
            dataTable = AtrributeUtil.GetDataTable(layer, features);
            this.DataGrdView.DataSource = dataTable;
        }

    }
}
