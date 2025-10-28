package com.company.assembleegameclient.ui.damagecounter {
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
import flash.display.JointStyle;
import flash.display.LineScaleMode;

import flash.display.Sprite;
import flash.filters.DropShadowFilter;
import flash.geom.Point;

import kabam.rotmg.ui.UIUtils;

import starling.utils.Color;

public class DamageCounterEnemyRect extends Sprite {

    private var enemyPortrait:Bitmap;
    private var enemyName:SimpleText;
    private var hpBar:StatusBar;

    private var bitmapCenter:Point;

    // Background stuff
    private var path:GraphicsPath = new GraphicsPath(new Vector.<int>(), new Vector.<Number>());
    private var backgroundFill:GraphicsSolidFill = new GraphicsSolidFill(3552822, 1);
    private var outlineFill:GraphicsSolidFill = new GraphicsSolidFill(10197915, 1);
    private var outlineStyle:GraphicsStroke = new GraphicsStroke(1, false, LineScaleMode.NORMAL, CapsStyle.NONE, JointStyle.ROUND, 3, outlineFill);
    private const graphicsData:Vector.<IGraphicsData> = new <IGraphicsData>[outlineStyle, backgroundFill, path, GraphicsUtil.END_FILL, GraphicsUtil.END_STROKE];

    public function DamageCounterEnemyRect() {
        addEnemyInfo();
        addHpBar();
        redraw();
    }

    private function addEnemyInfo() : void {
        var bd:BitmapData = AssetLibrary.getImageFromSet("lofiChar8x8", 0);
        enemyPortrait = new Bitmap(TextureRedrawer.redraw(bd, 80, false, 0));
        enemyPortrait.x = -4;
        enemyPortrait.y = -4;
        addChild(enemyPortrait);

        enemyName = new SimpleText(14, Color.WHITE);
        enemyName.filters = [new DropShadowFilter(0)];
        enemyName.text = "Enemy";
        enemyName.updateMetrics();
        addChild(enemyName);
    }

    private function addHpBar() : void {
        hpBar = new StatusBar("hp_bar_background", "hp_bar_fill");
        hpBar.draw(0, 0);
        addChild(hpBar);
    }

    private function redraw() : void {
        UIUtils.positionWithAnchors(enemyName, enemyPortrait, UIUtils.ANCHOR_BOTTOM_LEFT, UIUtils.ANCHOR_CENTER_RIGHT, 8, 2);
        UIUtils.positionWithAnchors(hpBar, enemyPortrait, UIUtils.ANCHOR_TOP_LEFT, UIUtils.ANCHOR_CENTER_RIGHT, 8, 4);

        graphics.clear();
        GraphicsUtil.clearPath(path);
        GraphicsUtil.drawCutEdgeRect(-4, 0, width + 8, height, 4, [1, 1, 1, 1], path);
        graphics.drawGraphicsData(this.graphicsData);
    }

    public function setEnemy(enemy:GameObject) : void {
        var size:int = 80 * (8 / enemy.texture_.width);
        enemyPortrait.bitmapData = TextureRedrawer.redraw(enemy.texture_, size, false, 0);

        enemyName.text = enemy.name_;
        enemyName.updateMetrics();

        hpBar.draw(enemy.hp, enemy.maxHP);

        if (enemy.name_ != enemyName.text)
            redraw();
    }

    public function dispose() : void {
        enemyPortrait = null;
        enemyName = null;
        hpBar = null;
    }
}

}
