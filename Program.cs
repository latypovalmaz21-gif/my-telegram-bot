using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

class Program
{
    private static readonly long MyChatId = 1744097143;

    static async Task Main(string[] args)
    {
        var botToken = "8918923997:AAH8fZ7UITw95XS67s9fMjYZjGEZrLm6-9k"; // ← ВСТАВЬ СВОЙ ТОКЕН

        var botClient = new TelegramBotClient(botToken);

        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        using var cts = new CancellationTokenSource();

        botClient.StartReceiving(
            HandleUpdateAsync,
            HandlePollingErrorAsync,
            receiverOptions,
            cts.Token
        );

        var me = await botClient.GetMe(cts.Token);
        Console.WriteLine($"✅ Бот @{me.Username} успешно запущен!");

        Console.ReadKey(); // Для локального запуска
    }

    static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Message is not { } message)
            return;

        var chatId = message.Chat.Id;
        var text = message.Text?.ToLower().Trim();

        if (string.IsNullOrEmpty(text)) return;

        switch (text)
        {
            case "/start":
                await botClient.SendMessage(chatId, "👋 Привет! Используй /sms для связи.", cancellationToken: cancellationToken);
                break;

            case "/sms":
            case "/связь":
                await botClient.SendMessage(chatId, "📩 Напиши сообщение для разработчика:", cancellationToken: cancellationToken);
                break;

            case "/help":
                await botClient.SendMessage(chatId, "Команды: /start, /sms, /help", cancellationToken: cancellationToken);
                break;

            default:
                if (!text.StartsWith("/"))
                {
                    // Пересылаем тебе
                    try
                    {
                        await botClient.ForwardMessage(MyChatId, chatId, message.MessageId, cancellationToken: cancellationToken);
                        await botClient.SendMessage(chatId, "✅ Сообщение отправлено!", cancellationToken: cancellationToken);
                    }
                    catch
                    {
                        await botClient.SendMessage(MyChatId, $"Сообщение от {message.From?.Username}:\n{text}", cancellationToken: cancellationToken);
                    }
                }
                break;
        }
    }

    static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Ошибка: {exception.Message}");
        return Task.CompletedTask;
    }
}