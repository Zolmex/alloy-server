using System;
using Common.Game;
using Common.Utilities;
using GameServer.Game.Entities.Extensions;

namespace GameServer.Game.Entities.Behaviors.Actions;

public class TauntInfo {
    public int CooldownLeft;
}

public record Taunt : BehaviorScript {
    private static readonly Random _rand = new();
    private readonly int _cooldownMS;
    private readonly float _probability;

    private readonly string[] _text;

    public Taunt(string text, int coolDownMS = 0, float probability = 1f) {
        _text = text.Split("||");
        _cooldownMS = coolDownMS;
        _probability = probability;
    }

    public override void Start(ref EntityView host) {
        if (_cooldownMS == 0 && _rand.NextDouble() < _probability) {
            var text = _text.RandomElement();
            foreach (var user in host.World.PlayerToUser.Values)
                user.SendEnemy(ref host.Entity, text);
        }
    }

    public override BehaviorTickState Tick(ref EntityView host, ref RealmTime time) {
        if (_cooldownMS == 0)
            return BehaviorTickState.BehaviorFailed; // IDK ??!??!

        var tauntInfo = host.Behavior.Resources.ResolveResource<TauntInfo>(this);
        if (tauntInfo.CooldownLeft > 0) {
            tauntInfo.CooldownLeft -= time.ElapsedMsDelta;
            return BehaviorTickState.OnCooldown;
        }

        tauntInfo.CooldownLeft = _cooldownMS;
        if (_rand.NextDouble() < _probability) {
            var text = _text.RandomElement();
            foreach (var user in host.World.PlayerToUser.Values)
                user.SendEnemy(ref host.Entity, text);
        }
        return BehaviorTickState.BehaviorActive;
    }
}