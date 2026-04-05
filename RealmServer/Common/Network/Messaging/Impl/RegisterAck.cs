using Common.Utilities;
using System;
using System.IO;

namespace Common.Network.Messaging.Impl;

public record struct RegisterAck : IAppMessageAck
{
    public AppMessageId MessageId => AppMessageId.Register;
    public int Sequence { get; set; }

    public RegisterStatus Status { get; set; }

    public RegisterAck(int seq)
    {
        Sequence = seq;
    }
    
    public void Write(ref SpanWriter wtr)
    {
        wtr.Write((byte)Status);
    }

    public void Read(ref SpanReader rdr)
    {
        Status = (RegisterStatus)rdr.ReadByte();
    }
}