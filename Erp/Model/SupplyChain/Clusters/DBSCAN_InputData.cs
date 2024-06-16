using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.SupplyChain.Clusters
{
    public class DBSCAN_InputData : RecordBaseModel
    {
        private double _Epsilon;
        private int _MinPoints { get; set; }

        public double Epsilon
        {
            get { return _Epsilon; }
            set { _Epsilon = value; OnPropertyChanged("Epsilon"); }
        }
        public int MinPoints
        {
            get { return _MinPoints; }
            set { _MinPoints = value; OnPropertyChanged("MinPoints"); }
        }
    }
}
