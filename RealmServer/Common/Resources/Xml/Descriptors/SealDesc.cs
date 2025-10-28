using Common.Utilities;
using System;
using System.Linq;
using System.Xml.Linq;

namespace Common.Resources.Xml.Descriptors;

public class SealDesc : ItemData
{
    public override Type FieldsEnum => typeof(SealDescField);

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

    public short MaxStack
    {
        get => GetValue<short>(2);
        set => SetValue(2, value);
    }

    public float Radius
    {
        get => GetValue<float>(3);
        set => SetValue(3, value);
    }
    
    public short ShieldAmount
    {
        get => GetValue<short>(4);
        set => SetValue(4, value);
    }
    
    public short MaxAllies
    {
        get => GetValue<short>(5);
        set => SetValue(5, value);
    }
    
    public float EfficiencyReductionPerPlayer
    {
        get => GetValue<float>(6);
        set => SetValue(6, value);
    }
    
    public short BannerSpeed
    {
        get => GetValue<short>(7);
        set => SetValue(7, value);
    }
    
    public GemstoneBoost[] StatsModifier
    {
        get => GetValue<GemstoneBoost[]>(8);
        set => SetValue(8, value);
    }
    
    public int BoostDuration
    {
        get => GetValue<int>(9);
        set => SetValue(9, value);
    }

    public short MaxBanners
    {
        get => GetValue<short>(10);
        set => SetValue(10, value);
    }

    public SealDesc(XElement e, ItemData parent = null, byte parentField = 0)
    {
        SetParent(parent, parentField);
        if (e == null) // Null when instance by itemdata import
        {
            _initialized = true;
            return;
        }

        MpCost = e.GetValue<short>("MpCost");
        Duration = e.GetValue<int>("Duration");
        MaxStack = e.GetValue<short>("MaxStack");
        BannerSpeed = e.GetValue<short>("BannerSpeed");
        Radius = e.GetValue<float>("Radius");
        ShieldAmount = e.GetValue<short>("ShieldAmount");
        MaxAllies = e.GetValue<short>("MaxAllies");
        EfficiencyReductionPerPlayer = e.GetValue<float>("EfficiencyReductionPerPlayer");
        StatsModifier = e.Elements("StatsModifier").Select(i => new GemstoneBoost(i)).ToArray();
        BoostDuration = e.GetValue<int>("BoostDuration");
        MaxBanners = e.GetValue<short>("MaxBanners");

        _initialized = true;
    }
}