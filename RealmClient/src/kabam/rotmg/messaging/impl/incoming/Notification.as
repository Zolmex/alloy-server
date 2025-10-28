package kabam.rotmg.messaging.impl.incoming
{
   import flash.utils.IDataInput;
   
   public class Notification extends IncomingMessage
   {
      public var objectId_:int;
      public var text_:String;
      public var color_:int;
      public var size_:int;
      public var isDamage:Boolean;

      public function Notification(id:uint, callback:Function)
      {
         super(id,callback);
      }
      
      override public function parseFromInput(data:IDataInput) : void
      {
         this.objectId_ = data.readInt();
         this.text_ = data.readUTF();
         this.color_ = data.readInt();
         this.size_ = data.readInt();
         this.isDamage = data.readBoolean();
      }
      
      override public function toString() : String
      {
         return formatToString("NOTIFICATION","objectId_","text_","color_","size_");
      }
   }
}
