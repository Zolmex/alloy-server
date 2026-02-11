using Common.Database;
using Common.Database.Models;
using Common.Utilities;
using System;

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
    
    public void Write(NetworkWriter wtr)
    {
        wtr.Write((byte)Status);
        if (Character == null)
            wtr.Write(false);
        else
        {
            wtr.Write(true);
            Character.WriteProperties(wtr);
        }
    }

    public void Read(NetworkReader rdr)
    {
        Status = (CreateCharacterStatus)rdr.ReadByte();
        Character = DbModel.Read<Character>(rdr);
    }
}