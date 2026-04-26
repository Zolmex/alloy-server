namespace Common.Network.Messaging.Impl;

public record struct CreateCharacterMessage : IAppMessage {
    public int AccountId { get; set; }
    public ushort ClassType { get; set; }
    public ushort SkinType { get; set; }
    public AppMessageId MessageId => AppMessageId.CreateCharacter;
    public int Sequence { get; set; }

    public void Write(ref SpanWriter wtr) {
        wtr.Write(AccountId);
        wtr.Write(ClassType);
        wtr.Write(SkinType);
    }

    public void Read(ref SpanReader rdr) {
        AccountId = rdr.ReadInt32();
        ClassType = rdr.ReadUInt16();
        SkinType = rdr.ReadUInt16();
    }
}