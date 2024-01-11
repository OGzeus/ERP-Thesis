using Erp.CommonFiles;
using Erp.Helper;
using Erp.Model.BasicFiles;
using Erp.Model.Suppliers;
using MyControls;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using LiveCharts.Wpf;
using System.Runtime.InteropServices.ComTypes;
using Erp.Model.Manufacture;
using Erp.Model.Enums;
using System.Windows.Forms;

namespace Erp.ViewModel.Manufacture
{
    public class MachineViewModel : ViewModelBase
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

        private MachineData flatData;
        public MachineData FlatData
        {
            get { return flatData; }
            set { flatData = value; }
        }

        private MachineData chooserData;
        public MachineData ChooserData
        {
            get { return chooserData; }
            set { chooserData = value; }
        }




        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
        
       #region Enums

        public BasicEnums.MachStatus[] MachStatuses
        {
            get { return (BasicEnums.MachStatus[])Enum.GetValues(typeof(BasicEnums.MachStatus)); }
        }

        public BasicEnums.MachType[] MachTypes
        {
            get { return (BasicEnums.MachType[])Enum.GetValues(typeof(BasicEnums.MachType)); }
        }

      #endregion
        public MachineViewModel()
        {
        FlatData = new MachineData();
        FlatData.MachCode = " ";
        FlatData.Factory = new FactoryData();
        ChooserData = new MachineData();
        ChooserData.Factory = new FactoryData();
        this.sfGridColumns = new Columns();
        FlatData.LastMaintenance = DateTime.Now;
        FlatData.NextMaintenance = DateTime.Now.AddMonths(1);



        ShowMachDataGridCommand = new RelayCommand2(ExecuteShowMachGridCommand);
        ShowFactoryDataGridCommand = new RelayCommand2(ExecuteShowFactoryDataGridCommand);

        rowDataCommand = new RelayCommand2(ChangeCanExecute);
        }

        #region F7

        public ICommand ShowMachDataGridCommand { get; }

        public void ExecuteShowMachGridCommand(object obj)
        {


            ClearColumns();

            var F7input = F7Common.F7Machine(ShowDeleted);
            F7key = F7input.F7key;
            CollectionView = F7input.CollectionView;
            var a = F7input.SfGridColumns;
            foreach (var item in a)
            {
                this.sfGridColumns.Add(item);
            }

        }

        public ICommand ShowFactoryDataGridCommand { get; }

