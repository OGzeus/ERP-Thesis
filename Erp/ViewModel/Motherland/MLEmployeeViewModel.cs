using Erp.Helper;
using Erp.Model.BasicFiles;
using Erp.Model.Thesis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using Syncfusion.UI.Xaml.Grid;

namespace Erp.ViewModel.Motherland
{
    public class MLEmployeeViewModel : ViewModelBase
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


        private AirportData flatData;
        public AirportData FlatData
        {
            get { return flatData; }
            set
            {
                flatData = value;
                INotifyPropertyChanged(nameof(FlatData));
            }
        }

        #endregion

        #region Commands

        #region CRUD Commands

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
            ResetAirportsViewmodelData();

        }
        public void ResetAirportsViewmodelData()
        {
            FlatData = new AirportData();
            FlatData.City = new CityData();
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
            int Flag = CommonFunctions.SaveAirportsData(FlatData);

            if (Flag == 1)
            {
                MessageBox.Show($"Saving/Updating Completed for the Airport with Code : {FlatData.Code}");
                ExecuteShowMLEmployeesGridCommand(obj);

            }
            else if (Flag == -1)
            {
                MessageBox.Show("Error during data processing", "", MessageBoxButton.OK, MessageBoxImage.Error);
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
            FlatData = CommonFunctions.GetAirportsChooserData(FlatData.Id, FlatData.Code);
        }

        #endregion
        #region Add

        public ICommand AddMLEmployeesDataCommand { get; }

        private void ExecuteAddMLEmployeesDataCommand(object obj)
        {
            if (string.IsNullOrWhiteSpace(FlatData.Code) || string.IsNullOrWhiteSpace(FlatData.Descr))
            {
                MessageBox.Show("Insert Code and Description");
            }

            else
            {
                int Flag = CommonFunctions.AddAirportsData(FlatData);
                if (Flag == 0)

                {
                    MessageBox.Show($"A new Airport was saved with Code: {FlatData.Code}");

                    ExecuteShowMLEmployeesGridCommand(obj);
                    FlatData.Id = 0;
                    ExecuteRefreshCommand(obj);

                }
                else if (Flag == 1)
                {
                    MessageBox.Show($"The Airport with Code : {FlatData.Code} already exists");

                }
                else if (Flag == 2)
                {
                    MessageBox.Show("Error during data processing", "", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }
        #endregion

        #endregion

        #endregion

        #region Data_Grid Commands

        public ICommand ShowMLEmployeesGridCommand { get; }
        public ICommand ShowPositionsGridCommand { get; }
        public ICommand ShowDepartmentsGridCommand { get; }
        public ICommand ShowCitiesGridCommand { get; }




        private void ExecuteShowMLEmployeesGridCommand(object obj)
        {
            ClearColumns();

            var F7input = F7Common.F7MLEmployee(ShowDeleted);
            F7key = F7input.F7key;
            CollectionView = F7input.CollectionView;
            var a = F7input.SfGridColumns;
            foreach (var item in a)
            {
                this.sfGridColumns.Add(item);
            }

        }

        private void ExecuteShowDepartmentsGridCommand(object obj)
        {


            ClearColumns();

            var F7input = F7Common.F7Department(false);
            F7key = F7input.F7key;
            CollectionView = F7input.CollectionView;
            var a = F7input.SfGridColumns;
            foreach (var item in a)
            {
                this.sfGridColumns.Add(item);
            }

        }

        private void ExecuteShowPositionsGridCommand(object obj)
        {


            ClearColumns();

            var F7input = F7Common.F7Position(false);
            F7key = F7input.F7key;
            CollectionView = F7input.CollectionView;
            var a = F7input.SfGridColumns;
            foreach (var item in a)
            {
                this.sfGridColumns.Add(item);
            }

        }
        private void ExecuteShowCitiesGridCommand(object obj)
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

            if (F7key == "Airport")
            {
                FlatData = new AirportData();
                FlatData.City = new CityData();
                FlatData = (SelectedItem as AirportData);


            }
            if (F7key == "City")
            {
                FlatData.City = new CityData();
                FlatData.City = (SelectedItem as CityData);

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

        #endregion

        public MLEmployeeViewModel()
        {
            #region Data Initialization

            ResetAirportsViewmodelData();
            this.sfGridColumns = new Columns();

            #endregion

            #region Commands Initialization

            ShowMLEmployeesGridCommand = new RelayCommand2(ExecuteShowMLEmployeesGridCommand);
            ShowPositionsGridCommand = new RelayCommand2(ExecuteShowPositionsGridCommand);
            ShowDepartmentsGridCommand = new RelayCommand2(ExecuteShowDepartmentsGridCommand);
            ShowCitiesGridCommand = new RelayCommand2(ExecuteShowCitiesGridCommand);

            AddMLEmployeesDataCommand = new RelayCommand2(ExecuteAddMLEmployeesDataCommand);
            rowDataCommand = new RelayCommand2(ChangeCanExecute);

            #endregion
        }

    }

}
