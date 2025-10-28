package com.company.assembleegameclient.ui {
import com.company.ui.SimpleText;

import flash.display.Bitmap;

import flash.display.Sprite;
import flash.events.Event;
import flash.events.MouseEvent;
import flash.filters.DropShadowFilter;

import io.decagames.rotmg.ui.texture.TextureParser;

public class StatusBar extends Sprite {

    private var background:Bitmap;
    private var fill:Bitmap;
    public var textColor_:uint;
    public var val_:int = -1;
    public var max_:int = -1;
    public var labelText_:SimpleText;
    public var valueText_:SimpleText;
    public var boostText_:SimpleText;
    public var hideValue_:Boolean = false;
    private var repetitions:int;
    private var direction:int = -1;
    private var speed:Number = 0.1;

    public function StatusBar(backgroundUI:String, fillUI:String, label:String = null) {
        super();
        background = TextureParser.instance.getTexture("UI", backgroundUI);
        addChild(background);
        fill = TextureParser.instance.getTexture("UI", fillUI);
        addChild(fill);
        textColor_ = 16777215;
        if (label != null && label.length != 0) {
            labelText_ = new SimpleText(13, textColor_, false, 0, 0);
            labelText_.setBold(true);
            labelText_.text = label;
            labelText_.updateMetrics();
            labelText_.y = label == "HP" ? -4 : -3;
            labelText_.filters = [new DropShadowFilter(1.5, 90, 0)];
            addChild(labelText_);
        }
        valueText_ = new SimpleText(13, 16777215, false, 0, 0);
        valueText_.setBold(true);
        valueText_.filters = [new DropShadowFilter(1.5, 90, 0)];
        valueText_.y = label == "HP" ? -4 : -3;
        boostText_ = new SimpleText(13, textColor_, false, 0, 0);
        boostText_.setBold(true);
        boostText_.alpha = 0.6;
        boostText_.y = label == "HP" ? -4 : -3;
        boostText_.filters = [new DropShadowFilter(1.5, 90, 0)];
    }

    public function draw(val:int, max:int):void {
        if (max > 0) {
            val = Math.min(max, Math.max(0, val));
        }
        if (val == val_ && max == max_) {
            return;
        }
        val_ = val;
        max_ = max;
        internalDraw();
    }

    private function setTextColor(textColor:uint):void {
        textColor_ = textColor;
        if (boostText_ != null) {
            boostText_.setColor(textColor_);
        }
        valueText_.setColor(textColor_);
    }

    private function internalDraw():void {
        var textColor:uint = 16777215;
        if (textColor_ != textColor) {
            setTextColor(textColor);
        }

        if (max_ > 0) {
            var perc:Number = val_ / max_;
            fill.scaleX = perc;
        } else {
            fill.scaleX = 1;
        }

        if (!hideValue_) {
            if (max_ > 0) {
                valueText_.text = "" + val_ + "/" + max_;
            } else {
                valueText_.text = "" + val_;
            }
            valueText_.updateMetrics();
            if (!contains(valueText_)) {
                addChild(valueText_);
            }
            valueText_.x = (background.width - valueText_.width) / 2;
        } else {
            if (contains(valueText_))
                removeChild(valueText_);
        }

        if (contains(boostText_)) {
            removeChild(boostText_);
        }
    }
}
}
