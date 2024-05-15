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
using Erp.Model.Data_Analytics.Forecast;
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
using System.Diagnostics;
using Syncfusion.Windows.Shared;
using System.Windows.Markup;
using static OfficeOpenXml.ExcelErrorValue;

namespace Erp.ViewModel.Manufacture
{
    public class MRPViewModel : ViewModelBase
    {

        #region DataProperties

        private int _selectedTabIndex;

        public int SelectedTabIndex
        {
            get { return _selectedTabIndex; }
            set
            {
                if (_selectedTabIndex != value)
                {
                    _selectedTabIndex = value;
                    INotifyPropertyChanged(nameof(SelectedTabIndex));
                }
            }
        }
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
            InputData.EndItems = new ObservableCollection<ItemData>();
            InputData.BomItems = new ObservableCollection<ItemData>();

            InputData.Inventory = new InventoryData();
            InputData.Inventory.StockData = new ObservableCollection<StockData>();

            InputData.StackPanelEnabled = false;
            OutputData = new MRPOutputData();
            OutputData.Diagram1 = new DiagramsMRPData();
            OutputData.Diagram2 = new DiagramsMRPData();

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
            OutputData.Diagram1 = new DiagramsMRPData();
            OutputData.Diagram1.Item = new ItemData();
            OutputData.Diagram2 = new DiagramsMRPData();
            OutputData.Diagram2.Workcenter = new WorkcenterData();

            OutputData.Diagram2.Workcenter = new WorkcenterData();
            this.SfGridColumnsD = new Columns();



            ShowItemGridCommand = new RelayCommand2(ExecuteShowItemGridCommand);
            ShowWorkcenterGridCommand = new RelayCommand2(ExecuteShowWorkcenterGridCommand);

            CreateDiagram1Command = new RelayCommand2(ExecuteCreateDiagram1Command);
            CreateDiagram2Command = new RelayCommand2(ExecuteCreateDiagram2Command);
            rowDataCommandD = new RelayCommand2(ChangeCanExecuteD);




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

            var F7input = F7Common.F7MRPItemsInfo();
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

            var F7input = F7Common.F7ItemMRPInput();
            F7key = F7input.F7key;
            CollectionView = CollectionViewSource.GetDefaultView(InputData.EndItems.ToList());
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
                InputData.W = 5;

                #region Forecast
                InputData.Forecast = new ForecastInfoData();

                int ForId = (SelectedItem as MRPInputData).Forecast.ID;
                InputData.Forecast = CommonFunctions.GetForecastInfoChooserData(ForId, null);

                string ForCode = InputData.Forecast.ForCode;
                InputData = CommonFunctions.GetDemandFItemData(InputData, ForCode);

                foreach(var item in InputData.EndItems)
                {
                    item.Bom = new ObservableCollection<BomData>();
                    item.Bom = CommonFunctions.GetBomData(item.ItemCode,false);
                }
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

                #region ItemsBoms

                var DistinctBomItems = GetDistinctBOMItems(InputData);

                foreach (var code in DistinctBomItems)
                {
                    ItemData row = new ItemData();
                    row = CommonFunctions.GetItemChooserData(0, code);

                    InputData.BomItems.Add(row);
                }

                #endregion

                #region Inventory

                var InvCode = InputData.Inventory.InvCode;
                var invId = InputData.Inventory.InvId;
                InputData.Inventory = CommonFunctions.GetInventoryChooserData(invId, InvCode);
                InputData.Inventory.StockData= CommonFunctions.GetMRPItemData(InvCode);



                foreach (var item in InputData.BomItems)
                {

                    if (!InputData.Inventory.StockData.Any(stock => stock.StockItem.ItemCode == item.ItemCode))
                    {
                        var newitem = new StockData();
                        newitem.StockItem = item;
                        newitem.Stock = 0;
                        newitem.InvMax = 1050;
                        newitem.InvMin = 0;

                        InputData.Inventory.StockData.Add(newitem);
                    }

                }
                foreach (var item in InputData.EndItems)
                {

                    if (!InputData.Inventory.StockData.Any(stock => stock.StockItem.ItemCode == item.ItemCode))
                    {
                        var newitem = new StockData();
                        newitem.StockItem = item;
                        newitem.Stock = 0;
                        newitem.InvMax = 1050;
                        newitem.InvMin = 0;

                        InputData.Inventory.StockData.Add(newitem);
                    }

                }
                // Create a list of item codes from BomItems and EndItems
                var bomAndEndItemCodes = InputData.BomItems.Select(item => item.ItemCode)
                                        .Union(InputData.EndItems.Select(item => item.ItemCode))
                                        .ToList();

