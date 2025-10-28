package kabam.rotmg.messaging.impl.incoming
{
   import flash.utils.IDataInput;
   
   public class StatsApplyResult extends IncomingMessage
   {
      public var success_:Boolean;

      public function StatsApplyResult(id:uint, callback:Function)
      {
         super(id,callback);
      }
      
      override public function parseFromInput(data:IDataInput) : void
      {
         this.success_ = data.readBoolean();
      }
      
      override public function toString() : String
      {
         return formatToString("STATSAPPLYRESULT","success_");
      }
   }
}
