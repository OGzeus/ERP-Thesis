using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Thesis
{
    public class LeaveStatusData : RecordBaseModel
    {

        private EmployeeData _Employee { get; set; }

        private int _Total { get; set; }
        private int _Used { get; set; }
        private int _CurrentBalance { get; set; }
        private int _ProjectedBalance{ get; set; }


        public EmployeeData Employee
        {
            get { return _Employee; }
            set { _Employee = value; OnPropertyChanged("Employee"); }
        }



        public int Total
        {
            get { return _Total; }
            set
            {
                if (_Total != value)
                {
                    _Total = value;
                    OnPropertyChanged(nameof(Total));
                    OnPropertyChanged(nameof(CurrentBalance)); // TotalCost depends on UnitCost, so we raise this too
                }
            }
        }

        public int Used
        {
            get { return _Used; }
            set
            {
                if (_Used != value)
                {
                    _Used = value;
                    OnPropertyChanged(nameof(Used));
                    OnPropertyChanged(nameof(CurrentBalance)); // TotalCost depends on UnitCost, so we raise this too
                }
            }
        }
        public int CurrentBalance
        {
            get { return _Total - _Used; } // Computed value based on Total and Used
        }

        public int ProjectedBalance
        {
            get { return _ProjectedBalance; }
            set { _ProjectedBalance = value; OnPropertyChanged("ProjectedBalance"); }
        }
    }
}
