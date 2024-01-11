using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Erp.Model.Enums.BasicEnums;

namespace Erp.Model.BasicFiles
{
    public class PrefectureData : RecordBaseModel
    {
        private int _PrefId { get; set; }
        private int _CountryId { get; set; }

        private string _PrefCode { get; set; }
        private string _PrefDescr { get; set; }
        private string _CountryCode { get; set; }

        private CountryData _Country { get; set; }
        public int PrefId
        {
            get { return _PrefId; }
            set { _PrefId = value; OnPropertyChanged("PrefId"); }
        }

        public string PrefCode
        {
            get { return _PrefCode; }
            set { _PrefCode = value; OnPropertyChanged("PrefCode"); }
        }
        public string PrefDescr
        {
            get { return _PrefDescr; }
            set { _PrefDescr = value; INotifyPropertyChanged("PrefDescr"); }
        }

        public int CountryId
        {
            get { return _CountryId; }
            set { _CountryId = value; OnPropertyChanged("CountryId"); }
        }
        public string CountryCode
        {
            get { return _CountryCode; }
            set { _CountryCode = value; OnPropertyChanged("CountryCode"); }
        }

    }
}
