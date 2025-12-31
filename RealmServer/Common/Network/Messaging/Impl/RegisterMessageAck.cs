using Common.Utilities;
using System;

namespace Common.Network.Messaging.Impl;

public record struct RegisterMessageAck : IAppMessageAck
{
    public AppMessageId MessageId => AppMessageId.Register;
    public int Sequence { get; set; }

    public RegisterStatus Status { get; set; }

    public void Write(NetworkWriter wtr)
    {
        wtr.Write((byte)Status);
    }

    public void Read(NetworkReader rdr)
    {
        Status = (RegisterStatus)rdr.ReadByte();
    }
}