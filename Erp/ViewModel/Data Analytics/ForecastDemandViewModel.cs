using Erp.CommonFiles;
using Erp.Helper;
using Erp.Model.BasicFiles;
using Erp.Model.Suppliers;
using MyControls;
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
using Erp.Model.Data_Analytics.Forecast;
using Erp.Model.Enums;
using Erp.Model.Data_Analytics;
using Erp.Model.Customers;
using static Erp.Model.Enums.BasicEnums;
using Erp.Model.Thesis;

namespace Erp.ViewModel.Data_Analytics
{
    public class ForecastDemandViewModel : ViewModelBase
    {

        #region DataProperties
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
        
        private ForecastInfoData flatData;
        public ForecastInfoData FlatData
        {
            get { return flatData; }
            set
            {
                flatData = value;
                INotifyPropertyChanged(nameof(FlatData));


            }
        }
        private bool _CurrentMRPForecast;
        public bool CurrentMRPForecast
        {
            get { return _CurrentMRPForecast; }
            set
            {
                _CurrentMRPForecast = value;
                INotifyPropertyChanged(nameof(CurrentMRPForecast));


            }
        }

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        #region Enums

        public BasicEnums.PeriodType[] PeriodTypes
        {
            get { return (BasicEnums.PeriodType[])Enum.GetValues(typeof(BasicEnums.PeriodType)); }
        }

        public BasicEnums.Timebucket[] Timebuckets
        {
            get { return (BasicEnums.Timebucket[])Enum.GetValues(typeof(BasicEnums.Timebucket)); }
        }

        #endregion
        public ForecastDemandViewModel()
        {

            FlatData = new ForecastInfoData();
            FlatData.ForCode = " ";
            FlatData.DemandForecast = new ObservableCollection<DemandForecastData>();

            FlatData.HoursPerTimeBucket = 1;
            FlatData.DateFrom = DateTime.Now;
            FlatData.DateTo = DateTime.Now.AddMonths(6);
            FlatData.Notes = "Notes";

            ShowItemDataGridCommand = new RelayCommand2(ExecuteShowItemGridCommand);
            ShowForecastDataGridCommand = new RelayCommand2(ExecuteShowForecastDataGridCommand);
            CreateDemandFCommand = new RelayCommand2(ExecuteCreateDemandFCommand);
            DefaultHoursPerTimeBucketCommand = new RelayCommand2(ExecuteDefaultHoursPerTimeBucketCommand);

            AddForecastCommand = new RelayCommand2(ExecuteAddForecastCommand);
            ShowForecastDataGridCommand = new RelayCommand2(ExecuteShowForecastDataGridCommand);

            this.sfGridColumns = new Columns();
            rowDataCommand = new RelayCommand2(ChangeCanExecute);
        }

        #region F7


        public ICommand ShowItemDataGridCommand { get; }

        private void ExecuteShowItemGridCommand(object obj)
        {


            ClearColumns();

            var F7input = F7Common.F7Item(false);
            F7key = F7input.F7key;
            CollectionView = F7input.CollectionView;
            var a = F7input.SfGridColumns;
            foreach (var item in a)
            {
                this.sfGridColumns.Add(item);
            }

        }

        public ICommand DefaultHoursPerTimeBucketCommand { get; }

        private void ExecuteDefaultHoursPerTimeBucketCommand(object obj)
        {
            if (FlatData.TimeBucket == BasicEnums.Timebucket.Daily)
            {
                FlatData.HoursPerTimeBucket = 2*8;

            }
            else if (FlatData.TimeBucket == BasicEnums.Timebucket.Weekly)
            {
                FlatData.HoursPerTimeBucket = 2*8*5;
            }
            else if (FlatData.TimeBucket == BasicEnums.Timebucket.Monthly)
            {
                FlatData.HoursPerTimeBucket = 2*8*5*4;
            }
            else if (FlatData.TimeBucket == BasicEnums.Timebucket.Quarterly)
            {
                FlatData.HoursPerTimeBucket = 2 * 8 * 5 * 22 * 3;
            }


        }

