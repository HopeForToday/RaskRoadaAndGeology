using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using RoadRaskEvaltionSystem.HelperClass;
using pixChange.HelperClass;
namespace pixChange
{
    public partial class LayerMangerView : Form
    {
        private List<string> LayerNameList;
        private List<string> LayerPathList;
        private bool isLoad=true;//为了初始加载时不把底图清空
        public LayerMangerView()
        {
            InitializeComponent();
            //LayerNameList=new List<string>();
            //getallLayers();
            //int i = MainFrom.m_mapControl.LayerCount;
            //exCheckedListBox1.DataSource = LayerNameList;//为什么这一句就把图层清空了呢
            //int y = MainFrom.m_mapControl.LayerCount;
            //judeLayer();
        }

        public LayerMangerView(ref List<string> LayerPathList)
        {
            InitializeComponent();
            LayerNameList = new List<string>();
            getallLayers();
            exCheckedListBox1.DataSource = LayerNameList;
            this.LayerPathList = LayerPathList;
            judeLayer();
        }
        //将图层名加载到exCheckedListBox1中
        private void getallLayers()
        {
            //加载栅格底图
            DirectoryInfo Rasfolder = new DirectoryInfo(Common.rasterPath);
            FileInfo[] fileInfos = Rasfolder.GetFiles();
            List<string> RasfileType = new List<string>();
            RasfileType.Add(".jpg");
            //fileType.Add(".shp");
            RasfileType.Add(".img");
            RasfileType.Add(".tif");
            foreach (FileInfo file in fileInfos)
            {
                //string name = file.Name;
                //这里需要加上判断是否可以成为图层数据源的代码  如：图片，矢量文件
                string ardess = file.Name.Substring(file.Name.LastIndexOf("."));
                if (RasfileType.Contains(ardess))
                { LayerNameList.Add(file.Name); }
            } 
            //加载矢量底图
            DirectoryInfo Shasfolder = new DirectoryInfo(Common.shapPath);
            FileInfo[] fileInfo2 = Shasfolder.GetFiles();
            foreach (FileInfo file in fileInfo2)
           {
                if(file.Name.Substring(file.Name.LastIndexOf("."))==".shp")
                {
                    LayerNameList.Add(file.Name);
                }
           }
        }
        //若果已经有图层,则相应的chexbox项应该被选中
        private void judeLayer()
        {
            int count=MainFrom.m_mapControl.LayerCount;
            if (count == 0) return;
            for (int i = 0; i < count;i++ )
            {
                //exCheckedListBox1.CheckedItems.Add(MainFrom.m_mapControl.get_Layer(i).Name);
                for (int y = 0; y < exCheckedListBox1.Items.Count;y++ )
                {
                    string layerName=exCheckedListBox1.Items[y].ToString();
                    int index = layerName.LastIndexOf(".");
                    if (layerName.Substring(index) == ".shp") layerName = layerName.Substring(0, index);
                    if (layerName == MainFrom.m_mapControl.get_Layer(i).Name)
                    {
                        //后台选中 改变checkState
                        exCheckedListBox1.SetItemChecked(y,true);
                    }
                }
                    //exCheckedListBox1.SetItemChecked();
            }
          
        }

        private void ok_Click(object sender, EventArgs e)
        {      
            this.Close();                            
        }

