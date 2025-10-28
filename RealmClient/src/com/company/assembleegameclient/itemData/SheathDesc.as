package com.company.assembleegameclient.itemData {
import com.company.assembleegameclient.objects.projectiles.ProjectilePath;

import flash.utils.ByteArray;

public class SheathDesc {

    public var Capacity:int;
    public var SlashDamage:int;
    public var Efficiency:Number;
    public var ManaPerSlash:int;
    public var SlashCooldownMS:int;
    public var Radius:Number;
    public var Effects:Vector.<ConditionEffectDesc>;
    public var StanceDuration:int;

    public function SheathDesc(obj:*) {
        if (!obj)
            return;

        this.Capacity = ItemData.GetValue(obj, "Capacity", 0);
        this.SlashDamage = ItemData.GetValue(obj, "SlashDamage", 0);
        this.Efficiency = ItemData.GetValue(obj, "Efficiency", 1);
        this.ManaPerSlash = ItemData.GetValue(obj, "ManaPerSlash", 0);
        this.SlashCooldownMS = ItemData.GetValue(obj, "SlashCooldownMS", 200);
        this.Radius = ItemData.GetValue(obj, "Radius", 0);
        this.StanceDuration = ItemData.GetValue(obj, "StanceDuration", 1000);
        if (ItemData.HasValue(obj, "ConditionEffect")) {
            this.Effects = new Vector.<ConditionEffectDesc>();
            for each (var eff:* in ItemData.GetValue(obj, "ConditionEffect", null)) {
                this.Effects.push(new ConditionEffectDesc(ItemData.GetValue(eff, "", 0), ItemData.GetValue(eff, "DurationMS/@duration", 0)));
            }
        }
    }

    public function ParseData(itemDatas:ByteArray):void {
        var fieldCount:int = itemDatas.readByte();
        for (var i:int = 0; i < fieldCount; i++) {
            var field:int = itemDatas.readByte();
            switch (field) {
                case 0:
                    this.Capacity = itemDatas.readInt();
                    break;
                case 1:
                    this.SlashDamage = itemDatas.readInt();
                    break;
                case 2:
                    this.Efficiency = itemDatas.readFloat();
                    break;
                case 3:
                    this.ManaPerSlash = itemDatas.readInt();
                    break;
                case 4:
                    this.SlashCooldownMS = itemDatas.readInt();
                    break;
                case 5:
                    this.Radius = itemDatas.readFloat();
                    break;
                case 6:
                    var effects:Vector.<ConditionEffectDesc> = new Vector.<ConditionEffectDesc>();
                    var len:int = itemDatas.readUnsignedShort();
                    for (i = 0; i < len; i++) {
                        var effId:int = itemDatas.readByte();
                        var duration:int = itemDatas.readInt();
                        effects.push(new ConditionEffectDesc(null, duration));
                    }
                    break;
                case 7:
                    this.StanceDuration = itemDatas.readInt();
                    break;
            }
        }
    }
}
}