        private void ExecuteShowFactoryDataGridCommand(object obj)
        {


            ClearColumns();

            var F7input = F7Common.F7Factory();
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
            if (F7key == "Mach")
            {
                FlatData.MachID = (SelectedItem as MachineData).MachID;
                FlatData.MachCode = (SelectedItem as MachineData).MachCode;
                FlatData.MachDescr = (SelectedItem as MachineData).MachDescr;
                FlatData.IsDeleted = (SelectedItem as MachineData).IsDeleted;

                ChooserData.MachCode = (SelectedItem as MachineData).MachCode;
                ChooserData.MachDescr = (SelectedItem as MachineData).MachDescr;

                FlatData.LastMaintenance = (SelectedItem as MachineData).LastMaintenance;
                FlatData.NextMaintenance = (SelectedItem as MachineData).NextMaintenance;
                FlatData.TotalOperatingHours = (SelectedItem as MachineData).TotalOperatingHours;
                FlatData.FailureRate = (SelectedItem as MachineData).FailureRate;
                FlatData.ProductionRate = (SelectedItem as MachineData).ProductionRate;
                FlatData.EfficiencyRate = (SelectedItem as MachineData).EfficiencyRate;
                FlatData.AverageRepairTime = (SelectedItem as MachineData).AverageRepairTime;

                FlatData.NumberOfFailures = (SelectedItem as MachineData).NumberOfFailures;
                FlatData.MachineType = (SelectedItem as MachineData).MachineType;
                FlatData.ModelYear = (SelectedItem as MachineData).ModelYear;
                FlatData.DateInstalled = (SelectedItem as MachineData).DateInstalled;
                FlatData.Status = (SelectedItem as MachineData).Status;
                FlatData.PrimaryModel = (SelectedItem as MachineData).PrimaryModel;
                FlatData.NumberOfMachines = (SelectedItem as MachineData).NumberOfMachines;

                FlatData.Factory.FactoryID = (SelectedItem as MachineData).Factory.FactoryID;
                FlatData.Factory.Code = (SelectedItem as MachineData).Factory.Code;
                FlatData.Factory.Descr = (SelectedItem as MachineData).Factory.Descr;

                FlatData.LastMaintenance = (SelectedItem as MachineData).LastMaintenance;
                FlatData.NextMaintenance = (SelectedItem as MachineData).NextMaintenance;
            }
            if (F7key == "Factory")
            {
                FlatData.Factory.FactoryID = (SelectedItem as FactoryData).FactoryID;
                FlatData.Factory.Code = (SelectedItem as FactoryData).Code;
                FlatData.Factory.Descr = (SelectedItem as FactoryData).Descr;

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

        #region ButtonCommands

        #region AddMachine Command
        #region Save


        private ViewModelCommand addmachinecommand;

        public ICommand AddMachineCommand
        {
            get
            {
                if (addmachinecommand == null)
                {
                    addmachinecommand = new ViewModelCommand(ExecuteAddMachineCommand);
                }

                return addmachinecommand;
            }
        }

        private void ExecuteAddMachineCommand(object obj)
        {
            if (FlatData.PrimaryModel == true)
            {
                var MachCodesList = new List<string>();
                for (var i = 1; i <= FlatData.NumberOfMachines; i++)
                {
                    MachCodesList.Add($"{FlatData.MachCode}{i}");
                }
                var allCodes = string.Join(", ", MachCodesList); // joining all codes with ", "
                var result = System.Windows.MessageBox.Show($"{FlatData.NumberOfMachines} will be created with the ItemCodes: {allCodes}. Proceed?", "Confirmation", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    // proceed with operation
                }
                else
                {
                    // cancel operation
                }
            }

            int Flag = CommonFunctions.SaveMachineData(FlatData);

            if (Flag == 0)
            {
                System.Windows.MessageBox.Show($"Ο Αποθηκεύτηκε νέα Μηχανή με Κωδικό : {FlatData.MachCode}");
                ExecuteShowMachGridCommand(obj);
            }
            else if (Flag == 1)
            {
                System.Windows.MessageBox.Show($"Η Αποθήκευση/Ανανέωση Ολοκληρώθηκε για την Μηχανή με Κωδικό : {FlatData.MachCode}");
                ExecuteShowMachGridCommand(obj);

            }
            else
            {
                System.Windows.MessageBox.Show("Σφάλμα κατά την επεξεργασία δεδομένων", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
        #endregion
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
            ChooserData.MachCode = "";
            ChooserData.MachDescr = "";
            ChooserData.MachID = -1;

            ChooserData = new MachineData();
            FlatData.Factory = new FactoryData();
            FlatData.MachCode = "";
            FlatData.MachDescr = "";
            FlatData.MachID = 0;

            //FlatData.MesUnit = "";
            //FlatData.ItemType = "";
            //FlatData.Assembly = 99;
            //FlatData.CanBeProduced = false;
            //FlatData.InputOrderFlag = false;
            //FlatData.OutputOrderFlag = false;

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


            int Flag = CommonFunctions.SaveMachineData(FlatData);

            if (Flag == 0)
            {
                System.Windows.MessageBox.Show($"Ο Αποθηκεύτηκε νέα Μηχανή με Κωδικό : {FlatData.MachCode} και Αριθμός Μηχανών :{FlatData.NumberOfMachines}  ");
            }
            else if (Flag == 1)
            {
                System.Windows.MessageBox.Show($"Η Αποθήκευση/Ανανέωση Ολοκληρώθηκε για την Μηχανή με Κωδικό : {FlatData.MachCode}");

            }
            else if (Flag == 2)
            {
                System.Windows.MessageBox.Show("Check the PrimaryModel if you want to add a new Machine ", "Confirmation", MessageBoxButton.OK);

            }
            else if (Flag == -1)
            {
                System.Windows.MessageBox.Show("Σφάλμα κατά την επεξεργασία δεδομένων", "", MessageBoxButton.OK, MessageBoxImage.Error);

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

            FlatData = CommonFunctions.GetMachineData(ChooserData);


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
