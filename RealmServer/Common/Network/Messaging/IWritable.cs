namespace Common.Network.Messaging;

public interface IWritable {
    void Write(ref SpanWriter wtr);
}