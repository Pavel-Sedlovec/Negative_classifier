using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using ToxicityDashboard.Models;
using ToxicityDashboard.Services;

namespace ToxicityDashboard.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        public string AdminName => _apiService.CurrentAdminLogin;

        [ObservableProperty]
        private ObservableCollection<ChatInfo> chats = new();

        [ObservableProperty]
        private ChatInfo? selectedChat;

        [ObservableProperty]
        private int totalMessages;

        [ObservableProperty]
        private string toxicityPercent = "0%";

        [ObservableProperty]
        private ObservableCollection<StatsTopNegativeUsersResponse> topNegativeUsers = new();

        [ObservableProperty]
        private ISeries[] pieChartSeries = new ISeries[0];

        [ObservableProperty]
        private DateTime selectedDate = DateTime.Now;

        [ObservableProperty]
        private ISeries[] dayPieChartSeries = new ISeries[0];

        [ObservableProperty]
        private string dayToxicityPercent = "0%";

        public MainViewModel(ApiService apiService)
        {
            _apiService = apiService;
            _ = LoadChatsAsync();
        }

        private async Task LoadChatsAsync()
        {
            var chatList = await _apiService.GetChatsAsync();
            Chats = new ObservableCollection<ChatInfo>(chatList);
        }

        partial void OnSelectedChatChanged(ChatInfo? value)
        {
            if (value != null)
            {
                _ = LoadChatStatsAsync(value.ChatIdTg);
                _ = LoadDayStatsAsync();
            }
        }

        private async Task LoadChatStatsAsync(long chatId)
        {
            var stats = await _apiService.GetGeneralStatsAsync(chatId);
            if (stats != null)
            {
                TotalMessages = stats.Total;
                ToxicityPercent = $"{stats.NegativePercent:F1}%";
                PieChartSeries = new ISeries[]
                {
                    new PieSeries<int>
                    {
                        Values = new[] { stats.Positive },
                        Name = "Адекватно",
                        Fill = new SolidColorPaint(SKColors.MediumAquamarine),
                        DataLabelsPaint = new SolidColorPaint(SKColors.White),
                        DataLabelsSize = 14,
                        DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle
                    },
                    new PieSeries<int>
                    {
                        Values = new[] { stats.Negative },
                        Name = "Токсично",
                        Fill = new SolidColorPaint(SKColors.Tomato),
                        DataLabelsPaint = new SolidColorPaint(SKColors.White),
                        DataLabelsSize = 14,
                        DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle
                    }
                };


            }

            var topUsers = await _apiService.GetTopNegativeUsersAsync(chatId);
            TopNegativeUsers = new ObservableCollection<StatsTopNegativeUsersResponse>(topUsers);
        }


        private async Task LoadDayStatsAsync()
        {
            if (SelectedChat == null) return;

            var stats = await _apiService.GetStatsByDayAsync(SelectedChat.ChatIdTg, SelectedDate);

            if (stats != null && stats.Total > 0)
            {
                DayToxicityPercent = $"{stats.NegativePercent:F1}%";
                DayPieChartSeries = new ISeries[]
                {
                    new PieSeries<int> {
                        Values = new[] { stats.Positive },
                        Name = "Адекватно",
                        Fill = new SolidColorPaint(SKColors.MediumAquamarine)
                    },
                    new PieSeries<int> {
                        Values = new[] { stats.Negative },
                        Name = "Токсично",
                        Fill = new SolidColorPaint(SKColors.Tomato)
                    }
                };
            }
            else
            {
                DayPieChartSeries = new ISeries[0];
                DayToxicityPercent = "Нет данных";
            }
        }

        [RelayCommand]
        private void Logout(System.Windows.Window window)
        {
            var loginWin = new Views.LoginWindow();
            loginWin.Show();
            window.Close();
        }

        partial void OnSelectedDateChanged(DateTime value)
        {
            _ = LoadDayStatsAsync();
        }
    }
}