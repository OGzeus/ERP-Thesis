using Erp.Model.BasicFiles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Manufacture.MRP
{
    public class DecisionVariableY
    {
        private ItemData _ItemFrom;
        public ItemData ItemFrom
        {
            get { return _ItemFrom; }
            set
            {
                _ItemFrom = value;
                OnPropertyChanged(nameof(_ItemFrom));
            }
        }

        private ItemData _ItemTo;

        public ItemData ItemTo
        {
            get { return _ItemTo; }
            set
            {
                _ItemTo = value;
                OnPropertyChanged(nameof(ItemTo));
            }
        }
        public string ItemCodeFrom { get; set; }
        public string ItemCodeTo { get; set; }

        public string WorkCenter { get; set; }
        public string Date { get; set; }
        public int Setup { get; set; }
        public double Value { get; set; }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public void RaisePropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }
        #endregion
    }
}
