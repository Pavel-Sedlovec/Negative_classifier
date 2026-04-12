using System;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ToxicityDashboard.Services;
using System.Windows.Controls;

namespace ToxicityDashboard.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly ApiService _apiService;
        private readonly Window _loginWindow;

        [ObservableProperty]
        private string login = string.Empty;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private bool isLoading = false;

        public LoginViewModel(ApiService apiService, Window loginWindow)
        {
            _apiService = apiService;
            _loginWindow = loginWindow;
        }

        [RelayCommand]
        public async Task DoLogin(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            string password = passwordBox?.Password ?? string.Empty;

            if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(password))
            {
                ErrorMessage = "Введите логин и пароль.";
                return;
            }

            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                var result = await _apiService.LoginAsync(Login, password);
                if (result != null)
                {
                    var mainWindow = new Views.MainWindow(new MainViewModel(_apiService));
                    mainWindow.Show();
                    _loginWindow.Close();
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (Exception)
            {
                ErrorMessage = "Ошибка связи с сервером.";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}