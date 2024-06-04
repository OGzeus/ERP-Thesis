using Erp.Model.Enums;
using Erp.Model.Interfaces;
using Erp.Model.SupplyChain.Clusters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Erp.Model.Enums.BasicEnums;

namespace Erp.Model.SupplyChain
{
    public class CL_InputData : RecordBaseModel
    {


        #region Cluster Techniques

        private BasicEnums.Clustering_Techniques _CL_Enum;

        public BasicEnums.Clustering_Techniques CL_Enum
        {
            get { return _CL_Enum; }
            set
            {
                _CL_Enum = value;
                UpdateActivePanel();
                OnPropertyChanged("CL_Enum");
            }
        }
        private KmeansInputData _KmeansInputData;
        public KmeansInputData KmeansInputData
        {
            get { return _KmeansInputData; }
            set { _KmeansInputData = value; OnPropertyChanged("KmeansInputData"); }
        }


        private HierarchicalInputData _HierInputData;
        public HierarchicalInputData HierInputData
        {
            get { return _HierInputData; }
            set { _HierInputData = value; OnPropertyChanged("HierInputData"); }
        }
        #endregion
        #region Panel Changes
        public enum CL_ActivePanel
        {
            K_means,
            Hierarchical,
            Optimization

        }

        private Clustering_Techniques _activePanel_CL ;
        public Clustering_Techniques CurrentActivePanel_CL
        {
            get { return _activePanel_CL; }
            private set
            {
                if (_activePanel_CL != value)
                {
                    _activePanel_CL = value;
                    OnPropertyChanged("CurrentActivePanel");
                }
            }
        }
        private void UpdateActivePanel()
        {
            if (CL_Enum == BasicEnums.Clustering_Techniques.K_means)
            {
                CurrentActivePanel_CL = (Clustering_Techniques)CL_ActivePanel.K_means;
            }
            else if (CL_Enum == BasicEnums.Clustering_Techniques.Hierarchical)
            {
                CurrentActivePanel_CL = (Clustering_Techniques)CL_ActivePanel.Hierarchical;

            }
            else if (CL_Enum == BasicEnums.Clustering_Techniques.Optimization)
            {
                CurrentActivePanel_CL = (Clustering_Techniques)CL_ActivePanel.Optimization;
            }

        }
        #endregion

        // Dictionary to store coordinates (e.g., city codes and their corresponding coordinates)
        public Dictionary<string, (double Latitude, double Longitude)> DataPoints { get; set; }
        public Dictionary<int, (double Latitude, double Longitude)> DataPoints_Int { get; set; }

        public Dictionary<string, int> City_Index { get; set; } 



    }
}
