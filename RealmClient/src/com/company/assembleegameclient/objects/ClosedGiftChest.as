package com.company.assembleegameclient.objects {
import com.company.assembleegameclient.game.GameSprite;
import com.company.assembleegameclient.ui.panels.Panel;
import com.company.assembleegameclient.ui.panels.TextPanel;
import com.company.assembleegameclient.ui.tooltip.TextToolTip;
import com.company.assembleegameclient.ui.tooltip.ToolTip;

import flash.display.BitmapData;

public class ClosedGiftChest extends GameObject implements IInteractiveObject {

    public function ClosedGiftChest(objectXML:XML) {
        super(objectXML);
        isInteractive_ = true;
    }

    public function getPanel(gs:GameSprite):Panel {
        return new TextPanel(gs, "Gift chest is empty");
    }
}
}
