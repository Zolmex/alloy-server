package kabam.rotmg.messaging.impl.outgoing {
import flash.utils.IDataOutput;

public class GemstoneRemove extends OutgoingMessage {

    public var itemSlot:int;
    public var gemSlot:int;
    public var invSlot:int;

    public function GemstoneRemove(id:uint, callback:Function) {
        super(id, callback);
    }

    override public function writeToOutput(data:IDataOutput):void {
        data.writeByte(this.itemSlot);
        data.writeByte(this.gemSlot);
        data.writeByte(this.invSlot);
    }

    override public function toString():String {
        return formatToString("STATSAPPLY", "allocatedPoints");
    }
}
}
