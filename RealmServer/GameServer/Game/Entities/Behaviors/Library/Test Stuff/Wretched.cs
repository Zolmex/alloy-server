#region

using GameServer.Game.Entities.Behaviors.Actions;

#endregion

namespace GameServer.Game.Entities.Behaviors
{
    public class Wretched : EntityBehavior
    {
        private Wander Wander;
        private Circle WanderCircle;
        private Shoot ShootOne;
        private Shoot ShootTwo;

        public override void RegisterStates()
        {
            StateManager.RegisterState(WretchedState.Start, StartTick);
            StateManager.RegisterState(WretchedState.Encircle, EncircleTick);
        }

        public override void RegisterBehaviors()
        {
            Wander = new Wander(5, 3);
            WanderCircle = new Circle(0.2f, 6, 2);
/*            ShootOne = new Shoot(radius: 6, count: 2, shootAngle: 15, angleOffset: -30, cooldownMS: 1000, targeted: true, coolDownOffset: 2000);
            ShootTwo = new Shoot(radius: 6, count: 2, shootAngle: 15, angleOffset: 30,cooldownMS: 1000, targeted: true, coolDownOffset: 2000);*/

            //COMMENTED DUE TO DEPRECATION. RETURN TO LATER. 
        }

        public override void Initialize(Character owner)
        {
            StateManager.SetCurrentState(owner, WretchedState.Start);
            base.Initialize(owner);
        }

        public void StartTick(RealmTime time, Character owner, StateTick state)
        {
            if (state == StateTick.Start)
            { }
            else if (state == StateTick.Tick)
            {
                Wander.Tick(owner, time);

                var player = owner.GetNearestPlayer(10);

                if (player.GetDistanceBetween(owner) <= 6)
                {
                    StateManager.Transition(time, owner, WretchedState.Encircle);
                }
            }
        }

        public void EncircleTick(RealmTime time, Character owner, StateTick state)
        {
            if (state == StateTick.Start)
            {
                var player = owner.GetNearestPlayer(7);

                var dmg = (int)(owner.MaxHP * 0.25);

                WanderCircle.Start(owner);

                var WretchedHelperEntity1 = owner.World.SpawnEntity("WretchedHelper", owner.Position.X + 2, owner.Position.Y);
                var WretchedHelperEntity2 = owner.World.SpawnEntity("WretchedHelper", owner.Position.X - 2, owner.Position.Y);
                var WretchedHelperEntity3 = owner.World.SpawnEntity("WretchedHelper", owner.Position.X, owner.Position.Y + 2);
                var WretchedHelperEntity4 = owner.World.SpawnEntity("WretchedHelper", owner.Position.X, owner.Position.Y - 2);
                var WretchedHelperEntity1Character = (Character)WretchedHelperEntity1;
                var WretchedHelperEntity2Character = (Character)WretchedHelperEntity2;
                var WretchedHelperEntity3Character = (Character)WretchedHelperEntity3;
                var WretchedHelperEntity4Character = (Character)WretchedHelperEntity4;

                for (var i = 1; i < 5; i++)
                {
                    //could also just do 
                    //int b = i == 4;
                    //then check b in the timer
                    //:grin:

                    RealmManager.AddTimedAction(2000 * i, () =>
                    {
                        owner.Damage(dmg);

                        if (i == 4)
                            owner.TryLeaveWorld();
                    });
                }
            }
            else if (state == StateTick.Tick)
            {
                WanderCircle.Tick(owner, time);
                ShootOne.Tick(owner, time);
                ShootTwo.Tick(owner, time);
            }
        }

        /*
         * for ([create variable]; [specify condition]; [increment variable])
         * {
         *  //code in here
         * }
         *
         * by create variable its as simple as just creating a variable [ int i = 0; ] for example, we are creating an integer called i and setting it to 0.
         *
         * the for loop will continue to loop until the condition is met, so for example if we want it to loop 4 times we could say [ i < 4 ]
         *
         * by increment variable i mean literally to increment the variable, so we would put [ i++ ] (which increases i by 1)
         *
         * i++ is the same as i = i + 1;
         *
         * the way a for loop works, it will first create the variable, and then run all of the code within the brackets these -> {}
         * then, when it has run all that code, it will increment the variable by 1, and check if the condition is met. If it is, it will continue looping from the top down
         *
         * so, for example, if I had a for loop like the following:
         *
         * for (int i = 0; i < 4; i++)
         * {
         *      Console.WriteLine($"Current Number: {i}");
                //this code will print our current number
         * }
         *
         * the output of this would be:
         *
         * Current Number: 0
         * Current Number: 1
         * Current Number: 2
         * Current Number: 3
         *
         * and it would end there, because the condition is checking if i is less than 4, and by the time it prints "Current Number: 3" it has changed to 4, and is no longer less than 4
         */
        public enum WretchedState
        {
            Start,
            Encircle
        }
    }
}