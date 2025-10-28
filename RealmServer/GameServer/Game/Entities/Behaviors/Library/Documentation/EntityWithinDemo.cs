#region

using GameServer.Game.Entities.Behaviors.Transitions;

#endregion

namespace GameServer.Game.Entities.Behaviors.Library
{
    public class EntityWithinDemo : EntityBehavior
    {
        private EntityWithinTransition PlayerWithin8Tiles;
        private EntityNotWithinTransition PlayerNotWithin12Tiles;

        public override void RegisterStates()
        {
            StateManager.RegisterState(DemoState.PlayerWithin, PlayerWithinTick);
            StateManager.RegisterState(DemoState.PlayerNotWithin, PlayerNotWithinTick);
        }

        public override void RegisterBehaviors()
        {
            PlayerWithin8Tiles = new EntityWithinTransition("player", 8f, targetStates: "PlayerNotWithin");
            PlayerNotWithin12Tiles = new EntityNotWithinTransition("player", 12f, targetStates: "PlayerWithin");
        }

        public override void Initialize(Character owner)
        {
            StateManager.SetCurrentState(owner, DemoState.PlayerWithin);
            base.Initialize(owner);
        }

        public void PlayerWithinTick(RealmTime time, Character owner, StateTick state)
        {
            if (state == StateTick.Start)
                owner.Say("Checking for Player Within 8 tiles");
            else if (state == StateTick.Tick)
                StateManager.CheckTransition<DemoState>(PlayerWithin8Tiles, owner, time);
        }

        public void PlayerNotWithinTick(RealmTime time, Character owner, StateTick state)
        {
            if (state == StateTick.Start)
                owner.Say("Checking for Player Not Within 12 tiles");
            else if (state == StateTick.Tick)
                StateManager.CheckTransition<DemoState>(PlayerNotWithin12Tiles, owner, time);
        }

        public enum DemoState
        {
            PlayerWithin,
            PlayerNotWithin
        }
    }
}