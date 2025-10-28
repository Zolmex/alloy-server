#region

using Common.ProjectilePaths;
using GameServer.Game.Entities.Behaviors.Actions;

#endregion

namespace GameServer.Game.Entities.Behaviors //.Library.BehaviorSkeleton << REMOVE THIS
{
    public class EntityName : EntityBehavior
    {
        //Replace Internal class with PUBLIC class || Add " : EntityBehavior" After EntityName

        public override void RegisterStates()
        {
            StateManager.RegisterState(EntityNameState.StateName, StateNameTick);
        }

        //Any and all states you create must be registered here and enumerated at the bottom of the page.
        //Here we are making a State named StateName for EntityName. We must enumerate StateName under public enum
        //StateName will remain with a red line under until you create the state itself, which i will do above public enum

        public override void RegisterBehaviors()
        {
            Follow = new Follow(1, 2, 6);
            Shoothaha = new Shoot(6, damage: 15, path: new LinePath(6), cooldownMS: 1000, targeted: true, projName: "Blade");

            //shootAngle is the degrees between each shot. fixedAngle is shooting a specific Degree each shot.
            // You MUST specify the type of path the projectile will take through the "path:" parameter. > path:(path)(speed),
            // we currently only have LinePath and BoomerangPath
        }

        //Any and all Behaviors will be registered here. an example will be shown below.
        //BehaviorName can be named anything but it must start with a capital.
        //You should name it something relative to the behavior to make it easy to read and understand for others who may read your code. 

        //private *Behavior* *BehaviorName* { get; set; }

        private Follow Follow { get; set; }
        private Shoot Shoothaha { get; set; }

        public override void Initialize(Character owner)
        {
            StateManager.SetCurrentState(owner, EntityNameState.StateName);

            base.Initialize(owner);
        }


        public void StateNameTick(RealmTime time, Character owner, StateTick state)
        {
            if (state == StateTick.Start)
            {
                //What EntityName will do one time, upon entering the StateName state.
            }

            else if (state == StateTick.Tick)
            {
                //What EntityName will do every tick while in StateName State.
                Follow.Tick(owner, time);
                Shoothaha.Tick(owner, time);
            }
        }

        public enum EntityNameState
        {
            StateName
        }
        //Now that our state,StateName has been Enumerated. We can use it.
    }
}