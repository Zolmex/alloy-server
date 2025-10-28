package com.company.assembleegameclient.ui.reusablebg
{
import com.company.assembleegameclient.game.GameSprite;
import com.company.assembleegameclient.screens.TitleMenuOption;
import com.company.assembleegameclient.ui.ClickableText;
import com.company.ui.SimpleText;
import com.company.util.GraphicsUtil;
import flash.display.CapsStyle;
import flash.display.DisplayObject;
import flash.display.GraphicsPath;
import flash.display.GraphicsSolidFill;
import flash.display.GraphicsStroke;
import flash.display.IGraphicsData;
import flash.display.JointStyle;
import flash.display.LineScaleMode;
import flash.display.Sprite;
import flash.events.Event;
import flash.events.MouseEvent;
import flash.filters.DropShadowFilter;
import flash.text.TextFieldAutoSize;

import kabam.rotmg.account.web.view.LabeledField;

public class ReusableBG extends Sprite
{
    public var titleText:SimpleText;
    public var descText:SimpleText;
    public var overlay:Sprite;
    public var button:TitleMenuOption;
    public var width_:int = 288;
    public var height_:int = 100;
    public var pureBg:Boolean;
    public var hasButton:Boolean;

    public var backgroundFill_:GraphicsSolidFill = new GraphicsSolidFill(3552822,1);
    public var outlineFill_:GraphicsSolidFill = new GraphicsSolidFill(16777215,1);
    public var lineStyle_:GraphicsStroke = new GraphicsStroke(1,false,LineScaleMode.NORMAL,CapsStyle.NONE,JointStyle.ROUND,3,outlineFill_);
    public var path2_:GraphicsPath= new GraphicsPath(new Vector.<int>(),new Vector.<Number>());
    public var gs_:GameSprite;
    public const graphicsData_:Vector.<IGraphicsData> = new <IGraphicsData>[backgroundFill_,path2_,GraphicsUtil.END_FILL,lineStyle_,path2_,GraphicsUtil.END_STROKE];

    public function ReusableBG(overlay:Sprite, title:String, desc:String, button:String, width:int = 300, height:int = 200, pureBg:Boolean = false, gs:GameSprite = null) {
        this.gs_ = gs;

        this.gs_.mui_.clearInput();
        this.gs_.mui_.inputWhitelist.push("ReusableBGBlacklist");

        this.width_ = width;
        this.height_ = height;
        this.pureBg = pureBg;
        this.overlay = overlay;
        if (!pureBg) {
            this.titleText = new SimpleText(22,0xFFFFFF,false);
            this.titleText.text = title;
            this.titleText.setAutoSize(TextFieldAutoSize.CENTER);
            this.titleText.autoResize = true;
            this.titleText.updateMetrics();
            this.titleText.setBold(true);
            this.titleText.filters = [new DropShadowFilter(0,0,0)];
            this.titleText.filters = [new DropShadowFilter(0,0,0,0.5,12,12)];
            addChild(this.titleText);
            this.descText = new SimpleText(18,11776947,false);
            this.descText.setAutoSize(TextFieldAutoSize.CENTER);
            this.descText.autoResize = true;
            this.descText.text = desc;
            this.descText.updateMetrics();
            addChild(this.descText);
            if (button != "") {
                this.hasButton = true;
                this.button = new TitleMenuOption(button, 20, false);
                this.button.addEventListener(MouseEvent.CLICK, onCloseClick);
                addChild(this.button);
            }
        }
        filters = [new DropShadowFilter(0,0,0,0.5,12,12)];
        addEventListener(Event.ADDED_TO_STAGE,this.onAddedToStage);
        addEventListener(Event.REMOVED_FROM_STAGE,this.onRemovedFromStage);
    }

    private function onCloseClick(e:MouseEvent) : void {
        this.close();
    }

    public function close():void {
        if (overlay != null && overlay.parent != null)
            parent.removeChild(overlay);
        parent.removeChild(this);
    }

    protected function onAddedToStage(event:Event) : void {
        this.draw();
    }

    private function onRemovedFromStage(event:Event) : void {
        this.gs_.mui_.inputWhitelist.pop();
    }

    public function draw() : void {
        graphics.clear();
        GraphicsUtil.clearPath(this.path2_);
        GraphicsUtil.drawCutEdgeRect(-6,-6,this.width_,this.height_,4,[1,1,1,1],this.path2_);
        graphics.drawGraphicsData(this.graphicsData_);

        x = 400 - (this.width_ - 6) / 2;
        y = 300 - height / 2;

        if (!pureBg) {
            this.titleText.x = width_ / 2 - this.titleText.width / 2 - 6;
            this.descText.x = width_ / 2 - this.descText.width / 2 - 6;
            this.descText.y = this.titleText.y + this.titleText.height + 5;
            if (hasButton) {
                this.button.x = width_ / 2 - this.button.width / 2 - 6;
                this.button.y = this.height_ - this.button.height - 15;
            }
        }
    }
}
}
