using Erp.Model.BasicFiles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Manufacture
{
    public class PlannedOrdersData : INotifyPropertyChanged
    {
        public string ItemCode { get; set; }
        public int LotPolicyId { get; set; }

        public float Quantity { get; set; }

        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
