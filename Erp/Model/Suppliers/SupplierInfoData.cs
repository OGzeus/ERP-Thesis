using Erp.Model.BasicFiles;
using Syncfusion.UI.Xaml.Charts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Suppliers
{
    public class SupplierInfoData : INotifyPropertyChanged
    {


        private string _SupplierCode { get; set; }
        private string _SupplierDescr { get; set; }
        private string _Email { get; set; }
        private string _Phone { get; set; }
        private string _Adress1 { get; set; }

        private string _SupplierType { get; set; }
        private bool _Fason { get; set; }

        private string _CountryCode{ get; set; }
        private string _PrefCode { get; set; }
        private string _CityCode { get; set; }

        private string _CountryDescr { get; set; }
        private string _PrefDescr{ get; set; }
        private string _CityDescr { get; set; }


        public string SupplierType
        {
            get { return _SupplierType; }
            set { _SupplierType = value; OnPropertyChanged("SupplierType"); }
        }

        public bool Fason
        {
            get { return _Fason; }
            set { _Fason = value; OnPropertyChanged("Fason"); }
        }

        public string SupplierCode
        {
            get { return _SupplierCode; }
            set { _SupplierCode = value; OnPropertyChanged("SupplierCode"); }
        }
        public string SupplierDescr
        {
            get { return _SupplierDescr; }
            set { _SupplierDescr = value; OnPropertyChanged("SupplierDescr"); }
        }
        public string Email
        {
            get { return _Email; }
            set { _Email = value; OnPropertyChanged("Email"); }
        }
        public string Phone
        {
            get { return _Phone; }
            set { _Phone = value; OnPropertyChanged("Phone"); }
        }
        public string Adress1
        {
            get { return _Adress1; }
            set { _Adress1 = value; OnPropertyChanged("Adress1"); }
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
        public string PrefCode
        {
            get { return _PrefCode; }
            set { _PrefCode = value; OnPropertyChanged("PrefCode"); }
        }
        public string PrefDescr
        {
            get { return _CountryCode; }
            set { _CountryCode = value; OnPropertyChanged("PrefDescr"); }
        }
        public string CityCode
        {
            get { return _CityCode; }
            set { _CityCode = value; OnPropertyChanged("CityCode"); }
        }
        public string CityDescr
        {
            get { return _CityDescr; }
            set { _CityDescr = value; OnPropertyChanged("CityDescr"); }
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
