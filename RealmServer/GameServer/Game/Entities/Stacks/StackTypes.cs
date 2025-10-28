#region

using Common;
using System;
using System.Linq;

#endregion

namespace GameServer.Game.Entities.Stacks
{
    public enum ModStacks
    {
        Wrath,
        Venom,
        Onslaught,
        FadingFury,
        Untouched,
        Desecrated,
        Fury,
        Machinations,
        BloodReaper,
        SoulReaper,
        MindReaper,
        ConditionEffect
    }

    public class WrathStack : Stack
    {
        public const int MaxSpreads = 3;

        public WrathStack(Character owner) : base(owner)
        {
            InitStack(ModStacks.Wrath, int.MaxValue, true);

            if (!_controller.SpecialStackDatas.ContainsKey(Type))
                _controller.SpecialStackDatas.TryAdd(Type, new int[1] { 0 });
        }

        public override void OnStackCountChanged(int count, Character from, object[] additionalData)
        {
            if (count >= 3)
            {
                float wis = from != null && from is Player player ? player.Wisdom : 10;
                var damage = (int)(300 * (wis / 10));

                if (_owner.IsPlayer)
                    damage = Math.Min(damage, _owner.MaxHP / 10);

                _owner.DamageWithText(damage, 0x129431, 24, from);

                var removedStacks = _controller.RemoveStack(ModStacks.Wrath, all: true);
                _controller.AddStack(ModStacks.Venom, 3000 + (3000 - (3000 * wis / 100)), removedStacks, from);

                if (_owner.World == null || _controller.SpecialStackDatas[Type][0] == MaxSpreads)
                    return;

                _controller.SpecialStackDatas[Type][0] = Math.Min(_controller.SpecialStackDatas[Type][0] + 1, MaxSpreads);

                foreach (var entity in _owner.GetEnemiesWithin(2.5f).Where(_ => _ != _owner))
                    entity.Stacks.AddStack(ModStacks.Wrath, 5000, 1, _owner);

                foreach (var entity in _owner.GetPlayersWithin(2.5f).Where(_ => _ != _owner && _.ActiveModifiers.ContainsKey(ModTypes.NaturesVengeance)))
                    entity.Stacks.AddStack(ModStacks.Wrath, 5000, 1, _owner);
            }
        }
    }

    public class VenomStack : Stack
    {
        public VenomStack(Character owner) : base(owner)
        {
            InitStack(ModStacks.Venom, int.MaxValue, true);
        }

        public override void OnTick(RealmTime time)
        {
            Timer += time.ElapsedMsDelta;
            if (Timer >= 1000)
            {
                var dps = 100 * _controller.GetStackCount(ModStacks.Venom) / 6;
                if (_owner.IsPlayer)
                    dps = Math.Min(dps, _owner.MaxHP / 20);

                _owner.DamageWithText(dps, 0x129431, 24, LastGiver);
                Timer = 0;
            }
        }
    }

    public class OnslaughtStack : Stack
    {
        public static int TimeSinceHit = 1500;
        public static int TimeBetweenStacks = 200;
        public Player Player;

        public OnslaughtStack(Character owner) : base(owner)
        {
            Player = owner as Player;
            InitStack(ModStacks.Onslaught, 15, true);
        }

        public override void OnStackAdded(object[] a)
        {
            Player.AddIncreasedBonus(StatType.AttackSpeed, 3.3f);
            Player.AddIncreasedBonus(StatType.CriticalChance, 1.6f);
        }

        public override void OnStackRemoved()
        {
            Player.RemoveIncreasedBonus(StatType.AttackSpeed, 3.3f);
            Player.RemoveIncreasedBonus(StatType.CriticalChance, 1.6f);
        }

        public override void OnTick(RealmTime time)
        {
            if (Player.TimeSinceLastEnemyHit > TimeSinceHit)
            {
                Timer += time.ElapsedMsDelta;

                if (Timer >= TimeBetweenStacks)
                {
                    if (_controller.GetStackCount(ModStacks.Onslaught) - 1 == 0)
                        _controller.PendingStacks.Add(new PendingStack(ModStacks.FadingFury, 1, -1)); //duration is not at 0 which means a stack is being added

                    _controller.PendingStacks.Add(new PendingStack(ModStacks.Onslaught, 1)); //duration is left at 0 meaning a stack is being removed

                    Timer = 0;
                }
            }
        }
    }

    public class FadingFuryStack : Stack
    {
        public static int TimeSinceHit = 1500;
        public static int TimeBetweenStacks = 200;
        public Player Player;

        public FadingFuryStack(Character owner) : base(owner)
        {
            Player = owner as Player;
            InitStack(ModStacks.FadingFury, 15, true);
        }

        public override void OnStackAdded(object[] a)
        {
            Player.AddReducedBonus(StatType.AttackSpeed, 3.3f);
            Player.AddReducedBonus(StatType.CriticalChance, 1.6f);
        }

