using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Inventory.InvControl_ConstantDemand
{
    public class RefillEOQData:BasicEOQData
    {
        private float _tR; //Refund Time

        private float _R; //Reorder point

        public float R 
        {
            get { return _R; }
            set { _R = value; OnPropertyChanged("R"); }
        }

        public float tR
        {
            get { return _tR; }
            set { _tR = value; OnPropertyChanged("TR"); }
        }
    }
}
