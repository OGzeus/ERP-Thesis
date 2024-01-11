using Erp.Model.BasicFiles;
using Erp.Model.Enums;
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
    public class CustomerOrderData : RecordBaseModel
    {


        private string _CustOrderId;
        private CustomerData _Customer;
        private PriceListData _PriceList;
        private string _CartId;

        private ObservableCollection<ItemQuantityOrderData> _CartData;
        private DateTime _DateCreated;
        private DateTime? _DeliveryDate;

        private string _Notes;
        private BasicEnums.Incoterms _Incoterms;
        private BasicEnums.OrderStatus _OrderStatus;

        public string CartId
        {
            get { return _CartId; }
            set { _CartId = value; OnPropertyChanged("CartId"); }
        }


        public PriceListData PriceList
        {
            get { return _PriceList; }
            set { _PriceList = value; OnPropertyChanged("PriceList"); }
        }
        public string Notes
        {
            get { return _Notes; }
            set { _Notes = value; OnPropertyChanged("Notes"); }
        }
        public BasicEnums.Incoterms Incoterms
        {
            get { return _Incoterms; }
            set { _Incoterms = value; OnPropertyChanged("Incoterms"); }
        }
        public BasicEnums.OrderStatus OrderStatus
        {
            get { return _OrderStatus; }
            set { _OrderStatus = value; OnPropertyChanged("OrderStatus"); }
        }

        public ObservableCollection<ItemQuantityOrderData> CartData
        {
            get { return _CartData; }
            set { _CartData = value; OnPropertyChanged("CartData"); }
        }

        public string CustOrderId
        {
            get { return _CustOrderId; }
            set { _CustOrderId = value; OnPropertyChanged("CustOrderId"); }
        }

        public CustomerData Customer
        {
            get { return _Customer; }
            set { _Customer = value; OnPropertyChanged("Customer"); }
        }

        public DateTime DateCreated
        {
            get { return _DateCreated; }
            set { _DateCreated = value; OnPropertyChanged("DateCreated"); }
        }
        public DateTime? DeliveryDate
        {
            get { return _DeliveryDate; }
            set { _DeliveryDate = value; OnPropertyChanged("DeliveryDate"); }
        }

        



    }
}
