package kabam.rotmg.fame.control
{
   import kabam.rotmg.core.signals.SetScreenSignal;
   import kabam.rotmg.fame.model.FameModel;
   import kabam.rotmg.fame.model.FameVO;
   import kabam.rotmg.fame.view.FameView;
import kabam.rotmg.stage3D.Renderer;

public class ShowFameViewCommand
   {
       
      
      [Inject]
      public var vo:FameVO;
      
      [Inject]
      public var model:FameModel;
      
      [Inject]
      public var setScreen:SetScreenSignal;
      
      public function ShowFameViewCommand()
      {
         super();
      }
      
      public function execute() : void
      {
         this.model.accountId = this.vo.getAccountId();
         this.model.characterId = this.vo.getCharacterId();
         Renderer.inDeathScreen = true;
         this.setScreen.dispatch(new FameView());
      }
   }
}
