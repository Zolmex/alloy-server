using Common.Utilities;
using System.Xml.Linq;

namespace Common.Resources.Xml.Descriptors;

public class TileDesc
{
    public readonly int Damage;
    public readonly float DX;
    public readonly float DY;
    public readonly string GroundId;
    public readonly ushort GroundType;
    public readonly bool NoWalk;
    public readonly bool Push;
    public readonly bool Sinking;
    public readonly float Speed;

    public TileDesc(XElement e, string id, ushort type)
    {
        GroundId = id;
        GroundType = type;
        NoWalk = e.HasElement("NoWalk");
        Damage = e.GetValue<int>("Damage");
        Speed = e.GetValue("Speed", 1.0f);
        Sinking = e.HasElement("Sinking");
        if (Push = e.HasElement("Push"))
        {
            DX = e.Element("Animate").GetAttribute<float>("dx") / 1000f;
            DY = e.Element("Animate").GetAttribute<float>("dy") / 1000f;
        }
    }
}