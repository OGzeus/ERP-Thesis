using Erp.Model.BasicFiles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Manufacture.MPS
{
    public class MachRepairResultsData
    {
        private MachineData _Mach;

        public MachineData Mach
        {
            get { return _Mach; }
            set
            {
                _Mach = value;
                OnPropertyChanged(nameof(Mach));
            }
        }

        public string Date { get; set; }
        public List<string> Dates { get; set; }
        public string MachCode { get; set; }
        public double NumberOfRepairs { get; set; }




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
