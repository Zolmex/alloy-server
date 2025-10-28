using Common.Utilities;
using System;
using System.Xml.Linq;

namespace Common.Resources.Xml.Descriptors;

public class GemstoneBoost : ItemData
{
    public override Type FieldsEnum => typeof(GemstoneBoostField);

    public string BoostType
    {
        get => GetValue<string>(0);
        set => SetValue(0, value);
    }

    public string Stat
    {
        get => GetValue<string>(1);
        set => SetValue(1, value);
    }

    public float Amount
    {
        get => GetValue<float>(2);
        set => SetValue(2, value);
    }

    public string BoostTarget
    {
        get => GetValue<string>(3);
        set => SetValue(3, value);
    }

    public GemstoneBoost(XElement e, ItemData parent = null, byte parentField = 0)
    {
        SetParent(parent, parentField);
        if (e == null) // Null when instance by itemdata import
        {
            _initialized = true;
            return;
        }

        Stat = e.GetAttribute<string>("stat");

        var amt = e.GetAttribute<string>("amount");
        var percentIndex = amt.IndexOf('%');
        Amount = float.Parse(amt.Substring(0, percentIndex == -1 ? amt.Length : percentIndex));
        BoostType = percentIndex == -1 ? "Static" : "Percentage";
        BoostTarget = e.Value;

        _initialized = true;
    }
}