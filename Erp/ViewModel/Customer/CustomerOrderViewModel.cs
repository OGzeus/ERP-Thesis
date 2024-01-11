using Erp.Helper;
using Erp.Model.BasicFiles;
using Erp.Model.Customers;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.ComponentModel;
using Syncfusion.Windows.Tools.Controls;
using System.Runtime.InteropServices.ComTypes;
using Erp.Model.Enums;
using static IronPython.Runtime.Profiler;
using Syncfusion.Data.Extensions;

namespace Erp.ViewModel.Customer
{
    public class CustomerOrderViewModel : ViewModelBase
    {


        #region DataProperties

        public float quantity { get; set; }
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

        private CustomerOrderData customerOrderData;
        public CustomerOrderData CustomerOrderData
        {
            get { return customerOrderData; }
            set
            {
                customerOrderData = value;
                INotifyPropertyChanged(nameof(CustomerOrderData));
            }
        }

        private ItemQuantityOrderData itemqdata;
        public ItemQuantityOrderData ItemQData
        {
            get { return itemqdata; }
            set
            {
                this.itemqdata = value;
                INotifyPropertyChanged("ItemQData");
            }
        }
        private ObservableCollection<ItemData> itemdata;
        public ObservableCollection<ItemData> ItemData
        {
            get { return itemdata; }
            set
            {
                itemdata = value;
                INotifyPropertyChanged(nameof(ItemData));
            }
        }

        #region Enums

        public BasicEnums.Incoterms[] Incoterms
        {
            get { return (BasicEnums.Incoterms[])Enum.GetValues(typeof(BasicEnums.Incoterms)); }
        }

        public BasicEnums.OrderStatus[] OrderStatus
        {
            get { return (BasicEnums.OrderStatus[])Enum.GetValues(typeof(BasicEnums.OrderStatus)); }
        }
        #endregion
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public CustomerOrderViewModel()
        {
            CustomerOrderData = new CustomerOrderData();
            CustomerOrderData.CustOrderId = " ";
            ItemQData = new ItemQuantityOrderData();
            ItemData = new ObservableCollection<ItemData>();
            this.sfGridColumns = new Columns();
            CustomerOrderData.Customer = new CustomerData();
            CustomerOrderData.PriceList = new PriceListData();
            CustomerOrderData.Customer.City = new CityData ();
            CustomerOrderData.Customer.Country = new CountryData();
            CustomerOrderData.Customer.Prefecture = new PrefectureData();


            CustomerOrderData.CartData = new ObservableCollection<ItemQuantityOrderData>();

            ShowCustomerOrderGridCommand = new RelayCommand2(ExecuteShowCustomerOrderGridCommand);
            ShowCustomerInfoGridCommand = new RelayCommand2(ExecuteShowCustomerGridCommand);
            ShowPriceListGridCommand = new RelayCommand2(ExecuteShowPriceListGridCommand);

            InsertItemFromRowCommand = new RelayCommand2(ExecuteInsertItemFromRowCommand);
            AddAdvancedItem = new RelayCommand2(ExecuteAddAdvancedItem);

            rowDataCommand = new RelayCommand2(ChangeCanExecute);

            ItemData = CommonFunctions.GetItemsForSaleData();

            DateTime DateCreated = DateTime.Now;
            CustomerOrderData.DateCreated = DateCreated;

            DateTime endDate = DateTime.Parse("2024-01-01");
            CustomerOrderData.DeliveryDate = DateTime.Now.AddDays(10);
            #region Enums

            #endregion
        }


        #region F7
        public ICommand ShowPriceListGridCommand { get; }
        public ICommand ShowCustomerOrderGridCommand { get; }

        public ICommand ShowCustomerInfoGridCommand { get; }
        public ICommand InsertItemFromRowCommand { get; }
        public ICommand AddAdvancedItem { get; }



