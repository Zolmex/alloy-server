using System.Buffers;
using System.Runtime.CompilerServices;
using Common;
using Common.Resources.Xml.Descriptors;
using Common.Structs;
using Common.Utilities;

namespace GameServer.Game.Entities.Components;

public struct Stats {
    public StatValueBuffer Values;
    public StatDataBuffer StatUpdates;
    public BitMask256 PublicMask;
    public BitMask256 PrivateMask;
    public int StatUpdateCount;
    
    private BitMask256 _statUpdatesMask;
    
    public Stats(ObjectDesc desc) {
        Values = new StatValueBuffer();
        StatUpdates = new StatDataBuffer();
        
        Set(StatType.Name, desc.ObjectId);
        Set(StatType.HP, desc.MaxHP);
        Set(StatType.MaxHP, desc.MaxHP);
    }
    
    public int GetInt(StatType s) {
        return Values[(int)s].IntVal;
    }

    public float GetFloat(StatType s) {
        return Values[(int)s].FloatVal;
    }

    public string GetString(StatType s) {
        return Values[(int)s].StrVal;
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
        if (sv == Values[id])
            return;

        Values[id] = sv;
        _statUpdatesMask.Set(id);

        if (!isPrivate)
            PublicMask.Set(id);
        PrivateMask.Set(id);
    }

    public void Update() {
        StatUpdateCount = 0;
        if (!_statUpdatesMask.IsEmpty)
            for (var i = 0; i < StatData.STAT_COUNT; i++) {
                if (_statUpdatesMask.IsSet(i))
                    StatUpdates[StatUpdateCount++] = new StatData((StatType)i, Values[i]);
            }

        _statUpdatesMask.Clear();
    }
}