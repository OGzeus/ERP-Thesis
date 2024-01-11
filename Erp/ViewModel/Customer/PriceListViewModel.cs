using Erp.Helper;
using Erp.Model.BasicFiles;
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
using Erp.Model.Customers;
using Syncfusion.Data.Extensions;
using Syncfusion.Windows.Shared;

namespace Erp.ViewModel.Customer
{
    public class PriceListViewModel : ViewModelBase
    {
        #region DataProperties




        private bool newitemflag;
        public bool NewItemFlag
        {
            get { return newitemflag; }
            set { newitemflag = value; }
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

        private PriceListData flatData;
        public PriceListData FlatData
        {
            get { return flatData; }
            set { flatData = value; }
        }

        private PriceListData chooserData;
        public PriceListData ChooserData
        {
            get { return chooserData; }
            set { chooserData = value; }
        }




        private ObservableCollection<PriceListItemData> olddata;
        public ObservableCollection<PriceListItemData> OldData
        {
            get { return olddata; }
            set
            {
                olddata = value;
                INotifyPropertyChanged(nameof(OldData));
            }
        }

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public PriceListViewModel()
        {
            FlatData = new PriceListData();
            FlatData.Code = " ";
            ChooserData = new PriceListData();
            this.sfGridColumns = new Columns();


            FlatData.DateStart = DateTime.Now;
            FlatData.DateEnd = DateTime.Now.AddYears(1);

            ShowPriceListGridCommand = new RelayCommand2(ExecuteShowPriceListGridCommand);
            ShowCustomersGridCommand = new RelayCommand2(ExecuteShowCustomersGridCommand);

            rowDataCommand = new RelayCommand2(ChangeCanExecute);


            AddPriceListCommand = new RelayCommand2(ExecuteAddPriceListCommand);

        }


        #region F7

        public ICommand ShowPriceListGridCommand { get; }
        public ICommand ShowCustomersGridCommand { get; }

        public void ExecuteShowPriceListGridCommand(object obj)
        {


            ClearColumns();

            var F7input = F7Common.F7Customer_PriceList(ShowDeleted);
            F7key = F7input.F7key;
            CollectionView = F7input.CollectionView;
            var a = F7input.SfGridColumns;
            foreach (var item in a)
            {
                this.sfGridColumns.Add(item);
            }

        }


        private void ExecuteShowCustomersGridCommand(object obj)
        {


            ClearColumns();

            var F7input = F7Common.F7CustomersOfPriceList(FlatData.Id);
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
            NewItemFlag = false;
            var add = 0;
            if (F7key == "PriceList")
            {
                FlatData.Id = (SelectedItem as PriceListData).Id;
                FlatData.Code = (SelectedItem as PriceListData).Code;
                FlatData.Descr = (SelectedItem as PriceListData).Descr;
                FlatData.IsDeleted = (SelectedItem as PriceListData).IsDeleted;

                ChooserData.Code = (SelectedItem as PriceListData).Code;
                ChooserData.Descr = (SelectedItem as PriceListData).Descr;
                FlatData.Retail = (SelectedItem as PriceListData).Retail;
                FlatData.Wholesale = (SelectedItem as PriceListData).Wholesale;
                FlatData.DateStart = (SelectedItem as PriceListData).DateStart;
                FlatData.DateEnd = (SelectedItem as PriceListData).DateEnd;
                add = add + 1;
                if (add >= 2)
                {
                    NewItemFlag = true;
                }
                #region Γεμίζω το Γκριντ του 2ου tab 
                FlatData.ItemsInfo = CommonFunctions.GetPriceListItemData(FlatData ,false);
                #endregion
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

            ChooserData.Code = "";
            ChooserData.Descr = "";
            FlatData.Id = 0;
            FlatData.Code = "";
            FlatData.Descr = "";
            FlatData.Retail = false;
            FlatData.Wholesale = false;
            FlatData.ItemsInfo.Clear();
            FlatData.CustomerData = new CustomerData();
            FlatData.DateStart = DateTime.Now;
            FlatData.DateEnd = DateTime.Now.AddYears(1);

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
            int Flag = CommonFunctions.SavePriceListData(FlatData);


            if (Flag == 1)
            {
                MessageBox.Show($"Saving/Updating completed for the Price Catalog with Code: {FlatData.Code}");
                ExecuteRefreshCommand(obj);
                ExecuteShowPriceListGridCommand(obj);
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

            var data = CommonFunctions.GetPriceListData(FlatData);
            FlatData.Code = data.Code;
            FlatData.Descr = data.Descr;
            FlatData.Retail = data.Retail;
            FlatData.Wholesale = data.Wholesale;
            FlatData.DateStart = data.DateStart;
            FlatData.DateEnd = data.DateEnd;

            #region Γεμίζω το Γκριντ του 2ου tab 
            FlatData.ItemsInfo = CommonFunctions.GetPriceListItemData(FlatData, false);
            OldData = FlatData.ItemsInfo;
            #endregion
        }

        #endregion
        #region Add

        public ICommand AddPriceListCommand { get; }

        private void ExecuteAddPriceListCommand(object obj)
        {
            if (string.IsNullOrWhiteSpace(FlatData.Code) || string.IsNullOrWhiteSpace(FlatData.Descr))
            {
                MessageBox.Show("Insert Code and Description");
            }

            else
            {
                int Flag = CommonFunctions.AddPriceListData(FlatData);
                if (Flag == 0)
                {
                    MessageBox.Show($"Ο Αποθηκεύτηκε νέος τιμοκατάλογος με Κωδικό : {FlatData.Code}");
                    ExecuteShowPriceListGridCommand(obj);
                    ExecuteRefreshCommand(obj);
                }
                else if (Flag == 1)
                {
                    MessageBox.Show($"The Pricelist with Code : {FlatData.Code} already exists");

                }
                else if (Flag == 2)
                {
                    MessageBox.Show("Σφάλμα κατά την επεξεργασία δεδομένων", "", MessageBoxButton.OK, MessageBoxImage.Error);
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
            FlatData.ItemsInfo = FlatData.ItemsInfo.Where(item =>  item.IsChecked == true || item.Existing == true ).ToObservableCollection();

            bool Flag = CommonFunctions.SavePriceListItemData(FlatData);


            if (Flag == true)
            {
                MessageBox.Show($"Η Αποθήκευση/Ανανέωση των ειδών Ολοκληρώθηκε για τον Τιμοκατάλογο με Κωδικό : {FlatData.Code}");
                ExecuteShowPriceListGridCommand(obj);
                ExecuteRefreshCommand(obj);

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

            FlatData.ItemsInfo = CommonFunctions.GetPriceListItemData(FlatData, false);

        }

        #endregion
        #region AddBomItem

        private ViewModelCommand addBomItem;

        public ICommand AddItem
        {
            get
            {
                if (addBomItem == null)
                {
                    addBomItem = new ViewModelCommand(ExecuteAddItem);
                }

                return addBomItem;
            }
        }


        private void ExecuteAddItem(object commandParameter)
        {

            FlatData.ItemsInfo = CommonFunctions.GetPriceListItemData(FlatData, true);

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
