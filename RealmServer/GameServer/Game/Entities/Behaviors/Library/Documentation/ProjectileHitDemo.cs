#region

using Common.Projectiles.ProjectilePaths;
using GameServer.Game.Entities.Behaviors.Actions;

#endregion

namespace GameServer.Game.Entities.Behaviors.Library.Documentation;

public class ProjectileHitDemo : EntityBehavior
{
    public enum DemoState
    {
        Tick
    }

    private Shoot Shoot;

    // Called once upon behavior loading
    public override void RegisterStates()
    {
        StateManager.RegisterState(DemoState.Tick, Tick);
    }

    // Called once upon behavior loading
    public override void RegisterBehaviors()
    {
        Shoot = new Shoot(12, damage: 10, path: new LinePath(5), count: 1, lifetimeMs: 5000, cooldownMS: 1000, targeted: false, projName: "Green Star", onHitEvent: _hitWithGreenStar);
    }

    // Called every time a behavior is added to an entity
    public override void Initialize(CharacterEntity owner)
    {
        StateManager.SetCurrentState(owner, DemoState.Tick);
        base.Initialize(owner);
    }

    public void Tick(RealmTime time, CharacterEntity owner, StateTick state)
    {
        if (state == StateTick.Tick)
        {
            Shoot.Tick(owner, time);
        }
    }

    private void _hitWithGreenStar(CharacterEntity hit, CharacterEntity hitBy)
    {
        hitBy.World.Taunt(hitBy, $"I just hit {hit.Name}!");
    }
}