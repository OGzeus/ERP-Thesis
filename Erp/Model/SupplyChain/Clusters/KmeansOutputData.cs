using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.SupplyChain.Clusters
{
    public class KmeansOutputData
    {
        public class ClusterData
        {
            public int ClusterId { get; set; }
            public string CentroidCode { get; set; }
            public double CentroidLatitude { get; set; }
            public double CentroidLongitude { get; set; }
            //public ObservableCollection<(double Latitude, double Longitude)> DataPoints { get; set; }
            public ObservableCollection<MainDatapoint> DataPoints { get; set; }

        }

        public List<ClusterData> Clusters { get; set; }
        public double SilhouetteScore { get; set; }
        public double WCSS { get; set; }

        public KmeansOutputData()
        {
            Clusters = new List<ClusterData>();
        }
    }
}

