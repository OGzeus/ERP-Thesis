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
using Xceed.Wpf.Toolkit.Primitives;
using System.ComponentModel;
using Erp.Model.BasicFiles;
using Erp.Model.Suppliers;
using Erp.Model.Customers;
using Erp.Model.Interfaces;
using Erp.Model.Enums;

namespace Erp.ViewModel.Customer
{
    public class CustomerViewModel : ViewModelBase
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

        private CustomerData flatData;
        public CustomerData FlatData
        {
            get { return flatData; }
            set 
            {
                flatData = value;
                INotifyPropertyChanged(nameof(FlatData));
            }
        }



        private ObservableCollection<SupItemData> flatData2;
        public ObservableCollection<SupItemData> FlatData2
        {
            get { return flatData2; }
            set
            {
                flatData2 = value;
                INotifyPropertyChanged(nameof(FlatData2));
            }
        }

        private SupItemData chooserData2;
        public SupItemData ChooserData2
        {
            get { return chooserData2; }
            set { chooserData2 = value; }
        }


        #endregion

        #region Enums

        public BasicEnums.CustomerType[] CustomerTypes
        {
            get { return (BasicEnums.CustomerType[])Enum.GetValues(typeof(BasicEnums.CustomerType)); }
        }

        public BasicEnums.MachType[] MachTypes
        {
            get { return (BasicEnums.MachType[])Enum.GetValues(typeof(BasicEnums.MachType)); }
        }

        #endregion

        public CustomerViewModel()
        {
            FlatData = new CustomerData();
            FlatData.Code = " ";
            FlatData.City = new CityData();
            FlatData.Prefecture = new PrefectureData();
            FlatData.Country = new CountryData();
            this.sfGridColumns = new Columns();

            ChooserData2 = new SupItemData();
            FlatData2 = new ObservableCollection<SupItemData>();




            ShowCityGridCommand = new RelayCommand2(ExecuteShowCityGridCommand);
            ShowCustomerInfoGridCommand = new RelayCommand2(ExecuteShowCustomerGridCommand);
            ShowPriceListGridCommand = new RelayCommand2(ExecuteShowPriceListGridCommand);
            AddCustomerDataCommand = new RelayCommand2(ExecuteAddCustomerDataCommand);

            rowDataCommand = new RelayCommand2(ChangeCanExecute);


        }


        #region F7

        public ICommand ShowCustomerInfoGridCommand { get; }
        public ICommand ShowCityGridCommand { get; }
        public ICommand ShowPriceListGridCommand { get; }

        public void ExecuteShowCustomerGridCommand(object obj)
        {


            ClearColumns();

            var F7input = F7Common.F7Customer(ShowDeleted);
            F7key = F7input.F7key;
            CollectionView = F7input.CollectionView;
            var a = F7input.SfGridColumns;
            foreach (var item in a)
            {
                this.sfGridColumns.Add(item);
            }

        }


        private void ExecuteShowCityGridCommand(object obj)
        {
            ClearColumns();

            var F7input = F7Common.F7City(ShowDeleted);
            F7key = F7input.F7key;
            CollectionView = F7input.CollectionView;
            var a = F7input.SfGridColumns;
            foreach (var item in a)
            {
                this.sfGridColumns.Add(item);
            }

        }

