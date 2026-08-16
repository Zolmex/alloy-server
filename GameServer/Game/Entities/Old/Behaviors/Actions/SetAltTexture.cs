using Common;
using Common.Game;

namespace GameServer.Game.Entities.Behaviors.Actions;

public class SetAltTextureInfo {
    public int CurrentTexture;
    public int RemainingTime;
}

public record SetAltTexture : BehaviorScript {
    private readonly int _cooldown;
    private readonly int _indexMax;
    private readonly int _indexMin;
    private readonly bool _loop;

    public SetAltTexture(int minValue, int maxValue = -1, int cooldown = 0, bool loop = false) {
        _indexMin = minValue;
        _indexMax = maxValue;
        _cooldown = cooldown;
        _loop = loop;
    }

    public override void Start(ref EntityView host) {
        var state = host.Behavior.Resources.ResolveResource<SetAltTextureInfo>(this);
        var altTexture = host.Stats.GetInt(StatType.AltTextureIndex);
        state.CurrentTexture = altTexture;
        state.RemainingTime = _cooldown;

        if (altTexture != _indexMin) {
            host.Stats.Set(StatType.AltTextureIndex, _indexMin);
            state.CurrentTexture = _indexMin;
        }
    }

    public override BehaviorTickState Tick(ref EntityView host, ref RealmTime time) {
        var state = host.Behavior.Resources.ResolveResource<SetAltTextureInfo>(this);

        if (_indexMax == -1 || (state.CurrentTexture == _indexMax && !_loop))
            return BehaviorTickState.BehaviorFailed;

        if (state.RemainingTime > 0) {
            state.RemainingTime -= time.ElapsedMsDelta;
            return BehaviorTickState.OnCooldown;
        }

        if (state.RemainingTime <= 0) {
            var newTexture = state.CurrentTexture >= _indexMax ? _indexMin : state.CurrentTexture + 1;
            state.CurrentTexture = newTexture;
            host.Stats.Set(StatType.AltTextureIndex, newTexture);
            state.RemainingTime = _cooldown;
        }

        return BehaviorTickState.BehaviorActive;
    }
}