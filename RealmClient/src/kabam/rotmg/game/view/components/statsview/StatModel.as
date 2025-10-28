package kabam.rotmg.game.view.components.statsview
{
   public class StatModel
   {
      public var name:String;
      public var description:String;
      public var prefix:String;
      public var suffix:String;

      public function StatModel(name:String, description:String, prefix:String = "", suffix:String = "")
      {
         super();
         this.name = name;
         this.description = description;
         this.prefix = prefix;
         this.suffix = suffix;
      }
   }
}
