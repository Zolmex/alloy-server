#region

using Common.Resources.Xml;
using System;

#endregion

namespace GameServer.Game.Entities.Behaviors.Actions
{
    public record TransformOnDeath : BehaviorScript
    {
        private readonly string _target;
        private readonly int _min;
        private readonly int _max;
        private readonly float _probability;

        public TransformOnDeath(string target, int min = 1, int max = 1, float probability = 1)
        {
            _target = target;
            _min = min;
            _max = max;
            _probability = probability;
        }

        public override void Start(Character host)
        {
            host.DeathEvent += HandleDeath;
        }

        public override void End(Character host, RealmTime time)
        {
            host.DeathEvent -= HandleDeath;
        }

        private void HandleDeath(Entity host)
        {
            if (!(Random.Shared.NextDouble() <= _probability))
                return;

            var obj = XmlLibrary.Id2Object(_target);
            if (obj.Class.Contains("Portal"))
                return;

            var max = _max;
            if (_min > _max)
                max = _min;

            var count = Random.Shared.Next(_min, max + 1);
            for (var i = 0; i < count; i++)
            {
                var entity = Entity.Resolve(obj.ObjectType);
                if (host.Spawned)
                {
                    entity.Spawned = true;
                }

                entity.Move(host.Position.X, host.Position.Y);
                entity.EnterWorld(host.World);
            }
        }
    }
}