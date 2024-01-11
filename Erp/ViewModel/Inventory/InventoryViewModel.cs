
using Erp.CommonFiles;
using Erp.Helper;
using Erp.Model.BasicFiles;
using Erp.Model.Inventory;
using Erp.Model.Suppliers;
using Erp.ViewModel;
using FontAwesome.Sharp;
using GalaSoft.MvvmLight.Command;
using MyControls;
using OxyPlot;
using Syncfusion.Data.Extensions;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Utility;
using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;

namespace Erp.ViewModel.Inventory
{
    public class InventoryViewModel : ViewModelBase
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

        private InventoryData flatData;
        public InventoryData FlatData
        {
            get { return flatData; }
            set { flatData = value; }
        }

        private InventoryData chooserData;
        public InventoryData ChooserData
        {
            get { return chooserData; }
            set { chooserData = value; }
        }

        private ObservableCollection<StockData> flatData2;
        public ObservableCollection<StockData> FlatData2
        {
            get { return flatData2; }
            set
            {
                flatData2 = value;
                INotifyPropertyChanged(nameof(FlatData2));
            }
        }

        private StockData chooserData2;
        public StockData ChooserData2
        {
            get { return chooserData2; }
            set { chooserData2 = value; }
        }


        #endregion

        public InventoryViewModel()
        {
            FlatData = new InventoryData();
            FlatData.InvCode = " ";
            ChooserData = new InventoryData();
            this.sfGridColumns = new Columns();

            ChooserData2 = new StockData();
            FlatData2 = new ObservableCollection<StockData>();




            ShowInventoryGridCommand = new RelayCommand2(ExecuteShowInventoryGridCommand);
            AddInventoryCommand = new RelayCommand2(ExecuteAddInventoryCommand);

            rowDataCommand = new RelayCommand2(ChangeCanExecute);


        }


        #region F7

        public ICommand ShowInventoryGridCommand { get; }
        public ICommand ShowCityGridCommand { get; }

        public void ExecuteShowInventoryGridCommand(object obj)
        {


            ClearColumns();

            var F7input = F7Common.F7Inventory(ShowDeleted);
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
            if (F7key == "InvCode")
            {
                FlatData.InvId = (SelectedItem as InventoryData).InvId;
                FlatData.InvCode = (SelectedItem as InventoryData).InvCode;
                FlatData.InvDescr = (SelectedItem as InventoryData).InvDescr;
                FlatData.IsDeleted = (SelectedItem as InventoryData).IsDeleted;

                ChooserData.InvCode = FlatData.InvCode;
                ChooserData.InvDescr = FlatData.InvDescr;


                var data = CommonFunctions.GetInventoryChooserData(FlatData.InvId,FlatData.InvCode);


                FlatData.Location = data.Location;
                FlatData.Capacity = data.Capacity;

                #region Γεμίζω το Γκριντ του 2ου tab 
                FlatData2 = CommonFunctions.GetStockData(ChooserData.InvCode,false);
                var a = 5;
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

            FlatData.InvCode = "";
            FlatData.InvDescr = "";

            FlatData.Capacity = 0;
            FlatData.Location = "";
            FlatData.IsDeleted = false;

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
            int Flag = CommonFunctions.SaveInventoryData(FlatData);

            if (Flag == 1)
            {
                MessageBox.Show($"Saving/Updating completed for the Inventory with Code: {FlatData.InvCode}");
                ExecuteShowInventoryGridCommand(obj);

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

            var data = CommonFunctions.GetInventoryChooserData(FlatData.InvId,FlatData.InvCode);

            FlatData.InvCode = data.InvCode;
            FlatData.InvDescr = data.InvDescr;

            FlatData.Location = data.Location;
            FlatData.Capacity = data.Capacity;

        }

        #endregion
        #region Add

        public ICommand AddInventoryCommand { get; }

        private void ExecuteAddInventoryCommand(object obj)
        {
            if (string.IsNullOrWhiteSpace(FlatData.InvCode) || string.IsNullOrWhiteSpace(FlatData.InvDescr))
            {
                MessageBox.Show("Insert Code and Description");
            }

            else
            {
                int Flag = CommonFunctions.AddInventoryData(FlatData);
                if (Flag == 0)
                {
                    MessageBox.Show($"Ο Αποθηκεύτηκε νέο Είδος με Κωδικό : {FlatData.InvCode}");
                    ExecuteRefreshCommand(obj);
                    ExecuteShowInventoryGridCommand(obj);

                }
                else if (Flag == 1)
                {
                    MessageBox.Show($"The Inventory with Code : {FlatData.InvCode} already exists");

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
        #region AddStockItem

        private ViewModelCommand _AddStockItem;

        public ICommand AddStockItem
        {
            get
            {
                if (_AddStockItem == null)
                {
                    _AddStockItem = new ViewModelCommand(ExecuteAddStockItem);
                }

                return _AddStockItem;
            }
        }


        private void ExecuteAddStockItem(object commandParameter)
        {

            FlatData2 = CommonFunctions.GetStockData(FlatData.InvCode, true);

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
                var list= FlatData2.Where(item => item.ExistingFlag == true || (item.NewItemFlag == true && item.StockItemFlag == true)).ToList();
                FlatData2 = list.ToObservableCollection();
            //FlatData2.Clear();
            //foreach (var item in NewData)
            //{
            //    FlatData2.Add(item);
            //}


            bool Flag = CommonFunctions.SaveStockData(FlatData2, FlatData.InvId);


                if (Flag == true)
                {
                MessageBox.Show($"The Update of Stock has been completed for the Warehouse with Code: {FlatData.InvCode}");
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

            FlatData2 = CommonFunctions.GetStockData(ChooserData.InvCode,false);

        }

        #endregion
        #endregion

        #endregion




    }
}



