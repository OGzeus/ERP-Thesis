using Erp.Model.BasicFiles;
using Erp.Model.Interfaces;
using Erp.Model.Manufacture.MRP;
using Erp.Model.SupplyChain.Clusters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.SupplyChain.TSP
{
    public class AntColony_TSP_OutputData : RecordBaseModel
    {
        public List<int> BestTour { get; set; }
        public double BestTourLength { get; set; }
        public double[,] PheromoneLevels { get; set; }

        public AntColony_TSP_OutputData()
        {
            BestTour = new List<int>();
        }


    }
}
