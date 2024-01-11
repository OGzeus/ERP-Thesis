using Erp.Model.BasicFiles;
using Erp.Model.Customers;
using Erp.Model.Inventory;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Manufacture
{
    public class MachMaintenanceData : INotifyPropertyChanged
    {

        private bool _Daily { get; set; }
        private bool _Weekly { get; set; }
        private bool _Monthly { get; set; }
        private bool _Flexible { get; set; }


        private DateTime _DateStart { get; set; }
        private DateTime _DateEnd { get; set; }

        private List<string> _daysOfWeek;
        private List<string> _daysOfMonth;
        private string _selectedDayOfWeek;
        private string _selectedDayOfMonth;

        public List<string> DaysOfWeek
        {
            get { return _daysOfWeek; }
            set { _daysOfWeek = value; OnPropertyChanged("DaysOfWeek"); }
        }

        public List<string> DaysOfMonth
        {
            get { return _daysOfMonth; }
            set { _daysOfMonth = value; OnPropertyChanged("DaysOfMonth"); }
        }

        public string SelectedDayOfWeek
        {
            get { return _selectedDayOfWeek; }
            set { _selectedDayOfWeek = value; OnPropertyChanged("SelectedDayOfWeek"); }
        }

        public string SelectedDayOfMonth
        {
            get { return _selectedDayOfMonth; }
            set { _selectedDayOfMonth = value; OnPropertyChanged("SelectedDayOfMonth"); }
        }

        private MachineData _Machine;
        private FactoryData _Factory;
        private InventoryData _Inventory;


        private string _DocumentPath;

        public string DocumentPath
        {
            get { return _DocumentPath; }
            set { _DocumentPath = value; OnPropertyChanged("DocumentPath"); }
        }


        public bool Daily
        {
            get { return _Daily; }
            set { _Daily = value; OnPropertyChanged("Daily"); }
        }

        public bool Weekly
        {
            get { return _Weekly; }
            set { _Weekly = value; OnPropertyChanged("Weekly"); }
        }
        public bool Monthly
        {
            get { return _Monthly; }
            set { _Monthly = value; OnPropertyChanged("Monthly"); }
        }
        public bool Flexible
        {
            get { return _Flexible; }
            set { _Flexible = value; OnPropertyChanged("Flexible"); }
        }
        public DateTime DateStart
        {
            get { return _DateStart; }
            set { _DateStart = value; OnPropertyChanged("DateStart"); }
        }
        public DateTime DateEnd
        {
            get { return _DateEnd; }
            set { _DateEnd = value; OnPropertyChanged("DateEnd"); }
        }
        public MachineData Machine
        {
            get { return _Machine; }
            set { _Machine = value; OnPropertyChanged("Machine"); }
        }
        public FactoryData Factory
        {
            get { return _Factory; }
            set { _Factory = value; OnPropertyChanged("Factory"); }
        }

        public InventoryData Inventory
        {
            get { return _Inventory; }
            set { _Inventory = value; OnPropertyChanged("Inventory"); }
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
