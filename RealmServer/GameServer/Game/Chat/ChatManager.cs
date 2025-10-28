#region

using Common.Utilities;
using GameServer.Game.Worlds;
using System.Linq;

#endregion

namespace GameServer.Game.Chat
{
    public static class ChatManager
    {
        private static readonly Logger _log = new(typeof(ChatManager));

        public static void Announce(string text, bool announcement = true, int worldId = 0, bool help = false, params int[] ignoreAccounts)
        {
            var msg = announcement ? $"<ANNOUNCEMENT> {text}" : text;

            RealmManager.BroadcastAllUsers(user =>
            {
                if (worldId != 0 && (user.GameInfo.State != GameState.Playing || user.GameInfo.Player.World.Id != worldId))
                    return;

                if (ignoreAccounts.Length > 0 && ignoreAccounts.Contains(user.Account.AccountId))
                    return;

                if (user.State == ConnectionState.Ready && user.GameInfo.State == GameState.Playing)
                {
                    var plr = user.GameInfo.Player;

                    if (help) plr.SendHelp(msg);
                    else plr.SendInfo(msg);
                }
            });
            _log.Debug(msg);
        }

        public static void Realm(string name, string text)
        {
            RealmManager.BroadcastAllUsers(user =>
            {
                if (user.GameInfo.State != GameState.Playing)
                    return;

                if (user.State == ConnectionState.Ready && user.GameInfo.State == GameState.Playing)
                {
                    var plr = user.GameInfo.Player;
                    plr.SendInfo($"<{name}> {text}");
                }
            });
            _log.Debug($"<{name}> {text}");
        }

        public static void Oryx(World world, string text)
        {
            world.BroadcastAll(plr => plr.SendEnemy("Oryx the Mad God", text));
            _log.Debug($"<Oryx the Mad God> {text}");
        }
    }
}