                // Remove items from StockData that are not in bomAndEndItemCodes
                // Filter StockData based on bomAndEndItemCodes and assign the result back
                InputData.Inventory.StockData = new ObservableCollection<StockData>(
                    InputData.Inventory.StockData.Where(stock => bomAndEndItemCodes.Contains(stock.StockItem.ItemCode))
                );

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


                foreach (var item in InputData.BomItems)
                {

                    if (!InputData.Inventory.StockData.Any(stock => stock.StockItem.ItemCode == item.ItemCode))
                    {
                        var newitem = new StockData();
                        newitem.StockItem = item;
                        newitem.Stock = 0;
                        newitem.InvMax = 1050;
                        newitem.InvMin = 0;

                        InputData.Inventory.StockData.Add(newitem);
                    }

                }
                foreach (var item in InputData.EndItems)
                {

                    if (!InputData.Inventory.StockData.Any(stock => stock.StockItem.ItemCode == item.ItemCode))
                    {
                        var newitem = new StockData();
                        newitem.StockItem = item;
                        newitem.Stock = 0;
                        newitem.InvMax = 1050;
                        newitem.InvMin = 0;

                        InputData.Inventory.StockData.Add(newitem);
                    }

                }
                // Create a list of item codes from BomItems and EndItems
                var bomAndEndItemCodes = InputData.BomItems.Select(item => item.ItemCode)
                                        .Union(InputData.EndItems.Select(item => item.ItemCode))
                                        .ToList();

                // Remove items from StockData that are not in bomAndEndItemCodes
                // Filter StockData based on bomAndEndItemCodes and assign the result back
                InputData.Inventory.StockData = new ObservableCollection<StockData>(
                    InputData.Inventory.StockData.Where(stock => bomAndEndItemCodes.Contains(stock.StockItem.ItemCode))
                );
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
                InputData.Forecast.DemandForecast = new ObservableCollection<DemandForecastData>();
                InputData.EndItems = new ObservableCollection<ItemData>();
                InputData = CommonFunctions.GetDemandFItemData(InputData,SelectedForecast.ForCode);
                foreach (var item in InputData.EndItems)
                {
                    item.Bom = new ObservableCollection<BomData>();
                    item.Bom = CommonFunctions.GetBomData(item.ItemCode, false);
                }
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
            InputData.Workcenters = new ObservableCollection<WorkcenterData>();

            InputData.T = new int();
            InputData.P = new int();
            InputData.Q = new int();
            InputData.Pw = new Dictionary<string, List<string>>();
            InputData.Qw = new Dictionary<string, List<string>>();
            InputData.Dit = new Dictionary<(string, string), double>();
            InputData.Ci = new Dictionary<string, List<string>>();
            InputData.Rij = new Dictionary<(string, string),double>();
            InputData.Miwt = new Dictionary<(string, string,string), double>();
            InputData.Awt = new Dictionary<(string, string), double>();
            InputData.Uiw = new Dictionary<(string, string), double>();
            InputData.Hi = new Dictionary<string, double>();
            InputData.Gi = new Dictionary<string, double>();
            InputData.I0W = new Dictionary<string, string>();

            InputData.Ii = new Dictionary<string, (double, double)>();
            InputData.Imax_min = new Dictionary<string, (double, double)>();


            #endregion

            Random random = new Random();

            #region T,P,Q,W ,Hi_Dict,Gi_Dict

            InputData.T = InputData.Forecast.NumberOfBuckets;
            InputData.P = InputData.EndItems.Count;
            var DistinctBomItems = GetDistinctBOMItems(InputData);
            InputData.Q = DistinctBomItems.Count + InputData.P;

            #region  Bom Items + Hi,Gi

            foreach (var code in DistinctBomItems)
            {
                ItemData row = new ItemData();
                row = CommonFunctions.GetItemChooserData(0, code);

                //InputData.BomItems.Add(row);

                InputData.Hi.Add(row.ItemCode, Math.Round(row.HoldingCost,2));
                InputData.Gi.Add(row.ItemCode, Math.Round(row.HoldingCost*100,2));
            }

            #endregion

            #endregion

            #region Dit

            foreach (var a in InputData.Forecast.DemandForecast)
            {          
                var Demand = a.Demand;
                var Itemcode = a.Item.ItemCode;
                string formattedDate = a.DateStr;
                var key = (Itemcode, formattedDate);

                // Add the data to the Dit dictionary
                InputData.Dit[key] = (double)Demand;              

            }

