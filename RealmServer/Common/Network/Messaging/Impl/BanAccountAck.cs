using Common.Database;
using Common.Database.Models;
using Common.Utilities;
using System;

namespace Common.Network.Messaging.Impl;

public record struct BanAccountAck : IAppMessageAck
{
    public AppMessageId MessageId => AppMessageId.BanAccount;
    public int Sequence { get; set; }

    public bool Success { get; set; } 
    public string Error { get; set; } 

    public BanAccountAck(int seq)
    {
        Sequence = seq;
    }
    
    public void Write(NetworkWriter wtr)
    {
        wtr.Write(Success);
        wtr.Write(Error);
    }

    public void Read(NetworkReader rdr)
    {
        Success = rdr.ReadBoolean();
        Error = rdr.ReadUTF();
    }
}