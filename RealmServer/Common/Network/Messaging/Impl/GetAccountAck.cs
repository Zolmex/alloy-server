using Common.Database;
using Common.Database.Models;
using Common.Utilities;
using System;
using System.IO;

namespace Common.Network.Messaging.Impl;

public record struct GetAccountAck : IAppMessageAck
{
    public AppMessageId MessageId => AppMessageId.GetAccount;
    public int Sequence { get; set; }

    public Account Account { get; set; } 

    public GetAccountAck(int seq)
    {
        Sequence = seq;
    }
    
    public void Write(ref SpanWriter wtr)
    {
        var hasValue = Account != null;
        wtr.Write(hasValue);
        if (hasValue)
            Account.WriteProperties(ref wtr);
    }

    public void Read(ref SpanReader rdr)
    {
        Account = DbModel.Read<Account>(ref rdr);
    }
}