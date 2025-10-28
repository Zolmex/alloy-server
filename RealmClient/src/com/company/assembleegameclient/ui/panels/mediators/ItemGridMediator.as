package com.company.assembleegameclient.ui.panels.mediators {
import com.company.assembleegameclient.itemData.ItemData;
import com.company.assembleegameclient.map.Map;
import com.company.assembleegameclient.objects.Container;
import com.company.assembleegameclient.objects.GameObject;
import com.company.assembleegameclient.objects.ObjectLibrary;
import com.company.assembleegameclient.objects.OneWayContainer;
import com.company.assembleegameclient.objects.Player;
import com.company.assembleegameclient.parameters.Parameters;
import com.company.assembleegameclient.ui.Slot;
import com.company.assembleegameclient.ui.panels.itemgrids.ContainerGrid;
import com.company.assembleegameclient.ui.panels.itemgrids.InventoryGrid;
import com.company.assembleegameclient.ui.panels.itemgrids.ItemGrid;
import com.company.assembleegameclient.ui.panels.itemgrids.itemtiles.InteractiveItemTile;
import com.company.assembleegameclient.ui.panels.itemgrids.itemtiles.ItemTile;
import com.company.assembleegameclient.ui.panels.itemgrids.itemtiles.ItemTileEvent;
import com.company.assembleegameclient.ui.tooltip.EquipmentToolTip;
import com.company.assembleegameclient.ui.tooltip.ToolTip;
import com.company.assembleegameclient.util.DisplayHierarchy;

import flash.utils.getTimer;

import kabam.rotmg.constants.ItemConstants;
import kabam.rotmg.core.model.MapModel;
import kabam.rotmg.core.model.PlayerModel;
import kabam.rotmg.game.model.PotionInventoryModel;
import kabam.rotmg.game.view.components.TabStripView;
import kabam.rotmg.messaging.impl.GameServerConnection;
import kabam.rotmg.ui.model.HUDModel;
import kabam.rotmg.ui.model.TabStripModel;

import robotlegs.bender.bundles.mvcs.Mediator;

public class ItemGridMediator extends Mediator {


    [Inject]
    public var view:ItemGrid;

    [Inject]
    public var mapModel:MapModel;

    [Inject]
    public var playerModel:PlayerModel;

    [Inject]
    public var potionInventoryModel:PotionInventoryModel;

    [Inject]
    public var hudModel:HUDModel;

    [Inject]
    public var tabStripModel:TabStripModel;

    public function ItemGridMediator() {
        super();
    }

    override public function initialize():void {
        this.view.addEventListener(ItemTileEvent.ITEM_MOVE, this.onTileMove);
        this.view.addEventListener(ItemTileEvent.ITEM_SHIFT_CLICK, this.onShiftClick);
        this.view.addEventListener(ItemTileEvent.ITEM_DOUBLE_CLICK, this.onDoubleClick);
        this.view.addEventListener(ItemTileEvent.ITEM_CTRL_CLICK, this.onCtrlClick);
    }

    override public function destroy():void {
        if (this.view.staticToolTip){
            (this.view.staticToolTip as EquipmentToolTip).clearGemstoneUI();
            this.view.staticToolTip = null;
            this.view.staticToolTipTile = null;
        }
        super.destroy();
    }

    private function onTileMove(e:ItemTileEvent):void {
        var targetTile:InteractiveItemTile = null;
        var tsv:TabStripView = null;
        var slot:int = 0;
        var sourceTile:InteractiveItemTile = e.tile;
        if (sourceTile == this.view.staticToolTipTile){
            return;
        }

        var scaleW:Number = WebMain.sWidth / 800;
        var scaleH:Number = WebMain.sHeight / 600;
        var target:* = DisplayHierarchy.getParentWithTypeArray(sourceTile.getDropTarget(), TabStripView, InteractiveItemTile, Map, Slot);
        if (target is InteractiveItemTile) {
            targetTile = target as InteractiveItemTile;
            if (this.canSwapItems(sourceTile, targetTile)) {
                this.swapItemTiles(sourceTile, targetTile);
            }
        } else if (target is Slot) {
            var targetSlot:Slot = target as Slot;
            if (this.canSwapItemToSlot(sourceTile, targetSlot)) {
                this.swapItemToSlot(sourceTile, targetSlot);
            }
        } else if (target is Map || WebMain.STAGE.mouseX < WebMain.sWidth - (200 * (scaleW / (scaleW / scaleH)))) {
            this.dropItem(sourceTile);
        } else if (target is TabStripView) {
            tsv = target as TabStripView;
            slot = sourceTile.ownerGrid.curPlayer.nextAvailableInventorySlot();
            if (slot != -1) {
                GameServerConnection.instance.invSwap(this.view.curPlayer, sourceTile.ownerGrid.owner, sourceTile.tileId, this.view.curPlayer, slot);
                sourceTile.setItem(null);
                sourceTile.updateUseability(this.view.curPlayer);
            }
        }
        sourceTile.resetItemPosition();
    }

    private function canSwapItems(sourceTile:InteractiveItemTile, targetTile:InteractiveItemTile):Boolean {
        if (sourceTile.ownerGrid.staticToolTipTile == sourceTile || targetTile.ownerGrid.staticToolTipTile == targetTile){
            return false;
        }
        if (!sourceTile.canHoldItem(targetTile.getItemId())) {
            return false;
        }
        if (!targetTile.canHoldItem(sourceTile.getItemId())) {
            return false;
        }
        if (ItemGrid(targetTile.parent).owner is OneWayContainer) {
            return false;
        }
        return true;
    }

    private function canSwapItemToSlot(sourceTile:InteractiveItemTile, targetSlot:Slot):Boolean {
        if (sourceTile.ownerGrid.staticToolTipTile == sourceTile) {
            return false;
        }
        if (!sourceTile.canHoldItem(targetSlot.getItemId())) {
            return false;
        }
        if (!targetSlot.canHoldItem(sourceTile.getItemId())) {
            return false;
        }
        return true;
    }

    private function dropItem(itemTile:InteractiveItemTile):void {
        if (itemTile == this.view.staticToolTipTile){
            return;
        }
        var groundContainer:Container = null;
        var equipment:Vector.<ItemData> = null;
        var equipCount:int = 0;
        var openIndex:int = 0;
        if (this.view.owner == this.view.curPlayer) {
            groundContainer = this.mapModel.currentInteractiveTarget as Container;
            if (groundContainer) {
                equipment = groundContainer.equipment_;
                equipCount = equipment.length;
                for (openIndex = 0; openIndex < equipCount; openIndex++) {
                    if (equipment[openIndex] == null) {
                        break;
                    }
                }
                if (openIndex < equipCount) {
                    this.dropWithoutDestTile(itemTile, groundContainer, openIndex);
                } else {
                    GameServerConnection.instance.invDrop(this.view.owner, itemTile.tileId);
                }
            } else {
                GameServerConnection.instance.invDrop(this.view.owner, itemTile.tileId);
            }
        }
        itemTile.setItem(null);
    }

    private function swapItemTiles(sourceTile:ItemTile, destTile:ItemTile):Boolean {
        if (!GameServerConnection.instance || !this.view.interactive || !sourceTile || !destTile) {
            return false;
        }

        if (sourceTile.ownerGrid.staticToolTipTile == sourceTile || destTile.ownerGrid.staticToolTipTile == destTile){
            return false;
        }

        GameServerConnection.instance.invSwap(this.view.curPlayer, this.view.owner, sourceTile.tileId, destTile.ownerGrid.owner, destTile.tileId);
        var tempItem:ItemData = sourceTile.getItemData();
        sourceTile.setItem(destTile.getItemData());
        destTile.setItem(tempItem);
        sourceTile.updateUseability(this.view.curPlayer);
        destTile.updateUseability(this.view.curPlayer);
        return true;
    }

    private function swapItemToSlot(sourceTile:ItemTile, destSlot:Slot):Boolean {
        if (!this.view.interactive || !sourceTile || !destSlot) {
            return false;
        }
        if (sourceTile.ownerGrid.staticToolTipTile == sourceTile){
            return false;
        }

        var tempItem:ItemData = sourceTile.getItemData();
        var oldItem:ItemData = destSlot.getItemData();
        var oldItemType:int = destSlot.getItemId();
        sourceTile.setItem(oldItem);
        destSlot.setItem(tempItem, sourceTile.tileId);
        sourceTile.updateUseability(this.view.curPlayer);

        var player:Player = this.view.curPlayer;
        player.equipment_[sourceTile.tileId] = oldItem;
        player.itemTypes[sourceTile.tileId] = oldItemType;

        return true;
    }

    private function dropWithoutDestTile(sourceTile:ItemTile, container:Container, containerIndex:int):void {
        if (!GameServerConnection.instance || !this.view.interactive || !sourceTile || !container || sourceTile.ownerGrid.staticToolTipTile == sourceTile) {
            return;
        }
        GameServerConnection.instance.invSwap(this.view.curPlayer, this.view.owner, sourceTile.tileId, container, containerIndex);
        sourceTile.setItem(null);
    }

    private function onShiftClick(e:ItemTileEvent):void {
        var tile:InteractiveItemTile = e.tile;
        if (tile.ownerGrid.staticToolTipTile == tile){
            return;
        }
        if (tile.ownerGrid is InventoryGrid || tile.ownerGrid is ContainerGrid) {
            GameServerConnection.instance.useItem_new(tile.ownerGrid.owner, tile.tileId, getTimer());
        }
    }

    private function onCtrlClick(e:ItemTileEvent):void {
        var tile:InteractiveItemTile = e.tile;
        if (tile.ownerGrid.staticToolTipTile == tile){
            return;
        }

        var slot:int = 0;
        if (tile.ownerGrid is InventoryGrid) {
            slot = tile.ownerGrid.curPlayer.swapInventoryIndex(this.tabStripModel.currentSelection);
            if (slot != -1) {
                GameServerConnection.instance.invSwap(this.view.curPlayer, tile.ownerGrid.owner, tile.tileId, this.view.curPlayer, slot);
                tile.setItem(null);
                tile.updateUseability(this.view.curPlayer);
            }
        }
    }

    private function onDoubleClick(e:ItemTileEvent):void {
        var tile:InteractiveItemTile = e.tile;
        if (tile.ownerGrid.staticToolTipTile == tile){
            return;
        }
        if (tile.ownerGrid is ContainerGrid) {
            this.equipOrUseContainer(tile);
        } else {
            this.equipOrUseInventory(tile);
        }
        this.view.refreshTooltip();
    }

    /*private function isStackablePotion(tile:InteractiveItemTile) : Boolean
    {
       return tile.getItemId() == PotionInventoryModel.HEALTH_POTION_ID || tile.getItemId() == PotionInventoryModel.MAGIC_POTION_ID;
    }*/

    private function pickUpItem(tile:InteractiveItemTile):void {
        var nextAvailable:int = this.view.curPlayer.nextAvailableInventorySlot();
        if (nextAvailable != -1) {
            GameServerConnection.instance.invSwap(this.view.curPlayer, this.view.owner, tile.tileId, this.view.curPlayer, nextAvailable);
        }
    }

    private function equipOrUseContainer(tile:InteractiveItemTile):void {
        if (tile.ownerGrid.staticToolTipTile == tile){
            return;
        }
        var tileOwner:GameObject = tile.ownerGrid.owner;
        var player:Player = this.view.curPlayer;
        var nextAvailableSlotIndex:int = this.view.curPlayer.nextAvailableInventorySlot();
        if (nextAvailableSlotIndex != -1) {
            GameServerConnection.instance.invSwap(player, this.view.owner, tile.tileId, this.view.curPlayer, nextAvailableSlotIndex);
        } else {
            GameServerConnection.instance.useItem_new(tileOwner, tile.tileId, getTimer());
        }
    }

    private function equipOrUseInventory(tile:InteractiveItemTile):void {
        if (tile.ownerGrid.staticToolTipTile == tile){
            return;
        }
        var tileOwner:GameObject = tile.ownerGrid.owner;
        var player:Player = this.view.curPlayer;
        var matchingSlotIndex:int = ObjectLibrary.getMatchingSlotIndex(tile.getItemData(), player);
        if (matchingSlotIndex != -1) {
            GameServerConnection.instance.invSwap(player, tileOwner, tile.tileId, player, matchingSlotIndex);
        } else {
            GameServerConnection.instance.useItem_new(tileOwner, tile.tileId, getTimer());
        }
    }
}
}
