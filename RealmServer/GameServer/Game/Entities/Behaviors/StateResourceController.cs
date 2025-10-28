#region

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

#endregion

namespace GameServer.Game.Entities.Behaviors
{
    public class StateResourceController
    {
        private readonly ConcurrentDictionary<int, object> _stateResources = new();

        public T ResolveResource<T>(IStateChild child)
        {
            var resource = GetResource(child);
            if (resource != null)
            {
                return (T)resource;
            }

            resource = Activator.CreateInstance<T>();
            InsertResource(child, resource);
            return (T)resource;
        }

        public void InsertResource(object instance, object resource)
        {
            _stateResources.TryAdd(instance.GetHashCode(), resource);
        }

        public object GetResource(object instance)
        {
            if (!_stateResources.TryGetValue(instance.GetHashCode(), out var resource))
                return null;
            return resource;
        }

        public void RemoveResource(object instance)
        {
            _stateResources.Remove(instance.GetHashCode(), out _);
        }

        public void ClearResources()
        {
            _stateResources.Clear();
        }
    }
}