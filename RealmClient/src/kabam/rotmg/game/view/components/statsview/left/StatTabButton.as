package kabam.rotmg.game.view.components.statsview.left
{
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

public class StatTabButton extends Sprite //note: deactivated tab is the current tab
{
    private var view:AllStatsView;
    public var index:int;
    public var activated:Boolean;

    public var titleText:SimpleText;

    private var tabSprite:Bitmap;
    private var tabSpriteDark:Bitmap;

    public function StatTabButton(view_:AllStatsView, index_:int)
    {
        super();
        var assetManager:StatAssetsManager = StatAssetsManager.getInstance()
        this.tabSprite = assetManager.getAssetByName("AllStatsTab");
        this.tabSpriteDark = assetManager.getAssetByName("AllStatsTabDark");
        this.view = view_;
        this.index = index_;

        addChild(tabSprite);
        addChild(tabSpriteDark);
    }

    public function makeTitle(dark:Boolean) : void
    {
        if (titleText && titleText.parent != null) removeChild(titleText);

        this.titleText = new SimpleText(12, dark ? 0x676767 : 0xb3b3b3);
        this.titleText.setText(getName());
        this.titleText.useTextDimensions();
        this.titleText.autoSize = TextFieldAutoSize.CENTER;
        this.titleText.filters = FilterUtil.getTextShadowFilter();
        this.titleText.x = this.tabSprite.x + this.tabSprite.width / 2 - this.titleText.width / 2;
        this.titleText.y = this.tabSprite.y;
        addChild(titleText);
    }

    public function getName() : String
    {
        return index == 0 ? "Offense" : index == 1 ? "Defense" : index == 2 ? "Misc" : "lalala";
    }

    public function activate() : void
    {
        addEventListener(MouseEvent.MOUSE_OVER,this.onMouseOver);
        addEventListener(MouseEvent.MOUSE_OUT,this.onMouseOut);
        addEventListener(MouseEvent.CLICK,this.onMouseClick);
        activated = true;
        mouseEnabled = true;
        mouseChildren = true;
        makeTitle(true);
        toggleSprite();
    }

    public function deactivate() : void
    {
        removeEventListener(MouseEvent.MOUSE_OVER,this.onMouseOver);
        removeEventListener(MouseEvent.MOUSE_OUT,this.onMouseOut);
        removeEventListener(MouseEvent.CLICK,this.onMouseClick);
        activated = false;
        mouseEnabled = false;
        mouseChildren = false;
        makeTitle(false);
        toggleSprite();
    }

    protected function onMouseOver(event:MouseEvent) : void
    {
        transform.colorTransform = new ColorTransform(1.2,1.2,1.2);
    }
    protected function onMouseOut(event:MouseEvent) : void
    {
        transform.colorTransform = new ColorTransform(1,1,1);
    }
    protected function onMouseClick(event:MouseEvent) : void
    {
        if (activated) this.view.tabClicked(this);
    }
    public function toggleSprite() : void
    {
        this.tabSprite.visible = !activated;
        this.tabSpriteDark.visible = activated;
    }
}
}
