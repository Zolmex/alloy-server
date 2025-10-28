package kabam.rotmg.messaging.impl.incoming {
import com.company.assembleegameclient.objects.Projectile;
import com.company.assembleegameclient.objects.projectiles.ProjectilePath;
import com.company.assembleegameclient.util.ConditionEffect;

import flash.utils.IDataInput;

import kabam.rotmg.messaging.impl.data.WorldPosData;

public class EnemyShoot extends IncomingMessage {

    public var bulletId_:uint;
    public var ownerId_:int;
    public var startingPos_:WorldPosData;
    public var angle_:Number;
    public var damage_:uint;
    public var projectileId:int;
    public var projType_:int;
    public var path_:ProjectilePath;
    public var lifetimeMs_:int;
    public var multiHit_:Boolean;
    public var passesCover_:Boolean;
    public var armorPiercing_:Boolean;
    public var size:int;
    public var numShots_:int;
    public var angleInc_:Number;
    public var effects:Vector.<uint>;

    public function EnemyShoot(id:uint, callback:Function) {
        this.startingPos_ = new WorldPosData();
        this.effects = new Vector.<uint>();
        super(id, callback);
    }

    override public function parseFromInput(data:IDataInput):void {
        this.bulletId_ = data.readUnsignedShort();
        this.ownerId_ = data.readInt();
        this.startingPos_.parseFromInput(data);
        this.angle_ = data.readFloat();
        this.damage_ = data.readShort();
        this.projectileId = data.readInt();
        if (this.projectileId == -1) {
            this.projType_ = data.readUnsignedShort();
            this.path_ = ProjectilePath.read(data);
            this.lifetimeMs_ = data.readFloat();
            this.multiHit_ = data.readBoolean();
            this.passesCover_ = data.readBoolean();
            this.armorPiercing_ = data.readBoolean();
            this.size = data.readInt();
            this.effects.length = 0;
            var len:int = data.readUnsignedShort();
            for (var i:int = 0; i < len; i++){
                this.effects.push(data.readUnsignedShort());
            }
        }
        if(data.bytesAvailable > 0)
        {
            this.numShots_ = data.readUnsignedByte();
            this.angleInc_ = data.readFloat();
        }
        else
        {
            this.numShots_ = 1;
            this.angleInc_ = 0;
        }
    }

    override public function toString():String {
        return formatToString("SHOOT", "bulletId_", "ownerId_", "bulletType_", "startingPos_", "angle_", "damage_", "numShots_", "angleInc_");
    }
}
}
