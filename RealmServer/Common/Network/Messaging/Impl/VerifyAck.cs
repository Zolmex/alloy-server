using Common.Database;
using Common.Database.Models;
using Common.Utilities;
using System;
using System.IO;

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
    
    public void Write(ref SpanWriter wtr)
    {
        wtr.Write((byte)Status);
        if (Status == VerifyStatus.Success)
        {
            wtr.Write(true);
            Account.WriteProperties(ref wtr);
        }
    }

    public void Read(ref SpanReader rdr)
    {
        Status = (VerifyStatus)rdr.ReadByte();
        if (Status == VerifyStatus.Success)
        {
            Account = DbModel.Read<Account>(ref rdr);
        }
    }
}