using Erp.Commands;
using Erp.CommonFiles;
using Erp.Helper;
using Erp.Model;
using Erp.Model.BasicFiles;
using Erp.Model.Suppliers;
using Erp.View;
using Erp.ViewModel.BasicFiles;
using FontAwesome.Sharp;
using Syncfusion.UI.Xaml.Collections;
using Syncfusion.UI.Xaml.Grid;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;





namespace Erp.ViewModel.Suppliers
{
    public class SupplierInfoSearchViewModel : ViewModelBase
    {
        CommonFunctions CommonFunctions = new CommonFunctions();
        F7Common F7Common = new F7Common();


        #region Fields

        private ViewModelBase _currentChildView;
        private string _caption;
        private IconChar _icon;

        private MainViewModel2 _mainViewModel2;
        #endregion
        #region Properties

        public MainViewModel2 MainViewModel2
        {
            get
            {
                return _mainViewModel2;
            }
            set
            {
                _mainViewModel2 = value;
                INotifyPropertyChanged(nameof(MainViewModel2));
            }
        }
        public ViewModelBase CurrentChildView
        {
            get
            {
                return _currentChildView;
            }
            set
            {
                _currentChildView = value;
                INotifyPropertyChanged(nameof(CurrentChildView));
            }
        }

        public string Caption
        {
            get
            {
                return _caption;
            }
            set
            {
                _caption = value;
                INotifyPropertyChanged(nameof(Caption));
            }
        }
        public IconChar Icon
        {
            get
            {
                return _icon;
            }
            set
            {
                _icon = value;
                INotifyPropertyChanged(nameof(Icon));
            }
        }
        #endregion

        #region DataProperties

        private object selectedItem;
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
        private Columns sfGridColumns;
        public Columns SfGridColumns
        {
            get { return sfGridColumns; }
            set
            {
                this.sfGridColumns = value;
                INotifyPropertyChanged("SfGridColumns");
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

        private SupplierInfoSearchFilterData Filter;
        public SupplierInfoSearchFilterData filter
        {
            get { return Filter; }
            set { Filter = value; }
        }




        #endregion

        public SupplierInfoSearchViewModel()
        {
            filter = new SupplierInfoSearchFilterData();
            this.sfGridColumns = new Columns();

            //F7key = "Supplier";
            //SetSfGridColumns(F7key);

            ShowSupplierInfoGridCommandFrom = new RelayCommand2(ExecuteShowSupplierInfoGridCommandFrom);
            ShowSupplierInfoGridCommandTo = new RelayCommand2(ExecuteShowSupplierInfoGridCommandTo);

            ShowCountryGridCommandFrom = new ViewModelCommand(ExecuteShowCountryGridCommandFrom);
            ShowCountryGridCommandTo = new ViewModelCommand(ExecuteShowCountryGridCommandTo);

            rowDataCommand = new RelayCommand2(ChangeCanExecute);

            RefreshCommand = new ViewModelCommand(ExecuteRefreshCommand);
            SearchCommand = new RelayCommand2(ExecuteSearchCommand);


        }


        private ICommand rowDataCommand { get; set; }
        public ICommand RowDataCommand
        {
            get
            {
                return rowDataCommand;
            }
            set
            {
                rowDataCommand = value;
            }
        }




        public void ChangeCanExecute(object obj)
        {
            if (F7key == "SupplierFrom")
            {
                filter.SupplierCodeFrom = (SelectedItem as SupplierInfoData).SupplierCode;
                filter.SupplierCodeTo = (SelectedItem as SupplierInfoData).SupplierCode;
                filter.SupplierDescrFrom = (SelectedItem as SupplierInfoData).SupplierDescr;
                filter.SupplierDescrTo = (SelectedItem as SupplierInfoData).SupplierDescr;
            }
            if (F7key == "SupplierTo")
            {
                filter.SupplierCodeTo = (SelectedItem as SupplierInfoData).SupplierCode;
                filter.SupplierDescrTo = (SelectedItem as SupplierInfoData).SupplierDescr;
            }

            if (F7key == "CountryFrom")
            {
                filter.CountryCodeFrom = (SelectedItem as CountryData).CountryCode;
                filter.CountryCodeTo = (SelectedItem as CountryData).CountryCode;

            }
            if (F7key == "CountryTo")
            {
                filter.CountryCodeTo = (SelectedItem as CountryData).CountryCode;
            }
        }



        #region F7

        public ICommand ShowSupplierInfoGridCommandFrom { get; }
        public ICommand ShowSupplierInfoGridCommandTo { get; }

        public ICommand ShowCountryGridCommandFrom { get; }
        public ICommand ShowCountryGridCommandTo { get; }

        public ICommand ShowPerfectureGridCommand { get; }
        public ICommand ShowCityGridCommand { get; }

        #region Refresh,Search

        public ICommand SearchCommand { get; }
        public ICommand RefreshCommand { get; }
        private void ExecuteSearchCommand(object obj)
        {
            //ChildViewModelData DataForChild = new ChildViewModelData("SupplierOrderSearchResultPage",Data);


            //MainViewModel2 = new MainViewModel2(DataForChild);
            //MainView2 a = new MainView2(MainViewModel2);
            //a.Show();

            var DataList = CommonFunctions.GetSupplierInfoSearchData(Filter).ToList();
            ResultCollectionView = CollectionViewSource.GetDefaultView(DataList);

        }
        private void ExecuteRefreshCommand(object obj)
        {

            filter.SupplierCodeFrom = " ";
            filter.SupplierCodeTo = " ";

            filter.SupplierDescrFrom = " ";
            filter.SupplierDescrTo = " ";

            filter.CountryCodeFrom = " ";
            filter.CountryCodeTo = " ";
        }

        protected void ClearColumns()
        {

            var ColumnsCount = this.SfGridColumns.Count();
            if (ColumnsCount != 0)
            {
                for (int i = 0; i < ColumnsCount; i++)
                {
                    this.sfGridColumns.RemoveAt(0);
                }
            }
        }
        #endregion
        private void ExecuteShowSupplierInfoGridCommandFrom(object obj)
        {
            ClearColumns();

            var F7input = F7Common.F7SupplierFrom();
            F7key = F7input.F7key;
            CollectionView = F7input.CollectionView;
            var a = F7input.SfGridColumns;
            foreach (var item in a)
            {
                this.sfGridColumns.Add(item);
            }

        }
        private void ExecuteShowSupplierInfoGridCommandTo(object obj)
        {
            ClearColumns();

            var F7input = F7Common.F7SupplierTo();
            F7key = F7input.F7key;
            CollectionView = F7input.CollectionView;
            var a = F7input.SfGridColumns;
            foreach (var item in a)
            {
                this.sfGridColumns.Add(item);

            }
        }

        private void ExecuteShowCountryGridCommandFrom(object obj)
        {
            ClearColumns();

            var F7input = F7Common.F7Country(false);
            F7key = F7input.F7key;
            CollectionView = F7input.CollectionView;
            var a = F7input.SfGridColumns;
            foreach (var item in a)
            {
                this.sfGridColumns.Add(item);

            }
        }

        private void ExecuteShowCountryGridCommandTo(object obj)
        {
            ClearColumns();

            var F7input = F7Common.F7Country(false);
            F7key = F7input.F7key;
            CollectionView = F7input.CollectionView;
            var a = F7input.SfGridColumns;
            foreach (var item in a)
            {
                this.sfGridColumns.Add(item);

            }
        }
        #endregion




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
