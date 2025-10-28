package com.company.assembleegameclient.ui.panels.itemgrids {
import com.company.assembleegameclient.constants.InventoryOwnerTypes;
import com.company.assembleegameclient.itemData.ItemData;
import com.company.assembleegameclient.objects.Container;
import com.company.assembleegameclient.objects.GameObject;
import com.company.assembleegameclient.objects.ObjectLibrary;
import com.company.assembleegameclient.objects.Player;
import com.company.assembleegameclient.ui.panels.Panel;
import com.company.assembleegameclient.ui.panels.itemgrids.itemtiles.EquipmentTile;
import com.company.assembleegameclient.ui.panels.itemgrids.itemtiles.InteractiveItemTile;
import com.company.assembleegameclient.ui.panels.itemgrids.itemtiles.ItemTile;
import com.company.assembleegameclient.ui.tooltip.EquipmentToolTip;
import com.company.assembleegameclient.ui.tooltip.TextToolTip;
import com.company.assembleegameclient.ui.tooltip.ToolTip;

import flash.events.MouseEvent;

import kabam.rotmg.constants.ItemConstants;

import org.osflash.signals.Signal;

public class ItemGrid extends Panel {

    public static const NO_CUT:Array = [0, 0, 0, 0];

    public static const CutsByNum:Object = {
        1: [[1, 0, 0, 1], NO_CUT, NO_CUT, [0, 1, 1, 0]],
        2: [[1, 0, 0, 0], NO_CUT, NO_CUT, [0, 1, 0, 0], [0, 0, 0, 1], NO_CUT, NO_CUT, [0, 0, 1, 0]],
        3: [[1, 0, 0, 1], NO_CUT, NO_CUT, [0, 1, 1, 0], [1, 0, 0, 0], NO_CUT, NO_CUT, [0, 1, 0, 0], [0, 0, 0, 1], NO_CUT, NO_CUT, [0, 0, 1, 0]]
    };


    private const padding:uint = 5;
    private const rowLength:uint = 4;

    public var owner:GameObject;

    public var staticToolTipTile:ItemTile;
    public var staticToolTip:ToolTip;
    public var tooltip:ToolTip;
    private var tooltipFocusTile:ItemTile;

    public var curPlayer:Player;

    protected var indexOffset:int;

    public var interactive:Boolean;

    public function ItemGrid(gridOwner:GameObject, currentPlayer:Player, itemIndexOffset:int) {
        super(gridOwner ? gridOwner.map_.gs_ : null);
        this.owner = gridOwner;
        this.curPlayer = currentPlayer;
        this.indexOffset = itemIndexOffset;
        var container:Container = gridOwner as Container;
        if (gridOwner == currentPlayer || container) {
            this.interactive = true;
        }
    }

    public function hideTooltip():void {
        if (this.tooltip) {
            this.tooltip.detachFromTarget();
            this.tooltip = null;
            this.tooltipFocusTile = null;
        }
    }

    public function refreshTooltip():void {
        if (!stage || !this.tooltip || !this.tooltip.stage) {
            return;
        }
        if (this.tooltipFocusTile) {
            this.tooltip.detachFromTarget();
            this.tooltip = null;
            this.addToolTipToTile(this.tooltipFocusTile);
        }
    }

    private function onTileHover(e:MouseEvent):void {
        if (!stage) {
            return;
        }
        var tile:ItemTile = e.currentTarget as ItemTile;
        this.addToolTipToTile(tile);
        this.tooltipFocusTile = tile;

        var itemData:ItemData = tile.getItemData();
        if (tile is EquipmentTile && itemData != null && ObjectLibrary.xmlLibrary_[itemData.ObjectType].hasOwnProperty("ShowOverlay")) {
            this.gs_.map.mapOverlay_.abilityOverlay.visible = true;
        }
    }

    private function onTileOut(e:MouseEvent):void {
        if (!stage) {
            return;
        }

        this.gs_.map.mapOverlay_.abilityOverlay.visible = false;
    }

    public function showGemstonesUI(clear:Boolean = false):void {
        if (this.staticToolTip || clear){
            if (this.staticToolTip) {
                (this.staticToolTipTile as InteractiveItemTile).blockInteraction = false;
                (this.staticToolTip as EquipmentToolTip).setGemstonesUI();
            }
            this.staticToolTip = null;
            this.staticToolTipTile = null;
            return;
        }

        if (!this.tooltip || this.tooltipFocusTile.itemSprite.itemData.GemstoneLimit < 1){
            return;
        }

        (this.tooltip as EquipmentToolTip).setGemstonesUI();
        this.staticToolTip = this.tooltip;
        this.staticToolTipTile = this.tooltipFocusTile;
        (this.staticToolTipTile as InteractiveItemTile).blockInteraction = true;
    }

    private function addToolTipToTile(tile:ItemTile):void {
        if (this.staticToolTipTile == tile){ // Don't add tooltip for an item we're already showing statically
            return;
        }

        var itemName:String = null;
        if (tile.itemSprite.itemData != null) {
            this.tooltip = new EquipmentToolTip(tile.itemSprite.itemData, this.curPlayer);
        } else {
            if (tile is EquipmentTile) {
                itemName = ItemConstants.itemTypeToName((tile as EquipmentTile).itemType);
            } else {
                itemName = "item";
            }
            this.tooltip = new TextToolTip(3552822, 10197915, null, "Empty " + itemName + " Slot", 200);
        }
        this.tooltip.attachToTarget(tile);
        stage.addChild(this.tooltip);
    }

    private function getCharacterType():String {
        if (this.owner == this.curPlayer) {
            return InventoryOwnerTypes.CURRENT_PLAYER;
        }
        if (this.owner is Player) {
            return InventoryOwnerTypes.OTHER_PLAYER;
        }
        return InventoryOwnerTypes.NPC;
    }

    protected function addToGrid(tile:ItemTile, numRows:uint, tileIndex:uint, equip:Boolean = false):void {
        if (!equip) { // Equip tiles don't use the same background as normal tiles
            tile.drawBackground(CutsByNum[numRows][tileIndex]);
        }
        tile.addEventListener(MouseEvent.ROLL_OVER, this.onTileHover);
        tile.addEventListener(MouseEvent.ROLL_OUT, this.onTileOut);
        tile.x = int(tileIndex % this.rowLength) * (ItemTile.WIDTH + (equip ? 4 : this.padding));
        tile.y = int(tileIndex / this.rowLength) * (ItemTile.HEIGHT + (equip ? 4 : this.padding));
        addChild(tile);
    }

    public function setItems(items:Vector.<ItemData>, itemIndexOffset:int = 0):void {

    }

    public function enableInteraction(enabled:Boolean):void {
        mouseEnabled = enabled;
    }

    override public function draw():void {
        this.setItems(this.owner.equipment_, this.indexOffset);
    }
}
}
