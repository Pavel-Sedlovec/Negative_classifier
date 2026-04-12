using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ToxicityDashboard.Models;

namespace ToxicityDashboard.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://localhost:5146";

        public string CurrentAdminLogin { get; private set; } = string.Empty;

        public ApiService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        }

        public async Task<AdminRegistrationResponse?> LoginAsync(string login, string password)
        {
            var request = new LoginRequest { Login = login, Password = password };
            var response = await _httpClient.PostAsJsonAsync("/api/Auth/Login", request);

            if (response.IsSuccessStatusCode)
            {
                CurrentAdminLogin = login;
                return await response.Content.ReadFromJsonAsync<AdminRegistrationResponse>();
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException("Неверный логин или пароль");

            throw new Exception("Ошибка при подключении к API.");
        }

        public async Task<List<ChatInfo>> GetChatsAsync()
        {
            if (string.IsNullOrEmpty(CurrentAdminLogin)) return new List<ChatInfo>();
            var response = await _httpClient.GetAsync($"/api/Chats/GetChats/{CurrentAdminLogin}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<ChatInfo>>() ?? new List<ChatInfo>();
        }

        public async Task<MessageStatsInChat?> GetGeneralStatsAsync(long chatId)
        {
            var response = await _httpClient.GetAsync($"/api/Stats/GeneralChat/{chatId}");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<MessageStatsInChat>();
            return null;
        }

        public async Task<List<StatsTopNegativeUsersResponse>> GetTopNegativeUsersAsync(long chatId, int count = 10)
        {
            var response = await _httpClient.GetAsync($"/api/Stats/TopNegative/{chatId}/{count}");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<List<StatsTopNegativeUsersResponse>>() ?? new();
            return new List<StatsTopNegativeUsersResponse>();
        }

        public async Task<MessageStatsInChat?> GetStatsByDayAsync(long chatId, DateTime day)
        {
            string dateStr = day.ToString("yyyy-MM-dd");
            var response = await _httpClient.GetAsync($"/api/Stats/ByDay/{chatId}/{dateStr}");

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<MessageStatsInChat>();
            return null;
        }
    }
}