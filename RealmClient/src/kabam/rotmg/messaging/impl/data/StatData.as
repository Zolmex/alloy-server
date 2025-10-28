package kabam.rotmg.messaging.impl.data {
import flash.utils.IDataInput;
import flash.utils.IDataOutput;

public class StatData {
    
    public static const MAX_HP_STAT:int = 0;
    public static const HP_STAT:int = 1;
    public static const SIZE_STAT:int = 2;
    public static const MAX_MP_STAT:int = 3;
    public static const MP_STAT:int = 4;
    public static const NEXT_LEVEL_EXP_STAT:int = 5;
    public static const EXP_STAT:int = 6;
    public static const LEVEL_STAT:int = 7;
    public static const INVENTORY_0_STAT:int = 8;
    public static const INVENTORY_1_STAT:int = 9;
    public static const INVENTORY_2_STAT:int = 10;
    public static const INVENTORY_3_STAT:int = 11;
    public static const INVENTORY_4_STAT:int = 12;
    public static const INVENTORY_5_STAT:int = 13;
    public static const INVENTORY_6_STAT:int = 14;
    public static const INVENTORY_7_STAT:int = 15;
    public static const INVENTORY_8_STAT:int = 16;
    public static const INVENTORY_9_STAT:int = 17;
    public static const INVENTORY_10_STAT:int = 18;
    public static const INVENTORY_11_STAT:int = 19;
    public static const ATTACK_STAT:int = 20;
    public static const DEFENSE_STAT:int = 21;
    public static const MOVEMENT_SPEED_STAT:int = 22;
    public static const LIFE_REGENERATION_STAT:int = 23;
    public static const WISDOM_STAT:int = 24;
    public static const DEXTERITY_STAT:int = 25;
    public static const CONDITION1_STAT:int = 26;
    public static const NUM_STARS_STAT:int = 27;
    public static const NAME_STAT:int = 28;
    public static const TEX1_STAT:int = 29;
    public static const TEX2_STAT:int = 30;
    public static const MERCHANDISE_TYPE_STAT:int = 31;
    public static const MERCHANDISE_PRICE_STAT:int = 32;
    public static const CREDITS_STAT:int = 33;
    public static const ACTIVE_STAT:int = 34;
    public static const ACCOUNT_ID_STAT:int = 35;
    public static const FAME_STAT:int = 36;
    public static const MERCHANDISE_CURRENCY_STAT:int = 37;
    public static const CONNECT_STAT:int = 38;
    public static const MERCHANDISE_COUNT_STAT:int = 39;
    public static const MERCHANDISE_MINS_LEFT_STAT:int = 40;
    public static const MERCHANDISE_DISCOUNT_STAT:int = 41;
    public static const MERCHANDISE_RANK_REQ_STAT:int = 42;
    public static const CHAR_FAME_STAT:int = 43;
    public static const NEXT_CLASS_QUEST_FAME_STAT:int = 44;
    public static const LEGENDARY_RANK_STAT:int = 45;
    public static const SINK_LEVEL_STAT:int = 46;
    public static const ALT_TEXTURE_STAT:int = 47;
    public static const GUILD_NAME_STAT:int = 48;
    public static const GUILD_RANK_STAT:int = 49;
    public static const OXYGEN_STAT:int = 50;
    public static const HEALTH_POTION_STACK_STAT:int = 51;
    public static const MAGIC_POTION_STACK_STAT:int = 52;
    public static const BACKPACK_0_STAT:int = 53;
    public static const BACKPACK_1_STAT:int = 54;
    public static const BACKPACK_2_STAT:int = 55;
    public static const BACKPACK_3_STAT:int = 56;
    public static const BACKPACK_4_STAT:int = 57;
    public static const BACKPACK_5_STAT:int = 58;
    public static const BACKPACK_6_STAT:int = 59;
    public static const BACKPACK_7_STAT:int = 60;
    public static const HASBACKPACK_STAT:int = 61;
    public static const TEXTURE_STAT:int = 62;
    public static const INVENTORYDATA_0_STAT:int = 63;
    public static const INVENTORYDATA_1_STAT:int = 64;
    public static const INVENTORYDATA_2_STAT:int = 65;
    public static const INVENTORYDATA_3_STAT:int = 66;
    public static const INVENTORYDATA_4_STAT:int = 67;
    public static const INVENTORYDATA_5_STAT:int = 68;
    public static const INVENTORYDATA_6_STAT:int = 69;
    public static const INVENTORYDATA_7_STAT:int = 70;
    public static const INVENTORYDATA_8_STAT:int = 71;
    public static const INVENTORYDATA_9_STAT:int = 72;
    public static const INVENTORYDATA_10_STAT:int = 73;
    public static const INVENTORYDATA_11_STAT:int = 74;
    public static const INVENTORYDATA_12_STAT:int = 75;
    public static const INVENTORYDATA_13_STAT:int = 76;
    public static const INVENTORYDATA_14_STAT:int = 77;
    public static const INVENTORYDATA_15_STAT:int = 78;
    public static const INVENTORYDATA_16_STAT:int = 79;
    public static const INVENTORYDATA_17_STAT:int = 80;
    public static const INVENTORYDATA_18_STAT:int = 81;
    public static const INVENTORYDATA_19_STAT:int = 82;
    public static const PRIMARY_CONSTELLATION_STAT:int = 83;
    public static const SECONDARY_CONSTELLATION_STAT:int = 84;
    public static const PRIMARY_NODE_DATA_STAT:int = 85;
    public static const SECONDARY_NODE_DATA_STAT:int = 86;
    public static const DODGE_CHANCE_STAT:int = 87;
    public static const CRITICAL_CHANCE_STAT:int = 88;
    public static const CRITICAL_DAMAGE_STAT:int = 89;
    public static const MAX_MS_STAT:int = 90;
    public static const MS_STAT:int = 91;
    public static const MANA_REGENERATION_STAT:int = 92;
    public static const MS_REGEN_RATE_STAT:int = 93;
    public static const ARMOR_STAT:int = 94;
    public static const DAMAGE_MULTIPLIER_STAT:int = 95;
    public static const UNUSED:int = 96;
    public static const STAT_POINTS_STAT:int = 97;
    public static const TIME_IN_COMBAT_STAT:int = 98;
    public static const ATTACK_SPEED_STAT:int = 116;
    public static const ATTACK_SPEED_BONUS_STAT:int = 117;
    public static const CONDITION2_STAT:int = 118;
    public static const ACC_RANK_STAT:int = 119;
    public static const QUEST_ID:int = 120;
    public static const PARTY_ID:int = 121;

    public static const ABILITY_DATA_A:int = 122;
    public static const ABILITY_DATA_B:int = 123;
    public static const ABILITY_DATA_C:int = 124;
    public static const ABILITY_DATA_D:int = 125;

    public var statType_:uint = 0;
    public var statValue_:int = 0;
    public var floatValue_:Number = 0;
    public var strStatValue_:String;

    public function StatData() {
        super();
    }

    public function isFloatStat():Boolean {
        switch (this.statType_) {
            case CRITICAL_CHANCE_STAT:
            case DODGE_CHANCE_STAT:
            case ATTACK_SPEED_STAT:
            case MOVEMENT_SPEED_STAT:
                return true;
            default:
                return false;
        }
    }

    public function isStringStat():Boolean {
        switch (this.statType_) {
            case NAME_STAT:
            case GUILD_NAME_STAT:
            case INVENTORYDATA_0_STAT:
            case INVENTORYDATA_1_STAT:
            case INVENTORYDATA_2_STAT:
            case INVENTORYDATA_3_STAT:
            case INVENTORYDATA_4_STAT:
            case INVENTORYDATA_5_STAT:
            case INVENTORYDATA_6_STAT:
            case INVENTORYDATA_7_STAT:
            case INVENTORYDATA_8_STAT:
            case INVENTORYDATA_9_STAT:
            case INVENTORYDATA_10_STAT:
            case INVENTORYDATA_11_STAT:
            case INVENTORYDATA_12_STAT:
            case INVENTORYDATA_13_STAT:
            case INVENTORYDATA_14_STAT:
            case INVENTORYDATA_15_STAT:
            case INVENTORYDATA_16_STAT:
            case INVENTORYDATA_17_STAT:
            case INVENTORYDATA_18_STAT:
            case INVENTORYDATA_19_STAT:
            case ABILITY_DATA_A:
            case ABILITY_DATA_B:
            case ABILITY_DATA_C:
            case ABILITY_DATA_D:
                return true;
            default:
                return false;
        }
    }

    public function parseFromInput(data:IDataInput):void {
        this.statType_ = data.readUnsignedByte();
        if (this.statType_ == 99)
            trace("lalala見つけた！");
        if (this.isStringStat())
            this.strStatValue_ = data.readUTF();
        else if (this.isFloatStat())
            this.floatValue_ = data.readFloat();
        else
            this.statValue_ = data.readInt();
    }

    public function writeToOutput(data:IDataOutput):void {
        data.writeByte(this.statType_);
        if (this.isStringStat())
            data.writeUTF(this.strStatValue_);
        else if (this.isFloatStat())
            data.writeFloat(this.floatValue_);
        else
            data.writeInt(this.statValue_);
    }

    public function toString():String {
        if (this.isStringStat())
            return "[" + this.statType_ + ": \"" + this.strStatValue_ + "\"]";
        else if (this.isFloatStat())
            return "[" + this.statType_ + ": " + this.floatValue_ + "]";
        else
            return "[" + this.statType_ + ": " + this.statValue_ + "]";
    }
}
}
