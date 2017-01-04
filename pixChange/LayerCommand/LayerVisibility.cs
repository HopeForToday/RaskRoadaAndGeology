using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoadRaskEvaltionSystem
{

        public sealed class LayerVisibility :BaseCommand, ICommandSubType
        {
            private IHookHelper hookHelper;
            private long subType;
            public LayerVisibility()
            {
            }
            public override void OnCreate(object hook)
            {
                hookHelper = new HookHelperClass();
                hookHelper.Hook = hook;
            }
            public override void OnClick()
            {
                for (int i = 0; i <= hookHelper.FocusMap.LayerCount - 1; i++)
                {
                    if (((hookHelper.FocusMap.get_Layer(i) as IFeatureLayer) as IFeatureSelection) != null)
                    {
                        string t = hookHelper.FocusMap.get_Layer(i).Name;
                        //((hookHelper.FocusMap.get_Layer(i) as IFeatureLayer) as IFeatureSelection).Clear();
                    }
                    if (subType == 1) hookHelper.FocusMap.get_Layer(i).Visible = true;
                    if (subType == 2) hookHelper.FocusMap.get_Layer(i).Visible = false;
                }
                hookHelper.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
                hookHelper.ActiveView.Refresh();
            }
            public override string Caption
            {
                get
                {
                    if (subType == 1) return "显示所有图层";
                    else return "隐藏所有图层";
                }
            }
            public override bool Enabled
            {
                get
                {
                    bool enabled = false; int i;
                    if (subType == 1)
                    {
                        for (i = 0; i <= hookHelper.FocusMap.LayerCount - 1; i++)
                        {
                            if (hookHelper.ActiveView.FocusMap.get_Layer(i).Visible == false)
                            {
                                enabled = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (i = 0; i <= hookHelper.FocusMap.LayerCount - 1; i++)
                        {
                            if (hookHelper.ActiveView.FocusMap.get_Layer(i).Visible == true)
                            {
                                enabled = true;
                                break;
                            }
                        }
                    }
                    return enabled;
                }
            }
            #region ICommandSubType 成员
            public int GetCount()
            {
                return 2;
            }
            public void SetSubType(int SubType)
            {
                subType = SubType;
            }
            #endregion
        }
}
