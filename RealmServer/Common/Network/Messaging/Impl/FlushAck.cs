using Common.Utilities;
using System;

namespace Common.Network.Messaging.Impl;

public record struct FlushAck : IAppMessageAck
{
    public AppMessageId MessageId => AppMessageId.Flush;
    public int Sequence { get; set; }

    public FlushStatus Status { get; set; }

    public FlushAck(int seq)
    {
        Sequence = seq;
    }
    
    public void Write(NetworkWriter wtr)
    {
        wtr.Write((byte)Status);
    }

    public void Read(NetworkReader rdr)
    {
        Status = (FlushStatus)rdr.ReadByte();
    }
}