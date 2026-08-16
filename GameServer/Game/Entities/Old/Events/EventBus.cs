using System;
using System.Collections.Generic;

namespace GameServer.Game.Entities.Events;

public delegate void RefAction<T>(ref T data);

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

    public void Publish(ref T data)
    {
        foreach (var handler in _handlers)
            handler(ref data);
    }
}