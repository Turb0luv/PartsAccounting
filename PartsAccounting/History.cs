namespace PartsAccounting;

public class History
{
    public List<string> _actions { get; set; } = new List<string>();

    public History(){}

    public History(List<string> s)
    {
        _actions = s;
    }

    public void AddPart(string action)
    {
        _actions.Add(action + " | " + DateTime.Now);
    }
    
    public override string ToString()
    {
        if (_actions.Count == 0) return "История пуста";
        string str = "ID | Номенклатура | Цена | Кол-во | Сумма | Действие | Время\n\n";
        foreach (var action in _actions)
        {
            str += action + "\n";
        }

        return str;
    }
    
}