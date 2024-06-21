using Erp.Model.Interfaces;
using Erp.Model.Manufacture.MRP;
using Erp.Model.SupplyChain.Clusters;
using Erp.Model.SupplyChain.TSP;
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
        private DBSCAN_OutputData _DBSCANoutputdata;
        public DBSCAN_OutputData DBSCANoutputdata
        {
            get { return _DBSCANoutputdata; }
            set
            {
                _DBSCANoutputdata = value;
                INotifyPropertyChanged(nameof(DBSCANoutputdata));


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

        private DiagramData _SilhouetteScores_Diagram;
        public DiagramData SilhouetteScores_Diagram
        {
            get { return _SilhouetteScores_Diagram; }
            set
            {
                _SilhouetteScores_Diagram = value;
                OnPropertyChanged(nameof(SilhouetteScores_Diagram));
            }
        }

        private DiagramData _DaviesBouldinIndex_Diagram;
        public DiagramData DaviesBouldinIndex_Diagram
        {
            get { return _DaviesBouldinIndex_Diagram; }
            set
            {
                _DaviesBouldinIndex_Diagram = value;
                OnPropertyChanged(nameof(DaviesBouldinIndex_Diagram));
            }
        }

        public CL_OutputData()
        {
            Clustering_Diagram = new DiagramClusteringData();
            SilhouetteScores_Diagram = new DiagramData();
            DaviesBouldinIndex_Diagram = new DiagramData();
        }
        #endregion
    }
}
