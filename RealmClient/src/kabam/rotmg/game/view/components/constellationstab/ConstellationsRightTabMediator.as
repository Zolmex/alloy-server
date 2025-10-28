package kabam.rotmg.game.view.components.constellationstab
{
import kabam.rotmg.game.view.components.constellationstab.ConstellationsRightTab;
import kabam.rotmg.game.view.components.statsview.right.*;
import kabam.rotmg.game.view.components.statsview.*;
import kabam.rotmg.game.view.components.*;
   import com.company.assembleegameclient.objects.Player;
   import kabam.rotmg.ui.signals.UpdateHUDSignal;
   import robotlegs.bender.bundles.mvcs.Mediator;
   
   public class ConstellationsRightTabMediator extends Mediator
   {
      [Inject]
      public var view:ConstellationsRightTab;
      
      [Inject]
      public var updateHUD:UpdateHUDSignal;
      
      public function ConstellationsRightTabMediator()
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
         this.view.update(player);
      }
   }
}
