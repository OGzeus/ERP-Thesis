using Erp.Model.BasicFiles;
using Erp.Model.Interfaces;
using Erp.ViewModel.Thesis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Thesis.CrewScheduling
{
    public class FlightLegsData : RecordBaseModel
    {
        private int _FlightLegId;
        private string _Code;
        private string _Descr;

        private AirportData _AirportDataFrom;
        private AirportData _AirportDataTo;
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

        public int FlightLegId
        {
            get { return _FlightLegId; }
            set { _FlightLegId = value; INotifyPropertyChanged(nameof(FlightLegId)); }
        }

        public string Code
        {
            get { return _Code; }
            set { _Code = value; INotifyPropertyChanged(nameof(Code)); }
        }
        public string Descr
        {
            get { return _Descr; }
            set { _Descr = value; INotifyPropertyChanged(nameof(Descr)); }
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
        public AirportData AirportDataFrom
        {
            get { return _AirportDataFrom; }
            set { _AirportDataFrom = value; INotifyPropertyChanged(nameof(AirportDataFrom)); }
        }

        public AirportData AirportDataTo
        {
            get { return _AirportDataTo; }
            set { _AirportDataTo = value; INotifyPropertyChanged(nameof(AirportDataTo)); }
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

    }
}
