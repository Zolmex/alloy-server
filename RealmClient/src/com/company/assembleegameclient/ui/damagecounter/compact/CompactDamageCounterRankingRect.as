package com.company.assembleegameclient.ui.damagecounter.compact {
import com.company.assembleegameclient.objects.GameObject;
import com.company.assembleegameclient.ui.StatusBar;
import com.company.ui.SimpleText;
import com.company.util.GraphicsUtil;

import flash.display.CapsStyle;

import flash.display.GraphicsPath;
import flash.display.GraphicsSolidFill;
import flash.display.GraphicsStroke;
import flash.display.IGraphicsData;
import flash.display.JointStyle;
import flash.display.LineScaleMode;
import flash.display.Sprite;
import flash.filters.DropShadowFilter;

import starling.utils.Color;

public class CompactDamageCounterRankingRect extends Sprite {
    private var nameText:SimpleText;
    private var damageBar:StatusBar;
    private var damageText:SimpleText;

    // Background stuff
    private var backgroundColor:uint = 3552822;
    private var outlineColor:uint = 10197915;
    private var path:GraphicsPath = new GraphicsPath(new Vector.<int>(), new Vector.<Number>());
    private var backgroundFill:GraphicsSolidFill = new GraphicsSolidFill(0, 1);
    private var outlineFill:GraphicsSolidFill = new GraphicsSolidFill(0, 1);
    private var outlineStyle:GraphicsStroke = new GraphicsStroke(1, false, LineScaleMode.NORMAL, CapsStyle.NONE, JointStyle.ROUND, 3, outlineFill);
    private const graphicsData:Vector.<IGraphicsData> = new <IGraphicsData>[outlineStyle, backgroundFill, path, GraphicsUtil.END_FILL, GraphicsUtil.END_STROKE];

    public function CompactDamageCounterRankingRect() {
        nameText = new SimpleText(14, Color.WHITE);
        nameText.filters = [new DropShadowFilter(1.5, 90, 0)];
        nameText.text = "PLAYERNAME"; // 10 character is max username length, make some space for hp bar
        nameText.updateMetrics();
        addChild(nameText);

        damageBar = new StatusBar("hp_bar_background", "hp_bar_fill");
        damageBar.scaleY = 0.8;
        damageBar.scaleX = 0.5;
        damageBar.hideValue_ = true;
        damageBar.draw(0, 0);
        damageBar.x = nameText.width + 2;
        damageBar.y = nameText.height / 2 - damageBar.height / 2 + 2;
        addChild(damageBar);

        damageText = new SimpleText(12, Color.WHITE);
        damageText.setBold(true);
        damageText.filters = [new DropShadowFilter(1.5, 90, 0)];
        damageText.text = "0";
        damageText.updateMetrics();
        damageText.x = damageBar.x;
        damageText.y = damageBar.y + damageBar.height / 2 - damageText.height / 2;
        addChild(damageText);

        backgroundFill.color = backgroundColor;
        outlineFill.color = outlineColor;
        graphics.clear();
        GraphicsUtil.clearPath(path);
        GraphicsUtil.drawCutEdgeRect(-4, 0, width + 8, height + 2, 4, [1, 1, 1, 1], path);
        graphics.drawGraphicsData(this.graphicsData);
    }

    public function SetPlayer(player:GameObject) : void {
        if (player.name_ == nameText.text)
            return;

        nameText.text = player.name_;
        nameText.updateMetrics();
    }

    public function SetBarFill(playerDamage:uint, enemyHpLost:int) : void {
        damageBar.draw(playerDamage, enemyHpLost);
        damageText.setText(playerDamage.toFixed());
        damageText.updateMetrics();
    }

    public function dispose() : void {
        nameText = null;
        damageBar = null;
        parent.removeChild(this);
    }
}
}