        private void exCheckedListBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            int i = exCheckedListBox1.SelectedIndex;
            var selectLayer = exCheckedListBox1.SelectedItem;
            //这里还可以用循环的方法 this.myCheckedlistBox.GetItemChecked(i)
            if (exCheckedListBox1.CheckedItems.Contains(selectLayer))
            {
                string fullPath = selectLayer.ToString();
                if (fullPath.Substring(fullPath.LastIndexOf(".")) == ".shp")
                {
                    fullPath = Common.shapPath + fullPath;//这里发现+=和普通写法还是有区别的
                    //利用"\\"将文件路径分成两部分 
                    int Position = fullPath.LastIndexOf("\\");
                    //文件目录
                    string FilePath = fullPath.Substring(0, Position);
                    //
                    string ShpName = fullPath.Substring(Position + 1);
                    IWorkspaceFactory pWF;
                    pWF = new ShapefileWorkspaceFactory();
                    IFeatureWorkspace pFWS;
                    pFWS = (IFeatureWorkspace)pWF.OpenFromFile(FilePath, 0);
                    IFeatureClass pFClass;
                    pFClass = pFWS.OpenFeatureClass(ShpName);

                    IFeatureLayer pFLayer;
                    pFLayer = new FeatureLayer();
                    pFLayer.FeatureClass = pFClass;
                    pFLayer.Name = pFClass.AliasName;

                    MainFrom.m_mapControl.AddLayer(pFLayer, 0);
                 //   MainFrom.m_mapControl.Refresh(esriViewDrawPhase.esriViewGeography, null, null);
                    //选择数据源
                    MainFrom.toolComboBox.Items.Add(pFLayer.Name);
                    MainFrom.m_pTocControl.Update();
                }
                else
                {
                    fullPath = Common.rasterPath + fullPath;
                    //这里将RasterLayerClass改为RasterLayer  即可以嵌入互操作类型  下同
                    IRasterLayer rasterLayer = new RasterLayer();

                    rasterLayer.CreateFromFilePath(fullPath);
                  // IRaster ir = (IRaster) rasterLayer;
                    MainFrom.m_mapControl.AddLayer(rasterLayer, 0);
              //      MainFrom.m_mapControl.Refresh(esriViewDrawPhase.esriViewGeography, null, null);
                    MainFrom.m_pTocControl.Update();
                }
            }
            else
            {
                string layerName = selectLayer.ToString();
                //若果已经没有选中的直接将图层清零
                if (exCheckedListBox1.CheckedItems.Count == 0 )
                {
                    if (layerName.Substring(layerName.LastIndexOf(".")) == ".shp")
                    {
                        layerName = layerName.Substring(0, layerName.LastIndexOf("."));
                        //选择数据源
                        MainFrom.toolComboBox.Items.Remove(layerName);

                    }
                    MainFrom.m_mapControl.ClearLayers();
                    MainFrom.m_pTocControl.Update();
                    return;
                }
                isLoad = true;
                if (layerName.Substring(layerName.LastIndexOf(".")) == ".shp")
                {
                    //选择数据源
                    layerName = layerName.Substring(0, layerName.LastIndexOf("."));
                    MainFrom.toolComboBox.Items.Remove(layerName);

                }
                try
                {
                    //这里需要注意的是矢量文件在图层中没有后缀名 而栅格文件在图层中有后缀名 如.tif
                    MainFrom.m_mapControl.DeleteLayer(LayerMange.returnIndexByLayerName(MainFrom.m_mapControl, layerName));
                    //  MainFrom.m_mapControl.de
                    MainFrom.m_pTocControl.Update();
                }
                catch (Exception ex)
                {

                }

            }         
        }

        private void LayerMangerView_Load(object sender, EventArgs e)
        {
         //   isLoad = false;
            exCheckedListBox1.Items.Clear();
            LayerNameList = new List<string>();
            getallLayers();
            int i = MainFrom.m_mapControl.LayerCount;
            exCheckedListBox1.DataSource = LayerNameList;//为什么这一句就把图层清空了呢 //找到原因 ：果然是应为exCheckedListBox1_SelectedIndexChanged_1事件，赋值的时候在底层执行了，我们看不见
            //foreach (var t in LayerNameList)
            //{
            //    exCheckedListBox1.Items.Add(t);
            //}  这里改为循环赋值解决了上面的问题 
            int y = MainFrom.m_mapControl.LayerCount;
            judeLayer();
            exCheckedListBox1.SelectedIndexChanged += exCheckedListBox1_SelectedIndexChanged_1;
        }

        private void exCheckedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

       
    }
}
