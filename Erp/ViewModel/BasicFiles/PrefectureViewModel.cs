using Erp.CommonFiles;
using Erp.Model.BasicFiles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.ComponentModel;

namespace Erp.ViewModel.BasicFiles
{
    public class PrefectureViewModel :ViewModelBase
    {
        #region Properties
        CommonFunctions CommonFunctions = new CommonFunctions();


        private ObservableCollection<PrefectureData> data;
        public ObservableCollection<PrefectureData>  Data
        {
            get { return data; }
            set { data = value; OnPropertyChanged("Data"); }
        }



        private List<CountryData> _CountryList;

        public List<CountryData> CountryList
        {
            get { return _CountryList; }
            set { _CountryList = value; OnPropertyChanged("CountryList"); }
        }

        #endregion
        public PrefectureViewModel()
        {
            Data = new ObservableCollection<PrefectureData>();

            CountryList = new List<CountryData>();


            OnLoad();

        }

        public void OnLoad()
        {
            CountryList = new List<CountryData>();
            Data = new ObservableCollection<PrefectureData>();
            CountryList = CommonFunctions.GetCountryData(false).ToList();

            Data = CommonFunctions.GetPrefectureData();


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
            CountryList = new List<CountryData>();
            Data = new ObservableCollection<PrefectureData>();

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
            bool Completed = CommonFunctions.SavePrefectureData(Data);

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

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}