        public ICommand ShowForecastDataGridCommand { get; }

        private void ExecuteShowForecastDataGridCommand(object obj)
        {


            ClearColumns();

            var F7input = F7Common.F7Forecast(ShowDeleted);
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
            //NewBomItemFlag = false;
            //NewProcessFlag = false;
            if (F7key == "MPSForecast")
            {
                ForecastInfoData SelectedForecast = new ForecastInfoData();

                SelectedForecast.ID = (SelectedItem as ForecastInfoData).ID;
                SelectedForecast.ForCode = (SelectedItem as ForecastInfoData).ForCode;
                SelectedForecast.ForDescr = (SelectedItem as ForecastInfoData).ForDescr;
                SelectedForecast.Notes = (SelectedItem as ForecastInfoData).Notes;
                SelectedForecast.TimeBucket = (SelectedItem as ForecastInfoData).TimeBucket;
                SelectedForecast.PeriodType = (SelectedItem as ForecastInfoData).PeriodType;
                SelectedForecast.PeriodNumber = (SelectedItem as ForecastInfoData).PeriodNumber;
                SelectedForecast.HoursPerTimeBucket = (SelectedItem as ForecastInfoData).HoursPerTimeBucket;

                SelectedForecast.DateFrom = (SelectedItem as ForecastInfoData).DateFrom;
                SelectedForecast.DateTo = (SelectedItem as ForecastInfoData).DateTo;
                SelectedForecast.IsDeleted = (SelectedItem as ForecastInfoData).IsDeleted;
                SelectedForecast.MRPForecast = (SelectedItem as ForecastInfoData).MRPForecast;



                FlatData = SelectedForecast;

                FlatData.DemandForecast = CommonFunctions.GetDemandForecast(SelectedForecast.ForCode);
                CurrentMRPForecast = FlatData.MRPForecast;



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

        #region ButtonCommands
        #region 1st Tab
        #region Add
        public ICommand AddForecastCommand { get; }

        private void ExecuteAddForecastCommand(object obj)
        {
            if (string.IsNullOrWhiteSpace(FlatData.ForCode) || string.IsNullOrWhiteSpace(FlatData.ForDescr))
            {
                MessageBox.Show("Insert Code and Description");
            }

            else
            {
                int Flag = CommonFunctions.AddForecastData(FlatData);
                if (Flag == 0)
                {
                    MessageBox.Show($"New Forecast saved with Code: {FlatData.ForCode}");

                    FlatData.ID = 0;
                    ExecuteRefreshCommand(obj);
                }
                else if (Flag == 1)
                {
                    MessageBox.Show($"The Forecast with Code : {FlatData.ForCode} already exists");

                }
                else if (Flag == 2)
                {
                    MessageBox.Show("Σφάλμα κατά την επεξεργασία δεδομένων", "", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }
        #endregion
        #region Clear

        private ViewModelCommand clearCommand;

        public ICommand ClearCommand
        {
            get
            {
                if (clearCommand == null)
                {
                    clearCommand = new ViewModelCommand(ExecuteClearCommand);
                }

                return clearCommand;
            }
        }

        private void ExecuteClearCommand(object commandParameter)
        {


            FlatData = new ForecastInfoData();


        }

        #endregion
        #region Save


        private ViewModelCommand _MainMrpForecastCommand;

        public ICommand MainMrpForecastCommand
        {
            get
            {
                if (_MainMrpForecastCommand == null)
                {
                    _MainMrpForecastCommand = new ViewModelCommand(ExecuteMainMrpForecastCommand);
                }

                return _MainMrpForecastCommand;
            }
        }

        private void ExecuteMainMrpForecastCommand(object obj)
        {
            int Flag = CommonFunctions.SetMainForecastForMRP(FlatData);

            if (Flag == 2)
            {
                MessageBox.Show($"The Main Forecast for MRP was updated successfully");

            }
            else if (Flag == -1)
            {
                MessageBox.Show("Error during data processing", "", MessageBoxButton.OK, MessageBoxImage.Error);
                FlatData.MRPForecast = CurrentMRPForecast;

            }
            else if (Flag == 1)
            {
                FlatData.MRPForecast = CurrentMRPForecast;
            }



        }

        private ViewModelCommand savecommand;

        public ICommand SaveCommand
        {
            get
            {
                if (savecommand == null)
                {
                    savecommand = new ViewModelCommand(ExecuteSaveCommand);
                }

                return savecommand;
            }
        }

        private void ExecuteSaveCommand(object obj)
        {
            CalculateNumberOfBuckets(obj);
            int Flag = CommonFunctions.SaveForecastData(FlatData);

            if (Flag == 1)
            {
                MessageBox.Show($"Saving/Updating completed for the Forecast with Code: {FlatData.ForCode}");

            }
            else if (Flag == -1)
            {
                MessageBox.Show("Error during data processing", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void CalculateNumberOfBuckets(object obj)
        {
            var DateFrom = flatData.DateFrom;
            var DateTo = flatData.DateTo;
            int NumberOfBuckets = 0; // Initialize to zero or appropriate default value

            if (flatData.TimeBucket == Timebucket.Daily)
            {
                var NumberOfDays = (int)(DateTo - DateFrom).TotalDays + 1; // NUMBER OF DAYS
                NumberOfBuckets = NumberOfDays;
            }
            else if (flatData.TimeBucket == Timebucket.Weekly)
            {
                var NumberOfWeeks = (int)(DateTo - DateFrom).TotalDays / 7; // NUMBER OF WEEKS
                NumberOfBuckets = NumberOfWeeks;
            }
            else if (flatData.TimeBucket == Timebucket.Monthly)
            {
                var NumberOfMonths = (DateTo.Month - DateFrom.Month +1) + 12 * (DateTo.Year - DateFrom.Year); // NUMBER OF MONTHS
                NumberOfBuckets = NumberOfMonths;
            }

            FlatData.NumberOfBuckets = NumberOfBuckets;
        }


        #endregion
        #region Refresh

        private ViewModelCommand refreshCommand;

        public ICommand RefreshCommand
        {
            get
            {
                if (refreshCommand == null)
                {
                    refreshCommand = new ViewModelCommand(ExecuteRefreshCommand);
                }

                return refreshCommand;
            }
        }

        private void ExecuteRefreshCommand(object commandParameter)
        {


            FlatData = CommonFunctions.GetForecastInfoChooserData(FlatData.ID,FlatData.ForCode);


        }

        #endregion

        #endregion

        #region 2nd Tab
        #region Create or UpdateForecast
        public ICommand CreateDemandFCommand { get; }

        private void ExecuteCreateDemandFCommand(object obj)
        { 
            FlatData = CommonFunctions.ChangeDatesFormat(FlatData);
            if(FlatData.DemandForecast.Count == 0)
            {
                var DemandForecast = new ObservableCollection<DemandForecastData>();

                // Calculate the number of days between DateFrom and DateTo
                var numberOfDays = (FlatData.DateTo - FlatData.DateFrom).Days;

                var numberOfMonths = (FlatData.DateTo.Year - FlatData.DateFrom.Year) * 12 + (FlatData.DateTo.Month - FlatData.DateFrom.Month + 1);

                if (FlatData.DateTo.Day < FlatData.DateFrom.Day)
                {
                    numberOfMonths--;
                }
                var numberOfWeeks = (FlatData.DateTo.Month - FlatData.DateFrom.Month);

                var Items = CommonFunctions.GetItemData(false);

                var selectedItems = Items.Where(item => item.OutputOrderFlag == true).ToObservableCollection();

                Items = selectedItems;


                foreach (var item in Items)
                {
                    if (FlatData.TimeBucket == Model.Enums.BasicEnums.Timebucket.Daily)
                    {
                        for (int i = 0; i <= numberOfDays; i++)
                        {
                            var DForecastLine = new DemandForecastData();

                            DForecastLine.Demand = 50;
                            DForecastLine.Date = FlatData.DateFrom.AddDays(i);
                            DForecastLine.Item = item;
                            DForecastLine.DateStr = CommonFunctions.ChangeSpecificDateFormat(FlatData, DForecastLine.Date);
                            DemandForecast.Add(DForecastLine);


                        }
                    }
                    if (FlatData.TimeBucket == Model.Enums.BasicEnums.Timebucket.Weekly)
                    {

                        for (int i = 0; i <= numberOfDays; i = i + 6)
                        {
                            var DForecastLine = new DemandForecastData();

                            DForecastLine.Demand = 200;
                            DForecastLine.Date = FlatData.DateFrom.AddDays(i); 
                            DForecastLine.Item = item;
                            DForecastLine.DateStr = CommonFunctions.ChangeSpecificDateFormat(FlatData, DForecastLine.Date);
                            DemandForecast.Add(DForecastLine);

                        }

                    }
                    if (FlatData.TimeBucket == Model.Enums.BasicEnums.Timebucket.Monthly)
                    {
                        for (int i = 0; i <= numberOfMonths-1; i++)
                        {
                            var DForecastLine = new DemandForecastData();

                            DForecastLine.Demand = 50;
                            DForecastLine.Date = FlatData.DateFrom.AddMonths(i);
                            DForecastLine.Item = item;
                            DForecastLine.DateStr = CommonFunctions.ChangeSpecificDateFormat(FlatData, DForecastLine.Date);
                            DemandForecast.Add(DForecastLine);


                        }
                    }


                }

                FlatData.DemandForecast = DemandForecast;

              }





        }
        #endregion
        #region Save
        private ViewModelCommand saveCommand2;
        public ICommand SaveCommand2
        {
            get
            {
                if (saveCommand2 == null)
                {
                    saveCommand2 = new ViewModelCommand(ExecuteSaveCommand2);
                }

                return saveCommand2;
            }
        }
        private void ExecuteSaveCommand2(object obj)
        {

            bool Flag = CommonFunctions.SaveDemandForecast(FlatData);


            if (Flag == true)
            {
                MessageBox.Show($"Η Αποθήκευση/Ανανέωση της ζήτησης Ολοκληρώθηκε για την Προβλεψη με Κωδικό : {FlatData.ForCode}");

            }
            else
            {
                MessageBox.Show("Σφάλμα κατά την επεξεργασία δεδομένων", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Refresh

        private ViewModelCommand refreshCommand2;

        public ICommand RefreshCommand2
        {
            get
            {
                if (refreshCommand2 == null)
                {
                    refreshCommand2 = new ViewModelCommand(ExecuteRefreshCommand2);
                }

                return refreshCommand2;
            }
        }

        private void ExecuteRefreshCommand2(object commandParameter)
        {

            FlatData.DemandForecast = CommonFunctions.GetDemandForecast(FlatData.ForCode);

        }
        #endregion

        #region Clear

        private ViewModelCommand _ClearCommand2;

        public ICommand ClearCommand2
        {
            get
            {
                if (_ClearCommand2 == null)
                {
                    _ClearCommand2 = new ViewModelCommand(ExecuteClearCommand2);
                }

                return _ClearCommand2;
            }
        }

        private void ExecuteClearCommand2(object commandParameter)
        {

            FlatData.DemandForecast.Clear();
            ExecuteCreateDemandFCommand(FlatData);
        }
        #endregion
        #endregion
        #endregion
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }
    }
}
