#region

using System.Collections.Generic;

#endregion

namespace GameServer.Game.Entities.Stacks;

public class Stack
{
    protected readonly StackController _controller;

    protected readonly CharacterEntity _owner;
    private readonly List<float> StackDurations = new();
    public bool ToRemove;

    public Stack(CharacterEntity owner)
    {
        _owner = owner;
        _controller = owner.Stacks;
    }

    public ModStacks Type { get; private set; }

    public bool SharedDuration { get; set; } // Indicates if the last duration is the duration for all of the stacks of this type
    public int MaxCount { get; set; }
    public int Count { get; set; }
    public CharacterEntity LastGiver { get; set; }

    public int Timer { get; set; }

    public virtual void OnStackAdded(object[] additionalData) { }
    public virtual void OnStackRemoved() { }
    public virtual void OnAllStacksRemoved() { }
    public virtual void OnStackCountChanged(int count, CharacterEntity from, object[] additionalData) { }
    public virtual void OnTick(RealmTime time) { }
    public virtual void OnSpecialUpdate() { }

    public void InitStack(ModStacks type, int maxCount, bool sharedDuration = false)
    {
        Type = type;
        MaxCount = maxCount;
        SharedDuration = sharedDuration;
    }

    public void UpdateLastGiver(CharacterEntity from)
    {
        LastGiver = from == null ? LastGiver : LastGiver != from ? from : null;
    }

    public void AddStack(int amount, float duration, CharacterEntity from = null, object[] additionalData = null)
    {
        UpdateLastGiver(from);
        for (var i = 0; i < amount; i++)
        {
            if (Count < MaxCount) // Make sure we don't add too many of the same stack
            {
                if (SharedDuration)
                {
                    StackDurations.Clear();
                    StackDurations.Add(duration);
                }
                else
                {
                    StackDurations.Add(duration);
                }

                Count++;
                OnStackAdded(additionalData);
                CountChanged(Count, from, additionalData);
            }
        }
    }

    public void RemoveStack(int amount, CharacterEntity from = null) // Will remove X amount of stacks with the lowest duration left
    {
        UpdateLastGiver(from);
        if (!SharedDuration)
        {
            var toRemove = new int[amount]; // Save the indexes to remove later
            for (var i = StackDurations.Count - 1; i >= 0; i--)
            {
                if (StackDurations[i] == -1) // Don't remove infinite stacks
                    continue;

                for (var j = 0; j < amount; j++)
                {
                    if (toRemove[j] <= StackDurations[i]) // Only save if the duration is smaller
                        continue;

                    toRemove[j] = i;
                }
            }

            for (var i = 0; i < amount; ++i)
            {
                StackDurations.RemoveAt(toRemove[i]);
                Count--;
                OnStackRemoved();
                CountChanged(Count, from, null);

                if (Count == 0)
                    OnAllStacksRemoved();
            }
        }
        else // Only remove from the stack
        {
            for (var i = 0; i < amount; i++)
            {
                Count--;
                OnStackRemoved();
                CountChanged(Count, from, null);

                if (Count == 0)
                {
                    OnAllStacksRemoved();
                    StackDurations.Clear();
                    ToRemove = true;
                }
            }
        }
    }

    public bool Tick(RealmTime time) // Returns whether to remove this stack or not
    {
        if (ToRemove)
            return true;

        OnTick(time);

        for (var i = StackDurations.Count - 1; i >= 0; i--)
        {
            if (StackDurations[i] == -1) //infinite stack
                return Count <= 0;

            StackDurations[i] -= time.ElapsedMsDelta;
            if (StackDurations[i] <= 0)
                RemoveStack(SharedDuration ? Count : 1); // Duration ended, remove either all or just 1 stack, depending on SharedDuration
        }

        return Count <= 0;
    }

    public void CountChanged(int count, CharacterEntity from, object[] additionalData)
    {
        UpdateLastGiver(from);
        OnStackCountChanged(count, from, additionalData);
    }
}

public class PendingStackRemoval
{
    public PendingStackRemoval(ModStacks stackType, int count)
    {
        StackType = stackType;
        Count = count;
    }

    public ModStacks StackType { get; set; }
    public int Count { get; set; }
}

public class PendingStack
{
    public bool All;
    public int Count;
    public int Duration;
    public ModStacks StackId;

    public PendingStack(ModStacks stackId, int count = 1, int duration = 0, bool all = false)
    {
        StackId = stackId;
        Count = count;
        Duration = duration;
        All = all;
    }
}