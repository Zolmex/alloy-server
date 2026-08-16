using System.Buffers;
using System.Runtime.CompilerServices;
using Common;
using Common.Resources.Xml.Descriptors;
using Common.Structs;
using Common.Utilities;

namespace GameServer.Game.Entities.Components;

[InlineArray(Stats.STAT_COUNT)]
public struct StatValueBuffer { private StatValue _; }

[InlineArray(Stats.STAT_COUNT)]
public struct StatDataBuffer { private StatData _; }

public struct Stats {
    public const int STAT_COUNT = (int)StatType.StatTypeCount;

    public StatValueBuffer All;
    public StatDataBuffer Updated;
    public BitMask256 PublicMask;
    public BitMask256 PrivateMask;
    public int StatUpdateCount;
    
    private BitMask256 _statUpdatesMask;
    
    public Stats(ObjectDesc desc) {
        All = new StatValueBuffer();
        Updated = new StatDataBuffer();
        
        Set(StatType.Name, desc.ObjectId);
        Set(StatType.HP, desc.MaxHP);
        Set(StatType.MaxHP, desc.MaxHP);
    }
    
    public int GetInt(StatType s) {
        return All[(int)s].IntVal;
    }

    public float GetFloat(StatType s) {
        return All[(int)s].FloatVal;
    }

    public string GetString(StatType s) {
        return All[(int)s].StrVal;
    }

    public void Set(StatType statType, int value, bool isPrivate = false) {
        SetInternal(statType, StatValue.FromInt(value), isPrivate);
    }

    public void Set(StatType statType, float value, bool isPrivate = false) {
        SetInternal(statType, StatValue.FromFloat(value), isPrivate);
    }

    public void Set(StatType statType, string value, bool isPrivate = false) {
        SetInternal(statType, StatValue.FromString(value), isPrivate);
    }
    
    private void SetInternal(StatType statType, StatValue sv, bool isPrivate) {
        var id = (int)statType;
        if (sv == All[id])
            return;

        All[id] = sv;
        _statUpdatesMask.Set(id);

        if (!isPrivate)
            PublicMask.Set(id);
        PrivateMask.Set(id);
    }

    public void Update() {
        StatUpdateCount = 0;
        if (!_statUpdatesMask.IsEmpty)
            for (var i = 0; i < STAT_COUNT; i++) {
                if (_statUpdatesMask.IsSet(i))
                    Updated[StatUpdateCount++] = new StatData((StatType)i, All[i]);
            }

        _statUpdatesMask.Clear();
    }
}