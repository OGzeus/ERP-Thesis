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
using DevExpress.Xpo.DB;

namespace Erp.ViewModel.Manufacture
{
    public class MPSViewModel : ViewModelBase
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

        private MPSInputData inputdata;
        public MPSInputData InputData
        {
            get { return inputdata; }
            set
            {
                inputdata = value;
                INotifyPropertyChanged(nameof(InputData));


            }
        }


        private MPSOutputData outputdata;
        public MPSOutputData OutputData
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
        public MPSViewModel()
        {


            InputData = new MPSInputData();
            InputData.MPSCode = " ";
            InputData.ItemsDefaultSettings = false;
            InputData.Items = new ObservableCollection<ItemData>();
            InputData.PriceList = new PriceListData();
            InputData.StackPanelEnabled = false;
            InputData.MachRepairOnlyData = new ObservableCollection<MachineRepairData>();
            InputData.MachRepairDateData = new ObservableCollection<MachineRepairData>();

            OutputData = new MPSOutputData();
            OutputData.MPSOptResultsData = new ObservableCollection<MPSOptResultsData>();
            OutputData.MachRepairResultsData = new ObservableCollection<MachRepairResultsData>();


            this.sfGridColumns = new Columns();
            this.SfGridColumnsRepair = new Columns();

            InsertDataCommand = new RelayCommand2(ExecuteInsertDataCommand);
            CalculateMps = new RelayCommand2(ExecuteCalculateMps);
            ShowForecastDemand = new RelayCommand2(ExecuteShowForecastDemand);
            ShowPriceListGridCommand = new RelayCommand2(ExecuteShowPriceListGridCommand);

            ClearItemsInputCommand = new RelayCommand2(ExecuteClearItemsInputCommand);
            RefreshItemsInputCommand = new RelayCommand2(ExecuteRefreshItemsInputCommand);
            InsertDataItemsInputCommand = new RelayCommand2(ExecuteInsertDataItemsInputCommand);


            ChooseRepairOptionsCommand = new RelayCommand2(ExecuteChooseRepairOptionsButtonCommand);

            rowDataCommand = new RelayCommand2(ChangeCanExecute);
            ShowMPSCommand = new RelayCommand2(ExecuteShowMPSCommand);
            ShowItemInputGridCommand = new RelayCommand2(ExecuteShowItemInputGridCommand);
            #region Diagrams
            #region Κατασκευή Κριτιρίων Αναζήτησης και Δήλωση Button Commands

            InputData.Diagram = new InvDiagramsSearchData();
            InputData.Diagram.Item = new ItemData();
            this.SfGridColumnsD = new Columns();


            ShowItemGridCommand = new RelayCommand2(ExecuteShowItemGridCommand);
            CreateDiagramCommand = new RelayCommand2(ExecuteCreateDiagramCommand);
            AddMPSCommand = new RelayCommand2(ExecuteAddMPSCommand);

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



            #region Κατασκευη linechart με 3 γραμμές

            DiagramData = OutputData.MPSOptResultsData;

            // Create the series collection and add the three lines
            InputData.Diagram.SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Demand",
                    Values = new ChartValues<ObservableValue>(
                        DiagramData.Select(d => new ObservableValue(d.Demand))),
                    PointGeometry = null
                },
                new LineSeries
                {
                    Title = "Make",
                    Values = new ChartValues<ObservableValue>(
                        DiagramData.Select(d => new ObservableValue(d.Make))),
                    PointGeometry = null
                },
                new LineSeries
                {
                    Title = "Sell",
                    Values = new ChartValues<ObservableValue>(
                        DiagramData.Select(d => new ObservableValue(d.Sell))),
                    PointGeometry = null
                },
                new LineSeries
                {
                    Title = "Store",
                    Values = new ChartValues<ObservableValue>(
                        DiagramData.Select(d => new ObservableValue(d.Store))),
                    PointGeometry = null
                }
            };

            // Set the labels for the x-axis to the months in the data
            InputData.Diagram.Labels = DiagramData.Select(d => d.Date).ToArray();

            InputData.Diagram.YFormatter = value => value.ToString("N0");
            #endregion


            #region PrimaryMachines GetCommands
            InputData.PrimaryMachines = CommonFunctions.GetPrimaryMachineChooserData();
            InputData.Machines = CommonFunctions.GetNonPrimaryMachineChooserData();
            #endregion

            #endregion
        }

        #region Commands

        #region FlatData
        public ICommand ShowForecastDemand { get; }
        public ICommand ShowMPSCommand { get; }
        public ICommand ShowPriceListGridCommand { get; }
        public ICommand ShowItemInputGridCommand { get; }
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
        public void ExecuteShowMPSCommand(object obj)
        {


            ClearColumns();

            var F7input = F7Common.F7MPSCode(ShowDeleted);
            F7key = F7input.F7key;
            CollectionView = F7input.CollectionView;
            var a = F7input.SfGridColumns;
            foreach (var item in a)
            {
                this.sfGridColumns.Add(item);
            }

        }
        private void ExecuteShowForecastDemand(object obj)
        {


            ClearColumns();

            var F7input = F7Common.F7Forecast(false);
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
            if (F7key == "MPSForecast")
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

                SelectedForecast.DateFrom = (SelectedItem as ForecastInfoData).DateFrom;
                SelectedForecast.DateTo = (SelectedItem as ForecastInfoData).DateTo;

              


                InputData.Forecast = SelectedForecast;
                InputData.Forecast.DemandForecast = CommonFunctions.GetDemandForecast(SelectedForecast.ForCode);
                InputData.HoursPerMonth = InputData.Forecast.HoursPerTimeBucket;

                #region DatesStr List  

                int numberOfUniqueDates = InputData.Forecast.DemandForecast.Select(df => df.DateStr).Distinct().Count();


                InputData.DatesStr = new string[numberOfUniqueDates]; 
                InputData.Dates = new DateTime[numberOfUniqueDates]; 

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
                var distinctDates = InputData.Forecast.DemandForecast
    .Select(d => d.Date)
    .Distinct()
    .OrderBy(date =>
    {
        // Replace "ItemCode" with the actual property name from your data model
        return InputData.Forecast.DemandForecast
            .Where(d => d.Date == date)
            .Select(d => d.Date)
            .FirstOrDefault();
    })
    .ToArray();






                for (int i = 0; i < numberOfUniqueDates; i++)
                {
                    // Do something with 'i'
                    InputData.DatesStr[i] = distinctDateStrs[i];
                    InputData.Dates[i] = distinctDates[i];

                }
                #endregion




            }
            if (F7key == "MPSCode")
            {
                InputData.MPSId = (SelectedItem as MPSInputData).MPSId;
                InputData.MPSCode = (SelectedItem as MPSInputData).MPSCode;
                InputData.MPSDescr = (SelectedItem as MPSInputData).MPSDescr;
                InputData.NumberDatesOfRepairs = (SelectedItem as MPSInputData).NumberDatesOfRepairs;
                InputData.NumberOfRepairsOnly = (SelectedItem as MPSInputData).NumberOfRepairsOnly;
                InputData.ExistingSchedule = (SelectedItem as MPSInputData).ExistingSchedule;
                InputData.ItemsDefaultSettings = (SelectedItem as MPSInputData).ItemsDefaultSettings;
                InputData.IsDeleted = (SelectedItem as MPSInputData).IsDeleted;


                InputData.InvStoreTarget = (SelectedItem as MPSInputData).InvStoreTarget;
                InputData.HoldingCost = (SelectedItem as MPSInputData).HoldingCost;
                InputData.MaxInventory = (SelectedItem as MPSInputData).MaxInventory;


                InputData.Items = CommonFunctions.GetItemMPSInputChooserData();



                #region Forecast
                InputData.Forecast = new ForecastInfoData();

                int ForId = (SelectedItem as MPSInputData).Forecast.ID;
                InputData.Forecast = CommonFunctions.GetForecastInfoChooserData(ForId,null);

                string ForCode = InputData.Forecast.ForCode;
                InputData.Forecast.DemandForecast = CommonFunctions.GetDemandForecast(ForCode);
                InputData.HoursPerMonth = InputData.Forecast.HoursPerTimeBucket;

                #region DatesStr List  

                int numberOfUniqueDates = InputData.Forecast.DemandForecast.Select(df => df.DateStr).Distinct().Count();


                InputData.DatesStr = new string[numberOfUniqueDates]; // auto thelei prosarmogh gia na ine dinamiko ,mexri stigmhs doulevei mono an valw monthy = px 6 (opws to paradigma )
                InputData.Dates = new DateTime[numberOfUniqueDates]; // auto thelei prosarmogh gia na ine dinamiko ,mexri stigmhs doulevei mono an valw monthy = px 6 (opws to paradigma )

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
                var distinctDates = InputData.Forecast.DemandForecast
    .Select(d => d.Date)
    .Distinct()
    .OrderBy(date =>
    {
        // Replace "ItemCode" with the actual property name from your data model
        return InputData.Forecast.DemandForecast
            .Where(d => d.Date == date)
            .Select(d => d.Date)
            .FirstOrDefault();
    })
    .ToArray();
                for (int i = 0; i < numberOfUniqueDates; i++)
                {
                    // Do something with 'i'
                    InputData.DatesStr[i] = distinctDateStrs[i];
                    InputData.Dates[i] = distinctDates[i].Date;

                }
                #endregion

                #endregion

                #region PriceList

                InputData.PriceList.Id = (SelectedItem as MPSInputData).PriceList.Id;
                InputData.PriceList.Code = (SelectedItem as MPSInputData).PriceList.Code;

                InputData.PriceList = CommonFunctions.GetPriceListData(InputData.PriceList);
                InputData.PriceList.ItemsInfo = CommonFunctions.GetPriceListItemData(InputData.PriceList, false);

                ExecuteInsertDataItemsInputCommand(InputData);
                #endregion
                if (InputData.NumberDatesOfRepairs == true)
                {
                    InputData.MachRepairDateData = CommonFunctions.GetMPSMachRepairData(InputData);
                    ExecuteChooseRepairOptionsCommand(InputData);

                }
                else if (InputData.NumberOfRepairsOnly == true)
                {
                    InputData.MachRepairOnlyData = CommonFunctions.GetMPSMachRepairData(InputData);
                    ExecuteChooseRepairOptionsCommand(InputData);
                }
                else if (InputData.ExistingSchedule == true)
                {

                }
            }
            if (F7key == "PriceList")
            {
                InputData.PriceList.Id = (SelectedItem as PriceListData).Id;
                InputData.PriceList.Code = (SelectedItem as PriceListData).Code;
                InputData.PriceList.Id = (SelectedItem as PriceListData).Id;
                InputData.PriceList.Retail = (SelectedItem as PriceListData).Retail;
                InputData.PriceList.Wholesale = (SelectedItem as PriceListData).Wholesale;
                InputData.PriceList.DateStart = (SelectedItem as PriceListData).DateStart;
                InputData.PriceList.DateEnd = (SelectedItem as PriceListData).DateEnd;

                InputData.PriceList.ItemsInfo = CommonFunctions.GetPriceListItemData(InputData.PriceList, false);

            }
        }

        #endregion

        #region Insert ItemData/PriceListData
        public ICommand ClearItemsInputCommand { get; }
        public ICommand RefreshItemsInputCommand { get; }
        public ICommand InsertDataItemsInputCommand { get; }

        private void ExecuteInsertDataItemsInputCommand(object obj)
        {
            if(InputData.ItemsDefaultSettings == true)
            {
                foreach (var item in InputData.Items)
                {
                    item.StoreTarget = InputData.InvStoreTarget;
                    item.MaxInventory = InputData.MaxInventory;
                    item.HoldingCost = (float)InputData.HoldingCost;
                }
            }
            #region Profit Dict 

            foreach (var itemInfo in InputData.PriceList.ItemsInfo)
            {
                var matchingItem = InputData.Items.FirstOrDefault(item => item.ItemCode == itemInfo.Item.ItemCode);
                if (matchingItem != null)
                {
                    matchingItem.SalesPrice = itemInfo.SalesPrice;
                }


            }

            #endregion




        }




        private void ExecuteClearItemsInputCommand(object commandParameter)
        {
            //ChooserData.ItemCode = "";
            //ChooserData.ItemDescr = "";
            //ChooserData.ItemId = -1;

            //ChooserData = new ItemData();

            //FlatData.ItemCode = "";
            //FlatData.ItemDescr = "";
            //FlatData.ItemId = 0;

            //FlatData.MesUnit = "";
            //FlatData.ItemType = "";
            //FlatData.Assembly = 99;
            //FlatData.CanBeProduced = false;
            //FlatData.InputOrderFlag = false;
            //FlatData.OutputOrderFlag = false;
            //FlatData2 = new ObservableCollection<BomData>();
            //ProcessData = new ObservableCollection<ItemProcessData>();

        }







        private void ExecuteRefreshItemsInputCommand(object commandParameter)
        {

            //FlatData = CommonFunctions.GetItemData(ChooserData);


        }


        #endregion
        #region MACHINE REPAIR INPUT

        #region CRUD  Commands
        #region Clear

        private ViewModelCommand _ClearMachRepairCommand;

        public ICommand ClearMachRepairCommand
        {
            get
            {
                if (_ClearMachRepairCommand == null)
                {
                    _ClearMachRepairCommand = new ViewModelCommand(ExecuteClearMachRepairCommand);
                }

                return _ClearMachRepairCommand;
            }
        }

        private void ExecuteClearMachRepairCommand(object commandParameter)
        {

            InputData.MachRepairOnlyData = new ObservableCollection<MachineRepairData>();
            InputData.MachRepairDateData = new ObservableCollection<MachineRepairData>();
            ExecuteChooseRepairOptionsCommand(InputData);
            ExecuteMachRepairInput(InputData);
        }

        #endregion
        #region Refresh

        private ViewModelCommand _RefreshMachRepairCommand;

        public ICommand RefreshMachRepairCommand
        {
            get
            {
                if (_RefreshMachRepairCommand == null)
                {
                    _RefreshMachRepairCommand = new ViewModelCommand(ExecuteRefreshMachRepairCommand);
                }

                return _RefreshMachRepairCommand;
            }
        }

        private void ExecuteRefreshMachRepairCommand(object commandParameter)
        {


            if (InputData.NumberDatesOfRepairs == true)
            {
                InputData.MachRepairDateData = CommonFunctions.GetMPSMachRepairData(InputData);
                ExecuteChooseRepairOptionsCommand(InputData);
                ExecuteMachRepairInput(InputData);

            }
            else if (InputData.NumberOfRepairsOnly == true)
            {
                InputData.MachRepairOnlyData = CommonFunctions.GetMPSMachRepairData(InputData);
                ExecuteChooseRepairOptionsCommand(InputData);
                ExecuteMachRepairInput(InputData);

            }
            else if (InputData.ExistingSchedule == true)
            {

            }
        }

        #endregion

        #endregion
        public ICommand InsertDataCommand { get; }
        public ICommand ChooseRepairOptionsCommand { get; }


        //2 epiloges ;1h harcoded ite me dates ite xwris dates 2h epilogh apo to hmerologio mhxanwn
        private void ExecuteChooseRepairOptionsCommand(object obj)
        {

            if (InputData.NumberDatesOfRepairs == true) //HardCoded Repair FactoryPlanning 1
            {





                    for (int j = 0; j < InputData.PrimaryMachines.Count; j++)
                    {
                        for (int i = 0; i < InputData.Dates.Length; i++) // Start from the second month (index 1)
                        {
                            var currentRow = InputData.MachRepairDateData.SingleOrDefault(b => b.MPSId == InputData.MPSId
                                                                                           && b.Mach.MachID == InputData.PrimaryMachines[j].MachID && b.RepairDateStr == InputData.DatesStr[i]);
                            if (currentRow == null)
                            {                       

                            MachineRepairData singleDataRecord = new MachineRepairData();
                            singleDataRecord.Mach = new MachineData();
                            singleDataRecord.MPSId = InputData.MPSId;

                            // Add the data to the MaxDemand dictionary
                            var repairvalue = 0;
                            singleDataRecord.NumberOfRepairsMPS = repairvalue;
                            singleDataRecord.Mach = InputData.PrimaryMachines[j];
                            singleDataRecord.RepairDateStr = InputData.DatesStr[i];
                            singleDataRecord.RepairDate = InputData.Dates[i];

                            InputData.MachRepairDateData.Add(singleDataRecord);

                            }
                        }

                    }
                


            }
            else if (InputData.NumberOfRepairsOnly == true) //SoftCoded Repair FactoryPlanning 2
            {



                    for (int j = 0; j < InputData.PrimaryMachines.Count; j++)
                    {
                        var currentRow = InputData.MachRepairOnlyData.SingleOrDefault(b => b.MPSId == InputData.MPSId
                                                               && b.Mach.MachID == InputData.PrimaryMachines[j].MachID );
                        if (currentRow == null)
                        {
                            MachineRepairData singleDataRecord = new MachineRepairData();
                            singleDataRecord.Mach = new MachineData();
                            singleDataRecord.MPSId = InputData.MPSId;


                            // Add the data to the MaxDemand dictionary
                            //singleDataRecord.NumberOfRepairs = Filter.MaxDemand[key];
                            var repairvalue = 0;
                            singleDataRecord.NumberOfRepairsMPS = repairvalue;
                            singleDataRecord.Mach = InputData.PrimaryMachines[j];

                            InputData.MachRepairOnlyData.Add(singleDataRecord);
                        }


                    }
                
            }
            else if (InputData.ExistingSchedule == true) //SoftCoded Repair FactoryPlanning 2
            {


            }

        }

        private void ExecuteChooseRepairOptionsButtonCommand(object obj)
        {

            ExecuteChooseRepairOptionsCommand(InputData);
            ExecuteMachRepairInput(InputData);


        }
        private void ExecuteMachRepairInput(MPSInputData inputData)
        {

            ClearColumns();

            var F7input = F7Common.F7MachRepairInput(inputData);
            F7key = F7input.F7key;
            CollectionView = F7input.CollectionView;
            var a = F7input.SfGridColumns;
            foreach (var item in a)
            {
                this.SfGridColumns.Add(item);
            }

        }
        #endregion
        #endregion
        #region InsertData For Optimisation

        private void ExecuteInsertDataCommand(object obj)
        {
            #region Arxikopoihsh Dictionaries 

            InputData.ProfitDict = new Dictionary<string, double>();
            InputData.HoldingCostDict = new Dictionary<string, double>();
            InputData.StoreTargetDict = new Dictionary<string, double>();
            InputData.MaxInventoryDict = new Dictionary<string, double>();

            InputData.TimeReq = new Dictionary<string, Dictionary<string, double>>();
            InputData.MachInstalled = new Dictionary<string, int>();
            InputData.MachDownReq = new Dictionary<(string, string), int>();
            InputData.MachDownReq2 = new Dictionary<string, int>();
            InputData.AdvancedMachDownReq = new Dictionary<(string, string), int>();
            InputData.MaintenanceRate = new Dictionary<string, double>();
            InputData.TotalOnlineMach = new Dictionary<(string, string), double>(); //Production capacity per machine + total capacity
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
            foreach (var i in InputData.PrimaryMachines)
            {
                if (i.PrimaryModel == true)
                {
                    InputData.TimeReq.Add(i.MachCode, new Dictionary<string, double>());
                }
            }

            foreach (var i in InputData.Items)
            {

                //InputData.Profit.Add(i.ItemCode, i.Profit);

                var ProcessData = CommonFunctions.GetPPFData(i.ItemCode, false);

                foreach (var y in ProcessData)
                {
                    if (InputData.TimeReq.ContainsKey(y.Machine.MachCode))
                    {
                        InputData.TimeReq[y.Machine.MachCode][i.ItemCode] = Math.Round(y.ProductionTime, 2);
                    }

                }

            }
            #endregion
            #region Machines /MachDownReq,AdvancedMachDownReq,MaintenanceRate,TotalOnlineMach Dicts

            if (InputData.NumberOfRepairsOnly == true) //SoftCoded Repair FactoryPlanning 2
            {
                for (int j = 0; j < InputData.MachRepairOnlyData.Count; j++)
                {
                    MachineRepairData repairData = InputData.MachRepairOnlyData[j];
                    string MachCode = repairData.Mach.MachCode; // Assuming MachineName is a property of MachineData

                    // Calculate the repair requirements for this machine (modify this logic as needed)
                    int repairRequirements = repairData.NumberOfRepairsMPS;

                    // Add the calculated repair requirements to the dictionary
                    InputData.MachDownReq2[MachCode] = repairRequirements;
                }
            }
            else  if (InputData.NumberDatesOfRepairs == true)
            {
                foreach (MachineRepairData repairData in InputData.MachRepairDateData)
                {
                    string machineName = repairData.Mach.MachCode; // Assuming MachineName is a property of MachineData
                    string repairDate = repairData.RepairDateStr; // Assuming RepairDate is a property of MachineRepairData

                    // Calculate the repair requirements for this combination of month and machine (modify this logic as needed)
                    int repairRequirements = repairData.NumberOfRepairsMPS;

                    // Add the calculated repair requirements to the dictionary
                    InputData.MachDownReq[(repairDate, machineName)] = repairRequirements;
                }


            }

                foreach (var i in InputData.PrimaryMachines)
            {
                if (i.PrimaryModel == true)
                {
                    InputData.MachInstalled.Add(i.MachCode, i.NumberOfMachines);
                }
            }

            foreach (var primary in InputData.PrimaryMachines)
            {
                var NumberOfMachines = primary.NumberOfMachines;
                var ProductionRate = primary.ProductionRate;
                var TotalProduction = primary.NumberOfMachines * ProductionRate;

                var AverageRepairTime = primary.AverageRepairTime;
                var MachCode = primary.MachCode;

                InputData.MaintenanceRate.Add(MachCode, AverageRepairTime);

            }
            foreach (var date in InputData.DatesStr)
            {
                foreach (var primary in InputData.PrimaryMachines)
                {
                    int offlinemachines = 0;

                    var NumberOfMachines = primary.NumberOfMachines;
                    for (int i = 0; i < NumberOfMachines; i++)
                    {
                        var currentmach = InputData.Machines[i];
                        if (date == currentmach.NextMaintenance.ToString())
                        {
                            offlinemachines++;

                        }
                    }

                    if (offlinemachines != 0)
                    {
                        if (InputData.Forecast.TimeBucket == Timebucket.Daily)
                        {
                            var key = (date, primary.MachCode);
                            // Add the data to the MaxDemand dictionary
                            InputData.AdvancedMachDownReq[key] = offlinemachines;
                        }






                    }

                }
            }

            #endregion

            #region Profit ,Holding Cost,Max Inventory,Store Target

            //foreach (var a in InputData.PriceList.ItemsInfo)
            //{
            //    //Prosarmogh me vazh tis epiloges sto timebucket,periodtype kai prosarmogh se string gia to optimisation solver .Quarter1,q2,q3,q4
            //    //H mines opws vlepoume.Tha ftiaksw tool gia prosarmogh dates analoga me autes tis 2 epiloges .
            //    //Prepei na ginei loopa opou tha taksinomw opws prepei gia kathe date kai idos wste na paraxthei to antistoixo me to hardcoded data.

            //    double Profit = double.Parse(a.SalesPrice.ToString());
            //    var Itemcode = a.Item.ItemCode;

            //    InputData.Profit.Add(Itemcode, Profit);

            //}
            foreach (var a in InputData.Items)
            {
                double Profit = double.Parse(a.Profit.ToString());
                double HoldingCost = double.Parse(a.HoldingCost.ToString());
                double MaxInventory = double.Parse(a.MaxInventory.ToString());
                double StoreTarget = double.Parse(a.StoreTarget.ToString());

                var Itemcode = a.ItemCode;

                InputData.ProfitDict.Add(Itemcode, Profit);
                InputData.HoldingCostDict.Add(Itemcode, HoldingCost);
                InputData.MaxInventoryDict.Add(Itemcode, MaxInventory);
                InputData.StoreTargetDict.Add(Itemcode, StoreTarget);

            }
            #endregion

        }
        #endregion

        #region Calculate MPS
        public ICommand CalculateMps { get; }


        private void ExecuteCalculateMps(object obj)
        {
            if (InputData.NumberDatesOfRepairs == true) //HardCoded Repair FactoryPlanning 1
            {
                OutputData = CommonFunctions.CalculateMPS1(InputData);
            }
            else if (InputData.NumberOfRepairsOnly == true)
            {
                OutputData = CommonFunctions.CalculateMPS2(InputData);
            }
        }
        #endregion

        #region Diagrams
        #region F7
        public ICommand ShowInventoryGridCommand { get; }
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





        #region Συνάρτηση Διαγράμμος

        public List<string> Stock { get; set; }
        public ICommand CreateDiagramCommand { get; }

        private void ExecuteCreateDiagramCommand(object obj)
        {


            #region Κατασκευη linechart με 3 γραμμές
            if (!string.IsNullOrWhiteSpace(InputData.Diagram.Item.ItemCode))
            {
                DiagramData = OutputData.MPSOptResultsData.Where(item => item.ItemCode == InputData.Diagram.Item.ItemCode).ToObservableCollection();

            }
            else
            {
                MessageBox.Show("Error: ItemCode is null or whitespace.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }


            InputData.Diagram.SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Demand",
                    Values = new ChartValues<ObservableValue>(
                        DiagramData.Select(d => new ObservableValue(d.Demand))),
                },
                new ColumnSeries
                {
                    Title = "Make",
                    Values = new ChartValues<double>(DiagramData.Select(d => Convert.ToDouble(d.Make))),
                },

                new ColumnSeries
                {
                    Title = "Sell",
                    Values = new ChartValues<ObservableValue>(
                        DiagramData.Select(d => new ObservableValue(d.Sell))),
                },
                new ColumnSeries
                {
                    Title = "Store",
                    Values = new ChartValues<ObservableValue>(
                        DiagramData.Select(d => new ObservableValue(d.Store))),

                }
            };

            // Set the labels for the x-axis to the months in the data
            InputData.Diagram.Labels = DiagramData.Select(d => d.Date).ToArray();

            InputData.Diagram.YFormatter = value => value.ToString("N0");
            #endregion

            // Set the labels for the x-axis to the months in the data



        }
        #endregion

        #endregion

        #region CRUD  Commands

        #region Input 
        #region ADD
        public ICommand AddMPSCommand { get; }

        private void ExecuteAddMPSCommand(object obj)
        {
            if (string.IsNullOrWhiteSpace(InputData.MPSCode) || string.IsNullOrWhiteSpace(InputData.MPSDescr))
            {
                MessageBox.Show("Insert Code and Description");
            }

            else
            {
                int Flag = CommonFunctions.AddMpsInputData(InputData);
                if (Flag == 0)
                {
                    MessageBox.Show($"New MPS saved with Code: {InputData.MPSCode}");
                    ExecuteShowMPSCommand(obj);
                    InputData.MPSId = 0;
                    ExecuteRefreshInputCommand(obj);
                }
                else if (Flag == 1)
                {
                    MessageBox.Show($"The MPS with Code : {InputData.MPSCode} already exists");

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

            InputData = new MPSInputData();
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
            int Flag = CommonFunctions.SaveMpsInputData(InputData);

            if (Flag == 1)
            {
                MessageBox.Show($"Save/Update Completed for MPS with Code : {InputData.MPSCode}");
                ExecuteShowMPSCommand(obj);
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

            InputData = CommonFunctions.GetMPSChooserData(InputData.MPSId,InputData.MPSCode,InputData);

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
