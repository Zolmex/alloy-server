namespace GameServer.Game
{
    public class ClientRandom
    {
        private uint _seed;

        public ClientRandom(uint seed)
        {
            _seed = seed;
        }

        public uint NextInt()
        {
            return Gen();
        }

        public uint NextIntRange(uint min, uint max)
        {
            return min == max ? min : min + (Gen() % (max - min));
        }

        private uint Gen()
        {
            var lb = 16807 * (_seed & 0xFFFF);
            var hb = 16807 * (_seed >> 16);
            lb += (hb & 32767) << 16;
            lb += hb >> 15;
            if (lb > 2147483647)
                lb -= 2147483647;
            return _seed = lb;
        }
    }
}