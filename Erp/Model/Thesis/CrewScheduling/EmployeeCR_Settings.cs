using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Thesis.CrewScheduling
{
    public class EmployeeCR_Settings : RecordBaseModel
    {
        private int _UpperBound { get; set; }

        private int _LowerBound { get; set; }

        public int UpperBound
        {
            get { return _UpperBound; }
            set { _UpperBound = value; OnPropertyChanged("UpperBound"); }
        }
        public int LowerBound
        {
            get { return _LowerBound; }
            set { _LowerBound = value; OnPropertyChanged("LowerBound"); }
        }
    }
}
