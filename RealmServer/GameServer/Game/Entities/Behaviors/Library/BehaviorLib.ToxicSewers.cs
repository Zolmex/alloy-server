#region

using Common;
using Common.ProjectilePaths;
using GameServer.Game.Entities.Behaviors.Actions;
using GameServer.Game.Entities.Behaviors.Classic;
using GameServer.Game.Entities.Behaviors.Transitions;
using GameServer.Game.Entities.Loot;

#endregion

namespace GameServer.Game.Entities.Behaviors.Library
{
    public partial class BehaviorLib
    {
        [CharacterBehavior("DS Gulpord the Slime God")]
        public static State DSGulpordTheSlimeGod =>
            new State(
                new CharacterLoot(
                    // new TierLoot(11, ItemType.Weapon, 0.15),
                    // new TierLoot(12, ItemType.Weapon, 0.07),
                    // new TierLoot(11, ItemType.Armor, 0.15),
                    // new TierLoot(12, ItemType.Armor, 0.07),
                    // new TierLoot(4, ItemType.Ability, 0.1),
                    // new TierLoot(5, ItemType.Ability, 0.07),
                    // new TierLoot(5, ItemType.Ring, 0.07),
                    new ItemLoot("Potion of Defense", 1, 0.01f),
                    new ItemLoot("Potion of Defense", 1, 0.01f),
                    new ItemLoot("Void Blade", 0.01f, threshold: 0.03f),
                    new ItemLoot("Murky Toxin", 0.01f),
                    new ItemLoot("Toxic Sewers Key", 0.1f, 0.03f)
                ),
                // new ScaleHP2(20),
                new State("Waiting Player",
                    new EntityWithinTransition("Start Shooting", radius: 10)
                ),
                new State("Start Shooting",
                    new Wander(0.3f * 5.55f + 0.74f, distanceFromSpawn: 1),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 1, fixedAngle: 0, cooldownMS: 500),
                    new TimedTransition(10000, "Shooting 2")
                ),
                new State("Shooting 2",
                    new Wander(0.3f * 5.55f + 0.74f, distanceFromSpawn: 1),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 1, fixedAngle: 0, cooldownMS: 500),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 90, shootAngle: 10, cooldownMS: 3400),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 110, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 200),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 130, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 400),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 150, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 600),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 170, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 800),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 190, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 1000),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 210, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 1200),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 230, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 1400),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 250, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 1600),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 270, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 1800),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 290, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 2000),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 310, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 2200),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 330, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 2400),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 350, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 2600),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 370, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 2800),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 390, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 3000),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 410, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 3200),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 430, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 3400),
                    new Shoot(15, 2, projectileIndex: 0, fixedAngle: 15, shootAngle: 10, cooldownMS: 7000),
                    new Shoot(15, 2, projectileIndex: 0, fixedAngle: 35, shootAngle: 10, cooldownMS: 7000, coolDownOffset: 400),
                    new Shoot(15, 2, projectileIndex: 0, fixedAngle: 55, shootAngle: 10, cooldownMS: 7000, coolDownOffset: 800),
                    new Shoot(15, 2, projectileIndex: 0, fixedAngle: 75, shootAngle: 10, cooldownMS: 7000, coolDownOffset: 1200),
                    new Shoot(15, 2, projectileIndex: 0, fixedAngle: 95, shootAngle: 10, cooldownMS: 7000, coolDownOffset: 1600),
                    new Shoot(15, 2, projectileIndex: 0, fixedAngle: 115, shootAngle: 10, cooldownMS: 7000, coolDownOffset: 2200),
                    new Shoot(15, 2, projectileIndex: 0, fixedAngle: 135, shootAngle: 10, cooldownMS: 7000, coolDownOffset: 2600),
                    new Shoot(15, 2, projectileIndex: 0, fixedAngle: 155, shootAngle: 10, cooldownMS: 7000, coolDownOffset: 3000),
                    new Shoot(15, 2, projectileIndex: 0, fixedAngle: 175, shootAngle: 10, cooldownMS: 7000, coolDownOffset: 3400),
                    new Shoot(15, 2, projectileIndex: 0, fixedAngle: 195, shootAngle: 10, cooldownMS: 7000, coolDownOffset: 3800),
                    new Shoot(15, 2, projectileIndex: 0, fixedAngle: 215, shootAngle: 10, cooldownMS: 7000, coolDownOffset: 4200),
                    new Shoot(15, 2, projectileIndex: 0, fixedAngle: 235, shootAngle: 10, cooldownMS: 7000, coolDownOffset: 4600),
                    new Shoot(15, 2, projectileIndex: 0, fixedAngle: 255, shootAngle: 10, cooldownMS: 7000, coolDownOffset: 5000),
                    new Shoot(15, 2, projectileIndex: 0, fixedAngle: 275, shootAngle: 10, cooldownMS: 7000, coolDownOffset: 5400),
                    new Shoot(15, 2, projectileIndex: 0, fixedAngle: 295, shootAngle: 10, cooldownMS: 7000, coolDownOffset: 5800),
                    new Shoot(15, 2, projectileIndex: 0, fixedAngle: 315, shootAngle: 10, cooldownMS: 7000, coolDownOffset: 6200),
                    new Shoot(15, 2, projectileIndex: 0, fixedAngle: 335, shootAngle: 10, cooldownMS: 7000, coolDownOffset: 6600),
                    new Shoot(15, 2, projectileIndex: 0, fixedAngle: 355, shootAngle: 10, cooldownMS: 7000, coolDownOffset: 7000),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 90, shootAngle: 10, cooldownMS: 3400),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 110, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 200),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 130, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 400),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 150, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 600),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 170, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 800),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 190, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 1000),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 210, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 1200),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 230, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 1400),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 250, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 1600),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 270, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 1800),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 290, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 2000),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 310, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 2200),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 330, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 2400),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 350, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 2600),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 370, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 2800),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 390, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 3000),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 410, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 3200),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 430, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 3400),
                    new Shoot(15, 2, projectileIndex: 0, fixedAngle: 295, shootAngle: 10, cooldownMS: 7000),
                    new Shoot(15, 2, projectileIndex: 0, fixedAngle: 315, shootAngle: 10, cooldownMS: 7000, coolDownOffset: 400),
                    new Shoot(15, 2, projectileIndex: 0, fixedAngle: 335, shootAngle: 10, cooldownMS: 7000, coolDownOffset: 800),
                    new Shoot(15, 2, projectileIndex: 0, fixedAngle: 355, shootAngle: 10, cooldownMS: 7000, coolDownOffset: 1200),
                    new Shoot(15, 2, projectileIndex: 0, fixedAngle: 375, shootAngle: 10, cooldownMS: 7000, coolDownOffset: 1600),
                    new Shoot(15, 2, projectileIndex: 0, fixedAngle: 395, shootAngle: 10, cooldownMS: 7000, coolDownOffset: 2200),
                    new Shoot(15, 2, projectileIndex: 0, fixedAngle: 415, shootAngle: 10, cooldownMS: 7000, coolDownOffset: 2600),
                    new Shoot(15, 2, projectileIndex: 0, fixedAngle: 435, shootAngle: 10, cooldownMS: 7000, coolDownOffset: 3000),
                    new Shoot(15, 2, projectileIndex: 0, fixedAngle: 455, shootAngle: 10, cooldownMS: 7000, coolDownOffset: 3400),
                    new Shoot(15, 2, projectileIndex: 0, fixedAngle: 475, shootAngle: 10, cooldownMS: 7000, coolDownOffset: 3800),
                    new Shoot(15, 2, projectileIndex: 0, fixedAngle: 495, shootAngle: 10, cooldownMS: 7000, coolDownOffset: 4200),
                    new Shoot(15, 2, projectileIndex: 0, fixedAngle: 515, shootAngle: 10, cooldownMS: 7000, coolDownOffset: 4600),
                    new Shoot(15, 2, projectileIndex: 0, fixedAngle: 535, shootAngle: 10, cooldownMS: 7000, coolDownOffset: 5000),
                    new Shoot(15, 2, projectileIndex: 0, fixedAngle: 555, shootAngle: 10, cooldownMS: 7000, coolDownOffset: 5400),
                    new Shoot(15, 2, projectileIndex: 0, fixedAngle: 575, shootAngle: 10, cooldownMS: 7000, coolDownOffset: 5800),
                    new Shoot(15, 2, projectileIndex: 0, fixedAngle: 595, shootAngle: 10, cooldownMS: 7000, coolDownOffset: 6200),
                    new Shoot(15, 2, projectileIndex: 0, fixedAngle: 615, shootAngle: 10, cooldownMS: 7000, coolDownOffset: 6600),
                    new Shoot(15, 2, projectileIndex: 0, fixedAngle: 635, shootAngle: 10, cooldownMS: 7000, coolDownOffset: 7000),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 90, shootAngle: 10, cooldownMS: 3400),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 110, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 200),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 130, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 400),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 150, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 600),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 170, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 800),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 190, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 1000),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 210, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 1200),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 230, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 1400),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 250, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 1600),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 270, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 1800),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 290, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 2000),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 310, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 2200),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 330, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 2400),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 350, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 2600),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 370, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 2800),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 390, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 3000),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 410, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 3200),
                    new Shoot(15, 3, projectileIndex: 0, fixedAngle: 430, shootAngle: 10, cooldownMS: 3400, coolDownOffset: 3400),
                    new HpLessTransition(0.70f, "Shooting 3 Prepare"),
                    new TimedTransition(15000, "Shooting 3 Prepare")
                ),
                new State("Shooting 3 Prepare",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable, durationMS: 500),
                    new ReturnToSpawn(1 * 5.55f + 0.74f),
                    new TimedTransition(500, "Shooting 3")
                ),
                new State("Shooting 3",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable, durationMS: 500),
                    new Shoot(15, 1, projectileIndex: 3, cooldownMS: 500, targeted: true),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 1, fixedAngle: 0, cooldownMS: 500),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 0, cooldownMS: 9200),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 10, cooldownMS: 9200, coolDownOffset: 200),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 20, cooldownMS: 9200, coolDownOffset: 400),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 30, cooldownMS: 9200, coolDownOffset: 600),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 15, cooldownMS: 9200, coolDownOffset: 800),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 40, cooldownMS: 9200, coolDownOffset: 1000),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 50, cooldownMS: 9200, coolDownOffset: 1200),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 60, cooldownMS: 9200, coolDownOffset: 1400),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 70, cooldownMS: 9200, coolDownOffset: 1600),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 45, cooldownMS: 9200, coolDownOffset: 1800),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 80, cooldownMS: 9200, coolDownOffset: 2000),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 90, cooldownMS: 9200, coolDownOffset: 2200),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 100, cooldownMS: 9200, coolDownOffset: 2400),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 110, cooldownMS: 9200, coolDownOffset: 2600),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 85, cooldownMS: 9200, coolDownOffset: 2800),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 120, cooldownMS: 9200, coolDownOffset: 3000),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 130, cooldownMS: 9200, coolDownOffset: 3200),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 140, cooldownMS: 9200, coolDownOffset: 3400),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 150, cooldownMS: 9200, coolDownOffset: 3600),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 125, cooldownMS: 9200, coolDownOffset: 3800),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 160, cooldownMS: 9200, coolDownOffset: 4000),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 170, cooldownMS: 9200, coolDownOffset: 4200),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 180, cooldownMS: 9200, coolDownOffset: 4400),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 190, cooldownMS: 9200, coolDownOffset: 4600),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 165, cooldownMS: 9200, coolDownOffset: 4800),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 200, cooldownMS: 9200, coolDownOffset: 5000),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 210, cooldownMS: 9200, coolDownOffset: 5200),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 220, cooldownMS: 9200, coolDownOffset: 5400),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 230, cooldownMS: 9200, coolDownOffset: 5600),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 205, cooldownMS: 9200, coolDownOffset: 5800),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 240, cooldownMS: 9200, coolDownOffset: 6000),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 250, cooldownMS: 9200, coolDownOffset: 6200),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 260, cooldownMS: 9200, coolDownOffset: 6400),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 270, cooldownMS: 9200, coolDownOffset: 6600),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 245, cooldownMS: 9200, coolDownOffset: 6800),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 280, cooldownMS: 9200, coolDownOffset: 7000),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 290, cooldownMS: 9200, coolDownOffset: 7200),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 300, cooldownMS: 9200, coolDownOffset: 7400),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 310, cooldownMS: 9200, coolDownOffset: 7600),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 285, cooldownMS: 9200, coolDownOffset: 7800),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 310, cooldownMS: 9200, coolDownOffset: 8000),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 320, cooldownMS: 9200, coolDownOffset: 8200),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 330, cooldownMS: 9200, coolDownOffset: 8400),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 340, cooldownMS: 9200, coolDownOffset: 8600),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 315, cooldownMS: 9200, coolDownOffset: 8800),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 350, cooldownMS: 9200, coolDownOffset: 9000),
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 0, fixedAngle: 360, cooldownMS: 9200, coolDownOffset: 9200),
                    new HpLessTransition(.5f, "Minions v1")
                ),
                new State("Minions v1",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable, 1100),
                    new ChangeSize(-35, 0),
                    new TimedTransition(1100, "Minions v2")
                ),
                new State("Minions v2",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Spawn("DS Gulpord the Slime God M", maxSpawnsPerReset: 2, cooldownMs: 0),
                    new EntitiesNotWithinTransition(30, "Chase v1", "DS Gulpord the Slime God M", "DS Gulpord the Slime God S")
                ),
                new State("Chase v1",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new ChangeSize(35, 120),
                    new TimedTransition(1100, "Chase v2")
                ),
                new State("Chase v2",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable),
                    new Flash(0xFF0000, 0.5, 6),
                    new TimedTransition(1500, "Chase v3")
                ),
                new State("Chase v3",
                    new ConditionEffectBehavior(ConditionEffectIndex.StunImmune),
                    new ConditionEffectBehavior(ConditionEffectIndex.ParalyzedImmune),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable, 2000),
                    new Follow(1, 15, 0),
                    new Shoot(15, 2, shootAngle: 25, projectileIndex: 0, cooldownMS: 100, targeted: true),
                    new Shoot(15, 8, shootAngle: 360 / 8f, fixedAngle: 0, projectileIndex: 2, cooldownMS: 500, coolDownOffset: 500),
                    new Shoot(15, 1, projectileIndex: 1, cooldownMS: 400, targeted: true),
                    new HpLessTransition(0.05f, "dead1")
                ),
                new State("dead1",
                    new Suicide()
                )
            );

        [CharacterBehavior("DS Gulpord the Slime God M")]
        public static State DSGulpordTheSlimeGodM =>
            new(
                new State("Shooting",
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 1, fixedAngle: 0, cooldownMS: 1000, coolDownOffset: 1000),
                    new Shoot(15, 5, shootAngle: 25, projectileIndex: 0, cooldownMS: 1000, targeted: true),
                    new Orbit(1, 4, 20, "DS Gulpord the Slime God", orbitClockwise: true),
                    new TransformOnDeath("DS Gulpord the Slime God S", 2, 2, 1)
                )
            );

        [CharacterBehavior("DS Gulpord the Slime God S")]
        public static State DSGulpordTheSlimeGodS =>
            new(
                new State("Shooting",
                    new Shoot(15, 8, shootAngle: 360 / 8f, projectileIndex: 1, fixedAngle: 0, cooldownMS: 1000, coolDownOffset: 1000),
                    new Shoot(15, 5, shootAngle: 25, projectileIndex: 0, cooldownMS: 1000, targeted: true),
                    new Orbit(1, 4, 20, "DS Gulpord the Slime God", orbitClockwise: true),
                    new HpLessTransition(.15f, "Back")
                ),
                new State("Back",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible),
                    new ReturnToSpawn(0.5f * 5.55f + 0.74f),
                    new TimedTransition(1000, "Suicide")
                ),
                new State("Suicide",
                    new Suicide()
                )
            );

        [CharacterBehavior("DS Alligator")]
        public static State DSAlligator =>
            new(
                new Wander(.6f * 5.55f +0.74f),
                new Shoot(5, 3, shootAngle: 15, projectileIndex: 0, cooldownMS: 1500, targeted: true)
            );

        [CharacterBehavior("DS Bat")]
        public static State DSBat =>
            new(
                new State("Without player",
                    new Wander(.7f * 5.55f + 0.74f),
                    new Shoot(3, cooldownMS: 100, targeted: true),
                    new EntityWithinTransition("Player", radius: 5)
                ),
                new State("Player",
                    new Charge(3 * 5.55f + 0.74f, 8, 2000),
                    new Follow(0.7f * 5.55f + 0.74f, 5, 0),
                    new Shoot(3, cooldownMS: 100, targeted: true),
                    new EntityNotWithinTransition("Without player", radius: 5)
                )
            );

        [CharacterBehavior("DS Brown Slime")]
        public static State DSBrownSlime =>
            new(
                new State("No Player",
                    new Follow(0.6f * 5.55f + 0.74f, 0, 5),
                    new Wander(0.6f * 5.55f + 0.74f),
                    new Shoot(10, 8, shootAngle: 360 / 8f, projectileIndex: 0, cooldownMS: 1500, targeted: true),
                    new Reproduce("DS Brown Slime Trail", 100, 10, 50),
                    new EntityWithinTransition("Player", radius: 5)
                ),
                new State("Player",
                    new Follow(0.6f * 5.55f + 0.74f, 5, 0),
                    new Shoot(10, 8, shootAngle: 360 / 8f, projectileIndex: 0, cooldownMS: 1500, targeted: true),
                    new Reproduce("DS Brown Slime Trail", 100, 10, 50),
                    new EntityNotWithinTransition("No Player", radius: 5)
                )
            );

        [CharacterBehavior("DS Brown Slime Trail")]
        public static State DSBrownSlimeTrail =>
            new(
                new State("Start",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                    new Shoot(1, projectileIndex: 0, cooldownMS: 50, targeted: true),
                    new TimedTransition(500, "Dissapear")
                ),
                new State("Dissapear",
                    new ChangeSize(-10, 0),
                    new TimedTransition(500, "Suicide")
                ),
                new State("Suicide",
                    new Suicide()
                )
            );

        [CharacterBehavior("DS Goblin Brute")]
        public static State DSGoblinBrute =>
            new(
                new State("0",
                    new Follow(0.8f * 5.55f + 0.74f, 0, 9),
                    new Shoot(10, 4, shootAngle: 15, projectileIndex: 0, cooldownMS: 1000, targeted: true)
                )
            );

        [CharacterBehavior("DS Goblin Knight")]
        public static State DSGoblinKnight =>
            new(
                new CharacterLoot(
                    // new TierLoot(7, ItemType.Weapon, 0.09),
                    // new TierLoot(7, ItemType.Armor, 0.09)
                ),
                new State("Waiting player",
                    new EntityWithinTransition("Player founded", radius: 5)
                ),
                new State("Player founded",
                    new Wander(0.6f * 5.55f + 0.74f),
                    new Follow(0.6f * 5.55f + 0.74f, 10, 1),
                    new Shoot(10, 1, projectileIndex: 0, cooldownMS: 1000, predictive: 0.5f, targeted: true)
                )
            );

        [CharacterBehavior("DS Goblin Peon")]
        public static State DSGoblinPeon =>
            new(
                new State("0",
                    new Follow(0.7f * 5.55f + 0.74f, 1, 10),
                    new Shoot(9, 2, shootAngle: 20, projectileIndex: 0, cooldownMS: 500, targeted: true)
                )
            );

        [CharacterBehavior("DS Goblin Warlock")]
        public static State DSGoblinWarlock =>
            new(
                new CharacterLoot(
                    // new TierLoot(4, ItemType.Ability, 0.03)
                ),
                new State("0",
                    new Wander(0.6f * 5.55f + 0.74f),
                    new StayAwayFrom(0.6f * 5.55f + 0.74f, 6),
                    new Shoot(10, 2, projectileIndex: 0, shootAngle: 0, cooldownMS: 1000, targeted: true),
                    new Shoot(10, 1, projectileIndex: 1, cooldownMS: 1000, targeted: true)
                )
            );

        [CharacterBehavior("DS Fly")]
        public static State DSFly =>
            new(
                new State("0",
                    new Wander(0.3f * 5.55f + 0.74f),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible)
                )
            );

        [CharacterBehavior("DS Goblin Sorcerer")]
        public static State DSGoblinSorcerer =>
            new(
                new State("0",
                    new Wander(0.6f * 5.55f + 0.74f),
                    new Shoot(10, 5, shootAngle: 20, projectileIndex: 0, cooldownMS: 1000, targeted: true),
                    new AOE(3, 30, range: 4, cooldownMs: 1000, effects: [(ConditionEffectIndex.Confused, 3000)])
                )
            );

        [CharacterBehavior("DS Golden Rat")]
        public static State DSGoldenRat =>
            new(
                new CharacterLoot(
                    new ItemLoot("Potion of Defense", 1, 0.3f),
                    new ItemLoot("Murky Toxin", 0.01f, 0.3f)
                ),
                new State("0",
                    new ConditionEffectBehavior(ConditionEffectIndex.StasisImmune),
                    new ConditionEffectBehavior(ConditionEffectIndex.StunImmune),
                    new ConditionEffectBehavior(ConditionEffectIndex.ParalyzedImmune),
                    new State("No player",
                        new Wander(0.6f * 5.55f + 0.74f),
                        new EntityWithinTransition("Player", radius: 10)
                    ),
                    new State("Player",
                        new Taunt("Squeerk!"),
                        new StayAwayFrom(.9f * 5.55f + 0.74f, 99),
                        new TimedTransition(15000, "Suicide")
                    ),
                    new State("Suicide",
                        new Suicide()
                    )
                )
            );

        [CharacterBehavior("DS Natural Slime God")]
        public static State DSNaturalSlimeGod =>
            new(
                new CharacterLoot(
                    new ItemLoot("Potion of Defense", 0.03f, 0.18f)
                    // new TierLoot(6, ItemType.Weapon, 0.04),
                    // new TierLoot(7, ItemType.Weapon, 0.1f),
                    // new TierLoot(8, ItemType.Weapon, 0.08f),
                    // new TierLoot(7, ItemType.Armor, 0.1f),
                    // new TierLoot(8, ItemType.Armor, 0.08f),
                    // new TierLoot(9, ItemType.Armor, 0.05f),
                    // new TierLoot(4, ItemType.Ability, 0.05f)
                ),
                new State("0",
                    new Follow(1 * 5.55f + 0.74f, distFromTarget: 7),
                    new Wander(0.4f * 5.55f + 0.74f),
                    new Shoot(12, projectileIndex: 0, count: 5, shootAngle: 10, predictive: 1, cooldownMS: 1000, targeted: true),
                    new Shoot(10, projectileIndex: 1, predictive: 0, cooldownMS: 650, targeted: true)
                )
            );

        [CharacterBehavior("DS Rat")]
        public static State DSRat =>
            new(
                new CharacterLoot(
                    // new TierLoot(7, ItemType.Weapon, 0.05)
                ),
                new State("0",
                    new Follow(0.6f * 5.55f + 0.74f, 1, 10),
                    new Shoot(10, 3, shootAngle: 20, projectileIndex: 0, cooldownMS: 1000, targeted: true)
                )
            );

        [CharacterBehavior("DS Yellow Slime")]
        public static State DSYellowSlime =>
            new(
                new State("No Player",
                    new Follow(0.6f * 5.55f + 0.74f, 0, 5),
                    new Wander(0.6f * 5.55f + 0.74f),
                    new Shoot(10, 8, projectileIndex: 0, cooldownMS: 1500, targeted: true),
                    new Reproduce("DS Brown Slime Trail", 100, 10, 50),
                    new EntityWithinTransition("Player", radius: 5)
                ),
                new State("Player",
                    new Follow(0.6f * 5.55f + 0.74f, 0, 5),
                    new Shoot(10, 8, shootAngle: 360 / 8f, projectileIndex: 0, cooldownMS: 1500, targeted: true),
                    new Reproduce("DS Brown Slime Trail", 100, 10, 50),
                    new EntityNotWithinTransition("No Player", radius: 5)
                )
            );

        [CharacterBehavior("DS Yellow Slime Trail")]
        public static State DSYellowSlimeTrail =>
            new(
                new State("Start",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                    new Shoot(1, projectileIndex: 0, cooldownMS: 50, targeted: true),
                    new TimedTransition(500, "Dissapear")
                ),
                new State("Dissapear",
                    new ChangeSize(-10, 0),
                    new TimedTransition(500, "Suicide")
                ),
                new State("Suicide",
                    new Suicide()
                )
            );

        [CharacterBehavior("DS Master Rat")]
        public static State DSMasterRat =>
            new(
                new State("Waiting player",
                    new ConditionEffectBehavior(ConditionEffectIndex.Invincible, persist: true),
                    new ConditionEffectBehavior(ConditionEffectIndex.Invulnerable, persist: true),
                    new EntityWithinTransition("Start", radius: 7)
                ),
                new State("Start",
                    new Taunt("Hello young adventurers, will you be able to answer my question correctly?"),
                    new TimedTransition(2000, TransitionType.Random, "First", "Second", "Third", "Four", "Five")
                ),
                new State("First",
                    new Taunt("What time is it?"),
                    new PlayerTextTransition("Correct!", "Its pizza time!", true),
                    new TimedTransition(10000, "Incorrect")
                ),
                new State("Second",
                    new Taunt("Where is the safest place in the world?"),
                    new PlayerTextTransition("Correct!", "Inside my shell.", true),
                    new TimedTransition(10000, "Incorrect")
                ),
                new State("Third",
                    new Taunt("What is fast, quiet and hidden by the night?"),
                    new PlayerTextTransition("Correct!", "A ninja of course!", true),
                    new TimedTransition(10000, "Incorrect")
                ),
                new State("Four",
                    new Taunt("How do you like your pizza?"),
                    new PlayerTextTransition("Correct!", "Extra cheese, hold the anchovies.", true),
                    new TimedTransition(10000, "Incorrect")
                ),
                new State("Five",
                    new Taunt("Who did this to me?"),
                    new PlayerTextTransition("Correct!", "Dr. Terrible, the mad scientist.", true),
                    new TimedTransition(10000, "Incorrect")
                ),
                new State("Correct!",
                    new Taunt("Cowabunga!"),
                    new TossObject("DS Blue Turtle", 3, cooldownMS: 1000, maxAngle: 360),
                    new TossObject("DS Orange Turtle", 3, cooldownMS: 1000, maxAngle: 360),
                    new TossObject("DS Purple Turtle", 3, cooldownMS: 1000, maxAngle: 360),
                    new TossObject("DS Red Turtle", 3, cooldownMS: 1000, maxAngle: 360),
                    new TimedTransition(500, "Spawn Correct")
                ),
                new State("Spawn Correct",
                    new Suicide()
                ),
                new State("Incorrect",
                    new Taunt("It's time you turtles learned your place!"),
                    new TimedTransition(300, "Incorrect Kill")
                ),
                new State("Incorrect Kill",
                    new Shoot(20, 16, shootAngle: 360 / 16f, projectileIndex: 0, fixedAngle: 0, cooldownMS: 100000),
                    new Suicide()
                )
            );

        [CharacterBehavior("DS Blue Turtle")]
        public static State DSBlueTurtle =>
            new(
                new CharacterLoot(
                    new ItemLoot("Potion of Defense", 0.09f, 0.03f),
                    new ItemLoot("Void Blade", 0.01f, threshold: 0.03f),
                    new ItemLoot("Murky Toxin", 0.01f, 0.03f)
                ),
                new State("0",
                    new Wander(0.3f * 5.55f + 0.74f, distanceFromSpawn: 5)
                )
            );

        [CharacterBehavior("DS Orange Turtle")]
        public static State DSOrangeTurtle =>
            new(
                new CharacterLoot(
                    new ItemLoot("Potion of Defense", 0.09f, 0.03f),
                    new ItemLoot("Void Blade", 0.01f, threshold: 0.03f),
                    new ItemLoot("Murky Toxin", 0.01f, 0.03f)
                ),
                new State("0",
                    new Wander(0.3f * 5.55f + 0.74f, 5)
                )
            );

        [CharacterBehavior("DS Purple Turtle")]
        public static State DSPurpleTurtle =>
            new(
                new CharacterLoot(
                    new ItemLoot("Potion of Defense", 0.09f, 0.03f),
                    new ItemLoot("Void Blade", 0.01f, threshold: 0.03f),
                    new ItemLoot("Murky Toxin", 0.01f, 0.03f)
                ),
                new State("0",
                    new Wander(0.3f * 5.55f + 0.74f, 5)
                )
            );

        [CharacterBehavior("DS Red Turtle")]
        public static State DSRedTurtle =>
            new(
                new CharacterLoot(
                    new ItemLoot("Potion of Defense", 0.09f, 0.03f),
                    new ItemLoot("Void Blade", 0.01f, threshold: 0.03f),
                    new ItemLoot("Murky Toxin", 0.01f, 0.03f)
                ),
                new State("0",
                    new Wander(0.3f * 5.55f + 0.74f, 5)
                )
            );
    }
}