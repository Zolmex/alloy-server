package com.company.assembleegameclient.ui.constellations.misc
{
import com.company.assembleegameclient.account.ui.CheckBoxField;
import com.company.assembleegameclient.game.GameSprite;
import com.company.assembleegameclient.parameters.Parameters;
import com.company.assembleegameclient.ui.reusablebg.*;

import flash.display.Sprite;
import flash.events.Event;

public class ConstellationsTutorial extends ReusableBG
{
    public var overlayT:Sprite;
    private var checkbox:CheckBoxField;

    public function ConstellationsTutorial(title:String, desc:String, button:String, width:int = 300, height:int = 200, pureBg:Boolean = false, gs:GameSprite = null)
    {
        overlayT = new Sprite();
        overlayT.graphics.clear();
        overlayT.graphics.beginFill(0x000000,0.3);
        overlayT.graphics.drawRect(-600,-300,1920,1080); //this is scaled so it will exceed the length of the screen and always cover the whole thing
        overlayT.graphics.endFill();
        super(overlayT, title, desc, button, width, height, pureBg, gs);

        this.checkbox = new CheckBoxField("Don't Show Again", false, "");
        addChild(checkbox);

        addEventListener(Event.ADDED_TO_STAGE,this.onAddedToStage);
    }

    override public function close():void {
        if (this.checkbox.isChecked()) {
            Parameters.data_.showConstellationsTutorial = false;
        }
        if (overlay != null && overlay.parent != null)
            parent.removeChild(overlay);
        parent.removeChild(this);
    }

    override protected function onAddedToStage(event:Event) : void {
        this.draw();
        this.checkbox.y = this.button.y + this.button.height / 2 - this.checkbox.height / 2;
        this.checkbox.x = 10;
    }
}
}
