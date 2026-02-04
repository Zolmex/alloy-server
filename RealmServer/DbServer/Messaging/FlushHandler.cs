using Common;
using Common.Network;
using Common.Network.Messaging;
using Common.Network.Messaging.Impl;
using Common.Utilities;
using DbServer.Database;

namespace DbServer.Messaging;

public class FlushHandler : IMessageHandler
{
    public AppMessageId MessageId => AppMessageId.Flush;

    public async Task HandleAsync(IAppMessage msg, AppConnection con)
    {
        var pkt = (FlushMessage)msg;
        pkt.Entity = await DbCache.Find(pkt.Key);
        if (pkt.Entity == null)
        {
            Logger.Error("You suck.");
            return;
        }

        var status = FlushStatus.Success;

        var version = pkt.Version;
        if (version < pkt.Entity.Version) // Version mismatch. Entity was modified before these changes were applied. Revert changes
            status = FlushStatus.VersionMismatch;
        else
        {
            using var rdr = new NetworkReader(new MemoryStream(pkt.PropertiesBuffer));
            pkt.Entity.ReadProperties(rdr);
            DbCache.Update(pkt.Key, pkt.Entity, pkt.Properties);
            
            Logger.Debug("You did it you son of a bitch. You did it.");
        }

        await con.SendAsync(new FlushAck() { Status = status });
    }
}