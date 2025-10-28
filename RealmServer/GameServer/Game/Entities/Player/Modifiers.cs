#region

using Common;
using Common.Resources.Config;
using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using GameServer.Game.DamageSources;
using GameServer.Game.DamageSources.Projectiles;
using GameServer.Game.Entities.Stacks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;

#endregion

namespace GameServer.Game.Entities
{
    public enum ModTypes
    {
        None,
        NaturesVengeance,
        SteadyAssault,
        Masterpiece,
        DivergingDestiny,
        RisingFury,
        BeatEmUp,
        HolyExcalibur,
        Lifeblood,
        DelightfulHarvest,
        InnerMachinations,
        BloodReaper,
        SoulReaper,
        MindReaper,
        PrimalStrike,

        EquipmentBonuses
    }

    public abstract class Modifier
    {
        public ModTypes Id { get; set; }
        public int Order { get; set; } = -1;

        protected Player _player;

        public virtual void Initialize(Player player)
        {
            _player = player;
        }

        public abstract void Apply();
        public abstract void SubscribeEvents();
        public abstract void Remove();
    }

    public class EquipmentBonuses : Modifier
    {
        private readonly Dictionary<StatType, int> _flatBonuses = new();
        private readonly Dictionary<StatType, float> _percentBonuses = new();

        public EquipmentBonuses()
        {
            Id = ModTypes.EquipmentBonuses;
        }

        public override void Apply()
        {
            RemoveBonuses();
            for (var i = 0; i < 4; i++)
                ApplyBonuses(i);
            _player.RecalculateStats();
        }

        public override void SubscribeEvents()
        {
            _player.OnInvChanged += OnInvChanged;
        }

        public override void Remove()
        {
            _player.OnInvChanged -= OnInvChanged;
            RemoveBonuses();
        }

        private void OnInvChanged(int slot, Item item)
        {
            if (slot > 3) // Ignore changes outside of equipment
                return;

            RemoveBonuses();
            for (var i = 0; i < 4; i++)
                ApplyBonuses(i);
            _player.RecalculateStats();
        }

        private void RemoveBonuses()
        {
            foreach (var kvp in _flatBonuses)
                _player.RemoveFlatBonus(kvp.Key, kvp.Value);

            foreach (var kvp in _percentBonuses)
                _player.RemoveIncreasedBonus(kvp.Key, kvp.Value);

            _flatBonuses.Clear();
            _percentBonuses.Clear();
        }

        private void ApplyBonuses(int slot)
        {
            var item = _player.Inventory.GetItem(slot);
            ApplyEquipmentBoosts(item);
            ApplyGemstonesBoosts(item);
        }

        private void ApplyEquipmentBoosts(Item item)
        {
            if (item?.StatBoosts == null || item.StatBoosts.Length < 1)
                return;

            foreach (var boost in item.StatBoosts)
            {
                var stat = GetBoostStatType(boost.Stat);
                var amount = boost.Amount;

                if (!_flatBonuses.TryGetValue(stat, out var prevValue))
                    _flatBonuses[stat] = 0;
                _flatBonuses[stat] = prevValue + amount;
                _player.AddFlatBonus(stat, amount);
            }
        }

        private void ApplyGemstonesBoosts(Item item)
        {
            if (item?.Gemstones == null || item.Gemstones.Length < 1)
                return;

            foreach (var gemType in item.Gemstones)
            {
                var gem = XmlLibrary.Gemstones[(ushort)gemType];
                if (gem.Gemstone == null || gem.Gemstone.Boosts == null)
                    continue;

                foreach (var boost in gem.Gemstone.Boosts)
                {
                    var amount = boost.Amount;
                    if (boost.BoostTarget == "Player")
                    {
                        var stat = Enum.Parse<StatType>(boost.Stat);
                        if (boost.BoostType != "Static") // Percentage boost
                        {
                            var floatAmount = amount / 100f;
                            if (!_percentBonuses.TryGetValue(stat, out var prevValue))
                                _percentBonuses[stat] = 0;
                            _percentBonuses[stat] = prevValue + floatAmount;
                            _player.AddIncreasedBonus(stat, floatAmount);
                        }
                        else
                        {
                            if (!_flatBonuses.TryGetValue(stat, out var prevValue))
                                _flatBonuses[stat] = 0;
                            _flatBonuses[stat] = (int)(prevValue + amount);
                            _player.AddFlatBonus(stat, amount);
                        }
                    }
                }
            }
        }

