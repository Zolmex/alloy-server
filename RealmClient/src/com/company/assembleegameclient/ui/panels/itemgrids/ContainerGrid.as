package com.company.assembleegameclient.ui.panels.itemgrids {
import com.company.assembleegameclient.itemData.ItemData;
import com.company.assembleegameclient.objects.GameObject;
import com.company.assembleegameclient.objects.Player;
import com.company.assembleegameclient.parameters.Parameters;
import com.company.assembleegameclient.ui.panels.itemgrids.itemtiles.InteractiveItemTile;
import com.company.assembleegameclient.ui.panels.itemgrids.itemtiles.ItemTile;
import com.company.util.GraphicsUtil;

import flash.display.Graphics;

import flash.display.Shape;

public class ContainerGrid extends ItemGrid {


    private const NUM_SLOTS:uint = 8;

    private var tiles:Vector.<InteractiveItemTile>;

    public function ContainerGrid(gridOwner:GameObject, currentPlayer:Player) {
        var tile:InteractiveItemTile = null;
        super(gridOwner, currentPlayer, 0);

        var background:Shape = new Shape();
        var g:Graphics = background.graphics;
        g.beginFill(0x16151e);
        GraphicsUtil.drawCutEdgeRectGraphics(-4, -4, 183, 93, 4, [1, 1, 1, 1], g);
        g.endFill();
        addChild(background);

        this.tiles = new Vector.<InteractiveItemTile>(this.NUM_SLOTS);
        for (var i:int = 0; i < this.NUM_SLOTS; i++) {
            tile = new InteractiveItemTile(i + indexOffset, this, interactive);
            addToGrid(tile, 2, i);
            this.tiles[i] = tile;
        }
    }

    override public function setItems(items:Vector.<ItemData>, itemIndexOffset:int = 0):void {
        var numItems:int = 0;
        var i:int = 0;
        var refresh:Boolean = false;
        if (items) {
            numItems = items.length;
            for (i = 0; i < this.NUM_SLOTS; i++) {
                if (i + indexOffset < numItems) {
                    if (this.tiles[i].setItem(items[i + indexOffset])) {
                        refresh = true;
                    }
                } else {
                    if (this.tiles[i].setItem(null)) {
                        refresh = true;
                    }
                }
            }
            if (refresh) {
                refreshTooltip();
            }
        }
    }

    public function toggleTierTag():void {
        for each (var tile:ItemTile in this.tiles){
            tile.toggleTierTag(Parameters.data_.showTierTag);
        }
    }
}
}
