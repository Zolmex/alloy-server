namespace Common.Network.Messaging.Impl;

public record struct RegisterMessage : IAppMessage {
    public string Username { get; set; }
    public string IPAddress { get; set; }
    public string Password { get; set; }
    public AppMessageId MessageId => AppMessageId.Register;
    public int Sequence { get; set; }

    public void Write(ref SpanWriter wtr) {
        wtr.WriteUTF(Username);
        wtr.WriteUTF(IPAddress);
        wtr.WriteUTF(Password);
    }

    public void Read(ref SpanReader rdr) {
        Username = rdr.ReadUTF();
        IPAddress = rdr.ReadUTF();
        Password = rdr.ReadUTF();
    }
}