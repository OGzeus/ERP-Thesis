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
        #region Clustering 1

        private ObservableCollection<ClusterDatapoint> clusters1 { get; set; }

        public ObservableCollection<ClusterDatapoint> Clusters1
        {
            get { return clusters1; }
            set { clusters1 = value; OnPropertyChanged("Clusters1"); }
        }

        private ObservableCollection<MainDatapoint> _DataPoints1 { get; set; }

        public ObservableCollection<MainDatapoint> DataPoints1
        {
            get { return _DataPoints1; }
            set { _DataPoints1 = value; OnPropertyChanged("DataPoints1"); }
        }

        #endregion

        #region Clustering 2


        private ObservableCollection<ClusterDatapoint> clusters2 { get; set; }

        public ObservableCollection<ClusterDatapoint> Clusters2
        {
            get { return clusters2; }
            set { clusters2 = value; OnPropertyChanged("Clusters2"); }
        }


        private ObservableCollection<MainDatapoint> _DataPoints2 { get; set; }

        public ObservableCollection<MainDatapoint> DataPoints2
        {
            get { return _DataPoints2; }
            set { _DataPoints2 = value; OnPropertyChanged("DataPoints2"); }
        }

        #endregion

        #region Diagram Data

        private DiagramsMRPData _Diagram1;
        public DiagramsMRPData Diagram1
        {
            get { return _Diagram1; }
            set
            {
                _Diagram1 = value;
                OnPropertyChanged(nameof(Diagram1));
            }
        }


        private DiagramsMRPData _Diagram2;
        public DiagramsMRPData Diagram2
        {
            get { return _Diagram2; }
            set
            {
                _Diagram2 = value;
                OnPropertyChanged(nameof(Diagram2));
            }
        }
        #endregion
    }
}
