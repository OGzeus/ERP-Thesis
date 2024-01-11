using Erp.Model;
using Erp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Erp.ViewModel
{
    public class LoginViewModel:ViewModelBase
    {
        //Fields
        private string _username ;
        private SecureString _password;
        private string _errormessage;
        private bool _isViewVisible = true;

        private IUserRepository userRepository;


        //Properties
        public string UserName
        {
            get
            { 
                return _username; 
            }
            set 
            { 
            _username = value;
             INotifyPropertyChanged(nameof(UserName));
            
            }
        }
        public SecureString Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                INotifyPropertyChanged(nameof(Password));

            }
        }
        public string ErrorMessage
        {
            get
            {
                return _errormessage;
            }
            set
            {
                _errormessage = value;
                INotifyPropertyChanged(nameof(ErrorMessage));

            }
        }
        public bool IsViewVisible
        {
            get
            {
                return _isViewVisible;
            }
            set
            {
                _isViewVisible = value;
                INotifyPropertyChanged(nameof(IsViewVisible));

            }
        }

        // Commands

        public ICommand LoginCommand { get;}
        public ICommand RecoverPasswordCommand { get; }
        public ICommand ShowPasswordCommand { get; }
        public ICommand RememberPasswordCommand { get; }

        public LoginViewModel()
        {
            userRepository = new UserRepository();
            LoginCommand = new ViewModelCommand(ExecuteLoginCommand, CanExecuteLoginCommand);
            RecoverPasswordCommand = new ViewModelCommand(p => ExecuteRecoverPassCommand("",""));

        }



        private bool CanExecuteLoginCommand(object obj)
        {
            bool validData;
            if (string.IsNullOrWhiteSpace(UserName) || UserName.Length < 3 || Password == null || Password.Length < 3)
                validData = false;
            else 
                validData = true;
            return validData;
        }

        private void ExecuteLoginCommand(object obj)
        {
            var isValidUser = userRepository.AuthenticateUser(new NetworkCredential(UserName, Password));
            if(isValidUser)
            {
                Thread.CurrentPrincipal = new GenericPrincipal(
                    new GenericIdentity(UserName),null);
                IsViewVisible = false;
            }
            else
            {
                ErrorMessage = "*Invalid username or password";
            }
        }

        private void ExecuteRecoverPassCommand(string username ,string email)
        {
            throw new NotImplementedException();
        }
    }
}
