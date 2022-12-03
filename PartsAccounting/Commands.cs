using System.Xml;
using System.Xml.Serialization;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace PartsAccounting;

public static class ChooseCommand
{
    private static Actions _actions = new (new List<Part>(), new History());

    private static string _newPart = "undefined",
        _search = "undefined",
        _deleteName = "undefined",
        _edit = "undefined", _eName = "undefined", _ePrice = "undefined", _eCount = "undefined";

    private static IReplyMarkup _buttons = null!;

    public static void Deserialize()
    {
        _actions = new Actions(DesParts(), DesHis());
    }
    
    private static History DesHis()
    {
        History history = new History();
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<string>));
        
        XmlReader textReader = XmlReader.Create(@"D:\Projects\C#\PartsAccounting\PartsAccounting\base1.xml");
        try
        {
            history = new History((List<string>)xmlSerializer.Deserialize(textReader));
        }
        catch
        {
            // ignored
        }

        textReader.Close();
        return history;
    }
    private static List<Part> DesParts()
    {
        List<Part> parts = new List<Part>();
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Part>));
        
        XmlReader textReader = XmlReader.Create(@"D:\Projects\C#\PartsAccounting\PartsAccounting\base.xml");
        try
        {
            parts = (List<Part>)xmlSerializer.Deserialize(textReader);
        }
        catch
        {
            // ignored
        }

        textReader.Close();
        return parts;
    }
    public static void Serialize() 
    {
        _actions.SerialiseToXml1();
        _actions.SerialiseToXml();
    }
    
    public static void HandlerMessage(Message message)
    {
        if (message.Text == null) return;
        
        string messText = message.Text.ToLower().Trim();
        
        if (_newPart == "")
        {
            _newPart += messText;
                
            ViewTelegram.SendMessage(message, 
                _actions.AddPart(_newPart), null);
            
            return;
        }

        if (_search == "")
        {
            ViewTelegram.SendMessage(message,
                _actions.Display(_actions.Search(messText)), null);
                
            _search = messText;
            return;
        }
        
        if (_deleteName == "")
        {
            ViewTelegram.SendMessage(message,
                _actions.RemovePart(messText), null);
            _deleteName = messText;
            return;
        }
        
        if (_edit == "")
        {
            if (!_actions.CheckName(messText))
            {
                ViewTelegram.SendMessage(message,
                    "Запчасти с данной номенклатурой не найдены", null);
                _edit = "undefined";
                return;
            }

            _edit = messText;
            _buttons = ViewTelegram.GetInlineButtons(
                new[] {"Ред. кол-во" , "Ред. цену", "Ред. номенклатуру" });
                            
            ViewTelegram.SendMessage(
                message, "Выберите параметр для редактирования", _buttons);
            return;
        }

        if (_eName == "")
        {
            if(_actions.CheckName(messText)) {
                ViewTelegram.SendMessage(message,
                    "Запчасть с данной номенклатурой уже существует", null);
                _eName = "undefined";
                return;
            }

            _eName = messText;
            _actions.EditName(_eName, _edit);
            
            MessageDone(message);
            return;
        }
        
        if (_ePrice == "")
        {
            _ePrice = messText;
            _actions.EditPrice(_ePrice, _edit);
            
            MessageDone(message);
            return;
        }
        
        if (_eCount == "")
        {
            _eCount = messText;
            _actions.EditCount(_eCount, _edit);
            
            MessageDone(message);
            return;
        }
        
        switch (messText)
        {
            case "/start":
                _buttons = ViewTelegram.GetButtons(
                    new[] { "Добавить", "Списать", "Провести", "Список", "Сортировка", "Поиск", "История" });
                
                ViewTelegram.SendMessage(message, $"Добрый день, {message.Chat.FirstName}\n" +
                                                  "Выберите действие", _buttons);
                return;

            case "добавить":
                ViewTelegram.SendMessage(
                    message, "Введите номенклатуру, цену, кол-во (Пример: Комлект_задних_стоек_Almera 130 10)", null);
                _newPart = "";
                return;
                
            case "список":
                ViewTelegram.SendMessage(
                    message, _actions.Display(), null);
                return;
            
            case "списать":
                ViewTelegram.SendMessage(
                    message, "Введите номенклатуру запчасти", null);
                _deleteName = "";
                return;
                
            case "сортировка":
                _buttons = ViewTelegram.GetInlineButtons(
                    new[] {"Цена", "Кол-во","Номенклатура" });
                    
                ViewTelegram.SendMessage(
                    message, "Выберите критерий для сортировки", _buttons);
                return;
                
            case "поиск":
                ViewTelegram.SendMessage(
                    message, "Введите любые данные", null);
                _search = "";
                return;
            
            case "провести":
                ViewTelegram.SendMessage(
                    message, "Введите номенклатуру запчасти", null);
                _edit = "";
                return;
            
            case "история":
                ViewTelegram.SendMessage(
                    message, _actions.DisplayH(), null);
                return;
        }

        _buttons = ViewTelegram.GetButtons(new[] { "/start", "", "", "", "", "", "" });
        ViewTelegram.SendMessage(message, "Такой команды нет", _buttons);
    }
    public static void HandleCallbackQuery(CallbackQuery? callbackQuery)
    {
        if (callbackQuery?.Data == null || callbackQuery.Message == null) return;
        
        string data = callbackQuery.Data.ToLower().Trim();
        
        switch (data)
        { 
            case "номенклатура": 
                _actions.SortByName();
                
                ViewTelegram.SendMessage(
                        callbackQuery.Message, "Сортировка по номенклатуре выполнена.", null);
                return;
                
            case "цена": 
                _actions.SortByPrice();
                ViewTelegram.SendMessage(
                        callbackQuery.Message,"Сортировка по цене выполнена.", null);
                return;
                
            case "кол-во": 
                _actions.SortByCount();
                ViewTelegram.SendMessage(
                        callbackQuery.Message, "Сортировка по кол-ву выполнена.", null);
                return;

            case "ред. номенклатуру":
                ViewTelegram.SendMessage(callbackQuery.Message,
                    "Введите новую номеклатуру", null);
                _eName = "";
                return;
            
            case "ред. цену":
                ViewTelegram.SendMessage(callbackQuery.Message,
                    "Введите новую цену", null);
                _ePrice = "";
                return;
            
            case "ред. кол-во":
                ViewTelegram.SendMessage(callbackQuery.Message,
                    "Введите новое кол-во", null);
                _eCount = "";
                return;
        }
    }
    private static void MessageDone(Message message)
    {
        ViewTelegram.SendMessage(
            message, "Готово", null);
    }
}