using Erp.CommonFiles;
using Erp.Helper;
using Erp.Model.BasicFiles;
using Erp.Model.Suppliers;
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
using Erp.Model.Manufacture;
using System.Runtime.InteropServices.ComTypes;
using DevExpress.Data.Filtering.Helpers;
using Erp.Model.Data_Analytics.Forecast;
using static IronPython.Runtime.Profiler;
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
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace Erp.ViewModel.Manufacture
{
    public class MRPViewModel : ViewModelBase
    {





        #region DataProperties

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
        private ObservableCollection<MPSOptResultsData> diagramdata;
        public ObservableCollection<MPSOptResultsData> DiagramData
        {
            get { return diagramdata; }
            set
            {
                diagramdata = value;
                INotifyPropertyChanged(nameof(DiagramData));
            }
        }

        private MRPInputData inputdata;
        public MRPInputData InputData
        {
            get { return inputdata; }
            set
            {
                inputdata = value;
                INotifyPropertyChanged(nameof(InputData));


            }
        }


        private MrpResultData _GridResultsData;
        public MrpResultData GridResultsData
        {
            get { return _GridResultsData; }
            set
            {
                _GridResultsData = value;
                INotifyPropertyChanged(nameof(GridResultsData));


            }
        }

        private MRPOutputData outputdata;
        public MRPOutputData OutputData
        {
            get { return outputdata; }
            set
            {
                outputdata = value;
                INotifyPropertyChanged(nameof(OutputData));


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

        public MRPViewModel()
        {


            InputData = new MRPInputData();
            InputData.MRPCode = " ";
            InputData.Items = new ObservableCollection<ItemData>();
            InputData.Bom = new ObservableCollection<BomData>();
            InputData.Inventory = new InventoryData();
            InputData.Inventory.StockData = new ObservableCollection<StockData>();

            InputData.StackPanelEnabled = false;
            OutputData = new MRPOutputData();
            GridResultsData = new MrpResultData();


            this.sfGridColumns = new Columns();
            this.SfGridColumnsRepair = new Columns();

            InsertDataCommand = new RelayCommand2(ExecuteInsertDataCommand);
            CalculateMrp = new RelayCommand2(ExecuteCalculateMrp);
            ShowMRPForecastDemand = new RelayCommand2(ExecuteShowMRPForecastDemand);
            ShowInventoryGridCommand = new RelayCommand2(ExecuteShowInventoryGridCommand);
            ShowStockCommand = new RelayCommand2(ExecuteShowStockCommand);

            rowDataCommand = new RelayCommand2(ChangeCanExecute);
            ShowMRPCommand = new RelayCommand2(ExecuteShowMRPCommand);
            ShowItemInputGridCommand = new RelayCommand2(ExecuteShowItemInputGridCommand);
            #region Diagrams
            #region Κατασκευή Κριτιρίων Αναζήτησης και Δήλωση Button Commands

            InputData.Diagram = new InvDiagramsSearchData();
            InputData.Diagram.Item = new ItemData();
            this.SfGridColumnsD = new Columns();

            ShowItemGridCommand = new RelayCommand2(ExecuteShowItemGridCommand);
            AddMRPCommand = new RelayCommand2(ExecuteAddMRPCommand);

            rowDataCommandD = new RelayCommand2(ChangeCanExecuteD);

            DateTime startDate = DateTime.Parse("2021-01-01");
            InputData.Diagram.DateStart = startDate;

            DateTime endDate = DateTime.Parse("2024-01-01");
            InputData.Diagram.DateEnd = endDate;

            InputData.Diagram.PeriodPolicy = new List<string>
            {
                            "Ανά Εβδομάδα",
                           "Ανά Μήνα",
                            "Ανά 12μηνο"
            };
            #endregion






            #endregion
        }

        #region Commands

        #region F7 
        public ICommand ShowInventoryGridCommand { get; }
        public ICommand ShowMRPForecastDemand { get; }
        public ICommand ShowMRPCommand { get; }
        public ICommand ShowPriceListGridCommand { get; }
        public ICommand ShowItemInputGridCommand { get; }
        public ICommand ShowStockCommand { get; }

        public void ExecuteShowStockCommand(object obj)
        {


            ClearColumns();

            var F7input = F7Common.F7MRPStock();
            F7key = F7input.F7key;
            CollectionView = CollectionViewSource.GetDefaultView(InputData.Inventory.StockData.ToList());
            var a = F7input.SfGridColumns;
            foreach (var item in a)
            {
                this.sfGridColumns.Add(item);
            }

        }

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

        private void ExecuteShowItemInputGridCommand(object obj)
        {


            ClearColumns();

            var F7input = F7Common.F7ItemMPSInput();
            F7key = F7input.F7key;
            CollectionView = CollectionViewSource.GetDefaultView(InputData.Items.ToList());
            var a = F7input.SfGridColumns;
            foreach (var item in a)
            {
                this.SfGridColumns.Add(item);
            }

        }
        public void ExecuteShowMRPCommand(object obj)
        {


            ClearColumns();

            var F7input = F7Common.F7MRPCode(ShowDeleted);
            F7key = F7input.F7key;
            CollectionView = F7input.CollectionView;
            var a = F7input.SfGridColumns;
            foreach (var item in a)
            {
                this.sfGridColumns.Add(item);
            }

        }
        private void ExecuteShowMRPForecastDemand(object obj)
        {


            ClearColumns();

            var F7input = F7Common.F7MRPForecast(false);
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

            if (F7key == "MRPCode")
            {
                InputData.MRPID = (SelectedItem as MRPInputData).MRPID;
                InputData.MRPCode = (SelectedItem as MRPInputData).MRPCode;
                InputData.MRPDescr = (SelectedItem as MRPInputData).MRPDescr;
                InputData.IsDeleted = (SelectedItem as MRPInputData).IsDeleted;

                InputData.Inventory.InvCode = (SelectedItem as MRPInputData).Inventory.InvCode;
                InputData.Inventory.InvId = (SelectedItem as MRPInputData).Inventory.InvId;


                #region ItemsBom
                InputData.Items = CommonFunctions.GetItemData(false);
                #endregion
                #region Forecast
                InputData.Forecast = new ForecastInfoData();

                int ForId = (SelectedItem as MRPInputData).Forecast.ID;
                InputData.Forecast = CommonFunctions.GetForecastInfoChooserData(ForId, null);

                string ForCode = InputData.Forecast.ForCode;
                InputData.Forecast.DemandForecast = CommonFunctions.GetDemandForecast(ForCode);

                #region DatesStr List  

                int numberOfUniqueDates = InputData.Forecast.DemandForecast.Select(df => df.DateStr).Distinct().Count();


                InputData.Dates = new string[numberOfUniqueDates]; // auto thelei prosarmogh gia na ine dinamiko ,mexri stigmhs doulevei mono an valw monthy = px 6 (opws to paradigma )

                var distinctDateStrs = InputData.Forecast.DemandForecast
                    .Select(d => d.DateStr)
                    .Distinct()
                    .OrderBy(dateStr =>
                    {
                        // Replace "ItemCode" with the actual property name from your data model
                        return InputData.Forecast.DemandForecast
                            .Where(d => d.DateStr == dateStr)
                            .Select(d => d.Date)
                            .FirstOrDefault();
                    })
                    .ToArray();



                int index = 0;

                foreach (var datestr in distinctDateStrs)
                {
                    InputData.Dates[index] = datestr;
                    index++;
                }

                #endregion

                #endregion

                #region Inventory

                var InvCode = InputData.Inventory.InvCode;
                var invId = InputData.Inventory.InvId;
                InputData.Inventory = CommonFunctions.GetInventoryChooserData(invId, InvCode);
                InputData.Inventory.StockData= CommonFunctions.GetStockData(InvCode, false);
                #endregion
            }
            else if (F7key == "InvCode")
            {
                
                    var selectedInventory = new InventoryData();
                    selectedInventory.InvId = (SelectedItem as InventoryData).InvId;
                    selectedInventory.InvCode = (SelectedItem as InventoryData).InvCode;
                    selectedInventory.InvDescr = (SelectedItem as InventoryData).InvDescr;
                    selectedInventory.IsDeleted = (SelectedItem as InventoryData).IsDeleted;

                    InputData.Inventory = selectedInventory;
                    InputData.Inventory.StockData = CommonFunctions.GetStockData(selectedInventory.InvCode, false);

            }
            else if (F7key == "MRPForecast")
            {
                ForecastInfoData SelectedForecast = new ForecastInfoData();
                InputData.Forecast = new ForecastInfoData();

                SelectedForecast.ID = (SelectedItem as ForecastInfoData).ID;
                SelectedForecast.ForCode = (SelectedItem as ForecastInfoData).ForCode;
                SelectedForecast.ForDescr = (SelectedItem as ForecastInfoData).ForDescr;
                SelectedForecast.Notes = (SelectedItem as ForecastInfoData).Notes;
                SelectedForecast.TimeBucket = (SelectedItem as ForecastInfoData).TimeBucket;
                SelectedForecast.PeriodType = (SelectedItem as ForecastInfoData).PeriodType;
                SelectedForecast.PeriodNumber = (SelectedItem as ForecastInfoData).PeriodNumber;
                SelectedForecast.HoursPerTimeBucket = (SelectedItem as ForecastInfoData).HoursPerTimeBucket;
                SelectedForecast.NumberOfBuckets = (SelectedItem as ForecastInfoData).NumberOfBuckets;

                SelectedForecast.DateFrom = (SelectedItem as ForecastInfoData).DateFrom;
                SelectedForecast.DateTo = (SelectedItem as ForecastInfoData).DateTo;




                InputData.Forecast = SelectedForecast;
                InputData.Forecast.DemandForecast = CommonFunctions.GetDemandForecast(SelectedForecast.ForCode);

                #region DatesStr List  

                int numberOfUniqueDates = InputData.Forecast.DemandForecast.Select(df => df.DateStr).Distinct().Count();


                InputData.Dates = new string[numberOfUniqueDates]; // auto thelei prosarmogh gia na ine dinamiko ,mexri stigmhs doulevei mono an valw monthy = px 6 (opws to paradigma )

                var distinctDateStrs = InputData.Forecast.DemandForecast
                    .Select(d => d.DateStr)
                    .Distinct()
                    .OrderBy(dateStr =>
                    {
                        // Replace "ItemCode" with the actual property name from your data model
                        return InputData.Forecast.DemandForecast
                            .Where(d => d.DateStr == dateStr)
                            .Select(d => d.Date)
                            .FirstOrDefault();
                    })
                    .ToArray();



                int index = 0;

                foreach (var datestr in distinctDateStrs)
                {
                    InputData.Dates[index] = datestr;
                    index++;
                }

                #endregion




            }
        }

        #endregion

        #endregion
        #region InsertData For Optimisation
        public ICommand InsertDataCommand { get; }
        private void ExecuteInsertDataCommand(object obj)
        {
            #region Arxikopoihsh Dictionaries 

            InputData.Profit = new Dictionary<string, double>();
            InputData.TimeReq = new Dictionary<string, Dictionary<string, double>>();
            InputData.MachInstalled = new Dictionary<string, int>();
            InputData.MaxDemand = new Dictionary<(string, string), double>();

            #endregion

            #region Forecast format to date se string kai epe3ergasia gia diofretiko periodtype,periodnum,timebucket

            foreach (var a in InputData.Forecast.DemandForecast)
            {
                //Prosarmogh me vazh tis epiloges sto timebucket,periodtype kai prosarmogh se string gia to optimisation solver .Quarter1,q2,q3,q4
                //H mines opws vlepoume.Tha ftiaksw tool gia prosarmogh dates analoga me autes tis 2 epiloges .
                //Prepei na ginei loopa opou tha taksinomw opws prepei gia kathe date kai idos wste na paraxthei to antistoixo me to hardcoded data.

                var Date = a.Date; // Assuming Date is already a string

                var Demand = a.Demand;
                var Itemcode = a.Item.ItemCode;

                int loopCounter = 0;
                decimal quarterDemand = 0;
                var periodtype = InputData.Forecast.PeriodType;
                var periodnum = InputData.Forecast.PeriodNumber;
                var timeBucket = InputData.Forecast.TimeBucket;

                string formattedDate = a.DateStr;
                periodnum = 1;

                if (periodtype == BasicEnums.PeriodType.Yearly)
                {
                    if (periodnum == 1)
                    {
                        if (timeBucket == BasicEnums.Timebucket.Monthly)
                        {
                            formattedDate = DateTime.Parse(Date.ToString()).ToString("MM");

                        }

                        if (timeBucket == BasicEnums.Timebucket.Quarterly)
                        {
                            formattedDate = DateTime.Parse(Date.ToString()).ToString("MMM");

                            loopCounter++;
                            quarterDemand += Demand;

                            if (loopCounter % 3 == 0)
                            {
                                if (loopCounter / 3 == 1)
                                {
                                    formattedDate = "Q1";
                                }
                                else if (loopCounter / 3 == 2)
                                {
                                    formattedDate = "Q2";

                                }
                                else if (loopCounter / 3 == 3)
                                {
                                    formattedDate = "Q3";

                                }
                                else if (loopCounter / 3 == 4)
                                {
                                    formattedDate = "Q4";
                                    loopCounter = 0;

                                }
                            }

                        }

                        if (timeBucket == BasicEnums.Timebucket.Weekly)
                        {
                            loopCounter++;
                            formattedDate = $"WEEK{loopCounter++}";
                        }

                        if (periodnum > 1 && timeBucket == BasicEnums.Timebucket.Daily)
                        {
                            formattedDate = DateTime.Parse(Date.ToString()).ToString("dd/MM");
                        }
                    }


                }

                if (periodtype == BasicEnums.PeriodType.Monthly)
                {
                    if (periodnum > 1 && timeBucket == BasicEnums.Timebucket.Weekly)
                    {
                        loopCounter++;
                        formattedDate = $"WEEK{loopCounter++}";
                    }
                    if (periodnum > 1 && timeBucket == BasicEnums.Timebucket.Daily)
                    {
                        formattedDate = DateTime.Parse(Date.ToString()).ToString("dd/MM");
                    }
                }

                if (timeBucket == Timebucket.Monthly || timeBucket == Timebucket.Weekly || timeBucket == Timebucket.Daily)
                {
                    var key = (formattedDate, Itemcode);

                    // Add the data to the MaxDemand dictionary
                    InputData.MaxDemand[key] = (double)Demand;
                }

                if (timeBucket == Timebucket.Quarterly && loopCounter == 3)
                {

                    var key = (formattedDate, Itemcode);

                    // Add the data to the MaxDemand dictionary
                    InputData.MaxDemand[key] = (double)quarterDemand;
                    quarterDemand = 0;

                }


            }
            #endregion

            #region Time Required Dict /  TimeReq Dict


            foreach (var i in InputData.Items)
            {

                //InputData.Profit.Add(i.ItemCode, i.Profit);

                //var BomData = CommonFunctions.GetBomData(i.ItemCode, false);

                //foreach (var y in BomData)
                //{
                //    if (InputData.TimeReq.ContainsKey(y.Machine.MachCode))
                //    {
                //        InputData.TimeReq[y.Machine.MachCode][i.ItemCode] = Math.Round(y.ProductionTime, 2);
                //    }

                //}

            }
            #endregion

            #region Profit Dict / PriceList 

            foreach (var a in InputData.Items)
            {


                double Profit = double.Parse(a.SalesPrice.ToString());
                var Itemcode = a.ItemCode;

                InputData.Profit.Add(Itemcode, Profit);

            }
            #endregion

        }
        #endregion

        #region Calculate MRP
        public ICommand CalculateMrp { get; }

        private void ExecuteCalculateMrp(object obj)
        {
            ScriptEngine engine = Python.CreateEngine();
            engine.ExecuteFile(@"C:\Users\npoly\Source\Repos\MrpPlanner2");

            InputData.TotalDemandDict = new Dictionary<string, List<decimal>>(); 
            var Finished = CommonFunctions.CalculateMRP(InputData);

        }
        #endregion

        #region Diagrams
        #region F7
        public ICommand ShowSupplierInfoGridCommand { get; }

        public ICommand ShowItemGridCommand { get; }

        public ICommand ShowCustomerInfoGridCommand { get; }
        public ICommand ShowCityGridCommand { get; }


        private void ExecuteShowItemGridCommand(object obj)
        {


            ClearColumnsD();

            var F7input = F7Common.F7ItemMPSDiagrams();
            F7key = F7input.F7key;
            CollectionViewD = F7input.CollectionView;
            var a = F7input.SfGridColumns;
            foreach (var item in a)
            {
                this.SfGridColumnsD.Add(item);
            }

        }


        public void ChangeCanExecuteD(object obj)
        {
            if (F7key == "InvCode")
            {
                InputData.Diagram.Inventory = (SelectedItem2 as InventoryData);
            }

            if (F7key == "ItemCode")
            {
                InputData.Diagram.Item = (SelectedItem2 as ItemData);
            }
            if (F7key == "Customer")
            {
                InputData.Diagram.Customer = (SelectedItem2 as CustomerData);
            }
            if (F7key == "City")
            {
                InputData.Diagram.City = (SelectedItem2 as CityData);
            }
            if (F7key == "PriceList")
            {
                InputData.Diagram.PriceList = (SelectedItem2 as PriceListData);
            }
        }


        private ICommand rowDataCommandD { get; set; }
        public ICommand RowDataCommandD
        {
            get
            {
                return rowDataCommandD;
            }
            set
            {
                rowDataCommandD = value;
            }
        }

        protected void ClearColumnsD()
        {

            var ColumnsCount = this.SfGridColumnsD.Count();
            if (ColumnsCount != 0)
            {
                for (int i = 0; i < ColumnsCount; i++)
                {
                    this.SfGridColumnsD.RemoveAt(0);
                }
            }
        }
        #endregion







        #endregion

        #region CRUD  Commands

        #region Input 
        #region ADD
        public ICommand AddMRPCommand { get; }

        private void ExecuteAddMRPCommand(object obj)
        {
            if (string.IsNullOrWhiteSpace(InputData.MRPCode) || string.IsNullOrWhiteSpace(InputData.MRPDescr))
            {
                MessageBox.Show("Insert Code and Description");
            }

            else
            {
                int Flag = CommonFunctions.AddMRPInputData(InputData);
                if (Flag == 0)
                {
                    MessageBox.Show($"New MPS saved with Code: {InputData.MRPCode}");
                    ExecuteShowMRPCommand(obj);
                    InputData.MRPID = 0;
                    ExecuteRefreshInputCommand(obj);
                }
                else if (Flag == 1)
                {
                    MessageBox.Show($"The MPS with Code : {InputData.MRPCode} already exists");

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

            InputData = new MRPInputData();
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
            int Flag = CommonFunctions.SaveMRPInputData(InputData);


            if (Flag == 1)
            {
                MessageBox.Show($"Save/Update Completed for MPS with Code : {InputData.MRPCode}");
                ExecuteShowMRPCommand(obj);
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


            InputData = CommonFunctions.GetMRPChooserData(InputData.MRPID, InputData.MRPCode);
        }

        #endregion
        #endregion

        #region Output 



        #endregion

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


