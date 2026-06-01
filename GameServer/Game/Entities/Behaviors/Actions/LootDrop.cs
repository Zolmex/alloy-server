using System.Collections;
using Common;
using Common.Game;
using Common.Resources.Xml.Descriptors;
using Common.Utilities;
using GameServer.Game.Entities.Behaviors.Loot;
using GameServer.Game.Entities.Events;
using GameServer.Game.Entities.Extensions;
using GameServer.Game.Network.Messaging.Outgoing;

namespace GameServer.Game.Entities.Behaviors.Actions;

public record LootDrop : BehaviorScript {
    private readonly ILoot[] _loots;
    private readonly bool _public;

    public LootDrop(bool publicLoot, params ILoot[] loots) {
        _loots = loots;
        _public = publicLoot;
    }

    public override void Start(ref EntityView host) {
        host.Events.OnDeath.Subscribe(HandleLoot);
    }

    public override void End(ref EntityView host, ref RealmTime time) {
        host.Events.OnDeath.Unsubscribe(HandleLoot);
    }

    private void HandleLoot(ref DeathEvent evt) {
        var entityView = new EntityView(evt.World, evt.HostId);
        var playerDrops = new Dictionary<int, Queue<Item>>();
        foreach (ref var record in entityView.Combat.DamageRecords) {
            var key = _public ? -1 : record.FromAccId;
            if (!playerDrops.TryGetValue(key, out var drops))
                drops = playerDrops[key] = new Queue<Item>();
            
            foreach (var loot in _loots)
                loot.Populate(ref entityView, ref drops, ref record);
        }

        if (playerDrops.Count < 0)
            return;

        var host = new EntityView(evt.World, evt.HostId);
        foreach (var (accId, drops) in playerDrops)
            while (drops.Count > 0) {
                var bagType = 0;
                var items = new Item[8];
                for (var i = 0; i < 8; i++) {
                    if (!drops.TryDequeue(out var item))
                        break;

                    if (item.BagType > bagType)
                        bagType = item.BagType;

                    items[i] = item;
                }

                var entity = new Entity(ItemUtils.GetBagIdFromType((BagType)bagType));
                ref var en = ref host.World.EnterWorld(ref entity);
                ref var enInv = ref host.World.EntityInventories.Get(en.Id);
                enInv.SetItems(items);
                if (!_public)
                    enInv.OwnerAccIds.Add(accId);

                var childX = host.Stats.Pos.X + (float)Random.Shared.NextDouble() * 1.5f;
                var childY = host.Stats.Pos.Y + (float)Random.Shared.NextDouble() * 1.5f;
                en.Move(host.World, childX, childY);
            }
    }
}