using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using Beerka.Desktop.Model;

namespace Beerka.Desktop.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly BeerkaAPIService _model;
        private Boolean _isLoading;

        public DelegateCommand LoginCommand { get; private set; }
        public DelegateCommand ExitCommand { get; private set; }

        public String UserName { get; set; }

        public Boolean IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public event EventHandler<LoginSucceededEventArgs> LoginSucceeded;

        public event EventHandler LoginFailed;

        public event EventHandler ExitRequested;

        public LoginViewModel(BeerkaAPIService model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            _model = model;
            UserName = String.Empty;
            IsLoading = false;

            LoginCommand = new DelegateCommand(_ => !IsLoading, param => LoginAsync(param as PasswordBox));
            ExitCommand = new DelegateCommand(_ => !IsLoading, param => RequestExit());
        }

        private async void LoginAsync(PasswordBox passwordBox)
        {
            if (passwordBox == null || passwordBox.Password=="" || UserName==null || UserName=="")
                return;

            try
            {
                IsLoading = true;
                bool result = await _model.LoginAsync(UserName, passwordBox.Password);
                IsLoading = false;

                if (result)
                    OnLoginSuccess();
                else
                    OnLoginFailed();
            }
            catch (NetworkException ex)
            {
                OnMessageApplication($"Unexpected error occured! ({ex.Message})");
            }
        }

        private void RequestExit()
        {
            OnExitRequested();
        }

        private void OnLoginSuccess()
        {
            LoginSucceeded?.Invoke(this, new LoginSucceededEventArgs(UserName));
        }

        private void OnLoginFailed()
        {
            LoginFailed?.Invoke(this, EventArgs.Empty);
        }

        private void OnExitRequested()
        {
            ExitRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
