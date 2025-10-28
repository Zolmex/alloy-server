#region

using GameServer.Game.Worlds;

#endregion

namespace GameServer.Game.Entities
{
    public class Portal : Character
    {
        public World PortalWorld { get; set; }

        public bool Usable { get; set; }

        public Portal(ushort type, bool attachWorld = false) : base(type)
        {
            if (attachWorld)
            {
                var realm = new Realm();
                RealmManager.AddWorld(realm);

                PortalWorld = realm;
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            Usable = true;
            if (Desc.RealmPortal)
            {
                Name = PortalWorld.DisplayName + " (" + PortalWorld.Players.Count;

                var maxPlayers = PortalWorld.Config.MaxPlayers;
                if (maxPlayers == -1)
                    Name += ")";
                else
                    Name += $"/{maxPlayers})";
            }
        }

        public override bool Tick(RealmTime time)
        {
            if (!base.Tick(time))
                return false;

            if (Desc.RealmPortal)
            {
                Name = PortalWorld.DisplayName + " (" + PortalWorld.Players.Count;
                var maxPlayers = PortalWorld.Config.MaxPlayers;
                if (maxPlayers == -1)
                    Name += ")";
                else
                    Name += $"/{maxPlayers})";
            }

            return true;
        }

        public void LoadWorld(User user)
        {
            if (!Usable)
            {
                var player = user.GameInfo.Player;
                player.SendError("This portal is locked.");
                return;
            }

            if (PortalWorld != null && PortalWorld is not Vault && PortalWorld.DisplayName != "Guild Hall")
                return;

            var worldName = Desc.DungeonName;
            if (worldName == null)
            {
                _log.Error($"Invalid portal type '{Desc.ObjectType:X4}'");
                return;
            }

            PortalWorld = worldName switch
            {
                "Nexus" => RealmManager.NexusInstance,
                "Vault" => user.GameInfo.Vault ?? new Vault(user),
                "Guild Hall" => RealmManager.GetGuildHall(user.Account.GuildId),
                _ => new World(worldName, -1)
            };

            if (PortalWorld == null)
                return;

            if (!PortalWorld.Active)
                RealmManager.AddWorld(PortalWorld);
        }
    }
}