package kabam.rotmg.messaging.impl.outgoing
{
   import flash.utils.IDataOutput;
   
   public class ConstellationsTrial extends OutgoingMessage
   {
      public function ConstellationsTrial(id:uint, callback:Function)
      {
         super(id,callback);
      }
      
      override public function writeToOutput(data:IDataOutput) : void
      {
      }
      
      override public function toString() : String
      {
         return formatToString("CONSTELLATIONSTRIAL");
      }
   }
}
