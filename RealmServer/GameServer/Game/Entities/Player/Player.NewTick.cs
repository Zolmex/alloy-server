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

    private void SendNewTick()
    {
        User.SendPacket(new NewTick(_entityStatUpdates));
    }

    public void HandleEntityStatChanged(Entity en, StatType type, StatValue value)
    {
        using (TimedLock.Lock(_entityStatUpdates))
        {
            if (!_entityStatUpdates.TryGetValue(en.Id, out var status))
                status = new ObjectStatusData { ObjectId = en.Id, Pos = en.Position, Stats = ArrayPool<StatValue>.Shared.Rent((int)StatType.StatTypeCount), Update = true };

            status.SetPos(en.Position);
            status.SetStat(type, value);
            _entityStatUpdates[en.Id] = status;
        }
    }
}