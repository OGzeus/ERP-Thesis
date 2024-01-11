using Erp.Helper;
using Erp.Model.BasicFiles;
using Erp.Model.Customers;
using Erp.Model.Inventory;
using Erp.Model.Suppliers;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Windows;
using System.Windows.Input;

namespace Erp.ViewModel.Inventory
{
    public class InvVisualisationViewModel : ViewModelBase
    {
        #region Δεδομένα Κατασκευής Διαγράμματος



        private ObservableCollection<OptimizationResultsInvData> diagramdata;
        public ObservableCollection<OptimizationResultsInvData> DiagramData
        {
            get { return diagramdata; }
            set
            {
                diagramdata = value;
                INotifyPropertyChanged(nameof(DiagramData));
            }
        }
        private SeriesCollection seriescollection;
        public SeriesCollection SeriesCollection
        {
            get { return seriescollection; }
            set
            {
                seriescollection = value;
                INotifyPropertyChanged(nameof(SeriesCollection));
            }
        }
        private string[] labels;
        public string[] Labels
        {
            get { return labels; }
            set
            {
                labels = value;
                INotifyPropertyChanged(nameof(Labels));
            }
        }

        public Func<double, string> Formatter { get; set; }
        public Func<double, string> YFormatter { get; set; }
        public SeriesCollection ProductionSeriesCollection { get; }
        #endregion

        #region Δεδομένα κατασκευής κριτηρίων Αναζήτησης

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




        private InvDiagramsSearchData filtedata;
        public InvDiagramsSearchData FilterData
        {
            get { return filtedata; }
            set { filtedata = value; }
        }


        #endregion
        public InvVisualisationViewModel()
        {



            #region Κατασκευή Κριτιρίων Αναζήτησης και Δήλωση Button Commands

            FilterData = new InvDiagramsSearchData();
            this.sfGridColumns = new Columns();

  
            ShowSupplierInfoGridCommand = new RelayCommand2(ExecuteShowSupplierGridCommand);
            ShowInventoryGridCommand = new RelayCommand2(ExecuteShowInventoryGridCommand);
            ShowItemGridCommand = new RelayCommand2(ExecuteShowItemGridCommand);
            CreateDiagramCommand = new RelayCommand2(ExecuteCreateDiagramCommand);
            ShowCityGridCommand = new RelayCommand2(ExecuteShowCityGridCommand);
            ShowCustomerInfoGridCommand = new RelayCommand2(ExecuteShowCustomerGridCommand);
            ShowPriceListGridCommand = new RelayCommand2(ExecuteShowPriceListGridCommand);

            rowDataCommand = new RelayCommand2(ChangeCanExecute);

            DateTime startDate = DateTime.Parse("2021-01-01");
            FilterData.DateStart = startDate;

            DateTime endDate = DateTime.Parse("2024-01-01");
            FilterData.DateEnd = endDate;

            FilterData.PeriodPolicy = new List<string>
            {
                            "Ανά Εβδομάδα",
                           "Ανά Μήνα",
                            "Ανά 12μηνο"
            };
            #endregion

            #region Κατασκευη linechart με 3 γραμμές


            DiagramData = CommonFunctions.GetOptimisationInvData("Porltand", "B1",FilterData);
            //DiagramData = CommonFunctions.GetOptimisationInvData("Type2", "B1", FilterData);

            // Create the series collection and add the three lines
            SeriesCollection = new SeriesCollection
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
                    Title = "Forecasted Demand",
                    Values = new ChartValues<ObservableValue>(
                        DiagramData.Select(d => new ObservableValue(d.DemandForecast))),
                    PointGeometry = null
                },
                new LineSeries
                {
                    Title = "Stock Quantity",
                    Values = new ChartValues<ObservableValue>(
                        DiagramData.Select(d => new ObservableValue(d.StockQ))),
                    PointGeometry = null
                }
            };

            // Set the labels for the x-axis to the months in the data
            Labels = DiagramData.Select(d => d.iDay.ToString("MMM")).ToArray();

