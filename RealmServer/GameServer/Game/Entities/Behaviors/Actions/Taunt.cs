#region

using Common.Utilities;
using System;
using GameServer.Game.Entities.Types;

#endregion

namespace GameServer.Game.Entities.Behaviors.Actions;

public class TauntInfo
{
    public int CooldownLeft;
}

public record Taunt : BehaviorScript
{
    private static readonly Random _rand = new();
    private readonly int _cooldownMS;
    private readonly float _probability;

    private readonly string[] _text;

    public Taunt(string text, int coolDownMS = 0, float probability = 1f)
    {
        _text = text.Split("||");
        _cooldownMS = coolDownMS;
        _probability = probability;
    }

    public override void Start(CharacterEntity host)
    {
        if (_cooldownMS == 0 && _rand.NextDouble() < _probability)
            host.World.Taunt(host, _text.RandomElement());
    }

    public override BehaviorTickState Tick(CharacterEntity host, RealmTime time)
    {
        if (_cooldownMS == 0)
            return BehaviorTickState.BehaviorFailed; // IDK ??!??!

        var tauntInfo = host.ResolveResource<TauntInfo>(this);
        if (tauntInfo.CooldownLeft > 0)
        {
            tauntInfo.CooldownLeft -= time.ElapsedMsDelta;
            return BehaviorTickState.OnCooldown;
        }

        tauntInfo.CooldownLeft = _cooldownMS;
        if (_rand.NextDouble() < _probability)
            host.World.Taunt(host, _text.RandomElement());
        return BehaviorTickState.BehaviorActive;
    }
}