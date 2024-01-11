using Erp.Model.BasicFiles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Customers
{
     public class ItemQuantityOrderData : INotifyPropertyChanged
    {
        private ItemData _Item { get; set; }
        private CityData _CityDelivered { get; set; }

        private float _Quantity { get; set; }
        private DateTime _DeliveryDate { get; set; }
        private float _UnitCost { get; set; }
        private float _TotalCost { get; set; }

        private float _UnitDiscount { get; set; }
        private bool _IsChecked { get; set; }
        private bool _ExistingFlag { get; set; }

        private bool _NewItemFlag { get; set; }
        public bool IsChecked
        {
            get { return _IsChecked; }
            set { _IsChecked = value; OnPropertyChanged("IsChecked"); }
        }
        public bool ExistingFlag
        {
            get { return _ExistingFlag; }
            set { _ExistingFlag = value; OnPropertyChanged("ExistingFlag"); }
        }
        public bool NewItemFlag
        {
            get { return _NewItemFlag; }
            set { _NewItemFlag = value; OnPropertyChanged("NewItemFlag"); }
        }
        public CityData CityDelivered
        {
            get { return _CityDelivered; }
            set { _CityDelivered = value; OnPropertyChanged("CityDelivered"); }
        }
        //public float TotalCost
        //{
        //    get { return _TotalCost; }
        //    set { _TotalCost = value; OnPropertyChanged("TotalCost"); }
        //}

        public float TotalCost
        {
            get { return _UnitCost * _Quantity; }

        }
        public float UnitDiscount
        {
            get { return _UnitDiscount; }
            set { _UnitDiscount = value; OnPropertyChanged("UnitDiscount"); }
        }
        public float UnitCost
        {
            get { return _UnitCost; }
            set
            {
                if (_UnitCost != value)
                {
                    _UnitCost = value;
                    OnPropertyChanged(nameof(UnitCost));
                    OnPropertyChanged(nameof(TotalCost)); // TotalCost depends on UnitCost, so we raise this too
                }
            }
        }
        public ItemData Item
        {
            get { return _Item; }
            set { _Item = value; OnPropertyChanged("Item"); }
        }
        public float Quantity
        {
            get { return _Quantity; }
            set
            {
                if (_Quantity != value)
                {
                    _Quantity = value;
                    OnPropertyChanged(nameof(Quantity));
                    OnPropertyChanged(nameof(TotalCost)); // TotalCost depends on Quantity, so we raise this too
                }
            }
        }
        public DateTime DeliveryDate
        {
            get { return _DeliveryDate; }
            set { _DeliveryDate = value; OnPropertyChanged("DeliveryDate"); }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public void INotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
