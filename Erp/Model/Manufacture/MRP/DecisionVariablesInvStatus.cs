using Erp.Model.BasicFiles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Manufacture.MRP
{
    public class DecisionVariablesInvStatus
    {
        private ItemData _Item;

        public ItemData Item
        {
            get { return _Item; }
            set
            {
                _Item = value;
                OnPropertyChanged(nameof(Item));
            }
        }
        public string ItemCode { get; set; }
        public string Date { get; set; }
        public double SIValue { get; set; }
        public double BIValue { get; set; }

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
