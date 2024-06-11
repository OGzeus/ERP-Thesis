using Erp.Model.Interfaces;
using Erp.Model.Manufacture.MRP;
using Erp.Model.SupplyChain.Clusters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.SupplyChain
{
    public class CL_OutputData : RecordBaseModel
    {
        #region Clustering 

        private ObservableCollection<ClusterDatapoint> clusters { get; set; }

        public ObservableCollection<ClusterDatapoint> Clusters
        {
            get { return clusters; }
            set { clusters = value; OnPropertyChanged("Clusters"); }
        }

        private ObservableCollection<MainDatapoint> _DataPoints { get; set; }

        public ObservableCollection<MainDatapoint> DataPoints
        {
            get { return _DataPoints; }
            set { _DataPoints = value; OnPropertyChanged("DataPoints"); }
        }

        #endregion

        #region Clustering Techniques
        private KmeansOutputData _Kmeansoutputdata;
        public KmeansOutputData Kmeansoutputdata
        {
            get { return _Kmeansoutputdata; }
            set
            {
                _Kmeansoutputdata = value;
                INotifyPropertyChanged(nameof(Kmeansoutputdata));


            }
        }

        #endregion

        #region Diagram Data

        private DiagramClusteringData _Clustering_Diagram;
        public DiagramClusteringData Clustering_Diagram
        {
            get { return _Clustering_Diagram; }
            set
            {
                _Clustering_Diagram = value;
                OnPropertyChanged(nameof(Clustering_Diagram));
            }
        }

        private DiagramsMRPData _ElbowMethod_Diagram;
        public DiagramsMRPData ElbowMethod_Diagram
        {
            get { return _ElbowMethod_Diagram; }
            set
            {
                _ElbowMethod_Diagram = value;
                OnPropertyChanged(nameof(ElbowMethod_Diagram));
            }
        }
        #endregion
    }
}