        public void ExecuteShowCustomerOrderGridCommand(object obj)
        {


            ClearColumns();

            var F7input = F7Common.F7CustomerOrder(ShowDeleted);
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

        private void ExecuteShowCustomerGridCommand(object obj)
        {


            ClearColumns();

            var F7input = F7Common.F7Customer(false);
            F7key = F7input.F7key;
            CollectionView = F7input.CollectionView;
            var a = F7input.SfGridColumns;
            foreach (var item in a)
            {
                this.sfGridColumns.Add(item);
            }

        }

        private void ExecuteInsertItemFromRowCommand(object obj)
        {
            ItemQData = new ItemQuantityOrderData();
            ItemQData.CityDelivered = new CityData();

            ItemQData = (SelectedItem as ItemQuantityOrderData);



            ItemQData.DeliveryDate = DateTime.Now;
            ItemQData.CityDelivered.CityCode = CustomerOrderData.Customer.City.CityCode;

        }
        private void ExecuteAddAdvancedItem(object obj)
        {
            CustomerOrderData.CartData.Add(ItemQData);
        }
        public void ChangeCanExecute(object obj)
        {
            if (F7key == "CustomerOrder")
            {


                CustomerOrderData = (SelectedItem as CustomerOrderData);
                //CustomerOrderData.CustOrderId = (SelectedItem as CustomerOrderData).CustOrderId;
                CustomerOrderData.CartData = CommonFunctions.GetCorderCartData(CustomerOrderData,false);
                //CustomerOrderData.Incoterms = (SelectedItem as CustomerOrderData).Incoterms;


            }
            if (F7key == "Customer")
            {
                CustomerOrderData.Customer = new CustomerData();
                CustomerOrderData.PriceList = new PriceListData();
                CustomerOrderData.Customer = (SelectedItem as CustomerData);


            }
            if (F7key == "PriceList")
            {
                CustomerOrderData.PriceList = new PriceListData();

                CustomerOrderData.PriceList = (SelectedItem as PriceListData);
            }


            CustomerOrderData.PriceList = CommonFunctions.GetPriceListData(CustomerOrderData.PriceList);
            CustomerOrderData.PriceList.ItemsInfo = CommonFunctions.GetPriceListItemData(CustomerOrderData.PriceList, false);
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
            //    CustomerOrderData.CartData = CustomerOrderData.CartData.Where(item => item.IsChecked == true || item.Existing == true).ToObservableCollection();

            //    bool Flag = CommonFunctions.SaveC(CustomerOrderData.CartData);


            //    if (Flag == true)
            //    {
            //        MessageBox.Show($"Η Αποθήκευση/Ανανέωση των ειδών Ολοκληρώθηκε για τον Τιμοκατάλογο με Κωδικό : {FlatData.Code}");

            //    }
            //    else
            //    {
            //        MessageBox.Show("Σφάλμα κατά την επεξεργασία δεδομένων", "", MessageBoxButton.OK, MessageBoxImage.Error);
            //    }
        }

        #endregion

        #region AddCommand

        public ICommand AddItemCommand { get; }

        private void ExecuteAddItemCommand(object obj)
        {
            //if (string.IsNullOrWhiteSpace(.ItemCode) || string.IsNullOrWhiteSpace(FlatData.ItemDescr))
            //{
            //    MessageBox.Show("Insert Code and Description");
            //}

            //else
            //{
            //    int Flag = CommonFunctions.AddItemData(FlatData);
            //    if (Flag == 0)
            //    {
            //        MessageBox.Show($"Ο Αποθηκεύτηκε νέο Είδος με Κωδικό : {FlatData.ItemCode}");
            //        ExecuteRefreshCommand(obj);
            //        ExecuteShowItemGridCommand(obj);

            //    }
            //    else if (Flag == 1)
            //    {
            //        MessageBox.Show($"The Item with Code : {FlatData.ItemCode} already exists");

            //    }
            //    else if (Flag == 2)
            //    {
            //        MessageBox.Show("Σφάλμα κατά την επεξεργασία δεδομένων", "", MessageBoxButton.OK, MessageBoxImage.Error);
            //    }
            //}

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

            //var data = CommonFunctions.GetCustomerInfoData(ChooserData);

            //FlatData.Code = data.Code;
            //FlatData.Descr = data.Descr;

            //FlatData.Email = data.Email;
            //FlatData.Phone = data.Phone;
            //FlatData.Adress1 = data.Adress1;
            //FlatData.CityCode = data.CityCode;
            //FlatData.CityDescr = data.CityCode;
            //FlatData.PrefDescr = data.PrefDescr;
            //FlatData.CountryDescr = data.CountryDescr;
            //FlatData.PromptPayer = data.PromptPayer;
            //FlatData.CustomerType = data.CustomerType;
        }

        #endregion

        #endregion

        #region 2nd Tab
       
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

            CustomerOrderData.CartData = CommonFunctions.GetCorderCartData(customerOrderData, false);

        }

        #endregion

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
            CustomerOrderData.CartData = CustomerOrderData.CartData.Where(item => item.IsChecked == true || item.ExistingFlag == true).ToObservableCollection();

            bool Flag = CommonFunctions.SaveCorderCartData(CustomerOrderData);


            if (Flag == true)
            {
                MessageBox.Show($"Η Αποθήκευση/Ανανέωση των ειδών Ολοκληρώθηκε για την Παραγγελία με CustOrderId : {CustomerOrderData.CustOrderId}");

            }
            else
            {
                MessageBox.Show("Σφάλμα κατά την επεξεργασία δεδομένων", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #region AddItem

        private ViewModelCommand addItem;

        public ICommand AddItem
        {
            get
            {
                if (addItem == null)
                {
                    addItem = new ViewModelCommand(ExecuteAddItem);
                }

                return addItem;
            }
        }
        private void ExecuteAddItem(object commandParameter)
        {
            CustomerOrderData.CartData = CommonFunctions.GetCorderCartData(CustomerOrderData, true);

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
