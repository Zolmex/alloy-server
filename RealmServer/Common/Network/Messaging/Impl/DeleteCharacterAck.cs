using Common.Database;
using Common.Database.Models;
using Common.Utilities;
using System;

namespace Common.Network.Messaging.Impl;

public record struct DeleteCharacterAck : IAppMessageAck
{
    public AppMessageId MessageId => AppMessageId.DeleteCharacter;
    public int Sequence { get; set; }

    public bool Success { get; set; } 

    public DeleteCharacterAck(int seq)
    {
        Sequence = seq;
    }
    
    public void Write(NetworkWriter wtr)
    {
        wtr.Write(Success);
    }

    public void Read(NetworkReader rdr)
    {
        Success = rdr.ReadBoolean();
    }
}