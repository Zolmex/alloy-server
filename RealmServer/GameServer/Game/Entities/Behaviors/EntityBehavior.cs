#region

using GameServer.Game.DamageSources;

#endregion

namespace GameServer.Game.Entities.Behaviors
{
    public class EntityBehavior
    {
        public StateManager StateManager { get; set; }

        public EntityBehavior()
        {
            StateManager = new StateManager();
            RegisterStates();
            RegisterBehaviors();
        }

        public virtual void RegisterStates() { }
        public virtual void RegisterBehaviors() { }

        public virtual void Initialize(Character owner)
        {
            StateManager.StartCurrentState(owner, RealmManager.WorldTime);
        }

        public virtual void OnHitEntity(Character owner, Character hit, DamageSource damageSource)
        { }

        public virtual void OnHitBy(Character owner, Character from, DamageSource damageSource)
        { }

        public virtual void OnDeath(Character owner, string killer)
        { }

        public void Tick(Character owner, RealmTime time)
        {
            StateManager.Tick(owner, time);
        }
    }
}