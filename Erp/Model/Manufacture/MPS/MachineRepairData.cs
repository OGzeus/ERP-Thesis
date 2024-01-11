using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Manufacture.MPS
{
    public class MachineRepairData : INotifyPropertyChanged
    {
        private int _Id;

        public int Id
        {
            get { return _Id; }
            set { _Id = value; OnPropertyChanged("Id"); }
        }

        private int _MPSId;

        public int MPSId
        {
            get { return _MPSId; }
            set { _MPSId = value; OnPropertyChanged("MPSId"); }
        }
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

        private string _MachCode;
        private string _MachDescr;

        private int _NumberOfRepairsMPS;
        private DateTime _RepairDate;
        private string _RepairDateStr;

        public DateTime RepairDate
        {
            get { return _RepairDate; }
            set { _RepairDate = value; OnPropertyChanged("RepairDate"); }
        }
        public string RepairDateStr
        {
            get { return _RepairDateStr; }
            set { _RepairDateStr = value; OnPropertyChanged("RepairDateStr"); }
        }
        public string MachCode
        {
            get { return _MachCode; }
            set { _MachCode = value; OnPropertyChanged("MachCode"); }
        }

        public string MachDescr
        {
            get { return _MachDescr; }
            set { _MachDescr = value; OnPropertyChanged("MachDescr"); }
        }



        public int NumberOfRepairsMPS
        {
            get { return _NumberOfRepairsMPS; }
            set { _NumberOfRepairsMPS = value; OnPropertyChanged("NumberOfRepairsMPS"); }
        }


        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
