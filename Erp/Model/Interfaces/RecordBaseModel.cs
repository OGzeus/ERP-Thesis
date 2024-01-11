using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Interfaces
{
    public class RecordBaseModel : INotifyPropertyChanged
    {
        private bool _IsDeleted { get; set; }

        public bool IsDeleted
        {
            get { return _IsDeleted; }
            set { _IsDeleted = value; INotifyPropertyChanged("IsDeleted"); }
        }
        public RecordBaseModel() 
        {
        }

        // Define the PropertyChanged event
        public event PropertyChangedEventHandler PropertyChanged;

        // Method to raise the PropertyChanged event
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public void INotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
