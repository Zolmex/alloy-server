#region

using Common.Utilities;

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing
{
    [Packet(PacketId.MAPINFO)]
    public class MapInfo : IOutgoingPacket
    {
        public int MapWidth { get; }
        public int MapHeight { get; }
        public string Name { get; }
        public string DisplayName { get; }
        public uint Seed { get; }
        public int Background { get; }
        public bool AllowPlayerTeleport { get; }
        public bool ShowDisplays { get; }
        public string Music { get; }
        public int Difficulty { get; }

        public static void Write(NetworkHandler network, int mapWidth, int mapHeight, string name, string displayName, uint seed, int background, bool showDisplays, bool allowPlayerTeleport, string music, int difficulty)
        {
            var state = network.SendState;
            var wtr = state.Writer;
            using (TimedLock.Lock(state))
            {
                var begin = state.PacketBegin();

                wtr.Write(mapWidth);
                wtr.Write(mapHeight);
                wtr.WriteUTF(name);
                wtr.WriteUTF(displayName);
                wtr.Write(seed);
                wtr.Write(background);
                wtr.Write(showDisplays);
                wtr.Write(allowPlayerTeleport);
                wtr.WriteUTF(music);
                wtr.Write(difficulty);

                state.PacketEnd(begin, PacketId.MAPINFO);
            }
        }

        public override string ToString()
        {
            var type = typeof(MapInfo);
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