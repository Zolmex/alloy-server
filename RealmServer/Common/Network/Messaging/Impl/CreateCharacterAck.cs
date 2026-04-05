using Common.Database;
using Common.Database.Models;
using Common.Utilities;
using System;
using System.IO;

namespace Common.Network.Messaging.Impl;

public record struct CreateCharacterAck : IAppMessageAck
{
    public AppMessageId MessageId => AppMessageId.CreateCharacter;
    public int Sequence { get; set; }

    public CreateCharacterStatus Status { get; set; } 
    public Character Character { get; set; } 

    public CreateCharacterAck(int seq)
    {
        Sequence = seq;
    }
    
    public void Write(ref SpanWriter wtr)
    {
        wtr.Write((byte)Status);
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
        Status = (CreateCharacterStatus)rdr.ReadByte();
        Character = DbModel.Read<Character>(ref rdr);
    }
}