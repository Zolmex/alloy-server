#region

using GameServer.Game.Entities.Behaviors.Transitions;

#endregion

namespace GameServer.Game.Entities.Behaviors;

public class StunnedStar : EntityBehavior
{
    public enum StunnedStarState
    {
        Start,
        SelfDestruction
    }

    private Circle OrbitPlayer;
    private TimedTransition SelfDestruct;

    public override void RegisterStates()
    {
        StateManager.RegisterState(StunnedStarState.Start, StartTick);
        StateManager.RegisterState(StunnedStarState.SelfDestruction, SelfDestructionTick);
    }

    public override void RegisterBehaviors()
    {
        OrbitPlayer = new Circle(0.2f, 1, 1.5f);
        SelfDestruct = new TimedTransition(4000, "SelfDestruction");
    }


    public override void Initialize(CharacterEntity owner)
    {
        StateManager.SetCurrentState(owner, StunnedStarState.Start);
        base.Initialize(owner);
    }

    public void StartTick(RealmTime time, CharacterEntity owner, StateTick state)
    {
        if (state == StateTick.Start)
        { }

        else if (state == StateTick.Tick)
        {
            OrbitPlayer.Tick(owner, time);
            StateManager.CheckTransition<StunnedStarState>(SelfDestruct, owner, time);
        }
    }

    public void SelfDestructionTick(RealmTime time, CharacterEntity owner, StateTick state)
    {
        if (state == StateTick.Start)
        {
            owner.TryLeaveWorld();
        }

        else if (state == StateTick.Tick)
        { }
    }
}