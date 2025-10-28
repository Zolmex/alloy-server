using Common.Utilities;
using System;
using System.Linq;
using System.Xml.Linq;

namespace Common.Resources.Xml.Descriptors;

public class HelmDesc : ItemData
{
    public override Type FieldsEnum => typeof(HelmDescField);

    public short MpCost
    {
        get => GetValue<short>(0);
        set => SetValue(0, value);
    }

    public int Duration
    {
        get => GetValue<int>(1);
        set => SetValue(1, value);
    }

    public short StackGain
    {
        get => GetValue<short>(2);
        set => SetValue(2, value);
    }

    public short StackLost
    {
        get => GetValue<short>(3);
        set => SetValue(3, value);
    }
    
    public GemstoneBoost[] StatsModifier
    {
        get => GetValue<GemstoneBoost[]>(4);
        set => SetValue(4, value);
    }
    
    public ConditionEffectIndex[] HoldEffects
    {
        get => GetValue<ConditionEffectIndex[]>(5);
        set => SetValue(5, value);
    }
    
    public short MpDrain
    {
        get => GetValue<short>(6);
        set => SetValue(6, value);
    }

    public HelmDesc(XElement e, ItemData parent = null, byte parentField = 0)
    {
        SetParent(parent, parentField);
        if (e == null) // Null when instance by itemdata import
        {
            _initialized = true;
            return;
        }

        MpCost = e.GetValue<short>("MpCost");
        Duration = e.GetValue<int>("Duration");
        StackGain = e.GetValue<short>("StackGain");
        StackLost = e.GetValue<short>("StackLost");
        StatsModifier = e.Elements("StatsModifier").Select(i => new GemstoneBoost(i)).ToArray();
        HoldEffects = e.Elements("HoldEffect").Select(i => (ConditionEffectIndex)Enum.Parse(typeof(ConditionEffectIndex), i.Value.Replace(" ", ""))).ToArray();
        MpDrain = e.GetValue<short>("MpDrain");

        _initialized = true;
    }
}