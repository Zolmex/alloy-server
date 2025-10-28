namespace GameServer.Game.Entities.Behaviors.Actions
{
    public class SetAltTextureInfo
    {
        public int CurrentTexture;
        public int RemainingTime;
    }

    public record SetAltTexture : BehaviorScript
    {
        private readonly int _indexMax;
        private readonly int _indexMin;
        private readonly bool _loop;
        private readonly int _cooldown;

        public SetAltTexture(int minValue, int maxValue = -1, int cooldown = 0, bool loop = false)
        {
            _indexMin = minValue;
            _indexMax = maxValue;
            _cooldown = cooldown;
            _loop = loop;
        }

        public override void Start(Character host)
        {
            var state = host.ResolveResource<SetAltTextureInfo>(this);
            state.CurrentTexture = host.AltTexture;
            state.RemainingTime = _cooldown;

            if (host.AltTexture != _indexMin)
            {
                host.AltTexture = _indexMin;
                state.CurrentTexture = _indexMin;
            }
        }

        public override BehaviorTickState Tick(Character host, RealmTime time)
        {
            var state = host.ResolveResource<SetAltTextureInfo>(this);

            if (_indexMax == -1 || (state.CurrentTexture == _indexMax && !_loop))
                return BehaviorTickState.BehaviorFailed;

            if (state.RemainingTime > 0)
            {
                state.RemainingTime -= time.ElapsedMsDelta;
                return BehaviorTickState.OnCooldown;
            }

            if (state.RemainingTime <= 0)
            {
                var newTexture = state.CurrentTexture >= _indexMax ? _indexMin : state.CurrentTexture + 1;
                state.CurrentTexture = newTexture;
                host.AltTexture = newTexture;
                state.RemainingTime = _cooldown;
            }

            return BehaviorTickState.BehaviorActive;
        }
    }
}