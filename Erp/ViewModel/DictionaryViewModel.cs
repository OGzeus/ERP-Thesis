using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.ViewModel
{
    public class DictionaryViewModel : ViewModelBase
    {

        public DictionaryViewModel()
        {
            Rows = new ObservableCollection<Object>();
        }

        private ObservableCollection<Object> rows;
        public ObservableCollection<Object> Rows
        {
            get
            {
                return rows;
            }
            set
            {
                rows = value;
                RaisePropertyChanged("Rows");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }
    }
}
