using Common;
using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using GameServer.Game.DamageSources;
using GameServer.Game.DamageSources.Projectiles;
using GameServer.Game.Entities.Stacks;
using System;
using System.Numerics;

namespace GameServer.Game.Entities;

public partial class Player
{
    public event Action<ModStacks, int> StacksLost;
    public event Action<Character> OnKill;
    public event Action<int> OnHeal;
    public event Action InCombat;
    public event Action<StatType> OnStatChanged;
    public event Action<Character, DamageSource> OnEnemyHit;
    public event Action<Character, int> OnDamageDealt;
    public event Action<int, Item> OnInvChanged; // slot, item
    public event Action<ProjectileDesc, float, Vector2> OnDoShoot; // Triggers every time the weapon is fired
    public event Action<Projectile> OnShoot; // Triggers for every projectile
    
    public void StacksLostInvoke(ModStacks type, int amount)
    {
        StacksLost?.Invoke(type, amount);
    }

    public void OnKillInvoke(Character killed)
    {
        OnKill?.Invoke(killed);
    }

    public void EnemyHit(Character target, DamageSource damageSource)
    {
        OnEnemyHit?.Invoke(target, damageSource);
    }

    public void InvChanged(int slot, Item item)
    {
        OnInvChanged?.Invoke(slot, item);
    }

    public void DamageDealt(Character target, int damage)
    {
        OnDamageDealt?.Invoke(target, damage);
    }
}