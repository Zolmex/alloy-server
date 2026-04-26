#region

using Common;
using GameServer.Game.Entities.Types;

#endregion

namespace GameServer.Game.Entities.Behaviors.Actions;

public record ConditionEffectBehavior : BehaviorScript {
    private readonly ConditionEffectIndex _condEffect;
    private readonly int _durationMS;
    private readonly bool _persist;

    public ConditionEffectBehavior(ConditionEffectIndex effect, int durationMS = -1, bool persist = false) {
        _condEffect = effect;
        _durationMS = durationMS;
        _persist = persist;
    }

    public override void Start(CharacterEntity host) {
        if (_durationMS == 0) { // Remove effect
            host.RemoveConditionEffect(_condEffect);
            return;
        }

        host.ApplyConditionEffect(_condEffect, _durationMS);
    }

    public override void End(CharacterEntity host, RealmTime time) {
        if (_persist)
            return;

        host.RemoveConditionEffect(_condEffect);
    }
}