package com.company.assembleegameclient.ui.constellations.misc {
import com.company.assembleegameclient.sound.SoundEffectLibrary;

import flash.display.Sprite;
import flash.events.MouseEvent;
import flash.geom.ColorTransform;
import flash.geom.Point;

import org.osflash.signals.Signal;

public class ConstellationNode extends Sprite {
    public var nodeName:String;
    public var constellation:int;
    public var description:String;
    public var large:Boolean;
    public var row:int;
    public var id:int;

    public function ConstellationNode(name:String, constellation:int, description:String, large:Boolean, row:int, id:int) {
        this.nodeName = name;
        this.constellation = constellation;
        this.description = description;
        this.large = large;
        this.row = row;
        this.id = id;
    }
}
}