        public override void OnStackRemoved()
        {
            Player.RemoveReducedBonus(StatType.AttackSpeed, 3.3f);
            Player.RemoveReducedBonus(StatType.CriticalChance, 1.6f);
        }

        public override void OnTick(RealmTime time)
        {
            if (Player.TimeSinceLastEnemyHit > TimeSinceHit)
            {
                Timer += time.ElapsedMsDelta;

                if (Timer >= TimeBetweenStacks)
                {
                    if (_controller.GetStackCount(ModStacks.FadingFury) < 15)
                        _controller.AddStack(ModStacks.FadingFury, -1);

                    Timer = 0;
                }
            }
        }
    }

    public class UntouchedStack : Stack
    {
        public Player Player;

        public UntouchedStack(Character owner) : base(owner)
        {
            Player = owner as Player;
            InitStack(ModStacks.Untouched, 1, false);
        }

        public override void OnStackAdded(object[] a)
        {
            Player.AddFlatBonus(StatType.CriticalDamage, 50);
        }

        public override void OnStackRemoved()
        {
            Player.RemoveFlatBonus(StatType.CriticalDamage, 50);
        }
    }

    public class DesecratedStack : Stack
    {
        public float DesecratedScale = 1f;
        public Player Player;

        public DesecratedStack(Character owner) : base(owner)
        {
            Player = owner as Player;
            InitStack(ModStacks.Desecrated, 1, false);

            SetDesecratedScale(Player);
        }

        public override void OnStackAdded(object[] a)
        {
            DoBonuses(Player, true);
        }

        public override void OnStackRemoved()
        {
            DoBonuses(Player, false);
        }

        public override void OnSpecialUpdate()
        {
            DoBonuses(Player, false);
            SetDesecratedScale(Player);
            DoBonuses(Player, true);
        }

        public void DoBonuses(Player player, bool add)
        {
            if (add)
            {
                player.AddIncreasedBonus(StatType.LifeRegeneration, -DesecratedScale);
                player.AddIncreasedBonus(StatType.Defense, -DesecratedScale);
                player.AddIncreasedBonus(StatType.MovementSpeed, -DesecratedScale);
            }
            else
            {
                player.RemoveIncreasedBonus(StatType.LifeRegeneration, -DesecratedScale);
                player.RemoveIncreasedBonus(StatType.Defense, -DesecratedScale);
                player.RemoveIncreasedBonus(StatType.MovementSpeed, -DesecratedScale);
            }
        }

        public void SetDesecratedScale(Player player)
        {
            DesecratedScale = (1f - ((float)player.HP / player.MaxHP)) * 100;
        }
    }

    public class FuryStack : Stack
    {
        public Player Player;

        public FuryStack(Character owner) : base(owner)
        {
            Player = owner as Player;
            InitStack(ModStacks.Fury, 1000, false);
        }

        public override void OnStackAdded(object[] a)
        {
            Player.AddFlatBonus(StatType.DamageMultiplier, GetAddAmt());
        }

        public override void OnStackRemoved()
        {
            Player.RemoveFlatBonus(StatType.DamageMultiplier, GetAddAmt());
        }

        public int GetAddAmt()
        {
            return Count > 20 ? Count % (Count / 10) == 1 ? 1 : 0 : 1;
        }
    }

    public class MachinationsStack : Stack
    {
        public Player Player;

        public MachinationsStack(Character owner) : base(owner)
        {
            Player = owner as Player;
            InitStack(ModStacks.Machinations, 1000, false);
        }

        public override void OnStackAdded(object[] a)
        {
            Player.AddIncreasedBonus(StatType.ManaRegeneration, 10);
        }

        public override void OnStackRemoved()
        {
            Player.RemoveIncreasedBonus(StatType.ManaRegeneration, 10);
        }
    }

    public class BloodReaperStack : Stack
    {
        public Player Player;

        public BloodReaperStack(Character owner) : base(owner)
        {
            Player = owner as Player;
            InitStack(ModStacks.BloodReaper, 1000, false);
        }

        public override void OnStackAdded(object[] a)
        {
            Player.AddIncreasedBonus(StatType.AttackSpeed, 2);
        }

        public override void OnStackRemoved()
        {
            Player.StacksLostInvoke(ModStacks.BloodReaper, 1);
            Player.RemoveIncreasedBonus(StatType.AttackSpeed, 2);
        }
    }

    public class SoulReaperStack : Stack
    {
        public Player Player;

        public SoulReaperStack(Character owner) : base(owner)
        {
            Player = owner as Player;
            InitStack(ModStacks.SoulReaper, 1000, false);
        }

        public override void OnStackAdded(object[] a)
        {
            Player.AddIncreasedBonus(StatType.CriticalChance, 2);
        }

