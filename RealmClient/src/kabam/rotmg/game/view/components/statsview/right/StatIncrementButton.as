package kabam.rotmg.game.view.components.statsview.right
{
import com.company.assembleegameclient.game.MapUserInput;
import com.company.util.KeyCodes;

import kabam.rotmg.game.view.components.statsview.left.*;
import kabam.rotmg.game.view.components.statsview.*;

import flash.geom.ColorTransform;
import flash.text.engine.TabAlignment;

import kabam.rotmg.game.view.components.statsview.assets.*;
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

import kabam.rotmg.game.view.components.statsview.StatModel;
import kabam.rotmg.game.view.components.statsview.assets.StatAssetsSheet;

import org.osflash.signals.natives.NativeSignal;

import starling.display.Button;

public class StatIncrementButton extends Sprite
{
    private var view:StatsView;
    public var index:int;
    public var activated:Boolean;
    public var add:Boolean;

    private var btnSprite:Bitmap;
    private var btnSpriteDark:Bitmap;
    private var btnSpriteGold:Bitmap;
    private var btnSpriteDarkGold:Bitmap;

    public function StatIncrementButton(view_:StatsView, index_:int, add:Boolean) {
        super();
        var assetManager:StatAssetsManager = StatAssetsManager.getInstance()
        this.add = add;
        this.btnSprite = add ? assetManager.getAssetByName("StatsAddButton") : assetManager.getAssetByName("StatsSubtractButton");
        this.btnSpriteDark = add ? assetManager.getAssetByName("StatsAddButtonDark") : assetManager.getAssetByName("StatsSubtractButtonDark");
        this.btnSpriteGold = add ? assetManager.getAssetByName("StatsAddButtonGold") : assetManager.getAssetByName("StatsSubtractButtonGold");
        this.btnSpriteDarkGold = add ? assetManager.getAssetByName("StatsAddButtonDarkGold") : assetManager.getAssetByName("StatsSubtractButtonDarkGold");
        this.view = view_;
        this.index = index_;
        addChild(btnSprite);
        addChild(btnSpriteDark);
        addChild(btnSpriteGold);
        addChild(btnSpriteDarkGold);
    }

    public function reload(points:int = 0) : void {
        if (points >= (view.gs_.mui_.isKeyDown[MapUserInput.SHIFT_KEY] ? 5 : 1))
            activate();
        else if (activated)
            deactivate();

        toggleSprite();
    }

    public function activate() : void {
        addEventListener(MouseEvent.MOUSE_OVER,this.onMouseOver);
        addEventListener(MouseEvent.MOUSE_OUT,this.onMouseOut);
        addEventListener(MouseEvent.CLICK,this.onMouseClick);
        activated = true;
        mouseEnabled = true;
        mouseChildren = true;
        toggleSprite();
    }

    public function deactivate() : void {
        if (!activated)
            return;
        transform.colorTransform = new ColorTransform(1,1,1);
        removeEventListener(MouseEvent.MOUSE_OVER,this.onMouseOver);
        removeEventListener(MouseEvent.MOUSE_OUT,this.onMouseOut);
        removeEventListener(MouseEvent.CLICK,this.onMouseClick);
        activated = false;
        mouseEnabled = false;
        mouseChildren = false;
        toggleSprite();
    }

    protected function onMouseOver(event:MouseEvent) : void {
        transform.colorTransform = new ColorTransform(1.2,1.2,1.2);
    }
    protected function onMouseOut(event:MouseEvent) : void {
        transform.colorTransform = new ColorTransform(1,1,1);
    }
    protected function onMouseClick(event:MouseEvent) : void {
        if (activated) this.view.incButtonClicked(this);
    }
    public function toggleSprite() : void {
        var shift:Boolean = view.gs_.mui_.isKeyDown[MapUserInput.SHIFT_KEY];
        if (shift) {
            this.btnSpriteGold.visible = activated;
            this.btnSpriteDarkGold.visible = !activated;

            this.btnSprite.visible = false;
            this.btnSpriteDark.visible = false;
        } else
        {
            this.btnSprite.visible = activated;
            this.btnSpriteDark.visible = !activated;

            this.btnSpriteGold.visible = false;
            this.btnSpriteDarkGold.visible = false;
        }
    }
}
}
