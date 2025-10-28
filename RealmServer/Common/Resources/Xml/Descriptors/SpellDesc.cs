using Common.Utilities;
using System;
using System.Xml.Linq;

namespace Common.Resources.Xml.Descriptors;

public class SpellDesc : ItemData
{
    public override Type FieldsEnum => typeof(SpellDescField);
    
    public short MpCost
    {
        get => GetValue<short>(0);
        set => SetValue(0, value);
    }

    public byte NumProjectiles
    {
        get => GetValue<byte>(1);
        set => SetValue(1, value);
    }

    public byte ProjectileId
    {
        get => GetValue<byte>(2);
        set => SetValue(2, value);
    }
    
    public SpellDesc(XElement e, ItemData parent = null, byte parentField = 0)
    {
        SetParent(parent, parentField);
        if (e == null) // Null when instance by itemdata import
        {
            _initialized = true;
            return;
        }

        MpCost = e.GetValue<short>("MpCost", 0);
        NumProjectiles = e.GetValue<byte>("NumProjectiles", 20);
        ProjectileId = e.GetValue<byte>("ProjectileId", 0);

        _initialized = true;
    }
}