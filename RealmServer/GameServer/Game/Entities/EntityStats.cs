#region

using Common;
using Common.Utilities;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;

#endregion

namespace GameServer.Game.Entities;

public class EntityStats
{
    public readonly HashSet<Player> StatChangedListeners = [];
    public static readonly int StatCount = (int)StatType.StatTypeCount;
    
    private readonly Entity _entity;
    private readonly StatValue[] _stats;
    private readonly StatValue[] _publicStats;
    private readonly StatValue[] _prevStats;
    private ObjectData _objData;
    private ObjectDropData _objDropData;

    public EntityStats(Entity entity)
    {
        _entity = entity;
        _stats = ArrayPool<StatValue>.Shared.Rent(StatCount);
        _publicStats = ArrayPool<StatValue>.Shared.Rent(StatCount);
        _prevStats = ArrayPool<StatValue>.Shared.Rent(StatCount);

        _stats.AsSpan(0, StatCount).Clear();
        _publicStats.AsSpan(0, StatCount).Clear();
        _prevStats.AsSpan(0, StatCount).Clear();
    }

    public bool Initializing { get; set; }

    public int GetInt(StatType s)
    {
        return _stats[(int)s].IntVal;
    }

    public float GetFloat(StatType s)
    {
        return _stats[(int)s].FloatVal;
    }

    public string GetString(StatType s)
    {
        return _stats[(int)s].StrVal;
    }

    public void Set(StatType statType, int value, bool isPrivate = false)
        => SetInternal(statType, StatValue.FromInt(value), isPrivate);

    public void Set(StatType statType, float value, bool isPrivate = false)
        => SetInternal(statType, StatValue.FromFloat(value), isPrivate);

    public void Set(StatType statType, string value, bool isPrivate = false)
        => SetInternal(statType, StatValue.FromString(value), isPrivate);
    
    // Initializing indicates that the stat is being set in LoadStats method, if it's true and the stat has already been set to a value, it will keep the original value
    private void SetInternal(StatType statType, StatValue sv, bool isPrivate)
    {
        var id = (int)statType;

        if (Initializing && _stats[id].HasValue)
            return;

        _stats[id] = sv;

        if (!isPrivate)
            _publicStats[id] = sv;

        if (!_entity.Dead && _entity.IsPlayer)
        {
            var plr = (Player)_entity;
            if (isPrivate)
                plr.HandleEntityStatChanged(plr, statType, sv);
        }
    }

    public void Update()
    {
        foreach (var p in StatChangedListeners)
            p.HandleEntityStatChanged(_entity, StatType.None, new StatValue());
        
        for (var i = 0; i < StatCount; i++)
        {
            var type = (StatType)i;
            var newValue = _publicStats[i];
            if (newValue.HasValue && !newValue.Equals(_prevStats[i]))
            {
                _prevStats[i] = newValue;
                foreach (var plr in StatChangedListeners)
                    plr.HandleEntityStatChanged(_entity, type, newValue);
            }
        }
    }

    public ObjectData GetObjectData(int objId)
    {
        if (_entity.IsConnected)
            (_entity as ConnectedObject).FindConnection();

        _objData.ObjectType = _entity.Desc.ObjectType;
        _objData.Status.ObjectId = _entity.Id;
        _objData.Status.Pos = _entity.Position;
        _objData.Status.Stats = objId == _entity.Id ? _stats : _publicStats;

        return _objData;
    }

    public ObjectDropData GetObjectDropData()
    {
        _objDropData.ObjectId = _entity.Id;
        if (GetInt(StatType.HP) <= 0)
            _objDropData.Explode = true;

        return _objDropData;
    }

    public void Clear()
    {
        _stats.AsSpan(0, StatCount).Clear();
        _publicStats.AsSpan(0, StatCount).Clear();
        _prevStats.AsSpan(0, StatCount).Clear();
        StatChangedListeners.Clear();
    }
}