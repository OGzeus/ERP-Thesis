using Erp.CommonFiles;
using Erp.Helper;
using Erp.Model.Inventory;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows;
using System.ComponentModel;
using Erp.Model.BasicFiles;
using Erp.Model.Suppliers;
using Erp.Model.Customers;
using Erp.Model.Interfaces;
using Erp.Model.Enums;
using Erp.Model.Thesis;
using Syncfusion.Data.Extensions;
using OxyPlot;
using Syncfusion.Windows.Shared;

namespace Erp.ViewModel.Thesis
{
    public class EmployeeViewModel : ViewModelBase
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


        private EmployeeData flatData;
        public EmployeeData FlatData
        {
            get { return flatData; }
            set
            {
                flatData = value;
                INotifyPropertyChanged(nameof(FlatData));
            }
        }
        #region Languages
        private ObservableCollection<EMPLanguageData> _EMPLanguageData;
        public ObservableCollection<EMPLanguageData> EMPLanguageData
        {
            get { return _EMPLanguageData; }
            set
            {
                _EMPLanguageData = value;
                INotifyPropertyChanged(nameof(EMPLanguageData));
            }
        }
        private List<EMPLanguageData> newlanguagedata;
        public List<EMPLanguageData> NewLanguageData
        {
            get { return newlanguagedata; }
            set
            {
                newlanguagedata = value;
                INotifyPropertyChanged(nameof(NewLanguageData));
            }
        }


        #endregion


        #region Leave Bids

     

        private BasicEnums.BidType _SpecificBidType;
        public BasicEnums.BidType SpecificBidType
        {
            get { return _SpecificBidType; }
            set
            {
                this._SpecificBidType = value;
                INotifyPropertyChanged("SpecificBidType");
            }
        }
        #endregion

        #endregion

        #region Enums

        public BasicEnums.EmployeeType[] EmployeeTypes
        {
            get { return (BasicEnums.EmployeeType[])Enum.GetValues(typeof(BasicEnums.EmployeeType)); }
        }

        public BasicEnums.Gender[] Genders
        {
            get { return (BasicEnums.Gender[])Enum.GetValues(typeof(BasicEnums.Gender)); }
        }
        public BasicEnums.BidType[] BidTypes
        {
            get { return (BasicEnums.BidType[])Enum.GetValues(typeof(BasicEnums.BidType)); }
        }

        #endregion

        public EmployeeViewModel()
        {


            FlatData = new EmployeeData();
            ResetEmployeeViewModelData();




            this.sfGridColumns = new Columns();

            ShowEmployeeInfoGridCommand = new RelayCommand2(ExecuteShowEmployeeInfoGridCommand);
            ShowCertificationsGridCommand = new RelayCommand2(ExecuteShowCertificationsGridCommand);
            ShowAirportsGridCommand = new RelayCommand2(ExecuteShowAirportsGridCommand);

            AddEmployeeDataCommand = new RelayCommand2(ExecuteAddEmployeeDataCommand);

            rowDataCommand = new RelayCommand2(ChangeCanExecute);
            rowDataCommand2 = new RelayCommand2(ChangeCanExecuteLeaveBid);

            FlatData.MainSchedule = CommonFunctions.GetMainScheduleInfoData();

        }

        public void ResetEmployeeViewModelData()
        {
            FlatData.LeaveStatus = new LeaveStatusData();
            FlatData.HireDate = DateTime.Now;
            FlatData.DateOfBirth = DateTime.Now.AddYears(-20);
            FlatData.BaseAirport = new AirportData();
            FlatData.Certification = new CertificationData();
            FlatData.LeaveBidInfo = new LeaveBidsData();
            FlatData.LeaveBidDataGrid = new ObservableCollection<LeaveBidsData>();

            FlatData.LeaveBidDataGridStatic = new ObservableCollection<LeaveBidsDataStatic>();
            FlatData.LeaveBidsInfoStatic = new LeaveBidsDataStatic();

            FlatData.LeaveBidInfo.PriorityLevel = 1;
            FlatData.LeaveBidInfo.DateFrom = DateTime.Now;
            FlatData.LeaveBidInfo.DateTo = DateTime.Now.AddDays(10);
            FlatData.LeaveBidInfo.NumberOfDays = 10;
            FlatData.LeaveBidInfo.NumberOfDaysMin = 8;
            FlatData.LeaveBidInfo.NumberOfDaysMax = 10;

            FlatData.MainSchedule = new ReqScheduleInfoData();

            FlatData.MainSchedule = CommonFunctions.GetMainScheduleInfoData();
            EMPLanguageData = new ObservableCollection<EMPLanguageData>();
        }
        public void ResetLeaveBidInfoData(EmployeeData Data)
        {

            Data.LeaveBidInfo = new LeaveBidsData();
            Data.LeaveBidDataGrid = new ObservableCollection<LeaveBidsData>();

            Data.LeaveBidDataGridStatic = new ObservableCollection<LeaveBidsDataStatic>();
            Data.LeaveBidsInfoStatic = new LeaveBidsDataStatic();

            Data.LeaveBidInfo.PriorityLevel = 1;
            Data.LeaveBidInfo.DateFrom = DateTime.Now;
            Data.LeaveBidInfo.DateTo = DateTime.Now.AddDays(10);
            Data.LeaveBidInfo.NumberOfDays = 10;
            Data.LeaveBidInfo.NumberOfDaysMin = 8;
            Data.LeaveBidInfo.NumberOfDaysMax = 10;


        }
        #region F7

