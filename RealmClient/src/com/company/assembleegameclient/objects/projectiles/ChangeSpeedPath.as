package com.company.assembleegameclient.objects.projectiles
{
import com.company.assembleegameclient.util.FreeList;

import flash.events.AccelerometerEvent;
import flash.geom.Point;
import flash.utils.IDataInput;

public class ChangeSpeedPath extends ProjectilePathSegment
    {
        public function ChangeSpeedPath()
        {
            super(CHANGESPEED_PATH);
        }

        public var Increment:Number;
        public var Cooldown:int;
        public var CooldownOffset:int;
        public var Repeat:int;

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

            var dist:Number = Math.max(Math.min(elapsed, CooldownOffset), 0) * (Speed / 1000.0);
            if (elapsed > CooldownOffset){
                elapsed -= CooldownOffset;
                var increments:int = Math.min((elapsed)/Cooldown, Repeat);
                for (var i:int = 1; i <= increments; i++){
                    dist += Cooldown * (Speed + i * Increment) / 1000.0;
                }
                var relElapsed:Number = elapsed - (Cooldown * increments);
                dist += relElapsed * (Speed + (increments + 1) * Increment) / 1000.0;
            }
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
            Increment = data.readFloat();
            Cooldown = data.readInt();
            CooldownOffset = data.readInt();
            Repeat = data.readInt();
        }

        public override function Clone() : ProjectilePathSegment
        {
            var path:ChangeSpeedPath = FreeList.newObject(ChangeSpeedPath) as ChangeSpeedPath;
            path.Speed = this.Speed;
            path.Increment = this.Increment;
            path.Cooldown = this.Cooldown;
            path.CooldownOffset = this.CooldownOffset;
            path.Repeat = this.Repeat;
            path.LifetimeMS = this.LifetimeMS;
            path.Angle = this.Angle;
            path.TimeOffset = this.TimeOffset;
            path.Mods = this.Mods;
            return path;
        }
    }
}
