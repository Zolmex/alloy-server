using Common.Utilities;
using System;

namespace Common.Network.Messaging.Impl;

public record struct BanAccountMessage : IAppMessage
{
    public AppMessageId MessageId => AppMessageId.BanAccount;
    public int Sequence { get; set; }
    
    public string Name { get; set; }
    public string Reason { get; set; }
    public DateTime ExpiresAt { get; set; }
    public int ModeratorId { get; set; }

    public void Write(NetworkWriter wtr)
    {
        wtr.Write(Name);
        wtr.Write(Reason);
        wtr.Write(ExpiresAt.ToUnixTimestamp());
        wtr.Write(ModeratorId);
    }

    public void Read(NetworkReader rdr)
    {
        Name = rdr.ReadUTF();
        Reason = rdr.ReadUTF();
        ExpiresAt = TimeUtils.FromUnixTimestamp(rdr.ReadInt32());
        ModeratorId = rdr.ReadInt32();
    }
}