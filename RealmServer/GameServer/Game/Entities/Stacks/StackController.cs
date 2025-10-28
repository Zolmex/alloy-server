#region

using System.Collections.Concurrent;
using System.Collections.Generic;

#endregion

namespace GameServer.Game.Entities.Stacks
{
    public class StackController
    {
        public ConcurrentDictionary<ModStacks, Stack> AllStacks = new();
        public ConcurrentDictionary<ModStacks, int[]> SpecialStackDatas = new();
        public List<PendingStack> PendingStacks = new();

        private readonly Character _host;

        public StackController(Character host)
        {
            _host = host;
        }

        public void Update(RealmTime time)
        {
            foreach (var stack in
                     PendingStacks) //only need to use PendingStacks if you are adding or removing stacks in the middle of enumeration (aka during tick)
            {
                if (stack.Duration == 0)
                    RemoveStack(stack.StackId, stack.Count, stack.All);
                else
                    AddStack(stack.StackId, stack.Duration, stack.Count);
            }

            PendingStacks.Clear();

            var toRemove = new List<ModStacks>();
            foreach (var stack in AllStacks.Values)
                if (stack.Tick(time))
                {
                    stack.OnAllStacksRemoved();
                    toRemove.Add(stack.Type);
                }

            toRemove.ForEach(i => AllStacks.Remove(i, out _));
        }

        public int GetStackCount(ModStacks stackId)
        {
            var currentStacks = 0;
            if (AllStacks.TryGetValue(stackId, out var stack))
                currentStacks = stack.Count;

            return currentStacks;
        }

        public Stack GetStack(ModStacks stackId)
        {
            if (!AllStacks.TryGetValue(stackId, out var stack))
                return null;
            return stack;
        }

        public void AddStack(ModStacks stackId, float duration, int amount = 1, Character from = null,
            params object[] additionalData)
        {
            if (!AllStacks.TryGetValue(stackId, out var stack))
            {
                stack = StackGetter.ResolveStack(_host, stackId);
                AllStacks.TryAdd(stackId, stack);
            }

            stack.AddStack(amount, duration, from, additionalData);
        }

        public void UpdateStack(ModStacks stackId)
        {
            if (AllStacks.TryGetValue(stackId, out var stack))
            {
                stack.OnSpecialUpdate();
            }
        }

        public int RemoveStack(ModStacks stackId, int amount = 1, bool all = false)
        {
            var removedStacks = -1;
            if (AllStacks.TryGetValue(stackId, out var stack))
            {
                removedStacks = all ? stack.Count : amount;
                stack.RemoveStack(removedStacks);
            }

            return removedStacks;
        }

        public void RemoveAllStacks()
        {
            foreach (var stack in AllStacks)
                stack.Value.RemoveStack(stack.Value.Count);
        }
    }
}