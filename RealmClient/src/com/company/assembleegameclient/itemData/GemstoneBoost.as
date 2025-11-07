package com.company.assembleegameclient.itemData {
import flash.utils.ByteArray;

public class GemstoneBoost {

    public var BoostType:String;
    public var Stat:String;
    public var DisplayStat:String;
    public var Amount:Number;
    public var BoostTarget:String;

    public function GemstoneBoost(obj:*) {
        if (obj == null){
            return;
        }

        this.Stat = ItemData.GetValue(obj, "@stat", "Unknown");
        this.DisplayStat = ItemData.GetValue(obj, "@display", null);
        var amt:String = ItemData.GetValue(obj, "@amount", null);
        var percentIndex:int = amt.indexOf("%");
        if (percentIndex != -1) {
            this.BoostType = "Percentage";
            this.Amount = Number(amt.substr(0, percentIndex));
        }
        else{
            this.BoostType = "Static";
            this.Amount = Number(amt);
        }
        this.BoostTarget = ItemData.GetValue(obj, "", null); // Player/Item
    }

    public function ParseData(itemDatas:ByteArray):void {
        var fieldCount:int = itemDatas.readUnsignedByte();
        for (var i:int = 0; i < fieldCount; i++) {
            var field:int = itemDatas.readUnsignedByte();
            switch (field) {
                case 0:
                    this.BoostType = itemDatas.readUTF();
                    break;
                case 1:
                    this.Stat = itemDatas.readUTF();
                    break;
                case 2:
                    this.Amount = itemDatas.readFloat();
                    break;
                case 3:
                    this.BoostTarget = itemDatas.readUTF();
                    break;
            }
        }
    }
}
}
