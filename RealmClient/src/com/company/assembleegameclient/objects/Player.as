package com.company.assembleegameclient.objects {
import com.adobe.serialization.json.JSON;
import com.company.assembleegameclient.game.MapUserInput;
import com.company.assembleegameclient.itemData.ProjectileDesc;
import com.company.assembleegameclient.itemData.QuiverDesc;
import com.company.assembleegameclient.objects.projectiles.*;
import com.company.assembleegameclient.itemData.ActivateEffect;
import com.company.assembleegameclient.itemData.ItemData;
import com.company.assembleegameclient.map.Camera;
import com.company.assembleegameclient.map.Square;
import com.company.assembleegameclient.map.mapoverlay.CharacterStatusText;
import com.company.assembleegameclient.objects.particles.HealingEffect;
import com.company.assembleegameclient.objects.particles.LevelUpEffect;
import com.company.assembleegameclient.parameters.Parameters;
import com.company.assembleegameclient.sound.SoundEffectLibrary;
import com.company.assembleegameclient.util.AnimatedChar;
import com.company.assembleegameclient.util.ConditionEffect;
import com.company.assembleegameclient.util.FameUtil;
import com.company.assembleegameclient.util.FreeList;
import com.company.assembleegameclient.util.MaskedImage;
import com.company.assembleegameclient.util.TextureRedrawer;
import com.company.assembleegameclient.util.redrawers.GlowRedrawer;
import com.company.ui.SimpleText;
import com.company.util.CachingColorTransformer;
import com.company.util.ConversionUtil;
import com.company.util.GraphicsUtil;
import com.company.util.IntPoint;
import com.company.util.MoreColorUtil;
import com.company.util.PointUtil;
import com.company.util.Trig;

import flash.display.BitmapData;
import flash.display.GraphicsBitmapFill;
import flash.display.GraphicsPath;
import flash.display.GraphicsSolidFill;
import flash.display.GraphicsStroke;
import flash.display.IGraphicsData;
import flash.display.Sprite;
import flash.filters.GlowFilter;
import flash.geom.ColorTransform;
import flash.geom.Matrix;
import flash.geom.Point;
import flash.utils.ByteArray;
import flash.utils.Dictionary;
import flash.utils.getTimer;

import kabam.rotmg.assets.services.CharacterFactory;
import kabam.rotmg.constants.ActivationType;
import kabam.rotmg.constants.GeneralConstants;
import kabam.rotmg.core.StaticInjectorContext;
import kabam.rotmg.game.model.AddTextLineVO;
import kabam.rotmg.game.model.PotionInventoryModel;
import kabam.rotmg.game.signals.AddTextLineSignal;
import kabam.rotmg.messaging.impl.GameServerConnection;
import kabam.rotmg.messaging.impl.GameServerConnection;
import kabam.rotmg.messaging.impl.data.WorldPosData;
import kabam.rotmg.stage3D.GraphicsFillExtra;
import kabam.rotmg.ui.model.TabStripModel;

import org.hamcrest.core.isA;

import org.swiftsuspenders.Injector;

public class Player extends Character {

    public static const MS_BETWEEN_TELEPORT:int = 10000;
    private static const MOVE_THRESHOLD:Number = 0.4;
    private static const NEARBY:Vector.<Point> = new <Point>[new Point(0, 0), new Point(1, 0), new Point(0, 1), new Point(1, 1)];

    private static var newP:Point = new Point();

    private static const RANK_OFFSET_MATRIX:Matrix = new Matrix(1, 0, 0, 1, 2, 4);
    private static const NAME_OFFSET_MATRIX:Matrix = new Matrix(1, 0, 0, 1, 20, 0);

    private static const LOW_HEALTH_CT_OFFSET:int = 128;
    private static var lowHealthCT:Dictionary = new Dictionary();
    public const levelCap:int = 30;

    public var skinId:int;
    public var skin:AnimatedChar;
    public var accountId_:int = -1;
    public var credits_:int = 0;
    public var numStars_:int = 0;
    public var fame_:int = 0;
    public var charFame_:int = 0;
    public var nextClassQuestFame_:int = -1;
    public var legendaryRank_:int = -1;
    public var guildName_:String = null;
    public var guildRank_:int = -1;
    public var isFellowGuild_:Boolean = false;
    public var oxygen_:int = -1;
    public var nextLevelXp_:int = 1000;
    public var experience_:int = 0;
    public var healthPotionCount_:int = 0;
    public var magicPotionCount_:int = 0;
    public var hasBackpack_:Boolean = false;
    public var starred_:Boolean = false;
    public var ignored_:Boolean = false;
    public var distSqFromThisPlayer_:Number = 0;
    protected var rotate_:Number = 0;
    protected var relMoveVec_:Point = null;
    public var moveMultiplier_:Number = 1;
    public var attackPeriod_:int = 0;
    public var nextAltAttack_:int = 0;
    public var nextTeleportAt_:int = 0;
    protected var healingEffect_:HealingEffect = null;
    protected var nearestMerchant_:Merchant = null;
    public var isDefaultAnimatedChar:Boolean = true;
    private var addTextLine:AddTextLineSignal;
    private var factory:CharacterFactory;
    private var ip_:IntPoint;
    private var breathBackFill_:GraphicsSolidFill = null;
    private var breathBackPath_:GraphicsPath = null;
    private var breathFill_:GraphicsSolidFill = null;
    private var breathPath_:GraphicsPath = null;
    private var hallucinatingMaskedImage_:MaskedImage = null;
    private var nextProjectileId:int = 0;
    public var timeInCombat:int = 0;
    public var partyId:int = -1;

    public var primaryConstellation:int = -1;
    public var secondaryConstellation:int = -1;
    public var primaryNodeData:int = -1
    public var secondaryNodeData:int = -1
    public var primaryNodes:Vector.<int>;
    public var secondaryNodes:Vector.<int>;

    // Stats
    public var maxMP:int = 200;
    public var mp:Number = 0;

    public var attack:int = 0;
    public var defense:int = 0;
    public var dexterity:int = 0;
    public var wisdom:int = 0;

    public var movementSpeed:Number = 0;
    public var lifeRegeneration:int = 0;
    public var dodgeChance:Number = 0;
    public var criticalChance:Number = 0;
    public var criticalDamage:int = 0;
    public var manaRegeneration:int = 0;
    public var msRegenRate:int = 0;
    public var damageMultiplier:int = 0;
    public var attackSpeed:Number = 0;

    // Ability stat helpers
    public var abilityDataA:String;
    public var abilityDataB:String;
    public var abilityDataC:String;
    public var abilityDataD:String;

    // Ninja sheath
    private var wellBarBackFill_:GraphicsSolidFill = null;
    private var wellBarBackPath_:GraphicsPath = null;
    private var wellBarFill_:GraphicsSolidFill = null;
    private var wellBarPath_:GraphicsPath = null;

    // Archer quiver
    private var quiverBackFill_:GraphicsSolidFill = new GraphicsSolidFill(5526612);
    private var quiverBackPath_:GraphicsPath = new GraphicsPath(new Vector.<int>(), new Vector.<Number>());
    private var quiverArrowsFill_:GraphicsSolidFill = new GraphicsSolidFill(0x910c0c);
    private var quiverArrowsPath_:GraphicsPath = new GraphicsPath(new Vector.<int>(), new Vector.<Number>());
    private var quiverMpArrowsFill_:GraphicsSolidFill = new GraphicsSolidFill(0x0c6091);
    private var quiverMpArrowsPath_:GraphicsPath = new GraphicsPath(new Vector.<int>(), new Vector.<Number>());

    private var warriorStacksText:SimpleText = new SimpleText(15, 0xFFFFFF)
    private var warriorTextBitmap:BitmapData = null;
    private var warriorBitmapFill:GraphicsBitmapFill = new GraphicsBitmapFill(null, new Matrix(), false, false);
    private var warriorPath:GraphicsPath = new GraphicsPath(GraphicsUtil.QUAD_COMMANDS, new Vector.<Number>());
    private var warriorArcPath:GraphicsPath = new GraphicsPath();
    private var warriorArcFill:GraphicsSolidFill = new GraphicsSolidFill(0xff0000); // rouge
    private var warriorArcStroke:GraphicsStroke = new GraphicsStroke(3, false, "normal", "none", "round", 3, warriorArcFill);

    public function Player(objectXML:XML) {
        this.ip_ = new IntPoint();
        var injector:Injector = StaticInjectorContext.getInjector();
        this.addTextLine = injector.getInstance(AddTextLineSignal);
        this.factory = injector.getInstance(CharacterFactory);
        super(objectXML);
        texturingCache_ = new Dictionary();
    }

    public static function fromPlayerXML(name:String, playerXML:XML):Player {
        var objectType:int = int(playerXML.ObjectType);
        var objXML:XML = ObjectLibrary.xmlLibrary_[objectType];
        var player:Player = new Player(objXML);
        player.name_ = name;
        player.level_ = int(playerXML.Level);
        player.statPoints_ = int(playerXML.StatPoints);
        player.experience_ = int(playerXML.Experience);
        player.nextLevelXp_ = int(playerXML.NextLevelXp);
        player.nextClassQuestFame_ = int(playerXML.NextClassQuestFame);
        player.itemTypes = ConversionUtil.toIntVector(playerXML.Equipment);
        player.equipment_ = new Vector.<ItemData>();
//        trace(playerXML.ItemDatas);
        var dataStream:ByteArray = ItemData.GetStream(playerXML.ItemDatas);
        for (var i:int = 0; i < player.itemTypes.length; i++) {
            var itemType:int = player.itemTypes[i];
            if (itemType == -1) {
                dataStream.readByte(); // Read byte 0
                player.equipment_.push(null);
                continue;
            }

            var item:ItemData = new ItemData(itemType);
            item.ParseData(dataStream);
            player.equipment_.push(item);
        }
        player.maxHP = int(playerXML.MaxHitPoints);
        player.hp = int(playerXML.HitPoints);
        player.maxMP = int(playerXML.MaxMagicPoints);
        player.mp = int(playerXML.MagicPoints);
        player.maxMS = int(playerXML.MaxMagicShield);
        player.ms = int(playerXML.MagicShield);
        player.attack = int(playerXML.Attack);
        player.defense = int(playerXML.Defense);
        player.dexterity = int(playerXML.Dexterity);
        player.wisdom = int(playerXML.Wisdom);
        player.movementSpeed = Number(playerXML.MovementSpeed);
        player.lifeRegeneration = int(playerXML.LifeRegeneration);
        player.dodgeChance = Number(playerXML.DodgeChance);
        player.criticalChance = Number(playerXML.CriticalChance);
        player.criticalDamage = int(playerXML.CriticalDamage);
        player.manaRegeneration = int(playerXML.ManaRegeneration);
        player.msRegenRate = int(playerXML.MagicShieldRegenRate);
        player.damageMultiplier = int(playerXML.DamageMultiplier);
        player.armor = int(playerXML.Armor);
        player.attackSpeed = Number(playerXML.AttackSpeed);
        player.tex1Id_ = int(playerXML.Tex1);
        player.tex2Id_ = int(playerXML.Tex2);
        player.primaryConstellation = int(playerXML.PrimaryConstellation);
        player.secondaryConstellation = int(playerXML.SecondaryConstellation);
        player.primaryNodeData = int(playerXML.PrimaryNodeData);
        player.secondaryNodeData = int(playerXML.SecondaryNodeData);
        player.primaryNodes = convertNodeData(player.primaryNodeData);
        player.secondaryNodes = convertNodeData(player.secondaryNodeData);
        return player;
    }

    public static function convertNodeData(data:int):Vector.<int> {
        var def:Vector.<int> = new Vector.<int>(4, true);
        def[0] = -1;
        def[1] = -1;
        def[2] = -1;
        def[3] = -1;
        if (data == -1)
            return def;

        var newStr:String = data.toString();

        for (var i:int = 0; i < newStr.length; i++) {
            def[i] = parseInt(newStr.charAt(i));
        }
        return def;
    }

    public function setRelativeMovement(rotate:Number, relMoveVecX:Number, relMoveVecY:Number):void {
        var temp:Number = NaN;
        if (this.relMoveVec_ == null) {
            this.relMoveVec_ = new Point();
        }
        this.rotate_ = rotate;
        this.relMoveVec_.x = relMoveVecX;
        this.relMoveVec_.y = relMoveVecY;
        if (isConfused()) {
            temp = this.relMoveVec_.x;
            this.relMoveVec_.x = -this.relMoveVec_.y;
            this.relMoveVec_.y = -temp;
            this.rotate_ = -this.rotate_;
        }
    }

    public function setCredits(credits:int):void {
        this.credits_ = credits;
    }

    public function setGuildName(guildName:String):void {
        var go:GameObject = null;
        var player:Player = null;
        var isFellowGuild:Boolean = false;
        this.guildName_ = guildName;
        var myPlayer:Player = map_.player_;
        if (myPlayer == this) {
            for each(go in map_.goDict_) {
                player = go as Player;
                if (player != null && player != this) {
                    player.setGuildName(player.guildName_);
                }
            }
        } else {
            isFellowGuild = myPlayer != null && myPlayer.guildName_ != null && myPlayer.guildName_ != "" && myPlayer.guildName_ == this.guildName_;
            if (isFellowGuild != this.isFellowGuild_) {
                this.isFellowGuild_ = isFellowGuild;
                nameBitmapData_ = null;
            }
        }
    }

    public function isTeleportEligible(player:Player):Boolean {
        return !player.isInvisible();
    }

    public function msUtilTeleport():int {
        var time:int = map_.gs_.lastUpdate_;
        return Math.max(0, this.nextTeleportAt_ - time);
    }

    public function teleportTo(player:Player):Boolean {
        var msUtil:int = this.msUtilTeleport();
        if (msUtil > 0) {
            this.addTextLine.dispatch(new AddTextLineVO(Parameters.ERROR_CHAT_NAME, "You can not teleport for another " + int(msUtil / 1000 + 1) + " seconds."));
            return false;
        }
        if (!this.isTeleportEligible(player)) {
            if (player.isInvisible()) {
                this.addTextLine.dispatch(new AddTextLineVO(Parameters.ERROR_CHAT_NAME, "Can not teleport to " + player.name_ + " while they are invisible."));
            }
            this.addTextLine.dispatch(new AddTextLineVO(Parameters.ERROR_CHAT_NAME, "Can not teleport to " + player.name_));
            return false;
        }
        map_.gs_.gsc_.teleport(player.objectId_);
        this.nextTeleportAt_ = map_.gs_.lastUpdate_ + MS_BETWEEN_TELEPORT;
        return true;
    }

    public function levelUpEffect(text:String):void {
        this.levelUpParticleEffect();
        map_.mapOverlay_.addStatusText(new CharacterStatusText(this, text, 65280, 2000));
    }

    public function handleLevelUp():void {
        SoundEffectLibrary.play("level_up");
        this.levelUpEffect("Level Up!");
    }

    public function levelUpParticleEffect():void {
        map_.addObj(new LevelUpEffect(this, 4278255360, 20), x_, y_);
    }

    public function handleExpUp(xp:int):void {
        if (level_ == levelCap) {
            return;
        }
        map_.mapOverlay_.addStatusText(new CharacterStatusText(this, "+" + xp + " XP", 65280, 1000));
    }

    public function handleFameUp(fame:int):void {
        if (level_ != levelCap) {
            return;
        }
        map_.mapOverlay_.addStatusText(new CharacterStatusText(this, "+" + fame + " Fame", 0xE25F00, 1000));
    }

    private function getNearbyMerchant():Merchant {
        for (var ip:IntPoint in map_.merchLookup_) {
            var m:Merchant = map_.merchLookup_[ip];
            if (m == null) {
                continue;
            }

            if (PointUtil.distanceSquaredXY(ip.x_, ip.y_, x_, y_) < 1) {
                return m;
            }
        }
        return null;
    }

    public function walkTo(x:Number, y:Number):Boolean {
        if (isNaN(x) || isNaN(y)) //Rarely happens during Reconnecting? Freezes client, temp fix (or perm fix?).
        {
            return false;
        }
        this.modifyMove(x, y, newP);
        return this.moveTo(newP.x, newP.y);
    }

    override public function moveTo(x:Number, y:Number):Boolean {
        var ret:Boolean = super.moveTo(x, y);
        if (map_.gs_.isNexus_) {
            this.nearestMerchant_ = this.getNearbyMerchant();
        }
        return ret;
    }

    public function modifyMove(x:Number, y:Number, newP:Point):void {
        if (isParalyzed()) {
            newP.x = x_;
            newP.y = y_;
            return;
        }
        var dx:Number = x - x_;
        var dy:Number = y - y_;
        if (dx < MOVE_THRESHOLD && dx > -MOVE_THRESHOLD && dy < MOVE_THRESHOLD && dy > -MOVE_THRESHOLD) {
            this.modifyStep(x, y, newP);
            return;
        }
        var stepSize:Number = MOVE_THRESHOLD / Math.max(Math.abs(dx), Math.abs(dy));
        var d:Number = 0;
        newP.x = x_;
        newP.y = y_;
        var done:Boolean = false;
        while (!done) {
            if (d + stepSize >= 1) {
                stepSize = 1 - d;
                done = true;
            }
            this.modifyStep(newP.x + dx * stepSize, newP.y + dy * stepSize, newP);
            d = d + stepSize;
        }
    }

    public function modifyStep(x:Number, y:Number, newP:Point):void {
        var nextXBorder:Number = NaN;
        var nextYBorder:Number = NaN;
        var xCross:Boolean = x_ % 0.5 == 0 && x != x_ || int(x_ / 0.5) != int(x / 0.5);
        var yCross:Boolean = y_ % 0.5 == 0 && y != y_ || int(y_ / 0.5) != int(y / 0.5);
        if (!xCross && !yCross || this.isValidPosition(x, y)) {
            newP.x = x;
            newP.y = y;
            return;
        }
        if (xCross) {
            nextXBorder = x > x_ ? Number(int(x * 2) / 2) : Number(int(x_ * 2) / 2);
            if (int(nextXBorder) > int(x_)) {
                nextXBorder = nextXBorder - 0.01;
            }
        }
        if (yCross) {
            nextYBorder = y > y_ ? Number(int(y * 2) / 2) : Number(int(y_ * 2) / 2);
            if (int(nextYBorder) > int(y_)) {
                nextYBorder = nextYBorder - 0.01;
            }
        }
        if (!xCross) {
            newP.x = x;
            newP.y = nextYBorder;
            return;
        }
        if (!yCross) {
            newP.x = nextXBorder;
            newP.y = y;
            return;
        }
        var xBorderDist:Number = x > x_ ? Number(x - nextXBorder) : Number(nextXBorder - x);
        var yBorderDist:Number = y > y_ ? Number(y - nextYBorder) : Number(nextYBorder - y);
        if (xBorderDist > yBorderDist) {
            if (this.isValidPosition(x, nextYBorder)) {
                newP.x = x;
                newP.y = nextYBorder;
                return;
            }
            if (this.isValidPosition(nextXBorder, y)) {
                newP.x = nextXBorder;
                newP.y = y;
                return;
            }
        } else {
            if (this.isValidPosition(nextXBorder, y)) {
                newP.x = nextXBorder;
                newP.y = y;
                return;
            }
            if (this.isValidPosition(x, nextYBorder)) {
                newP.x = x;
                newP.y = nextYBorder;
                return;
            }
        }
        newP.x = nextXBorder;
        newP.y = nextYBorder;
    }

    public function isValidPosition(x:Number, y:Number):Boolean {
        var square:Square = map_.getSquare(x, y);
        if (square_ != square && (square == null || !square.isWalkable())) {
            return false;
        }
        var xFrac:Number = x - int(x);
        var yFrac:Number = y - int(y);
        if (xFrac < 0.5) {
            if (this.isFullOccupy(x - 1, y)) {
                return false;
            }
            if (yFrac < 0.5) {
                if (this.isFullOccupy(x, y - 1) || this.isFullOccupy(x - 1, y - 1)) {
                    return false;
                }
            } else if (yFrac > 0.5) {
                if (this.isFullOccupy(x, y + 1) || this.isFullOccupy(x - 1, y + 1)) {
                    return false;
                }
            }
        } else if (xFrac > 0.5) {
            if (this.isFullOccupy(x + 1, y)) {
                return false;
            }
            if (yFrac < 0.5) {
                if (this.isFullOccupy(x, y - 1) || this.isFullOccupy(x + 1, y - 1)) {
                    return false;
                }
            } else if (yFrac > 0.5) {
                if (this.isFullOccupy(x, y + 1) || this.isFullOccupy(x + 1, y + 1)) {
                    return false;
                }
            }
        } else if (yFrac < 0.5) {
            if (this.isFullOccupy(x, y - 1)) {
                return false;
            }
        } else if (yFrac > 0.5) {
            if (this.isFullOccupy(x, y + 1)) {
                return false;
            }
        }
        return true;
    }

    public function isFullOccupy(x:Number, y:Number):Boolean {
        var square:Square = map_.lookupSquare(x, y);
        return square == null || square.tileType_ == 255 || square.obj_ != null && square.obj_.props_.fullOccupy_;
    }

    override public function update(time:int, dt:int):Boolean {
        var playerAngle:Number = NaN;
        var moveSpeed:Number = NaN;
        var moveVecAngle:Number = NaN;
        var d:int = 0;

        if (isHealing() && Parameters.data_.particles) {
            if (this.healingEffect_ == null) {
                this.healingEffect_ = new HealingEffect(this);
                map_.addObj(this.healingEffect_, x_, y_);
            }
        } else if (this.healingEffect_ != null) {
            map_.removeObj(this.healingEffect_.objectId_);
            this.healingEffect_ = null;
        }
        if (this.relMoveVec_ != null) {
            playerAngle = Parameters.data_.cameraAngle;
            if (this.rotate_ != 0) {
                playerAngle = playerAngle + dt * Parameters.PLAYER_ROTATE_SPEED * this.rotate_;
                Parameters.data_.cameraAngle = playerAngle;
            }
            if (this.relMoveVec_.x != 0 || this.relMoveVec_.y != 0) {
                moveSpeed = getMoveSpeed() * (dt / 1000.0); //testing tile per second
                moveVecAngle = Math.atan2(this.relMoveVec_.y, this.relMoveVec_.x);
                direction_.x = moveSpeed * Math.cos(playerAngle + moveVecAngle);
                direction_.y = moveSpeed * Math.sin(playerAngle + moveVecAngle);
            } else {
                direction_.x = 0;
                direction_.y = 0;
            }
            if (map_.pushX_ != 0 || map_.pushY_ != 0) {
                direction_.x = direction_.x - map_.pushX_;
                direction_.y = direction_.y - map_.pushY_;
            }
            this.walkTo(x_ + direction_.x, y_ + direction_.y);
        } else if (!super.update(time, dt)) {
            return false;
        }
        return true;
    }

    public function onMove():void {
        var square:Square = map_.getSquare(x_, y_);
        if (square.props_.sinking_) {
            sinkLevel_ = Math.min(sinkLevel_ + 1, Parameters.MAX_SINK_LEVEL);
            this.moveMultiplier_ = 0.1 + (1 - sinkLevel_ / Parameters.MAX_SINK_LEVEL) * (square.props_.speed_ - 0.1);
        } else {
            sinkLevel_ = 0;
            this.moveMultiplier_ = square.props_.speed_;
        }

        if (square.props_.damage_ > 0 && !isInvincible()) {
            if (square_.obj_ == null || !square_.obj_.props_.protectFromGroundDamage_) {
                damage(square.props_.damage_, null, null);
            }
        }

        if (square_.props_.push_) {
            map_.pushX_ = square_.props_.animate_.dx_ / 1000;
            map_.pushY_ = square_.props_.animate_.dy_ / 1000;
        } else {
            map_.pushX_ = 0;
            map_.pushY_ = 0;
        }
    }

    override protected function generateNameBitmapData(nameText:SimpleText):BitmapData {
        if (this.isFellowGuild_) {
            nameText.setColor(Parameters.FELLOW_GUILD_COLOR);
        } else {
            nameText.setColor(Parameters.NAME_COLOUR);
        }
        var nameBitmapData:BitmapData = new BitmapData(nameText.width + 20, 64, true, 0);
        nameBitmapData.draw(nameText, NAME_OFFSET_MATRIX);
        nameBitmapData.applyFilter(nameBitmapData, nameBitmapData.rect, PointUtil.ORIGIN, new GlowFilter(0, 1, 3, 3, 2, 1));
        var rankIcon:Sprite = FameUtil.numStarsToIcon(this.numStars_);
        nameBitmapData.draw(rankIcon, RANK_OFFSET_MATRIX);
        return nameBitmapData;
    }

    protected function drawBreathBar(graphicsData:Vector.<IGraphicsData>, time:int):void {
        var b:Number = NaN;
        var bw:Number = NaN;
        if (this.breathPath_ == null) {
            this.breathBackFill_ = new GraphicsSolidFill();
            this.breathBackPath_ = new GraphicsPath(GraphicsUtil.QUAD_COMMANDS, new Vector.<Number>());
            this.breathFill_ = new GraphicsSolidFill(2542335);
            this.breathPath_ = new GraphicsPath(GraphicsUtil.QUAD_COMMANDS, new Vector.<Number>());
        }
        if (this.oxygen_ <= Parameters.BREATH_THRESH) {
            b = (Parameters.BREATH_THRESH - this.oxygen_) / Parameters.BREATH_THRESH;
            this.breathBackFill_.color = MoreColorUtil.lerpColor(5526612, 16711680, Math.abs(Math.sin(time / 300)) * b);
        } else {
            this.breathBackFill_.color = 5526612;
        }
        var w:int = DEFAULT_HP_BAR_WIDTH;
        var yOffset:int = DEFAULT_HP_BAR_Y_OFFSET + DEFAULT_HP_BAR_HEIGHT;
        var h:int = DEFAULT_HP_BAR_HEIGHT;
        this.breathBackPath_.data.length = 0;
        this.breathBackPath_.data.push(posS_[0] - w, posS_[1] + yOffset, posS_[0] + w, posS_[1] + yOffset, posS_[0] + w, posS_[1] + yOffset + h, posS_[0] - w, posS_[1] + yOffset + h);
        graphicsData.push(this.breathBackFill_);
        graphicsData.push(this.breathBackPath_);
        graphicsData.push(GraphicsUtil.END_FILL);
        if (this.oxygen_ > 0) {
            bw = this.oxygen_ / 100 * 2 * w;
            this.breathPath_.data.length = 0;
            this.breathPath_.data.push(posS_[0] - w, posS_[1] + yOffset, posS_[0] - w + bw, posS_[1] + yOffset, posS_[0] - w + bw, posS_[1] + yOffset + h, posS_[0] - w, posS_[1] + yOffset + h);
            graphicsData.push(this.breathFill_);
            graphicsData.push(this.breathPath_);
            graphicsData.push(GraphicsUtil.END_FILL);
        }
        GraphicsFillExtra.setSoftwareDrawSolid(this.breathFill_, true);
        GraphicsFillExtra.setSoftwareDrawSolid(this.breathBackFill_, true);
    }

    override public function draw(graphicsData:Vector.<IGraphicsData>, camera:Camera, time:int):void {
        if (this.isInHideList(Parameters.data_.hideList)) {
            return;
        }

        super.draw(graphicsData, camera, time);
        if (this != map_.player_ && this.name_ != null && this.name_.length != 0) {
            drawName(graphicsData, camera);
        } else if (this.oxygen_ >= 0) {
            this.drawBreathBar(graphicsData, time);
        }
        if (this == map_.player_ && ObjectLibrary.getIdFromType(this.objectType_) == "Ninja") {
            this.drawSheathWellBar(graphicsData);
        }
        if (this == map_.player_ && ObjectLibrary.getIdFromType(this.objectType_) == "Archer") {
            this.drawQuiverArrows(graphicsData);
        }
        if (this == map_.player_ && ObjectLibrary.getIdFromType(this.objectType_) == "Warrior") {
            this.drawHelmStacks(graphicsData);
            this.drawHelmDurationBar(graphicsData)
        }
    }

    public function isInHideList(hideList:int):Boolean {
        switch (hideList) {
            case 0:
                return false;
            case 1:
                return this != this.map_.player_;
            case 2:
                return this != this.map_.player_ && !this.starred_;
            case 3:
                return this != this.map_.player_ && !this.isFellowGuild_;
            case 4:
                return this != this.map_.player_ && !this.starred_ && !this.isFellowGuild_;
            default:
                return false;
        }
    }

    private function getMoveSpeed():Number {
        if (isSlowed()) {
            return 1;
        }
        var moveSpeed:Number = map_.gs_.mui_.isKeyDown[MapUserInput.CROUCH] ? 1 : this.movementSpeed
        if (isSpeedy()) {
            moveSpeed = moveSpeed * 1.5;
        }
        moveSpeed = moveSpeed * this.moveMultiplier_;
        return moveSpeed;
    }

    public function attackFrequency():Number {
        if (isDazed()) {
            return 1;
        }
        var attFreq:Number = this.attackSpeed;
        if (isBerserk()) {
            attFreq = attFreq * 1.5;
        }
        return attFreq;
    }

    private function getProjDamage(projDesc:ProjectileDesc):Object {
        var minDmg:int = projDesc.MinDamage;
        var maxDmg:int = projDesc.MaxDamage;

        var damage:Number = this.map_.gs_.gsc_.nextIntRange(minDmg, maxDmg);

        damage *= getDamageMultiplier();
        var critMult:Number = getCritMultiplier();

        damage *= critMult;

        return {dmg: damage, crit: critMult};
    }

    private function getDamageMultiplier():Number {
        //ar attMult:Number = MIN_ATTACK_MULT + this.attack / 75 * (MAX_ATTACK_MULT - MIN_ATTACK_MULT);

        var dmgMult:Number = 1.0 + damageMultiplier / 100.0;

        return dmgMult;
    }

    private function getCritMultiplier():Number {
        var critMult:Number = 1.0;

        if (this.map_.gs_.gsc_.nextIntRange(0, 1001) < criticalChance * 10)
            critMult += criticalDamage / 100.0;

        return critMult;
    }

    public function checkDodge():Boolean {
        return this.map_.gs_.gsc_.nextIntRange(0, 1001) < (dodgeChance * 10);
    }

    private function makeSkinTexture():void {
        var image:MaskedImage = this.skin.imageFromAngle(0, AnimatedChar.STAND, 0);
        animatedChar_ = this.skin;
        texture_ = image.image_;
        mask_ = image.mask_;
        this.isDefaultAnimatedChar = true;
    }

    private function setToRandomAnimatedCharacter():void {
        var hexTransformList:Vector.<XML> = ObjectLibrary.hexTransforms_;
        var randIndex:uint = Math.floor(Math.random() * hexTransformList.length);
        var randomPetType:int = hexTransformList[randIndex].@type;
        var textureData:TextureData = ObjectLibrary.typeToTextureData_[randomPetType];
        texture_ = textureData.texture_;
        mask_ = textureData.mask_;
        animatedChar_ = textureData.animatedChar_;
        this.isDefaultAnimatedChar = false;
    }

    private function getHallucinatingMaskedImage():MaskedImage {
        if (hallucinatingMaskedImage_ == null) {
            hallucinatingMaskedImage_ = new MaskedImage(getHallucinatingTexture(), null);
        }
        return hallucinatingMaskedImage_;
    }

    override protected function getTexture(camera:Camera, time:int):BitmapData {
        var image:MaskedImage = null;
        var walkPer:int = 0;
        var dict:Dictionary = null;
        var rv:Number = NaN;
        var p:Number = 0;
        var action:int = AnimatedChar.STAND;
        if (time < attackStart_ + this.attackPeriod_) {
            facing_ = attackAngle_;
            p = (time - attackStart_) % this.attackPeriod_ / this.attackPeriod_;
            action = AnimatedChar.ATTACK;
        } else if (direction_.x != 0 || direction_.y != 0) {
            walkPer = 3.5 / (this.getMoveSpeed() / 1000.0);
            if (direction_.y != 0 || direction_.x != 0) {
                facing_ = Math.atan2(direction_.y, direction_.x);
            }
            p = time % walkPer / walkPer;
            action = AnimatedChar.WALK;
        }
        if (this.isHexed()) {
            this.isDefaultAnimatedChar && this.setToRandomAnimatedCharacter();
        } else if (!this.isDefaultAnimatedChar) {
            this.makeSkinTexture();
        }
        if (camera.isHallucinating_) {
            image = getHallucinatingMaskedImage();
        } else {
            image = animatedChar_.imageFromFacing(facing_, camera, action, p);
        }
        var tex1Id:int = tex1Id_;
        var tex2Id:int = tex2Id_;
        var texture:BitmapData = null;
        if (this.nearestMerchant_ != null) {
            dict = texturingCache_[this.nearestMerchant_];
            if (dict == null) {
                texturingCache_[this.nearestMerchant_] = new Dictionary();
            } else {
                texture = dict[image];
            }
            tex1Id = this.nearestMerchant_.getTex1Id(tex1Id_);
            tex2Id = this.nearestMerchant_.getTex2Id(tex2Id_);
        } else {
            texture = texturingCache_[image];
        }
        if (texture == null) {
            texture = TextureRedrawer.resize(image.image_, image.mask_, size_, false, tex1Id, tex2Id);
            if (this.nearestMerchant_ != null) {
                texturingCache_[this.nearestMerchant_][image] = texture;
            } else {
                texturingCache_[image] = texture;
            }
        }
        if (hp < maxHP * 0.2) {
            rv = int(Math.abs(Math.sin(time / 200)) * 10) / 10;
            var ct:ColorTransform = lowHealthCT[rv];
            if (ct == null) {
                ct = new ColorTransform(1, 1, 1, 1,
                        rv * LOW_HEALTH_CT_OFFSET,
                        -rv * LOW_HEALTH_CT_OFFSET,
                        -rv * LOW_HEALTH_CT_OFFSET);
                lowHealthCT[rv] = ct;
            }
            texture = CachingColorTransformer.transformBitmapData(texture, ct);
        }
        var filteredTexture:BitmapData = texturingCache_[texture];
        if (filteredTexture == null) {
            filteredTexture = GlowRedrawer.outlineGlow(texture, this.legendaryRank_ == -1 ? 0 : 16711680);
            texturingCache_[texture] = filteredTexture;
        }
        if (isStasis()) {
            filteredTexture = CachingColorTransformer.filterBitmapData(filteredTexture, PAUSED_FILTER);
        } else if (isInvisible()) {
            filteredTexture = CachingColorTransformer.alphaBitmapData(filteredTexture, 40);
        }
        return filteredTexture;
    }

    override public function getPortrait():BitmapData {
        var image:MaskedImage = null;
        var size:int = 0;
        if (portrait_ == null) {
            image = animatedChar_.imageFromDir(AnimatedChar.RIGHT, AnimatedChar.STAND, 0);
            size = 4 / image.image_.width * 100;
            portrait_ = TextureRedrawer.resize(image.image_, image.mask_, size, true, tex1Id_, tex2Id_);
            portrait_ = GlowRedrawer.outlineGlow(portrait_, 0);
        }
        return portrait_;
    }

    public function getPortraitImage():MaskedImage {
        return animatedChar_.imageFromDir(AnimatedChar.RIGHT, AnimatedChar.STAND, 0);
    }

    public function useAltWeapon(x:Number, y:Number, release:Boolean):Boolean {
        if (map_ == null) {
            return false;
        }

        var now:int = 0;
        var mpCost:int = 0;
        var cooldown:int = 0;
        var angle:Number = Parameters.data_.cameraAngle + Math.atan2(y, x);
        var item:ItemData = equipment_[1];
        if (item == null || !item.Usable)
            return false;

        var checkPoint:Boolean = false;
        var validatePoint:Boolean = false;
        var point:Point = null;

        for each(var ae:ActivateEffect in item.ActivateEffects) {
            var aeName:String = ae.EffectName;
            if (aeName == ActivationType.TELEPORT) {
                checkPoint = true;
                validatePoint = true;
            } else if (aeName == ActivationType.BULLET_NOVA || aeName == ActivationType.POISON_GRENADE || aeName == ActivationType.VAMPIRE_BLAST || aeName == ActivationType.TRAP || aeName == ActivationType.STASIS_BLAST || aeName == ActivationType.SHURIKEN) {
                checkPoint = true;
            }
        }

        if (checkPoint) {
            point = map_.pSTopW(x, y);
            if (point == null || (validatePoint && !this.isValidPosition(point.x, point.y))) {
                SoundEffectLibrary.play("error");
                return false;
            }
        }

        var toGrid:Number = Math.sqrt(x * x + y * y) / 50;
        point = new Point(x_ + toGrid * Math.cos(angle), y_ + toGrid * Math.sin((angle)));
        if (validatePoint && !this.isValidPosition(point.x, point.y)) {
            point = map_.pSTopW(x, y);
        }

        now = getTimer();

        // Key down
        if (!release)
        {
            if (now < this.nextAltAttack_) {
                SoundEffectLibrary.play("error");
                return false;
            }

            // Eventually replace MpCost check with specific ability use condition logic
            mpCost = item.MpCost;
            if (item.Spell) mpCost = item.Spell.MpCost;
            if (mpCost > this.mp) {
                SoundEffectLibrary.play("no_mana");
                return false;
            }

            if (!item.MultiPhase)
                this.nextAltAttack_ = now + item.Cooldown * 1000;

            map_.gs_.gsc_.useItem(map_.gs_.lastUpdate_, objectId_, 1, point.x, point.y);
        }
        // Key up
        else {
            if (!item.MultiPhase)
                return false;

            this.nextAltAttack_ = now + item.Cooldown * 1000;
            map_.gs_.gsc_.useItem(map_.gs_.lastUpdate_, objectId_, 1, point.x, point.y);
        }

        return true;
    }

    public function attemptAttackAngle(angle:Number):void {
        this.shoot(Parameters.data_.cameraAngle + angle);
    }

    override public function setAttack(containerType:int, attackAngle:Number):void {
        var weaponXML:XML = ObjectLibrary.xmlLibrary_[containerType];
        if (weaponXML == null || !weaponXML.hasOwnProperty("RateOfFire")) {
            return;
        }
        var rateOfFire:Number = Number(weaponXML.RateOfFire);
        this.attackPeriod_ = 1 / (this.attackFrequency() * rateOfFire) * 1000;
        super.setAttack(containerType, attackAngle);
    }

    private function shoot(attackAngle:Number):void {
        if (map_ == null || isStunned()) {
            return;
        }
        var weapon:ItemData = equipment_[0];
        if (weapon == null)
            return;

        var time:int = getTimer();
        var rateOfFire:Number = weapon.RateOfFire; // Multiplier
        this.attackPeriod_ = 1 / (this.attackFrequency() * rateOfFire) * 1000;
        if (time < attackStart_ + this.attackPeriod_) {
            return;
        }
        attackAngle_ = attackAngle;
        attackStart_ = time;

        var path:ProjectilePath = ProjectilePath.createFromDesc(weapon.Projectile);
        this.doShoot(attackStart_, weapon.NumProjectiles, weapon.ObjectType, weapon.Projectile, attackAngle_, weapon.ArcGap * Trig.toRadians, path, false);
    }

    public function doShoot(time:int, numShots:int, containerType:int, projDesc:ProjectileDesc, attackAngle:Number, arcGap:Number, path:ProjectilePath, isServerShoot:Boolean, pos:Point = null, damageList:Vector.<int> = null, critList:Vector.<Number> = null):void {
        var proj:Projectile = null;
        var totalArc:Number = arcGap * (numShots - 1);
        var angle:Number = attackAngle - totalArc / 2;
        var minDamage:int = projDesc.MinDamage;
        var maxDamage:int = projDesc.MaxDamage;
        var x:Number = pos == null ? this.x_ : pos.x;
        var y:Number = pos == null ? this.y_ : pos.y;

        for (var i:int = 0; i < numShots; i++) {
            proj = FreeList.newObject(Projectile) as Projectile;
            proj.resetPlayerProjectileValues(containerType, ObjectLibrary.getTypeFromId(projDesc.ObjectId), objectId_, getNextProjectileId(), angle,
                    time, path.Clone(), minDamage, maxDamage, projDesc.LifetimeMS, projDesc.MultiHit,
                    projDesc.PassesCover, projDesc.ArmorPiercing, projDesc.Size, null);

            if (damageList == null) {
                var dmgObj:Object = getProjDamage(projDesc);
                proj.setDamage(dmgObj.dmg, dmgObj.crit);
            }
            else{
                proj.setDamage(damageList[i], critList[i]);
            }

            if (i == 0 && proj.sound_ != null) {
                SoundEffectLibrary.play(proj.sound_, false);
            }
            map_.addObj(proj, x, y);
            angle = angle + arcGap;
        }

        if (!isServerShoot) {
            map_.gs_.gsc_.playerShoot(attackAngle * Trig.toDegrees, x, y, getTimer(), false, arcGap * Trig.toDegrees, damageList, critList, containerType);
        }
    }

    private function getNextProjectileId():int {
        return this.nextProjectileId++;
    }

    public function updateNextProjectileId(serverUpdate:int):void {
        this.nextProjectileId = serverUpdate;
    }

    public function isHexed():Boolean {
        return (condition_[ConditionEffect.CE_FIRST_BATCH] & ConditionEffect.HEXED_BIT) != 0;
    }

    public function isInventoryFull():Boolean {
        var len:int = equipment_.length;
        for (var i:uint = 4; i < len; i++) {
            if (equipment_[i] == null) {
                return false;
            }
        }
        return true;
    }

    public function nextAvailableInventorySlot():int {
        var len:int = this.hasBackpack_ ? int(equipment_.length) : int(equipment_.length - GeneralConstants.NUM_INVENTORY_SLOTS);
        for (var i:uint = 4; i < len; i++) {
            if (equipment_[i] == null) {
                return i;
            }
        }
        return -1;
    }

    public function swapInventoryIndex(current:String):int {
        var start:int = 0;
        var end:int = 0;
        if (!this.hasBackpack_) {
            return -1;
        }
        if (current == TabStripModel.BACKPACK) {
            start = GeneralConstants.NUM_EQUIPMENT_SLOTS;
            end = GeneralConstants.NUM_EQUIPMENT_SLOTS + GeneralConstants.NUM_INVENTORY_SLOTS;
        } else {
            start = GeneralConstants.NUM_EQUIPMENT_SLOTS + GeneralConstants.NUM_INVENTORY_SLOTS;
            end = equipment_.length;
        }
        for (var i:uint = start; i < end; i++) {
            if (equipment_[i] == null) {
                return i;
            }
        }
        return -1;
    }

    public function getPotionCount(objectType:int):int {
        switch (objectType) {
            case PotionInventoryModel.HEALTH_POTION_ID:
                return this.healthPotionCount_;
            case PotionInventoryModel.MAGIC_POTION_ID:
                return this.magicPotionCount_;
            default:
                return 0;
        }
    }

    private function drawSheathWellBar(graphicsData:Vector.<IGraphicsData>):void {
        var yOffset:int = -20;
        var xOffset:int = -30 * (100 / this.size_);
        var fPerc:Number = NaN;
        var bh:Number = NaN;
        if (this.wellBarPath_ == null) {
            this.wellBarBackFill_ = new GraphicsSolidFill();
            this.wellBarBackPath_ = new GraphicsPath(GraphicsUtil.QUAD_COMMANDS, new Vector.<Number>());
            this.wellBarFill_ = new GraphicsSolidFill();
            this.wellBarPath_ = new GraphicsPath(GraphicsUtil.QUAD_COMMANDS, new Vector.<Number>());
            this.wellBarBackFill_.color = 5526612;
            this.wellBarFill_.color = 0x910c0c;
        }

        if (this.isInStance()){
            this.wellBarFill_.color = 0xd9a71e;
        }
        else{
            this.wellBarFill_.color = 0x910c0c;
        }

        var w:int = 3;
        var h:int = 20;
        this.wellBarBackPath_.data.length = 0;
        this.wellBarBackPath_.data.push(posS_[0] + xOffset, posS_[1] - h + yOffset, posS_[0] + xOffset, posS_[1] + h + yOffset, posS_[0] + w + xOffset, posS_[1] + yOffset + h, posS_[0] + w + xOffset, posS_[1] + yOffset - h);

        graphicsData.push(this.wellBarBackFill_);
        graphicsData.push(this.wellBarBackPath_);
        graphicsData.push(GraphicsUtil.END_FILL);

        var wellDamage:int = int(this.abilityDataA);
        if (this.equipment_[1] == null || this.equipment_[1].Sheath == null){
            return;
        }

        var wellCapacity:int = this.equipment_[1].Sheath.Capacity;
        if (wellDamage > 0) { // Sheath well current damage stored
            fPerc = wellDamage / Number(wellCapacity);
            bh = fPerc * 2 * h;
            this.wellBarPath_.data.length = 0;
            this.wellBarPath_.data.push(posS_[0] + xOffset, posS_[1] - h + yOffset, posS_[0] + xOffset, posS_[1] - h + bh + yOffset, posS_[0] + w + xOffset, posS_[1] - h + bh + yOffset, posS_[0] + w + xOffset, posS_[1] + yOffset - h);

            graphicsData.push(this.wellBarFill_);
            graphicsData.push(this.wellBarPath_);
            graphicsData.push(GraphicsUtil.END_FILL);
        }
        GraphicsFillExtra.setSoftwareDrawSolid(this.wellBarFill_, true);
        GraphicsFillExtra.setSoftwareDrawSolid(this.wellBarBackFill_, true);
    }

    private function drawQuiverArrows(graphicsData:Vector.<IGraphicsData>)
    {
        const arrowsPerColumn:int = 4;
        const radius:int = 3;
        const gap:int = 3;

        var quiver:QuiverDesc = equipment_[1].Quiver;
        var maxArrows:int = quiver.MaxArrows;
        var arrows:int = int(abilityDataA);
        var mpArrows:int = quiver.UseMpArrows ? Math.min(maxArrows - arrows, mp / quiver.MpPerArrow) : 0;
        if (arrows < 1) mpArrows = 0;

        var playerX:int = posS_[0];
        var playerY:int = posS_[1];

        var startX:int = playerX - 20 * (100 / size_);
        var startY:int = playerY;

        var x:int = startX - radius;
        var y:int = startY - radius;

        var i:int = 0;
        var stop:int = 0 + arrows;

        quiverArrowsPath_.commands.length = 0;
        quiverArrowsPath_.data.length = 0;
        quiverMpArrowsPath_.commands.length = 0;
        quiverMpArrowsPath_.data.length = 0;
        quiverBackPath_.commands.length = 0;
        quiverBackPath_.data.length = 0;

        // Draw regular arrows
        for (i; i < stop; i++)
        {
            if (i % arrowsPerColumn == 0){
                x -= (gap + radius * 2);
                y = startY - radius;
            }

            GraphicsUtil.drawCircle(x, y, radius, quiverArrowsPath_);
            y -= (gap + radius * 2);
        }

        stop = i + mpArrows;
        // Draw mp arrows
        for (i; i < stop; i++)
        {
            if (i % arrowsPerColumn == 0){
                x -= (gap + radius * 2);
                y = startY - radius;
            }

            GraphicsUtil.drawCircle(x, y, radius, quiverMpArrowsPath_);
            y -= (gap + radius * 2);
        }

        stop = maxArrows;
        // Draw empty arrows
        for (i; i < stop; i++)
        {
            if (i % arrowsPerColumn == 0){
                x -= (gap + radius * 2);
                y = startY - radius;
            }

            GraphicsUtil.drawCircle(x, y, radius, quiverBackPath_);
            y -= (gap + radius * 2);
        }

        graphicsData.push(quiverArrowsFill_);
        graphicsData.push(quiverArrowsPath_);
        graphicsData.push(quiverMpArrowsFill_);
        graphicsData.push(quiverMpArrowsPath_);
        graphicsData.push(quiverBackFill_);
        graphicsData.push(quiverBackPath_);
        graphicsData.push(GraphicsUtil.END_FILL);

        GraphicsFillExtra.setSoftwareDrawSolid(quiverArrowsFill_, true);
        GraphicsFillExtra.setSoftwareDrawSolid(quiverMpArrowsFill_, true);
        GraphicsFillExtra.setSoftwareDrawSolid(quiverBackFill_, true);
    }

    private function drawHelmStacks(graphicsData:Vector.<IGraphicsData>):void {
        if (this.abilityDataA == null || this.abilityDataA == "0") {
            return;
        }

        warriorStacksText.text = this.abilityDataA;
        warriorStacksText.updateMetrics();

        warriorTextBitmap = new BitmapData(warriorStacksText.width + 20, 64, true, 0x00000000);
        warriorTextBitmap.draw(warriorStacksText);

        warriorTextBitmap.applyFilter(
                warriorTextBitmap,
                warriorTextBitmap.rect,
                PointUtil.ORIGIN,
                new GlowFilter(0x000000, 1, 3, 3, 2, 1)
        );

        var halfWidth:int = warriorTextBitmap.width + 3;
        var height:int = 60;

        var pathVertices:Vector.<Number> = warriorPath.data;
        pathVertices.length = 0;
        pathVertices.push(
                posS_[0] - halfWidth, posS_[1] - height,  // haut gauche
                posS_[0] + halfWidth, posS_[1] - height,  // haut droite
                posS_[0] + halfWidth, posS_[1],           // bas droite
                posS_[0] - halfWidth, posS_[1]            // bas gauche
        );

        warriorBitmapFill.bitmapData = warriorTextBitmap;

        var matrix:Matrix = warriorBitmapFill.matrix;
        matrix.identity();
        matrix.translate(pathVertices[0], pathVertices[1]);

        graphicsData.push(warriorBitmapFill);
        graphicsData.push(warriorPath);
        graphicsData.push(GraphicsUtil.END_FILL);
    }

    private function drawHelmDurationBar(graphicsData:Vector.<IGraphicsData>):void
    {
        if (this.equipment_[1] == null || this.equipment_[1].Helm == null) {
            return;
        }

        var centerX:Number = posS_[0] - 30;
        var centerY:Number = posS_[1] - 50;
        var radius:Number = 10;
        var startAngle:Number = 0; // en radians
        var endAngle:Number = Math.PI * 2; // 90° = π/2
        endAngle *= int(this.abilityDataB) / this.equipment_[1].Helm.Duration;

        var segments:int = 32;
        var angleStep:Number = (endAngle - startAngle) / segments;

        var angle:Number = startAngle;
        var first:Boolean = true;


        this.warriorArcPath = new GraphicsPath();
        for (var i:int = 0; i <= segments; i++) {
            var x:Number = centerX + radius * Math.cos(angle);
            var y:Number = centerY + radius * Math.sin(angle);

            if (first) {
                this.warriorArcPath.moveTo(x, y);
                first = false;
            } else {
                this.warriorArcPath.lineTo(x, y);
            }

            angle += angleStep;
        }

        graphicsData.push(this.warriorArcStroke);
        graphicsData.push(this.warriorArcPath);
        graphicsData.push(GraphicsUtil.END_FILL);
        graphicsData.push(GraphicsUtil.END_STROKE);
        GraphicsFillExtra.setSoftwareDrawSolid(this.warriorArcFill, true)
    }
}
}