        public override void OnStackRemoved()
        {
            Player.StacksLostInvoke(ModStacks.SoulReaper, 1);
            Player.RemoveIncreasedBonus(StatType.CriticalChance, 2);
        }
    }

    public class MindReaperStack : Stack
    {
        public Player Player;
        public int Scale;

        public MindReaperStack(Character owner) : base(owner)
        {
            Player = owner as Player;
            InitStack(ModStacks.MindReaper, 1000, false);

            Scale = GetScale();
            DoBonuses(true);
        }

        public override void OnStackAdded(object[] a)
        {
            UpdateBonuses();
        }

        public override void OnStackRemoved()
        {
            Player.StacksLostInvoke(ModStacks.MindReaper, 1);
            UpdateBonuses();
        }

        public override void OnAllStacksRemoved()
        {
            DoBonuses(false);
        }

        public void UpdateBonuses()
        {
            DoBonuses(false);
            Scale = GetScale();
            DoBonuses(true);
        }

        public void DoBonuses(bool add)
        {
            if (add)
                Player.AddIncreasedBonus(StatType.DamageMultiplier, Scale);
            else
                Player.RemoveIncreasedBonus(StatType.DamageMultiplier, Scale);
        }

        public int GetScale()
        {
            return 1 * (Player.MP / Player.MaxMP) * Count;
        }
    }

    // Based on Slendergo's ConditionEffectManager
    // https://github.com/kevinbudz/betterSkillys/blob/main/source/WorldServer/core/objects/ConditionEffectManager.cs
    public class ConditionEffectStack : Stack
    {
        private const byte CE_FIRST_BATCH = 0;
        private const byte CE_SECOND_BATCH = 1;
        private const byte NUMBER_CE_BATCHES = 2;
        private const byte NEW_CON_THREASHOLD = 32;

        private readonly int[] _masks;
        private readonly int[] _durations;

        public ConditionEffectStack(Character owner) : base(owner)
        {
            InitStack(ModStacks.ConditionEffect, 1);

            _masks = new int[NUMBER_CE_BATCHES];
            _durations = new int[(int)ConditionEffectIndex.ConditionCount];
        }

        public override void OnTick(RealmTime time)
        {
            var dt = time.ElapsedMsDelta;
            if (_masks[CE_FIRST_BATCH] == 0 && _masks[CE_SECOND_BATCH] == 0)
                return;

            for (byte effect = 0; effect < _durations.Length; effect++)
            {
                var duration = _durations[effect];
                if (duration == -1)
                    continue;

                if (duration <= dt)
                {
                    Remove(effect);
                    continue;
                }

                _durations[effect] -= dt;
            }
        }

        private void UpdateConditionStat(byte batchType)
        {
            switch (batchType)
            {
                case CE_FIRST_BATCH:
                    _owner.Condition1 = _masks[CE_FIRST_BATCH];
                    break;
                case CE_SECOND_BATCH:
                    _owner.Condition2 = _masks[CE_SECOND_BATCH];
                    break;
            }
        }

        public void ApplyConditionEffect(ConditionEffectIndex effIndex, int durationMS)
        {
            var effect = (byte)effIndex;
            if (_durations[effect] == -1) // Permanent
                return;

            _durations[effect] = durationMS == -1 ? durationMS : Math.Max(_durations[effect], durationMS);

            var batchType = GetBatch(effect);
            _masks[batchType] |= GetBit(effect);

            UpdateConditionStat(batchType);
        }

        public void RemoveConditionEffect(ConditionEffectIndex effIndex)
        {
            Remove((byte)effIndex);
        }

        private void Remove(byte effect)
        {
            _durations[effect] = 0;

            var batchType = GetBatch(effect);
            _masks[batchType] &= ~GetBit(effect);
            UpdateConditionStat(batchType);
        }

        public bool HasConditionEffect(ConditionEffectIndex effIndex)
        {
            var effect = (byte)effIndex;
            return (_masks[GetBatch(effect)] & GetBit(effect)) != 0;
        }

        private static int GetBit(int effect)
        {
            return 1 << (effect - (IsNewCondThreshold(effect) ? NEW_CON_THREASHOLD * (effect / 32) : 1));
        }

        private static bool IsNewCondThreshold(int effect)
        {
            return effect >= NEW_CON_THREASHOLD;
        }

        private static byte GetBatch(int effect)
        {
            return (byte)(effect / 32);
        }
    }

    public static class StackGetter
    {
        public static Stack ResolveStack(Character owner, ModStacks stackId)
        {
            var className = typeof(StackGetter).Namespace + "." + stackId + "Stack";

            var stackType = Type.GetType(className);
            if (stackType == null)
                throw new ArgumentException($"Stack type {className} not found.", nameof(stackId));

            var stack = (Stack)Activator.CreateInstance(stackType, owner);
            if (stack == null)
                throw new ArgumentException($"Could not create stack type {className}.", nameof(stackId));

            return stack;
        }
    }
}