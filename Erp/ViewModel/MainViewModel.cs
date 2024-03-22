using Erp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Erp.Model;
using Erp.Repositories;
using FontAwesome.Sharp;
using Erp.CommonFiles;
using Erp.Model.Interfaces;
using Erp.ViewModel.Inventory;
using Erp.ViewModel.BasicFiles;
using Erp.ViewModel.Suppliers;
using Erp.ViewModel.Manufacture;
using Erp.ViewModel.Stores;
using Erp.ViewModel.SupplyChain;
using Erp.ViewModel.Customer;
using Erp.ViewModel.Data_Analytics;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace Erp.ViewModel
{
    public class MainViewModel:ViewModelBase ,IToolbarRepository
    {
        //Fields
        private UserAccountModel _currentUserAccount;
        private ViewModelBase _currentChildView;
        private string _caption;
        private IconChar _icon;
        private IUserRepository userRepository;
        //Properties

        public ICommand AddTabCommand { get; }
        public UserAccountModel CurrentUserAccount
        {
            get
            {
                return _currentUserAccount;
            }
            set
            {
                _currentUserAccount = value;
                INotifyPropertyChanged(nameof(CurrentUserAccount));
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


        public MainViewModel()
        {
            userRepository = new UserRepository();
            CurrentUserAccount = new UserAccountModel();

            LoadCurrentUserData();
        }

        private void LoadCurrentUserData()
        {
            var user = userRepository.GetByUserName(Thread.CurrentPrincipal.Identity.Name);
            if (user != null)
            {

                CurrentUserAccount.Username = user.UserName;
                CurrentUserAccount.DisplayName = $"{user.Name} {user.LastName}";
                CurrentUserAccount.ProfilePicture = null;

            }
            else
            {
                CurrentUserAccount.DisplayName = "Invalid user, not logged in";
                //Hide Child Views

            }
        }

        #region Toolbar
        public ICommand ShowChooserViewModelCommand()
        {
            throw new NotImplementedException();
        }

        public ICommand ShowSearchViewModelCommand()
        {
            throw new NotImplementedException();
        }

        public ICommand RefreshCommand()
        {
            throw new NotImplementedException();
        }

        public ICommand SaveCommand()
        {
            throw new NotImplementedException();
        }


        #endregion
        //public void AddNewTab(string header, UserControl content)
        //{
        //    var newTab = new TabViewModel(header, content);
        //    Tabs.Add(newTab);
        //    CurrentChildView = content;
        //}

        //public ICommand AddTabCommand => new RelayCommand(() =>
        //{
        //    var tab = new TabViewModel();
        //    Tabs.Add(tab);
        //});
    }
}
