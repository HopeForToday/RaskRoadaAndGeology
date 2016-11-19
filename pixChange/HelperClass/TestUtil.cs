using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadRaskEvaltionSystem.HelperClass
{
    class TestUtil
    {
        private static void IniRouteData(string GDBfileName, string DataSetName, string sNDSName)
        {
            m_ipPathFinder = new ClsPathFinder();
            NetData = OpenNetworkDataset(GDBfileName, DataSetName, sNDSName);
            spatialRef = m_ipPathFinder.GetSpatialReferenceFromDataset(NetData as IDataset);
            CurrentNAContext = m_ipPathFinder.CreateSolverContext(NetData);
        }
        private static INetworkDataset OpenNetworkDataset(string GDBfileName, string featureDatasetName, string sNDSName)
        {
            try
            {
                IWorkspace pWorkspace = OpenWorkspace(GDBfileName);
                IFeatureWorkspace pFeatureWorkspace = pWorkspace as IFeatureWorkspace;
                if (pFeatureWorkspace == null) return null;
                IFeatureDatasetExtensionContainer pFeatureDatasetExtensionContainer = pFeatureWorkspace.OpenFeatureDataset(featureDatasetName) as IFeatureDatasetExtensionContainer;
                if (pFeatureDatasetExtensionContainer == null) return null;
                IDatasetContainer2 pDatasetContainer2 = pFeatureDatasetExtensionContainer.FindExtension(esriDatasetType.esriDTNetworkDataset) as IDatasetContainer2;
                INetworkDataset pNetworkDataset = pDatasetContainer2.get_DatasetByName(esriDatasetType.esriDTAny, sNDSName) as INetworkDataset;
                return pNetworkDataset;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public List<SimplePoint> SolvePath()
        {
            List<SimplePoint> Result = null;
            m_ipPathFinder.SetSolverSettings(CurrentNAContext);
            string msg = m_ipPathFinder.Solve(CurrentNAContext);
            if (msg.Equals("OK"))
            {
                IPolyline PL = m_ipPathFinder.ParseResultPathLine(SessionRouteLayer);
                Result = BuildPathPointCollection(PL);
            }
            return Result;
        }

    }
}
