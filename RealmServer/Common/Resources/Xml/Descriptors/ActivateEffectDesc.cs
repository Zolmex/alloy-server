using Common.Utilities;
using System;
using System.Linq;
using System.Xml.Linq;

namespace Common.Resources.Xml.Descriptors;

public class ActivateEffectDesc : ItemData
{
    public override Type FieldsEnum => typeof(ActivateEffectField);

    public ActivateEffectIndex AEIndex
    {
        get => GetValue<ActivateEffectIndex>(0);
        set => SetValue(0, value);
    }

    public ConditionEffectDesc EffectDesc
    {
        get => GetValue<ConditionEffectDesc>(1);
        set => SetValue(1, value);
    }

    public int DurationMS
    {
        get => GetValue<int>(2);
        set => SetValue(2, value);
    }

    public float Range
    {
        get => GetValue<float>(3);
        set => SetValue(3, value);
    }

    public int Amount
    {
        get => GetValue<int>(4);
        set => SetValue(4, value);
    }

    public int TotalDamage
    {
        get => GetValue<int>(5);
        set => SetValue(5, value);
    }

    public float Radius
    {
        get => GetValue<float>(6);
        set => SetValue(6, value);
    }

    public uint Color
    {
        get => GetValue<uint>(7);
        set => SetValue(7, value);
    }

    public int MaxTargets
    {
        get => GetValue<int>(8);
        set => SetValue(8, value);
    }

    public string ObjectId
    {
        get => GetValue<string>(9);
        set => SetValue(9, value);
    }

    public LevelIncreaseDesc[] LevelIncreases
    {
        get => GetValue<LevelIncreaseDesc[]>(10);
        set => SetValue(10, value);
    }

    public byte NumShots
    {
        get => GetValue<byte>(11);
        set => SetValue(11, value);
    }

    public ActivateEffectDesc(XElement e, ItemData parent = null, byte parentField = 0)
    {
        SetParent(parent, parentField);
        if (e == null)
        {
            _initialized = true;
            return;
        }

        AEIndex = (ActivateEffectIndex)Enum.Parse(typeof(ActivateEffectIndex), e.Value.Replace(" ", ""));
        DurationMS = (int)(e.GetAttribute<float>("duration", 0) * 1000);
        Range = e.GetAttribute<float>("range");
        Amount = e.GetAttribute<int>("amount");
        TotalDamage = e.GetAttribute<int>("totalDamage");
        Radius = e.GetAttribute<float>("radius");
        MaxTargets = e.GetAttribute<int>("maxTargets");

        EffectDesc = new ConditionEffectDesc(
            Utils.GetEnumValue<ConditionEffectIndex>(e.GetAttribute<string>("effect")),
            (int)(e.GetAttribute<float>("condDuration") * 1000));

        Color = e.GetAttribute<uint>("color");
        ObjectId = e.GetAttribute<string>("id");

        NumShots = e.GetAttribute<byte>("numShots", 20);

        LevelIncreases = e.Elements("LevelIncrease")
            .Select(i => new LevelIncreaseDesc(i, this, (byte)ActivateEffectField.LevelIncreases))
            .ToArray();

        _initialized = true;
    }
}