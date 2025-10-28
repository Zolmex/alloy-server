namespace Common.Resources.Xml.Descriptors;

public class ItemFieldBoost
{
    public float FlatAmount { get; private set; }
    public float ProportionalAmount { get; private set; }

    public ItemFieldBoost(bool flat, float amount)
    {
        if (flat)
            FlatAmount = amount;
        else
            ProportionalAmount = amount;
    }

    public void Add(bool flat, float amount)
    {
        if (flat)
            FlatAmount += amount;
        else
            ProportionalAmount += amount;
    }
}