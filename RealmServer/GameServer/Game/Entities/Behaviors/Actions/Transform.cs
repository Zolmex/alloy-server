#region

using Common.Resources.Xml;

#endregion

namespace GameServer.Game.Entities.Behaviors.Actions
{
    public record Transform : BehaviorScript
    {
        private readonly string _target;

        public Transform(string target)
        {
            _target = target;
        }

        public override void Start(Character host)
        {
            var obj = XmlLibrary.Id2Object(_target);
            if (obj.Class.Contains("Portal"))
                return;

            var entity = Entity.Resolve(obj.ObjectType);
            if (host.Spawned)
            {
                entity.Spawned = true;
            }

            entity.Move(host.Position.X, host.Position.Y);
            entity.EnterWorld(host.World);

            host.TryLeaveWorld();
        }
    }
}