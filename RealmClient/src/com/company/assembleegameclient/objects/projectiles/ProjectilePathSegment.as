package com.company.assembleegameclient.objects.projectiles {
import com.company.assembleegameclient.util.FreeList;

import flash.geom.Point;
import flash.utils.IDataInput;

public class ProjectilePathSegment {

    public var pathType_:int;
    public var Speed:Number;
    public var LifetimeMS:int;
    public var TimeOffset:int;
    public var Angle:Number = NaN;
    public var Mods:int;
    public var info:ProjectilePathInfo;

    public function ProjectilePathSegment(pathType:int) {
        pathType_ = pathType;
    }

    public virtual function GetSpeed():Number {
        return this.Speed;
    }

    public virtual function GetAngle():Number {
        if (isNaN(this.Angle)){
            if (this.info == null){
                return 0;
            }
            return this.info.angle_;
        }
        return this.Angle;
    }

    public virtual function PositionAt(elapsed:Number):Point {
        return new Point();
    }

    public function PositionAtEnd():Point {
        return this.PositionAt(this.LifetimeMS);
    }

    public virtual function Read(data:IDataInput):void {

    }

    public virtual function Clone():ProjectilePathSegment {
        return FreeList.newObject(LinePath) as LinePath;
    }

    public virtual function SetInfo(info:ProjectilePathInfo):void {
        this.info = info;
    }

    public function HasMod(modBit:uint):Boolean{
        return (this.Mods & modBit) != 0;
    }

    public function dispose():void {
        this.info = null;
        this.Angle = NaN;
        this.Mods = 0;
        FreeList.deleteObject(this);
    }

    public static const LINE_PATH:int = 0;
    public static const WAVY_PATH:int = 1;
    public static const CIRCLE_PATH:int = 2;
    public static const AMPLITUDE_PATH:int = 3;
    public static const BOOMERANG_PATH:int = 4;
    public static const ACCELERATE_PATH:int = 5;
    public static const DECELERATE_PATH:int = 6;
    public static const CHANGESPEED_PATH:int = 7;
    public static const COMBINED_PATH:int = 8;

    public static const MOD_BOOMERANG_BIT:uint = 1 << 1;
}
}
