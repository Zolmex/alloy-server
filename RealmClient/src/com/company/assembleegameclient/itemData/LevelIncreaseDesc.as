package com.company.assembleegameclient.itemData {
import flash.utils.ByteArray;

public class LevelIncreaseDesc {

    public var Field:String;
    public var Rate:int;
    public var Amount:Number;

    public function LevelIncreaseDesc(obj:*) {
        if (obj != null) {
            this.Field = ItemData.GetValue(obj, "", null);
            this.Rate = ItemData.GetValue(obj, "@amount", 0);
            this.Amount = ItemData.GetValue(obj, "@rate", 0);
        }
    }

    public function ParseData(itemDatas:ByteArray):void {
        var fieldCount:int = itemDatas.readUnsignedByte();
        for (var i:int = 0; i < fieldCount; i++) {
            var field:int = itemDatas.readUnsignedByte();
            switch (field) {
                case 0:
                    this.Field = itemDatas.readUTF();
                    break;
                case 1:
                    this.Rate = itemDatas.readInt();
                    break;
                case 2:
                    this.Amount = itemDatas.readFloat();
                    break;
            }
        }
    }
}
}
