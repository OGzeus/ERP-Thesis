using Erp.Model.BasicFiles;
using Erp.Model.Customers;
using Erp.Model.Inventory;
using Erp.ViewModel.Inventory;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Manufacture.MRP
{
    public class DiagramsMRPData : INotifyPropertyChanged
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

        private ObservableCollection<DataPerDayMRP> _DataPerDayMRP;
        public ObservableCollection<DataPerDayMRP> DataPerDayMRP
        {
            get { return _DataPerDayMRP; }
            set
            {
                _DataPerDayMRP = value;
                OnPropertyChanged(nameof(DataPerDayMRP));
            }
        }
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

        private ItemData item { get; set; }
        public ItemData Item
        {
            get { return item; }
            set { item = value; OnPropertyChanged("Item"); }
        }

        private WorkcenterData workcenter { get; set; }
        public WorkcenterData Workcenter
        {
            get { return workcenter; }
            set { workcenter = value; OnPropertyChanged("Workcenter"); }
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
