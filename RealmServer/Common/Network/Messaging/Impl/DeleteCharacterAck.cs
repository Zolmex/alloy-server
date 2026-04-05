using Common.Database;
using Common.Database.Models;
using Common.Utilities;
using System;
using System.IO;

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
    
    public void Write(ref SpanWriter wtr)
    {
        wtr.Write(Success);
    }

    public void Read(ref SpanReader rdr)
    {
        Success = rdr.ReadBoolean();
    }
}