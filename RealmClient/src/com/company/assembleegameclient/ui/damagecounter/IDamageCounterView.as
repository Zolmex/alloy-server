package com.company.assembleegameclient.ui.damagecounter {
import flash.display.Sprite;
import flash.utils.Dictionary;

public interface IDamageCounterView {
    function getSprite():Sprite;
    function update(targetId:int, playerDamage:uint, topDamagers:Vector.<Object>):void;
    function dispose():void;
}

}
