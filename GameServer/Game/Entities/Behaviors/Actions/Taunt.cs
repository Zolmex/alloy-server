using System;
using Common;
using Common.Game;
using Common.Utilities;
using GameServer.Game.Entities.Old;
using GameServer.Game.Entities.Old.Extensions;

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

    public override void Start(ref EntityContext host) {
        if (_cooldownMS == 0 && _rand.NextDouble() < _probability) {
            var text = _text.RandomElement();
            foreach (var user in host.World.Users.Values) {
                var name = host.Stats.GetString(StatType.Name);
                user.SendEnemy(name, text);
            }
        }
    }

    public override BehaviorTickState Tick(ref EntityContext host, ref RealmTime time) {
        if (_cooldownMS == 0)
            return BehaviorTickState.BehaviorFailed; // IDK ??!??!

        var tauntInfo = host.BehavController.Resources.ResolveResource<TauntInfo>(this);
        if (tauntInfo.CooldownLeft > 0) {
            tauntInfo.CooldownLeft -= time.ElapsedMsDelta;
            return BehaviorTickState.OnCooldown;
        }

        tauntInfo.CooldownLeft = _cooldownMS;
        if (_rand.NextDouble() < _probability) {
            var text = _text.RandomElement();
            foreach (var user in host.World.Users.Values) {
                var name = host.Stats.GetString(StatType.Name);
                user.SendEnemy(name, text);
            }
        }
        return BehaviorTickState.BehaviorActive;
    }
}