package kabam.rotmg.ui.view {
import com.company.assembleegameclient.objects.Player;
import com.company.assembleegameclient.ui.StatusBar;

import flash.display.Sprite;
import flash.events.Event;

public class StatMetersView extends Sprite {

    private var hpBar_:StatusBar;
    private var mpBar_:StatusBar;
    private var msBar_:StatusBar;

    public function StatMetersView() {
        super();
        this.hpBar_ = new StatusBar("hp_bar_background", "hp_bar_fill", "HP");
        this.mpBar_ = new StatusBar("mp_bar_background", "mp_bar_fill", "MP");
        this.msBar_ = new StatusBar("ms_bar_background", "ms_bar_fill", null); // Outlines the hp bar
        this.hpBar_.x = 2;
        this.hpBar_.y = -1;
        this.mpBar_.x = 1;
        this.mpBar_.y = 19;
        this.msBar_.x = 0;
        this.msBar_.y = -3;
        this.hpBar_.draw(0, 0);
        this.mpBar_.draw(0, 0);
        this.msBar_.draw(0, 0);
        addChild(this.mpBar_);
        addChild(this.msBar_);
        addChild(this.hpBar_); // Draw hp bar on top of ms bar
    }

    public function update(player:Player):void {
        this.hpBar_.draw(player.hp, player.maxHP);
        this.mpBar_.draw(player.mp, player.maxMP);
        this.msBar_.draw(player.ms, player.maxMS);
    }
}
}