        private static StatType GetBoostStatType(int statKey)
        {
            return statKey switch
            {
                0 => StatType.MaxHP,
                1 => StatType.MaxMP,
                2 => StatType.Attack,
                20 => StatType.Attack, // Backwards compatibility
                3 => StatType.Defense,
                21 => StatType.Defense,
                4 => StatType.MovementSpeed,
                22 => StatType.MovementSpeed,
                5 => StatType.Dexterity,
                28 => StatType.Dexterity,
                6 => StatType.LifeRegeneration,
                26 => StatType.LifeRegeneration,
                7 => StatType.Wisdom,
                27 => StatType.Wisdom,
                8 => StatType.Armor,
                29 => StatType.Armor,
                9 => StatType.CriticalDamage,
                30 => StatType.CriticalDamage,
                _ => throw new ArgumentException($"{statKey} is not a valid player stat.")
            };
        }
    }

    public class NaturesVengeance : Modifier
    {
        public NaturesVengeance()
        {
            Id = ModTypes.NaturesVengeance;
        }

        public override void Apply() { }

        public override void SubscribeEvents()
        {
            _player.OnEnemyHit += OnHitApply;
        }

        public override void Remove()
        {
            _player.OnEnemyHit -= OnHitApply;
        }

        public void OnHitApply(Character owner, DamageSource damageSource)
        {
            owner.Stacks.AddStack(ModStacks.Wrath, 5000, 1, _player);
        }
    }

    public class SteadyAssault : Modifier
    {
        public SteadyAssault()
        {
            Id = ModTypes.SteadyAssault;
        }

        public override void Apply() { }

        public override void SubscribeEvents()
        {
            _player.OnEnemyHit += OnHitApply;
            _player.OnShoot += OnShoot;
        }

        public override void Remove()
        {
            _player.OnEnemyHit -= OnHitApply;
            _player.OnShoot -= OnShoot;
        }

        public void OnHitApply(Character owner, DamageSource damageSource)
        {
            if (_player.Stacks.GetStackCount(ModStacks.FadingFury) > 0)
                _player.Stacks.RemoveStack(ModStacks.FadingFury);
            else
                _player.Stacks.AddStack(ModStacks.Onslaught, -1);
        }

        public void OnShoot(Projectile proj)
        {
            var item = _player.Inventory[0];
            if (item?.Projectiles[0] == null)
                return;

            var avgDmg = (int)(((item.Projectiles[0].MaxDamage * 1.2f) + item.Projectiles[0].MinDamage) / 2f);
            proj.SetDamage(avgDmg);
        }
    }

    public class Masterpiece : Modifier
    {
        public Masterpiece()
        {
            Id = ModTypes.Masterpiece;
        }

        public override void Apply()
        {
            OnStatChange(StatType.HP);
        }

        public override void SubscribeEvents()
        {
            _player.OnStatChanged += OnStatChange;
        }

        public override void Remove()
        {
            _player.OnStatChanged -= OnStatChange;
        }

        public void OnStatChange(StatType type)
        {
            if (type == StatType.HP)
            {
                if (_player.HP >= _player.MaxHP)
                    _player.Stacks.AddStack(ModStacks.Untouched, -1);
                else
                    _player.Stacks.RemoveStack(ModStacks.Untouched);

                if (_player.HP < _player.MaxHP / 2)
                {
                    _player.Stacks.AddStack(ModStacks.Desecrated, -1);
                    _player.Stacks.UpdateStack(ModStacks.Desecrated);
                }
                else
                    _player.Stacks.RemoveStack(ModStacks.Desecrated);
            }
        }
    }

    public class DivergingDestiny : Modifier
    {
        public DivergingDestiny()
        {
            Id = ModTypes.DivergingDestiny;
        }

        public override void Apply()
        { }

        public override void SubscribeEvents()
        {
            _player.OnEnemyHit += OnEnemyHit;
        }

        public override void Remove()
        {
            _player.OnEnemyHit -= OnEnemyHit;
        }

        public void OnEnemyHit(Character chr, DamageSource damageSource)
        { }
    }

    public class RisingFury : Modifier
    {
        public RisingFury()
        {
            Id = ModTypes.RisingFury;
        }

        public override void Apply() { }

        public override void SubscribeEvents()
        {
            _player.InCombat += InCombat;
        }

        public override void Remove()
        {
            _player.InCombat -= InCombat;
        }

        public void InCombat()
        {
            if (_player.TimeInCombat == 0)
            {
                _player.Stacks.RemoveStack(ModStacks.Fury, -1, true);
                return;
            }

            _player.Stacks.AddStack(ModStacks.Fury, -1, 1);
            _player.Stacks.UpdateStack(ModStacks.Fury);
        }
    }

