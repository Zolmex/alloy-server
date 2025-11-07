package kabam.rotmg.messaging.impl.incoming {
import flash.utils.IDataInput;

public class DamageCounterUpdate extends IncomingMessage {

    public var targetId:int;
    public var playerDamage:uint;
    public var topDamagers:Vector.<Object>;

    public function DamageCounterUpdate(id:uint, callback:Function)
    {
        super(id,callback);
    }

    override public function parseFromInput(data:IDataInput) : void
    {
        targetId = data.readInt();

        playerDamage = 0;
        topDamagers = new Vector.<Object>();

        if (targetId == -1)
            return;

        playerDamage = data.readUnsignedInt();

        var key:int;
        var value:uint;
        var count:int = data.readUnsignedByte();
        for (var i:int = 0; i < count; i++) {
            key = data.readInt();
            value = data.readUnsignedInt();
            topDamagers.push({key: key, value: value});
        }
    }

    override public function toString() : String
    {
        return formatToString("DAMAGECOUNTERUPDATE","targetId", "playerDamage", "topDamagers");
    }
}
}
