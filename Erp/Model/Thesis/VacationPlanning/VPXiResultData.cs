using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Thesis.VacationPlanning
{
    public class VPXiResultData
    {

        private string _LLi { get; set; }

        private string _Date { get; set; }


        private int _LimitLine { get; set; }


        public int LimitLine
        {
            get { return _LimitLine; }
            set { _LimitLine = value; OnPropertyChanged("LimitLine"); }
        }
        public string Date
        {
            get { return _Date; }
            set { _Date = value; OnPropertyChanged("Date"); }
        }
        public string LLi
        {
            get { return _LLi; }
            set { _LLi = value; OnPropertyChanged("LLi"); }
        }
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
