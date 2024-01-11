using Erp.Model.BasicFiles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Suppliers
{
    public class SupplierInfoSearchFilterData : INotifyPropertyChanged
    {

        private string _SupplierCodeFrom { get; set; }
        private string _SupplierCodeTo { get; set; }

        private string _SupplierDescrFrom { get; set; }

        private string _SupplierDescrTo { get; set; }

        private string _CountryCodeFrom { get; set; }
        private string _PrefectureCodeFrom { get; set; }
        private string _CityCodeFrom { get; set; }

        private string _CountryCodeTo { get; set; }
        private string _PrefectureCodeTo { get; set; }
        private string _CityCodeTo { get; set; }


        private string _SupplierType { get; set; }

        public string SupplierCodeFrom
        {
            get { return _SupplierCodeFrom; }
            set { _SupplierCodeFrom = value; OnPropertyChanged("SupplierCodeFrom"); }
        }
        public string SupplierCodeTo
        {
            get { return _SupplierCodeTo; }
            set { _SupplierCodeTo = value; OnPropertyChanged("SupplierCodeTo"); }
        }
        public string SupplierDescrFrom
        {
            get { return _SupplierDescrFrom; }
            set { _SupplierDescrFrom = value; OnPropertyChanged("SupplierDescrFrom"); }
        }

        public string SupplierDescrTo
        {
            get { return _SupplierDescrTo; }
            set { _SupplierDescrTo = value; OnPropertyChanged("SupplierDescrTo"); }
        }

        public string CountryCodeFrom
        {
            get { return _CountryCodeFrom; }
            set { _CountryCodeFrom = value; OnPropertyChanged("CountryCodeFrom"); }
        }

        public string CountryCodeTo
        {
            get { return _CountryCodeTo; }
            set { _CountryCodeTo = value; OnPropertyChanged("CountryCodeTo"); }
        }
        public string PrefectureCodeFrom
        {
            get { return _PrefectureCodeFrom; }
            set { _PrefectureCodeFrom = value; OnPropertyChanged("PrefectureCodeFrom"); }
        }

        public string PrefectureCodeTo
        {
            get { return _PrefectureCodeFrom; }
            set { _PrefectureCodeFrom = value; OnPropertyChanged("PrefectureCodeTo"); }
        }

        public string CityCodeFrom
        {
            get { return _CityCodeFrom; }
            set { _CityCodeFrom = value; OnPropertyChanged("CityCodeFrom"); }
        }

        public string CityCodeTo
        {
            get { return _CityCodeTo; }
            set { _CityCodeTo = value; OnPropertyChanged("CityCodeTo"); }
        }


        public string SupplierType
        {
            get { return _SupplierType; }
            set { _SupplierType = value; OnPropertyChanged("SupplierType"); }
        }


        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
