package kabam.rotmg.game.view.components {
import flash.display.Bitmap;
import flash.display.Sprite;
import flash.geom.ColorTransform;

public class TabView extends Sprite {

    public var index:int;
    private var bg:Bitmap;
    private var icon:Bitmap;

    public function TabView(index:int, bg:Bitmap, icon:Bitmap) {
        super();
        this.index = index;
        this.bg = bg;
        addChild(bg);
        if (icon) {
            this.icon = icon;
            icon.x = icon.x + 1;
            icon.y = icon.y - 11;
            addChild(icon);
        }
    }

    public function setSelected(selected:Boolean):void {
        var ct:ColorTransform = this.bg.transform.colorTransform;
        ct.color = selected ? uint(TabStripView.BACKGROUND_COLOR) : uint(TabStripView.TAB_COLOR);
        this.bg.transform.colorTransform = ct;
    }
}
}
