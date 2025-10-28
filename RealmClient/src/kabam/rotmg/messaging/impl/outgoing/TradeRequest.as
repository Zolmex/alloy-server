package kabam.rotmg.messaging.impl.outgoing {
import flash.utils.IDataOutput;

public class TradeRequest extends OutgoingMessage
{
    public var name:String;

    public function TradeRequest(id:uint, callback:Function)
    {
        super(id,callback);
    }

    override public function writeToOutput(data:IDataOutput) : void
    {
        data.writeUTF(this.name);
    }

    override public function toString() : String
    {
        return formatToString("TRADEREQUEST","name");
    }
}
}
