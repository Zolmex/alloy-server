package com.company.assembleegameclient.objects.projectiles {
import com.company.assembleegameclient.itemData.ItemData;
import com.company.assembleegameclient.itemData.ProjectileDesc;
import com.company.assembleegameclient.objects.ProjectileProperties;
import com.company.assembleegameclient.util.FreeList;

import flash.geom.Point;
import flash.utils.IDataInput;

public class AmplitudePath extends ProjectilePathSegment
{
    public function AmplitudePath()
    {
        super(AMPLITUDE_PATH);
    }

    public var Amplitude:Number;
    public var Frequency:Number;
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

        var phase:Number = this.info.bulletId_ % 2 == 0?Number(0):Number(Math.PI);
        var deflection:Number = Amplitude * Math.sin(phase + (elapsed / LifetimeMS) * Frequency * 2 * Math.PI);
        p.x = p.x + deflection * Math.cos(angle + Math.PI / 2);
        p.y = p.y + deflection * Math.sin(angle + Math.PI / 2);
        return p;
    }

    public override function Read(data:IDataInput) : void
    {
        Speed = data.readFloat();
        LifetimeMS = data.readInt();
        Angle = data.readFloat();
        this.TimeOffset = data.readInt();
        this.Mods = data.readInt();
        Amplitude = data.readFloat();
        Frequency = data.readFloat();
    }

    public override function Clone() : ProjectilePathSegment
    {
        var path:AmplitudePath = FreeList.newObject(AmplitudePath) as AmplitudePath;
        path.Speed = this.Speed;
        path.Amplitude = this.Amplitude;
        path.Frequency = this.Frequency;
        path.LifetimeMS = this.LifetimeMS;
        path.Angle = this.Angle;
        path.TimeOffset = this.TimeOffset;
        path.Mods = this.Mods;
        return path;
    }

    public function importProps(props:ProjectileProperties):void {
        this.Speed = props.speed_;
        this.Amplitude = props.amplitude_;
        this.Frequency = props.frequency_;
        this.LifetimeMS = props.lifetime_;
    }

    public function importDesc(desc:ProjectileDesc):void {
        this.Speed = desc.Speed;
        this.Amplitude = desc.Amplitude;
        this.Frequency = desc.Frequency;
        this.LifetimeMS = desc.LifetimeMS;
    }
}
}
