using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Thesis.VacationPlanning
{
    public class VPYijResultsData
    {
        private EmployeeData _Employee;

        public EmployeeData Employee
        {
            get { return _Employee; }
            set
            {
                _Employee = value;
                OnPropertyChanged(nameof(Employee));
            }
        }

        private LeaveBidsDataStatic _LeaveBidData;
        public LeaveBidsDataStatic LeaveBidData { get; set; }

        public string EmpCode { get; set; }
        public string LeaveBidCode { get; set; }
        public string Date { get; set; }
        public List<string> Dates { get; set; }

        public string Yij { get; set; }
        public string Yijr { get; set; }
        public string Yijrz { get; set; }

        public double YijFlag { get; set; }
        public double ConfirmedBidFlag { get; set; }

        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

        public string DateFromStr { get; set; }
        public string DateToStr { get; set; }

        public int NumberOfDays { get; set; }
        public int NumberOfDaysMin { get; set; }
        public int NumberOfDaysMax { get; set; }


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
