using System;

namespace Common.Utilities;

public interface IIdentifiable : IEquatable<IIdentifiable>
{
    int Id { get; set; }
    
    bool IEquatable<IIdentifiable>.Equals(IIdentifiable other) {
        return Id == other?.Id;
    }
    
    bool Equals(object obj) => obj is IIdentifiable other && Equals(other);
    
    int GetHashCode() => Id.GetHashCode();
}