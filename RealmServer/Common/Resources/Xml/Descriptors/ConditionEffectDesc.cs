using Common.Utilities;
using System;
using System.Xml.Linq;

namespace Common.Resources.Xml.Descriptors;

public class ConditionEffectDesc : ItemData
{
    public override Type FieldsEnum => typeof(ConditionEffectField);

    public ConditionEffectIndex Effect
    {
        get => GetValue<ConditionEffectIndex>(0);
        set => SetValue(0, value);
    }

    public int DurationMS
    {
        get => GetValue<int>(1);
        set => SetValue(1, value);
    }

    public ConditionEffectDesc(ConditionEffectIndex effect, int durationMs)
    {
        Effect = effect;
        DurationMS = durationMs;

        _initialized = true;
    }

    public ConditionEffectDesc(XElement e, ItemData parent = null, byte parentField = 0)
    {
        SetParent(parent, parentField);
        if (e == null) // Null when instance by itemdata import
        {
            _initialized = true;
            return;
        }

        Effect = (ConditionEffectIndex)Enum.Parse(typeof(ConditionEffectIndex), e.Value.Replace(" ", ""));
        DurationMS = (int)(e.GetAttribute<float>("duration") * 1000);

        _initialized = true;
    }
}