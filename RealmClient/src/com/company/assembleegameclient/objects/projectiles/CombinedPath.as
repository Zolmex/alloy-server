package com.company.assembleegameclient.objects.projectiles {
import com.company.assembleegameclient.objects.Projectile;
import com.company.assembleegameclient.util.FreeList;

import flash.geom.Point;
import flash.utils.IDataInput;

public class CombinedPath extends ProjectilePathSegment
    {
        private var segments:Vector.<ProjectilePathSegment>;

        public function CombinedPath()
        {
            super(COMBINED_PATH);
            this.segments = new Vector.<ProjectilePathSegment>();
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

            var deltaX:Number = 0;
            var deltaY:Number = 0;

            var count:int = 0;
            for each (var segment:ProjectilePathSegment in this.segments){
                if (segment.TimeOffset > 0 && elapsed < segment.TimeOffset){
                    continue;
                }

                var segOffset:Point = segment.PositionAt(elapsed);
                deltaX += segOffset.x;
                deltaY += segOffset.y;
                FreeList.deleteObject(segOffset);
                count++;
            }

            p.x += deltaX / count;
            p.y += deltaY / count;
            return p;
        }

        public override function SetInfo(info:ProjectilePathInfo):void{
            this.info = info;

            for each (var segment:ProjectilePathSegment in this.segments){
                segment.SetInfo(info);
            }
        }

        public override function Read(data:IDataInput) : void
        {
            var len:int = data.readUnsignedByte();
            this.segments.length = 0;
            for (var i:int = 0; i < len; i++){
                var pathType:int = data.readUnsignedByte();
                var segment:ProjectilePathSegment = ProjectilePath.getPath(pathType);
                segment.Read(data);
                this.segments.push(segment);
            }
            this.TimeOffset = data.readInt();
            this.Mods = data.readInt();

            var maxLifetime:int = 0;
            for each (segment in this.segments){
                if (segment.TimeOffset + segment.LifetimeMS > maxLifetime){
                    maxLifetime = segment.TimeOffset + segment.LifetimeMS;
                }
            }

            this.LifetimeMS = maxLifetime;
        }

        public override function Clone() : ProjectilePathSegment
        {
            var path:CombinedPath = FreeList.newObject(CombinedPath) as CombinedPath;
            path.Speed = this.Speed;
            path.LifetimeMS = this.LifetimeMS;
            path.Angle = this.Angle;
            path.TimeOffset = this.TimeOffset;
            path.Mods = this.Mods;
            path.segments = new Vector.<ProjectilePathSegment>();
            for (var i:int = 0; i < this.segments.length; i++){
                path.segments.push(this.segments[i].Clone());
            }
            return path;
        }
    }
}
