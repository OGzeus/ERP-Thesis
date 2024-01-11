using Erp.Model;
using Erp.View;
using Erp.View.BasicFiles;
using Erp.ViewModel;
using Erp.ViewModel.BasicFiles;
using FontAwesome.Sharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace Erp.ViewModel
{
    public class MainViewModel2:ViewModelBase
    {
        #region Fields
        private ViewModelBase _currentChildView;
        private string _caption;
        private IconChar _icon;
        #endregion

        #region Properties
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


        public MainViewModel2()
        {

        }
        public MainViewModel2(ChildViewModelData data)
        {
            CurrentChildView = data.ChildView;
            Caption = data.Caption;
            Icon = data.Icon;

        }


        #region Refresh,Save
        private ViewModelCommand refreshCommand;

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
        }

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
        }


        #endregion

    }
}
