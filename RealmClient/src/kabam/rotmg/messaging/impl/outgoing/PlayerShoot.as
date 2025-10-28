package kabam.rotmg.messaging.impl.outgoing {
import flash.utils.IDataOutput;

import kabam.rotmg.messaging.impl.data.WorldPosData;

public class PlayerShoot extends OutgoingMessage {
    
    public var angle_:Number;
    public var pos:WorldPosData;
    public var time:int;
    public var isServerShoot:Boolean;
    public var angleInc:Number;
    public var damageList:Vector.<int>;
    public var critList:Vector.<Number>;
    public var itemType:int;

    public function PlayerShoot(id:uint, callback:Function) {
        super(id, callback);
        this.pos = new WorldPosData();
    }

    override public function writeToOutput(data:IDataOutput):void {
        data.writeFloat(this.angle_);
        this.pos.writeToOutput(data);
        data.writeInt(this.time);
        data.writeBoolean(this.isServerShoot);
        if (this.isServerShoot){
            data.writeFloat(this.angleInc);
            data.writeByte(this.damageList.length);
            for (var i:int = 0; i < this.damageList.length; i++){
                data.writeInt(this.damageList[i]);
            }
            data.writeByte(this.critList.length);
            for (i = 0; i < this.critList.length; i++){
                data.writeFloat(this.critList[i]);
            }
            data.writeShort(this.itemType);
        }
    }

    override public function toString():String {
        return formatToString("PLAYERSHOOT", "angle_");
    }
}
}
