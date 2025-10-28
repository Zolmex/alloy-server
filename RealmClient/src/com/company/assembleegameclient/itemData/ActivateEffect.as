package com.company.assembleegameclient.itemData {
import com.company.assembleegameclient.util.ConditionEffect;

import flash.utils.ByteArray;

public class ActivateEffect {

    public var EffectName:String;
    public var EffectId:int;
    public var ConditionEffect:String;
    public var CondEffectId:int;
    public var CheckExistingEffect:int;
    public var TotalDamage:int;
    public var Radius:Number;
    public var CondEffectDuration:Number;
    public var DurationSec:Number;
    public var DurationMS:int;
    public var Amount:int;
    public var Range:Number;
    public var MaximumDistance:Number;
    public var ObjectId:String;
    public var MaxTargets:int;
    public var Color:uint;
    public var Stats:int;
    public var Cooldown:Number;
    public var RemoveSelf:Boolean;
    public var DungeonName:String;
    public var LockedName:String;
    public var Type:String;
    public var UseWisMod:Boolean;
    public var Target:String;
    public var Center:String;
    public var VisualEffect:int;
    public var AirDurationMS:int;
    public var SkinType:int;
    public var ImpactDmg:int;
    public var NodeReq:int;
    public var DosesReq:int;
    public var CurrencyName:String;
    public var Currency:int;
    public var HealAmount:int;
    public var NumShots:int;
    public var LevelIncrease:LevelIncreaseDesc;

    public function ActivateEffect(obj:*) {
        //trace(XML(obj).toXMLString());
        this.EffectName = ItemData.GetValue(obj, "", null);
        this.EffectId = ItemData.GetValue(obj, "Effect", 0);
        this.ConditionEffect = ItemData.GetValue(obj, "@effect", null);
        if (!this.ConditionEffect || this.ConditionEffect == "")
            this.ConditionEffect = ItemData.GetValue(obj, "@condEffect", null);
        this.CheckExistingEffect = ItemData.GetValue(obj, "@checkExistingEffect", 0);
        this.TotalDamage = ItemData.GetValue(obj, "@totalDamage", 0);
        this.Radius = ItemData.GetValue(obj, "@radius", 0);
        this.CondEffectDuration = ItemData.GetValue(obj, "@condDuration", 0);
        this.DurationSec = ItemData.GetValue(obj, "@duration", 0);
        this.DurationMS = this.DurationSec * 1000;
        this.Amount = ItemData.GetValue(obj, "@amount", 0);
        this.Range = ItemData.GetValue(obj, "@range", 0);
        this.MaximumDistance = ItemData.GetValue(obj, "@maxDistance", 0);
        this.ObjectId = ItemData.GetValue(obj, "@objectId", null);
        this.MaxTargets = ItemData.GetValue(obj, "@maxTargets", 0);
        this.Color = ItemData.GetValue(obj, "@color", 0);
        this.Stats = ItemData.GetValue(obj, "@stat", -1);
        this.Cooldown = ItemData.GetValue(obj, "@cooldown", 0);
        this.RemoveSelf = ItemData.GetValue(obj, "@removeSelf", false);
        this.DungeonName = ItemData.GetValue(obj, "@dungeonName", null);
        this.LockedName = ItemData.GetValue(obj, "@lockedName", null);
        this.Type = ItemData.GetValue(obj, "@type", null);
        this.UseWisMod = ItemData.GetValue(obj, "@useWisMod", false);
        this.Target = ItemData.GetValue(obj, "@target", null);
        this.Center = ItemData.GetValue(obj, "@center", null);
        this.VisualEffect = ItemData.GetValue(obj, "@visualEffect", 0);
        this.AirDurationMS = ItemData.GetValue(obj, "@airDurationMS", 1500);
        this.SkinType = ItemData.GetValue(obj, "@skinType", 0);
        this.ImpactDmg = ItemData.GetValue(obj, "@impactDmg", 0);
        this.NodeReq = ItemData.GetValue(obj, "@nodeReq", -1);
        this.DosesReq = ItemData.GetValue(obj, "@dosesReq", 0);
        this.CurrencyName = ItemData.GetValue(obj, "@currency", null);
        this.Currency = ItemData.GetValue(obj, "Currency", 0);
        this.HealAmount = ItemData.GetValue(obj, "@heal", 0);
        this.NumShots = ItemData.GetValue(obj, "@numShots", 0);
        if (ItemData.HasValue(obj, "LevelIncrease")) {
            this.LevelIncrease = new LevelIncreaseDesc(ItemData.GetValue(obj, "LevelIncrease", null));
        }
    }

    public function ParseData(itemDatas:ByteArray):void {
        var fieldCount:int = itemDatas.readByte();
        for (var i:int = 0; i < fieldCount; i++) {
            var field:int = itemDatas.readByte();
            switch (field) {
                case 0:
                    this.EffectId = itemDatas.readByte();
                    break;
                case 1:
                    this.CondEffectId = itemDatas.readByte();
                    this.CondEffectDuration = itemDatas.readInt();
                    break;
                case 2:
                    this.DurationMS = itemDatas.readInt();
                    break;
                case 3:
                    this.Range = itemDatas.readFloat();
                    break;
                case 4:
                    this.Amount = itemDatas.readInt();
                    break;
                case 5:
                    this.TotalDamage = itemDatas.readInt();
                    break;
                case 6:
                    this.Radius = itemDatas.readFloat();
                    break;
                case 7:
                    this.Color = itemDatas.readUnsignedInt();
                    break;
                case 8:
                    this.MaxTargets = itemDatas.readInt();
                    break;
                case 9:
                    this.ObjectId = itemDatas.readUTF();
                    break;
                case 10:
                    if (!this.LevelIncrease) {
                        return;
                    }
                    this.LevelIncrease.ParseData(itemDatas);
                    break;
                case 11:
                    this.NumShots = itemDatas.readInt();
                    break;
            }
        }
    }
}
}
