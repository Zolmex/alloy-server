package kabam.rotmg.messaging.impl.outgoing
{
   import flash.utils.IDataOutput;
   
   public class ConstellationsSave extends OutgoingMessage
   {
      public var savedPrimaries:int;
      public var savedSecondaries:int;

      public function ConstellationsSave(id:uint, callback:Function)
      {
         super(id,callback);
      }
      
      override public function writeToOutput(data:IDataOutput) : void
      {
         data.writeInt(savedPrimaries);
         data.writeInt(savedSecondaries);
      }
      
      override public function toString() : String
      {
         return formatToString("CONSTELLATIONSSAVE", "savedPrimaries", "savedSecondaries");
      }
   }
}
