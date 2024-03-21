using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Erp.CommonFiles;
using System.Windows.Input;
using Syncfusion.UI.Xaml.Grid;
using System.Security.Policy;
using Erp.Model.Interfaces;

namespace Erp.ViewModel
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {

        private bool _showDeleted;

        public bool ShowDeleted
        {
            get { return _showDeleted; }
            set
            {
                _showDeleted = value;
                RaisePropertyChanged("ShowDeleted");

            }
        }

        private string _caption;

        public string Caption
        {
            get { return _caption; }
            set
            {
                    _caption = value;
                RaisePropertyChanged("CollectionView");
                
            }
        }





        protected object selectedItem;
        public object SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                selectedItem = value;
                RaisePropertyChanged("SelectedItem");
            }
        }
        protected object selectedItem2;
        public object SelectedItem2
        {
            get
            {
                return selectedItem2;
            }
            set
            {
                selectedItem2 = value;
                RaisePropertyChanged("SelectedItem2");
            }
        }


        public string F7key { get; set; }

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
        private ICollectionView collectionview2;

        public ICollectionView CollectionView2
        {
            get
            {
                return collectionview2;
            }
            set
            {
                collectionview2 = value;
                INotifyPropertyChanged("CollectionView2");
            }
        }
        private ICollectionView resultcollectionview;

        public ICollectionView ResultCollectionView
        {
            get
            {
                return resultcollectionview;
            }
            set
            {
                resultcollectionview = value;
                INotifyPropertyChanged("ResultCollectionView");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void INotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public CommonFunctions CommonFunctions = new CommonFunctions();
        public F7Common F7Common = new F7Common();


        public void RaisePropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }

    }
}
