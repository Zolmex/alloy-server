using Common.Database;
using Common.Database.Models;
using Common.Utilities;
using System;

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
    
    public void Write(NetworkWriter wtr)
    {
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
        if (rdr.ReadBoolean())
            Character = DbModel.Read<Character>(rdr);
    }
}