package com.company.assembleegameclient.ui.options {
import com.company.ui.SimpleText;

import flash.display.Shape;
import flash.display.Sprite;
import flash.events.Event;
import flash.events.MouseEvent;
import flash.filters.DropShadowFilter;
import flash.geom.Point;
import flash.text.TextFieldAutoSize;

public class VolumeSliderBar extends Sprite {
    private const MIN:Number = 0;
    private const MAX:Number = 1;

    public function VolumeSliderBar(vol:Number) {
        this._mousePoint = new Point(0, 0);
        this._localPoint = new Point(0, 0);
        super();
        this.init();
        this.currentVolume = vol;
        this.draw(0x9B9B9B);
        this._isMouseDown = false;
        this.addEventListener(MouseEvent.MOUSE_DOWN, this.onMouseDown);
        this.addEventListener(MouseEvent.MOUSE_UP, this.onMouseUp);
    }
    private var bar:Shape;
    private var _label:SimpleText;
    private var _isMouseDown:Boolean;
    private var _mousePoint:Point;
    private var _localPoint:Point;

    private var _currentVolume:Number;

    public function get currentVolume():Number {
        return (this._currentVolume);
    }

    public function set currentVolume(vol:Number):void {
        vol = vol > this.MAX ? this.MAX : vol < this.MIN ? this.MIN : vol;
        this._currentVolume = vol;
        this.draw();
    }

    private function init():void {
        this._label = new SimpleText(14, 0xABABAB);
        this._label.autoSize = TextFieldAutoSize.CENTER;
        this._label.text = "Vol:";
        this._label.setBold(true);
        this._label.filters = [new DropShadowFilter(0, 0, 0, 1, 4, 4, 2)];
        this._label.x = -this._label.textWidth;
        this._label.y = -this._label.textHeight;
        addChild(this._label);
        this.bar = new Shape();
        this.bar.x = 20;
        addChild(this.bar);
        graphics.beginFill(0, 0);
        graphics.drawRect(0, -30, 130, 30);
        graphics.endFill();
    }

    private function draw(color:uint = 0x9B9B9B):void {
        var posX:* = this._currentVolume * 100;
        var posY:Number = (posX * -0.2);
        this.bar.graphics.clear();
        this.bar.graphics.lineStyle(2, 0x9B9B9B);
        this.bar.graphics.moveTo(0, 0);
        this.bar.graphics.lineTo(0, -1);
        this.bar.graphics.lineTo(100, -20);
        this.bar.graphics.lineTo(100, 0);
        this.bar.graphics.lineTo(0, 0);
        this.bar.graphics.beginFill(color, 0.8);
        this.bar.graphics.moveTo(0, 0);
        this.bar.graphics.lineTo(0, -1);
        this.bar.graphics.lineTo(posX, posY);
        this.bar.graphics.lineTo(posX, 0);
        this.bar.graphics.lineTo(0, 0);
        this.bar.graphics.endFill();
    }

    private function onMouseDown(e:MouseEvent):void {
        this._isMouseDown = true;
        this.setVolumeToCursor();
        dispatchEvent(new Event(Event.CHANGE, true));
        if (stage) {
            stage.addEventListener(MouseEvent.MOUSE_MOVE, this.onMouseMove);
            stage.addEventListener(MouseEvent.MOUSE_UP, this.onMouseUp);
        }
    }

    private function onMouseUp(e:MouseEvent):void {
        this._isMouseDown = false;
        if (stage) {
            stage.removeEventListener(MouseEvent.MOUSE_MOVE, this.onMouseMove);
        }
    }

    private function onMouseMove(e:MouseEvent):void {
        if (!this._isMouseDown) {
            return;
        }
        this.setVolumeToCursor();
        dispatchEvent(new Event(Event.CHANGE, true));
    }

    private function setVolumeToCursor():void {
        this._mousePoint.x = stage.mouseX;
        this._localPoint = this.globalToLocal(this._mousePoint);
        var x:Number = Math.max(0, Math.min(1, (this._localPoint.x - 20) / 100));
        this.currentVolume = x;
    }
}
}
