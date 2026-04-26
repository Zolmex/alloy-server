using System.Collections.Generic;
using System.Xml.Linq;
using Common.Utilities;

namespace Common.Resources.Xml.Descriptors;

public class PlayerDesc : ObjectDesc {
    public readonly int[] Equipment;
    public readonly int[] SlotTypes;
    public Dictionary<StatType, StatDesc> Stats;

    public PlayerDesc(XElement e, string id, ushort type)
        : base(e, id, type) {
        SlotTypes = e.GetValue<string>("SlotTypes")?.CommaToArray<int>();
        Equipment = e.GetValue<string>("Equipment")?.CommaToArray<int>();

        Stats = new Dictionary<StatType, StatDesc> {
            { StatType.MaxHP, new StatDesc(e.Element("MaxHitPoints")) },
            { StatType.MaxMP, new StatDesc(e.Element("MaxMagicPoints")) },
            { StatType.Attack, new StatDesc(e.Element("Attack")) },
            { StatType.Defense, new StatDesc(e.Element("Defense")) },
            { StatType.Speed, new StatDesc(e.Element("Speed")) },
            { StatType.Dexterity, new StatDesc(e.Element("Dexterity")) },
            { StatType.Vitality, new StatDesc(e.Element("HpRegen")) },
            { StatType.Wisdom, new StatDesc(e.Element("MpRegen")) }
        };
    }
}

public class StatDesc {
    public readonly int MaxValue;
    public readonly int StartValue;

    public StatDesc(XElement e) {
        StartValue = int.Parse(e.Value);
        MaxValue = e.GetAttribute<int>("max");
    }
}