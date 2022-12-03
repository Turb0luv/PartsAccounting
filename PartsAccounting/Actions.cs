using System.Xml.Serialization;

namespace PartsAccounting;
[XmlInclude(typeof(Part))]
[XmlInclude(typeof(History))]
public class Actions
{
    private readonly List<Part> _parts = new List<Part>();
    private History _history = new History();

    public Actions(List<Part> parts, History history)
    {
        _parts = parts;
        _history = history;
    }
    public void SortByName() =>
        _parts.Sort((x, y) => String.Compare(x.Name, y.Name,
            StringComparison.Ordinal));
    
    public void SortByPrice() =>
        _parts.Sort((x, y) => x.Price.CompareTo(y.Price));
    
    public void SortByCount() =>
        _parts.Sort((x, y) => x.Count.CompareTo(y.Count));

    public string AddPart(string str)
    {
        string[] st = str.Split(" ");
        if (st.Length != 3) return "Неверное кол-во параметров";

        try
        {
            if (Int32.Parse(st[1]) <= 0 || Int32.Parse(st[2]) <= 0)
                return "Введены неверные числовые значения";

            if (CheckName(st[0]))
                return "Запчастей с такой номенклатурой не существует";

            Part part = new Part(st[0], Int32.Parse(st[1]), Int32.Parse(st[2]));
            _parts.Add(part);
            AddHistory(part + " | Добавлен");
        }
        catch
        {
            return "Введены неверные данные";
        }

        return "Запчасть успешно добавлена";
    }
    

    private void AddHistory(string str)
    {
        _history.AddPart(str);
    }
    public string RemovePart(string name)
    {
        if(_parts.Count == 0) return "Запчасти отсутствуют";

        foreach (var part in _parts.Where(part => part.Name.Equals(name)))
        {
            _parts.Remove(part);
            AddHistory(part + " | Удалено");
            
            return Display(new List<Part> {part} ) + "\nУспешно удалён";
        }
        
        return "Запчастей с такой номенклатурой не существует";
    }

    public string Display()
    {
        return Display(_parts);
    }
    public string DisplayH()
    {
        return _history.ToString();
    }
    public string Display<T>(List<T> parts)
    {
        if(parts.Count == 0) return "Запчасти отсутствуют.";
        
        string str = "Номенклатура | Цена | Кол-во | Сумма\n\n";
        int i = 0;
             
        foreach (var part in parts) {
            str += ++i + ". " + part + "\n";
        }

        return str;
    }

    public List<Part> Search(string str)
    {
        if (_parts.Count == 0) return new List<Part>();
        
        List<string> names = new List<string>();
        List<Part> prt = new List<Part>();

        foreach (var part in _parts)
        {
            foreach (var s in str.Split(" "))
            {
                if(part.ToString().Contains(s) && !names.Contains(part.Name)) {
                    prt.Add(part);
                    names.Add(part.Name);
                }
            }
        }
        
        return prt;
    }

    private Part ChoosePart(string name)
    {
        return _parts.Where(part => part.Name.Equals(name)).FirstOrDefault();
    }
    public void EditName(string newName, string name)
    {
        Part part = ChoosePart(name);
        part.Name = newName;
        AddHistory(part.ToString() + " | Ред. номенкл.");
    }
    
    public void EditPrice(string a, string name)
    {
        Part part = ChoosePart(name);
        int price = 0;
        try
        {
            price = Int32.Parse(a);
        } catch {return;}

        part.Price = price;
        AddHistory(part.ToString() + " | Ред. цена");
    }

    public void EditCount(string pop, string name)
    {
        Part part = ChoosePart(name);
        int count = 0;
        try
        {
            count = Int32.Parse(pop);
        } catch {return;}

        part.Count = count;
        AddHistory(part.ToString() + " | Ред. кол-во.");
    }
    
    public bool CheckName(string name) =>
        _parts.Any(part => part.Name.ToLower() == name);
    
    public void SerialiseToXml()
    {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Part>));
        StreamWriter write = new StreamWriter(@"D:\Projects\C#\PartsAccounting\PartsAccounting\base.xml");
        xmlSerializer.Serialize(write, _parts);
    }
    
    public void SerialiseToXml1()
    { 
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<string>));
        StreamWriter write = new StreamWriter(@"D:\Projects\C#\PartsAccounting\PartsAccounting\base1.xml");
        xmlSerializer.Serialize(write, _history._actions);
    }
}