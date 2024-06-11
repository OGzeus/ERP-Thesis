using Erp.Model.BasicFiles;
using Erp.Model.Manufacture.MRP;
using Erp.Model.Manufacture;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveCharts;

namespace Erp.Model.SupplyChain.Clusters
{
    public class DiagramClusteringData : INotifyPropertyChanged
    {

        private string[] labels;
        public string[] Labels
        {
            get { return labels; }
            set
            {
                labels = value;
                OnPropertyChanged(nameof(Labels));
            }
        }

        public Func<double, string> Formatter { get; set; }
        public Func<double, string> YFormatter { get; set; }

        private SeriesCollection seriescollection;
        public SeriesCollection SeriesCollection
        {
            get { return seriescollection; }
            set
            {
                seriescollection = value;
                OnPropertyChanged(nameof(SeriesCollection));
            }
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
