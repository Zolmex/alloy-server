using System.Xml.Linq;
using Common.Utilities;

namespace Common.Resources.Xml.Descriptors;

public class SpawnDesc {
    public readonly int Deviation;
    public readonly int Max;
    public readonly int Mean;
    public readonly int Min;

    public SpawnDesc(XElement e) {
        Mean = e.GetValue<int>("Mean");
        Deviation = e.GetValue<int>("StdDev");
        Min = e.GetValue<int>("Min");
        Max = e.GetValue<int>("Max");
    }
}