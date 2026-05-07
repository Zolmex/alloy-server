using Common;
using Common.Game;
using GameServer.Game.Entities.Components;

namespace GameServer.Game.Entities.Behaviors.Actions;

public class SizeInfo {
    public int CooldownLeft;
}

public record ChangeSize : BehaviorScript {
    private readonly int _rate;
    private readonly int _target;

    public ChangeSize(int rate, int target) {
        _rate = rate;
        _target = target;
    }

    public override void Start(ref EntityView host) {
        var state = host.Behavior.Resources.ResolveResource<SizeInfo>(this);
        state.CooldownLeft = 0;
    }

    public override BehaviorTickState Tick(ref EntityView host, ref RealmTime time) {
        var state = host.Behavior.Resources.ResolveResource<SizeInfo>(this);

        if (state.CooldownLeft > 0) {
            state.CooldownLeft -= time.ElapsedMsDelta;
            return BehaviorTickState.OnCooldown;
        }

        if (state.CooldownLeft <= 0) {
            ref var stats = ref host.World.EntityStats.Get(host.Id);
            var size = stats.GetInt(StatType.Size);
            if (size != _target) {
                size += _rate;
                if ((_rate > 0 && size > _target) ||
                    (_rate < 0 && size < _target))
                    size = _target;
            }
            stats.Set(StatType.Size, size);

            state.CooldownLeft = 150;
        }

        return BehaviorTickState.BehaviorActive;
    }
}