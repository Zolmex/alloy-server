package kabam.rotmg.game.view.components.statsview
{
import com.company.assembleegameclient.objects.Player;

public class StatModels
   {
      public static const ATTACK_NAME:String = "Attack";
      public static const DEFENSE_NAME:String = "Defense";
      public static const DEXTERITY_NAME:String = "Dexterity";
      public static const WISDOM_NAME:String = "Wisdom";

      public static const DAMAGE_MULTIPLIER_NAME:String = "Damage Multiplier";
      public static const CRITICAL_CHANCE_NAME:String = "Critical Chance";
      public static const CRITICAL_DAMAGE_NAME:String = "Critical Damage";
      public static const ATTACK_SPEED_NAME:String = "Attack Speed";

      public static const MAXIMUM_LIFE_NAME:String = "Maximum Life";
      public static const LIFE_REGENERATION_NAME:String = "Life Regeneration";
      public static const LIFE_LEECH_NAME:String = "Life Leech";
      public static const MAXIMUM_MAGIC_SHIELD_NAME:String = "Maximum Magic Shield";
      public static const MAGIC_SHIELD_REGEN_RATE_NAME:String = "Magic Shield Regen Rate";
      public static const TIME_TIL_SHIELD_RECHARGE_NAME:String = "Time Til Shield Recharge";
      public static const ARMOR_NAME:String = "Armor";
      public static const DODGE_CHANCE_NAME:String = "Dodge Chance";

      public static const MAXIMUM_MANA_NAME:String = "Maximum Mana";
      public static const MANA_REGENERATION_NAME:String = "Mana Regeneration";
      public static const MOVEMENT_SPEED_NAME:String = "Movement Speed";

      //base
      public static const baseStatModels:Array = [
         new StatModel(ATTACK_NAME,"Increases your critical damage and damage multiplier.")
         ,new StatModel(DEFENSE_NAME,"Increases your armor and maximum life.")
         ,new StatModel(DEXTERITY_NAME,"Increases your dodge chance and critical chance.")
         ,new StatModel(WISDOM_NAME,"Increases your maximum mana, maximum magic shield, and magic damage multiplier.")
      ];

      //offense
      public static const damageStatModel:Array = [
         new StatModel(DAMAGE_MULTIPLIER_NAME,"Your damage is increased by this multiplier.", "", "%")
         //,new StatModel(MAGIC_DAMAGE_MULTIPLIER_NAME,"Your magic damage is increased by this multiplier.", "", "%")
      ];

      public static const criticalStatModel:Array = [
         new StatModel(CRITICAL_CHANCE_NAME,"The chance for an outgoing attack to become a critical hit.", "", "%")
         ,new StatModel(CRITICAL_DAMAGE_NAME,"The increase in damage that critical hits grant you.", "", "%")
      ];

      public static const attacksStatModel:Array = [
         new StatModel(ATTACK_SPEED_NAME,"Your weapon's attack speed is increased by this multiplier.", "", "")
      ];

      public static const offenseStatModels:Array = [damageStatModel, criticalStatModel, attacksStatModel];
      public static const offenseStatHeaders:Array = ["Damage", "Critical", "Attacks"];

      //defense
      public static const lifeStatModel:Array = [
         new StatModel(MAXIMUM_LIFE_NAME,"Your maximum life. You will die when your life reaches zero.")
         ,new StatModel(LIFE_REGENERATION_NAME,"The amount of life you regenerate each second.", "", "/s")
         ,new StatModel(LIFE_LEECH_NAME,"The amount of life you gain each time you hit an enemy with a projectile.")
      ];

      public static const magicShieldStatModel:Array = [
         new StatModel(MAXIMUM_MAGIC_SHIELD_NAME,"Your maximum magic shield. This shield takes damage before your life.")
         ,new StatModel(MAGIC_SHIELD_REGEN_RATE_NAME,"The amount of magic shield regenerated per second when it is regenerating.", "", "/s")
         ,new StatModel(TIME_TIL_SHIELD_RECHARGE_NAME,"The time until your magic shield begins to recharge.", "", "s")
      ];

      public static const defensesStatModel:Array = [
         new StatModel(ARMOR_NAME,"The flat amount of damage deducted from incoming attacks.")
         ,new StatModel(DODGE_CHANCE_NAME,"Your chance to dodge incoming attacks, taking zero damage.", "", "%")
      ];

      public static const defenseStatModels:Array = [lifeStatModel, magicShieldStatModel, defensesStatModel];
      public static const defenseStatHeaders:Array = ["Life", "Magic Shield", "Defenses"];

      //misc
      public static const manaStatModel:Array = [
         new StatModel(MAXIMUM_MANA_NAME,"Your maximum mana. This is the resource you use to cast abilities.")
         ,new StatModel(MANA_REGENERATION_NAME,"The amount of mana you regenerate each second.", "", "/s")
      ];

      public static const movementStatModel:Array = [
         new StatModel(MOVEMENT_SPEED_NAME,"The speed at which your character moves in tiles per second.", "", "")
      ];

      public static const miscStatModels:Array = [manaStatModel, movementStatModel];
      public static const miscStatHeaders:Array = ["Mana", "Movement"];

      public static function getStatValue(statView:StatView, player:Player):Number {
         switch (statView.statName) {

            case StatModels.DAMAGE_MULTIPLIER_NAME:
               return player.damageMultiplier;

            case StatModels.CRITICAL_CHANCE_NAME:
               return player.criticalChance;

            case StatModels.CRITICAL_DAMAGE_NAME:
               return player.criticalDamage;

            case StatModels.MAXIMUM_LIFE_NAME:
               return player.maxHP;

            case StatModels.LIFE_REGENERATION_NAME:
               return player.lifeRegeneration;

            case StatModels.LIFE_LEECH_NAME:
               //return player.lifeLeech;
               return 0;

            case StatModels.MAXIMUM_MAGIC_SHIELD_NAME:
               return player.maxMS;

            case StatModels.MAGIC_SHIELD_REGEN_RATE_NAME:
               return player.msRegenRate;

            case StatModels.TIME_TIL_SHIELD_RECHARGE_NAME:
               return 2;

            case StatModels.ARMOR_NAME:
               return player.armor;

            case StatModels.DODGE_CHANCE_NAME:
               return player.dodgeChance;

            case StatModels.MAXIMUM_MANA_NAME:
               return player.maxMP;

            case StatModels.MANA_REGENERATION_NAME:
               return player.manaRegeneration;

            case StatModels.ATTACK_SPEED_NAME:
               return player.attackSpeed;

            case StatModels.MOVEMENT_SPEED_NAME:
               return player.movementSpeed

            default:
               return 0;
         }
      }
   }
}
