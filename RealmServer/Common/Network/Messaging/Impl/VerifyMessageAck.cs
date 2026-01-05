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
        if (Status == VerifyStatus.Success)
            Account.Write(wtr);
        else wtr.Write((byte)0);
    }

    public void Read(NetworkReader rdr)
    {
        Status = (VerifyStatus)rdr.ReadByte();
        if (Status == VerifyStatus.Success)
            Account = Account.Read(rdr);
    }
}