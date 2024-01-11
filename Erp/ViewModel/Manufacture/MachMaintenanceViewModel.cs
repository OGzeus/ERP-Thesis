using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Erp.Helper;
using Erp.Model.BasicFiles;
using Erp.Model.Customers;
using Erp.Model.Inventory;
using Erp.Model.Manufacture;
using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid;

namespace Erp.ViewModel.Manufacture
{
    public class MachMaintenanceViewModel : ViewModelBase
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

        private MachMaintenanceData flatData;
        public MachMaintenanceData FlatData
        {
            get { return flatData; }
            set { flatData = value; }
        }

        private MachMaintenanceData chooserData;
        public MachMaintenanceData ChooserData
        {
            get { return chooserData; }
            set { chooserData = value; }
        }




        #endregion



        public MachMaintenanceViewModel()
        {
            FlatData = new MachMaintenanceData();

            ChooserData = new MachMaintenanceData();

            FlatData.Machine = new MachineData();
            FlatData.Factory = new FactoryData();
            FlatData.Inventory = new InventoryData();

            this.sfGridColumns = new Columns();

            FlatData.DaysOfWeek = Enumerable.Range(1, 7).Select(x => x.ToString()).ToList();
            FlatData.DaysOfMonth = Enumerable.Range(1, 31).Select(x => x.ToString()).ToList();




            ShowMachDataGridCommand = new RelayCommand2(ExecuteShowMachGridCommand);
            ShowFactoryDataGridCommand = new RelayCommand2(ExecuteShowFactoryDataGridCommand);
            ShowInvDataGridCommand = new RelayCommand2(ExecuteShowInvDataGridCommand);

            rowDataCommand = new RelayCommand2(ChangeCanExecute);


        }

        #region F7

        public ICommand ShowMachDataGridCommand { get; }
        public ICommand ShowInvDataGridCommand { get; }
        public ICommand ShowFactoryDataGridCommand { get; }

        private void ExecuteShowMachGridCommand(object obj)
        {


            ClearColumns();

            var F7input = F7Common.F7Machine(false);
            F7key = F7input.F7key;
            CollectionView = F7input.CollectionView;
            var a = F7input.SfGridColumns;
            foreach (var item in a)
            {
                this.sfGridColumns.Add(item);
            }

        }


        private void ExecuteShowFactoryDataGridCommand(object obj)
        {


            ClearColumns();

            var F7input = F7Common.F7Factory();
            F7key = F7input.F7key;
            CollectionView = F7input.CollectionView;
            var a = F7input.SfGridColumns;
            foreach (var item in a)
            {
                this.sfGridColumns.Add(item);
            }

        }