    public class Beatemup : Modifier
    {
        public int Hits;

        public Beatemup()
        {
            Id = ModTypes.BeatEmUp;
        }

        public override void Apply() { }

        public override void SubscribeEvents()
        {
            _player.OnEnemyHit += OnHit;
        }

        public override void Remove()
        {
            _player.OnEnemyHit -= OnHit;
        }

        public void OnHit(Character chr, DamageSource damageSource)
        {
            Hits++;
            if (Hits == 3)
            {
                Hits = 0;
                chr.DamageWithText(damageSource.GetDamage() / 2, from: _player, prefix: "Pow! ");
            }
        }
    }

    public class HolyExcalibur : Modifier
    {
        public const int ShootCd = 3000;
        public int Hits;
        public int Cooldown;
        public Item Item;

        public HolyExcalibur()
        {
            Id = ModTypes.HolyExcalibur;
            Item = XmlLibrary.Id2Item("HolyExcaliburNode");
        }

        public override void Apply() { }

        public override void SubscribeEvents()
        {
            _player.OnEnemyHit += OnHit;
            _player.OnDoShoot += OnDoShoot;
            _player.OnTick += OnTick;
        }

        public override void Remove()
        {
            _player.OnEnemyHit -= OnHit;
            _player.OnDoShoot -= OnDoShoot;
            _player.OnTick -= OnTick;
        }

        public void OnDoShoot(ProjectileDesc projDesc, float angle, Vector2 startPos)
        {
            if (Cooldown <= 0)
            {
                Cooldown = ShootCd;
                var desc = Item.Projectiles[0];
                var dmgs = _player.GetProjDamage(null, projDesc.MinDamage, projDesc.MaxDamage, false, true);
                dmgs[0] = (int)(dmgs[0] * (1 + (Hits / 10)));
                Hits = 0;

                _player.ServerShoot(desc, 1, angle, 0, startPos, Item.ObjectType, (int)dmgs[0], dmgs[1]);
            }
        }

        public void OnTick(RealmTime time)
        {
            Cooldown = Math.Max(0, Cooldown - (1000 / GameServerConfig.Config.TPS));
        }

        public void OnHit(Character chr, DamageSource damageSource)
        {
            Hits++;
        }
    }

    public class Lifeblood : Modifier
    {
        public int LifebloodPool;

        public Lifeblood()
        {
            Id = ModTypes.Lifeblood;
        }

        public override void Apply() { }

        public override void SubscribeEvents()
        {
            _player.OnTick += OnTick;
            _player.OnHeal += OnHeal;
        }

        public override void Remove()
        {
            _player.OnTick -= OnTick;
            _player.OnHeal -= OnHeal;
        }

        public void OnTick(RealmTime time)
        {
            var hpDiff = _player.MaxHP - _player.HP;
            if (hpDiff > 0 && LifebloodPool > 0)
            {
                var poolNewValue = LifebloodPool - hpDiff;
                int healAmt;
                if (poolNewValue > 0)
                {
                    LifebloodPool = poolNewValue;
                    healAmt = hpDiff;
                }
                else
                {
                    healAmt = LifebloodPool;
                    LifebloodPool = 0;
                }

                _player.Heal(healAmt);
            }
        }

        public void OnHeal(int amount)
        {
            var hpDiff = amount + _player.HP - _player.MaxHP;
            if (hpDiff > 0 && LifebloodPool < GetMaxSize())
            {
                var showAmt = LifebloodPool + hpDiff > GetMaxSize() ? GetMaxSize() - LifebloodPool : hpDiff;
                LifebloodPool = Math.Min(LifebloodPool + hpDiff, GetMaxSize());
                _player.SendNotif("Pool +" + showAmt, 0xae002b);
            }
        }

        public int GetMaxSize()
        {
            return _player.MaxHP / 10;
        }
    }

    public class DelightfulHarvest : Modifier
    {
        private readonly Dictionary<Character, int> _hitEnemyDict = new(); // TODO: Needs optimization

        public DelightfulHarvest()
        {
            Id = ModTypes.DelightfulHarvest;
        }

        public override void Apply() { }

        public override void SubscribeEvents()
        {
            _player.OnKill += OnKill;
            _player.OnTick += OnTick;
            _player.OnEnemyHit += OnEnemyHit;
        }

        public override void Remove()
        {
            _player.OnKill -= OnKill;
            _player.OnTick -= OnTick;
            _player.OnEnemyHit -= OnEnemyHit;
            _hitEnemyDict.Clear();
        }

        public void OnEnemyHit(Character hit, DamageSource damageSource)
        {
            if (!_hitEnemyDict.ContainsKey(hit))
                _hitEnemyDict.Add(hit, 0);
        }

