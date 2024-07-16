using Erp.Helper;
using Erp.Model.BasicFiles;
using Erp.Model.Thesis;
using Erp.Model.Thesis.CrewScheduling;
using Syncfusion.UI.Xaml.Grid;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Erp.ViewModel.Thesis
{
    public class FlightRoutesViewModel : ViewModelBase
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


        private FlightRoutesData flatData;
        public FlightRoutesData FlatData
        {
            get { return flatData; }
            set
            {
                flatData = value;
                INotifyPropertyChanged(nameof(FlatData));
            }
        }

        #endregion

        #region Commands

        #region CRUD Commands

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
        public void ResetViewmodelData()
        {
            FlatData = new FlightRoutesData();
            FlatData.Airport = new AirportData();

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
            int Flag = CommonFunctions.SaveFlightRoutesData(FlatData);

            if (Flag == 1)
            {
                MessageBox.Show($"Η Αποθήκευση/Ανανέωση Ολοκληρώθηκε για το Flight Route με Κωδικό : {FlatData.Code}");
                ExecuteShowFlightRoutesGridCommand(obj);
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
            FlatData = CommonFunctions.GetFlightRoutesChooserData(FlatData.FlightRouteId, FlatData.Code);

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
                int Flag = CommonFunctions.AddFlightRoutesData(FlatData);
                if (Flag == 0)

                {
                    MessageBox.Show($"Ο Αποθηκεύτηκε νέο Flight Route με Κωδικό : {FlatData.Code}");
                    ExecuteShowFlightRoutesGridCommand(obj);
                    FlatData.FlightRouteId = 0;
                    ExecuteRefreshCommand(obj);

                }
                else if (Flag == 1)
                {
                    MessageBox.Show($"The Flight Route with Code : {FlatData.Code} already exists");

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

        #region Data_Grid Commands

        public ICommand ShowFlightRoutesGridCommand { get; }
        public ICommand ShowAirportsGridCommand { get; }

        private void ExecuteShowFlightRoutesGridCommand(object obj)
        {

            ClearColumns();

            var F7input = F7Common.F7FlightRoutes(false);
            F7key = F7input.F7key;
            CollectionView = F7input.CollectionView;
            var a = F7input.SfGridColumns;
            foreach (var item in a)
            {
                this.sfGridColumns.Add(item);
            }

        }

        private void ExecuteShowAirportsGridCommand(object obj)
        {
            ClearColumns();

            var F7input = F7Common.F7Airports(ShowDeleted);
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

            if (F7key == "FlightRoute")
            {
                FlatData = new FlightRoutesData();
                FlatData.Airport = new AirportData();
                FlatData = (SelectedItem as FlightRoutesData);
            }

            if (F7key == "Airport")
            {
                FlatData.Airport = new AirportData();
                FlatData.Airport.City = new CityData();
                FlatData.Airport = (SelectedItem as AirportData);
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

        #endregion

        public FlightRoutesViewModel()
        {
            #region Data Initialization
            FlatData = new FlightRoutesData();
            FlatData.Airport = new AirportData();
            this.sfGridColumns = new Columns();
            #endregion

            #region Commands Initialization
            ShowFlightRoutesGridCommand = new RelayCommand2(ExecuteShowFlightRoutesGridCommand);
            ShowAirportsGridCommand = new RelayCommand2(ExecuteShowAirportsGridCommand);
            AddDataCommand = new RelayCommand2(ExecuteAddDataCommand);
            rowDataCommand = new RelayCommand2(ChangeCanExecute);

            #endregion
        }
    }
}
