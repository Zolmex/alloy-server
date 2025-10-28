//using GameServer.Game.Entities.Behaviors.Actions;
//using GameServer.Game.Entities.Behaviors.Library;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using static GameServer.Game.Entities.Behaviors.Library.MedusaHelper;
//using static GameServer.Game.Entities.Behaviors.RedDemon;

//namespace GameServer.Game.Entities.Behaviors
//{
//    public class RedDemonFriend : EntityBehavior
//    {
//        // Called once upon behavior loading
//        public override void RegisterStates()
//        {
//            StateManager.RegisterState(RedDemonFriendState.Shoot, ShootTick);
//            StateManager.RegisterState(RedDemonFriendState.Shoot2, Shoot2Tick);

//        }
//        // Called once upon behavior loading
//        public override void RegisterBehaviors()
//        {
//            Shoot = new Shoot(radius: 12, projId: 0, cooldownMS: 1000, targeted: true);
//            Shoot2 = new Shoot(radius: 12, projId: 0, cooldownMS: 400, targeted: true);
//        }
//        // Called every time a behavior is added to an entity
//        public override void Initialize(Character owner)
//        {
//            base.Initialize(owner);
//            Shoot.RegisterResources(owner);
//            Shoot2.RegisterResources(owner);
//        }
//        private Shoot Shoot { get; set; }
//        private Shoot Shoot2 { get; set; }
//        public void ShootTick(RealmTime time, Character owner, State state)
//        {
//            if (state == State.Start)
//            {


//            }
//            else if (state == State.Tick)
//            {
//                Shoot.Tick(owner, time);

//                if (owner.HP < (owner.MaxHP * 0.50))
//                {
//                    foreach (var redDemon in owner.World.GetEnemiesWithBehavior<RedDemon>(owner, 12f))
//                    {
//                        ((RedDemon)redDemon.GetBehavior()).StartShooting(time, redDemon);
//                    }
//                    StateManager.Transition(time, owner, RedDemonFriendState.Shoot2);
//                }
//            }

//        }
//        public void Shoot2Tick(RealmTime time, Character owner, State state)
//        {
//            if (state == State.Start)
//            {


//            }
//            else if (state == State.Tick)
//            {
//                Shoot2.Tick(owner, time);


//            }
//        }
//        public void StartShooting(RealmTime time, Character owner)
//        {
//            StateManager.Transition(time, owner, RedDemonFriendState.Shoot);

//        }
//        public enum RedDemonFriendState
//        {
//           Shoot,
//           Shoot2
//        }
//    }
//}

