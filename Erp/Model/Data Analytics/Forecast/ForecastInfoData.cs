using Erp.Model.BasicFiles;
using Erp.Model.Customers;
using Erp.Model.Enums;
using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Data_Analytics.Forecast
{
    public class ForecastInfoData : RecordBaseModel
    {
        public int ID { get; set; }
        public ItemData Item { get; set; }
        public string ForCode { get; set; }
        public string ForDescr { get; set; }
        public string Notes { get; set; }

        public int NumberOfBuckets { get; set; }
        public BasicEnums.PeriodType PeriodType { get; set; }



        public int PeriodNumber { get; set; }

        private DateTime _DateFrom { get; set; }
        private DateTime _DateTo { get; set; }

        private bool _MRPForecast { get; set; }

        public bool MRPForecast
        {
            get { return _MRPForecast; }
            set { _MRPForecast = value; INotifyPropertyChanged("MRPForecast"); }
        }
        public DateTime DateFrom
        {
            get { return _DateFrom; }
            set { _DateFrom = value; INotifyPropertyChanged("DateFrom"); }
        }
        public DateTime DateTo
        {
            get { return _DateTo; }
            set { _DateTo = value; INotifyPropertyChanged("DateTo"); }
        }

        private string _DateFromStr { get; set; }
        private string _DateToStr { get; set; }

        public string DateFromStr
        {
            get { return _DateFromStr; }
            set { _DateFromStr = value; INotifyPropertyChanged("DateFromStr"); }
        }
        public string DateToStr
        {
            get { return _DateToStr; }
            set { _DateToStr = value; INotifyPropertyChanged("DateTo"); }
        }



        public bool ItemsFlag { get; set; }

        private ObservableCollection<DemandForecastData> _DemandForecast;

        public ObservableCollection<DemandForecastData> DemandForecast
        {
            get { return _DemandForecast; }
            set { _DemandForecast = value; INotifyPropertyChanged("DemandForecast"); }
        }


        private int _HoursPerTimeBucket;

        public int HoursPerTimeBucket
        {
            get { return _HoursPerTimeBucket; }
            set { _HoursPerTimeBucket = value; INotifyPropertyChanged("HoursPerTimeBucket"); }
        }
        private BasicEnums.Timebucket _TimeBucket;

        public BasicEnums.Timebucket TimeBucket
        {
            get { return _TimeBucket; }
            set
            {
                _TimeBucket = value;
                INotifyPropertyChanged("TimeBucket");
            }
        }


    }
}
