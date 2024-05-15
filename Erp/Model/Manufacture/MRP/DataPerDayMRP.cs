using Erp.Model.BasicFiles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Manufacture.MRP
{
    public class DataPerDayMRP
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
        public double Demand { get; set; }
        public double Make { get; set; }
        public double Stock { get; set; }
        public double Backlog { get; set; }

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
