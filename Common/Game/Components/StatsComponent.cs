using System;
using System.Buffers;
using Common.Structs;
using Common.Utilities;

namespace Common.Game.Components;

public struct StatsComponent : IIdentifiable, IDisposable {
    public const int STAT_COUNT = (int)StatType.StatTypeCount;

    public int Id { get; set; }
    
    public readonly StatValue[] Stats;
    public BitMask256 PublicMask;
    public BitMask256 PrivateMask;

    public StatsComponent() {
        Stats = ArrayPool<StatValue>.Shared.Rent(STAT_COUNT);
        Stats.AsSpan(0, STAT_COUNT).Clear();
    }
    
    public int GetInt(StatType s) {
        return Stats[(int)s].IntVal;
    }

    public float GetFloat(StatType s) {
        return Stats[(int)s].FloatVal;
    }

    public string GetString(StatType s) {
        return Stats[(int)s].StrVal;
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

        Stats[id] = sv;

        if (!isPrivate)
            PublicMask.Set(id);
        PrivateMask.Set(id);
    }

    public void ClearMasks() {
        PublicMask.Clear();
        PrivateMask.Clear();
    }

    public void Dispose() {
        ArrayPool<StatValue>.Shared.Return(Stats);
    }
}