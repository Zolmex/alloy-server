using Common.Utilities;
using System;
using System.Xml.Linq;

namespace Common.Resources.Xml.Descriptors;

public class EquipmentBoost : ItemData
{
    public override Type FieldsEnum => typeof(EquipmentBoostField);

    public int Stat
    {
        get => GetValue<int>(0);
        set => SetValue(0, value);
    }

    public int Amount
    {
        get => GetValue<int>(1);
        set => SetValue(1, value);
    }

    public LevelIncreaseDesc LevelIncrease
    {
        get => GetValue<LevelIncreaseDesc>(2);
        set => SetValue(2, value);
    }

    public EquipmentBoost(XElement e, ItemData parent = null, byte parentField = 0)
    {
        SetParent(parent, parentField);
        if (e == null) // Null when instance by itemdata import
        {
            _initialized = true;
            return;
        }

        Stat = e.GetAttribute<int>("stat");
        Amount = e.GetAttribute<int>("amount");
        LevelIncrease = e.HasElement("LevelIncrease")
            ? new LevelIncreaseDesc(e.Element("LevelIncrease"), this, (byte)EquipmentBoostField.LevelIncrease)
            : null;

        _initialized = true;
    }
}