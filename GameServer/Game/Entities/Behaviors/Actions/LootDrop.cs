using Common;
using Common.Game;
using Common.Resources.Xml.Descriptors;
using GameServer.Game.Entities.Behaviors.Loot;
using GameServer.Game.Entities.Events;
using GameServer.Game.Network.Messaging.Outgoing;

namespace GameServer.Game.Entities.Behaviors.Actions;

public record LootDrop : BehaviorScript {

    private readonly ILoot[] _loots;
    
    public LootDrop(params ILoot[] loots) {
        _loots = loots;
    }
    
    public override void Start(ref EntityView host) {
        host.Events.OnDeath.Subscribe(HandleLoot);
    }

    public override void End(ref EntityView host, ref RealmTime time) {
        host.Events.OnDeath.Unsubscribe(HandleLoot);
    }

    private void HandleLoot(ref DeathEvent evt) {
        var entityView = new EntityView(evt.World, evt.HostId);
        var drops = new List<Item>();
        foreach (ref var record in entityView.Combat.DamageRecords)
            foreach (var loot in _loots)
                loot.Populate(ref entityView, ref drops, ref record);
    }
}