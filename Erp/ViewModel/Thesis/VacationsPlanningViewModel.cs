using Erp.CommonFiles;
using Erp.Helper;
using Erp.Model.BasicFiles;
using Erp.Model.Suppliers;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using LiveCharts.Wpf;
using Syncfusion.Data.Extensions;
using Erp.Model.Manufacture.MPS;
using Erp.Model.Manufacture;
using Erp.Model.Data_Analytics.Forecast;
using static Erp.Model.Enums.BasicEnums;
using Erp.Model.Enums;
using Syncfusion.Windows.Controls;
using Erp.Model.Customers;
using Erp.Model.Inventory;
using LiveCharts.Defaults;
using LiveCharts;
using Erp.ViewModel.Inventory;
using System.Windows.Data;
using Gurobi;
using Erp.Model.Thesis.VacationPlanning;
using Erp.Model.Thesis;
using System.Globalization;
using System.Runtime.InteropServices.ComTypes;

namespace Erp.ViewModel.Thesis
{
    public class VacationsPlanningViewModel : ViewModelBase
    {


        #region DataProperties

        private int _selectedTabIndex;

        public int SelectedTabIndex
        {
            get { return _selectedTabIndex; }
            set
            {
                if (_selectedTabIndex != value)
                {
                    _selectedTabIndex = value;
                    INotifyPropertyChanged(nameof(SelectedTabIndex));
                }
            }
        }

        private ICollectionView collectionviewD;

        public ICollectionView CollectionViewD
        {
            get
            {
                return collectionviewD;
            }
            set
            {
                collectionviewD = value;
                INotifyPropertyChanged("CollectionViewD");
            }
        }
        private ICollectionView collectionViewRepair;

        public ICollectionView CollectionViewRepair
        {
            get
            {
                return collectionViewRepair;
            }
            set
            {
                collectionViewRepair = value;
                INotifyPropertyChanged("CollectionViewRepair");
            }
        }
        private ObservableCollection<MPSOptResultsData> diagramdata;
        public ObservableCollection<MPSOptResultsData> DiagramData
        {
            get { return diagramdata; }
            set
            {
                diagramdata = value;
                INotifyPropertyChanged(nameof(DiagramData));
            }
        }

        private VacationPlanningInputData inputdata;
        public VacationPlanningInputData InputData
        {
            get { return inputdata; }
            set
            {
                inputdata = value;
                INotifyPropertyChanged(nameof(InputData));


            }
        }


        private VacationPlanningOutputData outputdata;
        public VacationPlanningOutputData OutputData
        {
            get { return outputdata; }
            set
            {
                outputdata = value;
                INotifyPropertyChanged(nameof(OutputData));


            }
        }

        private Columns sfGridColumns;
        public Columns SfGridColumns
        {
            get { return sfGridColumns; }
            set
            {
                this.sfGridColumns = value;
                INotifyPropertyChanged("SfGridColumns");
            }
        }
        private Columns sfGridColumnsd;
        public Columns SfGridColumnsD
        {
            get { return sfGridColumnsd; }
            set
            {
                this.sfGridColumnsd = value;
                INotifyPropertyChanged("SfGridColumnsD");
            }
        }
        private Columns sfGridColumnsRepair;
        public Columns SfGridColumnsRepair
        {
            get { return sfGridColumnsRepair; }
            set
            {
                this.sfGridColumnsRepair = value;
                INotifyPropertyChanged("SfGridColumnsRepair");
            }
        }

        #region Enums

        public BasicEnums.EmployeeType[] EmployeeTypes
        {
            get { return (BasicEnums.EmployeeType[])Enum.GetValues(typeof(BasicEnums.EmployeeType)); }
        }

        public BasicEnums.VPLogicType[] VPLogicTypes
        {
            get { return (BasicEnums.VPLogicType[])Enum.GetValues(typeof(BasicEnums.VPLogicType)); }
        }

        #endregion

        #endregion
        public VacationsPlanningViewModel()
        {


            InputData = new VacationPlanningInputData();
            InputData.VPCode = "";
            InputData.Schedule = new ReqScheduleInfoData();
            InputData.Schedule.ReqScheduleRowsData = new ObservableCollection<ReqScheduleRowsData>();

            OutputData = new VacationPlanningOutputData();
            OutputData.VPYijResultsDataGrid = new ObservableCollection<VPYijResultsData>();
            OutputData.VPXijResultsDataGrid = new ObservableCollection<VPXijResultsData>();
            OutputData.EmpLeaveStatusData = new ObservableCollection<EmployeeData>();


            this.sfGridColumns = new Columns();
            this.SfGridColumnsRepair = new Columns();

            CalculateVacationPlanning = new RelayCommand2(ExecuteCalculateVacationPlanning);

            ClearItemsInputCommand = new RelayCommand2(ExecuteClearItemsInputCommand);
            RefreshItemsInputCommand = new RelayCommand2(ExecuteRefreshItemsInputCommand);
            InsertDataItemsInputCommand = new RelayCommand2(ExecuteInsertDataItemsInputCommand);



            rowDataCommand = new RelayCommand2(ChangeCanExecute);
            ShowVPCommand = new RelayCommand2(ExecuteShowVPCommand);
            AddVPCommand = new RelayCommand2(ExecuteAddVPCommand);
            ShowScheduleGridCommand = new RelayCommand2(ExecuteShowScheduleGridCommand);

        }
        #region CRUD  Commands