        private void ExecuteShowPriceListGridCommand(object obj)
        {


            ClearColumns();

            var F7input = F7Common.F7Customer_PriceList(false);
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

            if (F7key == "Customer")
            {

                FlatData.Id = (SelectedItem as CustomerData).Id;

                FlatData.Code = (SelectedItem as CustomerData).Code;
                FlatData.Descr = (SelectedItem as CustomerData).Descr;
                FlatData.Email = (SelectedItem as CustomerData).Email;
                FlatData.Phone = (SelectedItem as CustomerData).Phone;
                FlatData.Adress = (SelectedItem as CustomerData).Adress;
                FlatData.CustomerType = (SelectedItem as CustomerData).CustomerType;
                FlatData.PromptPayer = (SelectedItem as CustomerData).PromptPayer;
                FlatData.PostalCode = (SelectedItem as CustomerData).PostalCode;

                FlatData.IsDeleted = (SelectedItem as CustomerData).IsDeleted;


                #region Γεμίζω το Γκριντ του 2ου tab 
                //FlatData2 = CommonFunctions.GetItemPerSupData(ChooserData.Code);

                #endregion
                var data = CommonFunctions.GetCustomerChooserData(FlatData.Id,FlatData.Code);


                FlatData.Email = data.Email;
                FlatData.Phone = data.Phone;
                FlatData.Adress = data.Adress;
                FlatData.City = data.City;
                FlatData.Prefecture = data.Prefecture;
                FlatData.Country = data.Country;
                FlatData.PromptPayer = data.PromptPayer;
                FlatData.CustomerType = data.CustomerType;
                FlatData.PriceList = data.PriceList;


            }
            if (F7key == "City")
            {
                FlatData.City = new CityData();
                FlatData.City = (SelectedItem as CityData);

                FlatData.Country.CountryCode = (SelectedItem as CityData).CountryCode;
                FlatData.Country.CountryDescr = (SelectedItem as CityData).CountryDescr;

                FlatData.Prefecture.PrefCode = (SelectedItem as CityData).PrefCode;
                FlatData.Prefecture.PrefDescr = (SelectedItem as CityData).PrefDescr;

            }
            if (F7key == "PriceList")
            {
                FlatData.PriceList = (SelectedItem as PriceListData);
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


        #region Refresh,Save,Clear,Add

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

            //FlatData.Code = "";
            //FlatData.Descr = "";

            //FlatData.Email = "";
            //FlatData.Phone = 0;
            //FlatData.Adress = "";
            //FlatData.City = new CityData();
            //FlatData.Prefecture = new PrefectureData();
            //FlatData.Country = new CountryData();

            //FlatData.PromptPayer = false;
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
            int Flag = CommonFunctions.SaveCustomerInfoData(FlatData);

            if (Flag == 1)
            {
                MessageBox.Show($"Η Αποθήκευση/Ανανέωση Ολοκληρώθηκε για τον Πελάτη με Κωδικό : {FlatData.Code}");
                ExecuteRefreshCommand(obj);
                ExecuteShowCustomerGridCommand(obj);

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

            FlatData = CommonFunctions.GetCustomerChooserData(FlatData.Id, FlatData.Code);

        }

        #endregion
        #region Add

        public ICommand AddCustomerDataCommand { get; }

        private void ExecuteAddCustomerDataCommand(object obj)
        {
            if (string.IsNullOrWhiteSpace(FlatData.Code) || string.IsNullOrWhiteSpace(FlatData.Descr))
            {
                MessageBox.Show("Insert Code and Description");
            }

            else
            {
                int Flag = CommonFunctions.AddCustomerData(FlatData);
                if (Flag == 0)
                {
                    MessageBox.Show($"Ο Αποθηκεύτηκε νέος πελάτης με Κωδικό : {FlatData.Code}");
                    ExecuteShowCustomerGridCommand(obj);
                    FlatData.Id = 0;
                    ExecuteRefreshCommand(obj);
                }
                else if (Flag == 1)
                {
                    MessageBox.Show($"The Customer with Code : {FlatData.Code} already exists");

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
            bool Flag = CommonFunctions.SaveItemPerSupData(FlatData2, FlatData.Code);


            if (Flag == true)
            {
                MessageBox.Show($"Η Αποθήκευση/Ανανέωση Ολοκληρώθηκε για τον Παραγωγό με Κωδικό : {FlatData.Code}");

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

        private void ExecuteRefreshCommand2(object commandParameter)
        {

            FlatData2 = CommonFunctions.GetItemPerSupData(FlatData.Code);

        }

        #endregion
        #endregion

        #endregion




    }
}



