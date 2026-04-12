using System.Windows;
using ToxicityDashboard.ViewModels;

namespace ToxicityDashboard.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}