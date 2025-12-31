using Common.Network;

namespace Common.Database.Models;

public interface IDbSerializable
{
    void Write(NetworkWriter wtr);
    IDbSerializable Read(NetworkReader rdr);
}