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

        private ObservableCollection<DemandForecastData> _DemandForecast;

        public ObservableCollection<DemandForecastData> DemandForecast
        {
            get { return _DemandForecast; }
            set
            {
                if (1==1)
                {
                    _DemandForecast = value;
                    INotifyPropertyChanged("DemandForecast");

                    // Unsubscribe from previous collection's PropertyChanged events
                    UnsubscribeFromPropertyChangedEvents();

                    // Subscribe to PropertyChanged events of new collection
                    SubscribeToPropertyChangedEvents();
                }
            }
        }
        private void SubscribeToPropertyChangedEvents()
        {
            foreach (var item in DemandForecast)
            {
                item.PropertyChanged += DemandForecastData_PropertyChanged;
            }
        }

        private void UnsubscribeFromPropertyChangedEvents()
        {
            foreach (var item in DemandForecast)
            {
                item.PropertyChanged -= DemandForecastData_PropertyChanged;
            }
        }

        private bool _isProcessingChange = false;

        private void DemandForecastData_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Check if already processing a change to avoid recursion
            if (_isProcessingChange)
            {
                return;
            }

            if (e.PropertyName == "Selected")
            {
                _isProcessingChange = true;

                DemandForecastData changedItem = (DemandForecastData)sender;
                bool newValue = changedItem.Selected;
                string itemCode = changedItem.Item.ItemCode;

                // Update Selected property of items with the same ItemCode
                foreach (var item in DemandForecast.Where(d => d.Item.ItemCode == itemCode))
                {
                    item.Selected = newValue;
                }

                _isProcessingChange = false;
            }
        }








    }
}
