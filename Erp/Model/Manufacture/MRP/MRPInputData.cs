using Erp.Model.BasicFiles;
using Erp.Model.Customers;
using Erp.Model.Data_Analytics.Forecast;
using Erp.Model.Interfaces;
using Erp.Model.Inventory;
using Erp.Model.Manufacture.MPS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Manufacture.MRP
{
    public class MRPInputData : RecordBaseModel
    {
        private int mrpid;
        private string mrpcode;
        private string mrpdescr;
        private string[] dates;
        private string[] days;
        private bool _ForecastFlag;
        private bool _OrdersFlag;
        private bool _SelectedItems;
        private DateTime _OrdersDateFrom;
        private DateTime _OrdersDateTo;
        private ObservableCollection<ForecastInfoData> forecastdatagrid;
        private ForecastInfoData forecast;
        private InventoryData inventory;

        private InvDiagramsSearchData diagram;
        private ObservableCollection<ItemData> items;
        private ObservableCollection<BomData> bom;
        public int MRPID
        {
            get { return mrpid; }
            set { mrpid = value; OnPropertyChanged("MRPID"); }
        }

        public string MRPCode
        {
            get { return mrpcode; }
            set { mrpcode = value; OnPropertyChanged("MRPCode"); }
        }

        public string MRPDescr
        {
            get { return mrpdescr; }
            set { mrpdescr = value; OnPropertyChanged("MRPDescr"); }
        }

        public DateTime OrdersDateFrom
        {
            get { return _OrdersDateFrom; }
            set { _OrdersDateFrom = value; OnPropertyChanged("OrdersDateFrom"); }
        }
        public DateTime OrdersDateTo
        {
            get { return _OrdersDateTo; }
            set { _OrdersDateTo = value; OnPropertyChanged("OrdersDateTo"); }
        }
        public bool ForecastFlag
        {
            get { return _ForecastFlag; }
            set { _ForecastFlag = value; OnPropertyChanged("ForecastFlag"); }
        }
        public bool OrdersFlag
        {
            get { return _OrdersFlag; }
            set { _OrdersFlag = value; OnPropertyChanged("OrdersFlag"); }
        }
        public bool SelectedItems
        {
            get { return _SelectedItems; }
            set { _SelectedItems = value; OnPropertyChanged("SelectedItems"); }
        }
        public string[] Dates
        {
            get { return dates; }
            set { dates = value; OnPropertyChanged("Dates"); }
        }

        public string[] Days
        {
            get { return days; }
            set { days = value; OnPropertyChanged("Days"); }
        }
        public ForecastInfoData Forecast
        {
            get { return forecast; }
            set
            {
                forecast = value;
                ChooseMachRep();
                INotifyPropertyChanged("Forecast");
            }
        }
        public InventoryData Inventory
        {
            get { return inventory; }
            set
            {
                inventory = value;
                INotifyPropertyChanged("Inventory");
            }
        }
        public ObservableCollection<ForecastInfoData> ForecastDatagrid
        {
            get { return forecastdatagrid; }
            set { forecastdatagrid = value; OnPropertyChanged("ForecastDatagrid"); }
        }

        public ObservableCollection<BomData> Bom
        {
            get { return bom; }
            set { bom = value; OnPropertyChanged("Bom"); }
        }
        public InvDiagramsSearchData Diagram
        {
            get { return diagram; }
            set { diagram = value; OnPropertyChanged("Diagram"); }
        }



        public ObservableCollection<ItemData> Items
        {
            get { return items; }
            set { items = value; OnPropertyChanged("Items"); }
        }



        public Dictionary<string, double> MaintenanceRate { get; set; } //<ItemCode, Demand * Itemdata.Profit>
        public Dictionary<string, double> Profit { get; set; } //<ItemCode, Demand * Itemdata.Profit>
        public Dictionary<string, Dictionary<string, double>> TimeReq { get; set; }//MachineCode,<<ItemCode ,ItemProcessTime >>
        public Dictionary<string, int> MachInstalled { get; set; } //Number of Machines (tha ftiaksw diaforetikes mixanes kai tha pe3w me to enum me to machinetype 
        public Dictionary<string, List<decimal>> TotalDemandDict { get; set; } //<ItemCode, Demand * Itemdata.Profit>
        public Dictionary<(string, string), double> MaxDemand { get; set; } //Number of Machines that need to be under maintenance


        #region FactoryPlanning1,2 bool visibility 
        private bool _StackPanelEnabled;

        public bool StackPanelEnabled
        {
            get { return _StackPanelEnabled; }
            set
            {
                if (_StackPanelEnabled != value)
                {
                    _StackPanelEnabled = value;
                    OnPropertyChanged("StackPanelEnabled");
                }
            }
        }
        private void ChooseMachRep()
        {
            if (string.IsNullOrWhiteSpace(Forecast.ForCode))
            {
                StackPanelEnabled = false;
            }
            else
            {
                StackPanelEnabled = true;
            }
        }

        #endregion



    }
}
