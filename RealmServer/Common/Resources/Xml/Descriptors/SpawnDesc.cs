using Common.Utilities;
using System.Xml.Linq;

namespace Common.Resources.Xml.Descriptors;

public class SpawnDesc
{
    public readonly int Max;
    public readonly int Mean;
    public readonly int Min;
    public readonly int Deviation;

    public SpawnDesc(XElement e)
    {
        Mean = e.GetValue<int>("Mean");
        Deviation = e.GetValue<int>("StdDev");
        Min = e.GetValue<int>("Min");
        Max = e.GetValue<int>("Max");
    }
}