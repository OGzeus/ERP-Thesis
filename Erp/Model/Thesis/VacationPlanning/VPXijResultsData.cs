using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Erp.Model.Thesis.VacationPlanning
{
    public class VPXijResultsData
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



        public string EmpCode { get; set; }
        public string Xij { get; set; }
        public double XijFlag { get; set; }

        public string Date { get; set; }
        public List<string> Dates { get; set; }


        public bool ConfirmedBidFlag { get; set; }




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
