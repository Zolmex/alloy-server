namespace Common.Network;

public interface IWritable {
    void Write(ref SpanWriter wtr);
}