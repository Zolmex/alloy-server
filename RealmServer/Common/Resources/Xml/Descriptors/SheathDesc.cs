using Common.Utilities;
using System;
using System.Linq;
using System.Xml.Linq;

namespace Common.Resources.Xml.Descriptors;

public class SheathDesc : ItemData
{
    public override Type FieldsEnum => typeof(SheathDescField);

    public int Capacity
    {
        get => GetValue<int>(0);
        set => SetValue(0, value);
    }

    public int SlashDamage
    {
        get => GetValue<int>(1);
        set => SetValue(1, value);
    }

    public float Efficiency
    {
        get => GetValue<float>(2);
        set => SetValue(2, value);
    }

    public int ManaPerSlash
    {
        get => GetValue<int>(3);
        set => SetValue(3, value);
    }
    
    public int SlashCooldownMS
    {
        get => GetValue<int>(4);
        set => SetValue(4, value);
    }
    
    public float Radius
    {
        get => GetValue<float>(5);
        set => SetValue(5, value);
    }
    
    public ConditionEffectDesc[] Effects
    {
        get => GetValue<ConditionEffectDesc[]>(6);
        set => SetValue(6, value);
    }
    
    public int StanceDuration
    {
        get => GetValue<int>(7);
        set => SetValue(7, value);
    }

    public SheathDesc(XElement e, ItemData parent = null, byte parentField = 0)
    {
        SetParent(parent, parentField);
        if (e == null) // Null when instance by itemdata import
        {
            _initialized = true;
            return;
        }

        Capacity = e.GetValue<int>("Capacity");
        SlashDamage = e.GetValue<int>("SlashDamage");
        Efficiency = e.GetValue<float>("Efficiency", 1);
        ManaPerSlash = e.GetValue<int>("ManaPerSlash");
        SlashCooldownMS = e.GetValue<int>("SlashCooldownMS", 200);
        Radius = e.GetValue<float>("Radius");
        Effects = e.Elements("ConditionEffect").Select(i => new ConditionEffectDesc(i)).ToArray();
        StanceDuration = e.GetValue<int>("StanceDuration", 1000);

        _initialized = true;
    }
}