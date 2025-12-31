using Common.Utilities;
using System;

namespace Common.Network.Messaging.Impl;

public record struct RegisterMessage : IAppMessage
{
    public AppMessageId MessageId => AppMessageId.Register;
    public int Sequence { get; set; }
    
    public string Username { get; set; }
    public string IPAddress { get; set; }
    public string Password { get; set; }

    public void Write(NetworkWriter wtr)
    {
        wtr.Write(Username);
        wtr.Write(IPAddress);
        wtr.Write(Password);
    }

    public void Read(NetworkReader rdr)
    {
        Username = rdr.ReadUTF();
        IPAddress = rdr.ReadUTF();
        Password = rdr.ReadUTF();
    }
}