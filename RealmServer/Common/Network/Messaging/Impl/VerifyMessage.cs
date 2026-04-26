namespace Common.Network.Messaging.Impl;

public record struct VerifyMessage : IAppMessage {
    public string Username { get; set; }
    public string Password { get; set; }
    public AppMessageId MessageId => AppMessageId.Verify;
    public int Sequence { get; set; }

    public void Write(ref SpanWriter wtr) {
        wtr.WriteUTF(Username);
        wtr.WriteUTF(Password);
    }

    public void Read(ref SpanReader rdr) {
        Username = rdr.ReadUTF();
        Password = rdr.ReadUTF();
    }
}