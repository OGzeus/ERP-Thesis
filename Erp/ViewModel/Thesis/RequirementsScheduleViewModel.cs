using Erp.CommonFiles;
using Erp.Helper;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Windows;
using Syncfusion.Data.Extensions;
using Erp.Model.Data_Analytics.Forecast;
using Erp.Model.Enums;
using Erp.Model.Data_Analytics;
using static Erp.Model.Enums.BasicEnums;
using Erp.Model.Thesis;

namespace Erp.ViewModel.Thesis
{
    public class RequirementsScheduleViewModel : ViewModelBase
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

        private ReqScheduleInfoData flatData;
        public ReqScheduleInfoData FlatData
        {
            get { return flatData; }
            set
            {
                flatData = value;
                INotifyPropertyChanged(nameof(FlatData));


            }
        }

        private bool currentmainschedule;
        public bool CurrentMainSchedule
        {
            get { return currentmainschedule; }
            set
            {
                currentmainschedule = value;
                INotifyPropertyChanged(nameof(CurrentMainSchedule));


            }
        }
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        #region Enums



        #endregion
        public RequirementsScheduleViewModel()
        {

            FlatData = new ReqScheduleInfoData();
            FlatData.ReqCode = " ";
            FlatData.ReqScheduleRowsData = new ObservableCollection<ReqScheduleRowsData>();
            FlatData.DateFrom = DateTime.Now;
            FlatData.DateTo = DateTime.Now.AddMonths(6);
            FlatData.Notes = "Notes";
            FlatData.LimitLineFixed = 2;

            ShowReqScheduleInfoDataGridCommand = new RelayCommand2(ExecuteShowReqScheduleInfoDataGridCommand);
            CreateScheduleCommand = new RelayCommand2(ExecuteCreateScheduleCommand);

            AddScheduleCommand = new RelayCommand2(ExecuteAddScheduleCommand);

            this.sfGridColumns = new Columns();
            rowDataCommand = new RelayCommand2(ChangeCanExecute);
        }

        #region F7

        public ICommand ShowReqScheduleInfoDataGridCommand { get; }

