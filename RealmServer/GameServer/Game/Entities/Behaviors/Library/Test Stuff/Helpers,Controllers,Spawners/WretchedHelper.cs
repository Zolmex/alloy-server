#region

using GameServer.Game.Entities.Behaviors.Actions;

#endregion

namespace GameServer.Game.Entities.Behaviors;

public class WretchedHelper : EntityBehavior
{
    public enum WretchedHelperState
    {
        CreateSquare
    }

    private Taunt Taunt { get; set; }

    public override void RegisterStates()
    {
        StateManager.RegisterState(WretchedHelperState.CreateSquare, CreateSquareTick);
    }

    public override void RegisterBehaviors()
    {
        //Taunt = new Taunt(text: "YOOO WE HERE");
    }

    public override void Initialize(CharacterEntity owner)
    {
        StateManager.SetCurrentState(owner, WretchedHelperState.CreateSquare);
        base.Initialize(owner);
    }

    public void CreateSquareTick(RealmTime time, CharacterEntity owner, StateTick state)
    {
        if (state == StateTick.Start)
        {
            // Taunt.RegisterResources(owner);
        }

        else if (state == StateTick.Tick)
        { }
    }
}