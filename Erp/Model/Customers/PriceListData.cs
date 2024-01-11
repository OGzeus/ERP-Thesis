using Erp.Model.BasicFiles;
using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Customers
{
    public class PriceListData : RecordBaseModel
    {

        private int _Id;
        private string _Code { get; set; }
        private string _Descr { get; set; }

        private bool _Retail { get; set; }
        private bool _Wholesale { get; set; }

        private ObservableCollection<PriceListItemData> _ItemsInfo;
        private CustomerData _CustomerData;

        private DateTime _DateStart {  get; set; }
        private DateTime _DateEnd { get; set; }
        public DateTime DateStart
        {
            get { return _DateStart; }
            set { _DateStart = value; OnPropertyChanged("DateStart"); }
        }
        public DateTime DateEnd
        {
            get { return _DateEnd; }
            set { _DateEnd = value; OnPropertyChanged("DateEnd"); }
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
        public bool Retail
        {
            get { return _Retail; }
            set { _Retail = value; OnPropertyChanged("Retail"); }
        }
        public bool Wholesale
        {
            get { return _Wholesale; }
            set { _Wholesale = value; OnPropertyChanged("Wholesale"); }
        }
        public ObservableCollection<PriceListItemData> ItemsInfo
        {
            get { return _ItemsInfo; }
            set { _ItemsInfo = value; OnPropertyChanged("ItemsInfo"); }
        }
        public CustomerData CustomerData
        {
            get { return _CustomerData; }
            set { _CustomerData = value; OnPropertyChanged("CustomerData"); }
        }


    }
}
