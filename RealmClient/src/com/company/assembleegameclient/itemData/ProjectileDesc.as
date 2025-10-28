package com.company.assembleegameclient.itemData {
import com.company.assembleegameclient.objects.projectiles.ProjectilePath;

import flash.utils.ByteArray;

public class ProjectileDesc {

    public var BulletId:int;
    public var ObjectId:String;
    public var Speed:Number;
    public var Damage:int;
    public var MinDamage:int;
    public var MaxDamage:int;
    public var LifetimeMS:Number;
    public var MultiHit:Boolean;
    public var PassesCover:Boolean;
    public var Parametric:Boolean;
    public var Boomerang:Boolean;
    public var ArmorPiercing:Boolean;
    public var Size:int;
    public var Wavy:Boolean;
    public var Effects:Vector.<ConditionEffectDesc>;
    public var Amplitude:Number;
    public var Frequency:Number;
    public var Magnitude:Number;
    public var Sound:String;
    public var LevelIncrease:LevelIncreaseDesc;
    public var Path:ProjectilePath;

    public function ProjectileDesc(obj:*) {
        if (!obj)
            return;
        this.BulletId = ItemData.GetValue(obj, "@id", 0);
        this.ObjectId = ItemData.GetValue(obj, "ObjectId", null);
        this.Speed = ItemData.GetValue(obj, "Speed", 0) / 10;
        if (ItemData.HasValue(obj, "Damage")) {
            this.Damage = ItemData.GetValue(obj, "Damage", 0);
            this.MinDamage = this.Damage;
            this.MaxDamage = this.Damage;
        } else {
            this.MinDamage = ItemData.GetValue(obj, "MinDamage", 0);
            this.MaxDamage = ItemData.GetValue(obj, "MaxDamage", 0);
        }
        this.LifetimeMS = ItemData.GetValue(obj, "LifetimeMS", 0);
        this.MultiHit = ItemData.GetValue(obj, "MultiHit", false);
        this.PassesCover = ItemData.GetValue(obj, "PassesCover", false);
        this.Parametric = ItemData.GetValue(obj, "Parametric", false);
        this.Boomerang = ItemData.GetValue(obj, "Boomerang", false);
        this.ArmorPiercing = ItemData.GetValue(obj, "ArmorPiercing", false);
        this.Size = ItemData.GetValue(obj, "Size", 0);
        this.Wavy = ItemData.GetValue(obj, "Wavy", false);
        if (ItemData.HasValue(obj, "ConditionEffect")) {
            this.Effects = new Vector.<ConditionEffectDesc>();
            for each (var eff:* in ItemData.GetValue(obj, "ConditionEffect", null)) {
                this.Effects.push(new ConditionEffectDesc(ItemData.GetValue(eff, "", 0), ItemData.GetValue(eff, "DurationMS/@duration", 0)));
            }
        }
        this.Amplitude = ItemData.GetValue(obj, "Amplitude", 0);
        this.Frequency = ItemData.GetValue(obj, "Frequency", 1);
        this.Magnitude = ItemData.GetValue(obj, "Magnitude", 3);
        this.Sound = ItemData.GetValue(obj, "Sound", null);
        if (ItemData.HasValue(obj, "LevelIncrease")) {
            this.LevelIncrease = new LevelIncreaseDesc(ItemData.GetValue(obj, "LevelIncrease", null));
        }
        if (ItemData.HasValue(obj, "Path")) {
            this.Path = ProjectilePath.createFromXML(ItemData.GetValue(obj, "Path", null));
        }
        else{
            this.Path = ProjectilePath.createFromDesc(this);
        }
    }

    public function ParseData(itemDatas:ByteArray):void {
        var fieldCount:int = itemDatas.readByte();
        for (var i:int = 0; i < fieldCount; i++) {
            var field:int = itemDatas.readByte();
            switch (field) {
                case 0:
                    this.BulletId = itemDatas.readByte();
                    break;
                case 1:
                    this.ObjectId = itemDatas.readUTF();
                    break;
                case 2:
                    this.LifetimeMS = itemDatas.readInt();
                    break;
                case 3:
                    this.Speed = itemDatas.readInt();
                    break;
                case 4:
                    this.Damage = itemDatas.readInt();
                    break;
                case 5:
                    this.MinDamage = itemDatas.readInt();
                    break;
                case 6:
                    this.MaxDamage = itemDatas.readInt();
                    break;
                case 7:
                    var effects:Vector.<ConditionEffectDesc> = new Vector.<ConditionEffectDesc>();
                    var len:int = itemDatas.readUnsignedShort();
                    for (i = 0; i < len; i++) {
                        var effId:int = itemDatas.readByte();
                        var duration:int = itemDatas.readInt();
                        effects.push(new ConditionEffectDesc(null, duration));
                    }
                    this.Effects = effects;
                    break;
                case 8:
                    this.MultiHit = itemDatas.readBoolean();
                    break;
                case 9:
                    this.PassesCover = itemDatas.readBoolean();
                    break;
                case 10:
                    this.ArmorPiercing = itemDatas.readBoolean();
                    break;
                case 11:
                    this.Wavy = itemDatas.readBoolean();
                    break;
                case 12:
                    this.Parametric = itemDatas.readBoolean();
                    break;
                case 13:
                    this.Boomerang = itemDatas.readBoolean();
                    break;
                case 14:
                    this.Amplitude = itemDatas.readFloat();
                    break;
                case 15:
                    this.Frequency = itemDatas.readFloat();
                    break;
                case 16:
                    this.Magnitude = itemDatas.readFloat();
                    break;
                case 17:
                    this.Size = itemDatas.readInt();
                    break;
                case 18:
                    if (!this.LevelIncrease) {
                        return;
                    }
                    this.LevelIncrease.ParseData(itemDatas);
                    break;
            }
        }
    }
}
}
