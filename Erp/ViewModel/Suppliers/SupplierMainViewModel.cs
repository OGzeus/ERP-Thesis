using Erp.Model;
using Erp.View;
using Erp.View.Suppliers;
using Erp.ViewModel.BasicFiles;
using FontAwesome.Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Erp.ViewModel.Suppliers
{
    public class SupplierMainViewModel : ViewModelBase
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

        public SupplierMainViewModel()
        {
            ShowSupplierInfoViewCommand = new ViewModelCommand(ExecuteShowSupplierInfoViewCommand);
            ShowSupplierInfoSearchViewCommand = new ViewModelCommand(ExecuteShowSupplierInfoSearchViewCommand);
            ShowSupplierOrderViewCommand = new ViewModelCommand(ExecuteShowSupplierOrderViewCommand);
            ShowSupplierOrderSearchViewCommand = new ViewModelCommand(ExecuteShowSupplierOrderSearchViewCommand);
        }

        #region Commands/buttons

        public ICommand ShowSupplierInfoViewCommand { get; }
        public ICommand ShowSupplierInfoSearchViewCommand { get; }
        public ICommand ShowSupplierOrderViewCommand { get; }
        public ICommand ShowSupplierOrderSearchViewCommand { get; }



        private void ExecuteShowSupplierInfoViewCommand(object obj)
        {
            SupplierChooserView view = new SupplierChooserView();

            view.Show();
        }

        private void ExecuteShowSupplierInfoSearchViewCommand(object obj)
        {
            //ChildViewModelData DataForChild = new ChildViewModelData("SupplierInfoSearchPage");

            //MainViewModel2 = new MainViewModel2(DataForChild);
            //MainView2 a = new MainView2(MainViewModel2);
            //a.Show();

            SupplyInfoSearchView view = new SupplyInfoSearchView();
            view.Show();
        }

        private void ExecuteShowSupplierOrderViewCommand(object obj)
        {
            ChildViewModelData DataForChild = new ChildViewModelData("SupplierOrderPage");

            MainViewModel2 = new MainViewModel2(DataForChild);
            MainView2 a = new MainView2(MainViewModel2);
            a.Show();
        }

        private void ExecuteShowSupplierOrderSearchViewCommand(object obj)
        {
            ChildViewModelData DataForChild = new ChildViewModelData("SupplierOrderSearchPage");


            MainViewModel2 = new MainViewModel2(DataForChild);
            MainView2 a = new MainView2(MainViewModel2);
            a.Show();
        }




        #endregion
    }
}