            YFormatter = value => value.ToString("N0");
            #endregion
        }

        #region F7
        public ICommand ShowInventoryGridCommand { get; }
        public ICommand ShowSupplierInfoGridCommand { get; }
        public ICommand ShowItemGridCommand { get; }
        public ICommand ShowCustomerInfoGridCommand { get; }
        public ICommand ShowCityGridCommand { get; }
        public ICommand ShowPriceListGridCommand { get; }

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
        private void ExecuteShowCityGridCommand(object obj)
        {
            ClearColumns();

            var F7input = F7Common.F7City(false);
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
        private void ExecuteShowInventoryGridCommand(object obj)
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
        private void ExecuteShowSupplierGridCommand(object obj)
        {


            ClearColumns();

            var F7input = F7Common.F7SupplierFrom();
            F7key = F7input.F7key;
            CollectionView = F7input.CollectionView;
            var a = F7input.SfGridColumns;
            foreach (var item in a)
            {
                this.sfGridColumns.Add(item);
            }

        }
        private void ExecuteShowItemGridCommand(object obj)
        {


            ClearColumns();

            var F7input = F7Common.F7Item(false);
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
                FilterData.Inventory = (SelectedItem as InventoryData);
            }

            if (F7key == "ItemCode")
            {
                FilterData.Item = (SelectedItem as ItemData);
            }
            if (F7key == "Customer")
            {
                FilterData.Customer = (SelectedItem as CustomerData);
            }
            if (F7key == "City")
            {
                FilterData.City = (SelectedItem as CityData);
            }
            if (F7key == "PriceList")
            {
                FilterData.PriceList = (SelectedItem as PriceListData);
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


        #region Clear,Run

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

        //    FilterData = new InvDiagramsSearchData();
        //}

        //#endregion

        //#region SearchDiagram


        //private ViewModelCommand savecommand;

        //public ICommand SaveCommand
        //{
        //    get
        //    {
        //        if (savecommand == null)
        //        {
        //            savecommand = new ViewModelCommand(ExecuteSaveCommand);
        //        }

        //        return savecommand;
        //    }
        //}

        //private void ExecuteSaveCommand(object obj)
        //{
        //    int Flag = CommonFunctions.SaveInventoryData(FlatData);

        //    if (Flag == 0)
        //    {
        //        MessageBox.Show($"Ο Αποθηκεύτηκε νέα Αποθήκη με Κωδικό : {FlatData.InvCode}");
        //    }
        //    else if (Flag == 1)
        //    {
        //        MessageBox.Show($"Η Αποθήκευση/Ανανέωση Ολοκληρώθηκε για την Αποθήκη με Κωδικό : {FlatData.InvCode}");

        //    }
        //    else
        //    {
        //        MessageBox.Show("Σφάλμα κατά την επεξεργασία δεδομένων", "", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        //#endregion


        #endregion

        #region Συνάρτηση Διαγράμμος

        public List<string> Stock { get; set; }
        public ICommand CreateDiagramCommand { get; }

        private void ExecuteCreateDiagramCommand(object obj)
        {

            #region Κατασκευη linechart με 3 γραμμές

            DiagramData = CommonFunctions.GetOptimisationInvData(FilterData.Item.ItemCode,FilterData.Inventory.InvCode,FilterData);
            //DiagramData = CommonFunctions.GetOptimisationInvData("Type2", "B1", FilterData);

            // Create the series collection and add the three lines
            SeriesCollection = new SeriesCollection
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
                    Title = "Forecasted Demand",
                    Values = new ChartValues<ObservableValue>(
                        DiagramData.Select(d => new ObservableValue(d.DemandForecast))),
                    PointGeometry = null
                },
                new LineSeries
                {
                    Title = "Stock Quantity",
                    Values = new ChartValues<ObservableValue>(
                        DiagramData.Select(d => new ObservableValue(d.StockQ))),
                    PointGeometry = null
                }
            };

            // Set the labels for the x-axis to the months in the data
            Labels = DiagramData.Select(d => d.iDay.ToString("MMM")).ToArray();


            #endregion

        }

        #endregion
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}