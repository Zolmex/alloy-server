package kabam.rotmg.ui.view {
import flash.display.Sprite;

import io.decagames.rotmg.ui.texture.TextureParser;

import kabam.rotmg.ui.view.components.PotionSlotView;

public class PotionInventoryView extends Sprite {

    private static const BUTTON_SPACE:int = 6;

    private const slots:Array = ["potion_slot_left", "potion_slot_right"];

    public function PotionInventoryView() {
        var psv:PotionSlotView = null;
        super();
        for (var i:int = 0; i < 2; i++) {
            psv = new PotionSlotView(this.slots[i], i);
            psv.x = i * (PotionSlotView.BUTTON_WIDTH + BUTTON_SPACE);
            addChild(psv);
        }
    }
}
}
