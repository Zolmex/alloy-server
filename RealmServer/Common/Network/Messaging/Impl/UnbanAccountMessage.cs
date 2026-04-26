namespace Common.Network.Messaging.Impl;

public record struct UnbanAccountMessage : IAppMessage {
    public string Name { get; set; }
    public AppMessageId MessageId => AppMessageId.UnbanAccount;
    public int Sequence { get; set; }

    public void Write(ref SpanWriter wtr) {
        wtr.WriteUTF(Name);
    }

    public void Read(ref SpanReader rdr) {
        Name = rdr.ReadUTF();
    }
}