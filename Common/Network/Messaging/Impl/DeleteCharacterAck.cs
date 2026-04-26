namespace Common.Network.Messaging.Impl;

public record struct DeleteCharacterAck : IAppMessageAck {
    public DeleteCharacterAck(int seq) {
        Sequence = seq;
    }

    public bool Success { get; set; }
    public AppMessageId MessageId => AppMessageId.DeleteCharacter;
    public int Sequence { get; set; }

    public void Write(ref SpanWriter wtr) {
        wtr.Write(Success);
    }

    public void Read(ref SpanReader rdr) {
        Success = rdr.ReadBoolean();
    }
}