package com.company.assembleegameclient.ui.panels.itemgrids.itemtiles {
import com.adobe.utils.ArrayUtil;
import com.company.assembleegameclient.itemData.ItemData;
import com.company.assembleegameclient.objects.ObjectLibrary;
import com.company.assembleegameclient.objects.Player;
import com.company.assembleegameclient.parameters.Parameters;
import com.company.assembleegameclient.ui.panels.itemgrids.ItemGrid;
import com.company.assembleegameclient.util.FilterUtil;
import com.company.assembleegameclient.util.TierUtil;
import com.company.assembleegameclient.util.UILabel;
import com.company.util.GraphicsUtil;
import com.company.util.MoreColorUtil;

import flash.display.Bitmap;

import flash.display.GraphicsPath;
import flash.display.GraphicsSolidFill;
import flash.display.IGraphicsData;
import flash.display.Shape;
import flash.display.Sprite;
import flash.geom.ColorTransform;

import io.decagames.rotmg.ui.texture.TextureParser;

import kabam.rotmg.constants.ItemConstants;

public class ItemTile extends Sprite {

    public static const TILE_DOUBLE_CLICK:String = "TILE_DOUBLE_CLICK";
    public static const TILE_SINGLE_CLICK:String = "TILE_SINGLE_CLICK";
    public static const WIDTH:int = 40;
    public static const HEIGHT:int = 40;
    public static const BORDER:int = 4;
    private static const RESTRICTED_COLOR:uint = 6036765;

    private var background:Bitmap;
    public var itemSprite:ItemTileSprite;
    public var tileId:int;
    public var ownerGrid:ItemGrid;
    private var tierText:UILabel;
    private var tagContainer:Sprite;
    private var bgCT:ColorTransform;

    public function ItemTile(id:int, parentGrid:ItemGrid) {
        super();
        this.tileId = id;
        this.ownerGrid = parentGrid;
        this.tagContainer = new Sprite();

        this.background = TextureParser.instance.getTexture("UI", "item_tile_no_cuts");
        this.background.x = -1;
        this.background.y = -1;
        this.background.visible = false;
        addChild(this.background);
        this.bgCT = this.background.transform.colorTransform;

        this.setItemSprite(new ItemTileSprite());
    }

    public function drawBackground(cuts:Array):void {
        this.background.visible = true;
        if (ArrayUtil.arraysAreEqual(cuts, ItemGrid.NO_CUT)){
            return;
        }

        try {
            this.background.bitmapData = TextureParser.instance.getTextureData("UI", "item_tile_cut_" + cuts[0] + cuts[1] + cuts[2] + cuts[3]);
        }
        catch (e:Error){
            trace("Item tile cut not supported. " + cuts[0] + ", " + cuts[1] + ", " + cuts[2] + ", " + cuts[3] + e.message)
        }
    }

    public function setItem(item:ItemData):Boolean {
        if (item == this.itemSprite.itemData) {
            //Need a way to tick this itemSprite and seems that setItem() gets called very frequently
            if(item != null && this.itemSprite.itemData.Animation != null) {
                this.itemSprite.setType(item);
            }

            return false;
        }
        this.itemSprite.setType(item);
        this.setTierTag();
        this.updateUseability(this.ownerGrid.curPlayer);
        return true;
    }

    public function setItemSprite(itemTileSprite:ItemTileSprite):void {
        this.itemSprite = itemTileSprite;
        this.itemSprite.x = WIDTH / 2;
        this.itemSprite.y = HEIGHT / 2;
        addChild(this.itemSprite);
        this.setTierTag();
    }

    public function updateUseability(player:Player):void {
        if (this.itemSprite.itemData != null) {
            if (!ObjectLibrary.isUsableByPlayer(this.itemSprite.itemData.ObjectType, player)) {
                var ct:ColorTransform = this.background.transform.colorTransform;
                ct.color = RESTRICTED_COLOR;
                ct.alphaMultiplier = 0.85;
                this.background.transform.colorTransform = ct;
            }
            else{
                this.background.transform.colorTransform = this.bgCT;
            }
        } else {
            this.background.transform.colorTransform = this.bgCT;
        }
    }

    public function canHoldItem(type:int):Boolean {
        return true;
    }

    public function resetItemPosition():void {
        this.setItemSprite(this.itemSprite);
    }

    public function getItemId():int {
        if (this.itemSprite.itemData == null)
            return -1;
        return this.itemSprite.itemData.ObjectType;
    }

    public function getItemData():ItemData {
        return this.itemSprite.itemData;
    }

    protected function getBackgroundColor():int {
        return 5526612;
    }

    public function setTierTag():void {
        this.clearTierTag();
        if (this.itemSprite.itemData) {
            this.tierText = TierUtil.getTierTag(this.itemSprite.itemData, 12);
            if (this.tierText) {
                if (contains(this.tagContainer)) {
                    removeChild(this.tagContainer);
                }
                addChild(this.tagContainer);
                this.tierText.filters = FilterUtil.getTextOutlineFilter();
                this.tierText.x = WIDTH - this.tierText.width;
                this.tierText.y = HEIGHT / 2 + 4;
                this.toggleTierTag(Parameters.data_.showTierTag);
                this.tagContainer.addChild(this.tierText);
            }
        }
    }

    private function clearTierTag():void {
        if (this.tierText && this.tagContainer && this.tagContainer.contains(this.tierText)) {
            this.tagContainer.removeChild(this.tierText);
            this.tierText = null;
        }
    }

    public function toggleTierTag(value:Boolean):void {
        if (this.tierText) {
            this.tierText.visible = value;
        }
    }

    protected function toggleDragState(value:Boolean):void {
        if (this.tierText && Parameters.data_.showTierTag) {
            this.tierText.visible = value;
        }
    }
}
}
