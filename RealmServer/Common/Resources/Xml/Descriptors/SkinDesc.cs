using Common.Utilities;
using System.Xml.Linq;

namespace Common.Resources.Xml.Descriptors;

public class SkinDesc
{
    public readonly string Id;
    public readonly ushort Type;

    public readonly ushort PlayerClassType;

    public SkinDesc(XElement e, string id, ushort type)
    {
        Id = id;
        Type = type;
        PlayerClassType = e.GetValue<ushort>("PlayerClassType");
    }
}