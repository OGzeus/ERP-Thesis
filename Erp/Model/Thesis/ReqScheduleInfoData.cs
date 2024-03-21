using Erp.Model.Data_Analytics;
using Erp.Model.Enums;
using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Thesis
{
    public class ReqScheduleInfoData : RecordBaseModel
    {
        public int ID { get; set; }
        public string ReqCode { get; set; }
        public string ReqDescr { get; set; }
        public string Notes { get; set; }

        private DateTime _DateFrom { get; set; }
        private DateTime _DateTo { get; set; }

        private bool _MainSchedule { get; set; }

        public bool MainSchedule
        {
            get { return _MainSchedule; }
            set { _MainSchedule = value; INotifyPropertyChanged("MainSchedule"); }
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

        private int _LimitLineFixed { get; set; }

        public int LimitLineFixed
        {
            get { return _LimitLineFixed; }
            set { _LimitLineFixed = value; INotifyPropertyChanged("LimitLineFixed"); }
        }

        public bool ItemsFlag { get; set; }

        private ObservableCollection<ReqScheduleRowsData> _ReqScheduleRowsData;

        public ObservableCollection<ReqScheduleRowsData> ReqScheduleRowsData
        {
            get { return _ReqScheduleRowsData; }
            set { _ReqScheduleRowsData = value; INotifyPropertyChanged("ReqScheduleRowsData"); }
        }
    }
}
