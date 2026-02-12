using Common.Utilities;
using System;

namespace Common.Network.Messaging.Impl;

public record struct GetAccountByNameMessage : IAppMessage
{
    public AppMessageId MessageId => AppMessageId.GetAccount;
    public int Sequence { get; set; }
    
    public string Name { get; set; }
    public int AccountId { get; set; }

    public void Write(NetworkWriter wtr)
    {
        wtr.Write(Name ?? "");
        wtr.Write(AccountId);
    }

    public void Read(NetworkReader rdr)
    {
        Name = rdr.ReadUTF();
        AccountId = rdr.ReadInt32();
    }
}