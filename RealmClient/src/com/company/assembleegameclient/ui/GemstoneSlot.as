package com.company.assembleegameclient.ui {
import com.company.assembleegameclient.objects.ObjectLibrary;
import com.company.util.ConversionUtil;

public class GemstoneSlot extends Slot {

    private var slotType:int;

    public function GemstoneSlot(slotId:int, slotType:int) {
        super(slotId, Slot.ALL_TYPE, 0, [1, 1, 1, 1], true);
        this.slotType = slotType;
    }

    public override function canHoldItem(type:int):Boolean{
        var xml:* = ObjectLibrary.getXMLfromId(ObjectLibrary.getIdFromType(type));
        if (!xml){
            return false;
        }
        if (!xml.hasOwnProperty("Gemstone")){
            return false;
        }
        var slotTypes:Vector.<int> = ConversionUtil.toIntVector(xml.Gemstone.@slotTypes);
        return slotTypes.indexOf(this.slotType) != -1;
    }
}
}
