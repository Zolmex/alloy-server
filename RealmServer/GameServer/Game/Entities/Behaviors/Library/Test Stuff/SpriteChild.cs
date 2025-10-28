// using GameServer.Game.Entities.Behaviors.Actions;
// using GameServer.Game.Projectiles.Paths;
//
// namespace GameServer.Game.Entities.Behaviors
// {
//     public class SpriteChild : EntityBehavior
//     {
//         private bool TimerOn;
//         private Shoot Shoot;
//         private Wander Crybitch;
//         private StayAwayFrom Ihateyou;
//         private Taunt Sobbing;
//         private Taunt GoAway;
//
//         // Called once upon behavior loading //(speed: 2, range: -2, acquireRange: 9, cooldownMS: 100);
//         public override void RegisterStates()
//         {
//             StateManager.RegisterState(SpriteChildState.Wander, WanderTick);
//             StateManager.RegisterState(SpriteChildState.StayAwayFrom, StayAwayFromTick);
//         }
//         // Called once upon behavior loading        
//         public override void RegisterBehaviors()
//         {
//             Shoot = new Shoot(radius: 12, damage: 100, path: new LinePath(speed: 6), count: 2, shootAngle: 15, cooldownMS: 400, targeted: true, projName:"Thunderbolt");
//             Crybitch = new Wander(speed:2 ,distance: 3);
//             Ihateyou = new StayAwayFrom(speed: 5    , distFromTarget: 16, acquireRange: 9, cooldownMS: 4000, followTimeMS: 6000);
//             Sobbing = new Taunt(text: "*Sniff Sniff*", coolDownMS: 2000);
//             GoAway = new Taunt(text: "Go Away!", coolDownMS: 9800);
//         }
//         // Called every time a behavior is added to an entity
//         public override void Initialize(Character owner)
//         {
//             StateManager.SetCurrentState(owner, SpriteChildState.Wander);
//             base.Initialize(owner);
//         }
//
//         public void WanderTick(RealmTime time, Character owner, StateTick state)
//         {
//             if (state == StateTick.Start)
//             {
//                
//             }
//             else if (state == StateTick.Tick)
//             {
//
//                 Crybitch.Tick(owner, time);
//
//                 Sobbing.Tick(owner, time);
//
//                 if (owner.IsPlayerWithin(radius: 7))
//                 {
//                     StateManager.Transition(time, owner, SpriteChildState.StayAwayFrom);
//                 }
//             }
//
//           
//
//         }
//         public void StayAwayFromTick(RealmTime time, Character owner, StateTick state)
//         {
//             if (state == StateTick.Start)
//             {
//                 
//                
//             }
//             else if(state == StateTick.Tick)
//             {
//                 Player player = owner.GetNearestPlayer(13);
//                 if (player == null)
//                     return;
//
//                 Ihateyou.Tick(owner, time);
//                 GoAway.Tick(owner, time);
//                 Shoot.Tick(owner, time);
//
//                 if (!TimerOn && player.GetDistanceBetween(owner) >= 13) 
//                 {
//                     TimerOn = true;
//                     RealmManager.AddTimedAction(2000, () =>
//                     {
//                         StateManager.Transition(time, owner, SpriteChildState.Wander);
//                         TimerOn = false;
//                     });
//                 }
//             }
//         }
//         // Sniff Sniff > Boohoo | Wandering
//         // Eek! > Waa > Go away | Reverse StayAwayFrom
//         public enum SpriteChildState
//         {
//             Wander,
//             StayAwayFrom
//         }
//     }
//
// }

