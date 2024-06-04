using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.BasicFiles
{
    public class CityData : RecordBaseModel
    {
        private int _CityId { get; set; }
        private int _PrefId { get; set; }
        private int _CountryId { get; set; }

        private string _CityCode { get; set; }
        private string _CityDescr { get; set; }
        private string _CountryCode { get; set; }

        private string _CountryDescr { get; set;}

        private string _PrefCode { get; set; }
        private string _PrefDescr { get; set; }
        private float _Longitude { get; set; }
        private float _Latitude { get; set; }

        private int _Population { get; set; }
        private int _Demand { get; set; }

        private bool _Selected { get; set; }

        public int PrefId
        {
            get { return _PrefId; }
            set { _PrefId = value; OnPropertyChanged("PrefId"); }
        }
        public int CountryId
        {
            get { return _CountryId; }
            set { _CountryId = value; OnPropertyChanged("CountryId"); }
        }
        public int CityId
        {
            get { return _CityId; }
            set { _CityId = value; OnPropertyChanged("CityId"); }
        }
        public float Longitude
        {
            get { return _Longitude; }
            set { _Longitude = value; OnPropertyChanged("Longitude"); }
        }
        public float Latitude
        {
            get { return _Latitude; }
            set { _Latitude = value; OnPropertyChanged("Latitude"); }
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
            get { return _PrefDescr; }
            set { _PrefDescr = value; OnPropertyChanged("PrefDescr"); }
        }
        public int Population
        {
            get { return _Population; }
            set { _Population = value; OnPropertyChanged("Population"); }
        }
        public int Demand
        {
            get { return _Demand; }
            set { _Demand = value; OnPropertyChanged("Demand"); }
        }
        public bool Selected
        {
            get { return _Selected; }
            set { _Selected = value; OnPropertyChanged("Selected"); }
        }
    }
}
