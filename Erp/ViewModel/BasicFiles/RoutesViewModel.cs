using Erp.CommonFiles;
using Erp.Helper;
using Erp.Model.BasicFiles;
using Syncfusion.Data.Extensions;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.Windows.Shared;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Windows;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Erp.ViewModel.BasicFiles
{
    public class RoutesViewModel : ViewModelBase
    {
        #region Properties

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

        private ObservableCollection<RoutesData> data;
        public ObservableCollection<RoutesData> Data
        {
            get { return data; }
            set
            {
                data = value;
                INotifyPropertyChanged(nameof(Data));


            }
        }
        private List<CityData> citylist;
        public List<CityData> CityList
        {
            get { return citylist; }
            set
            {
                citylist = value;
                INotifyPropertyChanged(nameof(CityList));


            }
        }


        #endregion
        public RoutesViewModel()
        {
            Data = new ObservableCollection<RoutesData>();
            CityList = new List<CityData>();
            this.sfGridColumns = new Columns();

            OnLoad();
            ExecuteShowCityDataGridCommand();

        }

        #region F7Cities

        public void ExecuteShowCityDataGridCommand()
        {


            ClearColumns();

            var F7input = F7Common.F7City(false);
            F7key = F7input.F7key;
            CollectionView = F7input.CollectionView;
            var a = F7input.SfGridColumns;
            foreach (var item in a.Take(a.Count - 1))
            {
                this.sfGridColumns.Add(item);
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
        public void OnLoad()
        {
            Data = CommonFunctions.GetRoutesData(ShowDeleted);
            CityList = CommonFunctions.GetCityData(false).ToList();

        }

        #region Toolbar
        private ViewModelCommand refreshCommand;

        #region Refresh
        public ICommand RefreshCommand
        {
            get
            {
                if (refreshCommand == null)
                {
                    refreshCommand = new ViewModelCommand(Refresh);
                }

                return refreshCommand;
            }
        }

        private void Refresh(object commandParameter)
        {
            Data = new ObservableCollection<RoutesData>();

            OnLoad();
        }

        #endregion

        #region SaveCommand

        private ViewModelCommand saveCommand;

        public ICommand SaveCommand
        {
            get
            {
                if (saveCommand == null)
                {
                    saveCommand = new ViewModelCommand(Save);
                }

                return saveCommand;
            }
        }

        private void Save(object commandParameter)
        {
            bool Completed = CommonFunctions.SaveRoutesData(Data);

            if (Completed == true)
            {
                MessageBox.Show($"Saving/Updating completed ");
            }
            else
            {
                MessageBox.Show("Error during data processing", "", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        #endregion

        #endregion

    }
}
