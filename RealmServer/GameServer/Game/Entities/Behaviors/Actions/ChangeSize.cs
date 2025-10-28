namespace GameServer.Game.Entities.Behaviors.Actions
{
    public class SizeInfo
    {
        public int CooldownLeft;
    }

    public record ChangeSize : BehaviorScript
    {
        private readonly int _rate;
        private readonly int _target;

        public ChangeSize(int rate, int target)
        {
            _rate = rate;
            _target = target;
        }

        public override void Start(Character host)
        {
            var state = host.ResolveResource<SizeInfo>(this);
            state.CooldownLeft = 0;
        }

        public override BehaviorTickState Tick(Character host, RealmTime time)
        {
            var state = host.ResolveResource<SizeInfo>(this);

            if (state.CooldownLeft > 0)
            {
                state.CooldownLeft -= time.ElapsedMsDelta;
                return BehaviorTickState.OnCooldown;
            }

            if (state.CooldownLeft <= 0)
            {
                if (host.Size != _target)
                {
                    host.Size += _rate;
                    if ((_rate > 0 && host.Size > _target) ||
                        (_rate < 0 && host.Size < _target))
                        host.Size = _target;
                }

                state.CooldownLeft = 150;
            }

            return BehaviorTickState.BehaviorActive;
        }
    }
}