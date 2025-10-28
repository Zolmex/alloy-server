#region

using GameServer.Game.Entities.Behaviors.Actions;

#endregion

namespace GameServer.Game.Entities.Behaviors
{
    public class IAMFRIEND : EntityBehavior
    {
        private Follow FollowYou;
        private Taunt EmbraceMe;
        private Taunt HurtTaunt;
        private Taunt BloodSuck;


        public override void RegisterStates()
        {
            StateManager.RegisterState(IAMFRIENDState.EmbraceMe, EmbraceMeTick);
            StateManager.RegisterState(IAMFRIENDState.YouveHurtMe, YouveHurtMeTick);
        }

        public override void RegisterBehaviors()
        {
            FollowYou = new Follow(0.4f, 0, 5);

            EmbraceMe = new Taunt("ILOVEYOU 🙂", 20000);
            HurtTaunt = new Taunt("Stop it! ):", 10000);
            BloodSuck = new Taunt("*blood sucking noises", 2000);
        }

        public override void Initialize(Character owner)
        {
            StateManager.SetCurrentState(owner, IAMFRIENDState.EmbraceMe);
            base.Initialize(owner);
        }

        public void EmbraceMeTick(RealmTime time, Character owner, StateTick state)
        {
            if (state == StateTick.Start)
            { }

            else if (state == StateTick.Tick)
            {
                FollowYou.Tick(owner, time);

                var player = owner.GetNearestPlayer(10);
                if (player == null)
                    return;

                if (player.GetDistanceBetween(owner) <= 0.2f)
                {
                    EmbraceMe.Tick(owner, time);
                    var HealPlayer = (int)(player.MaxHP * 0.003);

                    if (owner.HP < owner.MaxHP * 0.50)
                    {
                        StateManager.Transition(time, owner, IAMFRIENDState.YouveHurtMe);
                    }

                    else if (player.HP < player.MaxHP)
                    {
                        player.Heal(HealPlayer);
                    }
                }
            }
        }

        public void YouveHurtMeTick(RealmTime time, Character owner, StateTick state)
        {
            var player = owner.GetNearestPlayer(10);
            if (player == null)
                return;

            if (state == StateTick.Start)
            { }

            else if (state == StateTick.Tick)
            {
                var dmg = (int)(player.MaxHP * 0.05);
                player.Damage(dmg);

                var Healing3Percent = (int)(owner.MaxHP * -0.03);
                owner.Damage(Healing3Percent);

                HurtTaunt.Tick(owner, time);
                BloodSuck.Tick(owner, time);

                if (owner.HP > owner.MaxHP * 0.50)
                {
                    StateManager.Transition(time, owner, IAMFRIENDState.EmbraceMe);
                }
            }
        }


        public enum IAMFRIENDState
        {
            EmbraceMe,
            YouveHurtMe
        }
    }
}