using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows;
using Syncfusion.Data.Extensions;
using Erp.Model.Enums;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Syncfusion.UI.Xaml.Grid;
using Erp.Helper;
using Erp.Model.Thesis.CrewScheduling;
using Erp.Model.Thesis;
using OxyPlot;


namespace Erp.ViewModel.Thesis
{
    public class CrewSchedulingViewModel : ViewModelBase
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

        private CSInputData inputdata;
        public CSInputData InputData
        {
            get { return inputdata; }
            set
            {
                inputdata = value;
                INotifyPropertyChanged(nameof(InputData));


            }
        }
        private CSOutputData _OutputData;
        public CSOutputData OutputData
        {
            get { return _OutputData; }
            set
            {
                _OutputData = value;
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

        #region Enums

        public BasicEnums.EmployeeType[] EmployeeTypes
        {
            get { return (BasicEnums.EmployeeType[])Enum.GetValues(typeof(BasicEnums.EmployeeType)); }
        }

        public BasicEnums.CSType[] CSTypes
        {
            get { return (BasicEnums.CSType[])Enum.GetValues(typeof(BasicEnums.CSType)); }
        }
        #endregion

        #endregion

        #region Commands

        #region Data_Grid Commands
        public ICommand ShowCrewSchedulingGridCommand { get; }
        public ICommand ShowEmployeesGridCommand { get; }
        public ICommand ShowFlightRoutestGridCommand { get; }
        public void ExecuteShowCrewSchedulingGridCommand(object obj)
        {


            ClearColumns();

            var F7input = F7Common.F7CrewScheduling(ShowDeleted);
            F7key = F7input.F7key;
            CollectionView = F7input.CollectionView;
            var a = F7input.SfGridColumns;
            foreach (var item in a)
            {
                this.sfGridColumns.Add(item);
            }

        }
        public void ExecuteShowFlightRoutestGridCommand(object obj)
        {

            ClearColumns();

            var F7input = F7Common.F7CSFlightRoutes(InputData);
            F7key = F7input.F7key;
            var Data = CommonFunctions.GetCSFlightRoutesData(false, InputData);

            CollectionView = CollectionViewSource.GetDefaultView(Data);
            var a = F7input.SfGridColumns;
            foreach (var item in a)
            {
                this.sfGridColumns.Add(item);
            }

        }
        public void ExecuteShowEmployeesGridCommand(object obj)
        {
            ClearColumns();

            var F7input = F7Common.F7CSEmployee(InputData);
            F7key = F7input.F7key;

            var Data = CommonFunctions.GetEmployeesByTypeData(InputData.Position, false);
            foreach(var row in Data)
            {
                row.IsSelected = true;
            }


            CollectionView = CollectionViewSource.GetDefaultView(Data);

            var a = F7input.SfGridColumns;
            foreach (var item in a)
            {
                this.sfGridColumns.Add(item);
            }

        }

        public void ChangeCanExecute(object obj)
        {

            if (F7key == "CSCODE")
            {
                InputData = (SelectedItem as CSInputData);

            }

        }
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

        #region CRUD  Commands

        #region Input 
        #region ADD
        private ViewModelCommand _AddCommand;

        public ICommand AddCommand
        {
            get
            {
                if (_AddCommand == null)
                {
                    _AddCommand = new ViewModelCommand(ExecuteAddCommand);
                }

                return _AddCommand;
            }
        }

        private void ExecuteAddCommand(object obj)
        {
            if (string.IsNullOrWhiteSpace(InputData.Code) || string.IsNullOrWhiteSpace(InputData.Descr))
            {
                MessageBox.Show("Insert Code and Description");
            }

            else
            {
                int Flag = CommonFunctions.AddCSInputData(InputData);
                if (Flag == 0)
                {
                    MessageBox.Show($"A New Crew Scheduling saved with Code: {InputData.Code}");
                    ExecuteShowCrewSchedulingGridCommand(obj);
                    InputData.Id = 0;
                    ExecuteRefreshInputCommand(obj);
                }
                else if (Flag == 1)
                {
                    MessageBox.Show($"The Crew Scheduling with Code : {InputData.Code} already exists");

                }
                else if (Flag == 2)
                {
                    MessageBox.Show("Error during data processing", "", MessageBoxButton.OK, MessageBoxImage.Error);
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

            InputData = new CSInputData();
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
            int Flag = CommonFunctions.SaveCSInputData(InputData);


            if (Flag == 1)
            {
                MessageBox.Show($"Save/Update Completed for Crew Scheduling with Code : {InputData.Code}");
                ExecuteShowCrewSchedulingGridCommand(obj);
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


            InputData = CommonFunctions.GetCSInputChooserData(InputData.Id, InputData.Code,InputData);
        }

        #endregion
        #endregion

        #region Output 



        #endregion

        #endregion

        #region Crew Scheduling Optimization

        #region Gurobi
        public ICommand CalculateCS_GB { get; }
        private void ExecuteCalculateCS_Gurobi(object obj)
        {
            //Na diksw autes tis Sunarthseis
            InputData.Employees = CommonFunctions.GetEmployeesByTypeData(InputData.Position, false);
            InputData.FlightRoutesData = CommonFunctions.GetCSFlightRoutesData(false, InputData);

            #region Dictionaires,Indexes Initialization

            InputData.T = new int(); // DATES
            InputData.I = new int(); // EMPLOYEES
            InputData.F = new int(); // ROUTES

            InputData.DatesIndexMap = new Dictionary<int, DateTime>();
            InputData.EmployeesIndexMap = new Dictionary<int, string>();
            InputData.RoutesIndexMap = new Dictionary<int, string>();
            InputData.RoutesCompl_Dict = new Dictionary<int, int>();

            InputData.RoutesDates_Dict = new Dictionary<int, (DateTime, DateTime)>();
            InputData.RoutesDay_Dict = new Dictionary<int, (int, int)>();
            InputData.RoutesTime_Dict = new Dictionary<int, (int, int)>();
            InputData.EmpBounds_Dict = new Dictionary<int, (double, double)>();

            InputData.Ri = new Dictionary<int, List<int>>(); 
            InputData.Cij_Hours = new Dictionary<(int, int), double>(); 
            InputData.Aijf = new Dictionary<(int, int, int), int>();
            #endregion

            #region Populate Dictionaries,Indexes

           

            InputData.F = InputData.FlightRoutesData.Count;
            InputData.I = InputData.Employees.Count;

            int EmployeeCounter = 0;
            foreach (var emp in InputData.Employees)
            {
                InputData.EmployeesIndexMap.Add(EmployeeCounter, emp.Code);
                InputData.EmpBounds_Dict.Add(EmployeeCounter, (emp.EmpCrSettings.LowerBound, emp.EmpCrSettings.UpperBound));

                #region Cij,Ri,Aijf Initialization 
                //Create a List with the Empty Roster
                List<int> RostersForEmployee_List = new List<int>();
                RostersForEmployee_List.Add(0);

                //Insert into Ri
                InputData.Ri.Add(EmployeeCounter, RostersForEmployee_List);

                //Add the Roster to Cij with Cost = 0
                InputData.Cij_Hours.Add((EmployeeCounter, 0), 0);
                #endregion
                EmployeeCounter++;

            }
            int RouteCounter = 0;
            foreach (var Route in InputData.FlightRoutesData)
            {
                #region RoutesDates, RoutesDay, RouteTime 

                int StartDayIndex = InputData.DatesIndexMap.FirstOrDefault(x => x.Value.Date.Date == Route.StartDate.Date).Key;
                int EndDayIndex = InputData.DatesIndexMap.FirstOrDefault(x => x.Value.Date.Date == Route.EndDate.Date).Key;
                int StartTime = Route.StartDate.Minute >= 30 ? Route.StartDate.Hour + 1 : Route.StartDate.Hour;
                int EndTime = Route.EndDate.Minute >= 30 ? Route.EndDate.Hour + 1 : Route.EndDate.Hour;

                //RoutesDay
                InputData.RoutesDay_Dict.Add(RouteCounter, (StartDayIndex, EndDayIndex));
                //RoutesTime
                InputData.RoutesTime_Dict.Add(RouteCounter, (StartTime, EndTime));
                //RoutesDate
                InputData.RoutesDates_Dict.Add(RouteCounter, (Route.StartDate, Route.EndDate));

                #endregion

                #region RoutesCompl

                #region Retrieve Complement based on Position/EmployeeType
                int Complement = 0;
                if (InputData.Position == BasicEnums.EmployeeType.Captain)
                {
                    Complement = Route.Complement_Captain;
                }
                if (InputData.Position == BasicEnums.EmployeeType.FO)
                {
                    Complement = Route.Complement_FO;
                }
                if (InputData.Position == BasicEnums.EmployeeType.Cabin_Manager)
                {
                    Complement = Route.Complement_Cabin_Manager;
                }
                if (InputData.Position == BasicEnums.EmployeeType.Flight_Attendant)
                {
                    Complement = Route.Complement_Flight_Attendant;
                }
                #endregion

                //Insert Complement
                InputData.RoutesCompl_Dict.Add(RouteCounter, Complement);

                #endregion

                for(int i = 0; i<InputData.I; i++)
                {
                    InputData.Aijf[(i, 0, RouteCounter)] = 0;

                }

                RouteCounter++;
            }
            #endregion

            #region Optimize 
            OutputData = new CSOutputData();

            
            if(InputData.CSType == BasicEnums.CSType.Set_Partition)
            {
                //Set-Partition
                OutputData = CommonFunctions.CalculateCrewScheduling_SetPartition_GB(InputData);
            }
            else if (InputData.CSType == BasicEnums.CSType.Set_Covering)
            {
                //Set Cover
                OutputData = CommonFunctions.CalculateCrewScheduling_SetCover_GB(InputData);
            }


            #endregion

            #region Print Messages To Screen

            if (OutputData != null)
            {
                MessageBox.Show($"Crew Scheduling Succeeded");
                SelectedTabIndex = 1;
            }
            else
            {
                MessageBox.Show("Error during data processing", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            #endregion

            #region Extra

            InputData.T = (int)Math.Ceiling((InputData.DateTo - InputData.DateFrom).TotalDays);

            int dateCounter = 0;
            for (var date = InputData.DateFrom; date <= InputData.DateTo; date = date.AddDays(1))
            {
                InputData.DatesIndexMap.Add(dateCounter, date.Date);
                dateCounter++;
            }

            #endregion

        }
        #endregion

        #region CPLEX
        public ICommand CalculateCS_CPLEX { get; }
        private void ExecuteCalculateCS_CPLEX(object obj)
        {
            //Na diksw autes tis Sunarthseis
            InputData.Employees = CommonFunctions.GetEmployeesByTypeData(InputData.Position, false);
            InputData.FlightRoutesData = CommonFunctions.GetCSFlightRoutesData(false, InputData);

            #region Dictionaires,Indexes Initialization

            InputData.T = new int(); // DATES
            InputData.I = new int(); // EMPLOYEES
            InputData.F = new int(); // ROUTES

            InputData.DatesIndexMap = new Dictionary<int, DateTime>();
            InputData.EmployeesIndexMap = new Dictionary<int, string>();
            InputData.RoutesIndexMap = new Dictionary<int, string>();
            InputData.RoutesCompl_Dict = new Dictionary<int, int>();

            InputData.RoutesDates_Dict = new Dictionary<int, (DateTime, DateTime)>();
            InputData.RoutesDay_Dict = new Dictionary<int, (int, int)>();
            InputData.RoutesTime_Dict = new Dictionary<int, (int, int)>();
            InputData.EmpBounds_Dict = new Dictionary<int, (double, double)>();

            InputData.Ri = new Dictionary<int, List<int>>();
            InputData.Cij_Hours = new Dictionary<(int, int), double>();
            InputData.Aijf = new Dictionary<(int, int, int), int>();
            #endregion

            #region Populate Dictionaries,Indexes



            InputData.F = InputData.FlightRoutesData.Count;
            InputData.I = InputData.Employees.Count;

            int EmployeeCounter = 0;
            foreach (var emp in InputData.Employees)
            {
                InputData.EmployeesIndexMap.Add(EmployeeCounter, emp.Code);
                InputData.EmpBounds_Dict.Add(EmployeeCounter, (emp.EmpCrSettings.LowerBound, emp.EmpCrSettings.UpperBound));

                #region Cij,Ri,Aijf Initialization 
                //Create a List with the Empty Roster
                List<int> RostersForEmployee_List = new List<int>();
                RostersForEmployee_List.Add(0);

                //Insert into Ri
                InputData.Ri.Add(EmployeeCounter, RostersForEmployee_List);

                //Add the Roster to Cij with Cost = 0
                InputData.Cij_Hours.Add((EmployeeCounter, 0), 0);
                #endregion
                EmployeeCounter++;

            }
            int RouteCounter = 0;
            foreach (var Route in InputData.FlightRoutesData)
            {
                #region RoutesDates, RoutesDay, RouteTime 

                int StartDayIndex = InputData.DatesIndexMap.FirstOrDefault(x => x.Value.Date.Date == Route.StartDate.Date).Key;
                int EndDayIndex = InputData.DatesIndexMap.FirstOrDefault(x => x.Value.Date.Date == Route.EndDate.Date).Key;
                int StartTime = Route.StartDate.Minute >= 30 ? Route.StartDate.Hour + 1 : Route.StartDate.Hour;
                int EndTime = Route.EndDate.Minute >= 30 ? Route.EndDate.Hour + 1 : Route.EndDate.Hour;

                //RoutesDay
                InputData.RoutesDay_Dict.Add(RouteCounter, (StartDayIndex, EndDayIndex));
                //RoutesTime
                InputData.RoutesTime_Dict.Add(RouteCounter, (StartTime, EndTime));
                //RoutesDate
                InputData.RoutesDates_Dict.Add(RouteCounter, (Route.StartDate, Route.EndDate));

                #endregion

                #region RoutesCompl

                #region Retrieve Complement based on Position/EmployeeType
                int Complement = 0;
                if (InputData.Position == BasicEnums.EmployeeType.Captain)
                {
                    Complement = Route.Complement_Captain;
                }
                if (InputData.Position == BasicEnums.EmployeeType.FO)
                {
                    Complement = Route.Complement_FO;
                }
                if (InputData.Position == BasicEnums.EmployeeType.Cabin_Manager)
                {
                    Complement = Route.Complement_Cabin_Manager;
                }
                if (InputData.Position == BasicEnums.EmployeeType.Flight_Attendant)
                {
                    Complement = Route.Complement_Flight_Attendant;
                }
                #endregion

                //Insert Complement
                InputData.RoutesCompl_Dict.Add(RouteCounter, Complement);

                #endregion

                for (int i = 0; i < InputData.I; i++)
                {
                    InputData.Aijf[(i, 0, RouteCounter)] = 0;

                }

                RouteCounter++;
            }
            #endregion

            #region Optimize 
            OutputData = new CSOutputData();


            if (InputData.CSType == BasicEnums.CSType.Set_Partition)
            {
                //Set-Partition
                //OutputData = CplexFunctions.CalculateCrewScheduling_SetPartition_CPLEX(InputData);
            }
            else if (InputData.CSType == BasicEnums.CSType.Set_Covering)
            {
                //Set Cover
                //OutputData = CplexFunctions.CalculateCrewScheduling_SetCover_CPLEX(InputData);
            }


            #endregion

            #region Print Messages To Screen

            if (OutputData != null)
            {
                MessageBox.Show($"{InputData.CSType} completed for crew scheduling with code : {InputData.Code}", "", MessageBoxButton.OK, MessageBoxImage.Information);
                SelectedTabIndex = 1;
            }
            else
            {
                MessageBox.Show("Error during data processing", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            #endregion

            #region Extra

            InputData.T = (int)Math.Ceiling((InputData.DateTo - InputData.DateFrom).TotalDays);

            int dateCounter = 0;
            for (var date = InputData.DateFrom; date <= InputData.DateTo; date = date.AddDays(1))
            {
                InputData.DatesIndexMap.Add(dateCounter, date.Date);
                dateCounter++;
            }

            #endregion

        }
        #endregion
        #endregion

        #region Extra
        private double CalculateCijCost(EmployeeData emp, FlightRoutesData route)
        {
            var LowerBound = emp.EmpCrSettings.LowerBound;
            var UpperBound = emp.EmpCrSettings.UpperBound;

            double targetFlightHours = (LowerBound + UpperBound) / 2; // Example target value from the image


            // Calculate actual flight hours for the route
            double actualFlightHours = (route.EndDate - route.StartDate).TotalHours;

            // Calculate variance
            double variance = Math.Abs(actualFlightHours - targetFlightHours);

            // Calculate penalty deviation
            double penaltyDeviation = 0;
            if (actualFlightHours > UpperBound)
            {
                penaltyDeviation = actualFlightHours - UpperBound;
            }
            else if (actualFlightHours < LowerBound)
            {
                penaltyDeviation = LowerBound - actualFlightHours;
            }

            // Calculate Cij_Cost
            double Cij_Hours = penaltyDeviation;
            return Cij_Hours;
        }
        #endregion
        #endregion


        public CrewSchedulingViewModel()
        {
            #region Data Initialization

            InputData = new CSInputData();
            InputData.Code = " ";
            InputData.Position = BasicEnums.EmployeeType.Captain;
            InputData.DateFrom = new DateTime(2024, 6, 1);
            InputData.DateTo = new DateTime(2024, 6, 30);
            InputData.RoutesPenalty = 1000000;
            InputData.BoundsPenalty = 100;
            InputData.CSType = BasicEnums.CSType.Set_Partition;
            InputData.FlightRoutesData = new ObservableCollection<FlightRoutesData>();
            InputData.Employees = new ObservableCollection<EmployeeData>();
            this.sfGridColumns = new Columns();

            #endregion

            #region Commands Initialization

            CalculateCS_GB = new RelayCommand2(ExecuteCalculateCS_Gurobi);
            CalculateCS_CPLEX = new RelayCommand2(ExecuteCalculateCS_CPLEX);
            ShowEmployeesGridCommand = new RelayCommand2(ExecuteShowEmployeesGridCommand);
            ShowFlightRoutestGridCommand = new RelayCommand2(ExecuteShowFlightRoutestGridCommand);
            ShowCrewSchedulingGridCommand = new RelayCommand2(ExecuteShowCrewSchedulingGridCommand);
            rowDataCommand = new RelayCommand2(ChangeCanExecute);

            #endregion

        }

    }
}
