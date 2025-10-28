package com.company.assembleegameclient.ui.panels {
import com.company.assembleegameclient.game.GameSprite;
import com.company.ui.SimpleText;

import flash.filters.DropShadowFilter;

import flash.text.TextFieldAutoSize;

public class TextPanel extends Panel {

    private var titleText:SimpleText;

    public function TextPanel(gs:GameSprite, title:String) {
        super(gs);
        this.titleText = new SimpleText(16, 0xFFFFFF);
        this.titleText.setBold(true);
        this.titleText.setText(title);
        this.titleText.autoSize = TextFieldAutoSize.CENTER;
        this.titleText.filters = [new DropShadowFilter(0, 0, 0)];
        this.titleText.x = (WIDTH / 2) - (this.titleText.width / 2);
        this.titleText.y = (HEIGHT / 2) - this.titleText.height;
        addChild(this.titleText);
    }
}
}
