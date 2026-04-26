namespace Common.Network.Messaging.Impl;

public record struct RegisterAck : IAppMessageAck {
    public RegisterAck(int seq) {
        Sequence = seq;
    }

    public RegisterStatus Status { get; set; }
    public AppMessageId MessageId => AppMessageId.Register;
    public int Sequence { get; set; }

    public void Write(ref SpanWriter wtr) {
        wtr.Write((byte)Status);
    }

    public void Read(ref SpanReader rdr) {
        Status = (RegisterStatus)rdr.ReadByte();
    }
}