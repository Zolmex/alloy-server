package com.company.assembleegameclient.itemData {
import com.company.assembleegameclient.objects.animation.Animations;
import com.company.assembleegameclient.objects.animation.AnimationsData;
import com.company.util.ConversionUtil;
import flash.utils.ByteArray;
import flash.utils.Endian;
import com.company.assembleegameclient.objects.ObjectLibrary;
import com.company.assembleegameclient.util.AnimatedChar;
import com.company.assembleegameclient.util.AnimatedChars;
import com.company.assembleegameclient.util.TextureRedrawer;
import flash.display.BitmapData;

import kabam.rotmg.messaging.impl.incoming.Text;

public class ItemData {

    public var ObjectType:int;
    public var ObjectId:String;
    public var DisplayId:String;
    public var Tex1:int;
    public var Tex2:int;
    public var SlotType:int;
    public var Description:String;
    public var Consumable:Boolean;
    public var InvUse:Boolean;
    public var Soulbound:Boolean;
    public var Potion:Boolean;
    public var Usable:Boolean;
    public var Resurrects:Boolean;
    public var RateOfFire:Number;
    public var Tier:int;
    public var BagType:int;
    public var FameBonus:int;
    public var NumProjectiles:int;
    public var ArcGap:Number;
    public var MpCost:int;
    public var MpEndCost:int;
    public var Cooldown:Number;
    public var Doses:int;
    public var Backpack:Boolean;
    public var MaxDoses:int;
    public var MultiPhase:Boolean;
    public var Treasure:Boolean;
    public var PermaPet:Boolean;
    public var Rarity:String;
    public var GemstoneLimit:int;
    public var Gemstone:GemstoneDesc;
    public var Gemstones:Vector.<int>;
    public var StatsBoosts:Vector.<StatBoost>;
    public var ActivateEffects:Vector.<ActivateEffect>;
    public var Projectile:ProjectileDesc;
    public var CustomToolTipDataList:Vector.<CustomToolTipData>;
    public var Texture:TextureDesc;
    public var Animation:Animations;
    public var ItemLevel:int;
    public var LevelIncreases:Vector.<LevelIncreaseDesc>;
    public var Sheath:SheathDesc;
    public var Spell:SpellDesc;
    public var Quiver:QuiverDesc;
    public var Helm:HelmDesc;
    public var Poison:PoisonDesc;
    public var UpgradeLevels:int;
    public var MaxCharge:int;
    public var MpCostPerSecond:int;

    public function ItemData(objType:int) {
        if (objType == -1)
            return;

        var xml:XML = ObjectLibrary.xmlLibrary_[objType];
        this.ObjectType = objType;
        this.ObjectId = GetValue(xml, "@id", null);
        this.DisplayId = GetValue(xml, "DisplayId", null);
        this.Tex1 = GetValue(xml, "Tex1", 0);
        this.Tex2 = GetValue(xml, "Tex2", 0);
        this.SlotType = GetValue(xml, "SlotType", 0);
        this.Description = GetValue(xml, "Description", null);
        this.Consumable = GetValue(xml, "Consumable", false);
        this.InvUse = GetValue(xml, "InvUse", false);
        this.Soulbound = GetValue(xml, "Soulbound", false);
        this.Potion = GetValue(xml, "Potion", false);
        this.Usable = GetValue(xml, "Usable", false);
        this.Resurrects = GetValue(xml, "Resurrects", false);
        this.RateOfFire = GetValue(xml, "RateOfFire", 1);
        this.Tier = GetValue(xml, "Tier", -1);
        this.BagType = GetValue(xml, "BagType", 0);
        this.FameBonus = GetValue(xml, "FameBonus", 0);
        this.NumProjectiles = GetValue(xml, "NumProjectiles", 1);
        if (this.NumProjectiles == -1 && HasValue(xml, "Projectile"))
            this.NumProjectiles = 1;
        this.ArcGap = GetValue(xml, "ArcGap", 11.25);
        this.MpCost = GetValue(xml, "MpCost", -1);
        this.MpEndCost = GetValue(xml, "MpEndCost", 0);
        this.Cooldown = GetValue(xml, "Cooldown", 0.5);
        this.Doses = GetValue(xml, "Doses", 0);
        this.Backpack = GetValue(xml, "Backpack", false);
        this.MaxDoses = GetValue(xml, "MaxDoses", 0);
        this.MultiPhase = GetValue(xml, "MultiPhase", false);
        this.Treasure = GetValue(xml, "Treasure", false);
        this.PermaPet = GetValue(xml, "PermaPet", false);
        this.Rarity = GetValue(xml, "Rarity", null);
        this.GemstoneLimit = GetValue(xml, "GemstoneLimit", 0);
        this.ItemLevel = GetValue(xml, "ItemLevel", -1);
        this.Texture = new TextureDesc(GetValue(xml, "Texture", null));
        if (!this.Texture.File) {
            this.Texture = new TextureDesc(GetValue(xml, "AnimatedTexture", null));
        }
        if (HasValue(xml, "Gemstone")) {
            this.Gemstone = new GemstoneDesc(GetValue(xml, "Gemstone", null));
        }
        if (HasValue(xml, "Projectile")) {
            this.Projectile = new ProjectileDesc(GetValue(xml, "Projectile", null));
        }
        if (HasValue(xml, "ActivateOnEquip")) {
            this.StatsBoosts = new Vector.<StatBoost>();
            for each (var stat:* in GetValue(xml, "ActivateOnEquip", null)) {
                this.StatsBoosts.push(new StatBoost(stat));
            }
        }
        if (HasValue(xml, "Activate")) {
            this.ActivateEffects = new Vector.<ActivateEffect>();
            for each (var eff:* in GetValue(xml, "Activate", null)) {
                this.ActivateEffects.push(new ActivateEffect(eff));
            }
        }
        if (HasValue(xml, "LevelIncrease")) {
            this.LevelIncreases = new Vector.<LevelIncreaseDesc>();
            for each (var inc:* in GetValue(xml, "LevelIncrease", null)) {
                this.LevelIncreases.push(new LevelIncreaseDesc(inc));
            }
        }
        if (HasValue(xml, "ExtraTooltipData")) {
            this.CustomToolTipDataList = new Vector.<CustomToolTipData>();
            var data:* = GetValue(xml, "ExtraTooltipData", null);
            if (data) {
                for each (var effInfo:* in data.EffectInfo) {
                    this.CustomToolTipDataList.push(new CustomToolTipData(effInfo));
                }
            }
        }

        var animationsData:AnimationsData = ObjectLibrary.typeToAnimationsData_[objType];
        if (animationsData != null) {
            this.Animation = new Animations(animationsData);
        }

        if (HasValue(xml, "Sheath")) {
            this.Sheath = new SheathDesc(GetValue(xml, "Sheath", null));
        }

        if (HasValue(xml, "Spell")) {
            this.Spell = new SpellDesc(GetValue(xml, "Spell", null));
        }

        if (HasValue(xml, "Quiver")) {
            this.Quiver = new QuiverDesc(GetValue(xml, "Quiver", null));
        }

        if (HasValue(xml, "Helm")) {
            this.Helm = new HelmDesc(GetValue(xml, "Helm", null));
        }

        if (HasValue(xml, "Poison")) {
            this.Poison = new PoisonDesc(GetValue(xml, "Poison", null));
        }
    }

    public function getRedrawnTexture(size:uint, glowColor:int, scaleSize:Boolean = false, time:int = 0) : BitmapData {
        var texture:BitmapData = null;
        if(this.Animation != null) {
            texture = this.Animation.getTexture(time);
            if(texture != null)
                return TextureRedrawer.redraw(texture, size, true, glowColor);
        }

        return Texture.getRedrawnTexture(size, glowColor, scaleSize);
    }

    public function GetDisplayName():String {
        return this.DisplayId || this.ObjectId;
    }

    public function ParseData(itemDatas:ByteArray):void {
        var fieldCount:int = itemDatas.readUnsignedByte();
        for (var i:int = 0; i < fieldCount; i++) {
            var field:int = itemDatas.readUnsignedByte();
            switch (field) {
                case ItemDataField.OBJECT_ID:
                    this.ObjectId = itemDatas.readUTF();
                    break;
                case ItemDataField.OBJECT_TYPE:
                    this.ObjectType = itemDatas.readUnsignedShort();
                    break;
                case ItemDataField.SLOT_TYPE:
                    this.SlotType = itemDatas.readInt();
                    break;
                case ItemDataField.USABLE:
                    this.Usable = itemDatas.readBoolean();
                    break;
                case ItemDataField.BAG_TYPE:
                    this.BagType = itemDatas.readInt();
                    break;
                case ItemDataField.CONSUMABLE:
                    this.Consumable = itemDatas.readBoolean();
                    break;
                case ItemDataField.POTION:
                    this.Potion = itemDatas.readBoolean();
                    break;
                case ItemDataField.SOULBOUND:
                    this.Soulbound = itemDatas.readBoolean();
                    break;
                case ItemDataField.TEX1:
                    this.Tex1 = itemDatas.readInt();
                    break;
                case ItemDataField.TEX2:
                    this.Tex2 = itemDatas.readInt();
                    break;
                case ItemDataField.TIER:
                    this.Tier = itemDatas.readInt();
                    break;
                case ItemDataField.DESCRIPTION:
                    this.Description = itemDatas.readUTF();
                    break;
                case ItemDataField.RATE_OF_FIRE:
                    this.RateOfFire = itemDatas.readFloat();
                    break;
                case ItemDataField.MP_COST:
                    this.MpCost = itemDatas.readInt();
                    break;
                case ItemDataField.FAME_BONUS:
                    this.FameBonus = itemDatas.readInt();
                    break;
                case ItemDataField.NUM_PROJECTILES:
                    this.NumProjectiles = itemDatas.readUnsignedByte();
                    break;
                case ItemDataField.ARC_GAP:
                    this.ArcGap = itemDatas.readFloat();
                    break;
                case ItemDataField.DISPLAY_ID:
                    this.DisplayId = itemDatas.readUTF();
                    break;
                case ItemDataField.COOLDOWN:
                    this.Cooldown = itemDatas.readInt();
                    break;
                case ItemDataField.RESURRECTS:
                    this.Resurrects = itemDatas.readBoolean();
                    break;
                case ItemDataField.DOSES:
                    this.Doses = itemDatas.readInt();
                    break;
                case ItemDataField.MAX_DOSES:
                    this.MaxDoses = itemDatas.readInt();
                    break;
                case ItemDataField.RARITY:
                    this.Rarity = itemDatas.readUTF();
                    break;
                case ItemDataField.GEMSTONE_LIMIT:
                    this.GemstoneLimit = itemDatas.readInt();
                    break;
                case ItemDataField.GEMSTONES:
                    var gemstones:Vector.<int> = new Vector.<int>();
                    var len:int = itemDatas.readUnsignedShort();
                    for (var ii:int = 0; ii < len; ii++) {
                        gemstones.push(itemDatas.readInt()); // Check how arrays are written/read
                    }
                    this.Gemstones = gemstones;
                    break;
                case ItemDataField.STAT_BOOSTS:
                    len = itemDatas.readUnsignedShort();
                    for (ii = 0; ii < len; ii++) {
                        var statBoost:StatBoost = this.StatsBoosts[ii] || new StatBoost(null);
                        statBoost.ParseData(itemDatas);

                        if (this.StatsBoosts.indexOf(statBoost) == -1){
                            this.StatsBoosts.push(statBoost);
                        }
                    }
                    break;
                case ItemDataField.ACTIVATE_EFFECTS:
                    len = itemDatas.readUnsignedShort();
                    for (ii = 0; ii < len; ii++) {
                        var aEffect:ActivateEffect = this.ActivateEffects[ii] || new ActivateEffect(null);
                        aEffect.ParseData(itemDatas);

                        if (this.ActivateEffects.indexOf(aEffect) == -1){
                            this.ActivateEffects.push(aEffect);
                        }
                    }
                    break;
                case ItemDataField.PROJECTILE:
                    if (!this.Projectile) {
                        continue;
                    }
                    this.Projectile.ParseData(itemDatas);
                    break;
                case ItemDataField.GEMSTONE:
                    this.Gemstone.ParseData(itemDatas);
                    break;
                case ItemDataField.ITEM_LEVEL:
                    this.ItemLevel = itemDatas.readInt();
                    break;
                case ItemDataField.LEVEL_INCREASES:
                    len = itemDatas.readUnsignedShort();
                    for (ii = 0; ii < len; ii++) {
                        var inc:LevelIncreaseDesc = this.LevelIncreases[ii] || new LevelIncreaseDesc(null);
                        inc.ParseData(itemDatas);

                        if (this.LevelIncreases.indexOf(inc) == -1){
                            this.LevelIncreases.push(inc);
                        }
                    }
                    break;
                case ItemDataField.UPGRADE_LEVELS:
                    this.UpgradeLevels = itemDatas.readInt();
                    break;
                case ItemDataField.MP_END_COST:
                    this.MpEndCost = itemDatas.readInt();
                    break;
                case ItemDataField.MAX_CHARGE:
                    this.MaxCharge = itemDatas.readInt();
                    break;
                case ItemDataField.MP_COST_PER_SECOND:
                    this.MpCostPerSecond = itemDatas.readInt();
                    break;
                case ItemDataField.SHEATH:
                    if (!this.Sheath){
                        continue;
                    }
                    this.Sheath.ParseData(itemDatas);
                    break;
                case ItemDataField.SPELL:
                    if (!this.Spell){
                        continue;
                    }
                    this.Spell.ParseData(itemDatas);
                    break;
                case ItemDataField.HELM:
                    if (!this.Helm){
                        continue;
                    }
                    this.Helm.ParseData(itemDatas);
                case ItemDataField.POISON:
                    if (!this.Poison){
                        continue;
                    }
                    this.Poison.ParseData(itemDatas);
                    break;
            }
        }
    }

    public static function GetStream(str:String):ByteArray {
        var itemDatas:Vector.<int> = ConversionUtil.toIntVector(str);
        var dataStream:ByteArray = new ByteArray();
        for (var i:int = 0; i < itemDatas.length; i++) { // Convert the itemDatas to a byte array to read from later
            dataStream.writeByte(itemDatas[i]);
        }
        dataStream.position = 0;
        dataStream.endian = Endian.LITTLE_ENDIAN;
        return dataStream;
    }

    public static function GetValue(xml:*, prop:String, defValue:*):* {
        if (!xml)
            return defValue;
        if (prop == "")
            return xml;
        if (xml && xml.hasOwnProperty(prop))
            return xml[prop];
        return defValue;
    }

    public static function HasValue(xml:*, prop:String):Boolean {
        if (!xml)
            return false;
        return xml.hasOwnProperty(prop);
    }
}
}
