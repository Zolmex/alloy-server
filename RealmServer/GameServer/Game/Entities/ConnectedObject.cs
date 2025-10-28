#region

using Common;

#endregion

namespace GameServer.Game.Entities
{
    public class ConnectionBuilder
    {
        public static int[] Dot = BuildConnections(0x02020202);
        public static int[] UShortLine = BuildConnections(0x01020202);
        public static int[] L = BuildConnections(0x01010202);
        public static int[] Line = BuildConnections(0x01020102);
        public static int[] T = BuildConnections(0x01010201);
        public static int[] Cross = BuildConnections(0x01010101);

        public static int[] BuildConnections(uint bits)
        {
            var connections = new int[4];
            for (var k = 0; k < 4; k++)
            {
                connections[k] = (int)bits;
                bits = (bits >> 8) | (bits << 24);
            }

            return connections;
        }
    }

    public class ConnectedObject : Entity
    {
        public int Connect
        {
            get => Stats.Get<int>(StatType.Connect);
            set => Stats.Set(StatType.Connect, value);
        }

        public ConnectedObject(ushort type) : base(type)
        {
            IsConnected = true;
        }

        public void FindConnection()
        {
            var mx = (int)Position.X;
            var my = (int)Position.Y;
            var nearby = new bool[3, 3];

            for (var y = -1; y <= 1; y++)
                for (var x = -1; x <= 1; x++)
                    if (mx + x >= 0 && mx + x < World.Map.Width && my + y >= 0 && my + y < World.Map.Height)
                        nearby[x + 1, y + 1] = World.Map.Tiles[mx + x, my + y].ObjectType == Desc.ObjectType;

            int val;
            if (nearby[1, 0] && nearby[1, 2] && nearby[0, 1] && nearby[2, 1])
                val = ConnectionBuilder.Cross[0];
            else if (nearby[0, 1] && nearby[1, 1] && nearby[2, 1] && nearby[1, 0])
                val = ConnectionBuilder.T[0];
            else if (nearby[1, 0] && nearby[1, 1] && nearby[1, 2] && nearby[2, 1])
                val = ConnectionBuilder.T[1];
            else if (nearby[0, 1] && nearby[1, 1] && nearby[2, 1] && nearby[1, 2])
                val = ConnectionBuilder.T[2];
            else if (nearby[1, 0] && nearby[1, 1] && nearby[1, 2] && nearby[0, 1])
                val = ConnectionBuilder.T[3];
            else if (nearby[1, 0] && nearby[1, 1] && nearby[1, 2])
                val = ConnectionBuilder.Line[0];
            else if (nearby[0, 1] && nearby[1, 1] && nearby[2, 1])
                val = ConnectionBuilder.Line[1];
            else if (nearby[1, 0] && nearby[2, 1])
                val = ConnectionBuilder.L[0];
            else if (nearby[2, 1] && nearby[1, 2])
                val = ConnectionBuilder.L[1];
            else if (nearby[1, 2] && nearby[0, 1])
                val = ConnectionBuilder.L[2];
            else if (nearby[0, 1] && nearby[1, 0])
                val = ConnectionBuilder.L[3];
            else if (nearby[1, 0])
                val = ConnectionBuilder.UShortLine[0];
            else if (nearby[2, 1])
                val = ConnectionBuilder.UShortLine[1];
            else if (nearby[1, 2])
                val = ConnectionBuilder.UShortLine[2];
            else if (nearby[0, 1])
                val = ConnectionBuilder.UShortLine[3];
            else
                val = ConnectionBuilder.Dot[0];

            if (Connect != val)
                Connect = val;
        }
    }
}