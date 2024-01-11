using Erp.CommonFiles;
using Erp.Model;
using Erp.Model.BasicFiles;
using Erp.Model.Interfaces;
using Erp.View;
using Erp.View.BasicFiles;
using Erp.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Erp.ViewModel.BasicFiles
{
    public class CountryViewModel : ViewModelBase
    {

        private ObservableCollection<CountryData> data;
        public ObservableCollection<CountryData> Data
        {
            get { return data; }
            set
            {
                data = value;
                INotifyPropertyChanged(nameof(Data));


            }
        }

        public CountryViewModel()
        {
            Data = new ObservableCollection<CountryData>();

            OnLoad();

        }

        public void OnLoad()
        {
            Data = CommonFunctions.GetCountryData(ShowDeleted);
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
            Data = new ObservableCollection<CountryData>();

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
           bool Completed = CommonFunctions.SaveCountryData(Data);

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
