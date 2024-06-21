using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.SupplyChain.TSP
{
    public class SAnnealing_TSP_OutputData : RecordBaseModel
    {
        public List<int> BestTour { get; set; }
        public double BestTourLength { get; set; }

        public SAnnealing_TSP_OutputData()
        {
            BestTour = new List<int>();
        }


    }
}
