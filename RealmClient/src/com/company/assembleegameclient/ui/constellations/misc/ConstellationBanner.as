package com.company.assembleegameclient.ui.constellations.misc
{
import com.company.assembleegameclient.ui.reusablebg.BGWithOverlay;
import com.company.assembleegameclient.ui.reusablebg.TwoButtonUI;
import com.company.assembleegameclient.ui.constellations.misc.ConstellationNode;
import com.company.assembleegameclient.ui.constellations.misc.images.TempConstellationBanner;
import com.company.assembleegameclient.ui.constellations.xml.ConstellationsDataStore;
import com.company.assembleegameclient.ui.guild.*;
import com.company.assembleegameclient.game.GameSprite;
import com.company.assembleegameclient.game.events.GuildResultEvent;
import com.company.assembleegameclient.objects.Player;
import com.company.assembleegameclient.screens.TitleMenuOption;
import com.company.assembleegameclient.ui.dialogs.Dialog;
import com.company.rotmg.graphics.ScreenGraphic;
import com.company.ui.SimpleText;
import com.gskinner.motion.GTween;

import flash.display.DisplayObject;

import flash.display.Sprite;
import flash.events.Event;
import flash.events.MouseEvent;
import flash.utils.getTimer;

public class ConstellationBanner extends Sprite
{
    private var gs_:GameSprite;

    private var bannerContainer:Sprite;
    private var specialHitArea:Sprite;
    private var nodesContainer:Sprite;
    private var tempConstBanner:TempConstellationBanner;
    private var titleText:SimpleText;
    private var bottomTitleText:SimpleText;
    private var selectButton:TitleMenuOption;

    public var constellationType:String;
    public var position:int;
    private var lastUpdateTime:int;

    private var selectFunc:Function;
    private var over:Boolean;

    public function ConstellationBanner(gs:GameSprite, constellationType:String, position:int, nodes:Vector.<ConstellationNode>, onSelectFunc:Function)
    {
        super();
        this.gs_ = gs;
        this.lastUpdateTime = getTimer();
        this.constellationType = constellationType;
        this.position = position;
        this.tempConstBanner = new TempConstellationBanner()
        this.selectFunc = onSelectFunc;

        this.specialHitArea = new Sprite();
        this.specialHitArea.graphics.beginFill(0x000000, 0);
        this.specialHitArea.graphics.drawRect(0, 0, tempConstBanner.width, tempConstBanner.height);
        this.specialHitArea.graphics.endFill();
        this.specialHitArea.alpha = 0;
        addChild(specialHitArea);

        this.bottomTitleText = new SimpleText(20, 0xFFFFFF);
        this.bottomTitleText.text = constellationType;
        addChild(bottomTitleText);

        var i:int = 0;
        this.nodesContainer = new Sprite();
        for each (var node:ConstellationNode in nodes.filter(function(item:ConstellationNode, index:int, vector:Vector.<ConstellationNode>):Boolean {
            return item.large;
        })) {
            var newNode:ConstellationUINode = new ConstellationUINode(node, constellationType, gs);
            newNode.y = 140 + (i >= 2 ? 44 : 0);
            newNode.x = 50 - ((i == 0 || i == 2) ? newNode.width + 6 : -6);
            nodesContainer.addChild(newNode);
            i++;
        }
        addChild(nodesContainer);
        this.nodesContainer.alpha = 0;

        this.selectButton = new TitleMenuOption("Select", 18, true);
        addChild(this.selectButton);

        this.bannerContainer = new Sprite();
        addChild(bannerContainer);
        this.bannerContainer.addChild(this.tempConstBanner);

        this.titleText = new SimpleText(17, 0xFFFFFF);
        this.titleText.text = constellationType;
        this.titleText.updateMetrics();
        this.bottomTitleText.updateMetrics();
        this.bottomTitleText.x = width / 2 - this.bottomTitleText.width / 2;
        this.bottomTitleText.y = 90 - this.bottomTitleText.height;
        this.selectButton.x = width / 2 - this.selectButton.width / 2;
        this.selectButton.y = height - this.selectButton.height - 5;
        this.titleText.x = width / 2 - this.titleText.width / 2;
        this.titleText.y = height - this.titleText.height - 5;
        this.bannerContainer.addChild(titleText);

        addEventListener(Event.ADDED_TO_STAGE,this.onAddedToStage);
        addEventListener(Event.REMOVED_FROM_STAGE,this.onRemovedFromStage);
    }

    private function onAddedToStage(event:Event) : void
    {
        stage.addEventListener(Event.ENTER_FRAME,this.onEnterFrame, false, 1);
        addEventListener(MouseEvent.MOUSE_OVER,this.onMouseOver, false, 2);
        addEventListener(MouseEvent.MOUSE_OUT,this.onMouseOut, false, 2);
        this.selectButton.addEventListener(MouseEvent.CLICK, this.selectFunc, false, 1);
    }

    private function onRemovedFromStage(event:Event) : void
    {
        stage.removeEventListener(Event.ENTER_FRAME,this.onEnterFrame, false);
        removeEventListener(MouseEvent.MOUSE_OVER,this.onMouseOver, false);
        removeEventListener(MouseEvent.MOUSE_OUT,this.onMouseOut, false);
        this.selectButton.removeEventListener(MouseEvent.CLICK, this.selectFunc, false);
    }

    private function onMouseOver(e:MouseEvent):void {
        this.over = true;
    }
    private function onMouseOut(e:MouseEvent):void {
        this.over = false;
    }

    private function onEnterFrame(e:Event):void{
        if (this.parent == null)
            return;
        var currentTime:int = getTimer();
        var deltaTime:Number = (currentTime - lastUpdateTime) / 1000;
        this.lastUpdateTime = currentTime;

        var fadeAmount:Number = 0.06 * deltaTime * 60;

        if (!gs_.constellationsView.confirmationOpen && over) {
            this.bannerContainer.alpha = Math.max(bannerContainer.alpha - fadeAmount, 0);
            this.nodesContainer.alpha = Math.min(nodesContainer.alpha + fadeAmount, 1);
        } else {
            this.bannerContainer.alpha = Math.min(bannerContainer.alpha + fadeAmount, 1);
            this.nodesContainer.alpha = Math.max(nodesContainer.alpha - fadeAmount, 0);
        }

        if (bannerContainer.alpha > 0 && bannerContainer.parent == null)
            addChild(bannerContainer);
        else if (bannerContainer.alpha <= 0 && bannerContainer.parent != null)
            removeChild(bannerContainer);
    }
}
}
