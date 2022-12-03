namespace PartsAccounting;

public class Part
{
    public string Name { get; set; }
    private int price;
    private int count;

    public int Price
    {
        get { return price; }
        set
        {
            if (value <= 0)
                return;
            price = value;
        }
    }
    
    public int Count
    {
        get { return count; }
        set
        {
            if (value < 0)
                return;
            count = value;
        }
    }

    public Part(string name, int price, int count)
    {
        Name = name;
        Price = price;
        Count = count;
    }
    
    protected Part(){}

    public override string ToString()
    {
        return $"{Name} | {Price} | {Count} | {Price * Count}";
    }
}