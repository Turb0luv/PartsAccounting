using System.Xml;
using System.Xml.Serialization;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace PartsAccounting;

public static class Commands
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
        
        XmlReader textReader = XmlReader.Create(@"D:\Projects\C#\PartsAccounting\PartsAccounting\History.xml");
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
        
        XmlReader textReader = XmlReader.Create(@"D:\Projects\C#\PartsAccounting\PartsAccounting\Parts.xml");
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
        _actions.SerialiseToXmlHistory();
        _actions.SerialiseToXmlParts();
    }
    
    public static void HandlerMessage(Message message)
    {
        if (message.Text == null) return;
        
        string messText = message.Text.ToLower().Trim();
        
        if (_newPart == "")
        {
            _newPart += messText;
                
            Bot.SendMessage(message, 
                _actions.AddPart(_newPart), null);
            
            return;
        }

        if (_search == "")
        {
            Bot.SendMessage(message,
                _actions.Display(_actions.Search(messText)), null);
                
            _search = messText;
            return;
        }
        
        if (_deleteName == "")
        {
            Bot.SendMessage(message,
                _actions.RemovePart(messText), null);
            _deleteName = messText;
            return;
        }
        
        if (_edit == "")
        {
            if (!_actions.CheckName(messText) && !_actions.CheckNameId(messText))
            {
                Bot.SendMessage(message,
                    "Запчасти с данной номенклатурой не найдены", null);
                _edit = "undefined";
                return;
            }

            _edit = messText;
            _buttons = Bot.GetInlineButtons(
                new[] {"Ред. кол-во" , "Ред. цену", "Ред. номенклатуру" });
                            
            Bot.SendMessage(
                message, "Выберите параметр для редактирования", _buttons);
            return;
        }

        if (_eName == "")
        {
            if(_actions.CheckName(messText)) {
                Bot.SendMessage(message,
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
                _buttons = Bot.GetButtons(
                    new[] { "Добавить", "Списать", "Провести", "Список", "Сортировка", "Поиск", "История" });
                
                Bot.SendMessage(message, $"Добрый день, {message.Chat.FirstName}\n" +
                                                  "Выберите действие", _buttons);
                return;

            case "добавить":
                Bot.SendMessage(
                    message, "Введите номенклатуру, цену, кол-во (Пример: Комлект_задних_стоек_Almera 130 10)", null);
                _newPart = "";
                return;
                
            case "список":
                Bot.SendMessage(
                    message, _actions.FinalStr(_actions.Display()), null);
                return;
            
            case "списать":
                Bot.SendMessage(
                    message, "Введите номенклатуру запчасти", null);
                _deleteName = "";
                return;
                
            case "сортировка":
                _buttons = Bot.GetInlineButtons(
                    new[] {"Цена", "Кол-во","Номенклатура" });
                    
                Bot.SendMessage(
                    message, "Выберите критерий для сортировки", _buttons);
                return;
                
            case "поиск":
                Bot.SendMessage(
                    message, "Введите любые данные", null);
                _search = "";
                return;
            
            case "провести":
                Bot.SendMessage(
                    message, "Введите номенклатуру запчасти", null);
                _edit = "";
                return;
            
            case "история":
                Bot.SendMessage(
                    message, _actions.DisplayH(), null);
                return;
        }

        _buttons = Bot.GetButtons(new[] { "/start", "", "", "", "", "", "" });
        Bot.SendMessage(message, "Такой команды нет", _buttons);
    }
    public static void HandleCallbackQuery(CallbackQuery? callbackQuery)
    {
        if (callbackQuery?.Data == null || callbackQuery.Message == null) return;
        
        string data = callbackQuery.Data.ToLower().Trim();
        
        switch (data)
        { 
            case "номенклатура": 
                _actions.SortByName();
                
                Bot.SendMessage(
                        callbackQuery.Message, "Сортировка по номенклатуре выполнена.", null);
                return;
                
            case "цена": 
                _actions.SortByPrice();
                Bot.SendMessage(
                        callbackQuery.Message,"Сортировка по цене выполнена.", null);
                return;
                
            case "кол-во": 
                _actions.SortByCount();
                Bot.SendMessage(
                        callbackQuery.Message, "Сортировка по кол-ву выполнена.", null);
                return;

            case "ред. номенклатуру":
                Bot.SendMessage(callbackQuery.Message,
                    "Введите новую номеклатуру", null);
                _eName = "";
                return;
            
            case "ред. цену":
                Bot.SendMessage(callbackQuery.Message,
                    "Введите новую цену", null);
                _ePrice = "";
                return;
            
            case "ред. кол-во":
                Bot.SendMessage(callbackQuery.Message,
                    "Введите новое кол-во", null);
                _eCount = "";
                return;
        }
    }
    private static void MessageDone(Message message)
    {
        Bot.SendMessage(
            message, "Готово", null);
    }
}