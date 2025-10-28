package com.company.assembleegameclient.ui.constellations.misc
{
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
import com.company.rotmg.graphics.ScreenGraphic;
import com.company.ui.SimpleText;
import com.company.util.AssetLibrary;
import com.gskinner.motion.GTween;

import flash.display.Bitmap;

import flash.display.DisplayObject;

import flash.display.Sprite;
import flash.events.Event;
import flash.events.MouseEvent;
import flash.geom.Point;
import flash.utils.getTimer;

public class ConstellationUINode extends Sprite
{
    public var nodeData:ConstellationNode;
    public var nodeSprite:Bitmap;
    public var constellationType:String;
    public var toolTip:TextToolTip;
    private var gs_:GameSprite;
    private var over:Boolean;
    private var inView:Boolean;

    public function ConstellationUINode(node:ConstellationNode, constellationType:String, gs:GameSprite, inView:Boolean = true)
    {
        super();
        this.inView = inView;
        this.nodeData = node;
        this.constellationType = constellationType;
        this.gs_ = gs;
        this.nodeSprite = new Bitmap(AssetLibrary.getImageFromSet("constellations32x32", getNodeSpriteIndex(node)));
        this.toolTip = new TextToolTip(3552822, 10197915, "", "", 200, gs);
        addChild(nodeSprite);

        addEventListener(Event.ADDED_TO_STAGE,this.onAddedToStage);
        addEventListener(Event.REMOVED_FROM_STAGE,this.onRemovedFromStage);
    }

    private function onAddedToStage(event:Event) : void
    {
        addEventListener(MouseEvent.MOUSE_OVER,this.onMouseOver, false, 2);
        addEventListener(MouseEvent.MOUSE_OUT,this.onMouseOut, false, 2);
        stage.addEventListener(Event.ENTER_FRAME, this.onEnterFrame, false, 1);
    }

    private function onRemovedFromStage(event:Event) : void
    {
        removeEventListener(MouseEvent.MOUSE_OVER,this.onMouseOver, false);
        removeEventListener(MouseEvent.MOUSE_OUT,this.onMouseOut, false);
        stage.removeEventListener(Event.ENTER_FRAME, this.onEnterFrame, false);
        if (this.toolTip.parent != null) {
            this.toolTip.parent.removeChild(this.toolTip);
        }
    }
    private function onMouseOver(e:MouseEvent):void {
        this.over = true;
    }
    private function onMouseOut(e:MouseEvent):void {
        this.over = false;
    }
    private function onEnterFrame(event:Event):void {
        if ((!inView || !gs_.constellationsView.confirmationOpen) && over) {
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
