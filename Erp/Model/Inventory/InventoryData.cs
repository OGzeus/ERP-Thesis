using Erp.Model.BasicFiles;
using Erp.Model.Interfaces;
using Erp.Model.Manufacture.MPS;
using Syncfusion.UI.Xaml.Charts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace Erp.Model.Inventory
{
    public class InventoryData : RecordBaseModel
    {
        private int invid { get; set; }

        private string invcode { get; set; }
        private string invdescr { get; set; }
        private string location { get; set; }
        private double capacity { get; set; }
        private ObservableCollection<StockData> _StockData { get; set; }

        public int InvId
        {
            get { return invid; }
            set { invid = value; OnPropertyChanged("InvId"); }
        }
        public string InvCode
        {
            get { return invcode; }
            set { invcode = value; OnPropertyChanged("InvCode"); }
        }

        public string InvDescr
        {
            get { return invdescr; }
            set { invdescr = value; OnPropertyChanged("InvDescr"); }
        }

        public string Location
        {
            get { return location; }
            set { location = value; OnPropertyChanged("Location"); }
        }

        public double Capacity
        {
            get { return capacity; }
            set { capacity = value; OnPropertyChanged("Capacity"); }
        }

        public ObservableCollection<StockData> StockData
        {
            get { return _StockData; }
            set { _StockData = value; OnPropertyChanged("StockData"); }
        }

        public bool Production { get; set; }
        public bool Packaging{ get; set; }
        public bool FinishedGoodsInventory { get; set; }
    }
}


