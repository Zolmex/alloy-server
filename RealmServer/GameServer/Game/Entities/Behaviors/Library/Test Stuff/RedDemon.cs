//using GameServer.Game.Entities.Behaviors.Actions;
//using GameServer.Game.Entities.Behaviors.Library;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using static GameServer.Game.Entities.Behaviors.RedDemonFriend;

//namespace GameServer.Game.Entities.Behaviors
//{
//    public class RedDemon : EntityBehavior
//    {
//        // Called once upon behavior loading
//        public override void RegisterStates()
//        {
//            StateManager.RegisterState(RedDemonState.Start, StartTick);
//            StateManager.RegisterState(RedDemonState.ReEngage, ReEngageTick);
//        }
//        // Called once upon behavior loading
//        public override void RegisterBehaviors()
//        {
//            Shoot = new Shoot(radius: 12, projId: 0, cooldownMS: 1000, targeted: true);
//        }
//        // Called every time a behavior is added to an entity
//        public override void Initialize(Character owner)
//        {
//            StateManager.SetCurrentState(owner, RedDemonState.Start);
//            base.Initialize(owner);
//            Shoot.RegisterResources(owner);
//        }
//        private Shoot Shoot { get; set; }
//        private void StartTick(RealmTime time, Character owner, State state)
//        {
//            if (state == State.Start)
//            {


//                owner.World.SpawnEntity("Red Demon Friend", owner.Position.X + 5, owner.Position.Y);

//            }
//            else if (state == State.Tick)
//            {
//                Shoot.Tick(owner, time);

//                if (owner.HP < (owner.MaxHP * 0.50f))
//                {
//                    foreach (var redDemonFriend in owner.World.GetEnemiesWithBehavior<RedDemonFriend>(owner, 12f))
//                    {
//                        ((RedDemonFriend)redDemonFriend.GetBehavior()).StartShooting(time, redDemonFriend);
//                    }
//                    StateManager.Transition(time, owner, RedDemonState.Wait);
//                }
//            }
//            else if (state == State.End)
//            {

//            }
//        }

//        private void ReEngageTick(RealmTime time, Character owner, State state)
//        { 
//            if (state == State.Tick)
//            {
//            Shoot.Tick(owner, time);

//            }
//        }
//        public void StartShooting(RealmTime time, Character owner)
//        {
//            StateManager.Transition(time, owner, RedDemonState.ReEngage);

//        }
//        public enum RedDemonState
//        {
//            Start,
//            Wait,
//            ReEngage,
//        }
//    }
//}

