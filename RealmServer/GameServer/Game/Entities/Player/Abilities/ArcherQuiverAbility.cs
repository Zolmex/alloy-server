using Common;
using Common.Resources.Xml.Descriptors;
using GameServer.Game.DamageSources;
using System;

namespace GameServer.Game.Entities;

public class ArcherQuiverAbility : Ability
{
    private int _arrows;
    private int Arrows
    {
        get => _arrows;
        set
        {
            _arrows = value;
            _player.AbilityDataA = _arrows;
        }
    }

    public ArcherQuiverAbility(Player player) : base(player)
    {
        _player.OnEnemyHit += EnemyHit;
        _player.OnInvChanged += OnInvChanged;
    }

    private void OnInvChanged(int slot, Item item)
    {
        if (slot != 1 || _item == item)
            return;

        _item = item;
        Arrows = 0;
    }

    public override bool Validate(Item item, Entity en)
    {
        if (_player.MP < item.Quiver.MpCost)
            return false;
        
        if (_arrows < 1)
            return false;

        return true;
    }

    private void EnemyHit(Character enemy, DamageSource damageSource)
    {
        var arrowChance = _item.Quiver.ArrowChance;
        if (_player.Rand.NextSingle() <= arrowChance)
        {
            if (_arrows >= _item.Quiver.MaxArrows)
                return;
            
            Arrows++;
        }
    }

    public override void Use(Item item, WorldPosData usePos, int clientTime)
    {
        switch (item.ObjectType)
        {
            default:
                UseTieredQuiver(item, usePos, clientTime);
                break;
        }
    }

    private void UseTieredQuiver(Item item, WorldPosData usePos, int clientTime)
    {
        var projDesc = item.Projectiles[item.Quiver.ProjectileId];
        var mpProjDesc = item.Projectiles[item.Quiver.MpProjectileId];
        
        var arrows = _arrows;
        var mpArrows = GetSubstituteMpArrows();
        
        _player.ServerShoot(projDesc, (byte)arrows, _player.Position.AngleDegrees(usePos), 5f, _player.Position, item.ObjectType);
        
        Arrows = 0;
        _player.MP -= item.Quiver.MpCost;

        if (mpArrows < 1)
            return;
        
        _player.ServerShoot(mpProjDesc, (byte)mpArrows, _player.Position.AngleDegrees(usePos), 5f, _player.Position, item.ObjectType);
        _player.MP -= item.Quiver.MpPerArrow * mpArrows;
    }

    private int GetSubstituteMpArrows()
    {
        if (!_item.Quiver.UseMpArrows)
            return 0;
        
        var available = _item.Quiver.MaxArrows - _arrows;
        var mpPerArrow = _item.Quiver.MpPerArrow;
        var playerMp = _player.MP;
        return Math.Min(available, playerMp / mpPerArrow);
    }
}