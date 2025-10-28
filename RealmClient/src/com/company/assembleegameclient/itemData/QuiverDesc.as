package com.company.assembleegameclient.itemData {
import flash.utils.ByteArray;

public class QuiverDesc {

    public var MpCost:int;
    public var ProjectileId:int;
    public var ArrowChance:Number;
    public var MaxArrows:int;
    public var MpPerArrow:int;
    public var UseMpArrows:Boolean;
    public var MpProjectileId:int;

    public function QuiverDesc(obj:*) {
        if (!obj)
            return;

        MpCost = ItemData.GetValue(obj, "MpCost", 0);
        ProjectileId = ItemData.GetValue(obj, "ProjectileId", 0);
        ArrowChance = ItemData.GetValue(obj, "ArrowChance", 100) / 100;
        MaxArrows = ItemData.GetValue(obj, "MaxArrows", 4);
        MpPerArrow = ItemData.GetValue(obj, "UseMpArrows", 0);
        UseMpArrows = MpPerArrow > 0;
        MpProjectileId = ItemData.GetValue(obj, "MpProjectileId", 0);
    }

    public function ParseData(itemDatas:ByteArray):void {
        var fieldCount:int = itemDatas.readByte();
        for (var i:int = 0; i < fieldCount; i++) {
            var field:int = itemDatas.readByte();
            switch (field) {
                case 0:
                    MpCost = itemDatas.readShort();
                    break;
                case 1:
                    ProjectileId = itemDatas.readByte();
                    break;
                case 2:
                    ArrowChance = itemDatas.readFloat();
                    break;
                case 3:
                    MaxArrows = itemDatas.readByte();
                    break;
                case 4:
                    MpPerArrow = itemDatas.readShort();
                    break;
                case 5:
                    UseMpArrows = itemDatas.readBoolean();
                    break;
                case 6:
                    MpProjectileId = itemDatas.readByte();
                    break;
            }
        }
    }
}
}
