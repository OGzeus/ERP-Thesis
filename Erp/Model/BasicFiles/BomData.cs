using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.BasicFiles
{
    public class BomData : INotifyPropertyChanged
    {
        private int finalitemid { get; set; }

        private ItemData bomitem { get; set; }

        private float bompercentage { get; set; }
        private bool bomitemflag { get; set; }

        private bool newitemflag { get; set; }
        private bool _ExistingFlag { get; set; }

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


        public ItemData BomItem
        {
            get { return bomitem; }
            set { bomitem = value; OnPropertyChanged("BomItem"); }
        }

        public float BomPercentage
        {
            get { return bompercentage; }
            set { bompercentage = value; OnPropertyChanged("BomPercentage"); }
        }

        public bool BomItemFlag
        {
            get { return bomitemflag; }
            set { bomitemflag = value; OnPropertyChanged("BomItemFlag"); }
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

