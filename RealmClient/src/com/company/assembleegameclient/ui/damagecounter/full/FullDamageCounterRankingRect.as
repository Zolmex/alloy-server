package com.company.assembleegameclient.ui.damagecounter.full {
import com.company.assembleegameclient.objects.GameObject;
import com.company.assembleegameclient.ui.StatusBar;
import com.company.assembleegameclient.util.TextureRedrawer;
import com.company.ui.SimpleText;
import com.company.util.AssetLibrary;
import com.company.util.GraphicsUtil;

import flash.display.Bitmap;

import flash.display.BitmapData;

import flash.display.CapsStyle;
import flash.display.GraphicsPath;
import flash.display.GraphicsSolidFill;

import flash.display.GraphicsStroke;

import flash.display.IGraphicsData;

import flash.display.IGraphicsFill;
import flash.display.IGraphicsPath;
import flash.display.JointStyle;
import flash.display.LineScaleMode;
import flash.display.Sprite;
import flash.filters.DropShadowFilter;

import starling.utils.Color;

public class FullDamageCounterRankingRect extends Sprite {

    private var portrait:Bitmap;
    private var nameText:SimpleText;
    private var damageBar:StatusBar;
    private var damageText:SimpleText;

    // Background stuff
    private var path:GraphicsPath = new GraphicsPath(new Vector.<int>(), new Vector.<Number>());
    private var backgroundFill:GraphicsSolidFill = new GraphicsSolidFill(3552822, 1);
    private var outlineFill:GraphicsSolidFill = new GraphicsSolidFill(10197915, 1);
    private var outlineStyle:GraphicsStroke = new GraphicsStroke(1, false, LineScaleMode.NORMAL, CapsStyle.NONE, JointStyle.ROUND, 3, outlineFill);
    private const graphicsData:Vector.<IGraphicsData> = new <IGraphicsData>[outlineStyle, backgroundFill, path, GraphicsUtil.END_FILL, GraphicsUtil.END_STROKE];

    public function FullDamageCounterRankingRect() {
        addPlayerInfo();
        addDamageBar();

        graphics.clear();
        GraphicsUtil.clearPath(path);
        GraphicsUtil.drawCutEdgeRect(-4, 0, width + 8, height + 2, 4, [1, 1, 1, 1], path);
        graphics.drawGraphicsData(this.graphicsData);
    }

    private function addPlayerInfo() : void {
        portrait = new Bitmap();
        var bd:BitmapData = AssetLibrary.getImageFromSet("lofiInterface", 126);
        portrait.bitmapData = TextureRedrawer.redraw(bd, 32, true, 0);
        addChild(portrait);

        nameText = new SimpleText(14, Color.WHITE);
        nameText.filters = [new DropShadowFilter(1.5, 90, 0)];
        nameText.text = "Player";
        nameText.updateMetrics();
        nameText.x = portrait.width;
        addChild(nameText);
    }

    private function addDamageBar() : void {
        damageBar = new StatusBar("hp_bar_background", "hp_bar_fill");
        damageBar.scaleY = 0.8;
        damageBar.scaleX = 0.5;
        damageBar.hideValue_ = true;
        damageBar.draw(0, 0);
        damageBar.x = nameText.x;
        damageBar.y = nameText.height;
        addChild(damageBar);

        damageText = new SimpleText(12, Color.WHITE);
        damageText.setBold(true);
        damageText.filters = [new DropShadowFilter(1.5, 90, 0)];
        damageText.text = "0";
        damageText.updateMetrics();
        damageText.x = damageBar.x;
        damageText.y = damageBar.y + damageBar.height / 2 - damageText.height / 2;
        addChild(damageText);
    }

    public function SetPlayer(player:GameObject) : void {
        if (player.name_ == nameText.text)
            return;

        nameText.text = player.name_;
        nameText.updateMetrics();

        var texture:BitmapData = player.getPortraitTexture();
        var size:int = 64 * (8 / texture.width);
        portrait.bitmapData = TextureRedrawer.redraw(texture, size, true, 0);
        portrait.x = 16 - portrait.bitmapData.width / 2;
        portrait.y = 20 - portrait.bitmapData.height / 2;
    }

    public function SetBarFill(playerDamage:uint, enemyHpLost:int) : void {
        damageBar.draw(playerDamage, enemyHpLost);
        damageText.setText(playerDamage.toFixed());
        damageText.updateMetrics();
    }

    public function dispose() : void {
        portrait = null;
        nameText = null;
        damageBar = null;
        parent.removeChild(this);
    }
}
}
