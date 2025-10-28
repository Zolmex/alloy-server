package kabam.rotmg.game.view.components.statsview.left
{
import com.company.assembleegameclient.ui.tooltip.TextToolTip;
import com.company.ui.SimpleText;
import com.company.util.CachingColorTransformer;

import flash.display.Bitmap;
import flash.display.Sprite;
import flash.events.Event;
import flash.events.MouseEvent;
import flash.geom.ColorTransform;
import flash.text.TextFieldAutoSize;
import kabam.rotmg.game.view.components.statsview.StatView;
import kabam.rotmg.game.view.components.statsview.StatModel;
import kabam.rotmg.game.view.components.statsview.assets.StatAssetsManager;

   public class StatHeader extends Sprite
   {
      private static var textColor:uint = 0xb3b3b3;

      private var model:Array;
      public var headerName:String;

      public var nameText_:SimpleText;
      public var container:Sprite;

      public var line:Sprite;

      public var toolTip:TextToolTip;
      public var statViews:Vector.<StatView>;

      public function StatHeader(name:String, model:Array)
      {
         super();
         this.toolTip = new TextToolTip(3552822, 10197915, "", "", 200);
         this.container = new Sprite();
         this.statViews = new Vector.<StatView>();
         this.headerName = name;
         this.model = model;

         makeAssets();

         addEventListener(Event.ADDED_TO_STAGE, this.onAddedToStage);
         addEventListener(Event.REMOVED_FROM_STAGE, this.onRemovedFromStage);
      }

      private function onAddedToStage(event:Event):void {
         for each(var stat:StatView in statViews) {
            stat.addEventListener(MouseEvent.MOUSE_OVER, this.onMouseOver);
            stat.addEventListener(MouseEvent.MOUSE_OUT, this.onMouseOut);
         }
      }

      private function onRemovedFromStage(event:Event):void {
         if (this.toolTip.parent) {
            this.toolTip.parent.removeChild(toolTip);
         }
         for each(var stat:StatView in statViews) {
            stat.removeEventListener(MouseEvent.MOUSE_OVER, this.onMouseOver);
            stat.removeEventListener(MouseEvent.MOUSE_OUT, this.onMouseOut);
         }
      }

      private function onMouseOver(event:MouseEvent):void {
         var stat:StatView = event.currentTarget is StatView ? event.currentTarget as StatView : event.target.parent && event.target.parent is StatView ? event.target.parent as StatView : null;

         if (stat) stat.transform.colorTransform = new ColorTransform(1.2,1.2,1.2);

         this.toolTip.setTitle(stat.statName);
         this.toolTip.setText(stat.description);

         if (!stage.contains(this.toolTip)) {
            stage.addChild(this.toolTip);
         }
      }

      private function onMouseOut(event:MouseEvent):void {
         var stat:StatView = event.currentTarget is StatView ? event.currentTarget as StatView : event.target.parent && event.target.parent is StatView ? event.target.parent as StatView : null;

         if (stat) stat.transform.colorTransform = new ColorTransform(1,1,1);

         if (this.toolTip.parent) {
            this.toolTip.parent.removeChild(toolTip);
         }
      }
      public function updateValXOffsets(num:int) : void
      {
         for each (var statView:StatView in this.statViews)
         {
            statView.valXOffset = num;
            statView.updateValPos();
         }
      }

      public function updateValColor(index:int, incAmount:int) : void
      {
         this.statViews[index].makeValText(incAmount);
      }

      public function getIcon(name:String) : Bitmap
      {
         return StatAssetsManager.getInstance().getAssetByName(name + "Icon");
      }

      public function makeAssets() : void
      {
         for (var i:int = 0; i < model.length; i++)
         {
            var statModel:StatModel = model[i];
            var icon:Bitmap = getIcon(statModel.name);
            var stat:StatView = new StatView(statModel.name,statModel.description, statModel.prefix,statModel.suffix, 186, 18, i % 2 == 0 ? 0x242222 : 0x363636, icon);
            stat.x = 0;
            stat.y = (headerName == "" ? 0 : 23) + (i * 18);
            this.container.addChild(stat);
            this.statViews.push(stat);
         }
         addChild(this.container);

         if (headerName == "")
            return;

         this.nameText_ = new SimpleText(16, textColor);
         this.nameText_.text = headerName;
         this.nameText_.setBold(true);
         this.nameText_.setAutoSize(TextFieldAutoSize.LEFT);
         this.nameText_.updateMetrics();
         addChild(this.nameText_);

         this.line = new Sprite();
         this.line.graphics.beginFill(0x676767);
         this.line.graphics.drawRect(0, 0, width - 4, 1);
         this.line.x = nameText_.x;
         this.line.y = nameText_.y + nameText_.height;
         addChild(line);
      }
   }
}
