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
        [CharacterBehavior("Tomb Bomb Trap Switch")]
        public static State TombBombTrapSwitch =>
            new (
                new State("Start",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                    new SetAltTexture(0),
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "wait")
                ),
                new State("wait",
                    new TimedTransition(200, "press")
                ),
                new State("press",
                    new SetAltTexture(1),
                    new TimedTransition(800, "Start")
                )
           );

        [CharacterBehavior("TombPuzzlePAT1")]
        public static State TombPuzzlePAT1 =>
            new (
                new State("Start",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                    new Suicide(1000)
                )
           );

        [CharacterBehavior("TombPuzzlePAT2")]
        public static State TombPuzzlePAT2 =>
            new(
                new State("Start",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                    new Suicide(1000)
                )
           );

        [CharacterBehavior("TombPuzzlePAT3")]
        public static State TombPuzzlePAT3 =>
            new (
                new State("Start",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                    new Suicide(1000)
                )
           );


        [CharacterBehavior("TombCorrect")]
        public static State TombCorrect =>
           new (
               new State("suicide",
                   new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                   new Suicide(100)
               )
          );

        [CharacterBehavior("TombWrong")]
        public static State TombWrong =>
           new (
               new State("suicide",
                   new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                   new Suicide(100)
               )
          );

        [CharacterBehavior("TombPATLEFT")]
        public static State TombPAT1LEFT =>
           new (
               new State("suicide",
                   new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                   new Suicide(1500)
               )
          );

        [CharacterBehavior("TombPATRIGHT")]
        public static State TombPAT1RIGHT =>
           new (
               new State("suicide",
                   new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                   new Suicide(1500)
               )
          );

        [CharacterBehavior("TombPATUP")]
        public static State TombPATUP =>
           new (
               new State("suicide",
                   new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                   new Suicide(1500)
               )
          );

        [CharacterBehavior("TombPATBOT")]
        public static State TombPATBOT =>
           new (
               new State("suicide",
                   new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                   new Suicide(1500)
               )
          );

        [CharacterBehavior("TombT")]
        public static State TombT =>
           new (
               new State("open",
                   new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                   new Suicide(100),
                   new RemoveObjectOnDeath("Tomb Wall Break", range: 15)
               )
          );

        [CharacterBehavior("TombPuzzle")]
        public static State TombPuzzle =>
            new (
                new State("wait",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                    new EntityWithinTransition(target: "player", radius: 10, targetState: "Start")
                ),
                new State("Start",
                    new Taunt(text: "Stand on the middle tile to begin the puzzle!", coolDownMS: 10000),
                    new EntityWithinTransition(target: "player", radius: 1, targetState: "spawn")
                ),
                new State("spawn",
                    new Taunt(text: "Press 4 plates in the correct sequence!", coolDownMS: 10000),
                    new TossObject("TombPuzzleUP", minAngle: 270, maxAngle: 270, minRange: 4, maxRange: 4, cooldownMS: 10000),
                    new TossObject("TombPuzzleLEFT", minAngle: 180, maxAngle: 180, minRange: 4, maxRange: 4, cooldownMS: 10000),
                    new TossObject("TombPuzzleRIGHT", minAngle: 360, maxAngle: 360, minRange: 4, maxRange: 4, cooldownMS: 10000),
                    new TossObject("TombPuzzleBOT", minAngle: 90, maxAngle: 90, minRange: 4, maxRange: 4, cooldownMS: 10000),
                    new TimedTransition(500, TransitionType.Random, ["Pattern1", "Pattern2", "Pattern3"])

                ),
                new State("FAIL1",
                    new Taunt(text: "The vault rejects the unworthy!", coolDownMS: 1000000),
                    new TimedTransition(1000, "FAIL2")
                ),
                new State("FAIL2",
                    new Taunt(text: "Return when the ancient order is known to you!", coolDownMS: 1000000)

                ),
                new State("OPEN",
                    new Spawn("TombT", cooldownMs: 100000, densityRadius: 10, maxDensity: 1),
                    new TimedTransition(500, "GGG")
                ),
                new State("GGG",
                    new Suicide(500)

                // 1
                ),
                new State("pat1-mistake",
                    new Taunt(text: "Wrong! You have 2 tries left!", coolDownMS: 10000),
                    new TimedTransition(500, TransitionType.Random, ["Pattern2-mistake1", "Pattern3-mistake1"])
                ),
                new State("pat1-mistake-2",
                    new Taunt(text: "Wrong! Last Chance!", coolDownMS: 10000),
                    new TimedTransition(500, TransitionType.Random, ["Pattern3-mistake1-2"])
                ),
                new State("pat1-mistake-3",
                    new Taunt(text: "Wrong! Last Chance!", coolDownMS: 10000),
                    new TimedTransition(500, TransitionType.Random, ["Pattern2-mistake1-3"])

                // 1 ^


                // 2
                ),
                new State("pat2-mistake",
                    new Taunt(text: "Wrong! You have 2 tries left!", coolDownMS: 10000),
                    new TimedTransition(500, TransitionType.Random, ["Pattern1-mistake2", "Pattern3-mistake2"])
                ),
                new State("pat2-mistake-1",
                    new Taunt(text: "Wrong! Last Chance!", coolDownMS: 10000),
                    new TimedTransition(500, TransitionType.Random, ["Pattern3-mistake2-1"])
                ),
                new State("pat2-mistake-3",
                    new Taunt(text: "Wrong! Last Chance!", coolDownMS: 10000),
                    new TimedTransition(500, TransitionType.Random, ["Pattern1-mistake2-3"])

                // 2 ^


                // 3
                ),
                new State("pat3-mistake",
                    new Taunt(text: "Wrong! You have 2 tries left!", coolDownMS: 10000),
                    new TimedTransition(500, TransitionType.Random, ["Pattern1-mistake3", "Pattern2-mistake3"])
                ),
                new State("pat3-mistake-1",
                    new Taunt(text: "Wrong! Last Chance!", coolDownMS: 10000),
                    new TimedTransition(500, TransitionType.Random, ["Pattern2-mistake3-1"])
                ),
                new State("pat3-mistake-2",
                    new Taunt(text: "Wrong! Last Chance!", coolDownMS: 10000),
                    new TimedTransition(500, TransitionType.Random, ["Pattern1-mistake3-2"])
                // 3 ^


                // 1
                ),
                new State("Pattern1",
                    new Spawn("TombPuzzlePAT1", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(600, "pat1-idle"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "1mistake")
                ),
                new State("pat1-idle",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "1/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "1mistake")
                ),
                new State("1/4",
                    new Taunt(text: "1/4!", coolDownMS: 10000),
                    new TimedTransition(600, "pat1-idle2"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "1mistake")
                 ),
                new State("pat1-idle2",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "2/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "1mistake")
                ),
                new State("2/4",
                    new Taunt(text: "2/4!", coolDownMS: 10000),
                    new TimedTransition(600, "pat1-idle3"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "1mistake")
                ),
                new State("pat1-idle3",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "3/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "1mistake")
                ),
                new State("3/4",
                    new Taunt(text: "3/4!", coolDownMS: 10000),
                    new TimedTransition(200, TransitionType.Random, targetStates: ["1left","1right"]),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "1mistake")
                ),
                new State("1left",
                    new Spawn("TombPATLEFT", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(500, "pat1-idle4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "1mistake")
                ),
                new State("1right",
                    new Spawn("TombPATRIGHT", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(500, "pat1-idle4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "1mistake")
                ),
                new State("pat1-idle4",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "1gg"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "1mistake")
                ),
                new State("1gg",
                    new Taunt(text: "4/4! You may access the ancient vault!", coolDownMS: 10000),
                    new TimedTransition(1000, "OPEN")
                ),
                new State("1mistake",
                    new TimedTransition(600, "pat1-mistake")
                // 1 ^


                // 2
                ),
                new State("Pattern2",
                    new Spawn("TombPuzzlePAT2", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(500, "pat2-idle")
                ),
                new State("pat2-idle",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "2-1/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "2mistake")
                ),
                new State("2-1/4",
                    new Taunt(text: "1/4!", coolDownMS: 10000),
                    new TimedTransition(600, "pat2-idle2")
                 ),
                new State("pat2-idle2",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "2-2/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "2mistake")
                ),
                new State("2-2/4",
                    new Taunt(text: "2/4!", coolDownMS: 10000),
                    new TimedTransition(600, "pat2-idle3")
                ),
                new State("pat2-idle3",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "2-3/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "2mistake")
                ),
                new State("2-3/4",
                    new Taunt(text: "3/4!", coolDownMS: 10000),
                    new TimedTransition(500, TransitionType.Random, targetStates: ["2bot", "2right"])
                ),
                new State("2bot",
                    new Spawn("TombPATBOT", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(500, "pat2-idle4")
                ),
                new State("2right",
                    new Spawn("TombPATRIGHT", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(500, "pat2-idle4")
                ),
                new State("pat2-idle4",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "2gg"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "2mistake")
                ),
                new State("2gg",
                    new Taunt(text: "4/4! You may access the ancient vault!", coolDownMS: 10000),
                    new TimedTransition(1000, "OPEN")
                ),
                new State("2mistake",
                    new TimedTransition(600, "pat2-mistake")

                // 2 ^

                // 3
                ),
                new State("Pattern3",
                    new Spawn("TombPuzzlePAT3", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(500, "pat3-idle")
                ),
                new State("pat3-idle",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "3-1/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "3mistake")
                ),
                new State("3-1/4",
                    new Taunt(text: "1/4!", coolDownMS: 10000),
                    new TimedTransition(600, "pat3-idle2")
                 ),
                new State("pat3-idle2",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "3-2/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "3mistake")
                ),
                new State("3-2/4",
                    new Taunt(text: "2/4!", coolDownMS: 10000),
                    new TimedTransition(600, "pat3-idle3")
                ),
                new State("pat3-idle3",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "3-3/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "3mistake")
                ),
                new State("3-3/4",
                    new Taunt(text: "3/4!", coolDownMS: 10000),
                    new TimedTransition(500, TransitionType.Random, targetStates: ["3up", "3bot"])
                ),
                new State("3bot",
                    new Spawn("TombPATBOT", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(500, "pat3-idle4")
                ),
                new State("3up",
                    new Spawn("TombPATUP", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(500, "pat3-idle4")
                ),
                new State("pat3-idle4",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "3gg"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "3mistake")
                ),
                new State("3gg",
                    new Taunt(text: "4/4! You may access the ancient vault!", coolDownMS: 10000),
                    new TimedTransition(1000, "OPEN")
                ),
                new State("3mistake",
                    new TimedTransition(600, "pat3-mistake")


                // 3 ^


                 // 2 - mistake 1
                ),
                new State("Pattern2-mistake1",
                    new Spawn("TombPuzzlePAT2", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(500, "pat2/1-idle")
                ),
                new State("pat2/1-idle",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "2/1-1/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "2/1mistake")
                ),
                new State("2/1-1/4",
                    new Taunt(text: "1/4!", coolDownMS: 10000),
                    new TimedTransition(600, "pat2/1-idle2")
                 ),
                new State("pat2/1-idle2",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "2/1-2/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "2/1mistake")
                ),
                new State("2/1-2/4",
                    new Taunt(text: "2/4!", coolDownMS: 10000),
                    new TimedTransition(600, "pat2/1-idle3")
                ),
                new State("pat2/1-idle3",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "2/1-3/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "2/1mistake")
                ),
                new State("2/1-3/4",
                    new Taunt(text: "3/4!", coolDownMS: 10000),
                    new TimedTransition(500, TransitionType.Random, targetStates: ["2/1bot", "2/1right"])
                ),
                new State("2/1bot",
                    new Spawn("TombPATBOT", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(500, "pat2/1-idle4")
                ),
                new State("2/1right",
                    new Spawn("TombPATRIGHT", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(500, "pat2/1-idle4")
                ),
                new State("pat2/1-idle4",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "2/1gg"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "2/1mistake")
                ),
                new State("2/1gg",
                    new Taunt(text: "4/4! You may access the ancient vault!", coolDownMS: 10000),
                    new TimedTransition(1000, "OPEN")
                ),
                new State("2/1mistake",
                    new TimedTransition(600, "pat1-mistake-2")

                 // 2 - mistake1 ^


                 // 3 - mistake1
                ),
                new State("Pattern3-mistake1",
                    new Spawn("TombPuzzlePAT3", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(500, "pat3/1-idle")
                ),
                new State("pat3/1-idle",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "3/1-1/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "3/1mistake")
                ),
                new State("3/1-1/4",
                    new Taunt(text: "1/4!", coolDownMS: 10000),
                    new TimedTransition(600, "pat3/1-idle2")
                 ),
                new State("pat3/1-idle2",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "3/1-2/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "3/1mistake")
                ),
                new State("3/1-2/4",
                    new Taunt(text: "2/4!", coolDownMS: 10000),
                    new TimedTransition(600, "pat3/1-idle3")
                ),
                new State("pat3/1-idle3",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "3/1-3/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "3/1mistake")
                ),
                new State("3/1-3/4",
                    new Taunt(text: "3/4!", coolDownMS: 10000),
                    new TimedTransition(500, TransitionType.Random, targetStates: ["3/1up", "3/1bot"])
                ),
                new State("3/1bot",
                    new Spawn("TombPATBOT", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(500, "pat3/1-idle4")
                ),
                new State("3/1up",
                    new Spawn("TombPATUP", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(500, "pat3/1-idle4")
                ),
                new State("pat3/1-idle4",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "3/1gg"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "3/1mistake")
                ),
                new State("3/1gg",
                    new Taunt(text: "4/4! You may access the ancient vault!", coolDownMS: 10000),
                    new TimedTransition(1000, "OPEN")
                ),
                new State("3/1mistake",
                    new TimedTransition(600, "pat1-mistake-3")

                // 3 - mistake 1 ^

                // 3 - mistake1/2
                ),
                new State("Pattern3-mistake1-2",
                    new Spawn("TombPuzzlePAT3", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(500, "pat3/1/2-idle")
                ),
                new State("pat3/1/2-idle",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "3/1/2-1/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "3/1/2mistake")
                ),
                new State("3/1/2-1/4",
                    new Taunt(text: "1/4!", coolDownMS: 10000),
                    new TimedTransition(600, "pat3/1/2-idle2")
                 ),
                new State("pat3/1/2-idle2",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "3/1/2-2/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "3/1/2mistake")
                ),
                new State("3/1/2-2/4",
                    new Taunt(text: "2/4!", coolDownMS: 10000),
                    new TimedTransition(600, "pat3/1/2-idle3")
                ),
                new State("pat3/1/2-idle3",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "3/1/2-3/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "3/1/2mistake")
                ),
                new State("3/1/2-3/4",
                    new Taunt(text: "3/4!", coolDownMS: 10000),
                    new TimedTransition(500, TransitionType.Random, targetStates: ["3/1/2up", "3/1/2bot"])
                ),
                new State("3/1/2bot",
                    new Spawn("TombPATBOT", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(500, "pat3/1/2-idle4")
                ),
                new State("3/1/2up",
                    new Spawn("TombPATUP", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(500, "pat3/1/2-idle4")
                ),
                new State("pat3/1/2-idle4",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "3/1/2gg"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "3/1/2mistake")
                ),
                new State("3/1/2gg",
                    new Taunt(text: "4/4! You may access the ancient vault!", coolDownMS: 10000),
                    new TimedTransition(1000, "OPEN")
                ),
                new State("3/1/2mistake",
                    new TimedTransition(600, "FAIL1")

                // 3 - mistake 1/2 ^

                // 2 - mistake 1/3
                ),
                new State("Pattern2-mistake1-3",
                    new Spawn("TombPuzzlePAT2", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(500, "pat2/1/3-idle")
                ),
                new State("pat2/1/3-idle",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "2/1/3-1/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "2/1/3mistake")
                ),
                new State("2/1/3-1/4",
                    new Taunt(text: "1/4!", coolDownMS: 10000),
                    new TimedTransition(600, "pat2/1/3-idle2")
                 ),
                new State("pat2/1/3-idle2",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "2/1/3-2/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "2/1/3mistake")
                ),
                new State("2/1/3-2/4",
                    new Taunt(text: "2/4!", coolDownMS: 10000),
                    new TimedTransition(600, "pat2/1/3-idle3")
                ),
                new State("pat2/1/3-idle3",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "2/1/3-3/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "2/1/3mistake")
                ),
                new State("2/1/3-3/4",
                    new Taunt(text: "3/4!", coolDownMS: 10000),
                    new TimedTransition(500, TransitionType.Random, targetStates: ["2/1/3bot", "2/1/3right"])
                ),
                new State("2/1/3bot",
                    new Spawn("TombPATBOT", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(500, "pat2/1/3-idle4")
                ),
                new State("2/1/3right",
                    new Spawn("TombPATRIGHT", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(500, "pat2/1/3-idle4")
                ),
                new State("pat2/1/3-idle4",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "2/1/3gg"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "2/1/3mistake")
                ),
                new State("2/1/3gg",
                    new Taunt(text: "4/4! You may access the ancient vault!", coolDownMS: 10000),
                    new TimedTransition(1000, "OPEN")
                ),
                new State("2/1/3mistake",
                    new TimedTransition(600, "FAIL1")

                 // 2 - mistake1-3 ^

                // 1 - mistake 2
                ),
                new State("Pattern1-mistake2",
                    new Spawn("TombPuzzlePAT1", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(500, "pat1/2-idle")
                ),
                new State("pat1/2-idle",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "1/2-1/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "1/2mistake")
                ),
                new State("1/2-1/4",
                    new Taunt(text: "1/4!", coolDownMS: 10000),
                    new TimedTransition(600, "pat1/2-idle2")
                ),
                new State("pat1/2-idle2",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "1/2-2/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "1/2mistake")
                ),
                new State("1/2-2/4",
                    new Taunt(text: "2/4!", coolDownMS: 10000),
                    new TimedTransition(600, "pat1/2-idle3")
                ),
                new State("pat1/2-idle3",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "1/2-3/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "1/2mistake")
                ),
                new State("1/2-3/4",
                    new Taunt(text: "3/4!", coolDownMS: 10000),
                    new TimedTransition(500, TransitionType.Random, targetStates: ["1/2left", "1/2right"])
                ),
                new State("1/2left",
                    new Spawn("TombPATLEFT", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(500, "pat1/2-idle4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "1/2mistake")
                ),
                new State("1/2right",
                    new Spawn("TombPATRIGHT", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(500, "pat1/2-idle4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "1/2mistake")
                ),
                new State("pat1/2-idle4",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "1/2gg"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "1/2mistake")
                ),
                new State("1/2gg",
                    new Taunt(text: "4/4! You may access the ancient vault!", coolDownMS: 10000),
                    new TimedTransition(1000, "OPEN")
                ),
                new State("1/2mistake",
                    new TimedTransition(600, "pat2-mistake-1")

                // 1 - mistake 2 ^

                ),
                new State("Pattern3-mistake2",
                    new Spawn("TombPuzzlePAT3", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(500, "pat3/2-idle")
                ),
                new State("pat3/2-idle",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "3/2-1/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "3/2mistake")
                ),
                new State("3/2-1/4",
                    new Taunt(text: "1/4!", coolDownMS: 10000),
                    new TimedTransition(600, "pat3/2-idle2")
                ),
                new State("pat3/2-idle2",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "3/2-2/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "3/2mistake")
                ),
                new State("3/2-2/4",
                    new Taunt(text: "2/4!", coolDownMS: 10000),
                    new TimedTransition(600, "pat3/2-idle3")
                ),
                new State("pat3/2-idle3",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "3/2-3/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "3/2mistake")
                ),
                new State("3/2-3/4",
                    new Taunt(text: "3/4!", coolDownMS: 10000),
                    new TimedTransition(500, TransitionType.Random, targetStates: ["3/2up", "3/2bot"])
                ),
                new State("3/2bot",
                    new Spawn("TombPATBOT", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(500, "pat3/1-idle4")
                ),
                new State("3/2up",
                    new Spawn("TombPATUP", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(500, "pat3/2-idle4")
                ),
                new State("pat3/2-idle4",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "3/2gg"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "3/2mistake")
                ),
                new State("3/2gg",
                    new Taunt(text: "4/4! You may access the ancient vault!", coolDownMS: 10000),
                    new TimedTransition(1000, "OPEN")
                ),
                new State("3/2mistake",
                    new TimedTransition(600, "pat2-mistake-3")
                // 3 - mistake 2

                // 3 - mistake2/1
                ),
                new State("Pattern3-mistake2-1",
                    new Spawn("TombPuzzlePAT3", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(500, "pat3/2/1-idle")
                ),
                new State("pat3/2/1-idle",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "3/2/1-1/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "3/2/1mistake")
                ),
                new State("3/2/1-1/4",
                    new Taunt(text: "1/4!", coolDownMS: 10000),
                    new TimedTransition(600, "pat3/2/1-idle2")
                 ),
                new State("pat3/2/1-idle2",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "3/2/1-2/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "3/2/1mistake")
                ),
                new State("3/2/1-2/4",
                    new Taunt(text: "2/4!", coolDownMS: 10000),
                    new TimedTransition(600, "pat3/2/1-idle3")
                ),
                new State("pat3/2/1-idle3",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "3/2/1-3/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "3/2/1mistake")
                ),
                new State("3/2/1-3/4",
                    new Taunt(text: "3/4!", coolDownMS: 10000),
                    new TimedTransition(500, TransitionType.Random, targetStates: ["3/2/1up", "3/2/1bot"])
                ),
                new State("3/2/1bot",
                    new Spawn("TombPATBOT", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(500, "pat3/2/1-idle4")
                ),
                new State("3/2/1up",
                    new Spawn("TombPATUP", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(500, "pat3/2/1-idle4")
                ),
                new State("pat3/2/1-idle4",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "3/2/1gg"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "3/2/1mistake")
                ),
                new State("3/2/1gg",
                    new Taunt(text: "4/4! You may access the ancient vault!", coolDownMS: 10000),
                    new TimedTransition(1000, "OPEN")
                ),
                new State("3/2/1mistake",
                    new TimedTransition(600, "FAIL1")

                // 3 - mistake 2/1 ^

                // 1 - mistake 2/3
                ),
                new State("Pattern1-mistake2-3",
                    new Spawn("TombPuzzlePAT1", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(500, "pat1/2/3-idle")
                ),
                new State("pat1/2/3-idle",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "1/2/3-1/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "1/2/3mistake")
                ),
                new State("1/2/3-1/4",
                    new Taunt(text: "1/4!", coolDownMS: 10000),
                    new TimedTransition(600, "pat1/2/3-idle2")
                ),
                new State("pat1/2/3-idle2",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "1/2/3-2/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "1/2/3mistake")
                ),
                new State("1/2/3-2/4",
                    new Taunt(text: "2/4!", coolDownMS: 10000),
                    new TimedTransition(600, "pat1/2/3-idle3")
                ),
                new State("pat1/2/3-idle3",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "1/2/3-3/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "1/2/3mistake")
                ),
                new State("1/2/3-3/4",
                    new Taunt(text: "3/4!", coolDownMS: 10000),
                    new TimedTransition(500, TransitionType.Random, targetStates: ["1/2/3left", "1/2/3right"])
                ),
                new State("1/2/3left",
                    new Spawn("TombPATLEFT", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(500, "pat1/2/3-idle4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "1/2/3mistake")
                ),
                new State("1/2/3right",
                    new Spawn("TombPATRIGHT", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(500, "pat1/2/3-idle4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "1/2/3mistake")
                ),
                new State("pat1/2/3-idle4",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "1/2/3gg"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "1/2/3mistake")
                ),
                new State("1/2/3gg",
                    new Taunt(text: "4/4! You may access the ancient vault!", coolDownMS: 10000),
                    new TimedTransition(1000, "OPEN")
                ),
                new State("1/2/3mistake",
                    new TimedTransition(600, "FAIL1")

                // 1 - mistake 2/3 ^

                // 1 - mistake 3
                ),
                new State("Pattern1-mistake3",
                    new Spawn("TombPuzzlePAT1", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(500, "pat1/3-idle")
                ),
                new State("pat1/3-idle",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "1/3-1/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "1/3mistake")
                ),
                new State("1/3-1/4",
                    new Taunt(text: "1/4!", coolDownMS: 10000),
                    new TimedTransition(600, "pat1/3-idle2")
                ),
                new State("pat1/3-idle2",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "1/3-2/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "1/3mistake")
                ),
                new State("1/3-2/4",
                    new Taunt(text: "2/4!", coolDownMS: 10000),
                    new TimedTransition(600, "pat1/3-idle3")
                ),
                new State("pat1/3-idle3",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "1/3-3/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "1/3mistake")
                ),
                new State("1/3-3/4",
                    new Taunt(text: "3/4!", coolDownMS: 10000),
                    new TimedTransition(500, TransitionType.Random, targetStates: ["1/3left", "1/3right"])
                ),
                new State("1/3left",
                    new Spawn("TombPATLEFT", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(500, "pat1/3-idle4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "1/3mistake")
                ),
                new State("1/3right",
                    new Spawn("TombPATRIGHT", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(500, "pat1/3-idle4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "1/3mistake")
                ),
                new State("pat1/3-idle4",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "1/3gg"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "1/3mistake")
                ),
                new State("1/3gg",
                    new Taunt(text: "4/4! You may access the ancient vault!", coolDownMS: 10000),
                    new TimedTransition(1000, "OPEN")
                ),
                new State("1/3mistake",
                    new TimedTransition(600, "pat3-mistake-1")

                // 1 - mistake 3 ^

                // 2 - mistake 3
                ),
                new State("Pattern2-mistake3",
                    new Spawn("TombPuzzlePAT2", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(500, "pat2/3-idle")
                ),
                new State("pat2/3-idle",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "2/3-1/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "2/3mistake")
                ),
                new State("2/3-1/4",
                    new Taunt(text: "1/4!", coolDownMS: 10000),
                    new TimedTransition(600, "pat2/3-idle2")
                 ),
                new State("pat2/3-idle2",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "2/3-2/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "2/3mistake")
                ),
                new State("2/3-2/4",
                    new Taunt(text: "2/4!", coolDownMS: 10000),
                    new TimedTransition(600, "pat2/3-idle3")
                ),
                new State("pat2/3-idle3",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "2/3-3/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "2/3mistake")
                ),
                new State("2/3-3/4",
                    new Taunt(text: "3/4!", coolDownMS: 10000),
                    new TimedTransition(500, TransitionType.Random, targetStates: ["2/3bot", "2/3right"])
                ),
                new State("2/3bot",
                    new Spawn("TombPATBOT", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(500, "pat2/3-idle4")
                ),
                new State("2/3right",
                    new Spawn("TombPATRIGHT", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(500, "pat2/3-idle4")
                ),
                new State("pat2/3-idle4",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "2/3gg"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "2/3mistake")
                ),
                new State("2/3gg",
                    new Taunt(text: "4/4! You may access the ancient vault!", coolDownMS: 10000),
                    new TimedTransition(1000, "OPEN")
                ),
                new State("2/3mistake",
                    new TimedTransition(600, "pat3-mistake-2")

                // 2 - mistake3 ^

                // 2 - mistake 3/1
                ),
                new State("Pattern2-mistake3-1",
                    new Spawn("TombPuzzlePAT2", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(500, "pat2/3/1-idle")
                ),
                new State("pat2/3/1-idle",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "2/3/1-1/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "2/3/1mistake")
                ),
                new State("2/3/1-1/4",
                    new Taunt(text: "1/4!", coolDownMS: 10000),
                    new TimedTransition(600, "pat2/3/1-idle2")
                 ),
                new State("pat2/3/1-idle2",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "2/3/1-2/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "2/3/1mistake")
                ),
                new State("2/3/1-2/4",
                    new Taunt(text: "2/4!", coolDownMS: 10000),
                    new TimedTransition(600, "pat2/3/1-idle3")
                ),
                new State("pat2/3/1-idle3",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "2/3/1-3/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "2/3/1mistake")
                ),
                new State("2/3/1-3/4",
                    new Taunt(text: "3/4!", coolDownMS: 10000),
                    new TimedTransition(500, TransitionType.Random, targetStates: ["2/3/1bot", "2/3/1right"])
                ),
                new State("2/3/1bot",
                    new Spawn("TombPATBOT", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(500, "pat2/3/1-idle4")
                ),
                new State("2/3/1right",
                    new Spawn("TombPATRIGHT", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(500, "pat2/3/1-idle4")
                ),
                new State("pat2/3/1-idle4",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "2/3/1gg"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "2/3/1mistake")
                ),
                new State("2/3/1gg",
                    new Taunt(text: "4/4! You may access the ancient vault!", coolDownMS: 10000),
                    new TimedTransition(1000, "OPEN")
                ),
                new State("2/3/1mistake",
                    new TimedTransition(600, "FAIL1")

                // 2 - mistake3-1 ^

                // 1 - mistake 3/2
                ),
                new State("Pattern1-mistake3-2",
                    new Spawn("TombPuzzlePAT1", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(500, "pat1/3/2-idle")
                ),
                new State("pat1/3/2-idle",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "1/3/2-1/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "1/3/2mistake")
                ),
                new State("1/3/2-1/4",
                    new Taunt(text: "1/4!", coolDownMS: 10000),
                    new TimedTransition(600, "pat1/3/2-idle2")
                ),
                new State("pat1/3/2-idle2",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "1/3/2-2/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "1/3/2mistake")
                ),
                new State("1/3/2-2/4",
                    new Taunt(text: "2/4!", coolDownMS: 10000),
                    new TimedTransition(600, "pat1/3/2-idle3")
                ),
                new State("pat1/3/2-idle3",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "1/3/2-3/4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "1/3/2mistake")
                ),
                new State("1/3/2-3/4",
                    new Taunt(text: "3/4!", coolDownMS: 10000),
                    new TimedTransition(500, TransitionType.Random, targetStates: ["1/3/2left", "1/3/2right"])
                ),
                new State("1/3/2left",
                    new Spawn("TombPATLEFT", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(500, "pat1/3/2-idle4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "1/3/2mistake")
                ),
                new State("1/3/2right",
                    new Spawn("TombPATRIGHT", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(500, "pat1/3/2-idle4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "1/3/2mistake")
                ),
                new State("pat1/3/2-idle4",
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "1/3/2gg"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "1/3/2mistake")
                ),
                new State("1/3/2gg",
                    new Taunt(text: "4/4! You may access the ancient vault!", coolDownMS: 10000),
                    new TimedTransition(1000, "OPEN")
                ),
                new State("1/3/2mistake",
                    new TimedTransition(600, "FAIL1")
                )
                
                 // 1 - mistake 3/2
           );

        [CharacterBehavior("TombPuzzleUP")]
        public static State TombPuzzleUP =>
            new (
                new State("Start",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                    new EntityWithinTransition(target: "TombPuzzlePAT1", radius: 20, targetState: "Start1"),
                    new EntityWithinTransition(target: "TombPuzzlePAT2", radius: 20, targetState: "Start2"),
                    new EntityWithinTransition(target: "TombPuzzlePAT3", radius: 20, targetState: "Start3")
                ),
                new State("Start1",
                    new EntityWithinTransition(target: "TombPuzzlePAT1", radius: 10, targetState: "pat1")
                ),
                new State("Start2",
                    new EntityWithinTransition(target: "TombPuzzlePAT2", radius: 10, targetState: "pat2")
                ),
                new State("Start3",
                    new EntityWithinTransition(target: "TombPuzzlePAT3", radius: 10, targetState: "pat3")

                ),
                new State("pat1-mistake",
                    new TimedTransition(1000, "pat1-mistake-2/3")
                ),
                new State("pat1-mistake-2/3",
                    new EntityWithinTransition(target: "TombPuzzlePAT2", radius: 10, targetState: "pat2"),
                    new EntityWithinTransition(target: "TombPuzzlePAT3", radius: 10, targetState: "pat3")

                ),
                new State("pat2-mistake",
                    new TimedTransition(1000, "pat2-mistake-1/3")
                ),
                new State("pat2-mistake-1/3",
                    new EntityWithinTransition(target: "TombPuzzlePAT1", radius: 10, targetState: "pat1"),
                    new EntityWithinTransition(target: "TombPuzzlePAT3", radius: 10, targetState: "pat3")

                ),
                new State("pat3-mistake",
                    new TimedTransition(1000, "pat3-mistake-1/2")
                ),
                new State("pat3-mistake-1/2",
                    new EntityWithinTransition(target: "TombPuzzlePAT1", radius: 10, targetState: "pat1"),
                    new EntityWithinTransition(target: "TombPuzzlePAT2", radius: 10, targetState: "pat2")


                // 1
                ),
                new State("pat1",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "1correct"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat1-mistake"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat1-mistake")
                ),
                new State("pat1-2",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "1wrong"),
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "pat1-3"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat1-mistake")
                ),
                new State("pat1-3",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "correct2"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat1-mistake"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat1-mistake")
                ),
                new State("pat1-4",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "1wrong"),
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "1gg"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat1-mistake")
                ),
                new State("1correct",
                    new Spawn("TombCorrect", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(600, "pat1-2")
                ),
                new State("correct2",
                    new Spawn("TombCorrect", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(600, "pat1-4")
                ),
                new State("1gg",
                    new Suicide(500)
                ),
                new State("1wrong",
                    new Spawn("TombWrong", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(200, "pat1-mistake")

                // 1 ^

                // 2
                ),
                new State("pat2",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "2wrong"),
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "pat2-2-wait"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat2-mistake")
                ),
                new State("pat2-2-wait",
                    new TimedTransition(600, "pat2-2"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat2-mistake")
                ),
                new State("pat2-2",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "2wrong"),
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "pat2-3"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat2-mistake")
                ),
                new State("pat2-3",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "2correct"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat2-mistake")
                ),
                new State("pat2-4",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "2wrong"),
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "2gg"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat2-mistake")
                ),
                new State("2correct",
                    new Spawn("TombCorrect", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(600, "pat2-4")
                ),
                new State("2gg",
                    new Suicide(500)
                ),
                new State("2wrong",
                    new Spawn("TombWrong", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(200, "pat2-mistake")

                // 2 ^

                // 3
                ),
                new State("pat3",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "3wrong"),
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "pat3-2-wait"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat3-mistake")
                ),
                new State("pat3-2-wait",
                    new TimedTransition(600, "pat3-2")
                ),
                new State("pat3-2",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "3wrong"),
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "pat3-3"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat3-mistake")
                ),
                new State("pat3-3",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "3wrong"),
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "pat3-4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat3-mistake")
                ),
                new State("pat3-4",
                    new EntityWithinTransition(target: "TombPATUP", radius: 20, targetState: "pat3UP"),
                    new EntityWithinTransition(target: "TombPATBOT", radius: 20, targetState: "pat3BOT"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat3-mistake")
                ),
                new State("pat3UP",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "3correct"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat3-mistake")
                ),
                new State("pat3BOT",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "3wrong"),
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "3gg"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat3-mistake")
                ),
                new State("3correct",
                    new Spawn("TombCorrect", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(600, "3gg")
                ),
                new State("3gg",
                    new Suicide(500)
                ),
                new State("3wrong",
                    new Spawn("TombWrong", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(200, "pat3-mistake")
                )

                // 3 ^


           );

        [CharacterBehavior("TombPuzzleLEFT")]
        public static State TombPuzzleLEFT =>
            new (
                new State("Start",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                    new EntityWithinTransition(target: "TombPuzzlePAT1", radius: 20, targetState: "Start1"),
                    new EntityWithinTransition(target: "TombPuzzlePAT2", radius: 20, targetState: "Start2"),
                    new EntityWithinTransition(target: "TombPuzzlePAT3", radius: 20, targetState: "Start3")
                ),
                new State("Start1",
                    new EntityWithinTransition(target: "TombPuzzlePAT1", radius: 10, targetState: "pat1")
                ),
                new State("Start2",
                    new EntityWithinTransition(target: "TombPuzzlePAT2", radius: 10, targetState: "pat2")
                ),
                new State("Start3",
                    new EntityWithinTransition(target: "TombPuzzlePAT3", radius: 10, targetState: "pat3")

                ),
                new State("pat1-mistake",
                    new TimedTransition(1000, "pat1-mistake-2/3")
                ),
                new State("pat1-mistake-2/3",
                    new EntityWithinTransition(target: "TombPuzzlePAT2", radius: 10, targetState: "pat2"),
                    new EntityWithinTransition(target: "TombPuzzlePAT3", radius: 10, targetState: "pat3")

                ),
                new State("pat2-mistake",
                    new TimedTransition(1000, "pat2-mistake-1/3")
                ),
                new State("pat2-mistake-1/3",
                    new EntityWithinTransition(target: "TombPuzzlePAT1", radius: 10, targetState: "pat1"),
                    new EntityWithinTransition(target: "TombPuzzlePAT3", radius: 10, targetState: "pat3")

                ),
                new State("pat3-mistake",
                    new TimedTransition(1000, "pat3-mistake-1/2")
                ),
                new State("pat3-mistake-1/2",
                    new EntityWithinTransition(target: "TombPuzzlePAT1", radius: 10, targetState: "pat1"),
                    new EntityWithinTransition(target: "TombPuzzlePAT2", radius: 10, targetState: "pat2")


                // 1
                ),
                new State("pat1",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "1wrong"),
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "pat1-2-wait"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat1-mistake")
                ),
                new State("pat1-2-wait",
                    new TimedTransition(600, "pat1-2"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat1-mistake")
                ),
                new State("pat1-2",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "1wrong"),
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "pat1-3-wait"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat1-mistake")
                ),
                new State("pat1-3-wait",
                    new TimedTransition(600, "pat1-3"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat1-mistake")
                ),
                new State("pat1-3",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "1wrong"),
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "pat1-4-wait"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat1-mistake")
                ),
                new State("pat1-4-wait",
                    new TimedTransition(600, "pat1-4")
                ),
                new State("pat1-4",
                    new EntityWithinTransition(target: "TombPATLEFT", radius: 20, targetState: "pat1LEFT"),
                    new EntityWithinTransition(target: "TombPATRIGHT", radius: 20, targetState: "pat1RIGHT"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat1-mistake")
                ),
                new State("pat1LEFT",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "1correct"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat1-mistake")
                ),
                new State("pat1RIGHT",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "1wrong"),
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "1gg"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat1-mistake")
                ),
                new State("1gg",
                    new Suicide(500)
                ),
                new State("1correct",
                    new Spawn("TombCorrect", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(600, "1gg")

                ),
                new State("1wrong",
                    new Spawn("TombWrong", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(200, "pat1-mistake")


                // 1 ^

                // 2
                ),
                new State("pat2",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "2correct"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat2-mistake")
                ),
                new State("pat2-2",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "2wrong"),
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "pat2-3-wait"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat2-mistake")
                ),
                new State("pat2-3-wait",
                    new TimedTransition(600, "pat2-3"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat2-mistake")
                ),
                new State("pat2-3",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "2wrong"),
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "pat2-4-wait"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat2-mistake")
                ),
                new State("pat2-4-wait",
                    new TimedTransition(600, "pat2-4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat2-mistake")
                ),
                new State("pat2-4",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "2wrong"),
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "2gg"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat2-mistake")
                ),
                new State("2gg",
                    new Suicide(500)
                ),
                new State("2correct",
                    new Spawn("TombCorrect", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(600, "pat2-2")

                ),
                new State("2wrong",
                    new Spawn("TombWrong", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(200, "pat2-mistake")

                // 2 ^


                // 3
                ),
                new State("pat3",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "3wrong"),
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "pat3-2-wait"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat3-mistake")
                ),
                new State("pat3-2-wait",
                    new TimedTransition(600, "pat3-2"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat3-mistake")
                ),
                new State("pat3-2",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "3correct"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat3-mistake")
                ),
                new State("pat3-3",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "3wrong"),
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "pat3-4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat3-mistake")
                ),
                new State("pat3-4",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "3wrong"),
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "3gg"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat3-mistake")
                ),
                new State("3correct",
                    new Spawn("TombCorrect", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(600, "pat3-3")
                ),
                new State("3gg",
                    new Suicide(500)
                ),
                new State("3wrong",
                    new Spawn("TombWrong", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(200, "pat3-mistake")
                )

                // 3 ^
                
           );

        [CharacterBehavior("TombPuzzleRIGHT")]
        public static State TombPuzzleRIGHT =>
            new (
                new State("Start",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                    new EntityWithinTransition(target: "TombPuzzlePAT1", radius: 20, targetState: "Start1"),
                    new EntityWithinTransition(target: "TombPuzzlePAT2", radius: 20, targetState: "Start2"),
                    new EntityWithinTransition(target: "TombPuzzlePAT3", radius: 20, targetState: "Start3")
                ),
                new State("Start1",
                    new EntityWithinTransition(target: "TombPuzzlePAT1", radius: 10, targetState: "pat1")
                ),
                new State("Start2",
                    new EntityWithinTransition(target: "TombPuzzlePAT2", radius: 10, targetState: "pat2")
                ),
                new State("Start3",
                    new EntityWithinTransition(target: "TombPuzzlePAT3", radius: 10, targetState: "pat3")


                ),
                new State("pat1-mistake",
                    new TimedTransition(1000, "pat1-mistake-2/3")
                ),
                new State("pat1-mistake-2/3",
                    new EntityWithinTransition(target: "TombPuzzlePAT2", radius: 10, targetState: "pat2"),
                    new EntityWithinTransition(target: "TombPuzzlePAT3", radius: 10, targetState: "pat3")

                ),
                new State("pat2-mistake",
                    new TimedTransition(1000, "pat2-mistake-1/3")
                ),
                new State("pat2-mistake-1/3",
                    new EntityWithinTransition(target: "TombPuzzlePAT1", radius: 10, targetState: "pat1"),
                    new EntityWithinTransition(target: "TombPuzzlePAT3", radius: 10, targetState: "pat3")

                ),
                new State("pat3-mistake",
                    new TimedTransition(1000, "pat3-mistake-1/2")
                ),
                new State("pat3-mistake-1/2",
                    new EntityWithinTransition(target: "TombPuzzlePAT1", radius: 10, targetState: "pat1"),
                    new EntityWithinTransition(target: "TombPuzzlePAT2", radius: 10, targetState: "pat2")


                // 1
                ),
                new State("pat1",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "1wrong"),
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "pat1-2-wait"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat1-mistake")
                ),
                new State("pat1-2-wait",
                    new TimedTransition(600, "pat1-2"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat1-mistake")
                ),
                new State("pat1-2",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "1wrong"),
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "pat1-3-wait"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat1-mistake")
                ),
                new State("pat1-3-wait",
                    new TimedTransition(600, "pat1-3"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat1-mistake")
                ),
                new State("pat1-3",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "1wrong"),
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "pat1-4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat1-mistake")
                ),
                new State("pat1-4",
                    new EntityWithinTransition(target: "TombPATLEFT", radius: 20, targetState: "pat1LEFT"),
                    new EntityWithinTransition(target: "TombPATRIGHT", radius: 20, targetState: "pat1RIGHT"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat1-mistake")
                ),
                new State("pat1RIGHT",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "1correct"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat1-mistake")
                ),
                new State("pat1LEFT",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "1wrong"),
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "1gg"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat1-mistake")
                ),
                new State("1gg",
                    new Suicide(500)
                ),
                new State("1correct",
                    new Spawn("TombCorrect", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(600, "1gg")

                ),
                new State("1wrong",
                    new Spawn("TombWrong", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(200, "pat1-mistake")

                // 1 ^


                // 2
                ),
                new State("pat2",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "2wrong"),
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "pat2-2-wait"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat2-mistake")
                ),
                new State("pat2-2-wait",
                    new TimedTransition(600, "pat2-2"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat2-mistake")
                ),
                new State("pat2-2",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "2correct"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat2-mistake")

                ),
                new State("pat2-3-wait",
                    new TimedTransition(200, "pat2-3"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat2-mistake")
                ),
                new State("pat2-3",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "2wrong"),
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "pat2-4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat2-mistake")
                ),
                new State("pat2-4",
                    new EntityWithinTransition(target: "TombPATBOT", radius: 20, targetState: "pat2BOT"),
                    new EntityWithinTransition(target: "TombPATRIGHT", radius: 20, targetState: "pat2RIGHT"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat2-mistake")
                ),
                new State("pat2RIGHT",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "2correct2"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat2-mistake")
                ),
                new State("pat2BOT",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "2wrong"),
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "2gg"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat2-mistake")
                ),
                new State("2gg",
                    new Suicide(500)
                ),
                new State("2correct",
                    new Spawn("TombCorrect", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(600, "pat2-3-wait")
                ),
                new State("2correct2",
                    new Spawn("TombCorrect", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(600, "2gg")

                ),
                new State("2wrong",
                    new Spawn("TombWrong", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(200, "pat2-mistake")

                // 2 ^

                // 3
                ),
                new State("pat3",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "3wrong"),
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "pat3-2-wait"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat3-mistake")
                ),
                new State("pat3-2-wait",
                    new TimedTransition(600, "pat3-2"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat3-mistake")
                ),
                new State("pat3-2",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "3wrong"),
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "pat3-3-wait"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat3-mistake")
                ),
                new State("pat3-3-wait",
                    new TimedTransition(600, "pat3-3"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat3-mistake")
                ),
                new State("pat3-3",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "3correct"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat3-mistake")
                ),
                new State("pat3-4",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "3wrong"),
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "3gg"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat3-mistake")
                ),
                new State("3correct",
                    new Spawn("TombCorrect", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(600, "pat3-4")
                ),
                new State("3gg",
                    new Suicide(500)
                ),
                new State("3wrong",
                    new Spawn("TombWrong", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(200, "pat3-mistake")
                )

                // 3 ^

           );

        [CharacterBehavior("TombPuzzleBOT")]
        public static State TombPuzzleBOT =>
            new (
                new State("Start",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                    new EntityWithinTransition(target: "TombPuzzlePAT1", radius: 20, targetState: "Start1"),
                    new EntityWithinTransition(target: "TombPuzzlePAT2", radius: 20, targetState: "Start2"),
                    new EntityWithinTransition(target: "TombPuzzlePAT3", radius: 20, targetState: "Start3")
                ),
                new State("Start1",
                    new EntityWithinTransition(target: "TombPuzzlePAT1", radius: 10, targetState: "pat1")
                ),
                new State("Start2",
                    new EntityWithinTransition(target: "TombPuzzlePAT2", radius: 10, targetState: "pat2")
                ),
                new State("Start3",
                    new EntityWithinTransition(target: "TombPuzzlePAT3", radius: 10, targetState: "pat3")


                ),
                new State("pat1-mistake",
                    new TimedTransition(1000, "pat1-mistake-2/3")
                ),
                new State("pat1-mistake-2/3",
                    new EntityWithinTransition(target: "TombPuzzlePAT2", radius: 10, targetState: "pat2"),
                    new EntityWithinTransition(target: "TombPuzzlePAT3", radius: 10, targetState: "pat3")

                ),
                new State("pat2-mistake",
                    new TimedTransition(1000, "pat2-mistake-1/3")
                ),
                new State("pat2-mistake-1/3",
                    new EntityWithinTransition(target: "TombPuzzlePAT1", radius: 10, targetState: "pat1"),
                    new EntityWithinTransition(target: "TombPuzzlePAT3", radius: 10, targetState: "pat3")

                ),
                new State("pat3-mistake",
                    new TimedTransition(1000, "pat3-mistake-1/2")
                ),
                new State("pat3-mistake-1/2",
                    new EntityWithinTransition(target: "TombPuzzlePAT1", radius: 10, targetState: "pat1"),
                    new EntityWithinTransition(target: "TombPuzzlePAT2", radius: 10, targetState: "pat2")



                 // 1
                 ),
                new State("pat1",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "1wrong"),
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "pat1-2"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat1-mistake")
                ),
                new State("pat1-2",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "1correct"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat1-mistake")
                ),
                new State("pat1-3",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "1wrong"),
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "pat1-4-wait"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat1-mistake")
                ),
                new State("pat1-4-wait",
                    new TimedTransition(600, "pat1-4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat1-mistake")
                ),
                new State("pat1-4",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "1wrong"),
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "1gg"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat1-mistake")
                ),
                new State("1correct",
                    new Spawn("TombCorrect", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(600, "pat1-3")
                ),
                new State("1wrong",
                    new Spawn("TombWrong", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(200, "pat1-mistake")
                ),
                new State("1gg",
                    new Suicide(500)

                 // 1 ^

                 // 2
                 ),
                new State("pat2",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "2wrong"),
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "pat2-2-wait"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat2-mistake")
                ),
                new State("pat2-2-wait",
                    new TimedTransition(600, "pat2-2"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat2-mistake")
                ),
                new State("pat2-2",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "2wrong"),
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "pat2-3-wait"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat2-mistake")
                ),
                new State("pat2-3-wait",
                    new TimedTransition(600, "pat2-3"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat2-mistake")
                ),
                new State("pat2-3",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "2wrong"),
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "pat2-4-wait"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat2-mistake")
                ),
                new State("pat2-4-wait",
                    new TimedTransition(600, "pat2-4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat2-mistake")
                ),
                new State("pat2-4",
                    new EntityWithinTransition(target: "TombPATBOT", radius: 20, targetState: "pat2BOT"),
                    new EntityWithinTransition(target: "TombPATRIGHT", radius: 20, targetState: "pat2RIGHT"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat2-mistake")
                ),
                new State("pat2RIGHT",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "2wrong"),
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "2gg"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat2-mistake")
                ),
                new State("pat2BOT",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "2correct"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat2-mistake")

                ),
                new State("2correct",
                    new Spawn("TombCorrect", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(600, "2gg")
                ),
                new State("2wrong",
                    new Spawn("TombWrong", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(200, "pat2-mistake")
                ),
                new State("2gg",
                    new Suicide(500)

                // 2 ^


                // 3
                ),
                new State("pat3",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "3correct"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat3-mistake")
                ),
                new State("pat3-2",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "3wrong"),
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "pat3-3-wait"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat3-mistake")
                ),
                new State("pat3-3-wait",
                    new TimedTransition(600, "pat3-3"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat3-mistake")
                ),
                new State("pat3-3",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "3wrong"),
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "pat3-4-wait"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat3-mistake")
                ),
                new State("pat3-4-wait",
                    new TimedTransition(600, "pat3-4"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat3-mistake")
                ),
                new State("pat3-4",
                    new EntityWithinTransition(target: "TombPATUP", radius: 20, targetState: "pat3UP"),
                    new EntityWithinTransition(target: "TombPATBOT", radius: 20, targetState: "pat3BOT"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat3-mistake")
                ),
                new State("pat3UP",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "3wrong"),
                    new EntityWithinTransition(target: "TombCorrect", radius: 20, targetState: "3gg"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat3-mistake")
                    
                ),
                new State("pat3BOT",
                    new EntityWithinTransition(target: "player", radius: 0.6F, targetState: "3correct2"),
                    new EntityWithinTransition(target: "TombWrong", radius: 20, targetState: "pat3-mistake")
                ),
                new State("3correct",
                    new Spawn("TombCorrect", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(600, "pat3-2")
                ),
                new State("3correct2",
                    new Spawn("TombCorrect", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(600, "3gg")
                ),
                new State("3gg",
                    new Suicide(500)
                ),
                new State("3wrong",
                    new Spawn("TombWrong", cooldownMs: 100000, maxDensity: 1, densityRadius: 10),
                    new TimedTransition(200, "pat3-mistake")
                )

                // 3 ^
                
           );

    }
}
