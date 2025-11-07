package com.company.assembleegameclient.itemData {
import com.company.util.ConversionUtil;

import flash.utils.ByteArray;

public class GemstoneDesc {

    public var SlotTypes:Vector.<int>;
    public var Origin:String;
    public var Boosts:Vector.<GemstoneBoost>;

    public function GemstoneDesc(obj:*) {
        if (obj == null) {
            return;
        }

        this.SlotTypes = ConversionUtil.toIntVector(ItemData.GetValue(obj, "@slotTypes", null));
        this.Origin = ItemData.GetValue(obj, "@origin", 0);
        if (ItemData.HasValue(obj, "Boost")) {
            this.Boosts = new Vector.<GemstoneBoost>();
            for each (var boost:* in ItemData.GetValue(obj, "Boost", null)) {
                this.Boosts.push(new GemstoneBoost(boost));
            }
        }
    }

    public function ParseData(itemDatas:ByteArray):void {
        var fieldCount:int = itemDatas.readUnsignedByte();
        for (var i:int = 0; i < fieldCount; i++) {
            var field:int = itemDatas.readUnsignedByte();
            switch (field) {
                case 0:
                    var len:int = itemDatas.readInt();
                    var slotTypes:Vector.<int> = new Vector.<int>(len);
                    for (i = 0; i < len; i++) {
                        slotTypes.push(itemDatas.readInt());
                    }
                    this.SlotTypes = slotTypes;
                    break;
                case 1:
                    this.Origin = itemDatas.readUTF();
                    break;
                case 2:
                    len = itemDatas.readInt();
                    var boosts:Vector.<GemstoneBoost> = new Vector.<GemstoneBoost>(len);
                    for (i = 0; i < len; i++) {
                        var boost:GemstoneBoost = new GemstoneBoost(null);
                        boost.ParseData(itemDatas);
                        boosts.push(boost);
                    }
                    this.Boosts = boosts;
                    break;
            }
        }
    }
}
}
