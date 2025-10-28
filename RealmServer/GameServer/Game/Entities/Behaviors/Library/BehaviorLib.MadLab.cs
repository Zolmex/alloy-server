#region
using Common;
using Common.ProjectilePaths;
using GameServer.Game.Entities.Behaviors.Actions;
using GameServer.Game.Entities.Behaviors.Classic;
using GameServer.Game.Entities.Behaviors.Transitions;
using GameServer.Game.Entities.Loot;
using System;
namespace GameServer.Game.Entities.Behaviors.Library
#endregion
{
    public partial class BehaviorLib
    {
        [CharacterBehavior("Turret Attack")]
        public static State TurretAttack =>
            new (
                new State("wait",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable, persist: true),
                    new TimedTransition(1500, TransitionType.Random, targetStates: ["shoot", "wait"])
                ),
                new State("shoot",
                    new Shoot(15,
                        new ProjectilePath(4000, new LinePath(4)),
                        count: 2,
                        shootAngle: 180,
                        fixedAngle: 90,
                        projName: "Science Fire Ball",
                        size: 60,
                        targeted: false,
                        cooldownMS: 3000,
                        coolDownOffset: 100,
                        damage: 80,
                        armorPiercing: true,
                        multiHit: true),
                    new TimedTransition(500, "wait")
                )
            );

        [CharacterBehavior("Mini Bot")]
        public static State MiniBot =>
            new (
                new State("p1",
                    new Wander(4F),
                    new Shoot(15,
                        new ProjectilePath(3000, new LinePath(4)),
                        count: 2,
                        shootAngle: 45,
                        projName: "Purple Energy Shot",
                        effects: (ConditionEffectIndex.Confused, 500),
                        targeted: true,
                        cooldownMS: 1000,
                        damage: 80)
                )
            );

        [CharacterBehavior("Rampage Cyborg")]
        public static State RampageCyborg =>
            new (
                new State("idle",
                    new EntityWithinTransition(radius: 10, target: "player", targetState: "shoot")
                ),
                new State("shoot",
                    new Follow(3.5F, distFromTarget: 0.5F, acquireRange: 10),
                    new Wander(3F),
                    new Shoot(15,
                        new ProjectilePath(3000, new LinePath(6)),
                        count: 1,
                        predictive: 1,
                        projName: "Yellow Energy Shot",
                        targeted: true,
                        cooldownMS: 800,
                        damage: 80),
                    new TimedTransition(10000, "rage"),
                    new HpLessTransition(0.2F, "rage")
                ),
                new State("rage",
                    new Flash(0xf0e68c, flashRepeats: 5, flashPeriod: 0.1),
                    new Follow(5.5F, distFromTarget: 0.5F, acquireRange: 10, cooldownMS: 100, followTimeMS: 100000),
                    new Wander(1F),
                    new Shoot(15,
                        new ProjectilePath(3000, new LinePath(6)),
                        count: 1,
                        predictive: 1,
                        projName: "Yellow Energy Shot",
                        targeted: true,
                        cooldownMS: 800,
                        damage: 80),
                    new TimedTransition(2000, "explode")
                ),
                new State("explode",
                    new Flash(0xfFF0000, 1, 9000001),
                    new Shoot(15,
                        new ProjectilePath(3000, new LinePath(8)),
                        count: 10,
                        shootAngle: 36,
                        projName: "Directed Explosion",
                        targeted: false,
                        cooldownMS: 1000000,
                        coolDownOffset: 1000,
                        damage: 100),
                    new Suicide(1100)
                )
            );

        [CharacterBehavior("Escaped Experiment")]
        public static State EscapedExperiment =>
            new (
                new State("p1",
                    new Wander(4F),
                    new Shoot(15,
                        new ProjectilePath(3000, new LinePath(3.5F)),
                        count: 1,
                        projName: "Robot Attack",
                        targeted: true,
                        cooldownMS: 800,
                        damage: 90)
                )
            );

        [CharacterBehavior("Enforcer Bot 3000")]
        public static State EnforcerBot3000 =>
            new (
                new State("p1",
                    new Wander(3F),
                    new Shoot(15,
                        new ProjectilePath(1000, new ChangeSpeedPath(3, -1F, cooldown: 400, repeat: 2)),
                        count: 3,
                        shootAngle: 15,
                        projName: "Lightning Blast",
                        targeted: true,
                        cooldownMS: 1500,
                        damage: 120),
                    new Shoot(15,
                        new ProjectilePath(2500, new LinePath(5F)),
                        count: 3,
                        shootAngle: 20,
                        projName: "Lightning Spiral",
                        targeted: true,
                        cooldownMS: 1500,
                        damage: 80),
                    new TransformOnDeath("Mini Bot", min: 1, max: 3)
                )
            );

        [CharacterBehavior("Crusher Abomination")]
        public static State CrusherAbomination =>
            new (
                new State("p1",
                    new Wander(4F),
                    new Shoot(15,
                        new ProjectilePath(3000, new LinePath(5F)),
                        count: 3,
                        shootAngle: 15,
                        size: 100,
                        projName: "Shockwave Blast",
                        targeted: true,
                        cooldownMS: 1000,
                        damage: 80),
                    new HpLessTransition(0.75F, "p2")
                ),
                new State("p2",
                    new Wander(4F),
                    new ChangeSize(11, 150),
                    new Shoot(15,
                        new ProjectilePath(3000, new LinePath(5.5F)),
                        count: 3,
                        size: 150,
                        shootAngle: 15,
                        projName: "Shockwave Blast",
                        targeted: true,
                        cooldownMS: 1000,
                        damage: 120),
                    new HpLessTransition(0.5F, "p3")
                ),
                new State("p3",
                    new Wander(4F),
                    new ChangeSize(11, 200),
                    new Shoot(15,
                        new ProjectilePath(3000, new LinePath(6F)),
                        count: 3,
                        size: 175,
                        shootAngle: 15,
                        projName: "Shockwave Blast",
                        targeted: true,
                        cooldownMS: 1000,
                        damage: 150),
                    new HpLessTransition(0.25F, "p4")
                ),
                new State("p4",
                    new Wander(4F),
                    new ChangeSize(11, 250),
                    new Shoot(15,
                        new ProjectilePath(3000, new LinePath(6.5F)),
                        count: 3,
                        size: 200,
                        shootAngle: 15,
                        projName: "Shockwave Blast",
                        targeted: true,
                        cooldownMS: 1000,
                        damage: 180)
                )
            );

        [CharacterBehavior("LabTransition")]
        public static State LabTransition =>
            new(
                new State("wait",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                    new RemoveObjectOnDeath("Partial Horizontal Catwalk", range: 30),
                    new TimedTransition(2000, "suicide")
                ),
                new State("suicide",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                    new Suicide(300)
                )
            );

        [CharacterBehavior("Mad Gas Controller")]
        public static State MadGasController =>
            new(
                new State("p1",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                    new Suicide(20000)

                )
            );

        [CharacterBehavior("LabTurret")]
        public static State LabTurret =>
            new (
                new State("wait",
                    new TimedTransition(1000, "shoot")
                ),
                new State("shoot",
                    new Shoot(15,
                        new ProjectilePath(3000, new AmplitudePath(4, amplitude: 1.5F, frequency: 1.2F)),
                        count: 2,
                        size: 60,
                        shootAngle: 15,
                        projName: "Science Fire Ball",
                        targeted: true,
                        cooldownMS: 5000,
                        coolDownOffset: 100,
                        damage: 80),
                    new Shoot(15,
                        new ProjectilePath(3000, new AmplitudePath(4, amplitude: 1.5F, frequency: 1.2F)),
                        count: 2,
                        size: 60,
                        shootAngle: 15,
                        projName: "Science Fire Ball",
                        targeted: true,
                        cooldownMS: 5000,
                        coolDownOffset: 200,
                        damage: 80),
                    new TimedTransition(1000, "wait")
                )
            );

        [CharacterBehavior("Red Gas Spawner UL")]
        public static State RedGasSpawnerUL =>
           new (
               new State("wait",
                   new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                   new EntityWithinTransition(target: "Mad Gas Controller", radius: 20, targetState: "GAS!")

               ),
               new State("GAS!",
                   new Shoot(15,
                        new ProjectilePath(12000, new LinePath(2)),
                        count: 10,
                        size: 80,
                        shootAngle: 36,
                        rotateAngle: 10,
                        projName: "Red Gas",
                        targeted: false,
                        cooldownMS: 1000,
                        coolDownOffset: 100,
                        multiHit: true,
                        armorPiercing: true,
                        damage: 40),
                   new EntityNotWithinTransition(target: "Mad Gas Controller", radius: 20, targetState: "stop")
               ),
               new State("stop"
               )
           );

        [CharacterBehavior("Red Gas Spawner UR")]
        public static State RedGasSpawnerUR =>
           new(
               new State("wait",
                   new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                   new EntityWithinTransition(target: "Mad Gas Controller", radius: 20, targetState: "GAS!")

               ),
               new State("GAS!",
                   new Shoot(15,
                        new ProjectilePath(12000, new LinePath(2)),
                        count: 10,
                        size: 80,
                        shootAngle: 36,
                        rotateAngle: 10,
                        projName: "Red Gas",
                        targeted: false,
                        cooldownMS: 1000,
                        coolDownOffset: 100,
                        multiHit: true,
                        armorPiercing: true,
                        damage: 40),
                   new EntityNotWithinTransition(target: "Mad Gas Controller", radius: 20, targetState: "stop")
               ),
               new State("stop"
               )
           );

        [CharacterBehavior("Red Gas Spawner LL")]
        public static State RedGasSpawnerLL =>
           new(
               new State("wait",
                   new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                   new EntityWithinTransition(target: "Mad Gas Controller", radius: 20, targetState: "GAS!")

               ),
               new State("GAS!",
                   new Shoot(15,
                        new ProjectilePath(12000, new LinePath(2)),
                        count: 10,
                        size: 80,
                        shootAngle: 36,
                        rotateAngle: 10,
                        projName: "Red Gas",
                        targeted: false,
                        cooldownMS: 1000,
                        coolDownOffset: 100,
                        multiHit: true,
                        armorPiercing: true,
                        damage: 40),
                   new EntityNotWithinTransition(target: "Mad Gas Controller", radius: 20, targetState: "stop")
               ),
               new State("stop"
               )
           );

        [CharacterBehavior("Red Gas Spawner LR")]
        public static State RedGasSpawnerLR =>
           new(
               new State("wait",
                   new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                   new EntityWithinTransition(target: "Mad Gas Controller", radius: 20, targetState: "GAS!")

               ),
               new State("GAS!",
                   new Shoot(15,
                        new ProjectilePath(12000, new LinePath(2)),
                        count: 10,
                        size: 80,
                        shootAngle: 36,
                        rotateAngle: 10,
                        projName: "Red Gas",
                        targeted: false,
                        cooldownMS: 1000,
                        coolDownOffset: 100,
                        multiHit: true,
                        armorPiercing: true,
                        damage: 40),
                   new EntityNotWithinTransition(target: "Mad Gas Controller", radius: 20, targetState: "stop")
               ),
               new State("stop"
               )
           );

        [CharacterBehavior("MadLabT")]
        public static State MadLabT =>
            new ( 
                new State("wait",
                    new EntityWithinTransition(target: "player", radius: 10, targetStates: "p1")
                ),
                new State("p1",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                    new EntityNotWithinTransition(target: "Monster Cage", radius: 200, targetState: "p2")
                ),
                new State("p2",
                    new Suicide(5000)
                )
            );

        [CharacterBehavior("Monster Cage")]
        public static State MonsterCage =>
            new (
                new State("wait",
                    new EntityWithinTransition(target: "player", radius: 10, targetState: "p1")
                ),
                new State("p1",
                    new Taunt(text: "Destroy me!", probability: 0.3F)
                )
            );

        [CharacterBehavior("Dr Terrible Bubble")]
        public static State DrTerribleBubble =>
            new (
                new State("wait",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                    new EntityWithinTransition(radius: 20, target: "player", targetState: "spawn")
                ),
                new State("spawn",
                    new Spawn("Dr Terrible", cooldownMs: 1000000),
                    new TimedTransition(6000, "idle")
                ),
                new State("idle",
                    new EntityWithinTransition(target: "LabTransition", radius: 20, targetState: "HIDING") 
                ),
                new State("HIDING",
                    new EntityWithinTransition(target: "Dr Terrible", radius: 1F, targetState: "HIDE")
                ),
                new State("HIDE",
                    new SetAltTexture(1),
                    new TimedTransition(2000, "taunt")
                ),
                new State("taunt",
                    new Taunt(text: "Gas Deployment Protocol initiated - Oxygen levels falling", coolDownMS: 10000),
                    new TimedTransition(1000, "taunt2")
                ),
                new State("taunt2",
                    new Taunt(text: "Termination expected in 20 seconds", coolDownMS: 10000),
                    new TimedTransition(2000, "Gas")
                ),
                new State("Gas",
                    new Spawn("Mad Gas Controller", cooldownMs: 100000),
                    new TimedTransition(1000, "idle2")
                ),
                new State("idle2",
                    new TimedTransition(28000, "T or not T!?!?")
                ),
                new State("T or not T!?!?",
                    new EntityWithinTransition(target: "MadLabT", radius: 60, targetState: "No T"),
                    new EntityNotWithinTransition(target: "MadLabT", radius: 60, targetState: "T")
                ),
                new State("No T",
                    new SetAltTexture(0),
                    new TimedTransition(2000, "idle3")
                ),
                new State("T",
                    new SetAltTexture(0),
                    new Spawn("Horrific Creation", cooldownMs: 10000),
                    new TimedTransition(2000, "idle4")
                ),
                new State("idle3",
                    new EntityNotWithinTransition(target: "Dr Terrible", radius: 20, targetState: "ground")
                ),
                new State("idle4",
                    new EntityNotWithinTransition(target: "Horrific Creation", radius: 20, targetState: "ground")

                ),
                new State("ground",
                    new ChangeGround(["Liquid Evil"], ["Lab Grate Floor"], dist: 10)
                )
            );

        [CharacterBehavior("Dr Terrible")]
        public static State DrTerrible =>
            new (
                new State("wait",
                    new DropPortalOnDeath("Realm Portal"),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new TimedTransition(2000, "taunt1")
                ),
                new State("taunt1",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Taunt("You dare trespass in my lab? I'll enjoy dissecting your remains!"),
                    new TimedTransition(1000, "ground")
                ),
                new State("ground",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Spawn("LabTransition", cooldownMs: 100000),
                    new TimedTransition(1000, "p1")
                ),
                new State("p1",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, 1000),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable, 1000),
                    new Wander(3F, distanceFromSpawn: 10),
                    new AOE(radius: 4, damage: 80, cooldownMs: 3000, cooldownOffset: 500, color: 0x1243D3, damageColor: 0x1243D3, throwTime: 1000, range: 10, effects: [(ConditionEffectIndex.Silenced, 2000)]),
                    new AOE(radius: 6, damage: 100, cooldownMs: 4000, cooldownOffset: 1500, color: 0xF4F2F7, damageColor: 0xF4F2F7, throwTime: 2000, range: 10, effects: [(ConditionEffectIndex.Quiet, 2000)]),
                    new Spawn("LabTurret", cooldownMs: 4000, cooldownOffsetMs: 3000, maxDensity: 3, densityRadius: 10),
                    new Shoot(15,
                        new ProjectilePath(3000, new ChangeSpeedPath(3, increment: 1, cooldown: 1000)),
                        count: 3,
                        size: 130,
                        shootAngle: 15,
                        projName: "Poison Fireball",
                        targeted: true,
                        cooldownMS: 2000,
                        coolDownOffset: 2000,
                        damage: 180),
                    new Shoot(15,
                        new ProjectilePath(3000, new ChangeSpeedPath(3, increment: 1, cooldown: 1000)),
                        count: 4,
                        size: 70,
                        shootAngle: 40,
                        projName: "Poison Fireball",
                        targeted: true,
                        cooldownMS: 2000,
                        coolDownOffset: 2000,
                        damage: 180),
                    new HpLessTransition(0.5F, "RETREAT!")
                ),
                new State("RETREAT!",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Taunt("You're standing in the pinnacle of innovation... and soon, in a puddle of your own blood!", coolDownMS: 10000000),
                    new Spawn("LabTransition", cooldownMs: 100000),
                    new ReturnToSpawn(4),
                    new EntityWithinTransition(target: "Dr Terrible Bubble", radius: 1F, targetState: "hide")
                ),
                new State("hide",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new SetAltTexture(1),
                    new TimedTransition(28000, "T or not T!?!?")
                ),
                new State("T or not T!?!?",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new EntityWithinTransition(target: "MadLabT", radius: 60, targetState: "No T"),
                    new EntityNotWithinTransition(target: "MadLabT", radius: 60, targetState: "T")

                ),
                new State("T",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible)

                ),
                new State("No T",
                    new SetAltTexture(0),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Taunt(text: "No fatalities? That gas system was flawless! Fine... I'll do it myself, for science!", coolDownMS: 100000),
                    new TimedTransition(2000, "p2")
                ),
                new State("p2",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, 2000),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable, 2000),
                    new Wander(3),
                    new Follow(3, distFromTarget: 1, acquireRange: 10),
                    new Shoot(15,
                        new ProjectilePath(3000, new LinePath(5))
                            .Then(1000, new AmplitudePath(-1, 2, 0.3F))
                            .Then(3000, new LinePath(-5)),
                        count: 8,
                        size: 150,
                        shootAngle: 45,
                        projName: "Yellow Energy Shot",
                        targeted: false,
                        multiHit: true,
                        armorPiercing: true,
                        cooldownMS: 2000,
                        coolDownOffset: 2000,
                        damage: 100),
                    new Shoot(15,
                        new ProjectilePath(1500, new LinePath(7))
                            .Then(500, new AmplitudePath(-1, 2, 0.3F))
                            .Then(1500, new LinePath(-7)),
                        count: 4,
                        size: 130,
                        shootAngle: 20,
                        projName: "Purple Energy Shot",
                        targeted: true,
                        multiHit: true,
                        cooldownMS: 2000,
                        coolDownOffset: 4000,
                        damage: 120),
                    new Shoot(15,
                        new ProjectilePath(3000, new ChangeSpeedPath(3, increment: 1, cooldown: 1000)),
                        count: 3,
                        size: 130,
                        shootAngle: 10,
                        projName: "Poison Fireball",
                        targeted: true,
                        cooldownMS: 3000,
                        coolDownOffset: 3000,
                        damage: 130)
                )
            );

        [CharacterBehavior("Horrific Creation")]
        public static State HorrificCreation =>
            new (
                new State("wait",
                    new DropPortalOnDeath("Realm Portal"),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new TimedTransition(2000, "taunt")
                ),
                new State("taunt",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Taunt(text: "Terrible… no more… Only HORROR remains!", coolDownMS: 100000),
                    new TimedTransition(2000, "jump")
                ),
                new State("jump",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, 2000),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable, 2000),
                    new Dash(radius: 15, numDashes: 1, dashSpeed: 10, dashRange: 7, cooldownMs: 100),
                    new TimedTransition(1000, "p1")
                ),
                new State("p1",
                    new Taunt(text: "RAHHHHHHH!", coolDownMS: 1000000),
                    new Shoot(15,
                        new ProjectilePath(3000, new ChangeSpeedPath(7, 1.5F, 500)),
                        count: 20,
                        size: 175,
                        shootAngle: 18,
                        projName: "H Creation Shockwave",
                        targeted: false,
                        multiHit: true,
                        armorPiercing: true,
                        cooldownMS: 100000,
                        coolDownOffset: 100,
                        damage: 100),
                    new TimedTransition(1500, "p2")
                ),
                new State("p2",
                    new Wander(3),
                    new Follow(3.5F, distFromTarget: 3, acquireRange: 20),
                    new Shoot(15,
                        new ProjectilePath(3000, new LinePath(8)),
                        count: 5,
                        size: 225,
                        shootAngle: 15,
                        projName: "H Creation Blast",
                        targeted: true,
                        multiHit: true,
                        cooldownMS: 1000,
                        coolDownOffset: 500,
                        damage: 150),
                    new Shoot(15,
                        new ProjectilePath(200, new LinePath(9))
                            .Then(200, new LinePath(-9)),
                        count: 10,
                        size: 100,
                        shootAngle: 36,
                        projName: "H Creation Wave",
                        targeted: true,
                        multiHit: true,
                        armorPiercing: true,
                        cooldownMS: 1500,
                        coolDownOffset: 1000,
                        damage: 150),
                    new Shoot(15,
                        new ProjectilePath(9000, new ChangeSpeedPath(10, -2, 1000)),
                        count: 20,
                        size: 100,
                        shootAngle: 18,
                        projName: "H Creation Energy",
                        targeted: true,
                        multiHit: true,
                        cooldownMS: 5000,
                        coolDownOffset: 3000,
                        damage: 150),
                    new TimedTransition(8000, TransitionType.Random, targetStates: ["jump", "p2"])
                )
            );
    }
}