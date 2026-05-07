using System;
using System.Collections.Generic;

namespace GameServer.Game.Entities.Events;

public delegate void RefAction<T>(in T data);

public class EventBus<T>
{
    private readonly List<RefAction<T>> _handlers = new();

    public void Subscribe(RefAction<T> handler)
    {
        _handlers.Add(handler);
    }

    public void Unsubscribe(RefAction<T> handler)
    {
        _handlers.Remove(handler);
    }

    public void Publish(in T data)
    {
        foreach (var handler in _handlers)
            handler(data);
    }
}