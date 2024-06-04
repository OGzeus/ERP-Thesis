using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using LiveCharts.Wpf;
using Syncfusion.Data.Extensions;
using Erp.Model.Manufacture.MPS;
using Erp.Model.Manufacture;
using System.Runtime.InteropServices.ComTypes;
using Erp.Model.Data_Analytics.Forecast;
using System.Reflection.Emit;
using Erp.Model.Data_Analytics;
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
using Erp.Model.Manufacture.MRP;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Syncfusion.UI.Xaml.Grid;
using Erp.Helper;
using Erp.Model.BasicFiles;
using Erp.Model.Thesis.CrewScheduling;
using Erp.Model.Thesis;
using Erp.Model.Thesis.VacationPlanning;
using Syncfusion.Windows.Shared;

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


        #endregion

        #region Enums

        public BasicEnums.EmployeeType[] EmployeeTypes
        {
            get { return (BasicEnums.EmployeeType[])Enum.GetValues(typeof(BasicEnums.EmployeeType)); }
        }



        #endregion
        public CrewSchedulingViewModel()
        {


            InputData = new CSInputData();
            InputData.Code = " ";
            InputData.Position = BasicEnums.EmployeeType.Captain;
            InputData.DateFrom = new DateTime(2024, 5, 1);
            InputData.DateTo = new DateTime(2024, 5, 31);
            InputData.RoutesPenalty = 1000000;
            InputData.BoundsPenalty = 100;

            InputData.FlightRoutesData = new ObservableCollection<FlightRoutesData>();
            InputData.Employees = new ObservableCollection<EmployeeData>();





            this.sfGridColumns = new Columns();
            this.SfGridColumnsRepair = new Columns();

            CalculateCS = new RelayCommand2(ExecuteCalculateCS);
            ShowEmployeesGridCommand = new RelayCommand2(ExecuteShowEmployeesGridCommand);
            ShowFlightRoutestGridCommand = new RelayCommand2(ExecuteShowFlightRoutestGridCommand);
            ShowCrewSchedulingGridCommand = new RelayCommand2(ExecuteShowCrewSchedulingGridCommand);

            rowDataCommand = new RelayCommand2(ChangeCanExecute);


        }

        #region Commands

        #region F7 
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
                    MessageBox.Show($"New Crew Scheduling saved with Code: {InputData.Code}");
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

            InputData = new CSInputData();
        }

        #endregion

        #region SaveMPS


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
        #endregion


        #region Calculate MRP
        public ICommand CalculateCS { get; }

        private void ExecuteCalculateCS(object obj)
        {
            InputData.Employees = CommonFunctions.GetEmployeesByTypeData(InputData.Position, false);
            InputData.FlightRoutesData = CommonFunctions.GetCSFlightRoutesData(false, InputData);

            #region Column Generation

            #region Input Data

            InputData.T = new int(); // DATES
            InputData.N = new int(); // EMPLOYEES
            InputData.R = new int(); // ROUTES

            InputData.DatesIndexMap = new Dictionary<int, DateTime>();
            InputData.EmployeesIndexMap = new Dictionary<int, string>();
            InputData.RoutesIndexMap = new Dictionary<int, string>();

            InputData.RoutesDates_Dict = new Dictionary<int, (DateTime, DateTime)>();
            InputData.RoutesDay_Dict = new Dictionary<int, (int, int)>();
            InputData.RoutesTime_Dict = new Dictionary<int, (int, int)>();

            InputData.EmpBounds_Dict = new Dictionary<int, (double, double)>();
            #endregion  

            #region Fill Data to Dictionaries

            #region Dates


            InputData.T = (int)Math.Ceiling((InputData.DateTo - InputData.DateFrom).TotalDays);

                int dateCounter = 1;
                for (var date = InputData.DateFrom; date <= InputData.DateTo; date = date.AddDays(1))
                {
                    InputData.DatesIndexMap.Add(dateCounter, date.Date);
                    dateCounter++;
                }

            #endregion

            #region Routes
            InputData.R = InputData.FlightRoutesData.Count;
            int RoutCounter = 1;
            foreach(var Route in InputData.FlightRoutesData)
            {
                InputData.RoutesDates_Dict.Add(RoutCounter, (Route.StartDate, Route.EndDate));

                int StartDayIndex = InputData.DatesIndexMap.FirstOrDefault(x => x.Value.Date.Date == Route.StartDate.Date).Key;
                int EndDayIndex = InputData.DatesIndexMap.FirstOrDefault(x => x.Value.Date.Date == Route.EndDate.Date).Key;

                int StartTime = Route.StartDate.Minute >= 30 ? Route.StartDate.Hour + 1 : Route.StartDate.Hour;

                int EndTime = Route.EndDate.Minute >= 30 ? Route.EndDate.Hour + 1 : Route.EndDate.Hour;

                InputData.RoutesDay_Dict.Add(RoutCounter, (StartDayIndex, EndDayIndex));

                InputData.RoutesTime_Dict.Add(RoutCounter, (StartTime, EndTime));

                RoutCounter ++;

            }
            #endregion

            #region Employees
            InputData.N = InputData.Employees.Count;
            int EmployeeCounter = 1;
            foreach(var emp in InputData.Employees)
            {
                InputData.EmployeesIndexMap.Add(EmployeeCounter, emp.Code);
                InputData.EmpBounds_Dict.Add(EmployeeCounter, (emp.EmpCrSettings.LowerBound, emp.EmpCrSettings.UpperBound));
                EmployeeCounter++;

            }
            #endregion

            #endregion

            var a = CommonFunctions.CalculateCrewScheduling(InputData);


            #endregion            
            //SelectedTabIndex = 2;

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
