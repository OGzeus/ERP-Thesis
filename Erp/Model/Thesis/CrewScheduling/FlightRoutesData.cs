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
        private string _Descr;
        private AirportData _Airport;

        private DateTime _StartDate;
        private DateTime _EndDate;
        private float _FlightTime;
        private float _GroundTime;
        private float _TotalTime;

        private int _Complement_Captain;
        private int _Complement_FO;
        private int _Complement_Cabin_Manager;
        private int _Complement_Flight_Attendant;

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
        public string Descr
        {
            get { return _Descr; }
            set { _Descr = value; INotifyPropertyChanged(nameof(Descr)); }
        }
        public AirportData Airport
        {
            get { return _Airport; }
            set { _Airport = value; INotifyPropertyChanged(nameof(Airport)); }
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

        public DateTime StartDate
        {
            get { return _StartDate; }
            set
            {
                if (_StartDate != value)
                {
                    _StartDate = value;
                    OnPropertyChanged(nameof(StartDate));
                    OnPropertyChanged(nameof(TotalTime)); // TotalTime depends on StartDate and EndDate
                }
            }
        }

        public DateTime EndDate
        {
            get { return _EndDate; }
            set
            {
                if (_EndDate != value)
                {
                    _EndDate = value;
                    OnPropertyChanged(nameof(EndDate));
                    OnPropertyChanged(nameof(TotalTime)); // TotalTime depends on StartDate and EndDate
                }
            }
        }

        public float TotalTime
        {
            get { return _TotalTime; }
            set
            {
                if (_TotalTime != value)
                {
                    // Round the value to two decimal places
                    _TotalTime = (float)Math.Round(value, 2);
                    INotifyPropertyChanged(nameof(TotalTime));
                }
            }
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

        public float GroundTime
        {
            get { return _GroundTime; }
            set { _GroundTime = value; INotifyPropertyChanged(nameof(GroundTime)); }
        }

        public int Complement_Captain
        {
            get { return _Complement_Captain; }
            set { _Complement_Captain = value; INotifyPropertyChanged(nameof(Complement_Captain)); }
        }
        public int Complement_FO
        {
            get { return _Complement_FO; }
            set { _Complement_FO = value; INotifyPropertyChanged(nameof(Complement_FO)); }
        }
        public int Complement_Cabin_Manager
        {
            get { return _Complement_Cabin_Manager; }
            set { _Complement_Cabin_Manager = value; INotifyPropertyChanged(nameof(Complement_Cabin_Manager)); }
        }
        public int Complement_Flight_Attendant
        {
            get { return _Complement_Flight_Attendant; }
            set { _Complement_Flight_Attendant = value; INotifyPropertyChanged(nameof(Complement_Flight_Attendant)); }
        }

    }
}
