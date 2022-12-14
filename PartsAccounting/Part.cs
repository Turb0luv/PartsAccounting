namespace PartsAccounting;

public class Part
{
    
    private string name = "";
    private int price = 0;
    private int count = 0;
    private int id = 0;
    
    public string Name
    {
        get => name;
        set
        {
            if (!value.Equals(null) && !value.Trim().Equals(""))
                name = value;
        } 
    }
    public int Price
    {
        get => price;
        set
        {
            if (value <= 0)
                return;
            price = value;
        }
    }
    
    public int Count
    {
        get => count;
        set
        {
            if (value < 0)
                return;
            count = value;
        }
    }

    public int ID
    {
        get => id;
        set
        {
            if (value <= 0)
                return;
            id = value;
        }
    }
    public Part(string name, int price, int count, int id)
    {
        Name = name;
        Price = price;
        Count = count;
        ID = id;
    }
    
    protected Part(){}

    public override string ToString()
    {
        return $"{ID} | {Name} | {Price} | {Count} | {Price * Count}";
    }
}