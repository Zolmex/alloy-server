using Common.Projectiles.ProjectilePaths;
using GameServer.Game.Entities.Behaviors.Actions;
using GameServer.Game.Entities.Behaviors.Transitions;
using StackExchange.Redis;

namespace GameServer.Game.Entities.Behaviors.Library;

public partial class BehaviorLib {
    [CharacterBehavior("Pirate")]
    public static State Pirate =>
        new(
            // new LootDrop(
            //     // new TierLoot(1, ItemType.Weapon, 0.2),
            //     new ItemLoot("Health Potion", 0.03f)
            // ),
            new Shoot(3, new LinePath(4f), targeted: true, projName: "Blade", damage: 4,
                lifetimeMs: 600, cooldownMS: 2500),
            new Follow(distFromTarget: 1, speed: 5.46f),
            new Wander(2.94f)
        );
}