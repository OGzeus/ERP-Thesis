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
using ILOG.CPLEX;
using ILOG.Concert;

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
        private VPCGInputData cginputdata;
        public VPCGInputData CGInputdata
        {
            get { return cginputdata; }
            set
            {
                cginputdata = value;
                INotifyPropertyChanged(nameof(CGInputdata));


            }
        }
        private VPCGOutputData cgoutputdata;
        public VPCGOutputData CGOutputdata
        {
            get { return cgoutputdata; }
            set
            {
                cgoutputdata = value;
                INotifyPropertyChanged(nameof(CGOutputdata));


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
            OutputData.VPXiResultsDataGrid = new ObservableCollection<VPXiResultData>();

            OutputData.EmpLeaveStatusData = new ObservableCollection<EmployeeData>();
            CGInputdata = new VPCGInputData();
            CGOutputdata = new VPCGOutputData();

            CGInputdata.VPXiResultsDataGrid = new ObservableCollection<VPXiResultData>();

            this.sfGridColumns = new Columns();
            this.SfGridColumnsRepair = new Columns();

            CalculateVacationPlanningGB = new RelayCommand2(ExecuteCalculateVacationPlanning_Gurobi);
            CalculateVacationPlanningCP = new RelayCommand2(ExecuteCalculateVacationPlanning_Cplex);





            rowDataCommand = new RelayCommand2(ChangeCanExecute);
            ShowVPCommand = new RelayCommand2(ExecuteShowVPCommand);
            AddVPCommand = new RelayCommand2(ExecuteAddVPCommand);
            ShowScheduleGridCommand = new RelayCommand2(ExecuteShowScheduleGridCommand);

        }

        #region CRUD  Commands

        #region ADD 
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

        #region Save


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


        #region F7Commands

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
                InputData.Schedule.ReqScheduleRowsData = CommonFunctions.GetReqSchedulesRowsByEmpType(InputData.Schedule.ReqCode,InputData.EmployeeType);

                #region Dates,DatesStr List  

                InputData.Dates = InputData.Schedule.ReqScheduleRowsData.Select(df => df.Date).OrderBy(date => date).ToArray();
                InputData.DatesStr = InputData.Schedule.ReqScheduleRowsData.Select(df => df.DateStr).OrderBy(dateStr => DateTime.ParseExact(dateStr, "dd/MM/yyyy", CultureInfo.InvariantCulture)).ToArray();

                #endregion

                #endregion

            }
            if (F7key == "ReqSchedule")
            {
                InputData.Schedule = (SelectedItem as ReqScheduleInfoData);
                InputData.Schedule.ReqScheduleRowsData = CommonFunctions.GetReqSchedulesRowsByEmpType(InputData.Schedule.ReqCode, InputData.EmployeeType);

                #region Dates, DatesStr List  

                InputData.Dates = InputData.Schedule.ReqScheduleRowsData.Select(df => df.Date).OrderBy(date => date).ToArray();
                InputData.DatesStr = InputData.Schedule.ReqScheduleRowsData.Select(df => df.DateStr).OrderBy(dateStr => DateTime.ParseExact(dateStr, "dd/MM/yyyy", CultureInfo.InvariantCulture)).ToArray();

                #endregion
            }
        }

        #endregion


        #region Calculate VacationPlanning
        public ICommand CalculateVacationPlanningGB { get; }
        public ICommand CalculateVacationPlanningCP { get; }

        private void ExecuteCalculateVacationPlanning_Gurobi(object obj)
        {
            #region Vacation Planning

            #region  Dictionaries

            InputData.MaxLeaveBidsPerEmployee = new Dictionary<string, int>();

            InputData.ZBidsDict = new Dictionary<(string, string, int), int>();
            InputData.RBidsDict = new Dictionary<(string, string), int>();


            #endregion

            #region Populate Dictionaries

            InputData.Employees = CommonFunctions.GetEmployeesByTypeData(InputData.EmployeeType, false);
            InputData.MaxLeaveBids = 0;
            int LeaveBidRowsCount = 0;
            int EmpCount = 0;
            foreach (var emp in InputData.Employees)
            {
                EmpCount++;
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

                #region Zbids ,Rbids Dictionaries
                foreach (var Bid in emp.LeaveBidDataGridStatic)
                {


                    if (Bid.BidType == BasicEnums.BidType.Specific)
                    {
                        InputData.RBidsDict[(emp.Code, Bid.BidCode)] = 1;
                        InputData.ZBidsDict[(emp.Code, Bid.BidCode, 1)] = 1;

                    }
                    else if (Bid.BidType == BasicEnums.BidType.Non_Specific)
                    {

                        // Calculate the number of combinations
                        TimeSpan range = Bid.DateTo - Bid.DateFrom;
                        int totalDaysInRange = (int)range.TotalDays;

                        InputData.RBidsDict[(emp.Code, Bid.BidCode)] = 1;

                        int Z = totalDaysInRange - Bid.NumberOfDays + 2;
                        InputData.ZBidsDict[(emp.Code, Bid.BidCode, 1)] = Z;


                    }
                    else if (Bid.BidType == BasicEnums.BidType.Min_Max)
                    {
                        TimeSpan range = Bid.DateTo - Bid.DateFrom;
                        int totalDaysInRange = (int)range.TotalDays;

                        var Min = Bid.NumberOfDaysMin;
                        var Max = Bid.NumberOfDaysMax;

                        InputData.RBidsDict[(emp.Code, Bid.BidCode)] = Max - Min + 1;

                        for (int i = 0; i < Max - Min + 1; i++)
                        {
                            Bid.NumberOfDays = Max - i;
                            int Z = totalDaysInRange - Bid.NumberOfDays + 2;

                            InputData.ZBidsDict[(emp.Code, Bid.BidCode, i + 1)] = Z;

                        };

                    }

                }
                #endregion


            }
            #endregion

            #region Call Function For Optimization
            OutputData = CommonFunctions.Calculate_VacationPlanning_Gurobi_String(InputData);

            OutputData.VPYijResultsDataGrid = OutputData.VPYijResultsDataGrid.OrderBy(item => item.Employee.Seniority).ToObservableCollection();
            OutputData.VPYijzResultsDataGrid = OutputData.VPYijzResultsDataGrid.OrderBy(item => item.Employee.Seniority).ToObservableCollection();
            OutputData.VPXijResultsDataGrid = OutputData.VPXijResultsDataGrid.OrderBy(item => item.Employee.Seniority).ToObservableCollection();


            #endregion

            #endregion

            #region Column Generation

            #region Dictionaries
            CGInputdata.LeaveDays = new Dictionary<int, int>();
            CGInputdata.LLiDict = new Dictionary<int, int>();
            #endregion

            #region Populate Dictionaries

            var XijList = OutputData.VPXijResultsDataGrid;

            var groupedByDate = XijList
                .GroupBy(x => x.Date) // Group by Date
                .Select(g => new VPXiResultData
                {
                    Date = g.Key, // Date
                    LimitLine = g.Sum(x => (int)x.XijFlag)
                });

            var t = 0;
            foreach (var row in groupedByDate)
            {
                var PreviousLimitLine = InputData.Schedule.ReqScheduleRowsData.ElementAt(t).LimitLine;

                row.LimitLine = PreviousLimitLine - row.LimitLine;
                row.LLi = $"LL{t + 1}";

                OutputData.VPXiResultsDataGrid.Add(row);
                CGInputdata.LLiDict[t + 1] = row.LimitLine;

                t++;
            }

            CGInputdata.Dates = InputData.DatesStr;

            t = 0;
            foreach (var emp in OutputData.EmpLeaveStatusData)
            {

                CGInputdata.LeaveDays[t + 1] = emp.LeaveStatus.ProjectedBalance;
                t++;

            }

            #endregion

            #region Call Function For Optimization
            CGOutputdata = new VPCGOutputData();
            CGOutputdata = CommonFunctions.CalculateVPColumnGeneration(CGInputdata);

            #endregion

            #endregion

            SelectedTabIndex = 1;

        }

        private void ExecuteCalculateVacationPlanning_Cplex(object obj)
        {
            #region Vacation Planning

            #region  Dictionaries

            InputData.LLi_Dict = new Dictionary<int, int>();
            InputData.MaxD_Dict = new Dictionary<int, int>();
            InputData.N_Dict = new Dictionary<int, int>();
            InputData.NDays_Dict = new Dictionary<(int, int), int>();
            InputData.DateFrom_Dict = new Dictionary<(int, int), int>();
            InputData.DateTo_Dict = new Dictionary<(int, int), int>();

            InputData.RBids_Dict = new Dictionary<(int, int), int>();
            InputData.ZBids_Dict = new Dictionary<(int, int, int), int>();


            #region Extra
            InputData.MaxLeaveBidsPerEmployee = new Dictionary<string, int>();
            InputData.ZBidsDict = new Dictionary<(string, string, int), int>();
            InputData.RBidsDict = new Dictionary<(string, string), int>();
            #endregion

            #endregion


            InputData.Employees = CommonFunctions.GetEmployeesByTypeData(InputData.EmployeeType, false);

            InputData.MaxLeaveBids = 0;
            int LeaveBidRowsCount = 0;
            int EmpCount = 0;

            #region Populate Dictionaries

            foreach (var emp in InputData.Employees)
            {
                #region LeaveStatus ,MaxD_Dict
                emp.LeaveStatus = new LeaveStatusData();
                emp.LeaveStatus = CommonFunctions.GetLeaveStatusChooserData(emp.EmployeeId, emp.Code);
                var MaxLeaveDays = emp.LeaveStatus.CurrentBalance;
                InputData.MaxD_Dict[EmpCount] = MaxLeaveDays;
                #endregion

                #region LeaveBids,N_Dict

                emp.LeaveBidDataGridStatic = new ObservableCollection<LeaveBidsDataStatic>();
                emp.LeaveBidDataGridStatic = CommonFunctions.GetLeaveBids(emp.Code, InputData.Schedule.ReqCode);
                foreach(var LeaveBid in emp.LeaveBidDataGridStatic)
                {
                    if(LeaveBid.BidType == BasicEnums.BidType.Specific || LeaveBid.BidType == BasicEnums.BidType.Non_Specific)
                    {
                        LeaveBid.NumberOfDaysMax = LeaveBid.NumberOfDays;
                    }
                }

                LeaveBidRowsCount = emp.LeaveBidDataGridStatic.Count();

                InputData.N_Dict[EmpCount] = LeaveBidRowsCount; 
                if (LeaveBidRowsCount > InputData.MaxLeaveBids)
                {
                    InputData.MaxLeaveBids = LeaveBidRowsCount;
                }

                #endregion

                #region Zbids, Rbids, NDays,DateFrom,DateTo Dictionaries

                var DatesArray = InputData.Dates;

                foreach (var Bid in emp.LeaveBidDataGridStatic)
                {


                    #region DateFrom,DateTo,NDays Dictionaries 

                    int DateFrom_Index = Array.IndexOf(DatesArray, Bid.DateFrom);
                    int DateTo_Index = Array.IndexOf(DatesArray, Bid.DateTo);

                    InputData.DateFrom_Dict[(emp.Seniority - 1, Bid.PriorityLevel - 1)] = DateFrom_Index;
                    InputData.DateTo_Dict[(emp.Seniority - 1, Bid.PriorityLevel - 1)] = DateTo_Index;

                    InputData.NDays_Dict[(emp.Seniority - 1, Bid.PriorityLevel - 1)] = Bid.NumberOfDaysMax;

                    #endregion

                    #region ZBids,Rbids

                    if (Bid.BidType == BasicEnums.BidType.Specific)
                    {
                        InputData.RBids_Dict[(emp.Seniority - 1, Bid.PriorityLevel - 1)] = 1;
                        InputData.ZBids_Dict[(emp.Seniority - 1, Bid.PriorityLevel - 1, 0)] = 1;

                        #region Extra
                        InputData.RBidsDict[(emp.Code, Bid.BidCode)] = 1;
                        InputData.ZBidsDict[(emp.Code, Bid.BidCode, 1)] = 1;
                        #endregion
                    }
                    else if (Bid.BidType == BasicEnums.BidType.Non_Specific)
                    {

                        // Calculate the number of combinations
                        TimeSpan range = Bid.DateTo - Bid.DateFrom;
                        int totalDaysInRange = (int)range.TotalDays;

                        int Z = totalDaysInRange - Bid.NumberOfDays + 2;

                        InputData.RBids_Dict[(emp.Seniority - 1, Bid.PriorityLevel - 1)] = 1;
                        InputData.ZBids_Dict[(emp.Seniority - 1, Bid.PriorityLevel - 1, 0)] = Z;

                        #region Extra

                        InputData.RBidsDict[(emp.Code, Bid.BidCode)] = 1;
                        InputData.ZBidsDict[(emp.Code, Bid.BidCode, 1)] = Z;

                        #endregion

                    }
                    else if (Bid.BidType == BasicEnums.BidType.Min_Max)
                    {

                        TimeSpan range = Bid.DateTo - Bid.DateFrom;
                        int totalDaysInRange = (int)range.TotalDays;

                        var Min = Bid.NumberOfDaysMin;
                        var Max = Bid.NumberOfDaysMax;

                        InputData.RBids_Dict[(emp.Seniority - 1, Bid.PriorityLevel - 1)] = Max - Min + 1;



                        #region Extra

                        InputData.RBidsDict[(emp.Code, Bid.BidCode)] = Max - Min + 1;

                        #endregion

                        for (int i = 0; i < Max - Min + 1; i++)
                        {
                            Bid.NumberOfDays = Max - i;
                            int Z = totalDaysInRange - Bid.NumberOfDays + 2;

                            InputData.ZBids_Dict[(emp.Seniority - 1, Bid.PriorityLevel - 1, i)] = Z;

                            #region Extra

                            InputData.ZBidsDict[(emp.Code, Bid.BidCode, i + 1)] = Z;

                            #endregion
                        };

                    }
                    #endregion
                }
                #endregion

                EmpCount++;
            }

            #region LLi Dictionary

            int DateCount = 0;
            var ScheduleDates = InputData.Schedule.ReqScheduleRowsData;
            foreach (var Date in ScheduleDates)
            {
                InputData.LLi_Dict[DateCount] = Date.LimitLine;
                DateCount++;
            }
            #endregion



            #endregion


            #region Call Function For Optimization
            OutputData = CplexFunctions.CalculateVacationPlanning_CPLEX(InputData);

            OutputData.VPYijResultsDataGrid = OutputData.VPYijResultsDataGrid.OrderBy(item => item.Employee.Seniority).ToObservableCollection();
            OutputData.VPYijzResultsDataGrid = OutputData.VPYijzResultsDataGrid.OrderBy(item => item.Employee.Seniority).ToObservableCollection();
            OutputData.VPXijResultsDataGrid = OutputData.VPXijResultsDataGrid.OrderBy(item => item.Employee.Seniority).ToObservableCollection();


            #endregion

            #endregion



            #region Column Generation

            #region Dictionaries
            CGInputdata.LeaveDays = new Dictionary<int, int>();
            CGInputdata.LLiDict = new Dictionary<int, int>();
            #endregion

            #region Populate Dictionaries

            var XijList = OutputData.VPXijResultsDataGrid;

            var groupedByDate = XijList
                .GroupBy(x => x.Date) // Group by Date
                .Select(g => new VPXiResultData
                {
                    Date = g.Key, // Date
                    LimitLine = g.Sum(x => (int)x.XijFlag)
                });

            var t = 0;
            foreach (var row in groupedByDate)
            {
                var PreviousLimitLine = InputData.Schedule.ReqScheduleRowsData.ElementAt(t).LimitLine;

                row.LimitLine = PreviousLimitLine - row.LimitLine;
                row.LLi = $"LL{t + 1}";

                OutputData.VPXiResultsDataGrid.Add(row);
                CGInputdata.LLiDict[t + 1] = row.LimitLine;

                t++;
            }

            CGInputdata.Dates = InputData.DatesStr;

            t = 0;
            foreach (var emp in OutputData.EmpLeaveStatusData)
            {

                CGInputdata.LeaveDays[t + 1] = emp.LeaveStatus.ProjectedBalance;
                t++;

            }

            #endregion

            #region Call Function For Optimization
            CGOutputdata = new VPCGOutputData();
            CGOutputdata = CplexFunctions.CalculateVPColumnGeneration_CPLEX(CGInputdata);

            #endregion

            #endregion

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

    }
}
