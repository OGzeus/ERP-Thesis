using Erp.CommonFiles;
using Erp.Model.BasicFiles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using Newtonsoft.Json;
using System.Net.Http;
using System.Globalization;
using System.Net;

namespace Erp.ViewModel.BasicFiles
{
    public class CityViewModel : ViewModelBase
    {
        #region Properties




        private List<PrefectureData> prefList;

        public List<PrefectureData> PrefList
        {
            get { return prefList; }
            set { prefList = value; }
        }

        private ObservableCollection<CityData> data;
        public ObservableCollection<CityData> Data
        {
            get { return data; }
            set
            {
                data = value;
                INotifyPropertyChanged(nameof(Data));
            }
        }
        #endregion
        public CityViewModel()
        {
            Data = new ObservableCollection<CityData>();

            PrefList = new List<PrefectureData>();
            Data.CollectionChanged += Data_CollectionChanged; // Subscribe to the event


            OnLoad();

        }
        private async void Data_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (CityData newCity in e.NewItems)
                {
                    var geoCoordinates = await GetGeocodedData(newCity.CityDescr);

                    if (geoCoordinates != null)
                    {
                        newCity.Latitude = (float)geoCoordinates.Latitude;
                        newCity.Longitude = (float)geoCoordinates.Longitude;
                    }
                    else
                    {
                        MessageBox.Show("The city name you entered, '" + newCity.CityDescr + "', could not be found. Please check for any typos and try again.");
                    }
                }
            }
        }

        public void OnLoad()
        {
            Data = CommonFunctions.GetCityData(ShowDeleted);
            PrefList = CommonFunctions.GetPrefectureData().ToList();


        }


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
            Data = new ObservableCollection<CityData>();

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
            bool Completed = CommonFunctions.SaveCityData(Data);

            if (Completed == true)
            {
                MessageBox.Show($"Saving/Updating completed ");
            }
            else
            {
                MessageBox.Show("Error during data processing", "", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }




        #endregion

        #region GeocodeCommand

        public class NominatimResult
        {
            public string Lat { get; set; }
            public string Lon { get; set; }
        }

        public class GeoCoordinates
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }

        private async Task<GeoCoordinates> GetGeocodedData(string city)
        {
            GeoCoordinates geoCoordinates = null;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("ErpThesis");
                var response = await client.GetAsync($"https://nominatim.openstreetmap.org/search?city={WebUtility.UrlEncode(city)}&format=json");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var geoData = JsonConvert.DeserializeObject<List<NominatimResult>>(result);

                    if (geoData != null && geoData.Count > 0)
                    {
                        geoCoordinates = new GeoCoordinates
                        {
                            Latitude = double.Parse(geoData[0].Lat, CultureInfo.InvariantCulture),
                            Longitude = double.Parse(geoData[0].Lon, CultureInfo.InvariantCulture)
                        };
                    }
                }
            }

            return geoCoordinates;
        }

        private ViewModelCommand geocodeCommand;

        public ICommand GeocodeCommand
        {
            get
            {
                if (geocodeCommand == null)
                {
                    geocodeCommand = new ViewModelCommand(Geocode);
                }

                return geocodeCommand;
            }
        }

        private async void Geocode(object commandParameter)
        {
            var selectedCity = commandParameter as CityData;

            if (selectedCity != null)
            {
                var geoCoordinates = await GetGeocodedData(selectedCity.CityDescr);

                if (geoCoordinates != null)
                {
                    selectedCity.Latitude = (float)geoCoordinates.Latitude;
                    selectedCity.Longitude = (float)geoCoordinates.Longitude;
                }
                else
                {
                    MessageBox.Show("The city name you entered, '" + selectedCity.CityDescr + "', could not be found. Please check for any typos and try again.");
                }
            }
        }


        #endregion
    }
}

