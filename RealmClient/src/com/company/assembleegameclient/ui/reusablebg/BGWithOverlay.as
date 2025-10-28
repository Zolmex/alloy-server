package com.company.assembleegameclient.ui.reusablebg
{
import com.company.assembleegameclient.game.GameSprite;

import flash.display.Sprite;

public class BGWithOverlay extends ReusableBG
{
    public var overlayT:Sprite;

    public function BGWithOverlay(title:String, desc:String, button:String, width:int = 300, height:int = 200, pureBg:Boolean = false, gs:GameSprite = null)
    {
        overlayT = new Sprite();
        overlayT.graphics.clear();
        overlayT.graphics.beginFill(0x000000,0.3);
        overlayT.graphics.drawRect(-600,-300,1920,1080); //this is scaled so it will exceed the length of the screen and always cover the whole thing
        overlayT.graphics.endFill();

        super(overlayT, title, desc, button, width, height, pureBg, gs);
    }
}
}
