using Common.Utilities;
using System;
using System.IO;

namespace Common.Network.Messaging.Impl;

public record struct GetAccountByNameMessage : IAppMessage
{
    public AppMessageId MessageId => AppMessageId.GetAccount;
    public int Sequence { get; set; }
    
    public string Name { get; set; }
    public int AccountId { get; set; }

    public void Write(ref SpanWriter wtr)
    {
        wtr.WriteUTF(Name ?? "");
        wtr.Write(AccountId);
    }

    public void Read(ref SpanReader rdr)
    {
        Name = rdr.ReadUTF();
        AccountId = rdr.ReadInt32();
    }
}