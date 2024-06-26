﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.SupplyChain.Clusters
{
    public class DBSCAN_OutputData
    {
        public class ClusterData
        {
            public int ClusterId { get; set; }
            public string CentroidCode { get; set; }
            public double CentroidLatitude { get; set; }
            public double CentroidLongitude { get; set; }
            public double DaviesBouldinIndex { get; set; }

            
            public ObservableCollection<MainDatapoint> DataPoints { get; set; }

        }
        public List<ClusterData> Clusters { get; set; }
        public double SilhouetteScore_Avg { get; set; }
        public double DaviesBouldinIndex_Avg { get; set; }
        public double NoiseRatio { get; set; }
        // New variables to store data needed for plotting


        public DBSCAN_OutputData()
        {
            Clusters = new List<ClusterData>();

        }

    }
}
