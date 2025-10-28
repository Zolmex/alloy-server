package kabam.rotmg.ui.view
{
import com.company.assembleegameclient.game.GameSprite;
import com.company.assembleegameclient.objects.Player;
import com.company.assembleegameclient.ui.TradePanel;
import com.company.assembleegameclient.ui.panels.InteractPanel;
import com.company.assembleegameclient.ui.panels.itemgrids.EquippedGrid;
import com.company.assembleegameclient.ui.tooltip.TextToolTip;
import com.company.util.GraphicsUtil;
import com.company.util.SpriteUtil;

import flash.display.Bitmap;
import flash.display.BitmapData;
import flash.display.GraphicsPath;
import flash.display.GraphicsSolidFill;
import flash.display.IGraphicsData;
import flash.display.Sprite;
import flash.events.Event;
import flash.events.MouseEvent;
import flash.geom.Point;

import kabam.rotmg.assets.services.IconFactory;
import kabam.rotmg.game.view.components.TabStripView;
import kabam.rotmg.messaging.impl.incoming.TradeAccepted;
import kabam.rotmg.messaging.impl.incoming.TradeChanged;
import kabam.rotmg.messaging.impl.incoming.TradeStart;
import kabam.rotmg.minimap.view.MiniMap;

public class InCombatIcon extends Sprite
{
   public var iconFactory:IconFactory;

   public var toolTip:TextToolTip;
   public var inCombat:Boolean = false;
   public var combatIcon:Bitmap;
   public var inCombatIconData:BitmapData;
   public var notInCombatIconData:BitmapData;

   public var player:Player;

   public function InCombatIcon(player:Player) {
      super();
      this.player = player;
      this.iconFactory = new IconFactory();
      this.notInCombatIconData = this.iconFactory.makeIconBitmap(30).bitmapData;
      this.inCombatIconData = this.iconFactory.makeIconBitmap(31).bitmapData;
      this.toolTip = new TextToolTip(3552822, 10197915, null, "Been in combat for: " + (int)(player.timeInCombat / 1000) + " seconds.", 200);
      this.combatIcon = new Bitmap(notInCombatIconData);
      addChild(combatIcon);
      this.addEventListener(Event.ADDED_TO_STAGE, this.onAddedToStage);
      this.addEventListener(Event.REMOVED_FROM_STAGE, this.onRemovedFromStage);
   }

   public function onAddedToStage(e:Event):void {
      addEventListener(MouseEvent.MOUSE_OVER,this.onMouseOver);
      addEventListener(MouseEvent.MOUSE_OUT,this.onMouseOut);
      stage.addEventListener(Event.ENTER_FRAME, this.onEnterFrame);
   }

   private function onRemovedFromStage(e:Event) : void {
      removeEventListener(MouseEvent.MOUSE_OVER,this.onMouseOver);
      removeEventListener(MouseEvent.MOUSE_OUT,this.onMouseOut);
      stage.removeEventListener(Event.ENTER_FRAME, this.onEnterFrame);
      if (this.toolTip && this.toolTip.parent != null) {
         this.toolTip.parent.removeChild(this.toolTip);
      }
   }

   public function onEnterFrame(e:Event):void {
      this.toolTip.setText(inCombat ? "Been in combat for: " + (int)(player.timeInCombat / 1000) + " seconds." : "Not in combat.");
      if (inCombat != player.timeInCombat > 0) {
         inCombat = player.timeInCombat > 0;
         removeChild(combatIcon);
         this.combatIcon = new Bitmap(inCombat ? inCombatIconData : notInCombatIconData);
         addChild(combatIcon);
      }
   }

   public function onMouseOver(e:MouseEvent):void {
      if (!stage.contains(this.toolTip)) {
         stage.addChild(this.toolTip);
      }
   }

   public function onMouseOut(e:MouseEvent):void {
      if (this.toolTip.parent != null) {
         this.toolTip.parent.removeChild(this.toolTip);
      }
   }
}
}
