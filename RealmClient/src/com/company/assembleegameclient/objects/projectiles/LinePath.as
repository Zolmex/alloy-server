package com.company.assembleegameclient.objects.projectiles
{
import com.company.assembleegameclient.util.FreeList;

import flash.geom.Point;
import flash.utils.IDataInput;

public class LinePath extends ProjectilePathSegment
    {
        public function LinePath()
        {
            super(LINE_PATH);
        }

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

            var dist:Number = elapsed * (Speed / 1000.0);
            var angle:Number = GetAngle();
            p.x = dist * Math.cos(angle);
            p.y = dist * Math.sin(angle);
            return p;
        }

        public override function Read(data:IDataInput) : void
        {
            Speed = data.readFloat();
            LifetimeMS = data.readInt();
            Angle = data.readFloat();
            this.TimeOffset = data.readInt();
            this.Mods = data.readInt();
        }

        public override function Clone() : ProjectilePathSegment
        {
            var path:LinePath = FreeList.newObject(LinePath) as LinePath;
            path.Speed = this.Speed;
            path.LifetimeMS = this.LifetimeMS;
            path.Angle = this.Angle;
            path.TimeOffset = this.TimeOffset;
            path.Mods = this.Mods;
            return path;
        }
    }
}
