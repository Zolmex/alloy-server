using System;
using System.Xml.Linq;
using Common.Utilities;

namespace Common.Resources.Xml.Descriptors;

public class QuiverDesc : ItemData {
    public QuiverDesc(XElement e, ItemData parent = null, byte parentField = 0) {
        SetParent(parent, parentField);
        if (e == null) // Null when instance by itemdata import
        {
            _initialized = true;
            return;
        }

        MpCost = e.GetValue<short>("MpCost");
        ProjectileId = e.GetValue<byte>("ProjectileId");
        ArrowChance = e.GetValue<byte>("ArrowChance", 100) / 100.0f;
        MaxArrows = e.GetValue<byte>("MaxArrows", 10);
        MpPerArrow = e.GetValue<short>("UseMpArrows");
        UseMpArrows = MpPerArrow > 0;
        MpProjectileId = e.GetValue<byte>("MpProjectileId");

        _initialized = true;
    }

    public override Type FieldsEnum => typeof(QuiverDescField);

    public short MpCost {
        get => GetValue<short>(0);
        set => SetValue(0, value);
    }

    public byte ProjectileId {
        get => GetValue<byte>(1);
        set => SetValue(1, value);
    }

    public float ArrowChance {
        get => GetValue<float>(2);
        set => SetValue(2, value);
    }

    public byte MaxArrows {
        get => GetValue<byte>(3);
        set => SetValue(3, value);
    }

    public bool UseMpArrows {
        get => GetValue<bool>(4);
        set => SetValue(4, value);
    }

    public short MpPerArrow {
        get => GetValue<short>(5);
        set => SetValue(5, value);
    }

    public byte MpProjectileId {
        get => GetValue<byte>(6);
        set => SetValue(6, value);
    }
}