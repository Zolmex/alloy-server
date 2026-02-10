using Common.Database;
using Common.Database.Models;
using Common.Utilities;
using System;

namespace Common.Network.Messaging.Impl;

public record struct VerifyAck : IAppMessageAck
{
    public AppMessageId MessageId => AppMessageId.Verify;
    public int Sequence { get; set; }

    public VerifyStatus Status { get; set; }
    public Account Account { get; set; } 

    public VerifyAck(int seq)
    {
        Sequence = seq;
    }
    
    public void Write(NetworkWriter wtr)
    {
        wtr.Write((byte)Status);
        if (Status == VerifyStatus.Success)
        {
            wtr.Write(true);
            Account.WriteProperties(wtr);
        }
    }

    public void Read(NetworkReader rdr)
    {
        Status = (VerifyStatus)rdr.ReadByte();
        if (Status == VerifyStatus.Success)
        {
            Account = DbModel.Read<Account>(rdr);
        }
    }
}