using System;
using Common.Utilities.Collections;

namespace Common.Utilities;

public interface IEntityIdentifiable : IEquatable<IEntityIdentifiable> {
    EntityId Id { get; set; }
    
    bool IEquatable<IEntityIdentifiable>.Equals(IEntityIdentifiable other) {
        return Id == other?.Id;
    }
    
    bool Equals(object obj) => obj is IEntityIdentifiable other && Equals(other);
    
    int GetHashCode() => Id.GetHashCode();
}