using Erp.Model.BasicFiles;
using Erp.Model.Thesis.CrewScheduling;
using Syncfusion.UI.Xaml.Grid;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Erp.ViewModel.Thesis
{
    public class FlightRoutesViewModel : ViewModelBase
    {
        #region Properties

        private Columns cityGridColumns;
        public Columns CityGridColumns
        {
            get { return cityGridColumns; }
            set
            {
                cityGridColumns = value;
                INotifyPropertyChanged(nameof(CityGridColumns));
            }
        }

        private ObservableCollection<FlightRoutesData> data;
        public ObservableCollection<FlightRoutesData> Data
        {
            get { return data; }
            set
            {
                data = value;
                INotifyPropertyChanged(nameof(Data));
            }
        }

        private static List<CityData> cityList;

        public static List<CityData> CityList
        {
            get { return cityList; }
            set { cityList = value; }
        }

        #endregion

        public FlightRoutesViewModel()
        {
            Data = new ObservableCollection<FlightRoutesData>();
            CityList = new List<CityData>();
            CityGridColumns = new Columns();

            CityGridColumns.Add(new GridTextColumn() { MappingName = "CityCode", HeaderText = "City Code " });
            CityGridColumns.Add(new GridTextColumn() { MappingName = "CityDescr", HeaderText = "City Descr " });
            CityGridColumns.Add(new GridTextColumn() { MappingName = "CountryDescr", HeaderText = "Country Descr" });
            OnLoad();
        }

        public void OnLoad()
        {
            Data = CommonFunctions.GetFlightRoutesData(ShowDeleted);
            CityList = CommonFunctions.GetCityData(false).ToList();
        }

        #region Toolbar
        private ViewModelCommand refreshCommand;

        #region Refresh
        public ICommand RefreshCommand
        {
            get
            {
                if (refreshCommand == null)
                {
                    refreshCommand = new ViewModelCommand(Refresh);
                }

                return refreshCommand;
            }
        }

        private void Refresh(object commandParameter)
        {
            Data = new ObservableCollection<FlightRoutesData>();
            OnLoad();
        }

        #endregion

        #region SaveCommand

        private ViewModelCommand saveCommand;

        public ICommand SaveCommand
        {
            get
            {
                if (saveCommand == null)
                {
                    saveCommand = new ViewModelCommand(Save);
                }

                return saveCommand;
            }
        }

        private void Save(object commandParameter)
        {
            bool completed = CommonFunctions.SaveFlightRoutesData(Data, CityList);

            if (completed)
            {
                MessageBox.Show($"Saving/Updating completed ");
            }
            else
            {
                MessageBox.Show("Error during data processing", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #endregion
    }
}
