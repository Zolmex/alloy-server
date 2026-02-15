using Common.Utilities;
using System;

namespace Common.Network.Messaging.Impl;

public record struct UnbanAccountMessage : IAppMessage
{
    public AppMessageId MessageId => AppMessageId.UnbanAccount;
    public int Sequence { get; set; }
    
    public string Name { get; set; }

    public void Write(NetworkWriter wtr)
    {
        wtr.Write(Name);
    }

    public void Read(NetworkReader rdr)
    {
        Name = rdr.ReadUTF();
    }
}