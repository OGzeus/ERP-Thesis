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
using Erp.Model.Enums;
using Erp.Model.Inventory.InvControl_ConstantDemand;
using System.Globalization;
using System.Runtime.InteropServices.ComTypes;
using LiveCharts;
using LiveCharts.Wpf;
using Erp.Model.Inventory.InvControl_TimeVaryingDemand;
using static IronPython.Runtime.Profiler;
using Deedle;
using Syncfusion.Data.Extensions;

namespace Erp.ViewModel.Inventory
{
    public class InventoryControlViewModel : ViewModelBase
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

        private InvControlData filter;
        public InvControlData Filter
        {
            get { return filter; }
            set
            {
                filter = value;
                INotifyPropertyChanged("Filter");
            }
        }




        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Enums

        public BasicEnums.DemandType[] DemandTypes
        {
            get { return (BasicEnums.DemandType[])Enum.GetValues(typeof(BasicEnums.DemandType)); }
        }

        public BasicEnums.ConstantDemandType[] ConstantDemandTypes
        {
            get { return (BasicEnums.ConstantDemandType[])Enum.GetValues(typeof(BasicEnums.ConstantDemandType)); }
        }
        public BasicEnums.TimeVaryingDemandType[] TimeVaryingDemandTypes
        {
            get { return (BasicEnums.TimeVaryingDemandType[])Enum.GetValues(typeof(BasicEnums.TimeVaryingDemandType)); }
        }

        #endregion

        public InventoryControlViewModel()
        {
            this.sfGridColumns = new Columns();
            Filter = new InvControlData();
            rowDataCommand = new RelayCommand2(ChangeCanExecute);
            Filter.DateFrom = DateTime.Now;
            Filter.DateTo = DateTime.Now.AddMonths(1);
        }

        #region COLLECTION VIEW GIA TIME VARYING
        public void ChangeCanExecute(object obj)
        {
            if (F7key == "MrpResults")
            {
                //var MrpResults = CommonFunctions.GetMrpResultsData();

            }


        }
        public ICommand ShowInvOptimisationResults { get; }

