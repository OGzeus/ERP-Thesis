using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Erp.Model
{
    public class F7Data : INotifyPropertyChanged {


       public string F7key { get; set; }

        public Columns SfGridColumns;

        private ICollectionView collectionview;

        public ICollectionView CollectionView
        {
            get
            {
                return collectionview;
            }
            set
            {
                collectionview = value;
                INotifyPropertyChanged("CollectionView");
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


        public void INotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