        public ICommand ShowEmployeeInfoGridCommand { get; }
        public ICommand ShowAirportsGridCommand { get; }
        public ICommand ShowCertificationsGridCommand { get; }

        public void ExecuteShowEmployeeInfoGridCommand(object obj)
        {
            ClearColumns();

            var F7input = F7Common.F7Employee(ShowDeleted);
            F7key = F7input.F7key;
            CollectionView = F7input.CollectionView;
            var a = F7input.SfGridColumns;
            foreach (var item in a)
            {
                this.sfGridColumns.Add(item);
            }

        }


        private void ExecuteShowCertificationsGridCommand(object obj)
        {
            ClearColumns();

            var F7input = F7Common.F7Certification(ShowDeleted);
            F7key = F7input.F7key;
            CollectionView = F7input.CollectionView;
            var a = F7input.SfGridColumns;
            foreach (var item in a)
            {
                this.sfGridColumns.Add(item);
            }

        }

        private void ExecuteShowAirportsGridCommand(object obj)
        {


            ClearColumns();

            var F7input = F7Common.F7Airports(false);
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

            if (F7key == "Employee")
            {

                FlatData.EmployeeId = (SelectedItem as EmployeeData).EmployeeId;

                FlatData.Code = (SelectedItem as EmployeeData).Code;
                FlatData.Descr = (SelectedItem as EmployeeData).Descr;
                FlatData.FirstName = (SelectedItem as EmployeeData).FirstName;
                FlatData.LastName = (SelectedItem as EmployeeData).LastName;
                FlatData.Gender = (SelectedItem as EmployeeData).Gender;

                FlatData.DateOfBirth = (SelectedItem as EmployeeData).DateOfBirth;
                FlatData.ContactNumber = (SelectedItem as EmployeeData).ContactNumber;
                FlatData.Email = (SelectedItem as EmployeeData).Email;
                FlatData.Address = (SelectedItem as EmployeeData).Address;
                FlatData.Position = (SelectedItem as EmployeeData).Position;
                FlatData.HireDate = (SelectedItem as EmployeeData).HireDate;
                FlatData.TotalFlightHours = (SelectedItem as EmployeeData).TotalFlightHours;
                FlatData.Seniority = (SelectedItem as EmployeeData).Seniority;
                FlatData.Language = (SelectedItem as EmployeeData).Language;

                FlatData.BaseAirport = (SelectedItem as EmployeeData).BaseAirport;
                FlatData.Certification = (SelectedItem as EmployeeData).Certification;

                FlatData.IsDeleted = (SelectedItem as EmployeeData).IsDeleted;

                EMPLanguageData = CommonFunctions.GetEMPLanguageData(FlatData.Code, false);
                FlatData.MainSchedule = CommonFunctions.GetMainScheduleInfoData();

                ExecuteRefreshCommand3(FlatData);
                ExecuteRefreshCommandLeaveStatus(FlatData);

            }
            if (F7key == "Airport")
            {
                FlatData.BaseAirport = new AirportData();
                FlatData.BaseAirport = (SelectedItem as AirportData);

            }

            if (F7key == "Certification")
            {
                FlatData.Certification = new CertificationData();
                FlatData.Certification = (SelectedItem as CertificationData);
            }
            if(F7key == "LeaveBid")
            {
                FlatData.LeaveBidInfo = new LeaveBidsData();
                FlatData.LeaveBidInfo.Employee = new EmployeeData();
                FlatData.LeaveBidInfo.Schedule = new ReqScheduleInfoData();

                FlatData.LeaveBidInfo.BidCode = (SelectedItem as LeaveBidsData).BidCode;

                FlatData.LeaveBidInfo.PriorityLevel = (SelectedItem as LeaveBidsData).PriorityLevel;

                FlatData.LeaveBidInfo.BidType = (SelectedItem as LeaveBidsData).BidType;

                FlatData.LeaveBidInfo.DateFrom = (SelectedItem as LeaveBidsData).DateFrom;
                FlatData.LeaveBidInfo.DateTo = (SelectedItem as LeaveBidsData).DateTo;

                FlatData.LeaveBidInfo.NumberOfDays = (SelectedItem as LeaveBidsData).NumberOfDays;
                FlatData.LeaveBidInfo.NumberOfDaysMin = (SelectedItem as LeaveBidsData).NumberOfDaysMin;
                FlatData.LeaveBidInfo.NumberOfDaysMax = (SelectedItem as LeaveBidsData).NumberOfDaysMax;

                FlatData.LeaveBidInfo.Employee = FlatData;
                FlatData.LeaveBidInfo.Schedule = FlatData.MainSchedule;
            }
        }

