package com.company.assembleegameclient.objects.projectiles {
import com.company.assembleegameclient.itemData.ItemData;
import com.company.assembleegameclient.itemData.ProjectileDesc;
import com.company.assembleegameclient.objects.Projectile;
import com.company.assembleegameclient.objects.ProjectileProperties;
import com.company.assembleegameclient.objects.projectiles.ProjectilePath;
import com.company.assembleegameclient.util.FreeList;
import com.company.assembleegameclient.util.FreeList;

import flash.geom.Point;

import flash.geom.Point;
import flash.utils.IDataInput;

public class ProjectilePath {

    public var segments:Vector.<ProjectilePathSegment> = new Vector.<ProjectilePathSegment>();
    public var info:ProjectilePathInfo;
    private var lastSegment:ProjectilePathSegment;
    private var currentPoint:Point = new Point();

    public function PositionAt(elapsed:int):Point {
        var segmentEnd:int = 0;
        var segmentsTotal:int = 0;
        this.currentPoint.x = 0;
        this.currentPoint.y = 0;
        for each (var seg:ProjectilePathSegment in this.segments){
            segmentEnd += seg.LifetimeMS; // Time to switch to the next path
            if (elapsed <= segmentEnd){
                var segOffset:Point = seg.PositionAt(elapsed - segmentsTotal);
                this.currentPoint.x += segOffset.x;
                this.currentPoint.y += segOffset.y;
                FreeList.deleteObject(segOffset);
                this.lastSegment = seg;
                return this.currentPoint;
            }

            segOffset = seg.PositionAtEnd(); // Current segment finished, position at the end and update the start position for the next segment
            this.currentPoint.x += segOffset.x;
            this.currentPoint.y += segOffset.y;
            FreeList.deleteObject(segOffset);
            segmentsTotal += seg.LifetimeMS;
        }

        return this.currentPoint;
    }

    public function GetSpeed():Number {
        if (this.lastSegment == null){
            return 0;
        }

        return this.lastSegment.GetSpeed();
    }

    public function addSegment(segment:ProjectilePathSegment):void {
        segment.SetInfo(this.info);
        this.segments.push(segment);
    }

    public function setSegments(segments:Vector.<ProjectilePathSegment>):void {
        for each (var segment:ProjectilePathSegment in segments){
            var newSegment:ProjectilePathSegment = segment.Clone();
            newSegment.SetInfo(this.info);
            this.segments.push(newSegment);
        }
    }

    public function setInfo(info:ProjectilePathInfo):void {
        this.info = info;
        for each (var seg:ProjectilePathSegment in this.segments){
            seg.SetInfo(info);
        }
    }

    public function Clone():ProjectilePath {
        var ret:ProjectilePath = FreeList.newObject(ProjectilePath) as ProjectilePath;
        ret.setSegments(this.segments);
        return ret;
    }

    public function dispose():void {
        this.lastSegment = null;
        for each (var segment:ProjectilePathSegment in this.segments){
            segment.dispose();
        }
        this.segments.length = 0;
        FreeList.deleteObject(this.info);
        this.info = null;
        FreeList.deleteObject(this);
    }

    public static function read(data:IDataInput):ProjectilePath {
        var ret:ProjectilePath = FreeList.newObject(ProjectilePath) as ProjectilePath;
        var segmentCount:int = data.readInt();
        for (var i:int = 0; i < segmentCount; i++){
            var pathType:int = data.readUnsignedByte();
            var segment:ProjectilePathSegment = ProjectilePath.getPath(pathType);
            segment.Read(data);
            ret.addSegment(segment);
        }
        return ret;
    }

    public static function getPath(pathType:int) : ProjectilePathSegment
    {
        switch (pathType)
        {
            case ProjectilePathSegment.LINE_PATH:
                return FreeList.newObject(LinePath) as ProjectilePathSegment;
            case ProjectilePathSegment.WAVY_PATH:
                return FreeList.newObject(WavyPath) as ProjectilePathSegment;
            case ProjectilePathSegment.CIRCLE_PATH:
                return FreeList.newObject(CirclePath) as ProjectilePathSegment;
            case ProjectilePathSegment.AMPLITUDE_PATH:
                return FreeList.newObject(AmplitudePath) as ProjectilePathSegment;
            case ProjectilePathSegment.BOOMERANG_PATH:
                return FreeList.newObject(BoomerangPath) as ProjectilePathSegment;
            case ProjectilePathSegment.ACCELERATE_PATH:
                return FreeList.newObject(AcceleratePath) as ProjectilePathSegment;
            case ProjectilePathSegment.DECELERATE_PATH:
                return FreeList.newObject(DeceleratePath) as ProjectilePathSegment;
            case ProjectilePathSegment.CHANGESPEED_PATH:
                return FreeList.newObject(ChangeSpeedPath) as ProjectilePathSegment;
            case ProjectilePathSegment.COMBINED_PATH:
                return FreeList.newObject(CombinedPath) as ProjectilePathSegment;
        }

        return new LinePath();
    }

    public static function createFromProps(projProps:ProjectileProperties):ProjectilePath {
        var ret:ProjectilePath = FreeList.newObject(ProjectilePath) as ProjectilePath;
        ret.addSegment(getPathFromProps(projProps));
        return ret;
    }

    public static function createFromDesc(projDesc:ProjectileDesc):ProjectilePath {
        if (projDesc.Path != null){
            return projDesc.Path.Clone();
        }

        var ret:ProjectilePath = FreeList.newObject(ProjectilePath) as ProjectilePath;
        ret.addSegment(getPathFromDesc(projDesc));
        return ret;
    }

    public static function createFromXML(xmlList:XMLList):ProjectilePath {
        var ret:ProjectilePath = FreeList.newObject(ProjectilePath) as ProjectilePath;
        for each (var xml:XML in xmlList) {
            ret.addSegment(getPathFromXML(xml));
        }
        return ret;
    }

    public static function getPathFromProps(props:ProjectileProperties):ProjectilePathSegment {
        var ret:ProjectilePathSegment;
        if (props.wavy_) {
            ret = getPath(ProjectilePathSegment.WAVY_PATH);
        }
//        else if (props.parametric_){
//            ret = Projectile.getPath(Projectile.PARAMETRIC_PATH);
//        }
        else if (props.boomerang_) {
            ret = getPath(ProjectilePathSegment.BOOMERANG_PATH);
        } else if (props.amplitude_ != 0 || props.frequency_ != 1) {
            ret = getPath(ProjectilePathSegment.AMPLITUDE_PATH);
            (ret as AmplitudePath).importProps(props);
        } else {
            ret = getPath(ProjectilePathSegment.LINE_PATH);
        }

        ret.Speed = props.speed_;
        ret.LifetimeMS = props.lifetime_;
        return ret;
    }

    public static function getPathFromDesc(desc:ProjectileDesc):ProjectilePathSegment {
        var ret:ProjectilePathSegment;
        if (desc.Wavy) {
            ret = getPath(ProjectilePathSegment.WAVY_PATH);
        }
//        else if (desc.Parametric){
//            ret = Projectile.getPath(Projectile.PARAMETRIC_PATH);
//        }
        else if (desc.Boomerang) {
            ret = getPath(ProjectilePathSegment.BOOMERANG_PATH);
        } else if (desc.Amplitude != 0 || desc.Frequency != 1) {
            ret = getPath(ProjectilePathSegment.AMPLITUDE_PATH);
            (ret as AmplitudePath).importDesc(desc);
        } else {
            ret = getPath(ProjectilePathSegment.LINE_PATH);
        }

        ret.Speed = desc.Speed;
        ret.LifetimeMS = desc.LifetimeMS;
        return ret;
    }

    public  static function getPathFromXML(pathXml:XML):ProjectilePathSegment {
        var pathType:String = pathXml;
        var lifetime:Number = pathXml.@lifetimeMs;
        var speed:Number;
        switch (pathType) {
            case "Line":
                speed = pathXml.@speed;
                var linePath:LinePath = FreeList.newObject(LinePath) as LinePath;
                linePath.Speed = speed;
                linePath.LifetimeMS = lifetime;
                return linePath;
            case "Wavy":
                speed = pathXml.@speed;
                var wavyPath:WavyPath = FreeList.newObject(WavyPath) as WavyPath;
                wavyPath.Speed = speed;
                wavyPath.LifetimeMS = lifetime;
                return wavyPath;
            case "Boomerang":
                speed = pathXml.@speed;
                var boomerangPath:BoomerangPath = FreeList.newObject(BoomerangPath) as BoomerangPath;
                boomerangPath.Speed = speed;
                boomerangPath.LifetimeMS = lifetime;
                return boomerangPath;
            case "Circle":
                var rps:Number = pathXml.@rotationsPerSecond;
                var radius:Number = pathXml.@radius;
                var circlePath:CirclePath = FreeList.newObject(CirclePath) as CirclePath;
                circlePath.RotationsPerSecond = rps;
                circlePath.Radius = radius;
                circlePath.LifetimeMS = lifetime;
                return circlePath;
            case "Amplitude":
                speed = pathXml.@speed;
                var amplitude:Number = pathXml.@amplitude;
                var frequency:Number = pathXml.@frequency;
                var ampPath:AmplitudePath = FreeList.newObject(AmplitudePath) as AmplitudePath;
                ampPath.Speed = speed;
                ampPath.Amplitude = amplitude;
                ampPath.Frequency = frequency;
                ampPath.LifetimeMS = lifetime;
                return ampPath;
            case "Accelerate":
                speed = pathXml.@speed;
                var acceleratePath:AcceleratePath = FreeList.newObject(AcceleratePath) as AcceleratePath;
                acceleratePath.Speed = speed;
                acceleratePath.LifetimeMS = lifetime;
                return acceleratePath;
            case "Decelerate":
                speed = pathXml.@speed;
                var deceleratePath:DeceleratePath = FreeList.newObject(DeceleratePath) as DeceleratePath;
                deceleratePath.Speed = speed;
                deceleratePath.LifetimeMS = lifetime;
                return deceleratePath;
            case "ChangeSpeed":
                speed = pathXml.@speed;
                var inc:Number = pathXml.@inc;
                var cooldown:int = pathXml.@cooldown;
                var cooldownOffset:int = pathXml.@cooldownOffset;
                var repeat:int = pathXml.@repeat;
                var changeSpeedPath:ChangeSpeedPath = FreeList.newObject(ChangeSpeedPath) as ChangeSpeedPath;
                changeSpeedPath.Speed = speed;
                changeSpeedPath.LifetimeMS = lifetime;
                changeSpeedPath.Increment = inc;
                changeSpeedPath.Cooldown = cooldown;
                changeSpeedPath.CooldownOffset = cooldownOffset;
                changeSpeedPath.Repeat = repeat;
                return changeSpeedPath;
        }
        var path:ProjectilePathSegment;
        path = new LinePath();
        path.Speed = 10;
        path.LifetimeMS = 100;
        return path;
    }
}
}