            foreach (var date in InputData.Dates)
            {
                foreach (var code in DistinctBomItems)
                {
                    var key = (code, date);
                    if (!InputData.Dit.ContainsKey(key))
                    {
                        InputData.Dit[key] = 0;
                    }

                }
            }
            #endregion

            #region Ci,Rij, Hi and Gi for End Items

            // Assuming Ci is already initialized somewhere in your code

            // Iterate through InputData.Items to populate Ci
            foreach (var item in InputData.EndItems)
            {
                // Check if the item is an end item
                if (item.Bom.Count != 0)
                {
                    // Initialize the list of subcomponents for this end item
                    List<string> subcomponents = new List<string>();

                    // Iterate through the BOMData of this end item to get subcomponents and their quantities
                    foreach (var row in item.Bom)
                    {
                        // Add the component to the list of subcomponents
                        subcomponents.Add(row.BomItem.ItemCode);

                        // Add the relationship to the Rij dictionary
                        InputData.Rij.Add((item.ItemCode, row.BomItem.ItemCode), Math.Round((double)row.BomPercentage,2));
                        //ChatGpthere

                    }

                    // Add the end item and its subcomponents to the Ci dictionary
                    InputData.Ci.Add(item.ItemCode, subcomponents);
                    InputData.Hi.Add(item.ItemCode, Math.Round(item.HoldingCost, 2));
                    InputData.Gi.Add(item.ItemCode, Math.Round(item.HoldingCost * 100, 2));
                }

            }
            #endregion

            #region Pw,Qw

            // Initialize dictionaries to store end-items and products for each work center
            InputData.Pw = new Dictionary<string, List<string>>();
            InputData.Qw = new Dictionary<string, List<string>>();

            // Determine the number of end items and BOM items based on the total work centers (W)
            int numberOfEndItems = (int)Math.Ceiling((double)InputData.W * 0.6); // 60% for end items
            int numberOfBomItems = InputData.W - numberOfEndItems; // Remaining for BOM items

            foreach (var workCenter in Enumerable.Range(1, InputData.W))
            {
                WorkcenterData workcenter = new WorkcenterData();

                // Determine if this workstation will contain only end items or only BOM items
                bool isEndItemWorkstation = numberOfEndItems > 0 ? true : false;

                // Initialize the list of products for this work center
                List<string> productsForWorkCenter = new List<string>();

                if (isEndItemWorkstation)
                {
                    // Workstation contains only end items
                    var endItems = InputData.EndItems.Select(e => e.ItemCode).ToList();

                    for (int i = 0; i < numberOfEndItems; i++)
                    {
                        // Randomly choose an end item
                        int randomEndItemIndex = random.Next(0, endItems.Count);
                        string selectedProduct = endItems[randomEndItemIndex];

                        // Add the selected end item to the list of products for this work center
                        productsForWorkCenter.Add(selectedProduct);

                        // Remove the selected end item to prevent duplicate assignment
                        endItems.RemoveAt(randomEndItemIndex);
                    }

                    // Decrement the count of end items
                    numberOfEndItems--;

                    // Add the list of end items to the Pw dictionary
                    InputData.Pw.Add($"WorkCenter{workCenter}", productsForWorkCenter);
                    workcenter.WorkCode = $"WorkCenter{workCenter}";
                }
                else
                {
                    // Workstation contains only BOM items
                    var bomItems = InputData.BomItems.Select(b => b.ItemCode).ToList();

                    for (int i = 0; i < numberOfBomItems; i++)
                    {
                        // Randomly choose a BOM item
                        int randomBomItemIndex = random.Next(0, bomItems.Count);
                        string selectedProduct = bomItems[randomBomItemIndex];

                        // Add the selected BOM item to the list of products for this work center
                        productsForWorkCenter.Add(selectedProduct);

                        // Remove the selected BOM item to prevent duplicate assignment
                        bomItems.RemoveAt(randomBomItemIndex);
                    }

                    // Decrement the count of BOM items
                    numberOfBomItems--;

                    // Add the list of BOM items to the Qw dictionary
                    InputData.Qw.Add($"WorkCenter{workCenter}", productsForWorkCenter);
                    workcenter.WorkCode = $"WorkCenter{workCenter}";
                }
                InputData.Workcenters.Add(workcenter);
            }

            // Ensure that each product is processed on at least two work centers
            foreach (var product in InputData.EndItems.Select(e => e.ItemCode))
            {
                EnsureProductAssignedToTwoWorkCenters(InputData.Pw, product);
            }

