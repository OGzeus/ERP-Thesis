using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.BasicFiles
{
    public class SupItemData 
    {

        private int itemid { get; set; }

        private string itemcode { get; set; }

        private string itemdescr { get; set; }
        private string mesunit { get; set; }
        private string itemtype { get; set; }
        private string assembly { get; set; }

        private bool supplierflag { get; set; }

        private string pricelistflag { get; set; }


        public int ItemId
        {
            get { return itemid; }
            set { itemid = value; OnPropertyChanged("ItemId"); }
        }
        public string ItemCode
        {
            get { return itemcode; }
            set { itemcode = value; OnPropertyChanged("ItemCode"); }
        }



        public string ItemDescr
        {
            get { return itemdescr; }
            set { itemdescr = value; OnPropertyChanged("ItemDescr"); }
        }

        public string MesUnit
        {
            get { return mesunit; }
            set { mesunit = value; OnPropertyChanged("MesUnit"); }
        }

        public string ItemType
        {
            get { return itemtype; }
            set { itemtype = value; OnPropertyChanged("ItemType"); }
        }

        public string Assembly
        {
            get { return assembly; }
            set { assembly = value; OnPropertyChanged("Assembly"); }
        }

        public bool SupplierFlag
        {
            get { return supplierflag; }
            set { supplierflag = value; OnPropertyChanged("SupplierFlag"); }
        }

        public string PriceListFlag
        {
            get { return pricelistflag; }
            set { pricelistflag = value; OnPropertyChanged("PriceListFlag"); }
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
