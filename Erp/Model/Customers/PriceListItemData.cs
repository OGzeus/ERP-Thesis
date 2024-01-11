using Erp.Model.BasicFiles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Customers
{
    public class PriceListItemData : ModelBase
    {
        private int finalitemid { get; set; }

        private ItemData _Item;
        private float _UnitCost { get; set; }

        private float _Discount { get; set; }
        private float _Qmin { get; set; }
        private bool _IsChecked { get; set; }
        private bool _Existing { get; set; }

        public bool Existing
        {
            get { return _Existing; }
            set { _Existing = value; OnPropertyChanged("Existing"); }
        }

        public int FinalItemId
        {
            get { return finalitemid; }
            set { finalitemid = value; OnPropertyChanged("FinalItemId"); }
        }
        public ItemData Item
        {
            get { return _Item; }
            set { _Item = value; OnPropertyChanged("Item"); }
        }
        public float SalesPrice
        {
            get { return _UnitCost; }
            set { _UnitCost = value; OnPropertyChanged("UnitCost"); }
        }
        public float Discount
        {
            get { return _Discount; }
            set { _Discount = value; OnPropertyChanged("Discount"); }
        }
        public float Qmin
        {
            get { return _Qmin; }
            set { _Qmin = value; OnPropertyChanged("Qmin"); }
        }
        public bool IsChecked
        {
            get { return _IsChecked; }
            set { _IsChecked = value; OnPropertyChanged("IsChecked"); }
        }





    }
}
