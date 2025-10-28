using Common.ProjectilePaths;
using Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Common.Resources.Xml.Descriptors;

public class ProjectileDesc : ItemData
{
    public override Type FieldsEnum => typeof(ProjectileField);

    public readonly XElement Root;
    public ushort ContainerType { get; private set; }

    public byte BulletId
    {
        get => GetValue<byte>(0);
        set => SetValue(0, value);
    }

    public string ObjectId
    {
        get => GetValue<string>(1);
        set => SetValue(1, value);
    }

    public int LifetimeMS
    {
        get => GetValue<int>(2);
        set => SetValue(2, value);
    }

    public float Speed
    {
        get => GetValue<float>(3);
        set => SetValue(3, value);
    }

    public int Damage
    {
        get => GetValue<int>(4);
        set => SetValue(4, value);
    }

    public int MinDamage
    {
        get => GetValue<int>(5);
        set => SetValue(5, value);
    }

    public int MaxDamage
    {
        get => GetValue<int>(6);
        set => SetValue(6, value);
    }

    public ConditionEffectDesc[] Effects
    {
        get => GetValue<ConditionEffectDesc[]>(7);
        set => SetValue(7, value);
    }

    public bool MultiHit
    {
        get => GetValue<bool>(8);
        set => SetValue(8, value);
    }

    public bool PassesCover
    {
        get => GetValue<bool>(9);
        set => SetValue(9, value);
    }

    public bool ArmorPiercing
    {
        get => GetValue<bool>(10);
        set => SetValue(10, value);
    }

    public bool Wavy
    {
        get => GetValue<bool>(11);
        set => SetValue(11, value);
    }

    public bool Parametric
    {
        get => GetValue<bool>(12);
        set => SetValue(12, value);
    }

    public bool Boomerang
    {
        get => GetValue<bool>(13);
        set => SetValue(13, value);
    }

    public float Amplitude
    {
        get => GetValue<float>(14);
        set => SetValue(14, value);
    }

    public float Frequency
    {
        get => GetValue<float>(15);
        set => SetValue(15, value);
    }

    public float Magnitude
    {
        get => GetValue<float>(16);
        set => SetValue(16, value);
    }

    public int Size
    {
        get => GetValue<int>(17);
        set => SetValue(17, value);
    }

    public LevelIncreaseDesc[] LevelIncreases
    {
        get => GetValue<LevelIncreaseDesc[]>(18);
        set => SetValue(18, value);
    }

    public ProjectilePath Path { get; set; }

    public ProjectileDesc(XElement e, ItemData parent = null, byte parentField = 0)
    {
        SetParent(parent, parentField);
        if (e == null) // Null when instance by itemdata import
        {
            _initialized = true;
            return;
        }

        Root = e;
        BulletId = (byte)e.GetAttribute<int>("id");
        ObjectId = e.GetValue<string>("ObjectId");
        LifetimeMS = (int)e.GetValue<float>("LifetimeMS");
        Speed = e.GetValue<float>("Speed") / 10;
        Damage = e.GetValue<int>("Damage");
        MinDamage = e.GetValue<int>("MinDamage", Damage);
        MaxDamage = e.GetValue<int>("MaxDamage", Damage);

        var effects = new List<ConditionEffectDesc>();
        foreach (var k in e.Elements("ConditionEffect"))
            effects.Add(new ConditionEffectDesc(k));
        Effects = effects.ToArray();

        MultiHit = e.HasElement("MultiHit");
        PassesCover = e.HasElement("PassesCover");
        ArmorPiercing = e.HasElement("ArmorPiercing");
        Size = e.GetValue<int>("Size", 0);
        Wavy = e.HasElement("Wavy");
        Parametric = e.HasElement("Parametric");
        Boomerang = e.HasElement("Boomerang");

        Amplitude = e.GetValue<float>("Amplitude", 0);
        Frequency = e.GetValue<float>("Frequency", 1);
        Magnitude = e.GetValue<float>("Magnitude", 3);

        LevelIncreases = e.Elements("LevelIncrease")
            .Select(i => new LevelIncreaseDesc(i, this, (byte)ProjectileField.LevelIncreases))
            .ToArray();

        if (e.Element("Path") != null)
        {
            Path = new ProjectilePath();
            foreach (var elem in e.Elements("Path"))
                Path.RegisterSegment(ProjectilePathSegment.ParsePath(elem));
        }
        else
            Path = ProjectilePathSegment.ParsePath(this).ToPath();

        _initialized = true;
    }

    public void SetContainer(ushort containerType)
    {
        ContainerType = containerType;
    }
}