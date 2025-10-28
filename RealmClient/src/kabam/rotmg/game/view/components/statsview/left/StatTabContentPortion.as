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
import kabam.rotmg.messaging.impl.data.StatData;

import org.osflash.signals.natives.NativeSignal;

import starling.display.Button;

public class StatTabContentPortion extends Sprite
{
    private var view:AllStatsView;
    public var tabIndex_:int;

    public var statHeaders:Vector.<StatHeader>;

    public function StatTabContentPortion(view_:AllStatsView, tabIndex:int)
    {
        super();
        this.statHeaders = new Vector.<StatHeader>();
        this.view = view_;
        this.tabIndex_ = tabIndex;

        var lastHeader:StatHeader = null;

        for (var i:int = 0; i < getModel().length; i++)
        {
            var statModel:Array = getModel()[i];
            var header:String = getHeader()[i];
            var statHeader:StatHeader = new StatHeader(header, statModel);
            this.statHeaders.push(statHeader);

            if (lastHeader)
                statHeader.y = lastHeader.y + lastHeader.height + 3;

            lastHeader = statHeader;
            addChild(statHeader);
        }
    }

    public function draw(player:Player) : void
    {
        for (var i:int = 0; i < statHeaders.length; i++)
            for each (var statView:StatView in statHeaders[i].statViews)
                statView.draw(StatModels.getStatValue(statView, player));
    }

    public function getModel() : Array
    {
        return tabIndex_ == 0 ? StatModels.offenseStatModels : tabIndex_ == 1 ? StatModels.defenseStatModels : tabIndex_ == 2 ? StatModels.miscStatModels : StatModels.miscStatModels;
    }
    public function getHeader() : Array
    {
        return tabIndex_ == 0 ? StatModels.offenseStatHeaders : tabIndex_ == 1 ? StatModels.defenseStatHeaders : tabIndex_ == 2 ? StatModels.miscStatHeaders : StatModels.miscStatHeaders;
    }
}
}
