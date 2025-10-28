#region

using Common.Utilities;
using Common.Utilities.Net;
using GameServer.Game.Worlds;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming
{
    [Packet(PacketId.CONSTELLATIONSTRIAL)]
    public class ConstellationsTrial : IIncomingPacket
    {
        public void Read(NetworkReader rdr)
        { }

        public void Handle(User user)
        {
            if (user.GameInfo.State != GameState.Playing)
                return;

            var prevWorld = user.GameInfo.Player.World;

            var trialWorld = new World("Vault", -1);
            RealmManager.AddWorld(trialWorld);

            user.GameInfo.Player.SendInfo("Teleporting to trial in 3 seconds...");

            var accId = user.Account.AccountId;
            RealmManager.AddTimedAction(3000, () =>
            {
                if (user.Account.AccountId != accId) // User instance could've been recycled by another player
                    return;

                if (user.GameInfo.Player != null && user.GameInfo.Player.World != null &&
                    user.GameInfo.Player.World == prevWorld)
                { //so it wont tp you if you use then go to another instance
                    if (RealmManager.Worlds.TryGetValue(trialWorld.Id, out var world))
                        user.ReconnectTo(world);
                }
            });
        }

        public override string ToString()
        {
            var type = typeof(ConstellationsTrial);
            var props = type.GetProperties();
            var ret = $"\n";
            foreach (var prop in props)
            {
                ret += $"{prop.Name}:{prop.GetValue(this)}";
                if (!(props.IndexOf(prop) == props.Length - 1))
                    ret += "\n";
            }

            return ret;
        }
    }
}