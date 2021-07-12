using Beerka.Desktop.Model;
using Beerka.Desktop.View;
using Beerka.Desktop.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Beerka.Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        BeerkaAPIService _service;

        private LoginViewModel _loginViewModel;
        private MainViewModel _mainViewModel;

        private LoginWindow _loginView;
        private MainWindow _mainView;
        private ProductEditorWindow _productEditorView;

        public App()
        {
            Startup += App_Startup;
        }



        private void App_Startup(object sender, StartupEventArgs e)
        {
            _service = new BeerkaAPIService(ConfigurationManager.AppSettings["baseAddress"]);

            _loginViewModel = new LoginViewModel(_service);

            _loginViewModel.LoginSucceeded += ViewModel_LoginSucceeded;
            _loginViewModel.LoginFailed += ViewModel_LoginFailed;
            _loginViewModel.MessageApplication += ViewModel_MessageApplication;
            _loginViewModel.ExitRequested += ViewModel_ExitRequested;

            _loginView = new LoginWindow
            {
                DataContext = _loginViewModel
            };

            _mainViewModel = new MainViewModel(_service);

            _mainViewModel.LogoutSucceeded += ViewModel_LogoutSucceeded;
            _mainViewModel.ProductManipulationStarted += ViewModel_StartingProductManipulation;
            _mainViewModel.ProductManipulationFinished += ViewModel_FinishingProductManipulation;

            _mainView = new MainWindow
            {
                DataContext = _mainViewModel
            };



            _loginView.Show();
        }

        private void ViewModel_ExitRequested(object sender, EventArgs e)
        {
            Current.Shutdown();
        }

        private void ViewModel_LoginSucceeded(object sender, LoginSucceededEventArgs e)
        {
            _loginView.Hide();
            _mainViewModel.UserName = e.UserName;
            _mainView.Show();
        }

        private void ViewModel_LoginFailed(object sender, EventArgs e)
        {
            MessageBox.Show("Login failed!", "Beerka", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        private void ViewModel_LogoutSucceeded(object sender, EventArgs e)
        {
            _mainView.Hide();
            _loginView.Show();
        }

        private void ViewModel_MessageApplication(object sender, MessageEventArgs e)
        {
            MessageBox.Show(e.Message, "Beerka", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        private void ViewModel_StartingProductManipulation(object sender, EventArgs e)
        {
            _productEditorView = new ProductEditorWindow
            {
                DataContext = _mainViewModel
            };
            _productEditorView.ShowDialog();
        }

        private void ViewModel_FinishingProductManipulation(object sender, EventArgs e)
        {
            if (_productEditorView.IsActive)
            {
                _productEditorView.Close();
            }
        }
    }
}
