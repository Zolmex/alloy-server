using System;
using System.Collections.Generic;

namespace Common.Utilities;

public class EventBus<T>
{
    private readonly Dictionary<int, Action<T>> _handlers = new();
    private int _nextId = 0;

    public int Subscribe(Action<T> handler)
    {
        int id = _nextId++;
        _handlers[id] = handler;
        return id;
    }

    public void Unsubscribe(int id)
    {
        _handlers.Remove(id);
    }

    public void Publish(T data)
    {
        foreach (var handler in _handlers.Values)
            handler(data);
    }
}