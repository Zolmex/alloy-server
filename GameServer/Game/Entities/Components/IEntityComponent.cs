using Common.Utilities;

namespace GameServer.Game.Entities.Components;

public interface IEntityComponent : IIdentifiable, IDisposable, IEquatable<IEntityComponent> {
    bool IEquatable<IEntityComponent>.Equals(IEntityComponent other) {
        return Id == other?.Id;
    }
    
    bool Equals(object obj) => obj is IEntityComponent other && Equals(other);
    
    int GetHashCode() => Id.GetHashCode();
}