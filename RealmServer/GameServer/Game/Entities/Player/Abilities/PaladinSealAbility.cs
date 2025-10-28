using Common;
using Common.Resources.Config;
using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using Common.Utilities;
using GameServer.Game.Entities.Behaviors;
using GameServer.Game.Network.Messaging.Outgoing;
using GameServer.Game.Worlds;
using System;
using System.Diagnostics.Metrics;
using System.Numerics;

namespace GameServer.Game.Entities;

public class PaladinSealAbility : Ability
{
    private static readonly Logger _log = new Logger(typeof(PaladinSealAbility));

    private long _cooldownReset;
    private Entity _banner;
    private PaladinBanner _behavior;
    private short _bannerCount;

    public PaladinSealAbility(Player player) : base(player)
    {
        _player.OnInvChanged += OnInvChanged;
    }

    public override void Use(Item item, WorldPosData usePos, int clientTime) // Ability use is validated before this is called
    {
        var seal = item.Seal;

        _player.MP -= seal.MpCost;
        _cooldownReset = RealmManager.WorldTime.TotalElapsedMs + RealmManager.TicksFromTime(item.Cooldown); // Reset cooldown
        
        foreach (var player in _player.GetPlayersWithin(seal.Radius))
            if (player.MS < player.MaxMS + seal.ShieldAmount)
                player.MS += seal.ShieldAmount;
        if (_player.MS < _player.MaxMS + seal.ShieldAmount)
            _player.MS += seal.ShieldAmount;
        
        ShowEffect.Write(_player.User.Network,
            (byte)ShowEffectIndex.Nova,
            _player.Id,
            0,
            seal.Radius,
            _player.Position,
            default);
        
        if (_bannerCount >= seal.MaxBanners)
            return;
        
        _banner = _player.World.SpawnEntity("Paladin Banner", _player.Position);
        _behavior = (PaladinBanner)((Character)_banner).Behavior;
        _behavior.Seal = seal;
        _behavior.Player = _player;
        _bannerCount++;
        _player.World.AddTimedAction(seal.Duration, () => _bannerCount--);
    }

    public override bool Validate(Item item, Entity en)
    {
        if (item.Seal == null || _player.MP < _item.Seal.MpCost)
            return false;

        return en == _player && _cooldownReset <= RealmManager.WorldTime.TotalElapsedMs;
    }

    private void OnInvChanged(int slot, Item item)
    {
        if (slot != 1 || _item == item)
            return;
        
        _item = item; // Can be null
    }
}