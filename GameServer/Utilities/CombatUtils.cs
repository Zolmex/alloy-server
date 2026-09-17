using Arch.Core;
using Common;
using Common.Structs;
using Common.Utilities;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Behaviors;
using World = GameServer.Game.Worlds.World;

namespace GameServer.Utilities;

public static class CombatUtils {
    extension(World world) {
        public Entity GetAttackTarget(WorldPosData pos, float radiusSqr, BehaviorScript.TargetType targetType, string target = "player") {
            switch (targetType) {
                case BehaviorScript.TargetType.ClosestPlayer:
                    return world.Map.GetNearestPlayer(pos, radiusSqr);
                case BehaviorScript.TargetType.RandomPlayerPerBehavior:
                case BehaviorScript.TargetType.RandomPlayerPerCycle:
                    return world.Map.GetPlayersWithin(pos, MathF.Sqrt(radiusSqr)).RandomElement();
                case BehaviorScript.TargetType.Entity:
                    return world.Map.GetNearestEntityByName(target, pos, radiusSqr);
                case BehaviorScript.TargetType.FarthestPlayer:
                    return world.Map.GetFarthestPlayer(pos, radiusSqr);
                default:
                    return Entity.Null;
            }
        }
    }
}