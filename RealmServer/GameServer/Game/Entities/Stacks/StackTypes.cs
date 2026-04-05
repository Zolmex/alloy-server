#region

using Common;
using System;
using System.Linq;

#endregion

namespace GameServer.Game.Entities.Stacks;

public enum ModStacks
{
    ConditionEffect
}

// Based on Slendergo's ConditionEffectManager
// https://github.com/kevinbudz/betterSkillys/blob/main/source/WorldServer/core/objects/ConditionEffectManager.cs
public class ConditionEffectStack : Stack
{
    private const byte CE_FIRST_BATCH = 0;
    private const byte CE_SECOND_BATCH = 1;
    private const byte NUMBER_CE_BATCHES = 2;
    private const byte NEW_CON_THREASHOLD = 32;
    private readonly int[] _durations;

    private readonly int[] _masks;

    public ConditionEffectStack(CharacterEntity owner) : base(owner)
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
    public static Stack ResolveStack(CharacterEntity owner, ModStacks stackId)
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