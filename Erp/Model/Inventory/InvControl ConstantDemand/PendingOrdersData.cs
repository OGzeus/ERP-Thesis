using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Inventory.InvControl_ConstantDemand
{
    public class PendingOrdersData : BasicEOQData
    {
        private double _b; // cost per unit of time shortage

        private double _F; // Percentage of Demand that is satisfied by inventory

        public double B
        {
            get { return _b; }
            set { _b = value; OnPropertyChanged("B"); }
        }

        public double F
        {
            get { return _F; }
            set { _F = value; OnPropertyChanged("F"); }
        }
    }
}
