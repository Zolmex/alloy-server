package kabam.rotmg.game.view.components
{
import com.company.assembleegameclient.game.GameSprite;
import com.company.assembleegameclient.objects.Player;
import com.company.assembleegameclient.parameters.Parameters;
import com.company.assembleegameclient.screens.TitleMenuOption;
import com.company.assembleegameclient.ui.panels.itemgrids.InventoryGrid;
import com.company.util.KeyCodes;

import flash.display.Bitmap;
   import flash.display.Sprite;
import flash.events.MouseEvent;

import kabam.rotmg.assets.services.IconFactory;
   import kabam.rotmg.constants.GeneralConstants;
import kabam.rotmg.game.view.components.constellationstab.ConstellationsRightTab;
import kabam.rotmg.game.view.components.statsview.assets.StatAssetsManager;
import kabam.rotmg.game.view.components.statsview.right.StatsView;
import kabam.rotmg.ui.model.HUDModel;
   import kabam.rotmg.ui.model.TabStripModel;
import kabam.rotmg.ui.signals.StatsTabHotKeyInputSignal;
import kabam.rotmg.ui.signals.UpdateBackpackTabSignal;
   import kabam.rotmg.ui.signals.UpdateHUDSignal;
   import kabam.rotmg.ui.view.PotionInventoryView;
   import robotlegs.bender.bundles.mvcs.Mediator;
   
   public class TabStripMediator extends Mediator
   {
      [Inject]
      public var view:TabStripView;
      
      [Inject]
      public var hudModel:HUDModel;
      
      [Inject]
      public var tabStripModel:TabStripModel;
      
      [Inject]
      public var updateHUD:UpdateHUDSignal;
      
      [Inject]
      public var updateBackpack:UpdateBackpackTabSignal;
      
      [Inject]
      public var iconFactory:IconFactory;

      [Inject]
      public var statsTabHotKeyInput:StatsTabHotKeyInputSignal;
      
      public function TabStripMediator()
      {
         super();
      }
      
      override public function initialize() : void
      {
         this.view.tabSelected.add(this.onTabSelected);
         this.updateHUD.addOnce(this.addTabs);
         this.statsTabHotKeyInput.add(this.onTabHotkey);
      }
      
      override public function destroy() : void
      {
         this.view.tabSelected.remove(this.onTabSelected);
         this.updateBackpack.remove(this.onUpdateBackPack);
      }
      
      private function addTabs(player:Player) : void
      {
         this.addInventoryTab(player);
         this.addStatsTab(player.map_.gs_);
         this.addConstellationsTab(player);
         if(player.hasBackpack_)
         {
            this.addBackPackTab(player);
         }
         else
         {
            this.updateBackpack.add(this.onUpdateBackPack);
         }
      }
      
      private function onTabSelected(name:String) : void
      {
         this.tabStripModel.currentSelection = name;
      }
      
      private function onUpdateBackPack(hasBackpack:Boolean) : void
      {
         if(hasBackpack)
         {
            this.addBackPackTab(this.hudModel.gameSprite.map.player_);
            this.updateBackpack.remove(this.onUpdateBackPack);
         }
      }

      private function onTabHotkey():void
      {
         var index:int = (this.view.currentTabIndex + 1);
         index = (index % this.view.tabs.length);
         this.view.setSelectedTab(index);
      }
      
      private function addInventoryTab(player:Player) : void
      {
         var storageContent:Sprite = new Sprite();
         storageContent.name = TabStripModel.MAIN_INVENTORY;
         storageContent.x = 5;
         storageContent.y = 9;
         var storage:InventoryGrid = new InventoryGrid(player,player,4);
         this.view.inventoryGrid = storage;
         storage.y += 1;
         storageContent.addChild(storage);
         var potionsInventory:PotionInventoryView = new PotionInventoryView();
         potionsInventory.x = -1;
         potionsInventory.y = storage.height + 4;
         storageContent.addChild(potionsInventory);
         var icon:Bitmap = this.iconFactory.makeIconBitmap(24, 1.1);
         this.view.addTab(icon,storageContent);
      }
      
      private function addStatsTab(gs:GameSprite) : void
      {
         var stats:StatsView = null;
         stats = new StatsView(gs);
         gs.statsView = stats;
         stats.name = TabStripModel.STATS;
         stats.y = 0;
         var icon:Bitmap = this.iconFactory.makeIconBitmap(25, 1.1);
         this.view.addTab(icon,stats);
      }

      private function addConstellationsTab(player:Player):void
      {
         var stats:ConstellationsRightTab = null;
         stats = new ConstellationsRightTab(player);
         stats.name = TabStripModel.CONSTELLATIONS;
         stats.y = 0;
         var icon:Bitmap = this.iconFactory.makeIconBitmap(26, 1.1);
         this.view.addTab(icon,stats);
      }
      
      private function addBackPackTab(player:Player) : void
      {
         var backpackPotionsInventory:PotionInventoryView = null;
         var backpackContent:Sprite = new Sprite();
         backpackContent.name = TabStripModel.BACKPACK;
         backpackContent.x = 5;
         backpackContent.y = 9;
         var backpack:InventoryGrid = new InventoryGrid(player,player,GeneralConstants.NUM_EQUIPMENT_SLOTS + GeneralConstants.NUM_INVENTORY_SLOTS,true);
         backpack.y += 1;
         backpackContent.addChild(backpack);
         backpackPotionsInventory = new PotionInventoryView();
         backpackPotionsInventory.x = -1;
         backpackPotionsInventory.y = backpack.height + 4;
         backpackContent.addChild(backpackPotionsInventory);
         var icon:Bitmap = this.iconFactory.makeIconBitmap(27, 1.1);
         this.view.addTab(icon,backpackContent);
      }
   }
}
