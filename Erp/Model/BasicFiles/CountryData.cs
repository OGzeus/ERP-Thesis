using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.BasicFiles
{
    public class CountryData : RecordBaseModel
    {
        private int _CountryId { get; set; }
        private string _CountryCode { get; set; }

        private string _CountryDescr { get; set; }

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


        public string CountryDescr
        {
            get { return _CountryDescr; }
            set { _CountryDescr = value; OnPropertyChanged("CountryDescr"); }
        }

    }
}
