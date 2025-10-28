using Common;
using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using Common.Utilities;
using GameServer.Game.Network.Messaging.Outgoing;
using GameServer.Game.Worlds;
using System;
using System.Diagnostics.Metrics;

namespace GameServer.Game.Entities;

public class NinjaSheathAbility : Ability
{
    private static readonly Logger _log = new Logger(typeof(NinjaSheathAbility));

    public bool InStance { get; private set; }

    private long _cooldownReset;
    private int _wellDamage;
    private int _stanceLifetime;

    private long _nextSlashAt;

    public NinjaSheathAbility(Player player) : base(player)
    {
        _player.OnDamageDealt += OnDamageDealt;
        _player.OnInvChanged += OnInvChanged;
    }

    public override void Use(Item item, WorldPosData usePos, int clientTime) // Ability use is validated before this is called
    {
        _cooldownReset = RealmManager.WorldTime.TotalElapsedMs + RealmManager.TicksFromTime(item.Cooldown); // Reset cooldown
        StartStance();
    }

    public override bool Validate(Item item, Entity en)
    {
        if (item.Sheath == null || InStance || _wellDamage == 0)
            return false;

        return en == _player && _cooldownReset <= RealmManager.WorldTime.TotalElapsedMs;
    }

    private void Tick(RealmTime time)
    {
        _stanceLifetime = Math.Max(0, _stanceLifetime - time.ElapsedMsDelta);
        if (_stanceLifetime == 0) // Make sure stance is finished
        {
            EndStance();
            return;
        }

        TrySlash(time);
    }

    private void TrySlash(RealmTime time)
    {
        if (time.TotalElapsedMs < _nextSlashAt) // Wait for slash cooldown
            return;
        
        _nextSlashAt = time.TotalElapsedMs + _item.Sheath.SlashCooldownMS;

        var target = _player.GetNearestOtherEnemyByName(null, _item.Sheath.Radius); // Find target
        if (target == null) // Slash fails
            return;

        var slashDmg = Math.Min(_wellDamage, _item.Sheath.SlashDamage); // Get current slash damage
        if (slashDmg == 0) // Tick method will check if stance is finished (depends on sheath's StanceDuration)
            return;

        var dmgDealt = target.Damage(slashDmg, _player);
        if (_item.Sheath.Effects != null)
            target.ApplyConditionEffects(_item.Sheath.Effects);
        
        Notification.Write(_player.User.Network, // Enemy damage notif
            target.Id,
            dmgDealt.ToString(),
            0,
            0,
            true); // Must be true so that the client formats it properly
        
        ShowEffect.Write(_player.User.Network,
            (byte)ShowEffectIndex.SheatheSlash,
            target.Id,
            0,
            0,
            default,
            default);

        _wellDamage -= dmgDealt;
        _player.MP -= _item.Sheath.ManaPerSlash;
        _player.AbilityDataA = _wellDamage;
    }

    private void OnInvChanged(int slot, Item item)
    {
        if (slot != 1 || _item == item)
            return;

        _item = item; // Can be null
        _wellDamage = 0; // Reset well damage on ability change
        _player.AbilityDataA = 0;
    }
    
    private void OnDamageDealt(Character target, int damage)
    {
        if (_item?.Sheath == null || InStance)
            return;

        var add = (int)MathF.Ceiling(damage * _item.Sheath.Efficiency);
        _wellDamage = Math.Min(_wellDamage + add, _item.Sheath.Capacity);
        _player.AbilityDataA = _wellDamage;
    }

    public void StartStance()
    {
        if (InStance)
            return;
        
        InStance = true;
        _stanceLifetime = _item.Sheath.StanceDuration;
        
        _player.OnTick += Tick;
        _player.ApplyConditionEffect(ConditionEffectIndex.SheatheStance, -1);
    }

    public void EndStance()
    {
        if (!InStance)
            return;

        InStance = false;
        _stanceLifetime = 0;
        _wellDamage = 0;
        _player.AbilityDataA = 0;
        
        _player.OnTick -= Tick;
        _player.RemoveConditionEffect(ConditionEffectIndex.SheatheStance);
    }
}