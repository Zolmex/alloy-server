package kabam.rotmg.game.view.components.statsview.left
{
import kabam.rotmg.game.view.components.*;

import com.company.assembleegameclient.appengine.SavedCharacter;
import com.company.assembleegameclient.game.GameSprite;
import com.company.assembleegameclient.objects.ObjectLibrary;
import com.company.assembleegameclient.objects.Player;
import com.company.assembleegameclient.parameters.Parameters;
import com.company.assembleegameclient.screens.TitleMenuOption;
import com.company.assembleegameclient.ui.tooltip.TooltipHelper;
import com.company.assembleegameclient.util.AnimatedChar;
import com.company.assembleegameclient.util.FilterUtil;
import com.company.assembleegameclient.util.TextureRedrawer;
import com.company.ui.SimpleText;

import flash.display.DisplayObject;

import flash.geom.Point;
import flash.geom.Rectangle;
import flash.display.Bitmap;
import flash.display.BitmapData;

import flash.display.Graphics;
   import flash.display.Sprite;
import flash.events.Event;
import flash.events.MouseEvent;
import flash.text.StyleSheet;
import flash.text.TextFieldAutoSize;

import kabam.rotmg.game.view.components.statsview.assets.StatAssetsManager;

import kabam.rotmg.game.view.components.statsview.left.StatHeader;

import kabam.rotmg.game.view.components.statsview.StatModel;
import kabam.rotmg.game.view.components.statsview.assets.StatAssetsSheet;
import kabam.rotmg.game.view.components.statsview.left.StatTabButton;
import kabam.rotmg.game.view.components.statsview.left.StatTabContentPortion;
import kabam.rotmg.game.view.components.statsview.StatView;

import org.osflash.signals.natives.NativeSignal;

import starling.display.Button;

public class AllStatsView extends Sprite
   {
      public var gs_:GameSprite;
      private var savedChar:SavedCharacter;
      private var currentTabIndex:int;

      public var titleText:SimpleText;
      public var nameText:SimpleText;
      public var levelText:SimpleText;
      public var locationText:SimpleText;

      public var tabs:Vector.<StatTabButton>;
      public var statTabContentPortion:StatTabContentPortion;

      public var characterIcon:Sprite;
      public var characterBitmap:Bitmap;

      public var allStatsSprite:Sprite;
      public var allStatsBG:Bitmap;

      public function AllStatsView(gs:GameSprite)
      {
         super();
         this.gs_ = gs;
         this.allStatsSprite = new Sprite();
         this.allStatsBG = StatAssetsManager.getInstance().getAssetByName("AllStatsBackground");
         this.allStatsSprite.addChild(allStatsBG);
         this.tabs = new Vector.<StatTabButton>();

         makeAssets();

         addEventListener(Event.ADDED_TO_STAGE,this.onAddedToStage);
         addEventListener(Event.REMOVED_FROM_STAGE,this.onRemovedFromStage);
      }

      public function makeAssets() : void
      {
         addChild(this.allStatsSprite);

         var player:Player = gs_.map.player_;
         this.savedChar = gs_.model.getCharacterById(gs_.model.currentCharId);

         this.titleText = new SimpleText(20, 0xb3b3b3);
         this.titleText.setText("Stats View");
         this.titleText.useTextDimensions();
         this.titleText.setBold(true);
         this.titleText.autoSize = TextFieldAutoSize.CENTER;
         this.titleText.filters = FilterUtil.getTextShadowFilter();
         this.titleText.x = this.allStatsSprite.width / 2 - this.titleText.width / 2;
         this.titleText.y = 18 - this.titleText.height / 2;
         this.allStatsSprite.addChild(titleText);

         this.nameText = new SimpleText(18, 0xb3b3b3);
         this.nameText.setText(gs_.map.player_.name_);
         this.nameText.useTextDimensions();
         this.nameText.setBold(true);
         this.nameText.autoSize = TextFieldAutoSize.CENTER;
         this.nameText.filters = FilterUtil.getTextShadowFilter();
         this.nameText.x = this.allStatsSprite.width / 2 - this.nameText.width / 2;
         this.nameText.y = 49 - this.nameText.height / 2;
         this.allStatsSprite.addChild(nameText);
         
         this.levelText = new SimpleText(13, 0xb3b3b3);
         var levelHtml:String = "Level " + TooltipHelper.wrapInBold(player.level_ + " ") + savedChar.displayId();
         this.levelText.htmlText = levelHtml;
         this.levelText.useTextDimensions();
         this.levelText.autoSize = TextFieldAutoSize.LEFT;
         this.levelText.filters = FilterUtil.getTextShadowFilter();
         this.levelText.x = 60;
         this.levelText.y = 64;
         this.allStatsSprite.addChild(levelText);
         
         this.locationText = new SimpleText(13, 0xb3b3b3);
         this.locationText.setText(gs_.map.name_)
         this.locationText.useTextDimensions();
         this.locationText.autoSize = TextFieldAutoSize.LEFT;
         this.locationText.filters = FilterUtil.getTextShadowFilter();
         this.locationText.x = 60;
         this.locationText.y = 80;
         this.allStatsSprite.addChild(locationText);

         this.characterIcon = new Sprite();
         this.characterBitmap = new Bitmap(null);

         var playerXML:XML = ObjectLibrary.playerChars_[getCharIndex(savedChar.objectType())];

         var tempBitmapData:BitmapData = SavedCharacter.getImage(savedChar,playerXML,AnimatedChar.DOWN,AnimatedChar.STAND,0,false, false, 120, true);
         var croppedData:BitmapData = new BitmapData(64, 42);
         croppedData.copyPixels(tempBitmapData, new Rectangle(0, 0, 64, 48), new Point(0, 0));
         this.characterBitmap.bitmapData = croppedData;
         tempBitmapData.dispose();

         this.characterIcon.x = -7;
         this.characterIcon.y = 53;
         this.characterIcon.addChild(characterBitmap);
         this.allStatsSprite.addChild(characterIcon);

         for (var i:int = 0; i < 3; i++)
         {
            var tab:StatTabButton = new StatTabButton(this, i);
            this.tabs.push(tab);
            tab.y = 120;
            tab.x = 2 + (62 * i) + (i == 2 ? 1 : 0); //1 off pixel perfect GRAHHHHHHHHHHHHHHHHHHHH nobody will notioc.
            addChild(tab);
         }
         var startingTab:int = Parameters.data_.statsViewIndex;
         sortAndActivateTabs(tabs[startingTab]);
         showTabContents(startingTab);
      }

      public function getCharIndex(type:int) : int
      {
         var i:int = 0;
         for each(var plrXML:XML in ObjectLibrary.playerChars_) {
            if ((int)(plrXML.@type) == type)
                    return i;
            i++;
         }
         return 0;
      }

      public function draw(player:Player) : void
      {
         if(player != null)
         {
            var levelHtml:String = "Level " + TooltipHelper.wrapInBold(player.level_ + " ") + savedChar.displayId();
            this.levelText.htmlText = levelHtml;
            this.levelText.updateMetrics();

            this.statTabContentPortion.draw(player);
         }
      }

      public function tabClicked(clickedTab:StatTabButton) : void
      {
         sortAndActivateTabs(clickedTab);
         showTabContents(clickedTab.index);
      }

      public function sortAndActivateTabs(currentTab:StatTabButton) : void //input tab is the current tab
      {
         for each (var tab:StatTabButton in tabs)
            removeChild(tab);

         for (var i:int = 0; i < 3; i++) {
            var index:int = i;
            index = currentTab.index == 0 ? 2 - i : currentTab.index == 2 ? i : index;
            if (index == currentTab.index)
               continue;
            addChild(tabs[index]);
            tabs[index].activate();
         }

         addChild(currentTab);
         this.currentTabIndex = currentTab.index;
         currentTab.deactivate();
      }

      public function showTabContents(index:int) : void
      {
         if (statTabContentPortion && statTabContentPortion.parent) removeChild(statTabContentPortion);
         this.statTabContentPortion = null;

         this.statTabContentPortion = new StatTabContentPortion(this, index);
         this.statTabContentPortion.x = 9;
         this.statTabContentPortion.y = 140;
         addChild(this.statTabContentPortion);
      }

      private function onAddedToStage(event:Event) : void
      {
         Parameters.data_.statsViewOpen = true;
         this.gs_.allStatsView = this;
      }

      private function onRemovedFromStage(event:Event) : void
      {
         Parameters.data_.statsViewIndex = this.currentTabIndex;
         Parameters.data_.statsViewOpen = false;
         this.gs_.allStatsView = null;
      }
   }
}
