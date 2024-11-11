using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

internal class Program
{
    private static void Main(string[] args)//Точка входа в программу
    {
        Start();
        Console.ReadLine();
    }
    //Метод запуска обработки вашего бота
    private static async void Start()
    {
        //Вставляем наш токен в параметры класса TelegramBotClient
        var botClient = new TelegramBotClient(ConsoleTelegramBot.Properties.Resources.Token);

        using CancellationTokenSource cts = new();
        //Указываем параметры запросов
        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };
        //Запускаем обработку бота
        botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
        );
        //Выводим сообщение в консоль
        var me = await botClient.GetMeAsync();
        Console.WriteLine($"Start listening for @{me.Username}");
        Console.ReadLine();

        cts.Cancel();
    }
    //Метод обработки Сообщений
    private static async Task HandleUpdateMessagesAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        //Переменных для удобства обращения к данным
        var message = update.Message;
        var messageText = message!.Text;
        var chatId = message.Chat.Id;
        var userName = message.Chat.Username;

        //Вывод сообщения о сообщение и пользователе отправившего его 
        Console.WriteLine($"User Name - {userName},  ID - {chatId}\nMessage - {message.Text}\n");

        //Обработка стартового сообщения
        if (message.Text == "/start")
        {
            //Добавление кнопок в сообщении
            InlineKeyboardMarkup inlineKeyboard = new(new[]
            {
                new []//отдельная линия с кнопками
                {
                    InlineKeyboardButton.WithUrl(//добавление кнопки
                        text: "StackOverFlow",//название кнопки
                        url: "https://stackoverflow.com/"),//ссылка
                },
                new []
                {
                    InlineKeyboardButton.WithUrl(
                        text: "Мануал о создании",
                        url: "https://telegrambots.github.io/book/index.html"),

                    InlineKeyboardButton.WithUrl(
                        text: "О нас",
                        url: "https://1-mok.mskobr.ru/postuplenie-v-kolledzh/priemnaya-komissiya"),
                },
                //Добавть свою линию кнопок

              /*   new []
                {
                    InlineKeyboardButton.WithUrl(
                       text: "Название1",
                        url: "Место для сслыки1"),

                    InlineKeyboardButton.WithUrl(
                        text: "Название2",
                       url: "Место для сслыки2"),
                },*/
            });
            //отпраляем собщение с кнопками
            await botClient.SendTextMessageAsync(
                 chatId: chatId,//Индентификатор чата в который отправляется сообщение
                 text: "Выберите ссылку",//Сообщение
                 replyMarkup: inlineKeyboard,//Добавленные к сообщению кнопки
                 cancellationToken: cancellationToken//Указываем методя для обработки исключений
                 );

            //Добавляем кнопкок-клавиатуры
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
                new KeyboardButton[] { "StackOverFlow" },
                new KeyboardButton[] { "Мануал о создании", "О нас" },
                //Добавть свою линию кнопок
                //new KeyboardButton[] { "НазваниеКнопки1", "НазваниеКнопки2" },
            })
            {
                // Разрешаем автоматическое расширение размера кнопок
                ResizeKeyboard = true 
            };
            //отпраляем собщение с добавление клавиатуры
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Кнопки добавлены",
                replyMarkup: replyKeyboardMarkup,
                cancellationToken: cancellationToken);
            return;
        }
        //Поверка отправляемы пользователем сообщений  на соотвествие
        switch (message.Text)
        {
            //Должно совпадать с название текста которое требуется обработать
            case "StackOverFlow":
                //Отправляем сообщение
                await botClient.SendTextMessageAsync(
                  chatId: chatId,
                  text: "https://stackoverflow.com/",
                  cancellationToken: cancellationToken);
                break;//Окончание обработки текста

            case "Мануал о создании":
                await botClient.SendTextMessageAsync(
                  chatId: chatId,
                  text: "https://telegrambots.github.io/book/index.html",
                  cancellationToken: cancellationToken);
                break;
            case "О нас":
                await botClient.SendTextMessageAsync(
                  chatId: chatId,
                  text: "https://1-mok.mskobr.ru/postuplenie-v-kolledzh/priemnaya-komissiya",
                  cancellationToken: cancellationToken);
                break;

            //Добавте обработку сообщения

            case "привет":

                await botClient.SendTextMessageAsync(
                  chatId: chatId,
                  text: "привет!!!",
                  cancellationToken: cancellationToken);

                break;

            default://Обработка не разпознаного сообщения
                await botClient.SendTextMessageAsync(
                  chatId: chatId,
                  text: "Сообщене не распознано",
                  cancellationToken: cancellationToken);
                break;
        }

    }
    //Метод обработки действий пользователя
    private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        //Проверка на наличие сообщение и текста в нём
        if (update.Message is not { } message)
        {
            return;
        }
        if (message.Text is not { } messageText)
        {
            return;
        }

        //Запуск Метода проверки сообщений
        await HandleUpdateMessagesAsync(botClient, update, cancellationToken);
    }

    //Метод для обратоки исключительных ситуаций
    private static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }
}