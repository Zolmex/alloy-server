package kabam.rotmg.messaging.impl.outgoing
{
   import flash.utils.IDataOutput;
   
   public class PartyInvite extends OutgoingMessage
   {
      public var objId:int;

      public function PartyInvite(id:uint, callback:Function)
      {
         super(id,callback);
      }
      
      override public function writeToOutput(data:IDataOutput) : void
      {
         data.writeInt(objId);
      }
      
      override public function toString() : String
      {
         return formatToString("PARTYINVITE","objId");
      }
   }
}
