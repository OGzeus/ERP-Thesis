using Erp.CommonFiles;
using Erp.Helper;
using Erp.Model.BasicFiles;
using Erp.Model.Suppliers;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace Erp.ViewModel
{
    public class FlatSearchViewModel : ViewModelBase
    {
        CommonFunctions CommonFunctions = new CommonFunctions();



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


        #endregion

        public FlatSearchViewModel(SupplierInfoSearchFilterData Filter)
        {
            this.sfGridColumns = new Columns();
            SetSfGridColumns("SupplierInfo");
            var DataList = CommonFunctions.GetSupplierInfoSearchData(Filter).ToList();
            CollectionView = CollectionViewSource.GetDefaultView(DataList);
            rowDataCommand = new RelayCommand2(ChangeCanExecute);
        }

        protected void SetSfGridColumns(string Flag)
        {

            var ColumnsCount = this.SfGridColumns.Count();
            if (ColumnsCount != 0)
            {
                for (int i = 0; i < ColumnsCount; i++)
                {
                    this.sfGridColumns.RemoveAt(0);
                }
            }
            if (Flag.Contains("SupplierInfo"))
            {
                this.sfGridColumns.Add(new GridTextColumn() { MappingName = "SupplierCode" });
                this.sfGridColumns.Add(new GridTextColumn() { MappingName = "SupplierDescr" });
                this.sfGridColumns.Add(new GridTextColumn() { MappingName = "Email" });
                this.sfGridColumns.Add(new GridTextColumn() { MappingName = "Phone" });
                this.sfGridColumns.Add(new GridTextColumn() { MappingName = "Adress1" });
                this.sfGridColumns.Add(new GridTextColumn() { MappingName = "SupplierType" });

                this.sfGridColumns.Add(new GridTextColumn() { MappingName = "CountryCode" });
                this.sfGridColumns.Add(new GridTextColumn() { MappingName = "CountryDescr" });

                this.sfGridColumns.Add(new GridTextColumn() { MappingName = "PrefCode" });
                this.sfGridColumns.Add(new GridTextColumn() { MappingName = "PrefDescr" });


                this.sfGridColumns.Add(new GridTextColumn() { MappingName = "CityCode" });
                this.sfGridColumns.Add(new GridTextColumn() { MappingName = "CityDescr" });
            }


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

            //Edw tha kanw tin sindesh me thn flat othonh sto selectedrow
                //Data.S = (SelectedItem as SupplierInfoData).SupplierCode;

        }

        private bool _isopen = false;
        public bool isOpen
        {
            get { return _isopen; }
            set { _isopen = value; RaisePropertyChanged("isOpen"); }
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
