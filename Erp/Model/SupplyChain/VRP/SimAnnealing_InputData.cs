using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.SupplyChain.VRP
{
    public class SimAnnealing_InputData : RecordBaseModel
    {
        #region Initial Settings

        // Initial temperature for simulated annealing
        private double _InitialTemp { get; set; }

        // Cooling rate for simulated annealing
        private double _CoolingRate { get; set; }

        // Stopping temperature for simulated annealing
        private double _StoppingTemp { get; set; }


        // Maximum number of iterations for simulated annealing
        private int _MaxIterations { get; set; }
        private int _NumberOfDepots { get; set; }

        public double InitialTemp
        {
            get { return _InitialTemp; }
            set { _InitialTemp = value; OnPropertyChanged("InitialTemp"); }
        }

        public double CoolingRate
        {
            get { return _CoolingRate; }
            set { _CoolingRate = value; OnPropertyChanged("CoolingRate"); }
        }
        public double StoppingTemp
        {
            get { return _StoppingTemp; }
            set { _StoppingTemp = value; OnPropertyChanged("StoppingTemp"); }
        }

        public int MaxIterations
        {
            get { return _MaxIterations; }
            set { _MaxIterations = value; OnPropertyChanged("MaxIterations"); }
        }
        public int NumberOfDepots
        {
            get { return _NumberOfDepots; }
            set { _NumberOfDepots = value; OnPropertyChanged("NumberOfDepots"); }
        }
        #endregion



    }
}
