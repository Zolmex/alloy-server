package kabam.rotmg.game.view.components.statsview.left
{
import kabam.rotmg.game.view.components.statsview.*;
import kabam.rotmg.game.view.components.*;
   import com.company.assembleegameclient.objects.Player;
   import kabam.rotmg.ui.signals.UpdateHUDSignal;
   import robotlegs.bender.bundles.mvcs.Mediator;
   
   public class AllStatsMediator extends Mediator
   {
      [Inject]
      public var view:AllStatsView;
      
      [Inject]
      public var updateHUD:UpdateHUDSignal;
      
      public function AllStatsMediator()
      {
         super();
      }
      
      override public function initialize() : void
      {
         this.updateHUD.add(this.onUpdateHUD);
      }
      
      override public function destroy() : void
      {
         this.updateHUD.remove(this.onUpdateHUD);
      }
      
      private function onUpdateHUD(player:Player) : void
      {
         this.view.draw(player);
      }
   }
}
