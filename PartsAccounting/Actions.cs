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

            if (CheckName(st[0]) || CheckNameId(st[0]))
            {
                Part par;
                try
                {
                    par = ChoosePartId(Int32.Parse(st[0]));
                }
                catch
                {
                    par = ChoosePart(st[0]);
                }

                if(par.Price > Int32.Parse(st[1]))
                    EditPrice(st[1], st[0]);
                EditCount(st[2], st[0]);
                return "Существующая запчасть изменена";
            }

            var rand = new Random();
            int id;
            do
            {
                id = rand.Next(1000, 10000);
            } while (CheckID(id));
            
            Part part = new Part(st[0].Replace('_', ' '), Int32.Parse(st[1]), Int32.Parse(st[2]), id);
            _parts.Add(part);
            AddHistory(part + " | Добавлен");
        }
        catch
        {
            return "Введены неверные данные";
        }

        return "Запчасть успешно добавлена";
    }
    
    public bool CheckID(int id) => _parts.Any(part => part.ID == id);

    private void AddHistory(string str)
    {
        _history.AddPart(str);
    }
    public string RemovePart(string name)
    {
        if(_parts.Count == 0) return "Запчасти отсутствуют";

        Part part;
        try
        {
            part = ChoosePartId(Int32.Parse(name));
        }
        catch
        {
            part = ChoosePart(name);
        }
        if(part != null)
        {
            _parts.Remove(part);
            AddHistory(part + " | Удалено");
            
            return Display(new List<Part> {part} ) + "\nУспешно удалён";
        }
        
        return "Запчастей с такой номенклатурой не существует";
    }

    public string Display() => Display(_parts);
    public string DisplayH() => _history.ToString();
    public string Display<T>(List<T> parts)
    {
        if(parts.Count == 0) return "Запчасти отсутствуют.";
        
        string str = "ID | Номенклатура | Цена | Кол-во | Сумма\n\n";
        int i = 0;
             
        foreach (var part in parts) {
            str += ++i + ". " + part + "\n";
        }

        return str;
    }

    public string FinalStr(string final) => final + $"\nОбщее кол-во: {SumCount()}\nОбщая сумма: {SumPart()}";
    private string SumPart() => _parts.Sum(part => part.Price * part.Count).ToString();
    private string SumCount() => _parts.Sum(part => part.Count).ToString();

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

    private Part ChoosePart(string name) => _parts.Where(part => part.Name.Equals(name)).FirstOrDefault();

    private Part ChoosePartId(int id) => _parts.FirstOrDefault(part => part.ID == id);
    public void EditName(string newName, string name)
    {
        Part part;
        try
        {
            part = ChoosePartId(Int32.Parse(name));
        }
        catch
        {
            part = ChoosePart(name);
        }
        part.Name = newName;
        AddHistory(part.ToString() + " | Ред. номенкл.");
    }
    
    public void EditPrice(string a, string name)
    {
        Part part;
        try
        {
            part = ChoosePartId(Int32.Parse(name));
        }
        catch
        {
            part = ChoosePart(name);
        }
        int price = 0;
        try
        {
            price = Int32.Parse(a);
        } catch {return;}

        part.Price += price;
        AddHistory(part.ToString() + " | Ред. цена");
    }

    public void EditCount(string pop, string name)
    {
        Part part;
        try
        {
            part = ChoosePartId(Int32.Parse(name));
        }
        catch
        {
            part = ChoosePart(name);
        }
        int count = 0;
        try
        {
            count = Int32.Parse(pop);
        } catch {return;}

        part.Count += count;
        AddHistory(part.ToString() + " | Ред. кол-во.");
    }
    
    public bool CheckName(string name) => _parts.Any(part => part.Name.ToLower() == name);
    public bool CheckNameId(string name) => _parts.Any(part => part.ID.ToString().Trim() == name);
    
    public void SerialiseToXmlParts()
    {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Part>));
        StreamWriter write = new StreamWriter(@"D:\Projects\C#\PartsAccounting\PartsAccounting\Parts.xml");
        xmlSerializer.Serialize(write, _parts);
    }
    
    public void SerialiseToXmlHistory()
    { 
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<string>));
        StreamWriter write = new StreamWriter(@"D:\Projects\C#\PartsAccounting\PartsAccounting\History.xml");
        xmlSerializer.Serialize(write, _history._actions);
    }
}