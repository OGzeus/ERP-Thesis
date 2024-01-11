using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.ViewModel.Inventory
{
    public class OptimizationResultsInvData :INotifyPropertyChanged
    {
        //public float Demand { get; set; }
        public float DemandForecast { get; set; }
        public float BatchSize { get; set; }
        public float StockQ { get; set; }
        public bool ProductionBool { get; set; }
        public DateTime iDay { get; set; }

        private List<string> periodPolicy { get; set; }


        private float demand;

        public float Demand
        {
            get { return demand; }
            set { demand = value; OnPropertyChanged("Demand"); }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}
