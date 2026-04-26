using System.Xml.Linq;
using Common.Utilities;

namespace Common.Resources.Xml.Descriptors;

public class SkinDesc {
    public readonly string Id;

    public readonly ushort PlayerClassType;
    public readonly ushort Type;

    public SkinDesc(XElement e, string id, ushort type) {
        Id = id;
        Type = type;
        PlayerClassType = e.GetValue<ushort>("PlayerClassType");
    }
}