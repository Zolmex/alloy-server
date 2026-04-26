namespace Common.Network.Messaging.Impl;

public record struct FlushAck : IAppMessageAck {
    public FlushAck(int seq) {
        Sequence = seq;
    }

    public FlushStatus Status { get; set; }
    public AppMessageId MessageId => AppMessageId.Flush;
    public int Sequence { get; set; }

    public void Write(ref SpanWriter wtr) {
        wtr.Write((byte)Status);
    }

    public void Read(ref SpanReader rdr) {
        Status = (FlushStatus)rdr.ReadByte();
    }
}