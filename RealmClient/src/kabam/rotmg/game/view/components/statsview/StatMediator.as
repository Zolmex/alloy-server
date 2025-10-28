package kabam.rotmg.game.view.components.statsview
{
import kabam.rotmg.game.view.components.*;
   import flash.events.MouseEvent;

import kabam.rotmg.game.view.components.statsview.StatView;

import robotlegs.bender.bundles.mvcs.Mediator;
   
   public class StatMediator extends Mediator
   {
       
      
      [Inject]
      public var view:StatView;
      
      public function StatMediator()
      {
         super();
      }
      
      override public function initialize() : void
      {
      }
      
      override public function destroy() : void
      {
      }
   }
}
