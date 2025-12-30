using Common.Utilities;
using System;

namespace Common.Network.Messaging.Impl;

public record struct HelloMessage : IAppMessage
{
    public AppMessageId MessageId => AppMessageId.HELLO;
    public string AppName { get; set; }

    public void Write(NetworkWriter wtr)
    {
        wtr.Write(AppName);
    }

    public void Read(NetworkReader rdr)
    {
        AppName = rdr.ReadUTF();
    }
}