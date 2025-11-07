package com.company.assembleegameclient.itemData {
import com.company.assembleegameclient.objects.projectiles.ProjectilePath;

import flash.utils.ByteArray;

public class PoisonDesc {

    public var MpCost:int;
    public var Effects:Vector.<ConditionEffectDesc>;
    public var Damage:int;
    public var TickSpeed:int;
    public var Duration:int;
    public var ThrowRange:Number;
    public var ThrowTravelTime:int;
    public var PoisonRange:Number;
    public var SpreadRange:Number;
    public var SpreadTargetNum:int;
    public var SpreadEfficiency:Number;
    public var ImpactDamage:int;

    public function PoisonDesc(obj:*) {
        if (!obj)
            return;

        this.MpCost = ItemData.GetValue(obj, "MpCost", 0);
        this.Damage = ItemData.GetValue(obj, "Damage", 0);
        this.TickSpeed = ItemData.GetValue(obj, "TickSpeed", 0);
        this.Duration = ItemData.GetValue(obj, "Duration", 0);
        this.ThrowRange = ItemData.GetValue(obj, "ThrowRange", 0);
        this.ThrowTravelTime = ItemData.GetValue(obj, "ThrowTravelTime", 0);
        this.PoisonRange = ItemData.GetValue(obj, "PoisonRange", 0);
        this.SpreadRange = ItemData.GetValue(obj, "SpreadRange", 0);
        this.SpreadTargetNum = ItemData.GetValue(obj, "SpreadTargetNum", 0);
        this.SpreadEfficiency = ItemData.GetValue(obj, "SpreadEfficiency", 0);
        this.ImpactDamage = ItemData.GetValue(obj, "ImpactDamage", 0);
        if (ItemData.HasValue(obj, "ConditionEffect")) {
            this.Effects = new Vector.<ConditionEffectDesc>();
            for each (var eff:* in ItemData.GetValue(obj, "ConditionEffect", null)) {
                this.Effects.push(new ConditionEffectDesc(ItemData.GetValue(eff, "", 0), ItemData.GetValue(eff, "DurationMS/@duration", 0)));
            }
        }
    }

    public function ParseData(itemDatas:ByteArray):void {
        var fieldCount:int = itemDatas.readUnsignedByte();
        for (var i:int = 0; i < fieldCount; i++) {
            var field:int = itemDatas.readUnsignedByte();
            switch (field) {
                case 0:
                    this.MpCost = itemDatas.readInt();
                    break;
                case 1:
                    var effects:Vector.<ConditionEffectDesc> = new Vector.<ConditionEffectDesc>();
                    var len:int = itemDatas.readUnsignedShort();
                    for (i = 0; i < len; i++) {
                        var effId:int = itemDatas.readUnsignedByte();
                        var duration:int = itemDatas.readInt();
                        effects.push(new ConditionEffectDesc(null, duration));
                    }
                    break;
                case 2:
                    this.Damage = itemDatas.readInt();
                    break;
                case 3:
                    this.TickSpeed = itemDatas.readInt();
                    break;
                case 4:
                    this.Duration = itemDatas.readInt();
                    break;
                case 5:
                    this.ThrowRange = itemDatas.readFloat();
                    break;
                case 6:
                    this.ThrowTravelTime = itemDatas.readInt();
                    break;
                case 7:
                    this.PoisonRange = itemDatas.readFloat();
                    break;
                case 8:
                    this.SpreadRange = itemDatas.readFloat();
                    break;
                case 9:
                    this.SpreadTargetNum = itemDatas.readInt();
                    break;
                case 10:
                    this.SpreadEfficiency = itemDatas.readFloat();
                    break;
                case 11:
                    this.ImpactDamage = itemDatas.readInt();
                    break;
            }
        }
    }
}
}
