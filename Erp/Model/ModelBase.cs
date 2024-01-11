using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model
{
    public class ModelBase : INotifyPropertyChanged
    {
        private bool _isDeleted;

        public bool IsDeleted
        {
            get { return _isDeleted; }
            set { _isDeleted = value; OnPropertyChanged("IsDeleted"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
