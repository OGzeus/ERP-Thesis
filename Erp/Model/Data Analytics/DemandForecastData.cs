using Erp.Model.BasicFiles;
using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Data_Analytics
{
    public class DemandForecastData : RecordBaseModel
    {
        public int ForId { get; set; }
        public string ForCode { get; set; }

        private ItemData _Item;

        public ItemData Item
        {
            get { return _Item; }
            set { _Item = value; INotifyPropertyChanged("Item"); }
        }
        public DateTime Date { get; set; }
        public string DateStr { get; set; }

        public decimal Demand { get; set; }

        private bool _Selected;

        public bool Selected
        {
            get { return _Selected; }
            set { _Selected = value; INotifyPropertyChanged("Selected"); }
        }



    }
}
