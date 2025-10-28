package kabam.rotmg.game.view.components.statsview.right
{
import com.company.assembleegameclient.parameters.Parameters;
import com.company.assembleegameclient.ui.tooltip.TooltipHelper;
import com.company.ui.SimpleText;
import com.company.util.KeyCodes;
import com.gskinner.motion.GTween;

import flash.display.Bitmap;
import flash.events.Event;
import flash.text.TextFieldAutoSize;

import kabam.rotmg.game.view.components.*;

import com.company.assembleegameclient.game.GameSprite;
import com.company.assembleegameclient.objects.Player;
import com.company.assembleegameclient.screens.TitleMenuOption;

import flash.display.Graphics;
   import flash.display.Sprite;
   import flash.events.MouseEvent;

import kabam.rotmg.game.view.components.statsview.assets.StatAssetsManager;
import kabam.rotmg.game.view.components.statsview.left.StatHeader;
import kabam.rotmg.game.view.components.statsview.right.StatIncrementButton;
import kabam.rotmg.game.view.components.statsview.StatModel;
import kabam.rotmg.game.view.components.statsview.assets.StatAssetsSheet;
import kabam.rotmg.game.view.components.statsview.StatModels;
import kabam.rotmg.game.view.components.statsview.right.StatUIButton;
import kabam.rotmg.game.view.components.statsview.StatView;

import org.osflash.signals.natives.NativeSignal;
   
   public class StatsView extends Sprite
   {
      private static const WIDTH:int = 185;
      private static const HEIGHT:int = 130;

      public var statPoints:int = 0;
      public var editing:Boolean;

      public var statHeader:StatHeader;
      public var statPointsText:SimpleText;

      public var editButton:StatUIButton;
      public var applyButton:StatUIButton;
      public var cancelButton:StatUIButton;

      public var incrementButtons:Vector.<StatIncrementButton>;
      public var allocatedPoints:Array;

      public var gs_:GameSprite;

      public function StatsView(gs:GameSprite)
      {
         super();
         this.gs_ = gs;
         this.allocatedPoints = [ 0, 0, 0, 0 ];

         this.statHeader = new StatHeader("", StatModels.baseStatModels);
         this.statHeader.y = 28;
         addChild(statHeader);

         this.statPointsText = new SimpleText(14, 0xb3b3b3);
         this.statPointsText.setAutoSize(TextFieldAutoSize.CENTER);
         this.statPointsText.setBold(true);
         updateStatPointsText();
         addChild(statPointsText);

         this.editButton = new StatUIButton(this, "Edit Stats", true);
         this.editButton.x = WIDTH / 2 - this.editButton.width / 2;
         this.editButton.y = HEIGHT - 25;
         this.editButton.activate();
         addChild(this.editButton);

         this.applyButton = new StatUIButton(this, "Apply", false);
         this.applyButton.x = (WIDTH - WIDTH / 4) - this.applyButton.width / 2;
         this.applyButton.y = HEIGHT - 25;

         this.cancelButton = new StatUIButton(this, "Cancel", false);
         this.cancelButton.x = WIDTH / 4 - this.cancelButton.width / 2;
         this.cancelButton.y = HEIGHT - 25;

         addEventListener(Event.REMOVED_FROM_STAGE, onRemovedFromStage);
      }

      public function updateStatPointsText() : void
      {
         var text:String = editing ? "Stat Points: " + (statPoints > 0 ? TooltipHelper.wrapInFontTag(statPoints + "", "0xffffef") : statPoints ) : "Stats";
         this.statPointsText.htmlText = text;
         this.statPointsText.updateMetrics();
         this.statPointsText.x = WIDTH / 2 - this.statPointsText.width / 2;
         this.statPointsText.y = 5;
      }

      public function onShowAllStats(e:MouseEvent):void
      {
         (this.parent.parent.parent.parent as GameSprite).mui_.toggleStatsView(false);
      }

      public function incButtonClicked(btn:StatIncrementButton) : void
      {
         var incAmt:int = gs_.mui_.activeKeys[KeyCodes.SHIFT] ? 5 : 1;

         if (btn.add)
         {
            if (this.statPoints - incAmt >= 0)
            {
               this.allocatedPoints[btn.index] += incAmt;
               this.statPoints -= incAmt;
            }
         }
         else
         {
            if (this.statPoints + incAmt <= gs_.map.player_.statPoints_)
            {
               this.allocatedPoints[btn.index] -= incAmt;
               this.statPoints += incAmt;
            }
         }
         updateStatPointsText();

         this.statHeader.updateValColor(btn.index, allocatedPoints[btn.index]);
         reloadIncButtons();
      }
      public function reloadIncButtons() : void {
         for each (var incBtn:StatIncrementButton in  this.incrementButtons)
            incBtn.reload(incBtn.add ? statPoints : allocatedPoints[incBtn.index])
      }
      public function uiButtonClicked(btn:StatUIButton) : void {
         switch (btn.type_)
         {
            case "Edit Stats":
               toggleEdit(true);
               break;
            case "Apply":
               applyStats();
               break;
            case "Cancel":
               toggleEdit(false);
               break;
            default:
               throw new Error("lalala");
         }
      }
      public function toggleEdit(toggle:Boolean) : void
      {
         this.editing = toggle;
         this.allocatedPoints = [ 0, 0, 0, 0 ];
         if (toggle)
         {
            this.statHeader.updateValXOffsets(-35);

            this.editButton.deactivate();
            removeChild(editButton);

            addChild(applyButton);
            this.applyButton.activate();

            addChild(cancelButton);
            this.cancelButton.activate();

            makeIncButtons();
         } else {
            this.statHeader.updateValXOffsets(0);

            for (var i:int = 0; i < 4; i++)
               this.statHeader.updateValColor(i, 0);

            this.statPoints = gs_.map.player_.statPoints_;

            for each (var btn:StatIncrementButton in incrementButtons)
            {
               removeChild(btn);
               btn.deactivate();
               btn = null;
            }
            addChild(editButton)
            this.editButton.activate();

            this.applyButton.deactivate();
            removeChild(applyButton);

            this.cancelButton.deactivate();
            removeChild(cancelButton);
         }
         updateStatPointsText();
      }
      public function makeIncButtons() : void
      {
         this.incrementButtons = new Vector.<StatIncrementButton>();
         for (var i:int = 0; i < 4; i++)
         {
            var addButton:StatIncrementButton = new StatIncrementButton(this, i, true);
            addButton.x = 150;
            addButton.y = 28 + (i * 18);
            addButton.reload(statPoints);
            this.incrementButtons.push(addButton);
            addChild(addButton);

            var subButton:StatIncrementButton = new StatIncrementButton(this, i, false);
            subButton.x = 168;
            subButton.y = 28 + (i * 18);
            subButton.reload(allocatedPoints[i]); //checks the allocatedpoints of the sub button
            this.incrementButtons.push(subButton);
            addChild(subButton);
         }
      }

      public function applyStats() : void
      {
         var spentPoints:int = 0;

         for (var i:int = 0; i < 4; i++)
            spentPoints += allocatedPoints[i];

         if (spentPoints > 0)
            this.gs_.gsc_.statsApply(allocatedPoints);
      }

      public function statsApplied(success:Boolean):void
      {
         toggleEdit(false);
         createFloatingText(success ? "Success" : "Error", success ? 0x00FF00 : 0xFF0000, 12,
                 applyButton.x + applyButton.width / 4, applyButton.y + applyButton.height / 2);
      }

      public function createFloatingText(text:String, color:uint, size:int, x:int, y:int):void {
         var floatText:SimpleText = createSimpleText(text, size, color);
         floatText.x = x; floatText.y = y;
         addChild(floatText);
         var tween:GTween = new GTween(floatText, 1.5, {"alpha": 0, "y": y - 40});
         tween.onComplete = this.onFloatComplete;
      }

      public function createSimpleText(text:String, size:int, color:uint):SimpleText {
         var simpleText:SimpleText = new SimpleText(size, color);
         simpleText.setAutoSize(TextFieldAutoSize.CENTER);
         simpleText.autoResize = true;
         simpleText.setText(text);
         return simpleText;
      }

      public function onFloatComplete(tween:GTween):void {
         removeChild(SimpleText(tween.target));
      }

      public function onRemovedFromStage(e:Event):void
      {
         gs_.statsView = null;
      }

      private var oldName:String;

      public function draw(player:Player) : void
      {
         if(player != null)
         {
            if (!editing && this.statPoints != player.statPoints_)
            {
               this.statPoints = player.statPoints_;
               updateStatPointsText();
            }
            this.statHeader.statViews[0].draw(player.attack + (editing ? allocatedPoints[0] : 0));
            this.statHeader.statViews[1].draw(player.defense + (editing ? allocatedPoints[1] : 0));
            this.statHeader.statViews[2].draw(player.dexterity + (editing ? allocatedPoints[2] : 0));
            this.statHeader.statViews[3].draw(player.wisdom + (editing ? allocatedPoints[3] : 0));
         }
      }
   }
}