            foreach (var product in InputData.BomItems.Select(b => b.ItemCode))
            {
                EnsureProductAssignedToTwoWorkCenters(InputData.Qw, product);
            }

            #endregion

            // Function to ensure that each product is assigned to at least two work centers
            void EnsureProductAssignedToTwoWorkCenters(Dictionary<string, List<string>> workCenterProducts, string product)
            {
                // Check if the product is assigned to only one work center
                if (workCenterProducts.Count(wp => wp.Value.Contains(product)) < 2)
                {
                    // Find another work center to assign the product
                    foreach (var workCenter in workCenterProducts.Keys)
                    {
                        if (!workCenterProducts[workCenter].Contains(product))
                        {
                            workCenterProducts[workCenter].Add(product);
                            break;
                        }
                    }
                }
            }


            #region I0W
            foreach (var workCenter in InputData.Pw.Keys)
            {
                // Get the list of products that can be processed at this work center
                var productsForWorkCenter = InputData.Pw[workCenter];

                // Ensure there are products available for this work center
                if (productsForWorkCenter.Count > 0)
                {
                    // Randomly select a product from the list
                    string randomProduct = productsForWorkCenter[random.Next(productsForWorkCenter.Count)];

                    // Assign the random product to the work center
                    InputData.I0W[workCenter] = randomProduct;
                }
            }

            foreach (var workCenter in InputData.Qw.Keys)
            {
                // Get the list of products that can be processed at this work center
                var productsForWorkCenter = InputData.Qw[workCenter];

                // Ensure there are products available for this work center
                if (productsForWorkCenter.Count > 0)
                {
                    // Randomly select a product from the list
                    string randomProduct = productsForWorkCenter[random.Next(productsForWorkCenter.Count)];

                    // Assign the random product to the work center
                    InputData.I0W[workCenter] = randomProduct;
                }
            }

            #endregion
            #region Uiw  

            // Generate production times for Pw dictionary
            foreach (var workCenter in InputData.Pw.Keys)
            {
                foreach (var product in InputData.Pw[workCenter])
                {
                    // Check if the key already exists in the dictionary
                    if (!InputData.Uiw.ContainsKey((product, workCenter)))
                    {
                        double mean = 0.01;
                        double range = 0.015 - 0.005;
                        double standardDeviation = range / Math.Sqrt(12); // Calculate the standard deviation for a uniform distribution
                        double halfRange = range / 2.0;

                        // Generate a random value from a normal distribution with the specified mean and standard deviation
                        double productionTime = random.NextDouble() * range + (mean - halfRange);

                        // Ensure production time is within the specified range
                        productionTime = Math.Max(0.005, Math.Min(0.015, productionTime));

                        // Round to four decimal places
                        productionTime = Math.Round(productionTime, 4);

                        // Add the generated production time to the dictionary
                        InputData.Uiw.Add((product, workCenter), productionTime);
                    }
                }
            }

            // Generate production times for Qw dictionary
            foreach (var workCenter in InputData.Qw.Keys)
            {
                foreach (var product in InputData.Qw[workCenter])
                {
                    // Check if the key already exists in the dictionary
                    if (!InputData.Uiw.ContainsKey((product, workCenter)))
                    {
                        double mean = 0.01;
                        double range = 0.015 - 0.005;
                        double standardDeviation = range / Math.Sqrt(12); // Calculate the standard deviation for a uniform distribution
                        double productionTime = random.NextDouble() * range + 0.005; // Random value between 0.005 and 0.015
                        productionTime = Math.Max(0.005, Math.Min(0.015, productionTime)); // Ensure production time is within the specified range
                        productionTime = Math.Round(productionTime, 4); // Round to four decimal places
                        InputData.Uiw.Add((product, workCenter), productionTime);
                    }
                }
            }

            #endregion

            #region Sijw

            // Fill in the values for Sijw
            InputData.Sijw = new Dictionary<(string, string, string), double>();

            foreach (var fromWorkCenter in InputData.Pw.Keys)
            {
                foreach (var fromProduct in InputData.Pw[fromWorkCenter])
                {
                    foreach (var toWorkCenter in InputData.Pw.Keys)
                    {
                        foreach (var toProduct in InputData.Pw[toWorkCenter])
                        {
                            // Skip if the key already exists
                            if (InputData.Sijw.ContainsKey((fromProduct, toProduct, fromWorkCenter)))
                            {
                                continue;
                            }

                            if (fromWorkCenter == toWorkCenter && fromProduct == toProduct)
                            {
                                // Setup time from the same product at the same workstation is zero
                                InputData.Sijw.Add((fromProduct, toProduct, fromWorkCenter), 0);
                            }
                            else
                            {
                                // Generate setup time with mean 1.0 hour
                                double mean = 1.0;
                                double setupTime = random.NextDouble() * mean; // Random value between 0 and mean
                                setupTime = Math.Round(setupTime, 4); // Round to four decimal places
                                InputData.Sijw.Add((fromProduct , toProduct,fromWorkCenter), setupTime);
                            }
                        }
                    }
                }
            }

