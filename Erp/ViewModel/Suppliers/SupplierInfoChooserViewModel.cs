using Erp.CommonFiles;
using Erp.Helper;
using Erp.Model.BasicFiles;
using Erp.Model.Suppliers;
using Erp.ViewModel;
using FontAwesome.Sharp;
using MyControls;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Utility;
using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;

namespace Erp.ViewModel.Suppliers
{
    public class SupplierInfoChooserViewModel : ViewModelBase
    {


        #region DataProperties


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

        private SupplierInfoData flatData;
        public SupplierInfoData FlatData
        {
            get { return flatData; }
            set { flatData = value; }
        }

        private SupplierInfoData chooserData;
        public SupplierInfoData ChooserData
        {
            get { return chooserData; }
            set { chooserData = value; }
        }

        private ObservableCollection<SupItemData> flatData2;
        public ObservableCollection<SupItemData> FlatData2
        {
            get { return flatData2; }
            set
            {
                flatData2 = value;
                INotifyPropertyChanged(nameof(FlatData2));
            }
        }

        private SupItemData chooserData2;
        public SupItemData ChooserData2
        {
            get { return chooserData2; }
            set { chooserData2 = value; }
        }
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public SupplierInfoChooserViewModel()
        {
            FlatData = new SupplierInfoData();
            ChooserData =  new SupplierInfoData();
            this.sfGridColumns = new Columns();

            ChooserData2 = new SupItemData();
            FlatData2 = new ObservableCollection<SupItemData>();


            

            ShowCityGridCommand = new RelayCommand2(ExecuteShowCityGridCommand);
            ShowSupplierInfoGridCommand = new RelayCommand2(ExecuteShowSupplierGridCommand);

            rowDataCommand = new RelayCommand2(ChangeCanExecute);


        }


        #region F7

        public ICommand ShowSupplierInfoGridCommand { get; }
        public ICommand ShowCityGridCommand { get; }

        private void ExecuteShowSupplierGridCommand(object obj)
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


        private void ExecuteShowCityGridCommand(object obj)
        {
            ClearColumns();

            var F7input = F7Common.F7City(false);
            F7key = F7input.F7key;
            CollectionView = F7input.CollectionView;
            var a = F7input.SfGridColumns;
            foreach (var item in a)
            {
                this.sfGridColumns.Add(item);
            }

        }

