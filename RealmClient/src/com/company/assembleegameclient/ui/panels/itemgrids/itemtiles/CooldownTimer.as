package com.company.assembleegameclient.ui.panels.itemgrids.itemtiles {
import com.company.assembleegameclient.itemData.ItemData;
import com.company.assembleegameclient.util.FilterUtil;
import com.company.ui.SimpleText;
import com.company.util.MathUtil2;

import flash.display.Graphics;

import flash.display.Shape;
import flash.display.Sprite;
import flash.text.TextFieldAutoSize;
import flash.utils.getTimer;

public class CooldownTimer extends Sprite {

    private var abilityUseStart:int;
    private var cooldown:int;

    private var cooldownTimerBackground:Shape;
    private var cooldownTimerMask:Shape;
    private var cooldownText:SimpleText;

    public function CooldownTimer() {
        this.cooldownTimerMask = new Shape();
        this.cooldownTimerBackground = new Shape();
        var g:Graphics = this.cooldownTimerBackground.graphics;
        g.clear();
        g.beginFill(0, 0.8);
        g.drawRect(0, 0, ItemTile.WIDTH, ItemTile.HEIGHT);
        g.endFill();
        this.cooldownTimerBackground.mask = this.cooldownTimerMask;
        this.cooldownText = new SimpleText(14, 0xFFFFFF, false, 40);
        this.cooldownText.setAutoSize(TextFieldAutoSize.LEFT);
        this.cooldownText.filters = FilterUtil.getTextOutlineFilter();
        addChild(this.cooldownTimerMask);
        addChild(this.cooldownTimerBackground);
        addChild(this.cooldownText);
    }

    public function update():void {
        if (this.abilityUseStart > 0){
            var time:int = getTimer();
            if (time > this.abilityUseStart + this.cooldown){
                this.reset();
            }
            else {
                this.drawCooldownTimer(time);
            }
        }
    }

    private function drawCooldownTimer(time:int):void {
        var elapsed:int = (time - this.abilityUseStart);
        var progress:Number = elapsed / Number(this.cooldown);
        drawCircularSector(this.cooldownTimerMask, progress);

        var timeLeftSeconds:Number = MathUtil2.roundTo((this.cooldown - elapsed) / 1000.0, 1);
        this.cooldownText.setText(timeLeftSeconds + "s");
        this.cooldownText.updateMetrics();
        this.cooldownText.x = (ItemTile.WIDTH - this.cooldownText.actualWidth_) / 2;
        this.cooldownText.y = (ItemTile.HEIGHT - this.cooldownText.actualHeight_) / 2;

        this.cooldownTimerMask.visible = true;
        this.cooldownTimerBackground.visible = true;
        this.cooldownText.visible = true;
    }

    private static function drawCircularSector(shape:Shape, progress:Number):void {
        var g:Graphics = shape.graphics;
        var radius:Number = (ItemTile.WIDTH * 2) / 2.0;
        var cx:Number = radius / 2;
        var cy:Number = radius / 2;
        var thetaStart:Number = (3 * Math.PI) / 2;
        var thetaEnd:Number = thetaStart + 2 * Math.PI * (1 - progress);
        var numSegments:int = 100;
        var angleStep:Number = (thetaStart - thetaEnd) / numSegments;

        g.clear();
        g.beginFill(0);
        g.moveTo(cx, cy); // Start at center

        for (var i:int = 0; i < numSegments; i++) {
            var angle:Number = thetaStart + i * angleStep;
            var x:Number = cx + radius * Math.cos(angle);
            var y:Number = cy + radius * Math.sin(angle);
            g.lineTo(x, y);
        }

        g.lineTo(cx, cy);
        g.endFill();
    }

    public function start(itemData:ItemData):void {
        this.cooldown = itemData.Cooldown * 1000;
        this.abilityUseStart = getTimer();
    }

    public function reset():void {
        this.abilityUseStart = 0;
        this.cooldown = 0;
        this.cooldownTimerMask.visible = false;
        this.cooldownTimerBackground.visible = false;
        this.cooldownText.visible = false;
    }
}
}
