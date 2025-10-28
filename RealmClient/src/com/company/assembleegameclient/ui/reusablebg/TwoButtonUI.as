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

public class TwoButtonUI extends ReusableBG
{
    public var overlayT:Sprite;
    public var leftButton:TitleMenuOption;
    public var rightButton:TitleMenuOption;

    public function TwoButtonUI(title:String, desc:String, leftButton:String, rightButton:String, leftFunc:Function, rightFunc:Function, width:int = 300, height:int = 200, gs:GameSprite = null)
    {
        overlayT = new Sprite();
        overlayT.graphics.clear();
        overlayT.graphics.beginFill(0x000000,0.3);
        overlayT.graphics.drawRect(-600,-300,1920,1080);
        overlayT.graphics.endFill();
        super(overlayT, title, desc, "", width, height, false, gs);

        this.leftButton = new TitleMenuOption(leftButton, 20, false);
        this.leftButton.addEventListener(MouseEvent.CLICK, leftFunc);
        addChild(this.leftButton);
        
        this.rightButton = new TitleMenuOption(rightButton, 20, false);
        this.rightButton.addEventListener(MouseEvent.CLICK, rightFunc);
        addChild(this.rightButton);

        addEventListener(Event.ADDED_TO_STAGE,this.onAddedToStage);
    }

    override protected function onAddedToStage(event:Event) : void {
        this.draw();

        this.leftButton.y = this.height_ - this.leftButton.height - 10;
        this.rightButton.y = this.height_ - this.rightButton.height - 10;
        this.leftButton.x = width / 4 - this.leftButton.width / 2;
        this.rightButton.x = width / 2 + width / 4 - this.rightButton.width / 2;
    }
}
}
