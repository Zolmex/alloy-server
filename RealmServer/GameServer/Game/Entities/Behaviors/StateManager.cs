#region

using System;
using System.Collections.Generic;

#endregion

namespace GameServer.Game.Entities.Behaviors
{
    public class StateManager
    {
        public Dictionary<Enum, Action<RealmTime, Character, StateTick>> States = new();
        private Dictionary<int, Enum> _currentStates = new();

        public void RegisterEntity(Entity ent)
        {
            _currentStates.Add(ent.Id, null);
        }

        public void UnregisterEntity(Entity ent)
        {
            _currentStates.Remove(ent.Id);
        }

        public void RegisterState(Enum state, Action<RealmTime, Character, StateTick> action)
        {
            States.Add(state, action);
        }

        public void Transition(RealmTime time, Character owner, Enum targetState)
        {
            EndCurrentState(owner, time);
            SetCurrentState(owner, targetState);
            StartCurrentState(owner, time);
        }

        public Enum GetCurrentState(Character owner)
        {
            return _currentStates[owner.Id];
        }

        public void SetCurrentState(Character owner, Enum targetState)
        {
            _currentStates[owner.Id] = targetState;
        }

        public void StartCurrentState(Character owner, RealmTime time)
        {
            var state = GetCurrentState(owner);
            if (state == null || !States.ContainsKey(state) || States[state] == null) return;
            States[state]?.Invoke(time, owner, StateTick.Start);
        }

        public void Tick(Character owner, RealmTime time)
        {
            var state = GetCurrentState(owner);
            if (state == null || !States.ContainsKey(state) || States[state] == null) return;
            States[state]?.Invoke(time, owner, StateTick.Tick);
        }

        public void EndCurrentState(Character owner, RealmTime time)
        {
            var state = GetCurrentState(owner);
            if (state == null || !States.ContainsKey(state) || States[state] == null) return;
            States[state]?.Invoke(time, owner, StateTick.End);
            owner.StateResources.ClearResources();
        }


        public bool CheckTransition<T>(BehaviorTransition transition, Character owner, RealmTime time) where T : struct, Enum
        {
            var transResult = transition.Tick(owner, time);
            if (!string.IsNullOrEmpty(transResult))
            {
                Transition(time, owner, GetStateFromString<T>(transResult));
                return true;
            }

            return false;
        }

        public static T GetStateFromString<T>(string str) where T : struct
        {
            try
            {
                var res = (T)Enum.Parse(typeof(T), str);
                if (!Enum.IsDefined(typeof(T), res)) return default;
                return res;
            }
            catch
            {
                return default;
            }
        }
    }

    public enum StateTick
    {
        Start,
        Tick,
        End
    }
}