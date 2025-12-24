using Common.Utilities;
using System.Xml.Linq;

namespace Common.Resources.Xml.Descriptors;

public class ContainerDesc : ObjectDesc
{
    public readonly int[] Equipment;
    public readonly int[] SlotTypes;

    public ContainerDesc(XElement e, string id, ushort type)
        : base(e, id, type)
    {
        SlotTypes = e.GetValue<string>("SlotTypes")?.CommaToArray<int>();
        Equipment = e.GetValue<string>("Equipment")?.CommaToArray<int>();
    }
}