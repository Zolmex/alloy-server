using Common;
using Common.Resources.Xml.Descriptors;
using System;
using System.Numerics;
using GameServer.Game.Entities.DamageSources;
using GameServer.Game.Entities.DamageSources.Types;

namespace GameServer.Game.Entities.Types;

public partial class Player
{
    public event Action<CharacterEntity> OnKill;
    public event Action<int> OnHeal;
    public event Action InCombat;
    public event Action<CharacterEntity, DamageSource> OnEnemyHit;
    public event Action<CharacterEntity, int> OnDamageDealt;
    public event Action<int, Item> OnInvChanged; // slot, item
    public event Action<ProjectileDesc, float, Vector2> OnDoShoot; // Triggers every time the weapon is fired
    public event Action<Projectile> OnShoot; // Triggers for every projectile

    public void OnKillInvoke(CharacterEntity killed)
    {
        OnKill?.Invoke(killed);
    }

    public void EnemyHit(CharacterEntity target, DamageSource damageSource)
    {
        OnEnemyHit?.Invoke(target, damageSource);
    }

    public void InvChanged(int slot, Item item)
    {
        OnInvChanged?.Invoke(slot, item);
    }

    public void DamageDealt(CharacterEntity target, int damage)
    {
        OnDamageDealt?.Invoke(target, damage);
    }
}