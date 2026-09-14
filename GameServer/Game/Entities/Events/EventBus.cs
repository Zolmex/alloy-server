using Collections.Pooled;

namespace GameServer.Game.Entities.Events;

public delegate void EventHandler<T>(ref T evt) where T : struct;

public class EventBus<T> where T : struct {
    
    private readonly PooledList<EventHandler<T>> _handlers = [];

    public void Invoke(ref T evt) {
        foreach (var handler in _handlers)
            handler.Invoke(ref evt);
    }
    
    public void Add(EventHandler<T> handler) {
        _handlers.Add(handler);
    }

    public void Remove(EventHandler<T> handler) {
        _handlers.Remove(handler);
    }
}