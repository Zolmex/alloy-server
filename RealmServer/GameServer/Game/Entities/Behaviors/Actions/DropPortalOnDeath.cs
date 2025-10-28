#region

using Common.Resources.Xml;
using Common.Utilities;
using System;

#endregion

namespace GameServer.Game.Entities.Behaviors.Actions
{
    public record DropPortalOnDeath : BehaviorScript
    {
        private readonly float _probability;
        private readonly string _portalId;
        private readonly int _timeout;

        public DropPortalOnDeath(string portalId, float probability = 1, int timeout = 0)
        {
            _portalId = portalId;
            _probability = probability;
            _timeout = timeout;
        }

        public override void Start(Character host)
        {
            host.DeathEvent += HandleDeath;
        }

        private void HandleDeath(Entity host)
        {
            var owner = host.World;

            if (owner.Name.Contains("Arena") || host.Spawned)
                return;

            if (Random.Shared.NextDouble() <= _probability)
            {
                var portalDesc = XmlLibrary.Id2Object(_portalId);
                var timeoutTime = _timeout == null ? portalDesc.XML.GetValue<int>("Timeout") : _timeout;
                var entity = Entity.Resolve(portalDesc.ObjectType);
                entity.Move(host.Position.X, host.Position.Y);
                entity.MoveRelative((float)Random.Shared.NextDouble() * 1.5f, (float)Random.Shared.NextDouble() * 1.5f);
                entity.EnterWorld(host.World);

                if (timeoutTime != 0)
                    host.World.AddTimedAction(timeoutTime * 1000, () => entity.TryLeaveWorld());
            }

            host.DeathEvent -= HandleDeath;
        }
    }
}