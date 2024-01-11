using Erp.Helper;
using Erp.Model.BasicFiles;
using Erp.Model.Customers;
using Erp.Model.Manufacture;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static Erp.Model.Manufacture.MrpResultData;

namespace Erp.ViewModel.Manufacture
{
    public class MRPResultsViewModel : ViewModelBase
    {
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

        private ObservableCollection<MrpResultData> mrpresults;
        public ObservableCollection<MrpResultData> MrpResults
        {
            get { return mrpresults; }
            set
            {
                mrpresults = value;
                INotifyPropertyChanged(nameof(MrpResults));
            }
        }

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public MRPResultsViewModel()
        {

            this.sfGridColumns = new Columns();


            MrpResults = new ObservableCollection<MrpResultData>();




            ShowMrpResultsCommand = new RelayCommand2(ExecuteShowMrpResultsCommand);

            rowDataCommand = new RelayCommand2(ChangeCanExecute);


        }

        public ICommand ShowMrpResultsCommand { get; }

        private void ExecuteShowMrpResultsCommand(object obj)
        {
            ClearColumns();

            var F7input = F7Common.F7MrpResults();
            F7key = F7input.F7key;
            CollectionView = F7input.CollectionView;
            var a = F7input.SfGridColumns;
            foreach (var item in a)
            {
                System.Diagnostics.Debug.WriteLine($"Current MappingName: {item.MappingName}");
                System.Diagnostics.Debug.WriteLine($"Current HeaderText: {item.HeaderText}");
                this.sfGridColumns.Add(item);
            }

        }

        public void ChangeCanExecute(object obj)
        {
            if (F7key == "MrpResults")
            {
                //var MrpResults = CommonFunctions.GetMrpResultsData();

            }

            //if (F7key == "PriceList")
            //{
            //    FlatData.PriceList = (SelectedItem as PriceListData);
            //}
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

            //FlatData.Code = "";
            //FlatData.Descr = "";

            //FlatData.Email = "";
            //FlatData.Phone = "";
            //FlatData.Adress1 = "";
            //FlatData.CityCode = "";
            //FlatData.CityDescr = "";
            //FlatData.PrefDescr = "";
            //FlatData.CountryDescr = "";
            //FlatData.PromptPayer = false;
            //FlatData.CustomerType = "";
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
            //var data = CommonFunctions.GetCustomerInfoData(ChooserData);
            //FlatData.Code = data.Code;
            //FlatData.Descr = data.Descr;
            //FlatData.Email = data.Email;
            //FlatData.Phone = data.Phone;
            //FlatData.Adress1 = data.Adress1;
            //FlatData.CityCode = data.CityCode;
            //FlatData.CityDescr = data.CityCode;
            //FlatData.PrefDescr = data.PrefDescr;
            //FlatData.CountryDescr = data.CountryDescr;
            //FlatData.PromptPayer = data.PromptPayer;
            //FlatData.CustomerType = data.CustomerType;
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