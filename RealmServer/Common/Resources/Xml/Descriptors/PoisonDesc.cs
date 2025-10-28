using Common.Utilities;
using System;
using System.Linq;
using System.Xml.Linq;

namespace Common.Resources.Xml.Descriptors;

public class PoisonDesc : ItemData
{
    public override Type FieldsEnum => typeof(PoisonDescField);

    public short MpCost
    {
        get => GetValue<short>(0);
        set => SetValue(0, value);
    }

    public ConditionEffectDesc[] Effects
    {
        get => GetValue<ConditionEffectDesc[]>(1);
        set => SetValue(6, value);
    }

    public short Damage
    {
        get => GetValue<short>(2);
        set => SetValue(2, value);
    }

    public short TickSpeed
    {
        get => GetValue<short>(3);
        set => SetValue(3, value);
    }

    public int Duration
    {
        get => GetValue<int>(4);
        set => SetValue(4, value);
    }

    public float ThrowRange
    {
        get => GetValue<float>(5);
        set => SetValue(5, value);
    }

    public short ThrowTravelTime
    {
        get => GetValue<short>(6);
        set => SetValue(6, value);
    }

    public float PoisonRange
    {
        get => GetValue<float>(7);
        set => SetValue(7, value);
    }

    public float SpreadRange
    {
        get => GetValue<float>(8);
        set => SetValue(8, value);
    }

    public short SpreadTargetsNum
    {
        get => GetValue<short>(9);
        set => SetValue(9, value);
    }

    public float SpreadEfficiency
    {
        get => GetValue<float>(10);
        set => SetValue(10, value);
    }

    public short ImpactDamage
    {
        get => GetValue<short>(11);
        set => SetValue(11, value);
    }

    public PoisonDesc(XElement e, ItemData parent = null, byte parentField = 0)
    {
        SetParent(parent, parentField);
        if (e == null) // Null when instance by itemdata import
        {
            _initialized = true;
            return;
        }

        MpCost = e.GetValue<short>("MpCost", 0);
        Effects = e.Elements("ConditionEffect").Select(i => new ConditionEffectDesc(i)).ToArray();
        Damage = e.GetValue<short>("Damage", 0);
        TickSpeed = e.GetValue<short>("TickSpeed", 0);
        Duration = e.GetValue<int>("Duration", 0);
        ThrowRange = e.GetValue<float>("ThrowRange", 0);
        ThrowTravelTime = e.GetValue<short>("ThrowTravelTime", 0);
        PoisonRange = e.GetValue<float>("PoisonRange", 0);
        SpreadRange = e.GetValue<float>("SpreadRange", 0);
        SpreadTargetsNum = e.GetValue<short>("SpreadTargetsNum", 0);
        SpreadEfficiency = e.GetValue<float>("SpreadEfficiency", 0);
        ImpactDamage = e.GetValue<short>("ImpactDamage", 0);

        _initialized = true;
    }
}