package kabam.rotmg.messaging.impl.data {
import flash.utils.IDataInput;

public class TradeItemData {

    public var itemType_:int;
    public var slotType_:int;
    public var tradeable_:Boolean;
    public var included_:Boolean;


    public function parseFromInput(data:IDataInput):void {
        this.itemType_ = data.readInt();
        this.slotType_ = data.readInt();
        this.tradeable_ = data.readBoolean();
        this.included_ = data.readBoolean();
    }

    public function toString():String {
        return "ItemType: " + itemType_ + " SlotType: " + slotType_ + " Tradeable: " + tradeable_ + " Included: " + included_;
    }


}
}
