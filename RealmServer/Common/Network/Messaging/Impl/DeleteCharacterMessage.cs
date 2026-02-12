using Common.Utilities;
using System;

namespace Common.Network.Messaging.Impl;

public record struct DeleteCharacterMessage : IAppMessage
{
    public AppMessageId MessageId => AppMessageId.DeleteCharacter;
    public int Sequence { get; set; }
    
    public int AccountId { get; set; }
    public int CharacterId { get; set; }

    public void Write(NetworkWriter wtr)
    {
        wtr.Write(AccountId);
        wtr.Write(CharacterId);
    }

    public void Read(NetworkReader rdr)
    {
        AccountId = rdr.ReadInt32();
        CharacterId = rdr.ReadInt32();
    }
}