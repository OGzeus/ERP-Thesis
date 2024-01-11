using CommunityToolkit.Mvvm.ComponentModel;
using Erp.Model.BasicFiles;
using Erp.Model.Customers;
using Erp.Model.Suppliers;
using Erp.ViewModel.Inventory;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Inventory
{
    public class InvDiagramsSearchData : INotifyPropertyChanged
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

        private ObservableCollection<OptimizationResultsInvData> diagramdata;
        public ObservableCollection<OptimizationResultsInvData> DiagramData
        {
            get { return diagramdata; }
            set
            {
                diagramdata = value;
                OnPropertyChanged(nameof(DiagramData));
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

        private InventoryData inventory { get; set; }
        private ItemData item { get; set; }

        private CustomerData customer { get; set; }
        private PriceListData priceList { get; set; }
        private CityData city { get; set; }

        private DateTime datestart { get; set; }
        private DateTime dateend { get; set; }

        private List<string> periodPolicy { get; set; }
  

        private string selectedperiod;

        public CustomerData Customer
        {
            get { return customer; }
            set { customer = value; OnPropertyChanged("Customer"); }
        }

        public PriceListData PriceList
        {
            get { return priceList; }
            set { priceList = value; OnPropertyChanged("PriceList"); }
        }

        public CityData City
        {
            get { return city; }
            set { city = value; OnPropertyChanged("City"); }
        }

        public InventoryData Inventory
        {
            get { return inventory; }
            set { inventory = value; OnPropertyChanged("Inventory"); }
        }

        public ItemData Item
        {
            get { return item; }
            set { item = value; OnPropertyChanged("Item"); }
        }



        public DateTime DateStart
        {
            get { return datestart; }
            set { datestart = value; OnPropertyChanged("DateStart"); }
        }
        public DateTime DateEnd
        {
            get { return dateend; }
            set { dateend = value; OnPropertyChanged("DateEnd"); }
        }


        public List<string> PeriodPolicy
        {
            get { return periodPolicy; }
            set { periodPolicy = value; OnPropertyChanged("PeriodPolicy"); }
        }

        public string SelectedPeriod
        {
            get { return selectedperiod; }
            set { selectedperiod = value; OnPropertyChanged("SelectedPeriod"); }
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

