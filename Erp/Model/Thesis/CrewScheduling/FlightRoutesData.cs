using Erp.Model.BasicFiles;
using Erp.Model.Interfaces;
using Erp.ViewModel.Thesis;
using System;
using System.Linq;

namespace Erp.Model.Thesis.CrewScheduling
{
    public class FlightRoutesData : RecordBaseModel
    {
        private int _FlightRouteId;
        private string _Code;
        private string _CityCodeFrom;
        private string _CityCodeTo;
        private CityData _CityDataFrom;
        private CityData _CityDataTo;
        private string _CountryCodeFrom;
        private string _CountryCodeTo;
        private float _FlightTime;
        private DateTime _StartDate;
        private DateTime _EndDate;

        private string _StartDate_String;
        private string _EndDate_String;
        private bool _Selected { get; set; }

        public bool Selected
        {
            get { return _Selected; }
            set { _Selected = value; INotifyPropertyChanged("Selected"); }
        }

        public int FlightRouteId
        {
            get { return _FlightRouteId; }
            set { _FlightRouteId = value; INotifyPropertyChanged(nameof(FlightRouteId)); }
        }

        public string Code
        {
            get { return _Code; }
            set { _Code = value; INotifyPropertyChanged(nameof(Code)); }
        }

        public string CityCodeFrom
        {
            get { return _CityCodeFrom; }
            set
            {
                if (_CityCodeFrom != value)
                {
                    _CityCodeFrom = value;
                    INotifyPropertyChanged(nameof(CityCodeFrom));
                    UpdateCountryCodeFrom();
                }
            }
        }

        public string CityCodeTo
        {
            get { return _CityCodeTo; }
            set
            {
                if (_CityCodeTo != value)
                {
                    _CityCodeTo = value;
                    INotifyPropertyChanged(nameof(CityCodeTo));
                    UpdateCountryCodeTo();
                }
            }
        }
        public string StartDate_String
        {
            get { return _StartDate_String; }
            set { _StartDate_String = value; INotifyPropertyChanged(nameof(StartDate_String)); }
        }
        public string EndDate_String
        {
            get { return _EndDate_String; }
            set { _EndDate_String = value; INotifyPropertyChanged(nameof(EndDate_String)); }
        }
        public CityData CityDataFrom
        {
            get { return _CityDataFrom; }
            set { _CityDataFrom = value; INotifyPropertyChanged(nameof(CityDataFrom)); }
        }

        public CityData CityDataTo
        {
            get { return _CityDataTo; }
            set { _CityDataTo = value; INotifyPropertyChanged(nameof(CityDataTo)); }
        }

        public string CountryCodeFrom
        {
            get { return _CountryCodeFrom; }
            set { _CountryCodeFrom = value; INotifyPropertyChanged(nameof(CountryCodeFrom)); }
        }

        public string CountryCodeTo
        {
            get { return _CountryCodeTo; }
            set { _CountryCodeTo = value; INotifyPropertyChanged(nameof(CountryCodeTo)); }
        }

        public float FlightTime
        {
            get { return _FlightTime; }
            set
            {
                if (_FlightTime != value)
                {
                    // Round the value to two decimal places
                    _FlightTime = (float)Math.Round(value, 2);
                    INotifyPropertyChanged(nameof(FlightTime));
                }
            }
        }

        public DateTime StartDate
        {
            get
            {
                return _StartDate == default(DateTime) ? DateTime.Now : _StartDate;
            }
            set
            {
                if (_StartDate != value)
                {
                    _StartDate = value == default(DateTime) ? DateTime.Now : value;
                    FlightTime = (float)(_EndDate - _StartDate).TotalHours;
                    INotifyPropertyChanged(nameof(StartDate));
                }
            }
        }

        public DateTime EndDate
        {
            get
            {
                return _EndDate == default(DateTime) ? DateTime.Now.AddDays(1) : _EndDate;
            }
            set
            {
                if (_EndDate != value)
                {
                    _EndDate = value == default(DateTime) ? DateTime.Now.AddDays(1) : value;
                    FlightTime = (float)(_EndDate - _StartDate).TotalHours;
                    INotifyPropertyChanged(nameof(EndDate));
                }
            }
        }


        private void UpdateCountryCodeFrom()
        {
            var city = FlightRoutesViewModel.CityList.FirstOrDefault(c => c.CityCode == _CityCodeFrom);
            if (city != null)
            {
                CountryCodeFrom = city.CountryCode;
            }
        }

        private void UpdateCountryCodeTo()
        {
            var city = FlightRoutesViewModel.CityList.FirstOrDefault(c => c.CityCode == _CityCodeTo);
            if (city != null)
            {
                CountryCodeTo = city.CountryCode;
            }
        }
    }
}
