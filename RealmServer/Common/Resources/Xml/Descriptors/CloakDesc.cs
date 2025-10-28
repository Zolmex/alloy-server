using Common.Utilities;
using System;
using System.Linq;
using System.Xml.Linq;

namespace Common.Resources.Xml.Descriptors;

public class CloakDesc : ItemData
{
    public override Type FieldsEnum => typeof(CloakDescField);

    public short MpCost
    {
        get => GetValue<short>(0);
        set => SetValue(0, value);
    }

    public short Duration
    {
        get => GetValue<short>(1);
        set => SetValue(1, value);
    }
    
    public GemstoneBoost[] StatsModifier
    {
        get => GetValue<GemstoneBoost[]>(2);
        set => SetValue(2, value);
    }

    public float MinStatEfficiency
    {
        get => GetValue<float>(3);
        set => SetValue(3, value);
    }
    
    public short BoostDuration
    {
        get => GetValue<short>(4);
        set => SetValue(4, value);
    }

    public CloakDesc(XElement e, ItemData parent = null, byte parentField = 0)
    {
        SetParent(parent, parentField);
        if (e == null) // Null when instance by itemdata import
        {
            _initialized = true;
            return;
        }

        MpCost = e.GetValue<short>("MpCost");
        Duration = e.GetValue<short>("Duration");
        StatsModifier = e.Elements("StatsModifier").Select(i => new GemstoneBoost(i)).ToArray();
        MinStatEfficiency = e.GetValue<float>("MinStatEfficiency");
        BoostDuration = e.GetValue<short>("BoostDuration");

        _initialized = true;
    }
}