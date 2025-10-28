package kabam.rotmg.game.view.components.statsview
{
   import com.company.assembleegameclient.ui.tooltip.TextToolTip;
import com.company.assembleegameclient.ui.tooltip.TooltipHelper;
import com.company.ui.SimpleText;

import flash.display.Bitmap;

import flash.display.BitmapData;
import flash.display.Sprite;
   import flash.events.MouseEvent;
   import flash.filters.DropShadowFilter;
import flash.globalization.NumberFormatter;
import flash.text.TextFieldAutoSize;
import flash.text.TextFormat;
   import org.osflash.signals.natives.NativeSignal;
   
   public class StatView extends Sprite
   {
      public static var nf:NumberFormatter = new NumberFormatter("en-US");

      public var offsetX:int = (int)((Number)(w_ * 0.05));

      private static var textColor:uint = 0xb3b3b3;
      private static var valColor:uint = 0xffff8f;

      private static var valPosColor:uint = 0x40ee51;
      private static var valNegColor:uint = 0xff4e39;

      public var statName:String;
      public var description:String;
      public var w_:int;
      public var h_:int;
      public var bgColor:uint;
      public var valXOffset:int = 0;

      public var prefix:String;
      public var suffix:String;

      public var incAmount:int;

      public var nameText_:SimpleText;
      public var valText_:SimpleText;
      public var icon:Bitmap;

      public var val_:Number = -1;

      public function StatView(name:String, desc:String, prefix:String = "", suffix:String = "", width:int = 186, height:int = 18, bgColor:uint = 0x242222, icon:Bitmap = null)
      {
         super();
         nf.trailingZeros = true;
         nf.fractionalDigits = 1;
         this.statName = name;
         this.description = desc;
         this.w_ = width;
         this.h_ = height;
         this.bgColor = bgColor;
         this.prefix = prefix; // idk but maybe it will have use
         this.suffix = suffix; // for seconds or percentage
         this.icon = icon;
         makeAssets();
      }

      public function makeAssets() : void
      {
         var offsetX2:int = (int)((Number)(w_ * 0.12))
         var offsetY:int = (int)((Number)(h_ * 0.166));

         var background:Sprite = new Sprite();
         background.graphics.beginFill(bgColor);
         background.graphics.drawRect(0, 0, w_, h_);
         background.graphics.endFill();
         addChild(background);

         this.nameText_ = new SimpleText(13, textColor);
         this.nameText_.text = statName;
         this.nameText_.setAutoSize(TextFieldAutoSize.LEFT);
         this.nameText_.updateMetrics();
         addChild(this.nameText_);

         makeValText();

         if (icon)
         {
            this.icon.x = 5 + offsetX;
            this.icon.y = offsetY;
            addChild(icon);
         }
         this.nameText_.x = icon ? offsetX2 : 1;
         this.nameText_.y = height / 2 - nameText_.height / 2 - 1;
         this.nameText_.updateMetrics();
      }

      public function makeValText(incAmount:int = 0) : void
      {
         this.incAmount = incAmount;
         if (valText_ && valText_.parent) {
            removeChild(this.valText_);
            this.valText_ = null;
         }
         this.valText_ = new SimpleText(13, incAmount == 0 ? valColor : incAmount > 0 ? valPosColor : incAmount < 0 ? valNegColor : valColor);
         this.valText_.setBold(incAmount != 0);
         this.valText_.text = "-";
         this.valText_.setAutoSize(TextFieldAutoSize.RIGHT);
         addChild(this.valText_);
         updateValText();
      }

      public function draw(val:Number) : void
      {
         if (val == val_)
            return;

         this.val_ = val;
         updateValText();
      }

      public function getIncIcon(val:int) : String
      {
         return val > 0 ? "+" : "-";
      }

      public function updateValText() : void
      {
         this.valText_.text = prefix + getValText(val_) + suffix + (incAmount > 0 ? " " + getIncIcon(incAmount) + incAmount: "");
         updateValPos();
      }

      public function updateValPos() : void
      {
         this.valText_.updateMetrics();
         this.valText_.x = width - this.valText_.width - offsetX + valXOffset - 5;
      }

      public function getValText(val:Number):String
      {
         return String(isFloatStat() ? nf.formatNumber(this.val_) : val);
      }

      public function isFloatStat():Boolean {
         switch (this.statName) {
            case StatModels.CRITICAL_CHANCE_NAME:
            case StatModels.DODGE_CHANCE_NAME:
            case StatModels.MOVEMENT_SPEED_NAME:
               return true;
            default:
               return false;
         }
      }
   }
}
