using Common.Utilities;
using System;

namespace Common.Network.Messaging.Impl;

public record struct VerifyMessage : IAppMessage
{
    public AppMessageId MessageId => AppMessageId.Verify;
    public int Sequence { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }

    public void Write(NetworkWriter wtr)
    {
        wtr.Write(Username);
        wtr.Write(Password);
    }

    public void Read(NetworkReader rdr)
    {
        Username = rdr.ReadUTF();
        Password = rdr.ReadUTF();
    }
}