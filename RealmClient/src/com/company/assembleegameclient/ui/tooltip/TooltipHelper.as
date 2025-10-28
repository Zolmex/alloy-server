package com.company.assembleegameclient.ui.tooltip {
import com.company.assembleegameclient.itemData.ItemData;
import com.company.util.MathUtil2;

import kabam.rotmg.constants.ItemConstants;

public class TooltipHelper {

    public static const BETTER_COLOR:String = "#00ff00";
    public static const WORSE_COLOR:String = "#ff0000";
    public static const NO_DIFF_COLOR:String = "#FFFF8F";
    public static const SPECIAL_COLOR:String = "#8A2BE2";
    public static const WISMOD_COLOR:String = "#4063E3";
    public static const UNTIERED_COLOR:uint = 9055202;

    public static function wrapInFontTag(text:String, color:String):String {
        var tagStr:String = "<font color=\"" + color + "\">" + text + "</font>";
        return tagStr;
    }

    public static function wrapInBold(text:String):String {
        var tagStr:String = "<b>" + text + "</b>";
        return tagStr;
    }

    public static function getFormattedRangeString(range:Number):Number {
        return MathUtil2.roundTo(range, 2);
    }

    public static function getTextColor(difference:Number):String {
        if (difference < 0) {
            return WORSE_COLOR;
        }
        if (difference > 0) {
            return BETTER_COLOR;
        }
        return NO_DIFF_COLOR;
    }

    public static function getSpecialityColor(itemData:ItemData):uint {
        if (itemData.Rarity != null){
            switch (itemData.Rarity){
                case "Common":
                    return 0xBFBFBF;
                case "Uncommon":
                    return 0x6DFF00; // 0x6DFF00
                case "Rare":
                    return 0x00AAFF;
                case "Legendary":
                    return 0xFFD83F;
                case "Mythic":
                    return 0xC855FF;
            }
        }
        if (itemData.Tier != -1)
            return 16777215;
        if (ItemConstants.isEquippable(itemData.SlotType))
            return UNTIERED_COLOR;
        return 16777215;
    }

    public static function getRarityText(itemData:ItemData):String {
        if (itemData.Rarity != null)
            return itemData.Rarity;
        if (itemData.Tier != -1)
            return "Tiered";
        if (itemData.Potion)
            return "Potion";
        if (ItemConstants.isEquippable(itemData.SlotType))
            return "Untiered";
        return "Miscellaneous";
    }

    public static function getRarityTag(itemData:ItemData):String {
        if (itemData.Rarity != null) {
            switch (itemData.Rarity) {
                case "Common":
                    return "CM";
                case "Uncommon":
                    return "UC";
                case "Rare":
                    return "R";
                case "Legendary":
                    return "LG";
                case "Mythic":
                    return "MC";
            }
        }
        if (itemData.Tier != -1)
            return "Tiered";
        if (itemData.Potion)
            return "Potion";
        if (ItemConstants.isEquippable(itemData.SlotType))
            return "Untiered";
        return "Miscellaneous";
    }
}
}
