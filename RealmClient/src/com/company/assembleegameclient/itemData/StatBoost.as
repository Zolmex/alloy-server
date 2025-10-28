package com.company.assembleegameclient.itemData {
import com.company.assembleegameclient.itemData.LevelIncreaseDesc;

import flash.utils.ByteArray;

public class StatBoost {

    public var Stat:int;
    public var Amount:int;
    public var LevelIncrease:LevelIncreaseDesc;

    public function StatBoost(obj:*) {
        if (obj != null) {
            this.Stat = ItemData.GetValue(obj, "@stat", -1);
            this.Amount = ItemData.GetValue(obj, "@amount", 0);
            if (ItemData.HasValue(obj, "LevelIncrease")) {
                this.LevelIncrease = new LevelIncreaseDesc(ItemData.GetValue(obj, "LevelIncrease", null));
            }
        }
    }

    public function ParseData(itemDatas:ByteArray):void {
        var fieldCount:int = itemDatas.readByte();
        for (var i:int = 0; i < fieldCount; i++) {
            var field:int = itemDatas.readByte();
            switch (field) {
                case 0:
                    this.Stat = itemDatas.readInt();
                    break;
                case 1:
                    this.Amount = itemDatas.readInt();
                    break;
                case 2:
                    if (!this.LevelIncrease) {
                        return;
                    }
                    this.LevelIncrease.ParseData(itemDatas);
                    break;
            }
        }
    }
}
}
