using System;
using System.Numerics;
using Arch.Core;
using Common;
using Common.Game;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Components;

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

    public override void Start(ref EntityContext host) {
        var protectInfo = host.Behavior.Resources.ResolveResource<ProtectInfo>(this);
        protectInfo.State = ProtectState.DontKnowWhere;
    }

    public override BehaviorTickState Tick(ref EntityContext host, ref RealmTime time) {
        var protectInfo = host.Behavior.Resources.ResolveResource<ProtectInfo>(this);
        // if (host.HasConditionEffect(ConditionEffectIndex.Paralyzed)) // TODO: condition effects
        //     return BehaviorTickState.BehaviorFailed;

        Vector2 vect;
        var s = protectInfo.State;
        var entity = host.World.Map.GetNearestOtherEntityByName(host.Position.Pos, host.Entity, _protectee, _acquireRange);
        switch (s) {
            case ProtectState.DontKnowWhere:
                if (entity != Entity.Null) {
                    s = ProtectState.Protecting;

                    goto case ProtectState.Protecting;
                }

                break;

            case ProtectState.Protecting:
                if (entity == Entity.Null) {
                    s = ProtectState.DontKnowWhere;

                    break;
                }

                ref var pos = ref host.World.Ecs.Get<Position>(entity);
                vect = new Vector2(pos.Pos.X - host.Position.Pos.X, pos.Pos.Y - host.Position.Pos.Y);
                if (vect.Length() > _reprotectRange) {
                    vect = Vector2.Normalize(vect);

                    var dist = host.GetSpeed(_speed) * (time.ElapsedMsDelta / 1000f);
                    var newX = host.Position.Pos.X + vect.X * dist;
                    var newY = host.Position.Pos.Y + vect.Y * dist;
                    host.Position.Move(newX, newY);
                }
                else {
                    s = ProtectState.Protected;
                }

                break;

            case ProtectState.Protected:
                if (entity == Entity.Null) {
                    s = ProtectState.DontKnowWhere;

                    break;
                }

                pos = ref host.World.Ecs.Get<Position>(entity);
                vect = new Vector2(pos.Pos.X - host.Position.Pos.X, pos.Pos.Y - host.Position.Pos.Y);
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