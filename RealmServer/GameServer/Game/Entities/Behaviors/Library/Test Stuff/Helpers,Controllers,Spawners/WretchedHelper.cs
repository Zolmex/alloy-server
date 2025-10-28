#region

using GameServer.Game.Entities.Behaviors.Actions;

#endregion

namespace GameServer.Game.Entities.Behaviors
{
    public class WretchedHelper : EntityBehavior
    {
        public override void RegisterStates()
        {
            StateManager.RegisterState(WretchedHelperState.CreateSquare, CreateSquareTick);
        }

        public override void RegisterBehaviors()
        {
            //Taunt = new Taunt(text: "YOOO WE HERE");
        }

        private Taunt Taunt { get; set; }

        public override void Initialize(Character owner)
        {
            StateManager.SetCurrentState(owner, WretchedHelperState.CreateSquare);
            base.Initialize(owner);
        }

        public void CreateSquareTick(RealmTime time, Character owner, StateTick state)
        {
            if (state == StateTick.Start)
            {
                // Taunt.RegisterResources(owner);
            }

            else if (state == StateTick.Tick)
            { }
        }

        public enum WretchedHelperState
        {
            CreateSquare
        }
    }
}