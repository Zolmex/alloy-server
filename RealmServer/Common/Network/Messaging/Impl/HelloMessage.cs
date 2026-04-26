namespace Common.Network.Messaging.Impl;

public record struct HelloMessage : IAppMessage {
    public string AppName { get; set; }
    public AppMessageId MessageId => AppMessageId.Hello;
    public int Sequence { get; set; }

    public void Write(ref SpanWriter wtr) {
        wtr.WriteUTF(AppName);
    }

    public void Read(ref SpanReader rdr) {
        AppName = rdr.ReadUTF();
    }
}