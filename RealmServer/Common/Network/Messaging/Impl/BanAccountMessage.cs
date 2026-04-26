using System;
using Common.Utilities;

namespace Common.Network.Messaging.Impl;

public record struct BanAccountMessage : IAppMessage {
    public string Name { get; set; }
    public string Reason { get; set; }
    public DateTime ExpiresAt { get; set; }
    public int ModeratorId { get; set; }
    public AppMessageId MessageId => AppMessageId.BanAccount;
    public int Sequence { get; set; }

    public void Write(ref SpanWriter wtr) {
        wtr.WriteUTF(Name);
        wtr.WriteUTF(Reason);
        wtr.Write(ExpiresAt.ToUnixTimestamp());
        wtr.Write(ModeratorId);
    }

    public void Read(ref SpanReader rdr) {
        Name = rdr.ReadUTF();
        Reason = rdr.ReadUTF();
        ExpiresAt = TimeUtils.FromUnixTimestamp(rdr.ReadInt32());
        ModeratorId = rdr.ReadInt32();
    }
}