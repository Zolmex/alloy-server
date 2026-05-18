using System;
using System.Numerics;
using Common;
using Common.Game;
using Common.Utilities.Collections;

namespace GameServer.Game.Entities.Behaviors.Actions;

public enum ProtectState {
    DontKnowWhere,
    Protecting,
    Protected
}

public class ProtectInfo {
    public ProtectState State;
}

public record Protect : BehaviorScript {
    private readonly float _acquireRange;
    private readonly string _protectee;
    private readonly float _protectionRange;
    private readonly float _reprotectRange;
    private readonly float _speed;

    public Protect(float speed, string protectee, float acquireRange = 10, float protectionRange = 2,
        float reprotectRange = 1) {
        _acquireRange = acquireRange;
        _protectee = protectee;
        _protectionRange = protectionRange;
        _reprotectRange = reprotectRange;
        _speed = speed;
    }

    public override void Start(ref EntityView host) {
        var protectInfo = host.Behavior.Resources.ResolveResource<ProtectInfo>(this);
        protectInfo.State = ProtectState.DontKnowWhere;
    }

    public override BehaviorTickState Tick(ref EntityView host, ref RealmTime time) {
        var protectInfo = host.Behavior.Resources.ResolveResource<ProtectInfo>(this);
        // if (host.HasConditionEffect(ConditionEffectIndex.Paralyzed)) // TODO: condition effects
        //     return BehaviorTickState.BehaviorFailed;

        Vector2 vect;
        var s = protectInfo.State;
        var entityId = host.World.Map.GetNearestOtherEntityByName(host.Stats.Pos, host.Id, _protectee, _acquireRange);
        switch (s) {
            case ProtectState.DontKnowWhere:
                if (entityId != EntityId.Null) {
                    s = ProtectState.Protecting;

                    goto case ProtectState.Protecting;
                }

                break;

            case ProtectState.Protecting:
                if (entityId == EntityId.Null) {
                    s = ProtectState.DontKnowWhere;

                    break;
                }

                ref var stats = ref host.World.EntityStats.Get(entityId);
                vect = new Vector2(stats.Pos.X - host.Stats.Pos.X, stats.Pos.Y - host.Stats.Pos.Y);
                if (vect.Length() > _reprotectRange) {
                    vect = Vector2.Normalize(vect);

                    var dist = host.Stats.GetSpeed(_speed) * (time.ElapsedMsDelta / 1000f);
                    var newX = host.Stats.Pos.X + vect.X * dist;
                    var newY = host.Stats.Pos.Y + vect.Y * dist;
                    host.Stats.Move(newX, newY);
                }
                else {
                    s = ProtectState.Protected;
                }

                break;

            case ProtectState.Protected:
                if (entityId == EntityId.Null) {
                    s = ProtectState.DontKnowWhere;

                    break;
                }

                stats = ref host.World.EntityStats.Get(entityId);
                vect = new Vector2(stats.Pos.X - host.Stats.Pos.X, stats.Pos.Y - host.Stats.Pos.Y);
                if (vect.Length() > _protectionRange) {
                    s = ProtectState.Protecting;

                    goto case ProtectState.Protecting;
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        protectInfo.State = s;
        return BehaviorTickState.BehaviorActive;
    }
}