        private void ExecuteShowInvOptimisationResults(ObservableCollection<TimeVaryingInvResultsData> datas)
        {
            ClearColumns();

            var F7input = F7Common.F7InvControlInfDataResults(datas);

            F7key = F7input.F7key;
            CollectionView = F7input.CollectionView;
            var a = F7input.SfGridColumns;
            System.Diagnostics.Debug.WriteLine($"Size of 'a': {a.Count}");
            System.Diagnostics.Debug.WriteLine($"Size of 'sfGridColumns': {sfGridColumns.Count}");
            

            try
            {
                foreach (var item in a)
                {
                    System.Diagnostics.Debug.WriteLine($"Current MappingName: {item.MappingName}");
                    System.Diagnostics.Debug.WriteLine($"Current HeaderText: {item.HeaderText}");
                    this.sfGridColumns.Add(item);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
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
        #endregion

        #region Gurobi

        private ViewModelCommand calculateresults;

        public ICommand CalculateResults
        {
            get
            {
                if (calculateresults == null)
                {
                    calculateresults = new ViewModelCommand(ExecuteCalculateResults);
                }

                return calculateresults;
            }
        }



        private void ExecuteCalculateResults(object obj)
        {
            if (Filter.CurrentActivePanel == InvControlData.ActivePanel.TimeVarying_Infinite_Capacity || Filter.CurrentActivePanel == InvControlData.ActivePanel.TimeVarying_Finite_Capacity)
            {
                #region HardCoded Data
                //Filter.TimeVarInfiniteData = new ObservableCollection<TimeVaryingInvData>
                //{
                //        new TimeVaryingInvData { Day = 1,  Demand = 20, HoldingCost = 1.0, SetupCost = 100.0 },
                //        new TimeVaryingInvData { Day = 2,  Demand = 50, HoldingCost = 1.0, SetupCost = 100.0 },
                //        new TimeVaryingInvData { Day = 3,  Demand = 10, HoldingCost = 1.0, SetupCost = 100.0 },
                //        new TimeVaryingInvData { Day = 4,  Demand = 50, HoldingCost = 1.0, SetupCost = 100.0 },
                //        new TimeVaryingInvData { Day = 5,  Demand = 50, HoldingCost = 1.0, SetupCost = 100.0 },
                //        new TimeVaryingInvData { Day = 6,  Demand = 10, HoldingCost = 1.0, SetupCost = 100.0 },
                //        new TimeVaryingInvData { Day = 7,  Demand = 20, HoldingCost = 1.0, SetupCost = 100.0 },
                //        new TimeVaryingInvData { Day = 8,  Demand = 40, HoldingCost = 1.0, SetupCost = 100.0 },
                //        new TimeVaryingInvData { Day = 9,  Demand = 20, HoldingCost = 1.0, SetupCost = 100.0 },
                //        new TimeVaryingInvData { Day = 10, Demand = 23, HoldingCost = 1.0, SetupCost = 100.0 }
                // };


                //Filter= CommonFunctions.CalculateInvControlTimeVar(Filter);

                #endregion

                Filter.DateFrom = new DateTime(2023, 7, 27); // dd/MM/yyyy -> 28/07/2023
                Filter.DateTo = new DateTime(2023, 8, 6); // dd/MM/yyyy -> 06/08/2023
                #region Original Verse
                Filter.Item = CommonFunctions.GetItemsWithSalesData()
                    .Where(i => i.OutputOrderFlag == true)
                    .ToObservableCollection();
                Filter.TimeVarInfiniteDataResults = new ObservableCollection<TimeVaryingInvResultsData>();


                foreach (var item in Filter.Item)
                {


                    ObservableCollection<TimeVaryingInvResultsData> DataList = new ObservableCollection<TimeVaryingInvResultsData>
                    {
                            new TimeVaryingInvResultsData { Id = 1,  RowDescr = "t : TimePeriod", ItemId = item.ItemId, Values =  new List<string>()},
                            new TimeVaryingInvResultsData { Id = 2,  RowDescr = "λ(t) : Demand", ItemId = item.ItemId, Values =  new List<string>()},
                            new TimeVaryingInvResultsData { Id = 3,  RowDescr = "K(t) : SetupCost", ItemId = item.ItemId, Values =  new List<string>()},
                            new TimeVaryingInvResultsData { Id = 4,  RowDescr = "h(t) : HoldingCost/Unit", ItemId = item.ItemId, Values =  new List<string>()},
                            new TimeVaryingInvResultsData { Id = 5,  RowDescr = "Q(t) : Quantity Ordered", ItemId = item.ItemId, Values =  new List<string>()},
                            new TimeVaryingInvResultsData { Id = 6,  RowDescr = "I(t) : Stock", ItemId = item.ItemId, Values =  new List<string>()},
                            new TimeVaryingInvResultsData { Id = 7,  RowDescr = "Y(t) : IsOrdering flag", ItemId = item.ItemId, Values =  new List<string>()},
                            new TimeVaryingInvResultsData { Id = 8,  RowDescr = "K(t)*Y(t) : SetupCost", ItemId = item.ItemId, Values =  new List<string>()},
                            new TimeVaryingInvResultsData { Id = 9,  RowDescr = "h(t)*i(t) : HoldingCost", ItemId = item.ItemId, Values =  new List<string>()},
                            new TimeVaryingInvResultsData { Id = 10,  RowDescr = "G : Total Cost", ItemId = item.ItemId , Values =  new List<string>()},

                     };



                    int totalDays = (Filter.DateTo - Filter.DateFrom).Days;

                    for (int i = 0; i < totalDays; i++)
                    {
                        DataList[0].Values.Add(Filter.DateFrom.AddDays(i).ToString("MM/dd/yyyy"));
                        DataList[2].Values.Add("100");
                        DataList[3].Values.Add("1");

                    }

                    #region Insert Demand,h(t),K(t) 
                    var a = CommonFunctions.GetDailyItemQuantityData(Filter, item);
                    DataList[1].Values = CommonFunctions.GetDailyItemQuantityData(Filter, item).ToList();


                    #endregion

                    DataList = CommonFunctions.CalculateInvControlTimeVar2(Filter,DataList);
                    Filter.TimeVarInfiniteDataResults = Filter.TimeVarInfiniteDataResults.Concat(DataList).ToObservableCollection();


                    #region last Column Total HELP CHATGPT
                    DataList[0].Values.Add("Total");


                    for (int i = 1; i < DataList.Count; i++)
                    {
                        if (i == 2 || i == 3)
                        {
                            continue;
                        }

                        double sum = DataList[i].Values.Where(x => double.TryParse(x, out _)).Sum(x => double.Parse(x));
                        DataList[i].Values.Add(sum.ToString());
                    }
                    #endregion

                }

                #endregion
                ExecuteShowInvOptimisationResults(Filter.TimeVarInfiniteDataResults);

            }
            else 
            {
                Filter = CommonFunctions.CalculateInvControlConstant(Filter);

            }

            //if (Flag == 0)
            //{
            //    MessageBox.Show($"Ο Αποθηκεύτηκε νέος παραγωγός με Κωδικό : {filter.Code}");
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

        #endregion





        public event PropertyChangedEventHandler PropertyChanged;

    }
}
