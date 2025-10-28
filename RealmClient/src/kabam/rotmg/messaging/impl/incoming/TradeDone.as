package kabam.rotmg.messaging.impl.incoming
{
import flash.utils.IDataInput;

public class TradeDone extends IncomingMessage
{

    public static const TRADE_SUCCESSFUL:int = 0;

    public static const PLAYER_CANCELED:int = 1;

    public static const TRADE_ERROR:int = 2;


    public var code_:int;

    public var description_:String;

    public function TradeDone(id:uint, callback:Function)
    {
        super(id,callback);
    }

    override public function parseFromInput(data:IDataInput) : void
    {
        this.code_ = data.readByte();
        switch (code_) {
            case 0:
                description_ = "Trade Successful";
                break;
            case 1:
                description_ = "Trade Canceled";
                break;
            case 2:
                description_ = "An error occurred while trading"
        }
    }

    override public function toString() : String
    {
        return formatToString("TRADEDONE","code_","description_");
    }
}
}
