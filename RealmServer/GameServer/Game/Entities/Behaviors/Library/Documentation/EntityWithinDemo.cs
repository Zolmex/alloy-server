#region

using GameServer.Game.Entities.Behaviors.Transitions;

#endregion

namespace GameServer.Game.Entities.Behaviors.Library;

public class EntityWithinDemo : EntityBehavior
{
    public enum DemoState
    {
        PlayerWithin,
        PlayerNotWithin
    }

    private EntityNotWithinTransition PlayerNotWithin12Tiles;
    private EntityWithinTransition PlayerWithin8Tiles;

    public override void RegisterStates()
    {
        StateManager.RegisterState(DemoState.PlayerWithin, PlayerWithinTick);
        StateManager.RegisterState(DemoState.PlayerNotWithin, PlayerNotWithinTick);
    }

    public override void RegisterBehaviors()
    {
        PlayerWithin8Tiles = new EntityWithinTransition(targetStates: "PlayerNotWithin");
        PlayerNotWithin12Tiles = new EntityNotWithinTransition("player", 12f, targetStates: "PlayerWithin");
    }

    public override void Initialize(CharacterEntity owner)
    {
        StateManager.SetCurrentState(owner, DemoState.PlayerWithin);
        base.Initialize(owner);
    }

    public void PlayerWithinTick(RealmTime time, CharacterEntity owner, StateTick state)
    {
        if (state == StateTick.Start)
            owner.Say("Checking for Player Within 8 tiles");
        else if (state == StateTick.Tick)
            StateManager.CheckTransition<DemoState>(PlayerWithin8Tiles, owner, time);
    }

    public void PlayerNotWithinTick(RealmTime time, CharacterEntity owner, StateTick state)
    {
        if (state == StateTick.Start)
            owner.Say("Checking for Player Not Within 12 tiles");
        else if (state == StateTick.Tick)
            StateManager.CheckTransition<DemoState>(PlayerNotWithin12Tiles, owner, time);
    }
}