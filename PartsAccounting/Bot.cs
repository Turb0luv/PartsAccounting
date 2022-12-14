using PartsAccounting;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
static class Bot
    {
        static ITelegramBotClient bot = new TelegramBotClient("5715651192:AAHvCCg-3KZmA3BOtFzqxCAjiTDQEzq7xEg");

        static void Main(string[] args)
        {
            Commands.Deserialize();

            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };
            
            bot.StartReceiving(
                Update,
                Error,
                receiverOptions,
                cancellationToken
            );

            Console.ReadLine();
            Commands.Serialize();
        }
        
        private static async Task Update(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));

            if (update.Type == UpdateType.CallbackQuery)
            {
               Commands.HandleCallbackQuery(update.CallbackQuery);
                return;
            }
            
            if (update.Message == null) return;
            
            if (update.Type == UpdateType.Message && update.Message?.Text != null)
            {
                Commands.HandlerMessage(update.Message);
                return;
            }
        }

        public static async Task SendMessage(Message message, string text, IReplyMarkup replyMarkup)
        {
            await bot.SendTextMessageAsync(message.Chat.Id, text, 
                replyMarkup: replyMarkup, parseMode: ParseMode.Html);
        }
        
        public static IReplyMarkup GetButtons(string[] btn)
        {
            ReplyKeyboardMarkup keyboardMarkup = new(new[]
            {
                new KeyboardButton[] { btn[0], btn[1] },
                new KeyboardButton[] { btn[2], btn[3], btn[4] },
                new KeyboardButton[] { btn[5], btn[6] }
            })
            {
                ResizeKeyboard = true
            };

            return keyboardMarkup;
        }

        public static IReplyMarkup GetInlineButtons(string[] btn)
        {
            InlineKeyboardMarkup keyboard = new(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(btn[0], btn[0]),
                    InlineKeyboardButton.WithCallbackData(btn[1], btn[1]),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(btn[2], btn[2]),
                },
            });

            return keyboard;
        }

        private static async Task Error(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }
    }
