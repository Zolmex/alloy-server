package kabam.rotmg.messaging.impl.outgoing {
import flash.utils.IDataOutput;

import kabam.rotmg.messaging.impl.data.WorldPosData;

public class EnemyHit extends OutgoingMessage {

    public var bulletId_:int;
    public var targetId_:int;
    public var elapsed_:int;
    public var targetPos:WorldPosData;

    public function EnemyHit(id:uint, callback:Function) {
        super(id, callback);
        this.targetPos = new WorldPosData();
    }

    override public function writeToOutput(data:IDataOutput):void {
        data.writeInt(this.bulletId_);
        data.writeInt(this.targetId_);
        data.writeInt(this.elapsed_);
        this.targetPos.writeToOutput(data);
    }

    override public function toString():String {
        return formatToString("ENEMYHIT", "time_", "hitPos", "bulletId_", "targetId_");
    }
}
}
