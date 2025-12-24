#region

#endregion

namespace GameServer.Game.Entities.Behaviors.Library;

public class HealthLockDemo : EntityBehavior
{
    public enum DemoState
    {
        Tick
    }

    public override void RegisterStates()
    {
        StateManager.RegisterState(DemoState.Tick, TestTick);
    }

    public override void Initialize(Character owner)
    {
        StateManager.SetCurrentState(owner, DemoState.Tick);
        var healthLock = new HealthLock { LockAtPerc = 0.5f, LockDurationMs = int.MaxValue };
        owner.ApplyHealthLock(healthLock);
        base.Initialize(owner);
    }

    public void TestTick(RealmTime time, Character owner, StateTick state)
    {
        if (owner.GetNearestOtherEnemyByName("HealthLockDemo", 10f) != null)
        {
            owner.ReleaseHealthLock();
        }
    }
}