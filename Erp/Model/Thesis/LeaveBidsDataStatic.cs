using Erp.Model.Enums;
using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Thesis
{
    public class LeaveBidsDataStatic : RecordBaseModel
    {
        public enum ActivePanel
        {
            Fixed_Date_Range,
            Fixed_Number_Of_Days,
            Range_Of_Days

        }

        private ActivePanel _activePanel;
        public ActivePanel CurrentActivePanel
        {
            get { return _activePanel; }
            private set
            {
                if (_activePanel != value)
                {
                    _activePanel = value;
                    OnPropertyChanged("CurrentActivePanel");
                }
            }
        }

        private int _BidId;
        private string _BidCode;

        private int _PriorityLevel;

        private BasicEnums.BidType _BidType;

        private EmployeeData _Employee;
        private ReqScheduleInfoData _Schedule;

        private DateTime _DateFrom;
        private DateTime _DateTo;

        private string _DateFromStr;
        private string _DateToStr;

        private int _NumberOfDays;
        private int _NumberOfDaysMin;
        private int _NumberOfDaysMax;


        public int BidId
        {
            get { return _BidId; }
            set { _BidId = value; ; }
        }
        public string BidCode
        {
            get { return _BidCode; }
            set { _BidCode = value; ; }
        }
        public EmployeeData Employee
        {
            get { return _Employee; }
            set { _Employee = value; ; }
        }
        public ReqScheduleInfoData Schedule
        {
            get { return _Schedule; }
            set { _Schedule = value; ; }
        }
        public int PriorityLevel
        {
            get { return _PriorityLevel; }
            set { _PriorityLevel = value; ; }
        }

        public BasicEnums.BidType BidType
        {
            get { return _BidType; }
            set { _BidType = value; ; }
        }

        public DateTime DateFrom 
        {
            get { return _DateFrom; }
            set { _DateFrom = value; ; }
        }

        public DateTime DateTo
        {
            get { return _DateTo; }
            set { _DateTo = value; ; }
        }

        public string DateFromStr 
        {
            get { return _DateFromStr; }
            set { _DateFromStr = value; ; }
        }

        public string DateToStr 
{
            get { return _DateToStr; }
            set { _DateToStr = value; ; }
        }
        public int NumberOfDays
        {
            get { return _NumberOfDays; }
            set { _NumberOfDays = value; ; }
        }
        public int NumberOfDaysMin
        {
            get { return _NumberOfDaysMin; }
            set { _NumberOfDaysMin = value; ; }
        }
        public int NumberOfDaysMax
        {
            get { return _NumberOfDaysMax; }
            set { _NumberOfDaysMax = value; ; }
        }

        private bool bidflag { get; set; }

        private bool newbidflag { get; set; }
        private bool existingflag { get; set; }

        public bool ExistingFlag
        {
            get { return existingflag; }
            set { existingflag = value; OnPropertyChanged("ExistingFlag"); }
        }
        public bool NewBidFlag
        {
            get { return newbidflag; }
            set { newbidflag = value; OnPropertyChanged("NewBidFlag"); }
        }
        public bool Bidflag
        {
            get { return bidflag; }
            set { bidflag = value; OnPropertyChanged("Bidflag"); }
        }
    }
}
