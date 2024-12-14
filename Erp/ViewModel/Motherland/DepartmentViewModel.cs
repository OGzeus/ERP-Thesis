using Erp.Model.BasicFiles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using Erp.Model.Motherland.BasicFiles;
using Erp.Model.Motherland.Employee;

namespace Erp.ViewModel.Motherland
{
    public class DepartmentViewModel : ViewModelBase
    {

        private ObservableCollection<DepartmentData> data;
        public ObservableCollection<DepartmentData> Data
        {
            get { return data; }
            set
            {
                data = value;
                INotifyPropertyChanged(nameof(Data));


            }
        }

        public DepartmentViewModel()
        {
            Data = new ObservableCollection<DepartmentData>();

            OnLoad();

        }

        public void OnLoad()
        {
            Data = MLandFunctions.GetDepartmentData(ShowDeleted);
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
            Data = new ObservableCollection<DepartmentData>();

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
            bool Completed = MLandFunctions.SaveDepartmentData(Data);

            if (Completed == true)
            {
                MessageBox.Show($"Saving/Updating completed ");
            }
            else
            {
                MessageBox.Show("Error during data processing", "", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private ViewModelCommand cancelCommand;

        #endregion


        #endregion
    }

}
