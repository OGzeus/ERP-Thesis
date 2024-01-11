using Erp.Model.Manufacture;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.BasicFiles
{
    public class ItemProcessData : INotifyPropertyChanged
    {
        private int finalid { get; set; }

        private MachineData machine { get; set; }

        private float productiontime { get; set; }
        private int machineorder { get; set; }

        private bool existingflag { get; set; }
        private bool classicflag { get; set; }

        private bool newprocessflag { get; set; }
        private bool _IsDeleted { get; set; }

        public int MachineOrder
        {
            get { return machineorder; }
            set { machineorder = value; OnPropertyChanged("MachineOrder"); }
        }
        public bool IsDeleted
        {
            get { return _IsDeleted; }
            set { _IsDeleted = value; OnPropertyChanged("IsDeleted"); }
        }
        public bool NewProcessFlag
        {
            get { return newprocessflag; }
            set { newprocessflag = value; OnPropertyChanged("NewProcessFlag"); }
        }

        public int Finalid
        {
            get { return finalid; }
            set { finalid = value; OnPropertyChanged("Finalid"); }
        }


        public MachineData Machine
        {
            get { return machine; }
            set { machine = value; OnPropertyChanged("Machine"); }
        }

        public float ProductionTime
        {
            get { return productiontime; }
            set { productiontime = value; OnPropertyChanged("ProductionTime"); }
        }

        public bool ExistingFlag
        {
            get { return existingflag; }
            set { existingflag = value; OnPropertyChanged("ExistingFlag"); }
        }
        public bool ClassicFlag
        {
            get { return classicflag; }
            set { classicflag = value; OnPropertyChanged("ExistingFlag"); }
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