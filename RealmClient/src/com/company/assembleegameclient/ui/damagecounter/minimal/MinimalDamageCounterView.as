package com.company.assembleegameclient.ui.damagecounter.minimal {
import com.company.assembleegameclient.map.Map;
import com.company.assembleegameclient.objects.GameObject;
import com.company.assembleegameclient.ui.damagecounter.DamageCounterEnemyRect;
import com.company.assembleegameclient.ui.damagecounter.IDamageCounterView;
import flash.display.Sprite;

// Damage counter UI without the ranking
public class MinimalDamageCounterView extends Sprite implements IDamageCounterView {
    private var map:Map;

    private var enemyInfo:DamageCounterEnemyRect;

    public function MinimalDamageCounterView(map:Map) {
        this.map = map;

        enemyInfo = new DamageCounterEnemyRect();
        addChild(enemyInfo);

        visible = false;
    }

    public function show() : void {
        visible = true;
    }

    public function hide() : void {
        visible = false;
    }

    public function update(targetId:int, playerDamage:uint, topDamagers:Vector.<Object>) : void {
        if (targetId == -1) {
            hide();
            return;
        }

        var target:GameObject = map.goDict_[targetId];

        if (target == null) {
            if (map.player_ != null && map.player_.timeInCombat == 0)
                hide();
            return;
        }

        if (!visible)
            show();

        enemyInfo.setEnemy(target);
    }

    public function dispose():void {
        visible = false;
        enemyInfo.dispose();
        enemyInfo = null;
        removeChildren();
        parent.removeChild(this);
    }

    public function getSprite():Sprite {
        return this;
    }
}
}
