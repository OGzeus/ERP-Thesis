using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Inventory.InvControl_ConstantDemand
{
    public class LostSalesData : BasicEOQData
    {
        private float _r; // Revenue(Price) Per Unit of Sale

        private float _bL; // Cost per shortage unit

        private float _F; // Percentage of demand that is immediately satisfied by inventory

        public float r
        {
            get { return _r; }
            set { _r = value; OnPropertyChanged("B"); }
        }

        public float bL
        {
            get { return _bL; }
            set { _bL = value; OnPropertyChanged("bL"); }
        }

        public float F
        {
            get { return _F; }
            set { _F = value; OnPropertyChanged("F"); }
        }
    }
}
