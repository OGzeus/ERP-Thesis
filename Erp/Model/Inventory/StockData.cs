using CommunityToolkit.Mvvm.ComponentModel;
using Erp.Model.BasicFiles;
using Erp.Model.Interfaces;
using Erp.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Inventory
{
    public class StockData : RecordBaseModel
    {
        private int finalitemid { get; set; }
        private ItemData stockitem  { get; set; }
        private float quantity { get; set; }
        private bool _StockItemFlag { get; set; }
        private bool newitemflag { get; set; }
        private bool _ExistingFlag { get; set; }
        public ItemData StockItem
        {
            get { return stockitem; }
            set { stockitem = value; OnPropertyChanged("StockItem"); }
        }


        public float Quantity
        {
            get { return quantity; }
            set { quantity = value; OnPropertyChanged("Quantity"); }
        }

        public bool StockItemFlag
        {
            get { return _StockItemFlag; }
            set { _StockItemFlag = value; OnPropertyChanged("StockItemFlag"); }
        }
        public bool ExistingFlag
        {
            get { return _ExistingFlag; }
            set { _ExistingFlag = value; OnPropertyChanged("ExistingFlag"); }
        }
        public bool NewItemFlag
        {
            get { return newitemflag; }
            set { newitemflag = value; OnPropertyChanged("NewItemFlag"); }
        }

        public int FinalItemId
        {
            get { return finalitemid; }
            set { finalitemid = value; OnPropertyChanged("FinalItemId"); }
        }

    }
}

