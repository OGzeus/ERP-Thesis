using Erp.Model.BasicFiles;
using Erp.Model.Thesis.CrewScheduling;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using Erp.Model.Thesis;
using Erp.Helper;

namespace Erp.ViewModel.Thesis
{
    public class FlightLegsViewModel : ViewModelBase
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


        private FlightLegsData flatData;
        public FlightLegsData FlatData
        {
            get { return flatData; }
            set
            {
                flatData = value;
                INotifyPropertyChanged(nameof(FlatData));
            }
        }

        #endregion



        public FlightLegsViewModel()
        {


            ResetViewmodelData();


            this.sfGridColumns = new Columns();
            ShowFlightLegsGridCommand = new RelayCommand2(ExecuteShowFlightLegsGridCommand);

            ShowAirportsFromGridCommand = new RelayCommand2(ExecuteShowAirportsFromGridCommand);
            ShowAirportsToGridCommand = new RelayCommand2(ExecuteShowAirportsToGridCommand);

            AddDataCommand = new RelayCommand2(ExecuteAddDataCommand);

            rowDataCommand = new RelayCommand2(ChangeCanExecute);


        }

        public void ResetViewmodelData()
        {
            FlatData = new FlightLegsData();
            FlatData.AirportDataFrom = new AirportData();
            FlatData.AirportDataTo = new AirportData();

        }

        #region F7

        public ICommand ShowFlightLegsGridCommand { get; }
        public ICommand ShowAirportsFromGridCommand { get; }
        public ICommand ShowAirportsToGridCommand { get; }



        private void ExecuteShowAirportsFromGridCommand(object obj)
        {
            ClearColumns();

            var F7input = F7Common.F7FL_Airports(ShowDeleted,FlatData.AirportDataTo);
            F7key = "AirportFrom";

            CollectionView = F7input.CollectionView;
            var a = F7input.SfGridColumns;
            foreach (var item in a)
            {
                this.sfGridColumns.Add(item);
            }

        }
        private void ExecuteShowAirportsToGridCommand(object obj)
        {
            ClearColumns();

            var F7input = F7Common.F7FL_Airports(ShowDeleted,FlatData.AirportDataFrom);
            F7key = "AirportTo";
            CollectionView = F7input.CollectionView;
            var a = F7input.SfGridColumns;
            foreach (var item in a)
            {
                this.sfGridColumns.Add(item);
            }

        }

        private void ExecuteShowFlightLegsGridCommand(object obj)
        {

            ClearColumns();

            var F7input = F7Common.F7FlightLegs (false);
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

            if (F7key == "FlightLeg")
            {
                FlatData = new FlightLegsData();
                FlatData = (SelectedItem as FlightLegsData);
            }

            if (F7key == "AirportFrom")
            {
                FlatData.AirportDataFrom = new AirportData();
                FlatData.AirportDataFrom.City = new CityData();
                FlatData.AirportDataFrom = (SelectedItem as AirportData);
            }
            if (F7key == "AirportTo")
            {
                FlatData.AirportDataTo = new AirportData();
                FlatData.AirportDataTo.City = new CityData();
                FlatData.AirportDataTo = (SelectedItem as AirportData);
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

        #region Commands Crud

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
            ResetViewmodelData();

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
            int Flag = CommonFunctions.SaveFlightLegsData(FlatData);

            if (Flag == 1)
            {
                MessageBox.Show($"Η Αποθήκευση/Ανανέωση Ολοκληρώθηκε για το Flight Leg με Κωδικό : {FlatData.Code}");
                ExecuteShowFlightLegsGridCommand(obj);
                ExecuteRefreshCommand(obj);

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
            FlatData = CommonFunctions.GetFlightLegsChooserData(FlatData.FlightLegId, FlatData.Code);

        }

        #endregion
        #region Add

        public ICommand AddDataCommand { get; }

        private void ExecuteAddDataCommand(object obj)
        {
            if (string.IsNullOrWhiteSpace(FlatData.Code) || string.IsNullOrWhiteSpace(FlatData.Descr))
            {
                MessageBox.Show("Insert Code and Description");
            }

            else
            {
                int Flag = CommonFunctions.AddFlightLegsData(FlatData);
                if (Flag == 0)

                {
                    MessageBox.Show($"Ο Αποθηκεύτηκε νέο Flight Leg με Κωδικό : {FlatData.Code}");
                    ExecuteShowFlightLegsGridCommand(obj);
                    FlatData.FlightLegId = 0;
                    ExecuteRefreshCommand(obj);

                }
                else if (Flag == 1)
                {
                    MessageBox.Show($"The Flight Leg with Code : {FlatData.Code} already exists");

                }
                else if (Flag == 2)
                {
                    MessageBox.Show("Error during data processing", "", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }
        #endregion

        #endregion

        #endregion




    }
}
