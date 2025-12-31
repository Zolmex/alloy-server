using Common.Database;
using Common.Database.Models;
using Common.Utilities;
using System;

namespace Common.Network.Messaging.Impl;

public record struct VerifyMessageAck : IAppMessageAck
{
    public AppMessageId MessageId => AppMessageId.Verify;
    public int Sequence { get; set; }

    public VerifyStatus Status { get; set; }
    public Account Account { get; set; } 

    public void Write(NetworkWriter wtr)
    {
        wtr.Write((byte)Status);
        Account.Write(wtr);
    }

    public void Read(NetworkReader rdr)
    {
        Status = (VerifyStatus)rdr.ReadByte();
        Account = (Account)Account.Read(rdr);
    }
}