        public void ChangeCanExecute(object obj)
        {
            if (F7key == "Supplier")
            {
                FlatData.SupplierCode = (SelectedItem as SupplierInfoData).SupplierCode;
                FlatData.SupplierDescr = (SelectedItem as SupplierInfoData).SupplierDescr;

                ChooserData.SupplierCode = FlatData.SupplierCode;
                ChooserData.SupplierDescr = FlatData.SupplierDescr;

                #region Γεμίζω το Γκριντ του 2ου tab 
                FlatData2 = CommonFunctions.GetItemPerSupData(ChooserData.SupplierCode);

                #endregion
                var data = CommonFunctions.GetSupplierInfoData(FlatData);


                FlatData.Email = data.Email;
                FlatData.Phone = data.Phone;
                FlatData.Adress1 = data.Adress1;
                FlatData.CityCode = data.CityCode;
                FlatData.CityDescr = data.CityCode;
                FlatData.PrefDescr = data.PrefDescr;
                FlatData.CountryDescr = data.CountryDescr;
                FlatData.Fason = data.Fason;
                FlatData.SupplierType = data.SupplierType;

            }
            if (F7key == "City")
            {
                FlatData.CityCode = (SelectedItem as CityData).CityCode;
                FlatData.CityDescr = (SelectedItem as CityData).CityDescr;
                FlatData.CityDescr = (SelectedItem as CityData).CityDescr;
                FlatData.CountryDescr = (SelectedItem as CityData).CountryDescr;
                FlatData.PrefDescr = (SelectedItem as CityData).PrefDescr;

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


        #region Refresh,Save,Clear

        #region 1st Tab
        #region Clear

        private ViewModelCommand clearCommand;

        public ICommand ClearCommand
        {
            get
            {
                if (clearCommand == null)
                {
                    clearCommand = new ViewModelCommand(ExecuteClearCommand);
                }

                return clearCommand;
            }
        }

        private void ExecuteClearCommand(object commandParameter)
        {

            FlatData.SupplierCode = "";
            FlatData.SupplierDescr = "";

            FlatData.Email = "";
            FlatData.Phone = "";
            FlatData.Adress1 = "";
            FlatData.CityCode = "";
            FlatData.CityDescr = "";
            FlatData.PrefDescr = "";
            FlatData.CountryDescr = "";
            FlatData.Fason = false;
            FlatData.SupplierType = "";
        }

        #endregion
        #region Save


        private ViewModelCommand savecommand;

        public ICommand SaveCommand
        {
            get
            {
                if (savecommand == null)
                {
                    savecommand = new ViewModelCommand(ExecuteSaveCommand);
                }

                return savecommand;
            }
        }

        private void ExecuteSaveCommand(object obj)
        {
            int Flag = CommonFunctions.SaveSupplierInfoData(FlatData);

            if (Flag == 0)
            {
                MessageBox.Show($"Ο Αποθηκεύτηκε νέος παραγωγός με Κωδικό : {FlatData.SupplierCode}");
            }
            else if (Flag == 1)
            {
                MessageBox.Show($"Η Αποθήκευση/Ανανέωση Ολοκληρώθηκε για τον Παραγωγό με Κωδικό : {FlatData.SupplierCode}");

            }
            else
            {
                MessageBox.Show("Σφάλμα κατά την επεξεργασία δεδομένων", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
        #region Refresh

        private ViewModelCommand refreshCommand;

        public ICommand RefreshCommand
        {
            get
            {
                if (refreshCommand == null)
                {
                    refreshCommand = new ViewModelCommand(ExecuteRefreshCommand);
                }

                return refreshCommand;
            }
        }

        private void ExecuteRefreshCommand(object commandParameter)
        {
            
            var data = CommonFunctions.GetSupplierInfoData(ChooserData);

            FlatData.SupplierCode = data.SupplierCode;
            FlatData.SupplierDescr = data.SupplierDescr;

            FlatData.Email = data.Email;
            FlatData.Phone = data.Phone;
            FlatData.Adress1 = data.Adress1;
            FlatData.CityCode = data.CityCode;
            FlatData.CityDescr = data.CityCode;
            FlatData.PrefDescr = data.PrefDescr;
            FlatData.CountryDescr = data.CountryDescr;
            FlatData.Fason = data.Fason;
            FlatData.SupplierType = data.SupplierType;
        }

        #endregion

        #endregion

        #region 2nd Tab
        #region Save
        private ViewModelCommand saveCommand2;
        public ICommand SaveCommand2
        {
            get
            {
                if (saveCommand2 == null)
                {
                    saveCommand2 = new ViewModelCommand(ExecuteSaveCommand2);
                }

                return saveCommand2;
            }
        }
        private void ExecuteSaveCommand2(object obj)
        {
            bool Flag = CommonFunctions.SaveItemPerSupData(FlatData2,chooserData.SupplierCode);


            if (Flag == true)
            {
                MessageBox.Show($"Η Αποθήκευση/Ανανέωση Ολοκληρώθηκε για τον Παραγωγό με Κωδικό : {FlatData.SupplierCode}");

            }
            else
            {
                MessageBox.Show("Σφάλμα κατά την επεξεργασία δεδομένων", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
        #region Refresh

        private ViewModelCommand refreshCommand2;

        public ICommand RefreshCommand2
        {
            get
            {
                if (refreshCommand2 == null)
                {
                    refreshCommand2 = new ViewModelCommand(ExecuteRefreshCommand2);
                }

                return refreshCommand2;
            }
        }

        private void ExecuteRefreshCommand2(object commandParameter)
        {

            FlatData2 = CommonFunctions.GetItemPerSupData(ChooserData.SupplierCode);

        }

        #endregion
        #endregion

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

    