        public void OnTick(RealmTime time)
        {
            foreach (var enemy in _hitEnemyDict)
                _hitEnemyDict[enemy.Key] += 1000 / GameServerConfig.Config.TPS;
        }

        public void OnKill(Character killed)
        {
            if (_hitEnemyDict.TryGetValue(killed, out var enemy))
            {
                _player.Heal(GetHealAmt(_player.MaxHP, enemy));
                _player.HealMP(GetHealAmt(_player.MaxMP, enemy));
                _hitEnemyDict.Remove(killed);
            }
            else
            {
                _player.Heal(GetHealAmt(_player.MaxHP, 1000));
                _player.HealMP(GetHealAmt(_player.MaxMP, 1000));
            }
        }

        public int GetHealAmt(int val, int time)
        {
            return Math.Max((int)(val * ((float)Math.Min(1f + (14f * (time / 60000f)), 15f) / 100f)), 1);
        }
    }

    public class InnerMachinations : Modifier
    {
        public int Scale;

        public InnerMachinations()
        {
            Id = ModTypes.InnerMachinations;
        }

        public override void Apply()
        {
            Scale = GetScale();
            DoBonuses(true);
        }

        public override void SubscribeEvents()
        {
            _player.OnEnemyHit += OnEnemyHit;
            _player.OnStatChanged += OnStatChange;
        }

        public override void Remove()
        {
            _player.OnEnemyHit -= OnEnemyHit;
            _player.OnStatChanged -= OnStatChange;
        }

        public void OnEnemyHit(Character hit, DamageSource damageSource)
        {
            _player.Stacks.AddStack(ModStacks.Machinations, 3000);
        }

        public void OnStatChange(StatType type)
        {
            if (type == StatType.ManaRegeneration)
            {
                DoBonuses(false);
                Scale = GetScale();
                DoBonuses(true);
            }
        }

        public void DoBonuses(bool add)
        {
            if (add)
                _player.AddIncreasedBonus(StatType.AttackSpeed, Scale);
            else
                _player.RemoveIncreasedBonus(StatType.AttackSpeed, Scale);
        }

        public int GetScale()
        {
            return _player.ManaRegeneration;
        }
    }

    public class BloodReaper : Modifier
    {
        public int Hits;
        public int StacksLost;

        public BloodReaper()
        {
            Id = ModTypes.BloodReaper;
        }

        public override void Apply() { }

        public override void SubscribeEvents()
        {
            _player.OnEnemyHit += OnEnemyHit;
            _player.OnKill += OnKill;
            _player.StacksLost += OnStacksLost;
        }

        public override void Remove()
        {
            _player.OnEnemyHit -= OnEnemyHit;
            _player.OnKill -= OnKill;
            _player.StacksLost -= OnStacksLost;
        }

        public void OnEnemyHit(Character hit, DamageSource damageSource)
        {
            Hits++;
            if (Hits == 3)
            {
                _player.Stacks.AddStack(ModStacks.BloodReaper, 5000, 1);
                Hits = 0;
            }
        }

        public void OnKill(Character killed)
        {
            _player.Stacks.AddStack(ModStacks.BloodReaper, 5000, 1);
        }

        public void OnStacksLost(ModStacks type, int amount)
        {
            if (type == ModStacks.BloodReaper)
            {
                StacksLost++;
                if (StacksLost == 8)
                {
                    _player.Heal((int)(_player.MaxHP * 0.05));
                    StacksLost = 0;
                }
            }
        }
    }

    public class SoulReaper : Modifier
    {
        public int Hits;
        public int StacksLost;
        public bool IsDoubleDamage;

        public SoulReaper()
        {
            Id = ModTypes.SoulReaper;
        }

        public override void Apply() { }

        public override void SubscribeEvents()
        {
            _player.OnEnemyHit += OnEnemyHit;
            _player.OnKill += OnKill;
            _player.StacksLost += OnStacksLost;
        }

        public override void Remove()
        {
            _player.OnEnemyHit -= OnEnemyHit;
            _player.OnKill -= OnKill;
            _player.StacksLost -= OnStacksLost;
        }

        public void OnEnemyHit(Character hit, DamageSource damageSource)
        {
            Hits++;
            if (Hits == 3)
            {
                _player.Stacks.AddStack(ModStacks.SoulReaper, 5000, 1);
                Hits = 0;
            }

            if (IsDoubleDamage)
            {
                IsDoubleDamage = false;
                hit.DamageWithText(damageSource.GetDamage(), 0x9933ff, 24, _player, "Reaper! ");
            }
        }

