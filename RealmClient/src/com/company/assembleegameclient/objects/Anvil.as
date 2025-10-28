package com.company.assembleegameclient.objects {
import com.company.assembleegameclient.game.GameSprite;
import com.company.assembleegameclient.ui.constellations.ConstellationsPanel;
import com.company.assembleegameclient.ui.panels.GuildBoardPanel;
import com.company.assembleegameclient.ui.panels.Panel;

public class Anvil extends GameObject implements IInteractiveObject {
    public function Anvil(objectXML:XML) {
        super(objectXML);
        isInteractive_ = true;
    }

    public function getPanel(gs:GameSprite):Panel {
        return new ConstellationsPanel(gs);
    }
}
}
