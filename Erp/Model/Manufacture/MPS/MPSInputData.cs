using Erp.Model.BasicFiles;
using Erp.Model.Customers;
using Erp.Model.Data_Analytics;
using Erp.Model.Data_Analytics.Forecast;
using Erp.Model.Enums;
using Erp.Model.Interfaces;
using Erp.Model.Inventory;
using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Manufacture.MPS
{
    public class MPSInputData : RecordBaseModel
    {
        private int mpsid;
        private string mpscode;
        private string mpsdescr;
        private double holdingcost;
        private int maxinventory;
        private int invstoretarget;
        private int hourspermonth;
        private string[] datesstr;
        private DateTime[] dates;

        public int MPSId
        {
            get { return mpsid; }
            set { mpsid = value; OnPropertyChanged("MPSId"); }
        }


        public string[] DatesStr
        {
            get { return datesstr; }
            set { datesstr = value; OnPropertyChanged("DatesStr"); }
        }

        public DateTime[] Dates
        {
            get { return dates; }
            set { dates = value; OnPropertyChanged("Dates"); }
        }

        private string[] months;
        public string[] Months
        {
            get { return months; }
            set { months = value; OnPropertyChanged("Months"); }
        }
        private ForecastInfoData forecast;
        private InvDiagramsSearchData diagram;

        private ObservableCollection<ForecastInfoData> forecastdatagrid;

        private ObservableCollection<ItemData> items;
        private ObservableCollection<ItemProcessData>  itemprocesses;
        private ObservableCollection<MachineData> machines;
        private ObservableCollection<MachineData> primarymachines;


        private ObservableCollection<MachineRepairData> machrepairdatedata;
        public ObservableCollection<MachineRepairData> MachRepairDateData
        {
            get { return machrepairdatedata; }
            set { machrepairdatedata = value; OnPropertyChanged("MachRepairDateData"); }
        }

        private ObservableCollection<MachineRepairData> machrepaironlydata;
        public ObservableCollection<MachineRepairData> MachRepairOnlyData
        {
            get { return machrepaironlydata; }
            set { machrepaironlydata = value; OnPropertyChanged("MachRepairOnlyData"); }
        }
        private PriceListData _PriceList;
        public InvDiagramsSearchData Diagram
        {
            get { return diagram; }
            set { diagram = value; OnPropertyChanged("Diagram"); }
        }

        public PriceListData PriceList
        {
            get { return _PriceList; }
            set { _PriceList = value; OnPropertyChanged("PriceList"); }
        }
        public ObservableCollection<ItemData> Items
        {
            get { return items; }
            set { items = value; OnPropertyChanged("Items"); }
        }
        public ObservableCollection<ItemProcessData> ItemProcesses
        {
            get { return itemprocesses; }
            set { itemprocesses = value; OnPropertyChanged("ItemProcess"); }
        }
        public ForecastInfoData Forecast
        {
            get { return forecast; }
            set { forecast = value;
                ChooseMachRep();
                INotifyPropertyChanged("Forecast"); }
        }

        public ObservableCollection<ForecastInfoData> ForecastDatagrid
        {
            get { return forecastdatagrid; }
            set { forecastdatagrid = value; OnPropertyChanged("ForecastDatagrid"); }
        }

        public ObservableCollection<MachineData> Machines
        {
            get { return machines; }
            set { machines = value; OnPropertyChanged("Machines"); }
        }

        public ObservableCollection<MachineData> PrimaryMachines
        {
            get { return primarymachines; }
            set { primarymachines = value; OnPropertyChanged("PrimaryMachines"); }
        }


        public string MPSCode
        {
            get { return mpscode; }
            set { mpscode = value; OnPropertyChanged("MPSCode"); }
        }

        public string MPSDescr
        {
            get { return mpsdescr; }
            set { mpsdescr = value; OnPropertyChanged("MPSDescr"); }
        }

        public double HoldingCost
        {
            get { return holdingcost; }
            set { holdingcost = value; OnPropertyChanged("HoldingCost"); }
        }

        public int MaxInventory
        {
            get { return maxinventory; }
            set { maxinventory = value; OnPropertyChanged("MaxInventory"); }
        }

        public int InvStoreTarget
        {
            get { return invstoretarget; }
            set { invstoretarget = value; OnPropertyChanged("InvStoreTarget"); }
        }
        public int HoursPerMonth
        {
            get { return hourspermonth; }
            set { hourspermonth = value; OnPropertyChanged("HoursPerMonth"); }
        }


        #region MachineMaintenance
        private bool _NumberDatesOfRepairs;
        private bool numberofrepairsonly;
        private bool _ExistingSchedule;


        public bool NumberDatesOfRepairs
        {
            get { return _NumberDatesOfRepairs; }
            set { _NumberDatesOfRepairs = value; OnPropertyChanged("NumberDatesOfRepairs"); }
        }

        public bool NumberOfRepairsOnly
        {
            get { return numberofrepairsonly; }
            set { numberofrepairsonly = value; OnPropertyChanged("NumberOfRepairsOnly"); }
        }

        public bool ExistingSchedule
        {
            get { return _ExistingSchedule; }
            set { _ExistingSchedule = value; OnPropertyChanged("ExistingSchedule"); }
        }
        #endregion


        private bool _ItemsDefaultSettings;

        public bool ItemsDefaultSettings
        {
            get { return _ItemsDefaultSettings; }
            set { _ItemsDefaultSettings = value; OnPropertyChanged("ItemsDefaultSettings"); }
        }

        public Dictionary<string, double> MaintenanceRate { get; set; } //<ItemCode, Demand * Itemdata.Profit>
        public Dictionary<string, double> ProfitDict { get; set; } //<ItemCode, Demand * Itemdata.Profit>
        public Dictionary<string, double> HoldingCostDict { get; set; } //<ItemCode, Demand * Itemdata.Profit>
        public Dictionary<string, double> StoreTargetDict { get; set; } //<ItemCode, Demand * Itemdata.Profit>
        public Dictionary<string, double> MaxInventoryDict { get; set; } //<ItemCode, Demand * Itemdata.Profit>

        public Dictionary<string, Dictionary<string, double>> TimeReq { get; set; }//MachineCode,<<ItemCode ,ItemProcessTime >>
        public Dictionary<string, int> MachInstalled { get; set; } //Number of Machines (tha ftiaksw diaforetikes mixanes kai tha pe3w me to enum me to machinetype 
        public Dictionary<(string, string), int> AdvancedMachDownReq { get; set; } //Number of Machines that need to be under maintenance

        public Dictionary<(string, string), double> TotalOnlineMach { get; set; } //Number of Machines that need to be under maintenance

        public Dictionary<(string, string), double> MaxDemand { get; set; } //Number of Machines that need to be under maintenance

        public Dictionary<(string, string), int> MachDownReq { get; set; } //Number of Machines that need to be under maintenance
        public Dictionary<string, int> MachDownReq2 { get; set; } //Number of Machines (tha ftiaksw diaforetikes mixanes kai tha pe3w me to enum me to machinetype 



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

