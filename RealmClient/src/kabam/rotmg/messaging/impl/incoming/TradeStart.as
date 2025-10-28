package kabam.rotmg.messaging.impl.incoming
{
import com.company.assembleegameclient.util.FreeList;
import flash.utils.IDataInput;
import kabam.rotmg.messaging.impl.data.TradeItemData;

public class TradeStart extends IncomingMessage
{


    public var myItems_:Vector.<TradeItemData>;

    public var theirName_:String;

    public var theirItems_:Vector.<TradeItemData>;

    public function TradeStart(id:uint, callback:Function)
    {
        this.myItems_ = new Vector.<TradeItemData>();
        this.theirItems_ = new Vector.<TradeItemData>();
        super(id,callback);
    }

    override public function parseFromInput(data:IDataInput) : void
    {
        var i:int = 0;
        var len:int = data.readByte();
        for(i = len; i < this.myItems_.length; i++)
        {
            FreeList.deleteObject(this.myItems_[i]);
        }
        this.myItems_.length = Math.min(len,this.myItems_.length);
        while(this.myItems_.length < len)
        {
            this.myItems_.push(FreeList.newObject(TradeItemData) as TradeItemData);
        }
        for(i = 0; i < len; i++)
        {
            this.myItems_[i].parseFromInput(data);
        }
        this.theirName_ = data.readUTF();
        len = data.readByte();
        for(i = len; i < this.theirItems_.length; i++)
        {
            FreeList.deleteObject(this.theirItems_[i]);
        }
        this.theirItems_.length = Math.min(len,this.theirItems_.length);
        while(this.theirItems_.length < len)
        {
            this.theirItems_.push(FreeList.newObject(TradeItemData) as TradeItemData);
        }
        for(i = 0; i < len; i++)
        {
            this.theirItems_[i].parseFromInput(data);
        }
    }

    override public function toString() : String
    {
        return formatToString("TRADESTART","myItems_","theirName_","theirItems_");
    }
}
}
