using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadRaskEvaltionSystem
{
    /// <summary>
    /// 缩放至整个图层
    /// </summary>
    public sealed class ZoomToLayer : BaseCommand
    {
        private IMapControl3 m_mapControl;
        public ZoomToLayer()
        {
            base.m_caption = "缩放至图层";
        }
        public override void OnClick()
        {
            ILayer layer = (ILayer)m_mapControl.CustomProperty;
            m_mapControl.Extent = layer.AreaOfInterest;
        }
        public override void OnCreate(object hook)
        {
            m_mapControl = (IMapControl3)hook;
        }
    }
}
