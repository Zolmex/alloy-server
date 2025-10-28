package com.company.assembleegameclient.objects.projectiles {
import com.company.assembleegameclient.objects.Projectile;
import com.company.assembleegameclient.objects.ProjectileProperties;
import com.company.assembleegameclient.objects.projectiles.*;
import com.company.assembleegameclient.util.FreeList;
import com.company.util.Trig;

import flash.geom.Point;
import flash.utils.Dictionary;
import flash.utils.IDataInput;

public class CirclePath extends ProjectilePathSegment
{
    public function CirclePath()
    {
        super(CIRCLE_PATH);
    }

    public var RotationsPerSecond:Number;
    public var Radius:Number;

    public override function PositionAt(elapsed:Number) : Point
    {
        var p:Point = FreeList.newObject(Point) as Point;
        p.x = 0;
        p.y = 0;

        if (this.TimeOffset > 0 && elapsed < this.TimeOffset){
            return p;
        }

        elapsed -= this.TimeOffset;

        if (this.HasMod(MOD_BOOMERANG_BIT)){
            if (elapsed > (LifetimeMS / 2))
                elapsed = LifetimeMS - elapsed;
        }

        var elapsedSeconds:Number = elapsed / 1000.0;
        var angle:Number = GetAngle();
        if (elapsedSeconds != 0)
            angle += (RotationsPerSecond * elapsedSeconds * 360 * Trig.toRadians);
        else {
            angle = 0.0;
        }
        p.x = Math.cos(angle) * Radius;
        p.y = Math.sin(angle) * Radius;
        return p;
    }

    public override function Read(data:IDataInput) : void
    {
        RotationsPerSecond = data.readFloat();
        LifetimeMS = data.readInt();
        Angle = data.readFloat();
        this.TimeOffset = data.readInt();
        this.Mods = data.readInt();
        Radius = data.readFloat();
    }

    public override function GetSpeed() : Number{
        return RotationsPerSecond * 50.0;
    }

    public override function Clone() : ProjectilePathSegment
    {
        var path:CirclePath = FreeList.newObject(CirclePath) as CirclePath;
        path.RotationsPerSecond = this.RotationsPerSecond;
        path.Radius = this.Radius;
        path.LifetimeMS = this.LifetimeMS;
        path.Angle = this.Angle;
        path.TimeOffset = this.TimeOffset;
        path.Mods = this.Mods;
        return path;
    }
}
}
