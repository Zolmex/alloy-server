package kabam.rotmg.core.view
{
import com.company.assembleegameclient.game.GameSprite;

import flash.display.Sprite;
import flash.events.Event;

import kabam.rotmg.ui.view.NewEditorScreen;

public class ScreensView extends Sprite
   {
       
      
      private var current:Sprite;
      
      private var previous:Sprite;
      
      public function ScreensView()
      {
         super();
         addEventListener(Event.ADDED_TO_STAGE, this.onAddedToStage);
      }

      private function onAddedToStage(e:Event):void{
         stage.addEventListener(Event.RESIZE, this.onStageResize);
      }

      private function onStageResize(e:Event):void {
         if (this.current is GameSprite || this.current is NewEditorScreen){
            this.scaleX = 1;
            this.scaleY = 1;
            return;
         }

         var scaleW:Number = WebMain.sWidth / 800;
         var scaleH:Number = WebMain.sHeight / 600;
         this.scaleX = scaleW;
         this.scaleY = scaleH;
      }
      
      public function setScreen(sprite:Sprite) : void
      {
         if(this.current == sprite)
         {
            return;
         }
         this.removePrevious();
         this.current = sprite;
         addChild(sprite);
         this.onStageResize(null);
      }
      
      private function removePrevious() : void
      {
         if(this.current && contains(this.current))
         {
            this.previous = this.current;
            removeChild(this.current);
         }
      }
      
      public function getPrevious() : Sprite
      {
         return this.previous;
      }
   }
}
