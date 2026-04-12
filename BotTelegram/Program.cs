using System.Net.Http.Json;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Microsoft.Extensions.Configuration;

namespace BotTelegram
{
    internal class Program
    {
        private static readonly string _apiUrl = "http://localhost:5146";
        private static readonly HttpClient _httpClient = new HttpClient();

        static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();
            string botToken = config["BotConfiguration:BotToken"];

            var bot = new TelegramBotClient(botToken);

            var cts = new CancellationTokenSource();

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[] { UpdateType.Message }
            };

            bot.StartReceiving(
                updateHandler: HandleUpdateAsync,
                errorHandler: HandleErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );

            var me = await bot.GetMe();
            Console.WriteLine($"Бот запущен: @{me.Username}");
            Console.WriteLine("Нажми Enter для остановки");
            Console.ReadLine();

            cts.Cancel();
        }

        static async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
        {
            if (update.Message is not { Text: { } text } message) return;

            var chatId = message.Chat.Id;
            var userId = message.From.Id;

            if (message.Chat.Type == ChatType.Private)
            {
                await HandlePrivateChat(bot, message, text, ct);
            }
            else
            {
                await HandleGroupChat(bot, message, text, ct);
            }
        }

        private static async Task HandlePrivateChat(ITelegramBotClient bot, Message message, string text, CancellationToken ct)
        {
            if (!text.StartsWith("/start"))
            {
                await bot.SendMessage(message.Chat.Id, "Используйте /start для регистрации.", cancellationToken: ct);
                return;
            }

            var request = new DTOs.Requests.SetAdminRequest
            {
                UserIdTg = message.From.Id,
            };

            var response = await _httpClient.PostAsJsonAsync(
                $"{_apiUrl}/api/Auth/SetAdmin", request);

            if (!response.IsSuccessStatusCode)
            {
                await bot.SendMessage(message.Chat.Id, "Ошибка сервера.", cancellationToken: ct);
                return;
            }

            var result = await response.Content.ReadFromJsonAsync<DTOs.Responses.AdminRegistrationResponse>();

            string msg = result.Message == "Аккаунт уже существует"
                ? "Вы уже зарегистрированы, добавьте бота в чат."
                : $"Аккаунт создан\n\nЛогин: `{result.Login}`\nПароль: `{result.Password}`\n\nДобавьте бота в чат";

            await bot.SendMessage(message.Chat.Id, msg, parseMode: ParseMode.Markdown, cancellationToken: ct);
        }

        private static async Task HandleGroupChat(ITelegramBotClient bot, Message message, string text, CancellationToken ct)
        {
            if (text.StartsWith("/"))
            {
                var command = text.Split(' ')[0].ToLower();
                switch (command)
                {
                    case "/stats": await HandleStatsCommand(bot, message.Chat.Id, ct); break;
                    case "/top": await HandleTopCommand(bot, message.Chat.Id, ct); break;
                }
                return;
            }

            await ClassifyAndSave(bot, message, text, ct);
        }

        static async Task ClassifyAndSave(ITelegramBotClient bot, Message message, string text, CancellationToken ct)
        {
            try
            {
                var request = new DTOs.Requests.TgMessageRequest
                {
                    Text = text,
                    ChatIdTg = message.Chat.Id,
                    ChatName = message.Chat.Title ?? "Группа",
                    UserIdTg = message.From.Id,
                    Username = message.From.Username ?? "",
                    FirstName = message.From.FirstName ?? ""
                };

                var response = await _httpClient.PostAsJsonAsync(
                    $"{_apiUrl}/api/Telegram/Message", request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<DTOs.Responses.ClassifyResponse>();

                    if (result?.Label == 1)
                    {
                        await bot.SendMessage(
                            message.Chat.Id,
                            $"Сообщение от @{message.From.Username} распознано как токсичное ({result.Confidence:P0})",
                            replyParameters: message.MessageId,
                            cancellationToken: ct);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error]: {ex.Message}");
            }
        }

        static async Task HandleStatsCommand(ITelegramBotClient bot, long chatId, CancellationToken ct)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiUrl}/api/Stats/GeneralChat/{chatId}");
                if (response.IsSuccessStatusCode)
                {
                    var stats = await response.Content.ReadFromJsonAsync<DTOs.Responses.StatsResponse>();
                    string text = $"Статистика чата\n\n" +
                                  $"Всего сообщений: {stats.Total}\n" +
                                  $"Токсичных: {stats.Negative} ({stats.NegativePercent:F1}%)\n" +
                                  $"Нормальных: {stats.Positive} ({stats.PositivePercent:F1}%)";
                    await bot.SendMessage(chatId, text, cancellationToken: ct);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка статистики: {ex.Message}");
            }
        }

        static async Task HandleTopCommand(ITelegramBotClient bot, long chatId, CancellationToken ct)
        {
            try
            {
                int count = 5;
                var response = await _httpClient.GetAsync($"{_apiUrl}/api/Stats/TopNegative/{chatId}/{count}");

                if (response.IsSuccessStatusCode)
                {
                    var top = await response.Content.ReadFromJsonAsync<List<DTOs.Responses.StatsTopNegativeUsersResponse>>();

                    if (top == null || top.Count == 0)
                    {
                        await bot.SendMessage(chatId, "Статистика пуста", cancellationToken: ct);
                        return;
                    }

                    string text = "Топ негатива:\n\n";
                    for (int i = 0; i < top.Count; i++)
                    {
                        text += $"{i + 1}. ID: `{top[i].UserIdTg}` — *{top[i].NegativeMes}* токсичных ({top[i].NegativePercent:F1}%) из {top[i].TotalMes}\n";
                    }

                    await bot.SendMessage(chatId, text, parseMode: ParseMode.Markdown, cancellationToken: ct);
                }
                else
                {
                    await bot.SendMessage(chatId, "Не удалось получить статистику от сервера.", cancellationToken: ct);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                await bot.SendMessage(chatId, "Ошибка при формировании топа", cancellationToken: ct);
            }
        }

        static Task HandleErrorAsync(ITelegramBotClient bot, Exception ex, CancellationToken ct)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
            return Task.CompletedTask;
        }
    }
}