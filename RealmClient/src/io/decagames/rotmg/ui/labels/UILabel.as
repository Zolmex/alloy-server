package io.decagames.rotmg.ui.labels
{
import com.company.assembleegameclient.parameters.Parameters;

import flash.text.Font;
import flash.text.TextField;
   import flash.text.TextFieldAutoSize;
import flash.text.TextFormat;

import realmeditor.editor.ui.embed.fonts.MyriadPro;

public class UILabel extends TextField
   {
      
      public static var DEBUG:Boolean = false;
      
      private var chromeFixMargin:int = 2;
      
      public function UILabel()
      {
         super();
         if(DEBUG)
         {
            this.debugDraw();
         }
         this.embedFonts = true;
         this.selectable = false;
         this.autoSize = TextFieldAutoSize.LEFT;
         var _local7:TextFormat = this.defaultTextFormat;
         _local7.font = Parameters.BEAUFORT;
         _local7.bold = false;
         _local7.size = 20;
         _local7.color = 0xFFFFFF;
         defaultTextFormat = _local7;
      }
      
      private function debugDraw() : void
      {
         this.border = true;
         this.borderColor = 16711680;
      }
      
      public function setTextWidth(param1:Number) : void
      {
         this.width = param1;
      }
      
      public function setAutoSize(param1:String) : void
      {
         this.autoSize = param1;
      }
      
      public function setMultiLine(param1:Boolean) : void
      {
         multiline = param1;
         wordWrap = param1;
      }
      
      override public function set y(param1:Number) : void
      {
         super.y = param1;
      }
      
      override public function get textWidth() : Number
      {
         return super.textWidth + 4;
      }
   }
}
