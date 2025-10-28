package com.company.assembleegameclient.objects.projectiles {
import com.company.assembleegameclient.objects.Projectile;
import com.company.assembleegameclient.util.FreeList;

import flash.geom.Point;
import flash.utils.IDataInput;

public class WavyPath extends ProjectilePathSegment
    {
        public function WavyPath()
        {
            super(WAVY_PATH);
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

            var dist:Number = (elapsed * (Speed / 1000.0));
            var phase:Number = this.info.bulletId_ % 2 == 0?Number(0):Number(Math.PI);
            var periodFactor:Number = 6 * Math.PI;
            var amplitudeFactor:Number = Math.PI / 64;
            var angle:Number = GetAngle();
            var theta:Number = angle + amplitudeFactor * Math.sin(phase + periodFactor * elapsed / 1000.0);
            p.x = dist * Math.cos(theta);
            p.y = dist * Math.sin(theta);
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
            var path:WavyPath = FreeList.newObject(WavyPath) as WavyPath;
            path.Speed = this.Speed;
            path.LifetimeMS = this.LifetimeMS;
            path.Angle = this.Angle;
            path.TimeOffset = this.TimeOffset;
            path.Mods = this.Mods;
            return path;
        }
    }
}
