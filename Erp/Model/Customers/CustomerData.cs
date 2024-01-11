using Erp.Model.BasicFiles;
using Erp.Model.Enums;
using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Customers
{
    public class CustomerData : RecordBaseModel
    {

        private int _Id;
        private string _Code { get; set; }
        private string _Descr { get; set; }
        private string _Email { get; set; }
        private int _Phone { get; set; }
        private string _Adress { get; set; }

        private BasicEnums.CustomerType _CustomerType { get; set; }
        private string _PostalCode { get; set; }

        private bool _PromptPayer { get; set; }
        private CityData _City { get; set; }
        private PrefectureData _Prefecture { get; set; }
        private CountryData _Country { get; set; }
        private PriceListData _PriceList { get; set; }



        public string PostalCode
        {
            get { return _PostalCode; }
            set { _PostalCode = value; OnPropertyChanged("PostalCode"); }
        }
        public PriceListData PriceList
        {
            get { return _PriceList; }
            set { _PriceList = value; OnPropertyChanged("PriceList"); }
        }

        public CityData City
        {
            get { return _City; }
            set { _City = value; OnPropertyChanged("City"); }
        }
        public PrefectureData Prefecture
        {
            get { return _Prefecture; }
            set { _Prefecture = value; OnPropertyChanged("Prefecture"); }
        }
        public CountryData Country
        {
            get { return _Country; }
            set { _Country = value; OnPropertyChanged("Country"); }
        }


        public int Id
        {
            get { return _Id; }
            set { _Id = value; OnPropertyChanged("Id"); }
        }

        public BasicEnums.CustomerType CustomerType
        {
            get { return _CustomerType; }
            set { _CustomerType = value; OnPropertyChanged("CustomerType"); }
        }

        public bool PromptPayer
        {
            get { return _PromptPayer; }
            set { _PromptPayer = value; OnPropertyChanged("PromptPayer"); }
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
        public string Email
        {
            get { return _Email; }
            set { _Email = value; OnPropertyChanged("Email"); }
        }
        public int Phone
        {
            get { return _Phone; }
            set { _Phone = value; OnPropertyChanged("Phone"); }
        }
        public string Adress
        {
            get { return _Adress; }
            set { _Adress = value; OnPropertyChanged("Adress"); }
        }




    }

}
