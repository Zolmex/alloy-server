using Common.Database;
using Common.Database.Models;

namespace Common.Network.Messaging.Impl;

public record struct CreateCharacterAck : IAppMessageAck {
    public CreateCharacterAck(int seq) {
        Sequence = seq;
    }

    public CreateCharacterStatus Status { get; set; }
    public Character Character { get; set; }
    public AppMessageId MessageId => AppMessageId.CreateCharacter;
    public int Sequence { get; set; }

    public void Write(ref SpanWriter wtr) {
        wtr.Write((byte)Status);
        if (Character == null) {
            wtr.Write(false);
        }
        else {
            wtr.Write(true);
            Character.WriteProperties(ref wtr);
        }
    }

    public void Read(ref SpanReader rdr) {
        Status = (CreateCharacterStatus)rdr.ReadByte();
        Character = DbModel.Read<Character>(ref rdr);
    }
}