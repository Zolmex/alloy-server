using Common;
using Common.Network;
using Common.Network.Messaging;
using Common.Network.Messaging.Impl;
using Common.Utilities;
using DbServer.Database;

namespace DbServer.Messaging;

public class FlushHandler : IMessageHandler {
    public AppMessageId MessageId => AppMessageId.Flush;

    public async Task HandleAsync(IAppMessage msg, AppConnection con) {
        var pkt = (FlushMessage)msg;
        pkt.Entity = await DbCache.Find(pkt.Key);
        Logger.Debug($"{pkt.Entity.GetType()} | {pkt.Entity.GetHashCode()}");
        if (pkt.Entity == null) {
            Logger.Error("You suck.");
            return;
        }

        var status = FlushStatus.Success;

        var version = pkt.Version;

        Logger.Debug($"{pkt.Entity.GetType()} | {pkt.Version}");
        if (version < pkt.Entity
                .Version) // Version mismatch. Entity was modified before these changes were applied. Revert changes
        {
            status = FlushStatus.VersionMismatch;
        }
        else {
            var rdr = new SpanReader(pkt.PropertiesBuffer.AsSpan());
            pkt.Entity.ReadProperties(ref rdr);
            DbCache.Update(pkt.Key, pkt.Entity, pkt.Properties);

            if (await DbCache.SaveChanges() == 0)
                status = FlushStatus.InternalError;

            Logger.Debug("You did it you son of a bitch. You did it.");
        }

        con.Send(new FlushAck(pkt.Sequence) { Status = status });
    }
}