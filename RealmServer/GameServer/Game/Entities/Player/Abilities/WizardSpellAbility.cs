using Common;
using Common.Resources.Xml.Descriptors;
using Common.Utilities;
using GameServer.Game.DamageSources;
using GameServer.Game.DamageSources.Projectiles;
using GameServer.Game.Network.Messaging.Outgoing;
using System;
using System.Buffers;
using System.Linq;
using System.Numerics;

namespace GameServer.Game.Entities;

public class WizardSpellAbility : Ability
{
    private static readonly Logger _log = new Logger(typeof(WizardSpellAbility));
    
    public WizardSpellAbility(Player player) : base(player)
    {
    }

    public override bool Validate(Item item, Entity en)
    {
        if (en != _player)
            return false;

        if (_player.MP < item.Spell.MpCost)
            return false;
        
        return true;
    }
    
    public override void Use(Item item, WorldPosData usePos, int clientTime)
    {
        switch (item.ObjectType)
        {
            // Add methods for UTs
            default:
                UseTieredSpell(item, usePos, clientTime);
                break;
        }
    }
    
    private void UseTieredSpell(Item item, WorldPosData usePos, int clientTime)
    {
        var spell = item.Spell;
        
        _player.MP -= spell.MpCost;
        
        var projDesc = item.Projectiles[spell.ProjectileId];
        var numShots = spell.NumProjectiles;
        
        var angleDelta = 360f / numShots;
        
        _player.ServerShoot(projDesc, numShots, 0f, angleDelta, usePos, item.ObjectType);
        
        ShowEffect.Write(_player.User.Network,
            (byte)ShowEffectIndex.Line,
            _player.Id,
            0xFF00AA,
            0,
            usePos,
            default);
    }
}