        #region Input 
        #region ADD VacationPlanning
        public ICommand AddVPCommand { get; }

        private void ExecuteAddVPCommand(object obj)
        {
            if (string.IsNullOrWhiteSpace(InputData.VPCode) || string.IsNullOrWhiteSpace(InputData.VPDescr))
            {
                MessageBox.Show("Insert Code and Description");
            }

            else
            {
                int Flag = CommonFunctions.AddVPInputData(InputData);
                if (Flag == 0)
                {
                    MessageBox.Show($"New Vacation Planning added with Code: {InputData.VPCode}");
                    ExecuteShowVPCommand(obj);
                    InputData.VPId = 0;
                    ExecuteRefreshInputCommand(obj);
                }
                else if (Flag == 1)
                {
                    MessageBox.Show($"The Vacation Planning with Code : {InputData.VPCode} already exists");

                }
                else if (Flag == 2)
                {
                    MessageBox.Show("Σφάλμα κατά την επεξεργασία δεδομένων", "", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }
        #endregion

        #region Clear

        private ViewModelCommand _ClearInputCommand;

        public ICommand ClearInputCommand
        {
            get
            {
                if (_ClearInputCommand == null)
                {
                    _ClearInputCommand = new ViewModelCommand(ExecuteClearInputCommand);
                }

                return _ClearInputCommand;
            }
        }

        private void ExecuteClearInputCommand(object commandParameter)
        {



        }

        #endregion

        #region SaveVP


        private ViewModelCommand _SaveInputCommand;

        public ICommand SaveInputCommand
        {
            get
            {
                if (_SaveInputCommand == null)
                {
                    _SaveInputCommand = new ViewModelCommand(ExecuteSaveInputCommand);
                }

                return _SaveInputCommand;
            }
        }

        private void ExecuteSaveInputCommand(object obj)
        {
            int Flag = CommonFunctions.SaveVPInputData(InputData);

            if (Flag == 1)
            {
                MessageBox.Show($"Save/Update Completed for Vacation Planning with Code : {InputData.VPCode}");
                ExecuteShowVPCommand(obj);
                ExecuteRefreshInputCommand(obj);
            }
            else if (Flag == -1)
            {
                MessageBox.Show("Error during data processing", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Refresh

        private ViewModelCommand _RefreshInputCommand;

        public ICommand RefreshInputCommand
        {
            get
            {
                if (_RefreshInputCommand == null)
                {
                    _RefreshInputCommand = new ViewModelCommand(ExecuteRefreshInputCommand);
                }

                return _RefreshInputCommand;
            }
        }

        private void ExecuteRefreshInputCommand(object commandParameter)
        {

            InputData = CommonFunctions.GetVPInputChooserData(InputData.VPId, InputData.VPCode, InputData);

        }

        #endregion
        #endregion

        #region Output 



        #endregion

        #endregion

        #region Commands

        #region FlatData
        public ICommand ShowVPCommand { get; }
        public ICommand ShowScheduleGridCommand { get; }

        public void ExecuteShowVPCommand(object obj)
        {


            ClearColumns();

            var F7input = F7Common.F7VacationPlanning(ShowDeleted);
            F7key = F7input.F7key;
            CollectionView = F7input.CollectionView;
            var a = F7input.SfGridColumns;
            foreach (var item in a)
            {
                this.sfGridColumns.Add(item);
            }

        }
        private void ExecuteShowScheduleGridCommand(object obj)
        {


            ClearColumns();

            var F7input = F7Common.F7ReqSchedule(false);
            F7key = F7input.F7key;
            CollectionView = F7input.CollectionView;
            var a = F7input.SfGridColumns;
            foreach (var item in a)
            {
                this.sfGridColumns.Add(item);
            }

        }
        public void ChangeCanExecute(object obj)
        {
            if (F7key == "VPCode")
            {
                InputData.VPId = (SelectedItem as VacationPlanningInputData).VPId;
                InputData.VPCode = (SelectedItem as VacationPlanningInputData).VPCode;
                InputData.VPDescr = (SelectedItem as VacationPlanningInputData).VPDescr;
                InputData.EmployeeType = (SelectedItem as VacationPlanningInputData).EmployeeType;
                InputData.MaxSatisfiedBids = (SelectedItem as VacationPlanningInputData).MaxSatisfiedBids;
                InputData.SeparValue = (SelectedItem as VacationPlanningInputData).SeparValue;

                InputData.VPLogicType = (SelectedItem as VacationPlanningInputData).VPLogicType;
                InputData.IsDeleted = (SelectedItem as VacationPlanningInputData).IsDeleted;


                #region Schedule

                InputData.Schedule = (SelectedItem as VacationPlanningInputData).Schedule;
                InputData.Schedule.ReqScheduleRowsData = CommonFunctions.GetReqSchedulesRows(InputData.Schedule.ReqCode);

                #region Dates,DatesStr List  

                InputData.Dates = InputData.Schedule.ReqScheduleRowsData.Select(df => df.Date).OrderBy(date => date).ToArray();
                InputData.DatesStr = InputData.Schedule.ReqScheduleRowsData.Select(df => df.DateStr).OrderBy(dateStr => DateTime.ParseExact(dateStr, "dd/MM/yyyy", CultureInfo.InvariantCulture)).ToArray();

                #endregion

                #endregion






            }
            if (F7key == "ReqSchedule")
            {
                InputData.Schedule = (SelectedItem as ReqScheduleInfoData);
                InputData.Schedule.ReqScheduleRowsData = CommonFunctions.GetReqSchedulesRows(InputData.Schedule.ReqCode);

                #region Dates, DatesStr List  

                InputData.Dates = InputData.Schedule.ReqScheduleRowsData.Select(df => df.Date).OrderBy(date => date).ToArray();
                InputData.DatesStr = InputData.Schedule.ReqScheduleRowsData.Select(df => df.DateStr).OrderBy(dateStr => DateTime.ParseExact(dateStr, "dd/MM/yyyy", CultureInfo.InvariantCulture)).ToArray();

                #endregion
            }

        }

        #endregion

        #region Insert ItemData/PriceListData
        public ICommand ClearItemsInputCommand { get; }
        public ICommand RefreshItemsInputCommand { get; }
        public ICommand InsertDataItemsInputCommand { get; }

        private void ExecuteInsertDataItemsInputCommand(object obj)
        {
            //if (InputData.ItemsDefaultSettings == true)
            //{
            //    foreach (var item in InputData.Items)
            //    {
            //        item.StoreTarget = InputData.InvStoreTarget;
            //        item.MaxInventory = InputData.MaxInventory;
            //        item.HoldingCost = (float)InputData.HoldingCost;
            //    }
            //}
            //#region Profit Dict 

            //foreach (var itemInfo in InputData.PriceList.ItemsInfo)
            //{
            //    var matchingItem = InputData.Items.FirstOrDefault(item => item.ItemCode == itemInfo.Item.ItemCode);
            //    if (matchingItem != null)
            //    {
            //        matchingItem.SalesPrice = itemInfo.SalesPrice;
            //    }


            //}

            //#endregion




        }




        private void ExecuteClearItemsInputCommand(object commandParameter)
        {
            //ChooserData.ItemCode = "";
            //ChooserData.ItemDescr = "";
            //ChooserData.ItemId = -1;

            //ChooserData = new ItemData();

            //FlatData.ItemCode = "";
            //FlatData.ItemDescr = "";
            //FlatData.ItemId = 0;

            //FlatData.MesUnit = "";
            //FlatData.ItemType = "";
            //FlatData.Assembly = 99;
            //FlatData.CanBeProduced = false;
            //FlatData.InputOrderFlag = false;
            //FlatData.OutputOrderFlag = false;
            //FlatData2 = new ObservableCollection<BomData>();
            //ProcessData = new ObservableCollection<ItemProcessData>();

        }







        private void ExecuteRefreshItemsInputCommand(object commandParameter)
        {

            //FlatData = CommonFunctions.GetItemData(ChooserData);


        }


        #endregion


        #region Calculate VacationPlanning
        public ICommand CalculateVacationPlanning { get; }


        private void ExecuteCalculateVacationPlanning(object obj)
        {
            InputData.MaxLeaveBidsPerEmployee = new Dictionary<string, int>();
            InputData.BidsPerEmployee = new Dictionary<string, string>();

            InputData.ZBidsDict = new Dictionary<(string, string,int), int>();
            InputData.RBidsDict = new Dictionary<(string, string), int>();

            InputData.ZBidsDictInt = new Dictionary<(int, int, int), int>();
            InputData.RBidsDictInt = new Dictionary<(int, int), int>();


            #region Employee Insert Data




            InputData.Employees = CommonFunctions.GetEmployeesByTypeData(InputData,false);
            InputData.MaxLeaveBids = 0;
            int LeaveBidRowsCount = 0;
            foreach (var emp in InputData.Employees)
            {
                #region LeaveStatus
                emp.LeaveStatus = new LeaveStatusData();
                emp.LeaveStatus = CommonFunctions.GetLeaveStatusChooserData(emp.EmployeeId, emp.Code);

                #endregion

                #region LeaveBids
                emp.LeaveBidDataGridStatic = new ObservableCollection<LeaveBidsDataStatic>();
                emp.LeaveBidDataGridStatic = CommonFunctions.GetLeaveBids(emp.Code, InputData.Schedule.ReqCode);

                LeaveBidRowsCount = emp.LeaveBidDataGridStatic.Count();

                InputData.MaxLeaveBidsPerEmployee[emp.Code] = LeaveBidRowsCount;
                if (LeaveBidRowsCount > InputData.MaxLeaveBids)
                {
                    InputData.MaxLeaveBids = LeaveBidRowsCount;
                }
                #endregion

                #region Zbids Dict ,Rbids Dict
                foreach (var Bid in emp.LeaveBidDataGridStatic)
                {

                    InputData.BidsPerEmployee[emp.Code] = Bid.BidCode; //Fill BidsPerEmployee Dictionary

                    if (Bid.BidType == BasicEnums.BidType.Specific)
                    {
                        InputData.RBidsDict[(emp.Code, Bid.BidCode)] = 1;
                        InputData.ZBidsDict[(emp.Code, Bid.BidCode,1)] = 1;

                        InputData.RBidsDictInt[(emp.Seniority, Bid.PriorityLevel)] = 1;
                        InputData.ZBidsDictInt[(emp.Seniority, Bid.PriorityLevel, 1)] = 1;
                    }
                    else if (Bid.BidType == BasicEnums.BidType.Non_Specific)
                    {

                        // Calculate the number of combinations
                        TimeSpan range = Bid.DateTo - Bid.DateFrom; 
                        int totalDaysInRange = (int)range.TotalDays;

                        InputData.RBidsDict[(emp.Code, Bid.BidCode)] = 1;

                        int Z = totalDaysInRange - Bid.NumberOfDays + 2;

                        InputData.ZBidsDict[(emp.Code, Bid.BidCode,1)] = Z;

                        InputData.RBidsDictInt[(emp.Seniority, Bid.PriorityLevel)] = 1;
                        InputData.ZBidsDictInt[(emp.Seniority, Bid.PriorityLevel, 1)] = Z;

                    }
                    else if (Bid.BidType == BasicEnums.BidType.Min_Max)
                    {
                        TimeSpan range = Bid.DateTo - Bid.DateFrom;
                        int totalDaysInRange = (int)range.TotalDays;

                        var Min = Bid.NumberOfDaysMin;
                        var Max = Bid.NumberOfDaysMax;

                        InputData.RBidsDict[(emp.Code, Bid.BidCode)] = Max - Min + 1;
                        InputData.RBidsDictInt[(emp.Seniority, Bid.PriorityLevel)] = Max - Min + 1;

                        for (int i = 0; i < Max - Min + 1; i++)
                        {
                            Bid.NumberOfDays = Max - i;
                            int Z = totalDaysInRange - Bid.NumberOfDays + 2;

                            InputData.ZBidsDict[(emp.Code, Bid.BidCode, i+1)] = Z;
                            InputData.ZBidsDictInt[(emp.Seniority, Bid.PriorityLevel, 1)] = Z;

                        };

                    }

                }
                #endregion 

            }
            #endregion


            OutputData = CommonFunctions.CalculateVacationPlanningAdvanced(InputData);
            OutputData.VPYijResultsDataGrid = OutputData.VPYijResultsDataGrid.OrderBy(item => item.Employee.Seniority).ToObservableCollection();
            OutputData.VPYijzResultsDataGrid = OutputData.VPYijzResultsDataGrid.OrderBy(item => item.Employee.Seniority).ToObservableCollection();
            OutputData.VPXijResultsDataGrid = OutputData.VPXijResultsDataGrid.OrderBy(item => item.Employee.Seniority).ToObservableCollection();

            var result = System.Windows.MessageBox.Show($"Do you want to Create a Notepad as Python Input?", "Confirmation", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                var a = CommonFunctions.CreatePythonTxt(InputData);

            }

            SelectedTabIndex = 1;

        }
        #endregion

        private ICommand rowDataCommand { get; set; }
        public ICommand RowDataCommand
        {
            get
            {
                return rowDataCommand;
            }
            set
            {
                rowDataCommand = value;
            }
        }


        protected void ClearColumns()
        {

            var ColumnsCount = this.SfGridColumns.Count();
            if (ColumnsCount != 0)
            {
                for (int i = 0; i < ColumnsCount; i++)
                {
                    this.sfGridColumns.RemoveAt(0);
                }
            }
        }

        #endregion
    }
}
