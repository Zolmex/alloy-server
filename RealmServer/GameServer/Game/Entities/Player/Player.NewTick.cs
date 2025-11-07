#region

using Common;
using Common.Utilities;
using GameServer.Game.Network.Messaging.Outgoing;
using System.Buffers;
using System.Collections.Generic;

#endregion

namespace GameServer.Game.Entities;

public partial class Player
{
    private readonly Dictionary<int, ObjectStatusData> _entityStatUpdates = [];

    public bool TickStatChanges = true;

    private void SendNewTick()
    {
        User.SendPacket(new NewTick(_entityStatUpdates));
    }

    public void HandleEntityStatChanged(Entity en, StatType type, object value)
    {
        using (TimedLock.Lock(_entityStatUpdates))
        {
            if (!_entityStatUpdates.TryGetValue(en.Id, out var status))
                status = new ObjectStatusData() { ObjectId = en.Id, Pos = en.Position, Stats = ArrayPool<object>.Shared.Rent((int)StatType.StatTypeCount), Update = true };

            status.SetPos(en.Position);
            status.SetStat(type, value);
            _entityStatUpdates[en.Id] = status;
        }
    }

    public void StatChanged(StatType type)
    {
        if (type != StatType.None && TickStatChanges)
            OnStatChanged?.Invoke(type);
    }
}