        public void OnKill(Character killed)
        {
            _player.Stacks.AddStack(ModStacks.SoulReaper, 5000, 1);
        }

        public void OnStacksLost(ModStacks type, int amount)
        {
            if (type == ModStacks.SoulReaper)
            {
                StacksLost++;
                if (StacksLost == 10)
                {
                    IsDoubleDamage = true;
                    StacksLost = 0;
                }
            }
        }
    }

    public class MindReaper : Modifier
    {
        public int Hits;
        public int StacksLost;

        public MindReaper()
        {
            Id = ModTypes.MindReaper;
        }

        public override void Apply() { }

        public override void SubscribeEvents()
        {
            _player.OnEnemyHit += OnEnemyHit;
            _player.OnKill += OnKill;
            _player.StacksLost += OnStacksLost;
        }

        public override void Remove()
        {
            _player.OnEnemyHit -= OnEnemyHit;
            _player.OnKill -= OnKill;
            _player.StacksLost -= OnStacksLost;
        }

        public void OnEnemyHit(Character hit, DamageSource damageSource)
        {
            Hits++;
            if (Hits == 3)
            {
                _player.Stacks.AddStack(ModStacks.MindReaper, 5000, 1);
                Hits = 0;
            }
        }

        public void OnKill(Character killed)
        {
            _player.Stacks.AddStack(ModStacks.MindReaper, 5000, 1);
        }

        public void OnStacksLost(ModStacks type, int amount)
        {
            if (type == ModStacks.MindReaper)
            {
                StacksLost++;
                if (StacksLost == 8)
                {
                    _player.HealMP((int)(_player.MaxMP * 0.05));
                    StacksLost = 0;
                }
            }
        }
    }

    public class PrimalStrike : Modifier
    {
        private readonly Dictionary<int, (int, long)> _damageDealt = new(); // entity id, (dmg dealt, maxTime)

        public PrimalStrike()
        {
            Id = ModTypes.PrimalStrike;
        }

        public override void Apply() { }

        public override void SubscribeEvents()
        {
            _player.OnEnemyHit += OnHitApply;
        }

        public override void Remove()
        {
            _player.OnEnemyHit -= OnHitApply;
            _damageDealt.Clear();
        }

        public void OnHitApply(Character owner, DamageSource damageSource)
        {
            if (_damageDealt.TryAdd(owner.Id, (0, RealmManager.WorldTime.TotalElapsedMs + 5000)))
            {
                _player.World.AddTimedAction(5000, () => OnVitalEnds(owner));
                owner.DeathEvent += OnEnemyDeath; // Listen to when entity dies to remove it from the dictionary
            }

            var tup = _damageDealt[owner.Id];
            var time = tup.Item2;
            if (RealmManager.WorldTime.TotalElapsedMs < time)
            {
                damageSource.Bonus.ProportionalBonus += 0.12f;
                _damageDealt[owner.Id] = (tup.Item1 + damageSource.GetTotalDamage(), time);
            }
            else if (RealmManager.WorldTime.TotalElapsedMs < time + 10000) // Exhaustion effect
            {
                damageSource.Bonus.ProportionalBonus -= 0.05f;
            }
        }

        private void OnVitalEnds(Character target)
        {
            if (_player.Dead || _player.User.GameInfo.State != GameState.Playing)
                return;

            if (target == null || target.Dead)
                return;

            if (!_damageDealt.TryGetValue(target.Id, out var tup))
                return;

            // Deal smite
            var smiteDmg = (int)(tup.Item1 * 0.1); // 10% of dmg dealt during vital efect
            target.DamageWithText(smiteDmg, 0xFFE61F, 24, _player, "Smite! ");
        }

        private void OnEnemyDeath(Entity en)
        {
            en.DeathEvent -= OnEnemyDeath;
            _damageDealt.Remove(en.Id);
        }
    }

    public static class ModifierRegistry
    {
        private static Dictionary<ModTypes, Type> _allModifiers = new();

        static ModifierRegistry()
        {
            var modifierTypes = Assembly.GetAssembly(typeof(Modifier)).GetTypes().Where(t => t.IsSubclassOf(typeof(Modifier)) && !t.IsAbstract);

            foreach (var type in modifierTypes)
            {
                var modifierInstance = (Modifier)Activator.CreateInstance(type);
                _allModifiers[modifierInstance.Id] = type;
            }
        }

        public static Modifier Get(ModTypes id)
        {
            if (_allModifiers.TryGetValue(id, out var type))
                return (Modifier)Activator.CreateInstance(type);

            return null;
        }
    }
}