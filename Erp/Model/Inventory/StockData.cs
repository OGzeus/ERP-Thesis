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
        private double _stock { get; set; }
        private double _InvMin { get; set; }
        private double _InvMax { get; set; }

        private bool _StockItemFlag { get; set; }
        private bool newitemflag { get; set; }
        private bool _ExistingFlag { get; set; }
        public ItemData StockItem
        {
            get { return stockitem; }
            set { stockitem = value; OnPropertyChanged("StockItem"); }
        }


        public double Stock  
        {
            get { return _stock; }
            set { _stock = value; OnPropertyChanged("Stock"); }
        }
        public double InvMax
        {
            get { return _InvMax; }
            set { _InvMax = value; OnPropertyChanged("InvMax"); }
        }
        public double InvMin
        {
            get { return _InvMin; }
            set { _InvMin = value; OnPropertyChanged("InvMin"); }
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

