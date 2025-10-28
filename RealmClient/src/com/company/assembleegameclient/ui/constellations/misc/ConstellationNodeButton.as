package com.company.assembleegameclient.ui.constellations.misc
{
import com.company.assembleegameclient.sound.SoundEffectLibrary;
import com.company.assembleegameclient.ui.constellations.ConstellationsScreen;
import com.company.assembleegameclient.ui.constellations.misc.ConstellationNode;
import com.company.assembleegameclient.ui.constellations.misc.images.TempConstellationBanner;
import com.company.assembleegameclient.ui.constellations.misc.images.TestNode;
import com.company.assembleegameclient.ui.constellations.xml.ConstellationsDataStore;
import com.company.assembleegameclient.ui.guild.*;
import com.company.assembleegameclient.game.GameSprite;
import com.company.assembleegameclient.game.events.GuildResultEvent;
import com.company.assembleegameclient.objects.Player;
import com.company.assembleegameclient.screens.TitleMenuOption;
import com.company.assembleegameclient.ui.dialogs.Dialog;
import com.company.assembleegameclient.ui.tooltip.TextToolTip;
import com.company.assembleegameclient.util.TextureRedrawer;
import com.company.rotmg.graphics.ScreenGraphic;
import com.company.ui.SimpleText;
import com.company.util.AssetLibrary;
import com.gskinner.motion.GTween;

import flash.display.Bitmap;

import flash.display.BitmapData;

import flash.display.DisplayObject;

import flash.display.Sprite;
import flash.events.Event;
import flash.events.MouseEvent;
import flash.geom.ColorTransform;
import flash.geom.Point;
import flash.utils.getTimer;

public class ConstellationNodeButton extends Sprite
{
    public var toolTip:TextToolTip;
    private var gs_:GameSprite;
    public var nodeSprite:Bitmap;
    public var unselectedData:BitmapData;
    public var bitmapData:BitmapData;

    private var main:ConstellationsScreen;
    public var nodeData:ConstellationNode;
    public var row:Vector.<ConstellationNodeButton>;
    public var selected:Boolean;
    private var over:Boolean;
    private var activated:Boolean;

    public function ConstellationNodeButton(node:ConstellationNode, gs:GameSprite)
    {
        super();
        this.nodeData = node;
        this.gs_ = gs;
        this.main = gs.constellationsView;
        this.nodeSprite = new Bitmap();
        this.bitmapData = AssetLibrary.getImageFromSet("constellations32x32", getNodeSpriteIndex(node));
        this.unselectedData = TextureRedrawer.desaturate(bitmapData);
        this.toolTip = new TextToolTip(3552822, 10197915, "", "", 400, gs);
        addChild(nodeSprite);

        this.row = new Vector.<ConstellationNodeButton>();

        reloadSprite();
        addEventListener(Event.ADDED_TO_STAGE,this.onAddedToStage);
        addEventListener(Event.REMOVED_FROM_STAGE,this.onRemovedFromStage);
    }

    public function setRow(row:Vector.<ConstellationNodeButton>):void {
        this.row = row;
    }

    private function checkCanActivate():Boolean {
        return !selected;
    }

    public function reloadSprite():void {
        if (!selected) {
            this.nodeSprite.bitmapData = unselectedData;
            transform.colorTransform = new ColorTransform(0.7, 0.7, 0.7);
        }
        else {
            this.nodeSprite.bitmapData = bitmapData;
            transform.colorTransform = new ColorTransform(1, 1, 1);
        }
    }

    private function onAddedToStage(event:Event) : void
    {
        stage.addEventListener(Event.ENTER_FRAME, this.onEnterFrame, false, 1);
        addEventListener(MouseEvent.MOUSE_OVER,this.onMouseOver);
        addEventListener(MouseEvent.MOUSE_OUT,this.onMouseOut);
    }

    private function onRemovedFromStage(event:Event) : void
    {
        removeEventListener(MouseEvent.MOUSE_OVER,this.onMouseOver);
        removeEventListener(MouseEvent.MOUSE_OUT,this.onMouseOut);
        stage.removeEventListener(Event.ENTER_FRAME, this.onEnterFrame, false);
        if (this.toolTip.parent != null) {
            this.toolTip.parent.removeChild(this.toolTip);
        }
    }

    private function onEnterFrame(event:Event):void {
        if (over) {
            this.toolTip.setTitle(nodeData.nodeName);
            this.toolTip.setText(nodeData.description);
            if (!stage.contains(this.toolTip)) {
                stage.addChild(this.toolTip);
            }
        } else {
            if (this.toolTip.parent != null) {
                this.toolTip.parent.removeChild(this.toolTip);
            }
        }
    }

    public function activate():void {
        addEventListener(MouseEvent.MOUSE_DOWN,this.onMouseDown);
        addEventListener(MouseEvent.MOUSE_UP,this.onMouseUp);
        this.activated = true;
    }

    public function deactivate():void {
        removeEventListener(MouseEvent.MOUSE_DOWN,this.onMouseDown);
        removeEventListener(MouseEvent.MOUSE_UP,this.onMouseUp);
        this.activated = false;
    }

    protected function onMouseOver(event:MouseEvent) : void {
        this.over = true;

        if (activated) {
            if (!selected)
                transform.colorTransform = new ColorTransform(0.90,0.90,0.90);
            else
                transform.colorTransform = new ColorTransform(1.20,1.20,1.20);
        }
    }

    protected function onMouseOut(event:MouseEvent) : void {
        this.over = false;

        if (activated) {
            if (!selected)
                transform.colorTransform = new ColorTransform(0.70,0.70,0.70);
            else
                transform.colorTransform = new ColorTransform(1,1,1);
        }
    }

    protected function onMouseDown(event:MouseEvent) : void {
        SoundEffectLibrary.play("button_click");
    }

    protected function onMouseUp(event:MouseEvent) : void {
        if (main.currentMode == "selecting") {
            if (checkCanActivate()) {
                this.selected = true;

                if (nodeData.constellation == gs_.map.player_.primaryConstellation) {
                    main.chosenPrimary[index()] = nodeData.id;
                } else if (nodeData.constellation == gs_.map.player_.secondaryConstellation) {
                    main.chosenSecondary[index()] = nodeData.id;
                }
                for each (var node:ConstellationNodeButton in row) {
                    if (node != this && node.selected) {
                        node.selected = false;
                        node.reloadSprite();
                    }
                }
            }
        }
        reloadSprite();
    }

    public function index():int {
        return nodeData.row + (nodeData.large ? 0 : 1);
    }

    public function getNodeSpriteIndex(node:ConstellationNode):int { //INSANE MATHS SO INSANE SO PRO
        //finds the node sprite index from the sheet assuming all sprites are in the same order as their respective nodes in the XML file
        var baseIndex:int = node.constellation * 13;

        if (!node.large) {
            baseIndex += 4;
            baseIndex += node.row * 3;
        }
        baseIndex += node.id - 1;

        return baseIndex;
    }
}
}
