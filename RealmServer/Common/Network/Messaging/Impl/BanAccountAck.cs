namespace Common.Network.Messaging.Impl;

public record struct BanAccountAck : IAppMessageAck {
    public BanAccountAck(int seq) {
        Sequence = seq;
    }

    public bool Success { get; set; }
    public string Error { get; set; }
    public AppMessageId MessageId => AppMessageId.BanAccount;
    public int Sequence { get; set; }

    public void Write(ref SpanWriter wtr) {
        wtr.Write(Success);
        wtr.WriteUTF(Error);
    }

    public void Read(ref SpanReader rdr) {
        Success = rdr.ReadBoolean();
        Error = rdr.ReadUTF();
    }
}