package kabam.rotmg.ui.view {
import flash.display.Bitmap;
import flash.display.Sprite;

import io.decagames.rotmg.ui.texture.TextureParser;

public class CharacterWindowBackground extends Sprite {

    public function CharacterWindowBackground() {
        super();
        var background:Bitmap = TextureParser.instance.getTexture("UI", "hud_background");
        addChild(background);
    }
}
}
