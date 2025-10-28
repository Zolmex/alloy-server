using Common.Utilities;
using System;
using System.Linq;
using System.Xml.Linq;

namespace Common.Resources.Xml.Descriptors;

public class GemstoneDesc : ItemData
{
    public override Type FieldsEnum => typeof(GemstoneField);

    public int[] SlotTypes
    {
        get => GetValue<int[]>(0);
        set => SetValue(0, value);
    }

    public string Origin
    {
        get => GetValue<string>(1);
        set => SetValue(1, value);
    }

    public GemstoneBoost[] Boosts
    {
        get => GetValue<GemstoneBoost[]>(2);
        set => SetValue(2, value);
    }

    public GemstoneDesc(XElement e, ItemData parent = null, byte parentField = 0)
    {
        SetParent(parent, parentField);
        if (e == null) // Null when instance by itemdata import
        {
            _initialized = true;
            return;
        }

        SlotTypes = e.GetAttribute<string>("slotTypes").CommaToArray<int>(",");
        Origin = e.GetAttribute<string>("origin");
        Boosts = e.Elements("Boost")
            .Select(i => new GemstoneBoost(i))
            .ToArray();

        _initialized = true;
    }
}