        private void ExecuteShowInvDataGridCommand(object obj)
        {


            ClearColumns();

            var F7input = F7Common.F7Inventory(false);
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
            if (F7key == "Mach")
            {
                FlatData.Machine = (SelectedItem as MachineData);
                ChooserData.Machine = (SelectedItem as MachineData);

                //FlatData.MachCode = (SelectedItem as MachineData).MachCode;
                //FlatData.MachDescr = (SelectedItem as MachineData).MachDescr;

                //ChooserData.MachCode = (SelectedItem as MachineData).MachCode;
                //ChooserData.MachDescr = (SelectedItem as MachineData).MachDescr;

                //FlatData.LastMaintenance = (SelectedItem as MachineData).LastMaintenance;
                //FlatData.NextMaintenance = (SelectedItem as MachineData).NextMaintenance;
                //FlatData.TotalOperatingHours = (SelectedItem as MachineData).TotalOperatingHours;
                //FlatData.FailureRate = (SelectedItem as MachineData).FailureRate;
                //FlatData.ProductionRate = (SelectedItem as MachineData).ProductionRate;
                //FlatData.EfficiencyRate = (SelectedItem as MachineData).EfficiencyRate;
                //FlatData.AverageRepairTime = (SelectedItem as MachineData).AverageRepairTime;

                //FlatData.NumberOfFailures = (SelectedItem as MachineData).NumberOfFailures;
                //FlatData.MachineType = (SelectedItem as MachineData).MachineType;
                //FlatData.ModelYear = (SelectedItem as MachineData).ModelYear;
                //FlatData.DateInstalled = (SelectedItem as MachineData).DateInstalled;
                //FlatData.Status = (SelectedItem as MachineData).Status;
                //FlatData.Factory.Code = (SelectedItem as MachineData).Factory.Code;
                //FlatData.Factory.Descr = (SelectedItem as MachineData).Factory.Descr;
            }
            if (F7key == "Factory")
            {
                FlatData.Factory = (SelectedItem as FactoryData);

                //FlatData.Factory.Code = (SelectedItem as FactoryData).Code;
                //FlatData.Factory.Descr = (SelectedItem as FactoryData).Descr;

            }

            if (F7key == "InvCode")
            {
                FlatData.Inventory = (SelectedItem as InventoryData);
                ChooserData.Inventory = FlatData.Inventory;

                //FlatData.Inventory.InvCode = (SelectedItem as InventoryData).InvCode;
                //FlatData.Inventory.InvDescr = (SelectedItem as InventoryData).InvDescr;

                //ChooserData.Inventory.InvCode = FlatData.Inventory.InvCode;
                //ChooserData.Inventory.InvDescr = FlatData.Inventory.InvDescr;





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

        #region Refresh,Save,Clear

        //#region 1st Tab
        //#region Clear

        //private ViewModelCommand clearCommand;

        //public ICommand ClearCommand
        //{
        //    get
        //    {
        //        if (clearCommand == null)
        //        {
        //            clearCommand = new ViewModelCommand(ExecuteClearCommand);
        //        }

        //        return clearCommand;
        //    }
        //}

        //private void ExecuteClearCommand(object commandParameter)
        //{
        //    ChooserData.Code = "";
        //    ChooserData.Descr = "";
        //    ChooserData.Id = -1;

        //    ChooserData = new CustomerData();

        //    FlatData.Code = "";
        //    FlatData.Descr = "";

        //    FlatData.Email = "";
        //    FlatData.Phone = 0;
        //    FlatData.Adress = "";
        //    FlatData.City = new CityData();
        //    FlatData.Prefecture = new PrefectureData();
        //    FlatData.Country = new CountryData();

        //    FlatData.PromptPayer = false;
        //    FlatData.CustomerType = "";
        //}

        //#endregion
        //#region Save


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
            //int Flag = CommonFunctions.SaveCustomerInfoData(FlatData);

            //if (Flag == 0)
            //{
            //    MessageBox.Show($"Ο Αποθηκεύτηκε νέος παραγωγός με Κωδικό : {FlatData.Code}");
            //}
            //else if (Flag == 1)
            //{
            //    MessageBox.Show($"Η Αποθήκευση/Ανανέωση Ολοκληρώθηκε για τον Παραγωγό με Κωδικό : {FlatData.Code}");

            //}
            //else
            //{
            //    MessageBox.Show("Σφάλμα κατά την επεξεργασία δεδομένων", "", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
        }

        //#endregion
        //#region Refresh

        //private ViewModelCommand refreshCommand;

        //public ICommand RefreshCommand
        //{
        //    get
        //    {
        //        if (refreshCommand == null)
        //        {
        //            refreshCommand = new ViewModelCommand(ExecuteRefreshCommand);
        //        }

        //        return refreshCommand;
        //    }
        //}

        //private void ExecuteRefreshCommand(object commandParameter)
        //{

        //    var data = CommonFunctions.GetCustomerInfoData(ChooserData);

        //    FlatData.Email = data.Email;
        //    FlatData.Phone = data.Phone;
        //    FlatData.Adress = data.Adress;
        //    FlatData.City = data.City;
        //    FlatData.Prefecture = data.Prefecture;
        //    FlatData.Country = data.Country;
        //    FlatData.PromptPayer = data.PromptPayer;
        //    FlatData.CustomerType = data.CustomerType;
        //    FlatData.PriceList = data.PriceList;
        //}

        //#endregion

        //#endregion

        //#region 2nd Tab
        //#region Save
        //private ViewModelCommand saveCommand2;
        //public ICommand SaveCommand2
        //{
        //    get
        //    {
        //        if (saveCommand2 == null)
        //        {
        //            saveCommand2 = new ViewModelCommand(ExecuteSaveCommand2);
        //        }

        //        return saveCommand2;
        //    }
        //}
        //private void ExecuteSaveCommand2(object obj)
        //{
        //    bool Flag = CommonFunctions.SaveItemPerSupData(FlatData2, chooserData.Code);


        //    if (Flag == true)
        //    {
        //        MessageBox.Show($"Η Αποθήκευση/Ανανέωση Ολοκληρώθηκε για τον Παραγωγό με Κωδικό : {FlatData.Code}");

        //    }
        //    else
        //    {
        //        MessageBox.Show("Σφάλμα κατά την επεξεργασία δεδομένων", "", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        //#endregion
        //#region Refresh

        //private ViewModelCommand refreshCommand2;

        //public ICommand RefreshCommand2
        //{
        //    get
        //    {
        //        if (refreshCommand2 == null)
        //        {
        //            refreshCommand2 = new ViewModelCommand(ExecuteRefreshCommand2);
        //        }

        //        return refreshCommand2;
        //    }
        //}

        //private void ExecuteRefreshCommand2(object commandParameter)
        //{

        //    FlatData2 = CommonFunctions.GetItemPerSupData(ChooserData.Code);

        //}

        //#endregion
        //#endregion

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }
        //private void BrowseButton_Click(object sender, RoutedEventArgs e)
        //{
        //    OpenFileDialog openFileDialog = new OpenFileDialog();
        //    if (openFileDialog.ShowDialog() == true)
        //    {
        //        string selectedFilePath = openFileDialog.FileName;
        //        //FlatData.DocumentPath = selectedFilePath;
        //    }
        //}


    }
}
