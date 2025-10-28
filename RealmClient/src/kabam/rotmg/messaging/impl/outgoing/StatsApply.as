package kabam.rotmg.messaging.impl.outgoing
{
   import flash.utils.IDataOutput;
   
   public class StatsApply extends OutgoingMessage
   {
      public var allocatedPoints:Array;

      public function StatsApply(id:uint, callback:Function)
      {
         super(id,callback);
      }
      
      override public function writeToOutput(data:IDataOutput) : void
      {
         for (var i:int = 0; i < allocatedPoints.length; i++)
         {
            data.writeInt(allocatedPoints[i]);
         }
      }
      
      override public function toString() : String
      {
         return formatToString("STATSAPPLY", "allocatedPoints");
      }
   }
}
