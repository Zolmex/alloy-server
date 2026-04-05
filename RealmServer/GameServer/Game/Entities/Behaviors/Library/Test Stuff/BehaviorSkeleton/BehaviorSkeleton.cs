namespace GameServer.Game.Entities.Behaviors;

public class EntityNameHere : EntityBehavior
{
    public enum EntityNameHereState
    {
        StateName
    }

    public override void RegisterStates()
    {
        StateManager.RegisterState(EntityNameHereState.StateName, StateNameTick);
    }

    public override void RegisterBehaviors()
    { }

    public override void Initialize(CharacterEntity owner)
    {
        StateManager.SetCurrentState(owner, EntityNameHereState.StateName);


        base.Initialize(owner);
    }

    public void StateNameTick(RealmTime time, CharacterEntity owner, StateTick state)
    {
        if (state == StateTick.Start)
        { }

        else if (state == StateTick.Tick)
        { }
    }
}