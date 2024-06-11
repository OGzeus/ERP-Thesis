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

        private DiagramsMRPData _Diagrams;
        private ObservableCollection<ItemData> items;
        private ObservableCollection<ItemData> bomitems;
        private ObservableCollection<WorkcenterData> _workcenters;

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


        public ObservableCollection<ItemData> EndItems
        {
            get { return items; }
            set { items = value; OnPropertyChanged("EndItems"); }
        }
        public ObservableCollection<ItemData> BomItems
        {
            get { return bomitems; }
            set { bomitems = value; OnPropertyChanged("BomItems"); }
        }

        public ObservableCollection<WorkcenterData> Workcenters
        {
            get { return _workcenters; }
            set { _workcenters = value; OnPropertyChanged("Workcenters"); }
        }

        #region Old Dictionaries
        //public Dictionary<string, double> MaintenanceRate { get; set; } //<ItemCode, Demand * Itemdata.Profit>
        //public Dictionary<string, double> Profit { get; set; } //<ItemCode, Demand * Itemdata.Profit>
        //public Dictionary<string, Dictionary<string, double>> TimeReq { get; set; }//MachineCode,<<ItemCode ,ItemProcessTime >>
        //public Dictionary<string, int> MachInstalled { get; set; } //Number of Machines (tha ftiaksw diaforetikes mixanes kai tha pe3w me to enum me to machinetype 
        public Dictionary<string, List<decimal>> TotalDemandDict { get; set; } //<ItemCode, Demand * Itemdata.Profit>
        //public Dictionary<(string, string), double> MaxDemand { get; set; } //Number of Machines that need to be under maintenance
        #endregion

        #region MRP_Optimization_input 
        public int T { get; set; }  //Planning Horizon

        public int P { get; set; }//Number Of MPS end-item Products
        public int Q { get; set; }//Number Of MRP Component Products

        private int _W;
        public int W
        {
            get { return _W; }
            set
            {
                _W = value;
                INotifyPropertyChanged("W");
            }
        }
        //Sets:

        public Dictionary<string, List<string>> Pw { get; set; }  // P(w) is the set of end-items that can be processed on work center w.
        public Dictionary<string, List<string>> Qw { get; set; }  // Q(w) is the set of products that can be processed on work center w.
        public Dictionary<(string, string), double> Dit { get; set; } // Dit represents the demand for product i at the end of period t.
        public Dictionary<string, List<string>> Ci { get; set; }  //  C(i) represents the set of direct subcomponents of each component or end-item 
        public Dictionary<(string, string), double> Rij { get; set; } //Rij represents the number of units of direct subcomponent i required in each unit of component or end-item j.
        public Dictionary<(string, string), double> Awt { get; set; } //Awt represents the available capacity at work center w during period t.
        public Dictionary<(string, string,string), double> Sijw { get; set; } // Sijw represents the setup times for all products and workstations.
        public Dictionary<(string, string, string), double> SCijw { get; set; } // SCijw represents the setup cost for all products and workstations.
        public Dictionary<(string, string,string), double> Miwt { get; set; }  //Mjwt represents the maximum production quantity for product j at work center w during period t
        public Dictionary<(string, string), double> Uiw { get; set; } //	Uiw represents the unit production times for all products and workstations
        public Dictionary<string, double> Hi { get; set; } //	Hi
        public Dictionary<string, double> Gi { get; set; } //	Gi
        public Dictionary<string, string> I0W { get; set; } //	I0W

        public Dictionary<string, (double, double)> Ii { get; set; } //	itemcode = (Starting Inventory ,Backlong)
        public Dictionary<string, (double, double)> Imax_min { get; set; } // itemcode = (Inventory Max , Inventory Min)

        #endregion

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
