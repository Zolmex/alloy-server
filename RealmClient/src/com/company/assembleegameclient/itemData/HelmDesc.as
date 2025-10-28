package com.company.assembleegameclient.itemData {
import com.company.assembleegameclient.objects.projectiles.ProjectilePath;

import flash.utils.ByteArray;

public class HelmDesc {

    public var MpCost:int;
    public var Duration:int;
    public var StacksGain:Number;
    public var StacksLost:int;
    public var StatsModifier:int;
    public var HoldEffects:int;
    public var MpDrain:int;

    public function HelmDesc(obj:*) {
        if (!obj)
            return;

        this.MpCost = ItemData.GetValue(obj, "MpCost", 0);
        this.Duration = ItemData.GetValue(obj, "Duration", 0);
        this.StacksGain = ItemData.GetValue(obj, "StacksGain", 1);
        this.StacksLost = ItemData.GetValue(obj, "StacksLost", 0);
        this.StatsModifier = ItemData.GetValue(obj, "StatsModifier", 200);
        this.HoldEffects = ItemData.GetValue(obj, "HoldEffects", 0);
        this.MpDrain = ItemData.GetValue(obj, "MpDrain", 1000);
    }

    public function ParseData(itemDatas:ByteArray):void {
        var fieldCount:int = itemDatas.readByte();
        for (var i:int = 0; i < fieldCount; i++) {
            var field:int = itemDatas.readByte();
            switch (field) {
                case 0:
                    this.MpCost = itemDatas.readShort();
                    break;
                case 1:
                    this.Duration = itemDatas.readInt();
                    break;
                case 2:
                    this.StacksGain = itemDatas.readShort();
                    break;
                case 3:
                    this.StacksLost = itemDatas.readShort();
                    break;
//                case 4:
//                    this.StatsModifier = itemDatas.readInt();
//                    break;
//                case 5:
//                    this.HoldEffects = itemDatas.readFloat();
//                    break;
                case 6:
                    this.MpDrain = itemDatas.readShort();
                    break;
            }
        }
    }
}
}