            foreach (var fromWorkCenter in InputData.Qw.Keys)
            {
                foreach (var fromProduct in InputData.Qw[fromWorkCenter])
                {
                    foreach (var toWorkCenter in InputData.Qw.Keys)
                    {
                        foreach (var toProduct in InputData.Qw[toWorkCenter])
                        {
                            // Skip if the key already exists
                            if (InputData.Sijw.ContainsKey((fromProduct, toProduct, fromWorkCenter)))
                            {
                                continue;
                            }

                            if (fromWorkCenter == toWorkCenter && fromProduct == toProduct)
                            {
                                // Setup time from the same product at the same workstation is zero
                                InputData.Sijw.Add((fromProduct , toProduct,fromWorkCenter), 0);
                            }
                            else
                            {
                                // Generate setup time with mean 1.0 hour
                                double mean = 1.0;
                                double setupTime = random.NextDouble() * mean; // Random value between 0 and mean
                                setupTime = Math.Round(setupTime, 4); // Round to four decimal places
                                InputData.Sijw.Add((fromProduct, toProduct, fromWorkCenter), setupTime);
                            }
                        }
                    }
                }
            }

            #endregion

            #region  Mjwt represents the maximum production quantity for product j at work center w during period t
            // Fill in the values for Miwt

            // Define the mean and range
            double PmeanMiwt = 395.0;
            double PrangeMiwt = 30.0; // Range from 80 to 110

            double QmeanMiwt = PmeanMiwt*2;
            double QrangeMiwt = PrangeMiwt;
            // Generate maximum production quantity values for each product, work center, and date
            foreach (var date in InputData.Dates)
            {

                // Generate maximum production quantity values for Pw work centers
                foreach (var workCenter in InputData.Pw.Keys)
                {
                    foreach (var product in InputData.Pw[workCenter])
                    {
                        double maxProduction = PmeanMiwt + (random.NextDouble() * PrangeMiwt) - (PrangeMiwt / 2.0); // Random value around the mean
                        maxProduction = Math.Round(maxProduction, 4); // Round to four decimal places
                        InputData.Miwt.Add((product, workCenter, date), maxProduction); // Inserting date as Item2
                    }
                }

                // Generate maximum production quantity values for Qw work centers
                foreach (var workCenter in InputData.Qw.Keys)
                {
                    foreach (var product in InputData.Qw[workCenter])
                    {
                        double maxProduction = QmeanMiwt + (random.NextDouble() * QrangeMiwt) - (QrangeMiwt / 2.0); // Random value around the mean
                        maxProduction = Math.Round(maxProduction, 4); // Round to four decimal places
                        InputData.Miwt.Add((product, workCenter, date), maxProduction); // Inserting date as Item2
                    }
                }
            }
            #endregion

            #region Awt represents the available capacity at work center w during period t.
            // Fill in the values for Awt

            // Define the mean and range
            double PmeanAwt = 40.0;
            double PrangeAwt = 30.0; // Range from 90 to 120
            double QmeanAwt = PmeanAwt *3;
            double QrangeAwt = PrangeAwt;
            // Generate capacity values for each work center and date
            foreach (var date in InputData.Dates)
            {

                // Generate capacity values for Pw work centers
                foreach (var workCenter in InputData.Pw.Keys)
                {
                    double capacity = PmeanAwt + (random.NextDouble() * PrangeAwt) - (PrangeAwt / 2.0); // Random value around the mean
                    capacity = Math.Round(capacity, 4); // Round to four decimal places
                    InputData.Awt.Add((workCenter, date), capacity); // Inserting date as Item2
                }

                // Generate capacity values for Qw work centers
                foreach (var workCenter in InputData.Qw.Keys)
                {
                    double capacity = QmeanAwt + (random.NextDouble() * QrangeAwt) - (QrangeAwt / 2.0); // Random value around the mean
                    capacity = Math.Round(capacity, 4); // Round to four decimal places
                    InputData.Awt.Add((workCenter, date), capacity); // Inserting date as Item2
                }
            }
            #endregion

