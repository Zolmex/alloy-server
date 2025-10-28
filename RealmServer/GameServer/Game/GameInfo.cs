#region

using Common.Database;
using Common.Utilities;
using GameServer.Game.Entities;
using GameServer.Game.Worlds;

#endregion

namespace GameServer.Game
{
    public enum GameState
    {
        Idle, // User has established connection to server but hasn't loaded to any world yet
        Loading, // User has sent Hello packet, and now we're waiting for client to send Load packet
        Playing // User has established
    }

    public class GameInfo
    {
        private static readonly Logger _log = new(typeof(GameInfo));

        public readonly User User;
        public World World { get; private set; }
        public DbChar Char { get; private set; }
        public Player Player { get; private set; }
        public GameState State { get; private set; }

        public byte AllyShots { get; private set; }
        public byte AllyDamage { get; private set; }
        public byte AllyNotifs { get; private set; }
        public byte AllyParticles { get; private set; }
        public byte AllyEntities { get; private set; }
        public byte DamageCounter { get; private set; } // 0 = off, 1 = full, 2 = compact, 3 = minimal

        public Vault Vault { get; set; }

        public GameInfo(User user)
        {
            User = user;
        }

        public void AllySettings(byte allyShots, byte allyDamage, byte allyNotifs, byte allyParticles,
            byte allyEntities)
        {
            AllyShots = allyShots;
            AllyDamage = allyDamage;
            AllyNotifs = allyNotifs;
            AllyParticles = allyParticles;
            AllyEntities = allyEntities;
        }

        public void UiSettings(byte damageCounter)
        {
            DamageCounter = damageCounter;
        }

        public void SetWorld(World world)
        {
            State = GameState.Loading;
            World = world;
        }

        public void Load(DbChar chr, World world)
        {
            State = GameState.Playing;

            Char = chr;
            Player ??= new Player(User, chr);
            Player.EnterWorld(world);
        }

        public void Unload(bool reconnect, bool death)
        {
            State = GameState.Idle;

            if (Player == null || death) // Player leaves world on death
                return;

            // Don't set char to null, we need that for reconnecting
            Player.UnsetParty();
            Player.SaveCharacter(!reconnect); // Save to database if we're disconnecting
            Player.TryLeaveWorld();
        }

        public void Reset()
        {
            State = GameState.Idle; // Change our state first
            Char = null;
            Player = null;
            World = null;
        }
    }
}