namespace Common.Network.Messaging;

public interface IWritable
{
    void Write(NetworkWriter wtr);
}