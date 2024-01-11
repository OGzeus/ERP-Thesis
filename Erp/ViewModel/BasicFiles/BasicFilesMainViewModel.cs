using Erp.Model;
using Erp.View;
using Erp.View.BasicFiles;
using Erp.ViewModel;
using Erp.ViewModel.Stores;
using FontAwesome.Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace Erp.ViewModel.BasicFiles
{
    public class BasicFilesMainViewModel :ViewModelBase
    {
        #region Fields

        private ViewModelBase _currentChildView;
        private string _caption;
        private IconChar _icon;

        private MainViewModel2 _mainViewModel2;
        #endregion
        #region Properties

        public MainViewModel2 MainViewModel2
        {
            get
            {
                return _mainViewModel2;
            }
            set
            {
                _mainViewModel2 = value;
                INotifyPropertyChanged(nameof(MainViewModel2));
            }
        }
        public ViewModelBase CurrentChildView
        {
            get
            {
                return _currentChildView;
            }
            set
            {
                _currentChildView = value;
                INotifyPropertyChanged(nameof(CurrentChildView));
            }
        }

        public string Caption
        {
            get
            {
                return _caption;
            }
            set
            {
                _caption = value;
                INotifyPropertyChanged(nameof(Caption));
            }
        }
        public IconChar Icon
        {
            get
            {
                return _icon;
            }
            set
            {
                _icon = value;
                INotifyPropertyChanged(nameof(Icon));
            }
        }
        #endregion
        public BasicFilesMainViewModel()
        {
            ShowCountryViewCommand = new ViewModelCommand(ExecuteShowCountryViewCommand);
            ShowPerfectureViewCommand = new ViewModelCommand(ExecuteShowPerfectureViewCommand);
            ShowCityViewCommand = new ViewModelCommand(ExecuteShowCityCommand);

        }
        #region Commands/buttons

        public ICommand ShowCountryViewCommand { get; }
        public ICommand ShowPerfectureViewCommand { get; }
        public ICommand ShowCityViewCommand { get; }


        private void ExecuteShowCountryViewCommand(object obj)
        {

            ChildViewModelData DataForChild = new ChildViewModelData("BasicCountryPage");

            MainViewModel2  = new MainViewModel2(DataForChild);
            MainView2 a = new MainView2(MainViewModel2);
            a.Show();

        }

        private void ExecuteShowPerfectureViewCommand(object obj)
        {
            ChildViewModelData DataForChild = new ChildViewModelData("BasicPrefecturePage");
            DataForChild.ChildView = new PrefectureViewModel();
            DataForChild.Caption = "Αρχείο Νομών";
            DataForChild.Icon = IconChar.GlobeEurope;

            MainViewModel2 = new MainViewModel2(DataForChild);
            MainView2 a = new MainView2(MainViewModel2);
            a.Show();
        }

        private void ExecuteShowCityCommand(object obj)
        {
            ChildViewModelData DataForChild = new ChildViewModelData("BasicCityPage");
            DataForChild.ChildView = new PrefectureViewModel();
            DataForChild.Caption = "Αρχείο Πόλεων";
            DataForChild.Icon = IconChar.MountainCity;

            MainViewModel2 = new MainViewModel2(DataForChild);
            MainView2 a = new MainView2(MainViewModel2);
            a.Show();
        }

        #endregion
    }
}