        private void ExecuteShowReqScheduleInfoDataGridCommand(object obj)
        {


            ClearColumns();

            var F7input = F7Common.F7ReqSchedule(ShowDeleted);
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
            if (F7key == "ReqSchedule")
            {
                ReqScheduleInfoData SelectedSchedule = new ReqScheduleInfoData();

                SelectedSchedule.ID = (SelectedItem as ReqScheduleInfoData).ID;
                SelectedSchedule.ReqCode = (SelectedItem as ReqScheduleInfoData).ReqCode;
                SelectedSchedule.ReqDescr = (SelectedItem as ReqScheduleInfoData).ReqDescr;
                SelectedSchedule.Notes = (SelectedItem as ReqScheduleInfoData).Notes;
                SelectedSchedule.LimitLineFixed = (SelectedItem as ReqScheduleInfoData).LimitLineFixed;

                SelectedSchedule.DateFrom = (SelectedItem as ReqScheduleInfoData).DateFrom;
                SelectedSchedule.DateTo = (SelectedItem as ReqScheduleInfoData).DateTo;
                SelectedSchedule.IsDeleted = (SelectedItem as ReqScheduleInfoData).IsDeleted;
                SelectedSchedule.MainSchedule = (SelectedItem as ReqScheduleInfoData).MainSchedule;

                FlatData = SelectedSchedule;

                FlatData.ReqScheduleRowsData = CommonFunctions.GetReqSchedulesRows(SelectedSchedule.ReqCode);
                CurrentMainSchedule = FlatData.MainSchedule;
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
        public ICommand AddScheduleCommand { get; }

        private void ExecuteAddScheduleCommand(object obj)
        {
            if (string.IsNullOrWhiteSpace(FlatData.ReqCode) || string.IsNullOrWhiteSpace(FlatData.ReqDescr))
            {
                MessageBox.Show("Insert Code and Description");
            }

            else
            {
                int Flag = CommonFunctions.AddReqScheduleInfoData(FlatData);
                if (Flag == 0)
                {
                    MessageBox.Show($"New Schedule saved with Code: {FlatData.ReqCode}");

                    FlatData.ID = 0;
                    ExecuteRefreshCommand(obj);
                }
                else if (Flag == 1)
                {
                    MessageBox.Show($"The Schedule with Code : {FlatData.ReqCode} already exists");

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
            //ChooserData.ItemCode = "";
            //ChooserData.ItemDescr = "";
            //ChooserData.ItemId = -1;

            //ChooserData = new ItemData();

            //FlatData.ItemCode = "";
            //FlatData.ItemDescr = "";
            //FlatData.ItemId = 0;



        }

        #endregion
        #region Save


        private ViewModelCommand _MainScheduleCommand;

        public ICommand MainScheduleCommand
        {
            get
            {
                if (_MainScheduleCommand == null)
                {
                    _MainScheduleCommand = new ViewModelCommand(ExecuteMainScheduleCommand);
                }

                return _MainScheduleCommand;
            }
        }

        private void ExecuteMainScheduleCommand(object obj)
        {
            int Flag = CommonFunctions.SetMainSchedule(FlatData);

            if (Flag == 2)
            {
                MessageBox.Show($"The Main Main Schedule was updated successfully");
                ExecuteShowReqScheduleInfoDataGridCommand(FlatData);

            }
            else if (Flag == -1)
            {
                MessageBox.Show("Error during data processing", "", MessageBoxButton.OK, MessageBoxImage.Error);
                FlatData.MainSchedule = CurrentMainSchedule;

            }
            else if (Flag == 1)
            {
                FlatData.MainSchedule = CurrentMainSchedule;
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
            int Flag = CommonFunctions.SaveReqScheduleInfoData(FlatData);

            if (Flag == 1)
            {
                MessageBox.Show($"Saving/Updating completed for the Schedule with Code: {FlatData.ReqCode}");

            }
            else if (Flag == -1)
            {
                MessageBox.Show("Error during data processing", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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


            FlatData = CommonFunctions.GetReqScheduleInfoChooserData(FlatData.ID, FlatData.ReqCode);


        }

        #endregion

        #endregion

        #region 2nd Tab
        #region Create or UpdateSchedule
        public ICommand CreateScheduleCommand { get; }

        private void ExecuteCreateScheduleCommand(object obj)
        {

            int LimitLine = FlatData.LimitLineFixed;
            //FlatData = CommonFunctions.ChangeDatesFormat(FlatData);
            if (FlatData.ReqScheduleRowsData.Count == 0)
            {
                var ReqScheduleRowsData = new ObservableCollection<ReqScheduleRowsData>();

                // Calculate the number of days between DateFrom and DateTo
                var numberOfDays = (FlatData.DateTo - FlatData.DateFrom).Days;

                var numberOfMonths = (FlatData.DateTo.Year - FlatData.DateFrom.Year) * 12 + (FlatData.DateTo.Month - FlatData.DateFrom.Month + 1);




                for (int i = 0; i <= numberOfDays; i++)
                {
                    var Row = new ReqScheduleRowsData();

                        Row.LimitLine = LimitLine;
                        Row.Date = FlatData.DateFrom.AddDays(i);
                        Row.DateStr = FlatData.DateFrom.AddDays(i).ToString("dd/MM/yyyy");


                    ReqScheduleRowsData.Add(Row);


                }



                FlatData.ReqScheduleRowsData = ReqScheduleRowsData;

            }



        }


        #endregion

        #region Crud
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

            bool Flag = CommonFunctions.SaveReqScheduleRows(FlatData);


            if (Flag == true)
            {
                MessageBox.Show($"Η Αποθήκευση/Ανανέωση του προγραμματος Ολοκληρώθηκε για τo Schedule με Κωδικό : {FlatData.ReqCode}");

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

            FlatData.ReqScheduleRowsData = CommonFunctions.GetReqSchedulesRows(FlatData.ReqCode);

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

            FlatData.ReqScheduleRowsData.Clear();
            ExecuteCreateScheduleCommand(FlatData);
        }
        #endregion
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
