using System.Windows;
using ToxicityDashboard.Services;
using ToxicityDashboard.ViewModels;

namespace ToxicityDashboard.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            var apiService = new ApiService();
            DataContext = new LoginViewModel(apiService, this);
        }
    }
}