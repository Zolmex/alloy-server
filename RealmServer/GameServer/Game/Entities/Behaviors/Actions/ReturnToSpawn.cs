#region

using Common;
using Common.Utilities;
using System.Xml.Linq;

#endregion

namespace GameServer.Game.Entities.Behaviors.Actions
{
    public record ReturnToSpawn : BehaviorScript
    {
        private readonly float _speed;
        private readonly float _distanceFromSpawn;

        public ReturnToSpawn(XElement xml)
        {
            _speed = xml.GetAttribute<float>("speed", 1f);
            _distanceFromSpawn = xml.GetAttribute<float>("distanceFromSpawn", 1f);
        }

        public ReturnToSpawn(float speed = 1f, float distanceFromSpawn = 0f)
        {
            _speed = speed;
            _distanceFromSpawn = distanceFromSpawn;
        }

        public override BehaviorTickState Tick(Character host, RealmTime time)
        {
            var distToSpawn = EntityUtils.GetDistanceBetweenF(host.Position.X, host.SpawnPosition.X, host.Position.Y, host.SpawnPosition.Y);
            if (distToSpawn <= _distanceFromSpawn + (_speed / 50))
                return BehaviorTickState.BehaviorDeactivate;

            host.MoveTowards(time, host.SpawnPosition.ToVec2(), _speed);
            return BehaviorTickState.BehaviorActive;
        }
    }
}