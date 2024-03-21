using Erp.Model.Enums;
using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Thesis
{
    public class CertificationData : RecordBaseModel
    {
        private int _Id { get; set; }

        private string _Code { get; set; }
        private string _Descr { get; set; }
        private int _ValidityPeriod { get; set; }
        private BasicEnums.CertPosition _CertPosition { get; set; }
        private BasicEnums.Timebucket _ValidityTimeBucket { get; set; }

        private DateTime _DateFrom { get; set; }
        private DateTime _DateTo { get; set; }

        public DateTime DateFrom
        {
            get { return _DateFrom; }
            set { _DateFrom = value; OnPropertyChanged("DateFrom"); }
        }

        public DateTime DateTo
        {
            get { return _DateTo; }
            set { _DateTo = value; OnPropertyChanged("DateTo"); }
        }
        public int Id
        {
            get { return _Id; }
            set { _Id = value; OnPropertyChanged("Id"); }
        }

        public string Code
        {
            get { return _Code; }
            set { _Code = value; OnPropertyChanged("Code"); }
        }

        public string Descr
        {
            get { return _Descr; }
            set { _Descr = value; OnPropertyChanged("Descr"); }
        }

        public int ValidityPeriod
        {
            get { return _ValidityPeriod; }
            set { _ValidityPeriod = value; OnPropertyChanged("ValidityPeriod"); }
        }

        public BasicEnums.CertPosition CertPosition
        {
            get { return _CertPosition; }
            set { _CertPosition = value; OnPropertyChanged("CertPosition"); }
        }

        public BasicEnums.Timebucket ValidityTimeBucket
        {
            get { return _ValidityTimeBucket; }
            set { _ValidityTimeBucket = value; OnPropertyChanged("ValidityTimeBucket"); }
        }
    }
}
