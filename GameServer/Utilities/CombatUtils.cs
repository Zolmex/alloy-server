using Common;
using Common.Structs;
using Common.Utilities;
using Common.Utilities.Collections;
using GameServer.Game.Entities;
using GameServer.Game.Entities.Behaviors;
using GameServer.Game.Entities.Components;
using GameServer.Game.Worlds;

namespace GameServer.Utilities;

public static class CombatUtils {
    extension(World world) {
        public EntityId GetAttackTarget(WorldPosData pos, float radiusSqr, BehaviorScript.TargetType targetType) {
            switch (targetType) {
                case BehaviorScript.TargetType.ClosestPlayer:
                    return world.Map.GetNearestPlayer(pos, radiusSqr);
                case BehaviorScript.TargetType.RandomPlayerPerBehavior:
                case BehaviorScript.TargetType.RandomPlayerPerCycle:
                    return world.Map.GetPlayersWithin(pos, MathF.Sqrt(radiusSqr)).RandomElement();
                case BehaviorScript.TargetType.FarthestPlayer:
                    return world.Map.GetFarthestPlayer(pos, radiusSqr);
                default:
                    return EntityId.Null;
            }
        }
    }
}