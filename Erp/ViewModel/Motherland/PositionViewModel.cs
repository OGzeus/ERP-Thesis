using Erp.Model.BasicFiles;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using Erp.Model.Motherland.BasicFiles;

namespace Erp.ViewModel.Motherland
{
    public class PositionViewModel : ViewModelBase
    {
        #region Properties

        private List<DepartmentData> departList;
        public List<DepartmentData> DepartList
        {
            get { return departList; }
            set { departList = value; }
        }

        private ObservableCollection<PositionData> data;
        public ObservableCollection<PositionData> Data
        {
            get { return data; }
            set
            {
                data = value;
                INotifyPropertyChanged(nameof(Data));
            }
        }
        #endregion
        public PositionViewModel()
        {
            Data = new ObservableCollection<PositionData>();
            DepartList = new List<DepartmentData>();

            OnLoad();

        }


        public void OnLoad()
        {
            Data = MLandFunctions.GetPositionData(ShowDeleted);
            DepartList = MLandFunctions.GetDepartmentData(false).ToList();


        }


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
            Data = new ObservableCollection<PositionData>();

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
            bool Completed = MLandFunctions.SavePositionData(Data);

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

    }

}
