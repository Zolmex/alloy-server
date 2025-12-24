namespace Common.Resources.Xml.Descriptors;

public class ItemFieldBoost
{
    public ItemFieldBoost(bool flat, float amount)
    {
        if (flat)
            FlatAmount = amount;
        else
            ProportionalAmount = amount;
    }

    public float FlatAmount { get; private set; }
    public float ProportionalAmount { get; private set; }

    public void Add(bool flat, float amount)
    {
        if (flat)
            FlatAmount += amount;
        else
            ProportionalAmount += amount;
    }
}