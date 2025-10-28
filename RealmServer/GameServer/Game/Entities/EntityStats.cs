#region

using Common;
using Common.Utilities;
using System;
using System.Buffers;
using System.Collections.Generic;

#endregion

namespace GameServer.Game.Entities
{
    public class EntityStats
    {
        public readonly List<Player> StatChangedListeners = [];

        public bool Initializing { get; set; }

        private readonly Entity _entity;
        private readonly object[] _stats = ArrayPool<object>.Shared.Rent((int)StatType.StatTypeCount);
        private readonly object[] _publicStats = ArrayPool<object>.Shared.Rent((int)StatType.StatTypeCount);
        private readonly object[] _prevStats = ArrayPool<object>.Shared.Rent((int)StatType.StatTypeCount);
        private ObjectDropData _objDropData;
        private ObjectData _objData;

        public EntityStats(Entity entity)
        {
            _entity = entity;
        }

        public T Get<T>(StatType statType)
        {
            var ret = _stats[(int)statType];
            if (ret == null)
                return default;

            if (StatData.IsStringStat(statType))
                return (T)ret;

            if (StatData.IsFloatStat(statType) &&
                ret is int) // Backwards compatibility if stat changed from int to float
                return (T)(object)Convert.ToSingle(ret);

            if (!StatData.IsFloatStat(statType) && ret is float) // Same case as previous but from float to int
                return (T)(object)Convert.ToInt32(ret);

            return (T)ret;
        }

        // Initializing indicates that the stat is being set in LoadStats method, if it's true and the stat has already been set to a value, it will keep the original value
        public void Set(StatType statType, object value, bool isPrivate = false)
        {
            var realValue = value;

            if (StatData.IsFloatStat(statType) &&
                value is int) // Backwards compatibility if stat changed from int to float
                realValue = Convert.ToSingle(value);

            if (!StatData.IsFloatStat(statType) && value is float) // Same case as previous but from float to int
                realValue = Convert.ToInt32(value);

            var statId = (int)statType;
            if (Initializing &&
                _stats[statId] != null) // Don't load the value if it's already been set
                return;

            _stats[statId] = realValue;

            if (!isPrivate)
                _publicStats[statId] = realValue;

            if (!_entity.Dead && _entity.IsPlayer)
            {
                var plr = (Player)_entity;
                if (isPrivate)
                    plr.HandleEntityStatChanged(plr, statType, realValue);
                plr.StatChanged(statType);
            }
        }

        public void Update()
        {
            var sentUpdate = false;
            for (var i = 0; i < _publicStats.Length; i++)
            {
                var newStatValue = _publicStats[i];
                if (newStatValue != null && !newStatValue.Equals(_prevStats[i]))
                {
                    sentUpdate = true;
                    _prevStats[i] = newStatValue;

                    using (TimedLock.Lock(StatChangedListeners))
                        foreach (var p in StatChangedListeners)
                            p.HandleEntityStatChanged(_entity, (StatType)i, newStatValue);
                }
            }

            if (!sentUpdate) // Make sure to send update
                using (TimedLock.Lock(StatChangedListeners))
                    foreach (var p in StatChangedListeners)
                        p.HandleEntityStatChanged(_entity, StatType.None, 0);
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
            if (Get<int>(StatType.HP) <= 0)
                _objDropData.Explode = true;

            return _objDropData;
        }

        public void Clear()
        {
            _stats.Clear();
            _publicStats.Clear();
            _prevStats.Clear();
            StatChangedListeners.Clear();
        }
    }
}