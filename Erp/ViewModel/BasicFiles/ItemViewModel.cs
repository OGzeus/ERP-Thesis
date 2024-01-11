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
using static Community.CsharpSqlite.Sqlite3;
using Erp.Model.Enums;

namespace Erp.ViewModel.BasicFiles
{
    public class ItemViewModel : ViewModelBase
    {


        #region DataProperties


        private bool newprocessflag;
        public bool NewProcessFlag
        {
            get { return newprocessflag; }
            set { newprocessflag = value; }
        }
        private bool newbomitemflag;
        public bool NewBomItemFlag
        {
            get { return newbomitemflag; }
            set { newbomitemflag = value; }
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

        private ItemData flatData;
        public ItemData FlatData
        {
            get { return flatData; }
            set
            {
                flatData = value;
                INotifyPropertyChanged(nameof(FlatData));


            }
        }



        #endregion
        #region Bill Of Materials
        private ObservableCollection<BomData> flatData2;
        public ObservableCollection<BomData> FlatData2
        {
            get { return flatData2; }
            set
            {
                flatData2 = value;
                INotifyPropertyChanged(nameof(FlatData2));
            }
        }
        private List<BomData> newdata;
        public List<BomData> NewData
        {
            get { return newdata; }
            set
            {
                newdata = value;
                INotifyPropertyChanged(nameof(NewData));
            }
        }

        private BomData chooserData2;
        public BomData ChooserData2
        {
            get { return chooserData2; }
            set { chooserData2 = value; }
        }
        #endregion

        #region Processes
        private ObservableCollection<ItemProcessData> processdata;
        public ObservableCollection<ItemProcessData> ProcessData
        {
            get { return processdata; }
            set
            {
                processdata = value;
                INotifyPropertyChanged(nameof(ProcessData));
            }
        }
        private List<ItemProcessData> newprocessdata;
        public List<ItemProcessData> NewProcessData
        {
            get { return newprocessdata; }
            set
            {
                newprocessdata = value;
                INotifyPropertyChanged(nameof(NewProcessData));
            }
        }

        private ItemProcessData chooserData3;
        public ItemProcessData ChooserData3
        {
            get { return chooserData3; }
            set { chooserData3 = value; }
        }
        #endregion




        #region Enums

        public BasicEnums.ItemType[] ItemTypes
        {
            get { return (BasicEnums.ItemType[])Enum.GetValues(typeof(BasicEnums.ItemType)); }
        }

        public BasicEnums.Assembly[] Assemblies
        {
            get { return (BasicEnums.Assembly[])Enum.GetValues(typeof(BasicEnums.Assembly)); }
        }

        #endregion

        public ItemViewModel()
        {
            FlatData = new ItemData();
            FlatData.ItemCode = " ";
            this.sfGridColumns = new Columns();
            FlatData.LotPolicy = new LotPolicyData();
            FlatData.LotPolicy.Item = new ItemData();
            ChooserData2 = new BomData();
            FlatData2 = new ObservableCollection<BomData>();

            ChooserData3 = new ItemProcessData();
            ProcessData = new ObservableCollection<ItemProcessData>();
            ShowItemDataGridCommand = new RelayCommand2(ExecuteShowItemGridCommand);
            ShowLotPolicyDataGridCommand = new RelayCommand2(ExecuteShowLotPolicyDataGridCommand);

            rowDataCommand = new RelayCommand2(ChangeCanExecute);
            AddItemCommand = new RelayCommand2(ExecuteAddItemCommand);

        }


        #region F7
        public ICommand ShowLotPolicyDataGridCommand { get; }

        private void ExecuteShowLotPolicyDataGridCommand(object obj)
        {


            ClearColumns();

            var F7input = F7Common.F7SpecificLotPolicy(FlatData);
            F7key = F7input.F7key;
            CollectionView = F7input.CollectionView;
            var a = F7input.SfGridColumns;
            foreach (var item in a)
            {
                this.sfGridColumns.Add(item);
            }

        }

        public ICommand ShowItemDataGridCommand { get; }

        public void ExecuteShowItemGridCommand(object obj)
        {


            ClearColumns();

            var F7input = F7Common.F7Item(ShowDeleted);
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
            NewBomItemFlag = false;
            NewProcessFlag = false;
            var add = 0;
            if (F7key == "ItemCode")
             {
                FlatData.ItemId = (SelectedItem as ItemData).ItemId;
                FlatData.ItemCode = (SelectedItem as ItemData).ItemCode;
                FlatData.ItemDescr = (SelectedItem as ItemData).ItemDescr;

                FlatData.MesUnit  = (SelectedItem as ItemData).MesUnit;
                FlatData.ItemType = (SelectedItem as ItemData).ItemType;
                FlatData.Assembly = (SelectedItem as ItemData).Assembly;

                FlatData.InputOrderFlag = (SelectedItem as ItemData).InputOrderFlag;
                FlatData.OutputOrderFlag = (SelectedItem as ItemData).OutputOrderFlag;
                FlatData.CanBeProduced = (SelectedItem as ItemData).CanBeProduced;

                FlatData.Profit = (SelectedItem as ItemData).Profit;
                FlatData.SalesPrice = (SelectedItem as ItemData).SalesPrice;
                FlatData.ManufacturingCost = (SelectedItem as ItemData).ManufacturingCost;
                FlatData.HoldingCost = (SelectedItem as ItemData).HoldingCost;
                FlatData.ShortageCost = (SelectedItem as ItemData).ShortageCost;
                FlatData.LeadTime = (SelectedItem as ItemData).LeadTime;
                FlatData.IsDeleted = (SelectedItem as ItemData).IsDeleted;



                FlatData.LotPolicy = new LotPolicyData();
                FlatData.LotPolicy.Item = new ItemData();

                FlatData.LotPolicy.LotPolicyId = (SelectedItem as ItemData).LotPolicy.LotPolicyId;
                FlatData.LotPolicy.Code = (SelectedItem as ItemData).LotPolicy.Code;
                FlatData.LotPolicy.Descr = (SelectedItem as ItemData).LotPolicy.Descr;
                FlatData.LotPolicy.LeadTime = (SelectedItem as ItemData).LotPolicy.LeadTime;
                FlatData.LotPolicy.BatchSize = (SelectedItem as ItemData).LotPolicy.BatchSize;
                FlatData.LotPolicy.Period = (SelectedItem as ItemData).LotPolicy.Period;
                FlatData.LotPolicy.MainPolicy = (SelectedItem as ItemData).LotPolicy.MainPolicy;
                FlatData.LotPolicy.Item.ItemId = (SelectedItem as ItemData).LotPolicy.Item.ItemId;

                add = add + 1;
                if (add >= 2)
                {
                    NewBomItemFlag = true;
                    NewProcessFlag = true;

                }
                #region Γεμίζω το Γκριντ του 2ου,3ou tab 
                FlatData2 = CommonFunctions.GetBomData(FlatData.ItemCode, false);
                ProcessData = CommonFunctions.GetPPFData(FlatData.ItemCode, false);
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
            FlatData = new ItemData
            {
                ItemCode = FlatData.ItemCode,
                ItemDescr = FlatData.ItemDescr,
                ItemId = FlatData.ItemId,
                LotPolicy = FlatData.LotPolicy
            };

            //FlatData.MesUnit = "";
            //FlatData.ItemType = "";
            //FlatData.Assembly = 99;
            //FlatData.CanBeProduced = false;
            //FlatData.InputOrderFlag = false;
            //FlatData.OutputOrderFlag = false;
            //FlatData2 = new ObservableCollection<BomData>();
            //ProcessData = new ObservableCollection<ItemProcessData>();

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
            int Flag = CommonFunctions.SaveItemData(FlatData);

            if (Flag == 1)
            {
                MessageBox.Show($"Save/Update Completed for Item with Code : {FlatData.ItemCode}");
                ExecuteShowItemGridCommand(obj);
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

            FlatData = CommonFunctions.GetItemChooserData(FlatData.ItemId,FlatData.ItemCode);



            #region Γεμίζω το Γκριντ του 2ου,3ou tab 
            FlatData2 = CommonFunctions.GetBomData(FlatData.ItemCode, false);
            ProcessData = CommonFunctions.GetPPFData(FlatData.ItemCode, false);
            #endregion

        }

        #endregion


        #region AddCommand

        public ICommand AddItemCommand { get; }

        private void ExecuteAddItemCommand(object obj)
        {
            if (string.IsNullOrWhiteSpace(FlatData.ItemCode) || string.IsNullOrWhiteSpace(FlatData.ItemDescr))
            {
                MessageBox.Show("Insert Code and Description");
            }

            else
            {
                int Flag = CommonFunctions.AddItemData(FlatData);
                if (Flag == 0)
                {
                    MessageBox.Show($"Ο Αποθηκεύτηκε νέο Είδος με Κωδικό : {FlatData.ItemCode}");
                    ExecuteShowItemGridCommand(obj);
                    FlatData.ItemId = 0;

                    ExecuteRefreshCommand(obj);

                }
                else if (Flag == 1)
                {
                    MessageBox.Show($"The Item with Code : {FlatData.ItemCode} already exists");

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
            FlatData2 = FlatData2.Where(item => item.ExistingFlag == true || (item.NewItemFlag == true && item.BomItemFlag == true)).ToObservableCollection();

            //FlatData2.Clear();
            //foreach (var item in NewData)
            //{
            //    FlatData2.Add(item);
            //}


            bool Flag = CommonFunctions.SaveBomData(FlatData2, FlatData.ItemCode);


            if (Flag == true)
            {
                MessageBox.Show($"The Update of the Bill of Materials has been completed for the Item with Code: {FlatData.ItemCode}");
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

            FlatData2 = CommonFunctions.GetBomData(FlatData.ItemCode , false);

        }

        #endregion

        #region AddBomItem

        private ViewModelCommand addBomItem;

        public ICommand AddBomItem
        {
            get
            {
                if (addBomItem == null)
                {
                    addBomItem = new ViewModelCommand(ExecuteAddBomItem);
                }

                return addBomItem;
            }
        }


        private void ExecuteAddBomItem(object commandParameter)
        {

            FlatData2 = CommonFunctions.GetBomData(FlatData.ItemCode, true);

        }

        #endregion
        #endregion
        #region 3d Tab
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
            ProcessData = ProcessData.Where(item => item.ExistingFlag == true || (item.NewProcessFlag == true && item.ClassicFlag == true)).ToObservableCollection();




            bool Flag = CommonFunctions.SavePPFData(ProcessData, FlatData.ItemCode);


            if (Flag == true)
            {
                MessageBox.Show($"Η Αποθήκευση/Ανανέωση του PPF Ολοκληρώθηκε για τον Είδος με Κωδικό : {FlatData.ItemCode}");
                ExecuteRefreshCommand3(obj);

            }
            else
            {
                MessageBox.Show("Σφάλμα κατά την επεξεργασία δεδομένων", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

        private void ExecuteRefreshCommand3(object commandParameter)
        {

            ProcessData = CommonFunctions.GetPPFData(FlatData.ItemCode, false);

        }

        #endregion

        #region AddProcess

        private ViewModelCommand addProcess;

        public ICommand AddProcess
        {
            get
            {
                if (addProcess == null)
                {
                    addProcess = new ViewModelCommand(ExecuteAddProcess);
                }

                return addProcess;
            }
        }


        private void ExecuteAddProcess(object commandParameter)
        {

            ProcessData = CommonFunctions.GetPPFData(FlatData.ItemCode, true);

        }

        #endregion
        #endregion
        #endregion





    }
}
