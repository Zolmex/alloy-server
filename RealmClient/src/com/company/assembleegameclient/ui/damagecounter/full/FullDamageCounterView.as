package com.company.assembleegameclient.ui.damagecounter.full {
import com.company.assembleegameclient.ui.damagecounter.*;
import com.company.assembleegameclient.map.Map;
import com.company.assembleegameclient.objects.GameObject;
import flash.display.Sprite;

public class FullDamageCounterView extends Sprite implements IDamageCounterView {
    private var map:Map;

    private var enemyInfo:DamageCounterEnemyRect;

    private var rankingRects:Vector.<FullDamageCounterRankingRect> = new Vector.<FullDamageCounterRankingRect>();

    public function FullDamageCounterView(map:Map) {
        this.map = map;

        enemyInfo = new DamageCounterEnemyRect();
        addChild(enemyInfo);

        addRanking();

        visible = false;
    }

    private function addRanking() : void {
        var y:int = height + 4;

        for (var i:int = 0; i < 5; i++) {
            var rankingRect:FullDamageCounterRankingRect = new FullDamageCounterRankingRect();
            rankingRect.y = y;
            rankingRect.visible = false;
            addChild(rankingRect);
            rankingRects.push(rankingRect);
            y += rankingRect.height + 10;
        }
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

        drawRanking(topDamagers, target.maxHP - target.hp);
    }

    public function drawRanking(topDamagers:Vector.<Object>, enemyHpLost:int) : void {
        var i:int = 0;
        var playerId:int;
        var dmg:uint;
        var player:GameObject;
        for each (var pair:Object in topDamagers) {
            playerId = pair.key;
            dmg= pair.value;

            player = map.goDict_[playerId];
            if (player == null)
                continue;

            rankingRects[i].SetPlayer(player);
            rankingRects[i].SetBarFill(dmg, enemyHpLost);
            rankingRects[i].visible = true;

            i++;
        }

        // Hide empty rects
        for (i; i < 5; i++) {
            rankingRects[i].visible = false;
        }
    }

    public function dispose():void {
        visible = false;

        enemyInfo.dispose();
        enemyInfo = null;

        for each (var rect:FullDamageCounterRankingRect in rankingRects)
            rect.dispose();
        rankingRects.length = 0;
        rankingRects = null;

        removeChildren();
        parent.removeChild(this);
    }

    public function getSprite():Sprite {
        return this;
    }
}
}
