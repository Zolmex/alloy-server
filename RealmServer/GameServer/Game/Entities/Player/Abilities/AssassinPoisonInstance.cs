using Common;
using Common.Resources.Xml.Descriptors;
using Common.Utilities;
using GameServer.Game.Network.Messaging.Outgoing;
using System;

namespace GameServer.Game.Entities;

public class PoisonInstance
{
    private static readonly Logger _log = new Logger(typeof(PoisonInstance));

    public Item Item { get; private set; }
    public Character Target { get; private set; }
    public Player Caster { get; private set; }
    public long NextTickTime { get; private set; }
    public long StartTime { get; private set; }
    public int DamageLeft { get; private set; }
    public float Efficiency { get; private set; }
    public bool Active { get; private set; }

    public PoisonInstance(Item item, Character target, Player caster, long startTime, float efficiency)
    {
        if (target.Dead)
        {
            Deactivate();
            return;
        }

        Item = item;
        Target = target;
        Caster = caster;
        StartTime = startTime;
        Efficiency = efficiency;
        Active = true;

        var poison = item.Poison;
        
        // Calculate total damage over time with efficiency
        var totalTicks = poison.Duration / poison.TickSpeed;
        DamageLeft = (int)Math.Floor(poison.Damage * totalTicks * efficiency / 100);
        
        // First tick happens after impact damage
        NextTickTime = startTime + poison.TickSpeed;
        
        if (poison.ImpactDamage > 0)
        {
            int impactDamage = (int)Math.Floor(poison.ImpactDamage * (efficiency / 100));
            target.DamageWithText(impactDamage, from: caster, prefix: "Poison Impact: ", ignoreInvincible: true);
        }
        
        caster.OnTick += Tick;
    }

    private void Tick(RealmTime time)
    {
        var poison = Item.Poison;

        // Check if poison has expired or target is dead
        if (StartTime + poison.Duration < time.TotalElapsedMs || DamageLeft <= 0 || Target.Dead)
        {
            // Handle poison spread on expiration/death
            if (Target.Dead && poison.SpreadEfficiency != 0 && poison.SpreadRange != 0)
                SpreadPoison(time.TotalElapsedMs);
            
            Deactivate();
            return;
        }
        
        if (time.TotalElapsedMs < NextTickTime)
            return;
        
        var tickDamage = (int)Math.Floor(poison.Damage * (Efficiency / 100));
        var actualDamage = Math.Min(DamageLeft, tickDamage);

        Target.DamageWithText(actualDamage, from: Caster, prefix: "Poison: ", ignoreInvincible: true);

        DamageLeft -= actualDamage;
        NextTickTime += poison.TickSpeed;
        
        if (poison.Effects != null)
            Target.ApplyConditionEffects(poison.Effects);
    }

    private void SpreadPoison(long currentTime)
    {
        var poison = Item.Poison;
        var spreadTargetNum = 0;

        foreach (var enemy in Caster.World.GetEnemiesWithin(Target.Position.X, Target.Position.Y, poison.SpreadRange))
        {
            if (enemy is not Enemy || enemy.Dead)
                continue;
            
            if (spreadTargetNum >= poison.SpreadTargetsNum)
                break;

            // Create new poison instance on spread target
            // Making poison efficiency degressive
            new PoisonInstance(Item, enemy, Caster, currentTime, Efficiency * poison.SpreadEfficiency);

            // Visual effect per enemy when poison spreads
            ShowEffect.Write(Caster.User.Network,
                (byte)ShowEffectIndex.Nova,
                Caster.Id,
                0xFFFFFF,
                1,
                new WorldPosData(enemy.Position.X, enemy.Position.Y),
                new WorldPosData());

            spreadTargetNum++;
        }

        // Visual effect at spread origin
        ShowEffect.Write(Caster.User.Network,
            (byte)ShowEffectIndex.Nova,
            Caster.Id,
            0xFF0000,
            poison.SpreadRange,
            new WorldPosData(Target.Position.X, Target.Position.Y),
            new WorldPosData());
    }

    public void Deactivate()
    {
        if (!Active)
            return;

        Active = false;
        Caster.OnTick -= Tick;
    }
}