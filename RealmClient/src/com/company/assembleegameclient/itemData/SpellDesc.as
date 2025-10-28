package com.company.assembleegameclient.itemData {
import flash.utils.ByteArray;

public class SpellDesc {

    public var MpCost:int;
    public var NumProjectiles:int;
    public var ProjectileId:int;

    public function SpellDesc(obj:*) {
        if (!obj)
            return;

        MpCost = ItemData.GetValue(obj, "MpCost", 0);
        NumProjectiles = ItemData.GetValue(obj, "NumProjectiles", 20);
        ProjectileId = ItemData.GetValue(obj, "ProjectileId", 0);
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
                    NumProjectiles = itemDatas.readByte();
                    break;
                case 2:
                    ProjectileId = itemDatas.readByte();
                    break;
            }
        }
    }
}
}
