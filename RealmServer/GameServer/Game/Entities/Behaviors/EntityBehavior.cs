#region

using GameServer.Game.DamageSources;

#endregion

namespace GameServer.Game.Entities.Behaviors;

public class EntityBehavior
{
    public EntityBehavior()
    {
        StateManager = new StateManager();
        RegisterStates();
        RegisterBehaviors();
    }

    public StateManager StateManager { get; set; }

    public virtual void RegisterStates() { }
    public virtual void RegisterBehaviors() { }

    public virtual void Initialize(CharacterEntity owner)
    {
        StateManager.StartCurrentState(owner, RealmManager.WorldTime);
    }

    public virtual void OnHitEntity(CharacterEntity owner, CharacterEntity hit, DamageSource damageSource)
    { }

    public virtual void OnHitBy(CharacterEntity owner, CharacterEntity from, DamageSource damageSource)
    { }

    public virtual void OnDeath(CharacterEntity owner, string killer)
    { }

    public void Tick(CharacterEntity owner, RealmTime time)
    {
        StateManager.Tick(owner, time);
    }
}