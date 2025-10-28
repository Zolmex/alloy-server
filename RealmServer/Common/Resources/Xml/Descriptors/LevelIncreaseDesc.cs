using Common.Utilities;
using System;
using System.Xml.Linq;

namespace Common.Resources.Xml.Descriptors;

public class LevelIncreaseDesc : ItemData
{
    public override Type FieldsEnum => typeof(LevelIncreaseField);

    public string Field
    {
        get => GetValue<string>(0);
        set => SetValue(0, value);
    }

    public int Rate
    {
        get => GetValue<int>(1);
        set => SetValue(1, value);
    } // Will determine how many levels does it need to do the increase

    public float Amount
    {
        get => GetValue<float>(2);
        set => SetValue(2, value);
    }

    public LevelIncreaseDesc(XElement e, ItemData parent = null, byte parentField = 0)
    {
        SetParent(parent, parentField);
        if (e == null) // Null when instance by itemdata import
        {
            _initialized = true;
            return;
        }

        Field = e.Value;
        Rate = e.GetAttribute<int>("rate");
        Amount = e.GetAttribute<float>("amount");

        _initialized = true;
    }
}