            #region Ii ,Imax_min
            foreach(var row in InputData.Inventory.StockData) 
            {
                InputData.Ii.Add(row.StockItem.ItemCode, (row.Stock,0));

                InputData.Imax_min.Add(row.StockItem.ItemCode, (row.InvMax, row.InvMin));

            }



            #endregion

            #region Print Dictionaries
            // Print Pw dictionary
            Console.WriteLine("Pw (End-Items for each Work Center):");
            foreach (var kvp in InputData.Pw)
            {
                Console.WriteLine($"Work Center: {kvp.Key}");
                Console.WriteLine("End-Items:");
                foreach (var item in kvp.Value)
                {
                    Console.WriteLine($"- {item}");
                }
            }

            // Print Qw dictionary
            Console.WriteLine("\nQw (Products for each Work Center):");
            foreach (var kvp in InputData.Qw)
            {
                Console.WriteLine($"Work Center: {kvp.Key}");
                Console.WriteLine("Products:");
                foreach (var item in kvp.Value)
                {
                    Console.WriteLine($"- {item}");
                }
            }

            // Print Dit dictionary
            Console.WriteLine("\nDit (Demand for each product at the end of period t):");
            foreach (var kvp in InputData.Dit)
            {
                Console.WriteLine($"Product: {kvp.Key}, Demand: {kvp.Value}");
            }

            // Print Ci dictionary
            Console.WriteLine("\nCi (Direct subcomponents of each component or end-item):");
            foreach (var kvp in InputData.Ci)
            {
                Console.WriteLine($"Component or End-Item: {kvp.Key}");
                Console.WriteLine("Subcomponents:");
                foreach (var subcomponent in kvp.Value)
                {
                    Console.WriteLine($"- {subcomponent}");
                }
            }

            // Print Rij dictionary
            Console.WriteLine("\nRij (Number of units of direct subcomponent i required in each unit of component or end-item j):");
            foreach (var kvp in InputData.Rij)
            {
                Console.WriteLine($"Component or End-Item: {kvp.Key.Item2}, Subcomponent: {kvp.Key.Item1}, Quantity: {kvp.Value}");
            }

            // Print Awt dictionary
            Console.WriteLine("\nAwt (Available capacity at each work center during period t):");
            foreach (var kvp in InputData.Awt)
            {
                Console.WriteLine($"Work Center: {kvp.Key.Item1}, Capacity: {kvp.Value}");
            }

            // Print Sijw dictionary
            Console.WriteLine("\nSijw (Setup times for all products and workstations):");
            foreach (var kvp in InputData.Sijw)
            {
                Console.WriteLine($"From Work Center: {kvp.Key.Item1}, From Product: {kvp.Key.Item2}, To Product: {kvp.Key.Item3}, Setup Time: {kvp.Value}");
            }

            // Print Miwt dictionary
            Console.WriteLine("\nMiwt (Maximum production quantity for product j at work center w during period t):");
            foreach (var kvp in InputData.Miwt)
            {
                Console.WriteLine($"Work Center: {kvp.Key.Item1}, Product: {kvp.Key.Item2}, Max Quantity: {kvp.Value}");
            }

            // Print Uiw dictionary
            Console.WriteLine("\nUiw (Unit production times for all products and workstations):");
            foreach (var kvp in InputData.Uiw)
            {
                Console.WriteLine($"Work Center: {kvp.Key.Item1}, Product: {kvp.Key.Item2}, Production Time: {kvp.Value}");
            }

            // Print Hi dictionary
            Console.WriteLine("\nHi (Except from Awt and Miwt):");
            foreach (var kvp in InputData.Hi)
            {
                Console.WriteLine($"Product: {kvp.Key}, Value: {kvp.Value}");
            }

            #endregion

            var dd = 1;

        }
        #endregion

        public HashSet<string> GetDistinctBOMItems(MRPInputData inputData)
        {
            HashSet<string> distinctItems = new HashSet<string>();

            foreach (var item in inputData.EndItems)
            {
                foreach (var bomItem in item.Bom)
                {
                    distinctItems.Add(bomItem.BomItem.ItemCode);

                }

            }


            return (distinctItems);
        }
        #region Calculate MRP
        public ICommand CalculateMrp { get; }

        private void ExecuteCalculateMrp(object obj)
        {
            OutputData = CommonFunctions.CalculateMRP2(InputData);
            SelectedTabIndex = 2;

        }
        #endregion

        #region Diagrams

