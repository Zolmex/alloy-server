package kabam.rotmg.messaging.impl.incoming {
import flash.utils.IDataInput;

import kabam.rotmg.messaging.impl.data.WorldPosData;

public class ServerPlayerShoot extends IncomingMessage {

    public var startingPos_:WorldPosData;
    public var angle_:Number;
    public var angleInc_:Number;
    public var damageList:Vector.<int>;
    public var critList:Vector.<Number>;
    public var itemType_:int;

    public function ServerPlayerShoot(id:uint, callback:Function) {
        this.startingPos_ = new WorldPosData();
        this.damageList = new Vector.<int>();
        this.critList = new Vector.<Number>();
        super(id, callback);
    }

    override public function parseFromInput(data:IDataInput):void {
        this.startingPos_.parseFromInput(data);
        this.angle_ = data.readFloat();
        this.angleInc_ = data.readFloat();
        var len:int = data.readUnsignedByte();
        this.damageList.length = 0;
        for (var i:int = 0; i < len; i++){
            this.damageList.push(data.readInt());
        }
        var len2:int = data.readUnsignedByte();
        this.critList.length = 0;
        for (var ii:int = 0; ii < len2; ii++){
            this.critList.push(data.readFloat());
        }
        this.itemType_ = data.readShort();
    }

    override public function toString():String {
        return formatToString("SHOOT", "bulletId_", "ownerId_", "containerType_", "startingPos_", "angle_");
    }
}
}
