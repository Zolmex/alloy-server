using Common.Database;
using Common.Database.Models;
using Common.Utilities;
using System;
using System.IO;

namespace Common.Network.Messaging.Impl;

public record struct GetCharacterAck : IAppMessageAck
{
    public AppMessageId MessageId => AppMessageId.GetCharacter;
    public int Sequence { get; set; }

    public Character Character { get; set; } 

    public GetCharacterAck(int seq)
    {
        Sequence = seq;
    }
    
    public void Write(ref SpanWriter wtr)
    {
        if (Character == null)
            wtr.Write(false);
        else
        {
            wtr.Write(true);
            Character.WriteProperties(ref wtr);
        }
    }

    public void Read(ref SpanReader rdr)
    {
        Character = DbModel.Read<Character>(ref rdr);
    }
}