        public void ChangeCanExecuteLeaveBid(object obj)
        {

                FlatData.LeaveBidInfo = new LeaveBidsData();
                FlatData.LeaveBidInfo.Employee = new EmployeeData();
                FlatData.LeaveBidInfo.Schedule = new ReqScheduleInfoData();

                FlatData.LeaveBidInfo.BidCode = (SelectedItem2 as LeaveBidsDataStatic).BidCode;

                FlatData.LeaveBidInfo.PriorityLevel = (SelectedItem2 as LeaveBidsDataStatic).PriorityLevel;

                FlatData.LeaveBidInfo.BidType = (SelectedItem2 as LeaveBidsDataStatic).BidType;

                FlatData.LeaveBidInfo.DateFrom = (SelectedItem2 as LeaveBidsDataStatic).DateFrom;
                FlatData.LeaveBidInfo.DateTo = (SelectedItem2 as LeaveBidsDataStatic).DateTo;

                FlatData.LeaveBidInfo.NumberOfDays = (SelectedItem2 as LeaveBidsDataStatic).NumberOfDays;
                FlatData.LeaveBidInfo.NumberOfDaysMin = (SelectedItem2 as LeaveBidsDataStatic).NumberOfDaysMin;
                FlatData.LeaveBidInfo.NumberOfDaysMax = (SelectedItem2 as LeaveBidsDataStatic).NumberOfDaysMax;

                FlatData.LeaveBidInfo.Employee = FlatData;
                FlatData.LeaveBidInfo.Schedule = FlatData.MainSchedule;
         
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
    private ICommand rowDataCommand2 { get; set; }
    public ICommand RowDataCommand2
    {
        get
        {
            return rowDataCommand2;
        }
        set
        {
            rowDataCommand2 = value;
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


        #region Commands Crud

        #region 1st Tab
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
            FlatData = new EmployeeData();
            ResetEmployeeViewModelData();

        }

        #endregion
        #region Save


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
            int Flag = CommonFunctions.SaveEmployeeData(FlatData);

            if (Flag == 1)
            {
                MessageBox.Show($"Η Αποθήκευση/Ανανέωση Ολοκληρώθηκε για τον Υπάλληλο με Κωδικό : {FlatData.Code}");
                ExecuteRefreshCommand(obj);
                ExecuteShowEmployeeInfoGridCommand(obj);

            }
            else if (Flag == -1)
            {
                MessageBox.Show("Σφάλμα κατά την επεξεργασία δεδομένων", "", MessageBoxButton.OK, MessageBoxImage.Error);
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
            ResetEmployeeViewModelData();

            FlatData = CommonFunctions.GetEmployeeChooserData(FlatData.EmployeeId, FlatData.Code);

            if (FlatData.Code != null)
            {
                EMPLanguageData = CommonFunctions.GetEMPLanguageData(FlatData.Code, false);
                FlatData.LeaveStatus = CommonFunctions.GetLeaveStatusChooserData(FlatData.EmployeeId, FlatData.Code);
                ResetLeaveBidInfoData(FlatData);
            }

        }

        #endregion
        #region Add

        public ICommand AddEmployeeDataCommand { get; }

        private void ExecuteAddEmployeeDataCommand(object obj)
        {
            if (string.IsNullOrWhiteSpace(FlatData.Code) || string.IsNullOrWhiteSpace(FlatData.Descr))
            {
                MessageBox.Show("Insert Code and Description");
            }

            else
            {
                int Flag = CommonFunctions.AddEmployeeData(FlatData);
                if (Flag == 0)
                
                {
                    MessageBox.Show($"Ο Αποθηκεύτηκε νέος Υπάλληλος με Κωδικό : {FlatData.Code}");
                    ExecuteShowEmployeeInfoGridCommand(obj);
                    FlatData.EmployeeId = 0;
                    FlatData.MainSchedule = CommonFunctions.GetMainScheduleInfoData();
                    ExecuteRefreshCommand(obj);

                }
                else if (Flag == 1)
                {
                    MessageBox.Show($"The Employee with Code : {FlatData.Code} already exists");

                }
                else if (Flag == 2)
                {
                    MessageBox.Show("Error during data processing", "", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }
        #endregion

        #endregion

        #region 2nd Tab
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
            EMPLanguageData = EMPLanguageData.Where(d => d.ExistingFlag == true || (d.NewLanguageFlag == true && d.LanguageFlag == true)).ToObservableCollection();

            //FlatData2.Clear();
            //foreach (var item in NewData)
            //{
            //    FlatData2.Add(item);
            //}


            bool Flag = CommonFunctions.SaveEMPLanguageData(EMPLanguageData, FlatData.Code);


            if (Flag == true)
            {
                MessageBox.Show($"The Update of the Languages has been completed for the Employee with Code: {FlatData.Code}");
                ExecuteRefreshCommand2(obj);
            }
            else
            {
                MessageBox.Show("Error during data processing", "", MessageBoxButton.OK, MessageBoxImage.Error);
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

        public void ExecuteRefreshCommand2(object commandParameter)
        {

            FlatData.LeaveBidDataGridStatic = CommonFunctions.GetLeaveBids(FlatData.Code, FlatData.MainSchedule.ReqCode);

        }

        #endregion

        #region AddEMPLanguage

        private ViewModelCommand _AddEMPLanguage;

        public ICommand AddEMPLanguage
        {
            get
            {
                if (_AddEMPLanguage == null)
                {
                    _AddEMPLanguage = new ViewModelCommand(ExecuteAddEMPLanguage);
                }

                return _AddEMPLanguage;
            }
        }


        private void ExecuteAddEMPLanguage(object commandParameter)
        {

            EMPLanguageData = CommonFunctions.GetEMPLanguageData(FlatData.Code, true);

        }

        #endregion
        #endregion

        #region 3d Tab Leave Bids
        #region Save
        private ViewModelCommand saveCommand3;
        public ICommand SaveCommand3
        {
            get
            {
                if (saveCommand3 == null)
                {
                    saveCommand3 = new ViewModelCommand(ExecuteSaveCommand3);
                }

                return saveCommand3;
            }
        }
        private void ExecuteSaveCommand3(object obj)
        {
            FlatData.LeaveBidDataGridStatic = FlatData.LeaveBidDataGridStatic.Where(d => d.ExistingFlag == true || (d.NewBidFlag == true && d.Bidflag == true)).ToObservableCollection();


            bool Flag = CommonFunctions.SaveLeaveBidsData(FlatData.LeaveBidDataGridStatic, FlatData.Code,FlatData.MainSchedule.ReqCode);


            if (Flag == true)
            {
                MessageBox.Show($"The Update of the LeaveBids has been completed for the Employee with Code: {FlatData.Code} and for Schedule with Code :{FlatData.MainSchedule.ReqCode} ");
                ExecuteRefreshCommand3(obj);
            }
            else
            {
                MessageBox.Show("Error during data processing", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            FlatData.LeaveBidRows = new ObservableCollection<LeaveBidRowData>();
            #region Save LeaveBidRows
            //foreach (var row in FlatData.LeaveBidDataGridStatic)
            //{
            //    if(row.BidType == BasicEnums.BidType.Specific)
            //    {
            //        for(int i =0; i <= row.NumberOfDays; i++)
            //        {
            //            LeaveBidRowData dataRow = new LeaveBidRowData();
            //            dataRow.LeaveBid = new LeaveBidsDataStatic();
            //            dataRow.EmpId = FlatData.EmployeeId;
            //            dataRow.LeaveBid.BidCode = row.BidCode;
            //            dataRow.ScheduleId = MainSchedule.ID;
            //            dataRow.Date = row.DateFrom.AddDays(i);
            //            dataRow.DateStr = dataRow.Date.ToShortDateString();
            //            FlatData.LeaveBidRows.Add(dataRow);
            //        }
            //    }
            //}
            //if(FlatData.LeaveBidRows.Count > 0)
            //{
            //    bool Flag2 = CommonFunctions.SaveLeaveBidsRows(FlatData.LeaveBidRows);
            //    if (Flag == true)
            //    {
            //        MessageBox.Show($"The LeaveBidRows where Updated");
            //    }
            //    else
            //    {
            //        MessageBox.Show("Error during data processing", "", MessageBoxButton.OK, MessageBoxImage.Error);
            //    }
            //}
            #endregion
        }
        #endregion
        #region Refresh

        private ViewModelCommand refreshCommand3;

        public ICommand RefreshCommand3
        {
            get
            {
                if (refreshCommand3 == null)
                {
                    refreshCommand3 = new ViewModelCommand(ExecuteRefreshCommand3);
                }

                return refreshCommand3;
            }
        }

        public void ExecuteRefreshCommand3(object commandParameter)
        {
            FlatData.LeaveBidDataGridStatic = CommonFunctions.GetLeaveBids(FlatData.Code, FlatData.MainSchedule.ReqCode);
            #region Reset Values
            FlatData.LeaveBidInfo.BidCode = null;
            FlatData.LeaveBidInfo.DateFrom = DateTime.Now;
            FlatData.LeaveBidInfo.DateTo = DateTime.Now.AddDays(10);
            FlatData.LeaveBidInfo.NumberOfDays = 10;
            FlatData.LeaveBidInfo.NumberOfDaysMin = 8;
            FlatData.LeaveBidInfo.NumberOfDaysMax = 10;
            if(FlatData.LeaveBidDataGridStatic.Count != 0)
            {
                FlatData.LeaveBidInfo.PriorityLevel = FlatData.LeaveBidDataGridStatic.Select(item => item.PriorityLevel).Max() + 1;
            }
            else
            {
                FlatData.LeaveBidInfo.PriorityLevel = 1;
            }

            #endregion
        }

        #endregion
        #region Clear

        private ViewModelCommand clearCommand3;

        public ICommand ClearCommand3
        {
            get
            {
                if (clearCommand3 == null)
                {
                    clearCommand3 = new ViewModelCommand(ExecuteClearCommand3);
                }

                return refreshCommand3;
            }
        }

        public void ExecuteClearCommand3(object commandParameter)
        {

            FlatData.LeaveBidInfo = new LeaveBidsData();
            FlatData.LeaveBidDataGridStatic = new ObservableCollection<LeaveBidsDataStatic>();

            #region Reset Values
            FlatData.LeaveBidInfo.BidCode = null;
            FlatData.LeaveBidInfo.DateFrom = DateTime.Now;
            FlatData.LeaveBidInfo.DateTo = DateTime.Now.AddDays(10);
            FlatData.LeaveBidInfo.NumberOfDays = 10;
            FlatData.LeaveBidInfo.NumberOfDaysMin = 8;
            FlatData.LeaveBidInfo.NumberOfDaysMax = 10;
            FlatData.LeaveBidInfo.PriorityLevel =  1;

            #endregion
        }

        #endregion
        #region UpdateLeaveBid

        private ViewModelCommand _UpdateLeaveBid;

        public ICommand UpdateLeaveBid
        {
            get
            {
                if (_UpdateLeaveBid == null)
                {
                    _UpdateLeaveBid = new ViewModelCommand(ExecuteUpdateLeaveBid);
                }

                return _UpdateLeaveBid;
            }
        }


        private void ExecuteUpdateLeaveBid(object commandParameter)
        {
            //var result = System.Windows.MessageBox.Show($"The Forecast with Code {FlatData.ForCode}  will be set as the Main Forecast for the MRP . Proceed?", "Confirmation", MessageBoxButton.YesNo);
            bool add = string.IsNullOrWhiteSpace(FlatData.LeaveBidInfo.BidCode);
            bool modify = !string.IsNullOrWhiteSpace(FlatData.LeaveBidInfo.BidCode);
            #region Error Messages

            #region Wrong Input
            if (FlatData.LeaveBidInfo.BidType == BasicEnums.BidType.Specific)
            {

                if (FlatData.LeaveBidInfo.DateFrom > FlatData.LeaveBidInfo.DateTo)
                {
                    string dateFromString =  FlatData.LeaveBidInfo.DateFrom.ToString("dd/MM/yyyy") ;
                    string dateToString = FlatData.LeaveBidInfo.DateTo.ToString("dd/MM/yyyy") ;

                    System.Windows.MessageBox.Show($"The Date From = {dateFromString} is Beyond the Date To = {dateToString}",
                                                   "Error",
                                                   MessageBoxButton.OK,
                                                   MessageBoxImage.Error);
                    return;
                }
            }
            else if (FlatData.LeaveBidInfo.BidType == BasicEnums.BidType.Non_Specific)
            {
                if (FlatData.LeaveBidInfo.NumberOfDays <= 0)
                {
                    System.Windows.MessageBox.Show($"The Number Of Days is <= 0 . Current Number Of Days = {FlatData.LeaveBidInfo.NumberOfDays}" , "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else if (FlatData.LeaveBidInfo.BidType == BasicEnums.BidType.Min_Max)
            {
                if (FlatData.LeaveBidInfo.NumberOfDaysMin <= 0)
                {
                    System.Windows.MessageBox.Show($"The Minimum Number Of Days is <= 0 . Current Number Of Days = {FlatData.LeaveBidInfo.NumberOfDaysMin}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (FlatData.LeaveBidInfo.NumberOfDaysMax <= 0)
                {
                    System.Windows.MessageBox.Show($"The Maximum Number Of Days is <= 0 . Current Number Of Days = {FlatData.LeaveBidInfo.NumberOfDaysMax}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (FlatData.LeaveBidInfo.NumberOfDaysMin > FlatData.LeaveBidInfo.NumberOfDaysMax)
                {
                    System.Windows.MessageBox.Show($"Minimum Number Of Days  > Maximum Number Of Days ", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (FlatData.LeaveBidInfo.NumberOfDaysMin == FlatData.LeaveBidInfo.NumberOfDaysMax)
                {
                    System.Windows.MessageBox.Show($"Minimum Number Of Days  = Maximum Number Of Days .         If you want Fixed Number of Days set Bid Type = Non_Specific", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            #endregion

            #region Same Bid Priority
            if( add ==true )
            {
                foreach (var row in FlatData.LeaveBidDataGridStatic)
                {
                    if (row.PriorityLevel == FlatData.LeaveBidInfo.PriorityLevel)
                    {
                        System.Windows.MessageBox.Show($"There is an already added Leave Bid with Priority = {FlatData.LeaveBidInfo.PriorityLevel} .Leave BidCode = {row.BidCode} ", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
            }

            #endregion

            #region Date Errors 
            if(FlatData.LeaveBidInfo.DateFrom < FlatData.MainSchedule.DateFrom || FlatData.LeaveBidInfo.DateFrom > FlatData.MainSchedule.DateTo 
                || FlatData.LeaveBidInfo.DateTo < FlatData.MainSchedule.DateFrom || FlatData.LeaveBidInfo.DateTo > FlatData.MainSchedule.DateTo)
            {
                string dateFromString = FlatData.MainSchedule.DateFrom.ToString("dd/MM/yyyy");
                string dateToString = FlatData.MainSchedule.DateTo.ToString("dd/MM/yyyy");
                System.Windows.MessageBox.Show($"The Date Range  should be between the Dates of the Main Schedule :  {dateFromString} <= Date Range <= {dateToString}",
                               "Error",
                               MessageBoxButton.OK,
                               MessageBoxImage.Error);
                return;
            }

            #endregion

            #region Number Of Days> Current Balance
            if (FlatData.LeaveBidInfo.BidType == BasicEnums.BidType.Specific)
            {
                TimeSpan duration = FlatData.LeaveBidInfo.DateTo - FlatData.LeaveBidInfo.DateFrom;
                int numberOfDays = duration.Days; 
                if (numberOfDays > FlatData.LeaveStatus.CurrentBalance)
                {
                    System.Windows.MessageBox.Show($"The Number of days = {numberOfDays} is Beyond the Current Balance ={FlatData.LeaveStatus.CurrentBalance}",
                                                   "Error",
                                                   MessageBoxButton.OK,
                                                   MessageBoxImage.Error);
                    return;
                }
            }
            else if (FlatData.LeaveBidInfo.BidType == BasicEnums.BidType.Non_Specific)
            {
                if (FlatData.LeaveBidInfo.NumberOfDays > FlatData.LeaveStatus.CurrentBalance)
                {
                    System.Windows.MessageBox.Show($"The Number of days = {FlatData.LeaveBidInfo.NumberOfDays} is Beyond the Current Balance ={FlatData.LeaveStatus.CurrentBalance}",
                                                   "Error",
                                                   MessageBoxButton.OK,
                                                   MessageBoxImage.Error);
                    return;
                }
            }
            else if (FlatData.LeaveBidInfo.BidType == BasicEnums.BidType.Min_Max)
            {

            }
            #endregion

            #endregion



            if (add == true)
            {
                FlatData.LeaveBidInfo.BidCode = FlatData.Code + "-" + FlatData.MainSchedule.ReqCode + "-" + FlatData.LeaveBidInfo.PriorityLevel;
                FlatData.LeaveBidInfo.ExistingFlag = false;
                FlatData.LeaveBidInfo.NewBidFlag = true;
                FlatData.LeaveBidInfo.Bidflag = true;
            }
            else if (modify == true)
            {
                var RowToRemove = FlatData.LeaveBidDataGridStatic.FirstOrDefault(item => item.BidCode == FlatData.LeaveBidInfo.BidCode);
                FlatData.LeaveBidInfo.OldBidCode = RowToRemove.BidCode;
                if (RowToRemove != null)
                {
                    FlatData.LeaveBidDataGridStatic.Remove(RowToRemove);

                }
                FlatData.LeaveBidInfo.BidCode = FlatData.Code + "-" + FlatData.MainSchedule.ReqCode + "-" + FlatData.LeaveBidInfo.PriorityLevel;
                FlatData.LeaveBidInfo.ExistingFlag = true;
                FlatData.LeaveBidInfo.NewBidFlag = false;
                FlatData.LeaveBidInfo.Bidflag = true;
                FlatData.LeaveBidInfo.Modify = true;
            }

            FlatData.LeaveBidInfo.Schedule = FlatData.MainSchedule;

            if (FlatData.LeaveBidInfo.BidType == BasicEnums.BidType.Specific)
            {
                TimeSpan duration = FlatData.LeaveBidInfo.DateTo - FlatData.LeaveBidInfo.DateFrom;
                int numberOfDays = duration.Days + 1;

                FlatData.LeaveBidInfo.NumberOfDays = numberOfDays;
                
                FlatData.LeaveBidInfo.NumberOfDaysMin = FlatData.LeaveBidInfo.NumberOfDays;
                FlatData.LeaveBidInfo.NumberOfDaysMax = FlatData.LeaveBidInfo.NumberOfDays;

            }
            else if (FlatData.LeaveBidInfo.BidType == BasicEnums.BidType.Non_Specific)
            {

                FlatData.LeaveBidInfo.NumberOfDaysMin = FlatData.LeaveBidInfo.NumberOfDays;
                FlatData.LeaveBidInfo.NumberOfDaysMax = FlatData.LeaveBidInfo.NumberOfDays;
            }
            else if (FlatData.LeaveBidInfo.BidType == BasicEnums.BidType.Min_Max)
            {

                FlatData.LeaveBidInfo.NumberOfDays = 0;
            }



            #region LeaveBidsInfoStatic

            FlatData.LeaveBidsInfoStatic.Schedule = FlatData.LeaveBidInfo.Schedule;
            FlatData.LeaveBidsInfoStatic.BidCode = FlatData.LeaveBidInfo.BidCode;
            FlatData.LeaveBidsInfoStatic.OldBidCode = FlatData.LeaveBidInfo.OldBidCode;
            FlatData.LeaveBidsInfoStatic.BidType = FlatData.LeaveBidInfo.BidType;
            FlatData.LeaveBidsInfoStatic.PriorityLevel = FlatData.LeaveBidInfo.PriorityLevel;
            FlatData.LeaveBidsInfoStatic.DateFrom = FlatData.LeaveBidInfo.DateFrom;
            FlatData.LeaveBidsInfoStatic.DateTo = FlatData.LeaveBidInfo.DateTo;
            FlatData.LeaveBidsInfoStatic.DateFromStr = FlatData.LeaveBidInfo.DateFrom.ToString("dd/MM/yyyy");
            FlatData.LeaveBidsInfoStatic.DateToStr = FlatData.LeaveBidInfo.DateTo.ToString("dd/MM/yyyy");
            FlatData.LeaveBidsInfoStatic.NumberOfDays = FlatData.LeaveBidInfo.NumberOfDays;
            FlatData.LeaveBidsInfoStatic.NumberOfDaysMin = FlatData.LeaveBidInfo.NumberOfDaysMin;
            FlatData.LeaveBidsInfoStatic.NumberOfDaysMax = FlatData.LeaveBidInfo.NumberOfDaysMax;
            FlatData.LeaveBidsInfoStatic.ExistingFlag = FlatData.LeaveBidInfo.ExistingFlag; 
            FlatData.LeaveBidsInfoStatic.NewBidFlag = FlatData.LeaveBidInfo.NewBidFlag;
            FlatData.LeaveBidsInfoStatic.Bidflag = FlatData.LeaveBidInfo.Bidflag;
            FlatData.LeaveBidsInfoStatic.Modify = FlatData.LeaveBidInfo.Modify;

            FlatData.LeaveBidDataGridStatic.Add(FlatData.LeaveBidsInfoStatic);
            var orderedCollection = new ObservableCollection<LeaveBidsDataStatic>(
    FlatData.LeaveBidDataGridStatic.OrderBy(d => d.PriorityLevel)
);
            FlatData.LeaveBidDataGridStatic = orderedCollection;

            FlatData.LeaveBidsInfoStatic = new LeaveBidsDataStatic();

            #region Reset Values
            FlatData.LeaveBidInfo.BidCode = null;
            FlatData.LeaveBidInfo.DateFrom = DateTime.Now;
            FlatData.LeaveBidInfo.DateTo = DateTime.Now.AddDays(10);
            FlatData.LeaveBidInfo.NumberOfDays = 10;
            FlatData.LeaveBidInfo.NumberOfDaysMin = 8;
            FlatData.LeaveBidInfo.NumberOfDaysMax = 10;
            FlatData.LeaveBidInfo.PriorityLevel = FlatData.LeaveBidDataGridStatic.Select(item => item.PriorityLevel).Max() + 1;

            #endregion
            #endregion
        }


        #endregion
        #region DeleteLeaveBid

        private ViewModelCommand deleteBidCommand;

        public ICommand DeleteBidCommand
        {
            get
            {
                if (deleteBidCommand == null)
                {
                    deleteBidCommand = new ViewModelCommand(ExecuteDeleteBidCommand);
                }

                return deleteBidCommand;
            }
        }

        private void ExecuteDeleteBidCommand(object commandParameter)
        {
            var result = System.Windows.MessageBox.Show($"The LeaveBid with Code {FlatData.LeaveBidInfo.BidCode}  will Deleted . Proceed?", "Confirmation", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                var RowToRemove = FlatData.LeaveBidDataGridStatic.FirstOrDefault(item => item.BidCode == FlatData.LeaveBidInfo.BidCode);
                if (RowToRemove != null)
                {
                    FlatData.LeaveBidDataGridStatic.Remove(RowToRemove);
                }

                int Flag = CommonFunctions.DeleteLeaveBidData(FlatData.LeaveBidInfo);
                if (Flag == 2)
                {
                    MessageBox.Show($"The Delete has been completed for the LeaveBid  with Code : {FlatData.LeaveBidInfo.BidCode}. ");
                    ExecuteRefreshCommand3(FlatData);
                    #region Reset Values
                    FlatData.LeaveBidInfo.BidCode = null;
                    FlatData.LeaveBidInfo.DateFrom = DateTime.Now;
                    FlatData.LeaveBidInfo.DateTo = DateTime.Now.AddDays(10);
                    FlatData.LeaveBidInfo.NumberOfDays = 10;
                    FlatData.LeaveBidInfo.NumberOfDaysMin = 8;
                    FlatData.LeaveBidInfo.NumberOfDaysMax = 10;
                    FlatData.LeaveBidInfo.PriorityLevel = RowToRemove.PriorityLevel;
                    #endregion
                }
                else if(Flag ==1) 
                {
                    MessageBox.Show($"The Leave Bid with Code '{FlatData.LeaveBidInfo.BidCode}' was not saved in the database in the First Place. However, it has been successfully removed from the user interface.");
                    #region Reset Values
                    FlatData.LeaveBidInfo.BidCode = null;
                    FlatData.LeaveBidInfo.DateFrom = DateTime.Now;
                    FlatData.LeaveBidInfo.DateTo = DateTime.Now.AddDays(10);
                    FlatData.LeaveBidInfo.NumberOfDays = 10;
                    FlatData.LeaveBidInfo.NumberOfDaysMin = 8;
                    FlatData.LeaveBidInfo.NumberOfDaysMax = 10;
                    FlatData.LeaveBidInfo.PriorityLevel = RowToRemove.PriorityLevel;

                    #endregion;
                }
                else if(Flag==0)
                {
                    MessageBox.Show("Error during data processing", "", MessageBoxButton.OK, MessageBoxImage.Error);

                }

            }
            else
            {
                return;
            }


        }
        #endregion
        #endregion

        #region 3d Tab Leave Status

        #region Save Leave Status


        private ViewModelCommand _SaveCommandLeaveStatus;

        public ICommand SaveCommandLeaveStatus
        {
            get
            {
                if (_SaveCommandLeaveStatus == null)
                {
                    _SaveCommandLeaveStatus = new ViewModelCommand(ExecuteSaveCommandLeaveStatus);
                }

                return _SaveCommandLeaveStatus;
            }
        }

        private void ExecuteSaveCommandLeaveStatus(object obj)
        {
            int Flag = CommonFunctions.SaveLeaveStatusData(FlatData);

            if (Flag == 1)
            {
                MessageBox.Show($"Η Αποθήκευση/Ανανέωση του Leave Status Ολοκληρώθηκε για τον Υπάλληλο με Κωδικό : {FlatData.Code}");

            }
            else if (Flag == -1)
            {
                MessageBox.Show("Σφάλμα κατά την επεξεργασία δεδομένων", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
        #region Refresh Leave Status

        private ViewModelCommand _RefreshCommandLeaveStatus;

        public ICommand RefreshCommandLeaveStatus
        {
            get
            {
                if (_RefreshCommandLeaveStatus == null)
                {
                    _RefreshCommandLeaveStatus = new ViewModelCommand(ExecuteRefreshCommandLeaveStatus);
                }

                return _RefreshCommandLeaveStatus;
            }
        }

        private void ExecuteRefreshCommandLeaveStatus(object commandParameter)
        {
            FlatData.LeaveStatus = CommonFunctions.GetLeaveStatusChooserData(FlatData.EmployeeId, FlatData.Code);
        }

        #endregion

        #endregion
        #endregion




    }
}