        #region F7
        public ICommand ShowItemGridCommand { get; }
        public ICommand ShowWorkcenterGridCommand { get; }


        private void ExecuteShowItemGridCommand(object obj)
        {


            ClearColumnsD();

            var F7input = F7Common.F7ItemMRPDiagram1();
            F7key = F7input.F7key;

            var unitedItems = new ObservableCollection<ItemData>();

            // Combine EndItems and BomItems
            foreach (var item in InputData.EndItems.Union(InputData.BomItems))
            {
                unitedItems.Add(item);
            }
            unitedItems.Distinct();
            F7input.CollectionView = CollectionViewSource.GetDefaultView(unitedItems);

            CollectionViewD = F7input.CollectionView;
            var a = F7input.SfGridColumns;
            foreach (var item in a)
            {
                this.SfGridColumnsD.Add(item);
            }


        }

        private void ExecuteShowWorkcenterGridCommand(object obj)
        {


            ClearColumnsD();
            var F7input = F7Common.F7WorkcenterMRPDiagram2();
            F7key = F7input.F7key;
            var Data = InputData.Workcenters;
            F7input.CollectionView = CollectionViewSource.GetDefaultView(Data);
            CollectionViewD = F7input.CollectionView;
            var a = F7input.SfGridColumns;
            foreach (var item in a)
            {
                this.SfGridColumnsD.Add(item);
            }


        }
        public void ChangeCanExecuteD(object obj)
        {

            if (F7key == "ItemCode")
            {
                OutputData.Diagram1.Item = (SelectedItem2 as ItemData);
            }
            if (F7key == "Workcenter")
            {
                OutputData.Diagram2.Workcenter = (SelectedItem2 as WorkcenterData);
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

        #region Diagram 1
        public List<string> Stock { get; set; }
        public ICommand CreateDiagram1Command { get; }

        private void ExecuteCreateDiagram1Command(object obj)
        {
            try
            {
                var DiagramData_1 = new ObservableCollection<DataPerDayMRP>();
                var XData = new ObservableCollection<DecisionVariableX>();

                #region Κατασκευη linechart με 3 γραμμές
                if (!string.IsNullOrWhiteSpace(OutputData.Diagram1.Item.ItemCode))
                {
                    DiagramData_1 = OutputData.Diagram1.DataPerDayMRP.Where(item => item.ItemCode == OutputData.Diagram1.Item.ItemCode).ToObservableCollection();
                    XData = OutputData.XData.Where(item => item.ItemCode == OutputData.Diagram1.Item.ItemCode).ToObservableCollection();
                }
                else
                {
                    MessageBox.Show("Error: ItemCode is null or whitespace.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                }


                OutputData.Diagram1.SeriesCollection = new SeriesCollection();

                // Add "Demand" series
                var demandSeries = new LineSeries
                {
                    Title = "Demand",
                    Values = new ChartValues<ObservableValue>(
                        DiagramData_1.Select(d => new ObservableValue(d.Demand))),
                };
                OutputData.Diagram1.SeriesCollection.Add(demandSeries);

                // Add "Make" series
                var makeSeries = new ColumnSeries
                {
                    Title = "Make",
                    Values = new ChartValues<double>(DiagramData_1.Select(d => Convert.ToDouble(d.Make))),
                };
                OutputData.Diagram1.SeriesCollection.Add(makeSeries);

                //// Add "Make2" series
                //var make2Series = new StackedColumnSeries
                //{
                //    Title = "Make2",
                //    Values = new ChartValues<double>(XData.Select(d => Convert.ToDouble(d.Value))),
                //    StackMode = StackMode.Values
                //};
                //OutputData.Diagram1.SeriesCollection.Add(make2Series);

                // Add "Store" series
                var storeSeries = new ColumnSeries
                {
                    Title = "Store",
                    Values = new ChartValues<ObservableValue>(
                        DiagramData_1.Select(d => new ObservableValue(d.Stock))),
                };
                OutputData.Diagram1.SeriesCollection.Add(storeSeries);

                // Add "BackLog" series
                var backlogSeries = new ColumnSeries
                {
                    Title = "BackLog",
                    Values = new ChartValues<ObservableValue>(
                        DiagramData_1.Select(d => new ObservableValue(d.Backlog))),
                };
                OutputData.Diagram1.SeriesCollection.Add(backlogSeries);



                // Set the labels for the x-axis to the months in the data
                OutputData.Diagram1.Labels = DiagramData_1.Select(d => d.Date).ToArray();

                OutputData.Diagram1.YFormatter = value => value.ToString("N0");
                #endregion
            }
            catch
            {
                Console.WriteLine("An error occurred");

            }

        }
        #endregion

        #region Diagram 2
        public ICommand CreateDiagram2Command { get; }

        private void ExecuteCreateDiagram2Command(object obj)
        {
            try
            {
                var DiagramData_2 = new ObservableCollection<DataPerDayMRP>();
                var selectedworkcenter = OutputData.Diagram2.Workcenter.WorkCode;
                #region Κατασκευη linechart με 3 γραμμές




                OutputData.Diagram2.SeriesCollection = new SeriesCollection();

                // Add "Demand" series

                var demandSeries = new LineSeries
                {
                    Title = "Capacity",
                    Values = new ChartValues<ObservableValue>(
            InputData.Awt.Where(d => d.Key.Item1 == selectedworkcenter).Select(d => new ObservableValue(d.Value))),
                };

                //OutputData.Diagram2.SeriesCollection.Add(demandSeries);

                #region Teliko

                var WorkXData = OutputData.XData
                    .Where(d => d.WorkCenter == selectedworkcenter && d.Value > 0);

                var WorkYData = OutputData.YData
                    .Where(d => d.WorkCenter == selectedworkcenter && d.Value > 0);

                Dictionary<(string, string, string), (double, double)> Diagram2Dict = new Dictionary<(string, string, string), (double, double)>();

                foreach (var x in WorkXData)
                {
                    var speficicYData = WorkYData.Where(d => d.ItemCodeTo == x.ItemCode && d.Date == x.Date);
                    foreach (var y in speficicYData)
                    {
                        double UiwValue = InputData.Uiw[(x.ItemCode, selectedworkcenter)];
                        double X_U = x.Value * UiwValue;

                        double SijwValue = InputData.Sijw[(y.ItemCodeFrom, x.ItemCode, selectedworkcenter)];
                        double Y_S = y.Value * SijwValue;

                        var key = (y.ItemCodeFrom, x.ItemCode, x.Date);

                        if (Diagram2Dict.ContainsKey(key))
                        {
                            var (existingX_U, existingY_S) = Diagram2Dict[key];
                            Diagram2Dict[key] = (existingX_U + X_U, existingY_S + Y_S);
                        }
                        else
                        {
                            Diagram2Dict.Add(key, (X_U, Y_S));
                        }
                    }
                }
                foreach (var kvp in Diagram2Dict)
                {
                    var key = kvp.Key;
                    var values = kvp.Value;

                    Console.WriteLine($"Key: {key.Item1}, {key.Item2}, {key.Item3} - Values: {values.Item1}, {values.Item2}");
                }
                #endregion

                var filteredList = OutputData.XData
                    .Where(d => d.WorkCenter == selectedworkcenter && d.Value > 0)
                    .GroupBy(d => d.Date)
                    .Select(group => new
                    {
                        Date = group.Key,
                        SumValue = group.Sum(item => item.Value)
                    })
                    .ToList(); // To materialize the query and convert it to a list

                var AverageUjw = InputData.Uiw.Sum(item => item.Value)/InputData.Uiw.Count();

                //var makeSeries = new ColumnSeries
                //{
                //    Title = "Make",
                //    Values = new ChartValues<double>(filteredList.Select(item => item.SumValue) ),
                //};
                //OutputData.Diagram2.SeriesCollection.Add(makeSeries);
                OutputData.Diagram2.Labels = InputData.Dates.ToArray();

                OutputData.Diagram2.YFormatter = value => value.ToString("N0");


                foreach (var date in InputData.Dates)
                {
                    var stackedColumnSeries1 = new ColumnSeries
                    {
                        Values = new ChartValues<double>(),
                        Title = "Production Time"

                    };
                    var stackedColumnSeries2 = new ColumnSeries
                    {
                        Values = new ChartValues<double>(),
                        Title = "Setup Time"

                    };

                    var Dict = Diagram2Dict.Where(kv => kv.Key.Item3 == date);
                    foreach (var kvp in Dict)
                    {
                        var Values = kvp.Value;
                        stackedColumnSeries1.Title = kvp.Key.Item1 +"_X";
                        stackedColumnSeries1.Values.Add(Values.Item1);

                        stackedColumnSeries2.Title = kvp.Key.Item2 + "_Y";
                        stackedColumnSeries2.Values.Add(Values.Item2);
                        
                    }

                    OutputData.Diagram2.SeriesCollection.Add(stackedColumnSeries1);

                }

            }
            catch
            {
                Console.WriteLine("An error occurred: " );
            }
           
            #endregion


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


