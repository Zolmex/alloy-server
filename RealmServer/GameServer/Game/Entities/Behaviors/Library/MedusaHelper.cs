/* using Common.Utilities;
 using GameServer.Game.Entities.Behaviors.Actions;
 using System;
 using System.Collections.Generic;
 using System.Linq;
 using System.Text;
 using System.Threading.Tasks;

 namespace GameServer.Game.Entities.Behaviors.Library
 {
     public class MedusaHelperInfo
     {
         public int CooldownLeft = 1000;
         public float ShootAngle = 0f;
     }
     public class MedusaHelper : EntityBehavior
     {
         public override void RegisterStates()
         {
             StateManager.RegisterState(MedusaHelperState.Shoot, ShootTick);
         }

           Called once upon behavior loading
         public override void RegisterBehaviors()
         {
             Shoot = new Shoot(radius: 12, projId: 0, cooldownMS: 1000, targeted: true);
             Circle = new Circle(rotationsPerSecond: 0.5f, radius: 4f, target: "Medusa");
         }
         public override void Initialize(Character owner)
         {
             owner.InsertResource(this, new MedusaHelperInfo());

             Shoot.RegisterResources(owner);
             Circle.RegisterResources(owner);
         }

         public MedusaHelperInfo GetInfo(Character owner)
             => (MedusaHelperInfo)owner.GetResource(this);
         public void SetAngle(Character owner, float angle)
         {
             GetInfo(owner).ShootAngle = angle.Deg2Rad();
         }
         private Shoot Shoot { get; set; }
         private Circle Circle { get; set; }
        public void ShootTick(RealmTime time, Character owner, State state)
        {
            if (state == State.Start)


        }
         public void StartShooting(RealmTime time, Character owner)
         {
             StateManager.Transition(time, owner, MedusaHelperState.Shoot);
         }
         public enum MedusaHelperState
         {
             Idle,
             Shoot
         }
     }
 }
*/

