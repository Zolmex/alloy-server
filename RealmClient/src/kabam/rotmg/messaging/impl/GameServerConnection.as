package kabam.rotmg.messaging.impl {
import com.company.assembleegameclient.objects.ObjectProperties;
import com.company.assembleegameclient.objects.ProjectileProperties;
import com.company.assembleegameclient.objects.particles.SheatheSlashEffect;
import com.company.assembleegameclient.objects.projectiles.*;
import com.company.assembleegameclient.game.GameSprite;
import com.company.assembleegameclient.game.events.GuildResultEvent;
import com.company.assembleegameclient.game.events.ReconnectEvent;
import com.company.assembleegameclient.itemData.ItemData;
import com.company.assembleegameclient.map.GroundLibrary;
import com.company.assembleegameclient.map.Map;
import com.company.assembleegameclient.map.mapoverlay.CharacterStatusText;
import com.company.assembleegameclient.objects.Character;
import com.company.assembleegameclient.objects.FlashDescription;
import com.company.assembleegameclient.objects.GameObject;
import com.company.assembleegameclient.objects.Merchant;
import com.company.assembleegameclient.objects.ObjectLibrary;
import com.company.assembleegameclient.objects.Player;
import com.company.assembleegameclient.objects.Portal;
import com.company.assembleegameclient.objects.Projectile;
import com.company.assembleegameclient.objects.SellableObject;
import com.company.assembleegameclient.objects.particles.AOEEffect;
import com.company.assembleegameclient.objects.particles.BurstEffect;
import com.company.assembleegameclient.objects.particles.CollapseEffect;
import com.company.assembleegameclient.objects.particles.ConeBlastEffect;
import com.company.assembleegameclient.objects.particles.FlowEffect;
import com.company.assembleegameclient.objects.particles.HealEffect;
import com.company.assembleegameclient.objects.particles.LightningEffect;
import com.company.assembleegameclient.objects.particles.LineEffect;
import com.company.assembleegameclient.objects.particles.NovaEffect;
import com.company.assembleegameclient.objects.particles.ParticleEffect;
import com.company.assembleegameclient.objects.particles.PoisonEffect;
import com.company.assembleegameclient.objects.particles.RingEffect;
import com.company.assembleegameclient.objects.particles.StreamEffect;
import com.company.assembleegameclient.objects.particles.TeleportEffect;
import com.company.assembleegameclient.objects.particles.ThrowEffect;
import com.company.assembleegameclient.objects.thrown.ThrowProjectileEffect;
import com.company.assembleegameclient.parameters.Parameters;
import com.company.assembleegameclient.sound.SoundEffectLibrary;
import com.company.assembleegameclient.ui.dialogs.Dialog;
import com.company.assembleegameclient.ui.panels.GuildInvitePanel;
import com.company.assembleegameclient.util.ConditionEffect;
import com.company.assembleegameclient.util.FreeList;
import com.company.util.Random;
import com.company.assembleegameclient.ui.panels.TradeRequestPanel;
import com.company.util.Trig;

import flash.display.BitmapData;
import flash.events.Event;
import flash.geom.Point;
import flash.utils.getTimer;
import kabam.lib.net.api.MessageMap;
import kabam.lib.net.api.MessageProvider;
import kabam.lib.net.impl.SocketServer;
import kabam.rotmg.account.core.Account;
import kabam.rotmg.classes.model.CharacterClass;
import kabam.rotmg.classes.model.ClassesModel;
import kabam.rotmg.constants.GeneralConstants;
import kabam.rotmg.core.StaticInjectorContext;
import kabam.rotmg.core.model.PlayerModel;
import kabam.rotmg.death.control.HandleDeathSignal;
import kabam.rotmg.dialogs.control.OpenDialogSignal;
import kabam.rotmg.game.focus.control.SetGameFocusSignal;
import kabam.rotmg.game.model.AddSpeechBalloonVO;
import kabam.rotmg.game.model.AddTextLineVO;
import kabam.rotmg.game.model.GameModel;
import kabam.rotmg.game.model.PotionInventoryModel;
import kabam.rotmg.game.signals.AddSpeechBalloonSignal;
import kabam.rotmg.game.signals.AddTextLineSignal;
import kabam.rotmg.messaging.impl.data.GroundTileData;
import kabam.rotmg.messaging.impl.data.ObjectData;
import kabam.rotmg.messaging.impl.data.ObjectDropData;
import kabam.rotmg.messaging.impl.data.ObjectStatusData;
import kabam.rotmg.messaging.impl.data.StatData;
import kabam.rotmg.messaging.impl.data.WorldPosData;
import kabam.rotmg.messaging.impl.incoming.AccountList;
import kabam.rotmg.messaging.impl.incoming.Aoe;
import kabam.rotmg.messaging.impl.incoming.BuyResult;
import kabam.rotmg.messaging.impl.incoming.CreateSuccess;
import kabam.rotmg.messaging.impl.incoming.Damage;
import kabam.rotmg.messaging.impl.incoming.DamageCounterUpdate;
import kabam.rotmg.messaging.impl.incoming.Death;
import kabam.rotmg.messaging.impl.incoming.EnemyShoot;
import kabam.rotmg.messaging.impl.incoming.Failure;
import kabam.rotmg.messaging.impl.incoming.Goto;
import kabam.rotmg.messaging.impl.incoming.GuildResult;
import kabam.rotmg.messaging.impl.incoming.InvResult;
import kabam.rotmg.messaging.impl.incoming.InvitedToGuild;
import kabam.rotmg.messaging.impl.incoming.MapInfo;
import kabam.rotmg.messaging.impl.incoming.NewTick;
import kabam.rotmg.messaging.impl.incoming.Notification;
import kabam.rotmg.messaging.impl.incoming.PlaySound;
import kabam.rotmg.messaging.impl.incoming.Reconnect;
import kabam.rotmg.messaging.impl.incoming.ServerPlayerShoot;
import kabam.rotmg.messaging.impl.incoming.ShowEffect;
import kabam.rotmg.messaging.impl.incoming.StatsApplyResult;
import kabam.rotmg.messaging.impl.incoming.Text;
import kabam.rotmg.messaging.impl.incoming.Update;
import kabam.rotmg.messaging.impl.outgoing.GemstoneSwap;
import kabam.rotmg.messaging.impl.outgoing.PartyInvite;
import kabam.rotmg.messaging.impl.outgoing.TradeRequest;
import kabam.rotmg.messaging.impl.incoming.TradeAccepted;
import kabam.rotmg.messaging.impl.incoming.TradeChanged;
import kabam.rotmg.messaging.impl.incoming.TradeDone;
import kabam.rotmg.messaging.impl.incoming.TradeRequested;
import kabam.rotmg.messaging.impl.incoming.TradeStart;
import kabam.rotmg.messaging.impl.outgoing.AoeAck;
import kabam.rotmg.messaging.impl.outgoing.Buy;
import kabam.rotmg.messaging.impl.outgoing.ChangeGuildRank;
import kabam.rotmg.messaging.impl.outgoing.ConstellationsSave;
import kabam.rotmg.messaging.impl.outgoing.ConstellationsTrial;
import kabam.rotmg.messaging.impl.outgoing.Create;
import kabam.rotmg.messaging.impl.outgoing.CreateGuild;
import kabam.rotmg.messaging.impl.outgoing.EditAccountList;
import kabam.rotmg.messaging.impl.outgoing.EnemyHit;
import kabam.rotmg.messaging.impl.outgoing.Escape;
import kabam.rotmg.messaging.impl.outgoing.GemstoneApply;
import kabam.rotmg.messaging.impl.outgoing.GemstoneRemove;
import kabam.rotmg.messaging.impl.outgoing.GotoAck;
import kabam.rotmg.messaging.impl.outgoing.GuildInvite;
import kabam.rotmg.messaging.impl.outgoing.GuildRemove;
import kabam.rotmg.messaging.impl.outgoing.Hello;
import kabam.rotmg.messaging.impl.outgoing.InvDrop;
import kabam.rotmg.messaging.impl.outgoing.InvSwap;
import kabam.rotmg.messaging.impl.outgoing.JoinGuild;
import kabam.rotmg.messaging.impl.outgoing.Load;
import kabam.rotmg.messaging.impl.outgoing.Move;
import kabam.rotmg.messaging.impl.outgoing.PlayerHit;
import kabam.rotmg.messaging.impl.outgoing.PlayerShoot;
import kabam.rotmg.messaging.impl.outgoing.PlayerText;
import kabam.rotmg.messaging.impl.outgoing.Reskin;
import kabam.rotmg.messaging.impl.outgoing.ShootAck;
import kabam.rotmg.messaging.impl.outgoing.SquareHit;
import kabam.rotmg.messaging.impl.outgoing.StatsApply;
import kabam.rotmg.messaging.impl.outgoing.Teleport;
import kabam.rotmg.messaging.impl.outgoing.UseItem;
import kabam.rotmg.messaging.impl.outgoing.UsePortal;
import kabam.rotmg.messaging.impl.outgoing.AcceptTrade;
import kabam.rotmg.messaging.impl.outgoing.CancelTrade;
import kabam.rotmg.messaging.impl.outgoing.ChangeTrade;
import kabam.rotmg.minimap.control.UpdateGameObjectTileSignal;
import kabam.rotmg.minimap.control.UpdateGroundTileSignal;
import kabam.rotmg.minimap.model.UpdateGroundTileVO;
import kabam.rotmg.servers.api.Server;
import kabam.rotmg.ui.model.UpdateGameObjectTileVO;
import kabam.rotmg.ui.signals.UpdateBackpackTabSignal;
import kabam.rotmg.ui.view.MessageCloseDialog;
import org.swiftsuspenders.Injector;
import robotlegs.bender.framework.api.ILogger;
import kabam.rotmg.messaging.impl.outgoing.OptionsChanged;

public class GameServerConnection {

    public static const FAILURE:int = 0;
    public static const CREATE_SUCCESS:int = 1;
    public static const CREATE:int = 2;
    public static const PLAYERSHOOT:int = 3;
    public static const MOVE:int = 4;
    public static const PLAYERTEXT:int = 5;
    public static const TEXT:int = 6;
    public static const SERVERPLAYERSHOOT:int = 7;
    public static const DAMAGE:int = 8;
    public static const UPDATE:int = 9;
    public static const NOTIFICATION:int = 10;
    public static const NEWTICK:int = 11;
    public static const INVSWAP:int = 12;
    public static const USEITEM:int = 13;
    public static const SHOWEFFECT:int = 14;
    public static const HELLO:int = 15;
    public static const GOTO:int = 16;
    public static const INVDROP:int = 17;
    public static const INVRESULT:int = 18;
    public static const RECONNECT:int = 19;
    public static const MAPINFO:int = 20;
    public static const LOAD:int = 21;
    public static const TELEPORT:int = 22;
    public static const USEPORTAL:int = 23;
    public static const DEATH:int = 24;
    public static const BUY:int = 25;
    public static const BUYRESULT:int = 26;
    public static const AOE:int = 27;
    public static const PLAYERHIT:int = 28;
    public static const ENEMYHIT:int = 29;
    public static const AOEACK:int = 30;
    public static const SHOOTACK:int = 31;
    public static const SQUAREHIT:int = 32;
    public static const EDITACCOUNTLIST:int = 33;
    public static const ACCOUNTLIST:int = 34;
    public static const DAMAGECOUNTERUPDATE:int = 35;
    public static const CREATEGUILD:int = 36;
    public static const GUILDRESULT:int = 37;
    public static const GUILDREMOVE:int = 38;
    public static const GUILDINVITE:int = 39;
    public static const ENEMYSHOOT:int = 40;
    public static const ESCAPE:int = 41;
    public static const INVITEDTOGUILD:int = 42;
    public static const JOINGUILD:int = 43;
    public static const CHANGEGUILDRANK:int = 44;
    public static const PLAYSOUND:int = 45;
    public static const RESKIN:int = 46;
    public static const GOTOACK:int = 47;
    public static const CONSTELLATIONSTRIAL:int = 48;
    public static const CONSTELLATIONSSAVE:int = 49;
    public static const OPTIONS_CHANGED:int = 50;
    public static const STATSAPPLY:int = 51;
    public static const STATSAPPLYRESULT:int = 52;
    public static const GEMSTONE_APPLY:int = 53;
    public static const GEMSTONE_REMOVE:int = 54;
    public static const TRADEREQUEST:int = 55
    public static const TRADEREQUESTED:int = 56;
    public static const TRADESTART:int = 57;
    public static const CHANGETRADE:int = 58;
    public static const TRADECHANGED:int = 59;
    public static const CANCELTRADE:int = 60;
    public static const TRADEDONE:int = 61;
    public static const ACCEPTTRADE:int = 62;
    public static const TRADEACCEPTED:int = 63;
    public static const GEMSTONE_SWAP:int = 64;
    public static const PARTYINVITE:int = 65;

    public static var instance:GameServerConnection;

    private static const NORMAL_SPEECH_COLORS:Vector.<uint> = new <uint>[14802908, 16777215, 5526612];
    private static const ENEMY_SPEECH_COLORS:Vector.<uint> = new <uint>[5644060, 16549442, 13484223];
    private static const TELL_SPEECH_COLORS:Vector.<uint> = new <uint>[2493110, 61695, 13880567];
    private static const GUILD_SPEECH_COLORS:Vector.<uint> = new <uint>[4098560, 10944349, 13891532];
    private static const PARTY_SPEECH_COLORS:Vector.<uint> = new <uint>[5966261, 15650295, 15650295];

    public var gs_:GameSprite;
    public var server:Server;
    public var jitterWatcher_:JitterWatcher = null;
    public var serverConnection:SocketServer;
    private var messages:MessageProvider;
    private var playerId_:int = -1;
    private var player:Player;
    public var outstandingBuy_:OutstandingBuy = null;
    private var rand_:Random = null;
    private var death:Death;
    private var addTextLine:AddTextLineSignal;
    private var addSpeechBalloon:AddSpeechBalloonSignal;
    private var updateGroundTileSignal:UpdateGroundTileSignal;
    private var updateGameObjectTileSignal:UpdateGameObjectTileSignal;
    private var logger:ILogger;
    private var handleDeath:HandleDeathSignal;
    private var setGameFocus:SetGameFocusSignal;
    private var updateBackpackTab:UpdateBackpackTabSignal;
    private var classesModel:ClassesModel;
    private var playerModel:PlayerModel;
    private var injector:Injector;
    private var model:GameModel;
    public var conReady:Boolean;
    private var lastPos:WorldPosData;

    public function GameServerConnection(gs:GameSprite) {
        super();
        this.injector = StaticInjectorContext.getInjector();
        this.addTextLine = this.injector.getInstance(AddTextLineSignal);
        this.addSpeechBalloon = this.injector.getInstance(AddSpeechBalloonSignal);
        this.updateGroundTileSignal = this.injector.getInstance(UpdateGroundTileSignal);
        this.updateGameObjectTileSignal = this.injector.getInstance(UpdateGameObjectTileSignal);
        this.updateBackpackTab = StaticInjectorContext.getInjector().getInstance(UpdateBackpackTabSignal);
        this.logger = this.injector.getInstance(ILogger);
        this.handleDeath = this.injector.getInstance(HandleDeathSignal);
        this.setGameFocus = this.injector.getInstance(SetGameFocusSignal);
        this.classesModel = this.injector.getInstance(ClassesModel);
        this.serverConnection = this.injector.getInstance(SocketServer);
        this.messages = this.injector.getInstance(MessageProvider);
        this.model = this.injector.getInstance(GameModel);
        this.playerModel = this.injector.getInstance(PlayerModel);
        instance = this;
        this.gs_ = gs;
        this.lastPos = new WorldPosData();
    }

    public function setServer(server:Server):void {
        this.server = server;
    }

    public function disconnect():void {
        this.reset();
        this.removeServerConnectionListeners();
        this.unmapMessages();
        this.serverConnection.disconnect();
    }

    private function reset():void {
        this.conReady = false;
        this.player != null && this.player.dispose();
        this.player = null;
        this.playerId_ = -1;

        this.jitterWatcher_ = null;
    }

    private function removeServerConnectionListeners():void {
        this.serverConnection.connected.remove(this.onConnected);
        this.serverConnection.closed.remove(this.onClosed);
        this.serverConnection.error.remove(this.onError);
    }

    public function connect():void {
        this.addServerConnectionListeners();
        this.mapMessages();
        this.addTextLine.dispatch(new AddTextLineVO(Parameters.CLIENT_CHAT_NAME, "Connecting to " + this.server.name));
        this.serverConnection.connect(this.server.address, this.server.port);
    }

    private function addServerConnectionListeners():void {
        this.serverConnection.connected.add(this.onConnected);
        this.serverConnection.closed.add(this.onClosed);
        this.serverConnection.error.add(this.onError);
    }

    private function mapMessages():void {
        var messages:MessageMap = this.injector.getInstance(MessageMap);
        messages.map(CREATE).toMessage(Create);
        messages.map(PLAYERSHOOT).toMessage(PlayerShoot);
        messages.map(MOVE).toMessage(Move);
        messages.map(PLAYERTEXT).toMessage(PlayerText);
        messages.map(INVSWAP).toMessage(InvSwap);
        messages.map(USEITEM).toMessage(UseItem);
        messages.map(HELLO).toMessage(Hello);
        messages.map(INVDROP).toMessage(InvDrop);
        messages.map(LOAD).toMessage(Load);
        messages.map(TELEPORT).toMessage(Teleport);
        messages.map(USEPORTAL).toMessage(UsePortal);
        messages.map(BUY).toMessage(Buy);
        messages.map(PLAYERHIT).toMessage(PlayerHit);
        messages.map(ENEMYHIT).toMessage(EnemyHit);
        messages.map(AOEACK).toMessage(AoeAck);
        messages.map(SHOOTACK).toMessage(ShootAck);
        messages.map(SQUAREHIT).toMessage(SquareHit);
        messages.map(CREATEGUILD).toMessage(CreateGuild);
        messages.map(GUILDREMOVE).toMessage(GuildRemove);
        messages.map(GUILDINVITE).toMessage(GuildInvite);
        messages.map(ESCAPE).toMessage(Escape);
        messages.map(JOINGUILD).toMessage(JoinGuild);
        messages.map(CHANGEGUILDRANK).toMessage(ChangeGuildRank);
        messages.map(EDITACCOUNTLIST).toMessage(EditAccountList);
        messages.map(OPTIONS_CHANGED).toMessage(OptionsChanged);
        messages.map(CONSTELLATIONSTRIAL).toMessage(ConstellationsTrial);
        messages.map(CONSTELLATIONSSAVE).toMessage(ConstellationsSave);
        messages.map(STATSAPPLY).toMessage(StatsApply);
        messages.map(GEMSTONE_APPLY).toMessage(GemstoneApply);
        messages.map(GEMSTONE_REMOVE).toMessage(GemstoneRemove);
        messages.map(GEMSTONE_SWAP).toMessage(GemstoneSwap);
        messages.map(TRADEREQUEST).toMessage(TradeRequest);
        messages.map(CHANGETRADE).toMessage(ChangeTrade);
        messages.map(CANCELTRADE).toMessage(CancelTrade);
        messages.map(ACCEPTTRADE).toMessage(AcceptTrade);
        messages.map(PARTYINVITE).toMessage(PartyInvite);
        messages.map(FAILURE).toMessage(Failure).toMethod(this.onFailure);
        messages.map(CREATE_SUCCESS).toMessage(CreateSuccess).toMethod(this.onCreateSuccess);
        messages.map(TEXT).toMessage(Text).toMethod(this.onText);
        messages.map(SERVERPLAYERSHOOT).toMessage(ServerPlayerShoot).toMethod(this.onServerPlayerShoot);
        messages.map(DAMAGE).toMessage(Damage).toMethod(this.onDamage);
        messages.map(UPDATE).toMessage(Update).toMethod(this.onUpdate);
        messages.map(NOTIFICATION).toMessage(Notification).toMethod(this.onNotification);
        messages.map(NEWTICK).toMessage(NewTick).toMethod(this.onNewTick);
        messages.map(SHOWEFFECT).toMessage(ShowEffect).toMethod(this.onShowEffect);
        messages.map(GOTO).toMessage(Goto).toMethod(this.onGoto);
        messages.map(GOTOACK).toMessage(GotoAck);
        messages.map(INVRESULT).toMessage(InvResult).toMethod(this.onInvResult);
        messages.map(RECONNECT).toMessage(Reconnect).toMethod(this.onReconnect);
        messages.map(MAPINFO).toMessage(MapInfo).toMethod(this.onMapInfo);
        messages.map(DEATH).toMessage(Death).toMethod(this.onDeath);
        messages.map(BUYRESULT).toMessage(BuyResult).toMethod(this.onBuyResult);
        messages.map(AOE).toMessage(Aoe).toMethod(this.onAoe);
        messages.map(ACCOUNTLIST).toMessage(AccountList).toMethod(this.onAccountList);
        messages.map(GUILDRESULT).toMessage(GuildResult).toMethod(this.onGuildResult);
        messages.map(ENEMYSHOOT).toMessage(EnemyShoot).toMethod(this.onEnemyShoot);
        messages.map(INVITEDTOGUILD).toMessage(InvitedToGuild).toMethod(this.onInvitedToGuild);
        messages.map(PLAYSOUND).toMessage(PlaySound).toMethod(this.onPlaySound);
        messages.map(STATSAPPLYRESULT).toMessage(StatsApplyResult).toMethod(this.onStatsApplyResult);
        messages.map(TRADEREQUESTED).toMessage(TradeRequested).toMethod(this.onTradeRequested);
        messages.map(TRADESTART).toMessage(TradeStart).toMethod(this.onTradeStart);
        messages.map(TRADECHANGED).toMessage(TradeChanged).toMethod(this.onTradeChanged);
        messages.map(TRADEDONE).toMessage(TradeDone).toMethod(this.onTradeDone);
        messages.map(TRADEACCEPTED).toMessage(TradeAccepted).toMethod(this.onTradeAccepted);
    }

    private function unmapMessages():void {
        var messages:MessageMap = this.injector.getInstance(MessageMap);
        messages.unmap(CREATE);
        messages.unmap(PLAYERSHOOT);
        messages.unmap(MOVE);
        messages.unmap(PLAYERTEXT);
        messages.unmap(INVSWAP);
        messages.unmap(USEITEM);
        messages.unmap(HELLO);
        messages.unmap(INVDROP);
        messages.unmap(LOAD);
        messages.unmap(TELEPORT);
        messages.unmap(USEPORTAL);
        messages.unmap(BUY);
        messages.unmap(PLAYERHIT);
        messages.unmap(ENEMYHIT);
        messages.unmap(AOEACK);
        messages.unmap(SHOOTACK);
        messages.unmap(SQUAREHIT);
        messages.unmap(CREATEGUILD);
        messages.unmap(GUILDREMOVE);
        messages.unmap(GUILDINVITE);
        messages.unmap(ESCAPE);
        messages.unmap(JOINGUILD);
        messages.unmap(CHANGEGUILDRANK);
        messages.unmap(EDITACCOUNTLIST);
        messages.unmap(FAILURE);
        messages.unmap(CREATE_SUCCESS);
        messages.unmap(TEXT);
        messages.unmap(SERVERPLAYERSHOOT);
        messages.unmap(DAMAGE);
        messages.unmap(UPDATE);
        messages.unmap(NOTIFICATION);
        messages.unmap(NEWTICK);
        messages.unmap(SHOWEFFECT);
        messages.unmap(GOTO);
        messages.unmap(INVRESULT);
        messages.unmap(RECONNECT);
        messages.unmap(MAPINFO);
        messages.unmap(DEATH);
        messages.unmap(BUYRESULT);
        messages.unmap(AOE);
        messages.unmap(ACCOUNTLIST);
        messages.unmap(GUILDRESULT);
        messages.unmap(ENEMYSHOOT);
        messages.unmap(INVITEDTOGUILD);
        messages.unmap(PLAYSOUND);
        messages.unmap(OPTIONS_CHANGED);
        messages.unmap(CONSTELLATIONSTRIAL);
        messages.unmap(CONSTELLATIONSSAVE);
        messages.unmap(STATSAPPLY);
        messages.unmap(STATSAPPLYRESULT);
        messages.unmap(GEMSTONE_APPLY);
        messages.unmap(GEMSTONE_REMOVE);
        messages.unmap(GEMSTONE_SWAP);
        messages.unmap(TRADEREQUEST);
        messages.unmap(CHANGETRADE);
        messages.unmap(CANCELTRADE);
        messages.unmap(ACCEPTTRADE);
        messages.unmap(PARTYINVITE);
    }

    public function nextIntRange(min:uint, max:uint):uint {
        return this.rand_.nextIntRange(min, max);
    }

    public function enableJitterWatcher():void {
        if (this.jitterWatcher_ == null) {
            this.jitterWatcher_ = new JitterWatcher();
        }
    }

    public function disableJitterWatcher():void {
        if (this.jitterWatcher_ != null) {
            this.jitterWatcher_ = null;
        }
    }

    private function create():void {
        var charClass:CharacterClass = this.classesModel.getSelected();
        var create:Create = this.messages.require(CREATE) as Create;
        create.classType = charClass.id;
        create.skinType = charClass.skins.getSelectedSkin().id;
        this.serverConnection.sendMessage(create);
    }

    private function load():void {
        var load:Load = this.messages.require(LOAD) as Load;
        load.charId_ = this.gs_.charId_;
        this.serverConnection.sendMessage(load);
    }

    public function playerShoot(angle:Number, projX:Number, projY:Number, time:int, isServerShoot:Boolean, angleInc:Number, damageList:Vector.<int>, critList:Vector.<Number>, itemType:int):void {
        var playerShoot:PlayerShoot = this.messages.require(PLAYERSHOOT) as PlayerShoot;
        playerShoot.angle_ = angle;
        playerShoot.pos.x_ = projX;
        playerShoot.pos.y_ = projY;
        playerShoot.time = time;
        playerShoot.isServerShoot = isServerShoot;
        playerShoot.angleInc = angleInc;
        playerShoot.damageList = damageList;
        playerShoot.critList = critList;
        playerShoot.itemType = itemType;
        this.serverConnection.sendMessage(playerShoot);
    }

    private function onGoto(gotoPkt:Goto):void {
        this.gs_.map.gotoRequested_++;
        var go:GameObject = this.gs_.map.goDict_[playerId_];
        if (go == null) {
            return;
        }
        go.onGoto(gotoPkt.pos_.x_, gotoPkt.pos_.y_, this.gs_.lastUpdate_);
    }

    public function playerHit(ownerId:int, bulletId:uint):void {
        var playerHit:PlayerHit = this.messages.require(PLAYERHIT) as PlayerHit;
        playerHit.ownerId = ownerId;
        playerHit.bulletId_ = bulletId;
        this.serverConnection.sendMessage(playerHit);
    }

    public function enemyHit(bulletId:int, targetId:int, elapsed:int, targetX:Number, targetY:Number):void {
        var enemyHit:EnemyHit = this.messages.require(ENEMYHIT) as EnemyHit;
        enemyHit.bulletId_ = bulletId;
        enemyHit.targetId_ = targetId;
        enemyHit.elapsed_ = elapsed;
        enemyHit.targetPos.x_ = targetX;
        enemyHit.targetPos.y_ = targetY;
        this.serverConnection.sendMessage(enemyHit);
    }

    public function squareHit(time:int, bulletId:int):void {
        var squareHit:SquareHit = this.messages.require(SQUAREHIT) as SquareHit;
        squareHit.time_ = time;
        squareHit.bulletId_ = bulletId;
        this.serverConnection.sendMessage(squareHit);
    }

    public function aoeAck(time:int, x:Number, y:Number):void {
        var aoeAck:AoeAck = this.messages.require(AOEACK) as AoeAck;
        aoeAck.time_ = time;
        aoeAck.position_.x_ = x;
        aoeAck.position_.y_ = y;
        this.serverConnection.sendMessage(aoeAck);
    }

    public function playerText(textStr:String):void {
        var playerTextMessage:PlayerText = this.messages.require(PLAYERTEXT) as PlayerText;
        playerTextMessage.text_ = textStr;
        this.serverConnection.sendMessage(playerTextMessage);
    }


    public function invSwap(player:Player, sourceObj:GameObject, slotId1:int, targetObj:GameObject, slotId2:int):Boolean {
        if (!this.gs_) {
            return false;
        }
        var invSwap:InvSwap = this.messages.require(INVSWAP) as InvSwap;
        invSwap.slotObject1_.objectId_ = sourceObj.objectId_;
        invSwap.slotObject1_.slotId_ = slotId1;
        invSwap.slotObject2_.objectId_ = targetObj.objectId_;
        invSwap.slotObject2_.slotId_ = slotId2;
        this.serverConnection.sendMessage(invSwap);

        var tempItem:ItemData = sourceObj.equipment_[slotId1];
        var tempType:int = sourceObj.itemTypes[slotId1];
        if (tempType != PotionInventoryModel.HEALTH_POTION_ID && tempType != PotionInventoryModel.MAGIC_POTION_ID) {
            sourceObj.equipment_[slotId1] = targetObj.equipment_[slotId2];
            targetObj.equipment_[slotId2] = tempItem;
            sourceObj.itemTypes[slotId1] = targetObj.itemTypes[slotId2];
            targetObj.itemTypes[slotId2] = tempType;
        }

        SoundEffectLibrary.play("inventory_move_item");
        return true;
    }

    public function invSwapPotion(player:Player, sourceObj:GameObject, slotId1:int, itemId:int, targetObj:GameObject, slotId2:int):Boolean {
        if (!this.gs_) {
            return false;
        }
        var invSwap:InvSwap = this.messages.require(INVSWAP) as InvSwap;
        invSwap.slotObject1_.objectId_ = sourceObj.objectId_;
        invSwap.slotObject1_.slotId_ = slotId1;
        invSwap.slotObject2_.objectId_ = targetObj.objectId_;
        invSwap.slotObject2_.slotId_ = slotId2;
        sourceObj.equipment_[slotId1] = null;
        sourceObj.itemTypes[slotId1] = -1;
        this.serverConnection.sendMessage(invSwap);
        SoundEffectLibrary.play("inventory_move_item");
        return true;
    }

    public function invDrop(object:GameObject, slotId:int):void {
        var invDrop:InvDrop = this.messages.require(INVDROP) as InvDrop;
        invDrop.slotId_ = slotId;
        this.serverConnection.sendMessage(invDrop);
        object.equipment_[slotId] = null;
        object.itemTypes[slotId] = -1;
    }

    public function useItem(time:int, objectId:int, slotId:int, posX:Number, posY:Number):void {
        var useItemMess:UseItem = this.messages.require(USEITEM) as UseItem;
        useItemMess.slotObject_.objectId_ = objectId;
        useItemMess.slotObject_.slotId_ = slotId;
        useItemMess.itemUsePos_.x_ = posX;
        useItemMess.itemUsePos_.y_ = posY;
        useItemMess.time = time;
        this.serverConnection.sendMessage(useItemMess);
    }

    public function useItem_new(itemOwner:GameObject, slotId:int, time:int):Boolean {
        var item:ItemData = itemOwner.equipment_[slotId];
        if (item && (item.Consumable || item.InvUse)) {
            this.applyUseItem(itemOwner, slotId, item, time);
            SoundEffectLibrary.play("use_potion");
            return true;
        }
        SoundEffectLibrary.play("error");
        return false;
    }

    private function applyUseItem(owner:GameObject, slotId:int, item:ItemData, time:int):void {
        var useItem:UseItem = this.messages.require(USEITEM) as UseItem;
        useItem.slotObject_.objectId_ = owner.objectId_;
        useItem.slotObject_.slotId_ = slotId;
        useItem.itemUsePos_.x_ = 0;
        useItem.itemUsePos_.y_ = 0;
        useItem.time = time;
        this.serverConnection.sendMessage(useItem);
        if (item.Consumable) {
            owner.equipment_[slotId] = null;
            owner.itemTypes[slotId] = -1;
        }
    }

    public function move(player:Player):void {
        var pX:Number = player.x_;
        var pY:Number = player.y_;

        this.lastPos.x_ = pX;
        this.lastPos.y_ = pY;
        var move:Move = this.messages.require(MOVE) as Move;
        move.newPosition_.x_ = pX;
        move.newPosition_.y_ = pY;
        this.serverConnection.sendMessage(move);
    }

    public function teleport(objectId:int):void {
        var teleport:Teleport = this.messages.require(TELEPORT) as Teleport;
        teleport.objectId_ = objectId;
        this.serverConnection.sendMessage(teleport);
    }

    public function usePortal(objectId:int):void {
        var usePortal:UsePortal = this.messages.require(USEPORTAL) as UsePortal;
        usePortal.objectId_ = objectId;
        this.serverConnection.sendMessage(usePortal);
    }

    public function buy(sellableObjectId:int, currencyType:int):void {
        if (this.outstandingBuy_) {
            return;
        }
        var sObj:SellableObject = this.gs_.map.goDict_[sellableObjectId];
        if (sObj == null) {
            return;
        }
        trace("Indi: TODO I think this can be switched to a Boolean with no consequences");
        this.outstandingBuy_ = new OutstandingBuy(sObj.soldObjectInternalName(), sObj.price_, sObj.currency_);
        var buyMesssage:Buy = this.messages.require(BUY) as Buy;
        buyMesssage.objectId_ = sellableObjectId;
        this.serverConnection.sendMessage(buyMesssage);
    }

    public function gotoAck():void {
        var gotoAck:GotoAck = this.messages.require(GOTOACK) as GotoAck;
        this.serverConnection.sendMessage(gotoAck);
    }

    public function editAccountList(accountListId:int, add:Boolean, objectId:int):void {
        var eal:EditAccountList = this.messages.require(EDITACCOUNTLIST) as EditAccountList;
        eal.accountListId_ = accountListId;
        eal.add_ = add;
        eal.objectId_ = objectId;
        this.serverConnection.sendMessage(eal);
    }

    public function createGuild(name:String):void {
        var createGuild:CreateGuild = this.messages.require(CREATEGUILD) as CreateGuild;
        createGuild.name_ = name;
        this.serverConnection.sendMessage(createGuild);
    }

    public function guildRemove(name:String):void {
        var guildRemove:GuildRemove = this.messages.require(GUILDREMOVE) as GuildRemove;
        guildRemove.name_ = name;
        this.serverConnection.sendMessage(guildRemove);
    }

    public function guildInvite(name:String):void {
        var guildInvite:GuildInvite = this.messages.require(GUILDINVITE) as GuildInvite;
        guildInvite.name_ = name;
        this.serverConnection.sendMessage(guildInvite);
    }

    public function partyInvite(objId:int) : void {
        var partyInvite:PartyInvite = messages.require(PARTYINVITE) as PartyInvite;
        partyInvite.objId = objId;
        serverConnection.sendMessage(partyInvite);
    }

    public function escape():void {
        if (this.playerId_ == -1 || this.gs_.isNexus_) {
            return;
        }
        this.serverConnection.sendMessage(this.messages.require(ESCAPE) as Escape);
    }

    public function constellationsTrial():void {
        if (this.playerId_ == -1)
            return;
        this.serverConnection.sendMessage(this.messages.require(CONSTELLATIONSTRIAL) as ConstellationsTrial);
    }

    public function constellationsSave(savedPrimaries:int, savedSecondaries:int):void {
        if (this.playerId_ == -1)
            return;
        var constellationsSave:ConstellationsSave = this.messages.require(CONSTELLATIONSSAVE) as ConstellationsSave;
        constellationsSave.savedPrimaries = savedPrimaries;
        constellationsSave.savedSecondaries = savedSecondaries;
        this.serverConnection.sendMessage(constellationsSave);
    }

    public function statsApply(allocatedPoints:Array):void {
        if (this.playerId_ == -1)
            return;
        var statsApply:StatsApply = this.messages.require(STATSAPPLY) as StatsApply;
        statsApply.allocatedPoints = allocatedPoints;
        this.serverConnection.sendMessage(statsApply);
    }

    public function joinGuild(guildName:String):void {
        var joinGuild:JoinGuild = this.messages.require(JOINGUILD) as JoinGuild;
        joinGuild.guildName_ = guildName;
        this.serverConnection.sendMessage(joinGuild);
    }

    public function changeGuildRank(name:String, rank:int):void {
        var changeGuildRank:ChangeGuildRank = this.messages.require(CHANGEGUILDRANK) as ChangeGuildRank;
        changeGuildRank.name_ = name;
        changeGuildRank.guildRank_ = rank;
        this.serverConnection.sendMessage(changeGuildRank);
    }

    private function onConnected():void {
        this.conReady = true;
        this.addTextLine.dispatch(new AddTextLineVO(Parameters.CLIENT_CHAT_NAME, "Connected!"));
        this.sendHello();
    }

    public function sendHello():void {
        var account:Account = StaticInjectorContext.getInjector().getInstance(Account);
        var hello:Hello = this.messages.require(HELLO) as Hello;
        hello.buildVersion_ = Parameters.BUILD_VERSION;
        hello.gameId_ = this.gs_.gameId_;
        hello.username_ = account.getUsername();
        hello.password_ = account.getPassword();
        hello.mapJSON_ = this.gs_.mapJSON_ == null ? "" : this.gs_.mapJSON_;
        this.serverConnection.sendMessage(hello);
    }

    private function onCreateSuccess(createSuccess:CreateSuccess):void {
        this.playerId_ = createSuccess.objectId_;
        this.gs_.charId_ = createSuccess.charId_;
        this.gs_.initialize();
        this.gs_.createCharacter_ = false;
    }

    private function onDamage(damage:Damage):void {
        var map:Map = this.gs_.map;
        var target:GameObject = map.goDict_[damage.targetId_];
        if (target != null) {
            target.damage(damage.damageAmount_, damage.effects_, null);
        }
    }

    private function onServerPlayerShoot(serverPlayerShoot:ServerPlayerShoot):void {
        var weapon:ItemData = new ItemData(serverPlayerShoot.itemType_);
        var path:ProjectilePath = ProjectilePath.createFromDesc(weapon.Projectile);
        var pos:Point = new Point(serverPlayerShoot.startingPos_.x_, serverPlayerShoot.startingPos_.y_);

        this.player.doShoot(this.gs_.lastUpdate_, serverPlayerShoot.damageList.length, weapon.ObjectType, weapon.Projectile, serverPlayerShoot.angle_ * Trig.toRadians, serverPlayerShoot.angleInc_ * Trig.toRadians, path, true, pos, serverPlayerShoot.damageList, serverPlayerShoot.critList);
        this.playerShoot(serverPlayerShoot.angle_, serverPlayerShoot.startingPos_.x_, serverPlayerShoot.startingPos_.y_, getTimer(), true, serverPlayerShoot.angleInc_, serverPlayerShoot.damageList, serverPlayerShoot.critList, weapon.ObjectType); // Manually send PlayerShoot with exact angle/pos values received from ServerPlayerShoot
    }

    private function onEnemyShoot(enemyShoot:EnemyShoot):void {
        var owner:GameObject = this.gs_.map.goDict_[enemyShoot.ownerId_];
        if (owner == null) {
            return;
        }

        for (var i:int = 0; i < enemyShoot.numShots_; i++) {
            var proj:Projectile = FreeList.newObject(Projectile) as Projectile;
            var projProps:ProjectileProperties = null;
            if (enemyShoot.projectileId != -1) { // Get the projectile properties from xml if an id was provided
                var enemyDesc:ObjectProperties = ObjectLibrary.propsLibrary_[owner.objectType_];
                projProps = enemyDesc.projectiles_[enemyShoot.projectileId];
            }
            var lifetime:int = projProps ? projProps.lifetime_ : enemyShoot.lifetimeMs_;
            var projType:int = projProps ? ObjectLibrary.getTypeFromId(projProps.objectId_) : enemyShoot.projType_;
            var multiHit:Boolean = projProps ? projProps.multiHit_ : enemyShoot.multiHit_;
            var passesCover:Boolean = projProps ? projProps.passesCover_ : enemyShoot.passesCover_;
            var armorPiercing:Boolean = projProps ? projProps.armorPiercing_ : enemyShoot.armorPiercing_;
            var size:int = projProps ? projProps.size_ : enemyShoot.size;
            var effects:Vector.<uint> = projProps ? projProps.effects_ : enemyShoot.effects;

            var angle:Number = enemyShoot.angle_ + enemyShoot.angleInc_ * i;
            var path:ProjectilePath = projProps ? ProjectilePath.createFromProps(projProps) : enemyShoot.path_.Clone();
            if (projProps) {
                path.segments[0].LifetimeMS = lifetime;
            }
            proj.reset(owner.objectType_, projType, enemyShoot.ownerId_, enemyShoot.bulletId_ + i,
                    angle, this.gs_.lastUpdate_, path, lifetime,
                    multiHit, passesCover, armorPiercing, size, effects);
            proj.setDamage(enemyShoot.damage_, 1);
            this.gs_.map.addObj(proj, enemyShoot.startingPos_.x_, enemyShoot.startingPos_.y_);
        }

        owner.setAttack(owner.objectType_, enemyShoot.angle_ + enemyShoot.angleInc_ * ((enemyShoot.numShots_ - 1) / 2));
    }

    private function dropObject(obj:ObjectDropData):void {
        var go:GameObject = this.gs_.map.goDict_[obj.objectId_];
        if (obj.explode_ && go is Character) {
            (go as Character).explode();
        }
        this.gs_.map.removeObj(obj.objectId_);
    }

    private function addObject(obj:ObjectData):void {
        var map:Map = this.gs_.map;
        var go:GameObject = ObjectLibrary.getObjectFromType(obj.objectType_);
        if (go == null) {
            trace("unhandled object type: " + obj.objectType_);
            return;
        }
        var status:ObjectStatusData = obj.status_;
        go.setObjectId(status.objectId_);
        map.addObj(go, status.pos_.x_, status.pos_.y_);
        if (go is Player) {
            this.handleNewPlayer(go as Player, map);
        }
        this.processObjectStatus(status);
        if (go.props_.static_ && go.props_.occupySquare_ && !go.props_.noMiniMap_) {
            this.updateGameObjectTileSignal.dispatch(new UpdateGameObjectTileVO(go.x_, go.y_, go));
        }
    }

    private function handleNewPlayer(player:Player, map:Map):void {
        this.setPlayerSkinTemplate(player, 0);
        if (player.objectId_ == this.playerId_) {
            this.player = player;
            this.model.player = player;
            map.player_ = player;
            map.mapOverlay_.addAbilityGuideOverlay(player);
            this.gs_.setFocus(player);
            this.setGameFocus.dispatch(this.playerId_.toString());
        }
    }

    private function onUpdate(update:Update):void {
        var i:int = 0;
        var tile:GroundTileData = null;
        for (i = 0; i < update.tiles_.length; i++) {
            tile = update.tiles_[i];
            this.gs_.map.setGroundTile(tile.x_, tile.y_, tile.type_);
            this.updateGroundTileSignal.dispatch(new UpdateGroundTileVO(tile.x_, tile.y_, tile.type_));
        }
        for (i = 0; i < update.newObjs_.length; i++) {
            this.addObject(update.newObjs_[i]);
        }
        for (i = 0; i < update.drops_.length; i++) {
            this.dropObject(update.drops_[i]);
        }
    }

    private function onNotification(notification:Notification):void {
        // used to be queued
        var text:CharacterStatusText = null;
        var go:GameObject = this.gs_.map.goDict_[notification.objectId_];

        if (go != null) {
            if (notification.isDamage){
                go.damageStatus(int(notification.text_), false);
                return;
            }

            text = new CharacterStatusText(go, notification.text_, notification.color_, 2000, 0, notification.size_);
            this.gs_.map.mapOverlay_.addStatusText(text);
            if (go == this.player && notification.text_ == "Quest Complete!") {
                this.gs_.map.quest_.completed();
            }
        }
    }

    private function onStatsApplyResult(result:StatsApplyResult):void {
        this.gs_.statsView.statsApplied(result.success_);
    }

    private function onNewTick(newTick:NewTick):void {
        var objectStatus:ObjectStatusData = null;
        if (this.jitterWatcher_ != null) {
            this.jitterWatcher_.record();
        }
        for each(objectStatus in newTick.statuses_) {
            this.processObjectStatus(objectStatus);
        }
        if (this.player != null && this.player.map_ != null)
            this.player.map_.movesRequested_++;
    }

    private function onShowEffect(showEffect:ShowEffect):void {
        var go:GameObject = null;
        var e:ParticleEffect = null;
        var start:Point = null;
        var map:Map = this.gs_.map;
        switch (showEffect.effectType_) {
            case ShowEffect.HEAL_EFFECT_TYPE:
                go = map.goDict_[showEffect.targetObjectId_];
                if (go == null) {
                    break;
                }
                map.addObj(new HealEffect(go, showEffect.color_), go.x_, go.y_);
                break;
            case ShowEffect.TELEPORT_EFFECT_TYPE:
                map.addObj(new TeleportEffect(), showEffect.pos1_.x_, showEffect.pos1_.y_);
                break;
            case ShowEffect.STREAM_EFFECT_TYPE:
                e = new StreamEffect(showEffect.pos1_, showEffect.pos2_, showEffect.color_);
                map.addObj(e, showEffect.pos1_.x_, showEffect.pos1_.y_);
                break;
            case ShowEffect.THROW_EFFECT_TYPE:
                go = map.goDict_[showEffect.targetObjectId_];
                start = go != null ? new Point(go.x_, go.y_) : showEffect.pos2_.toPoint();
                e = new ThrowEffect(start, showEffect.pos1_.toPoint(), showEffect.color_, showEffect.effectParam_);
                map.addObj(e, start.x, start.y);
                break;
            case ShowEffect.NOVA_EFFECT_TYPE:
//                go = map.goDict_[showEffect.targetObjectId_];
//                if (go == null) {
//                    break;
//                }
                e = new NovaEffect(showEffect.pos1_.x_, showEffect.pos1_.y_, showEffect.effectParam_, showEffect.color_);
                map.addObj(e, showEffect.pos1_.x_, showEffect.pos1_.y_);
                break;
            case ShowEffect.POISON_EFFECT_TYPE:
                go = map.goDict_[showEffect.targetObjectId_];
                if (go == null) {
                    break;
                }
                e = new PoisonEffect(go, showEffect.color_);
                map.addObj(e, go.x_, go.y_);
                break;
            case ShowEffect.LINE_EFFECT_TYPE:
                go = map.goDict_[showEffect.targetObjectId_];
                if (go == null) {
                    break;
                }
                e = new LineEffect(go, showEffect.pos1_, showEffect.color_);
                map.addObj(e, showEffect.pos1_.x_, showEffect.pos1_.y_);
                break;
            case ShowEffect.BURST_EFFECT_TYPE:
                go = map.goDict_[showEffect.targetObjectId_];
                if (go == null) {
                    break;
                }
                e = new BurstEffect(go, showEffect.pos1_, showEffect.pos2_, showEffect.color_);
                map.addObj(e, showEffect.pos1_.x_, showEffect.pos1_.y_);
                break;
            case ShowEffect.FLOW_EFFECT_TYPE:
                go = map.goDict_[showEffect.targetObjectId_];
                if (go == null) {
                    break;
                }
                e = new FlowEffect(showEffect.pos1_, go, showEffect.color_);
                map.addObj(e, showEffect.pos1_.x_, showEffect.pos1_.y_);
                break;
            case ShowEffect.RING_EFFECT_TYPE:
                go = map.goDict_[showEffect.targetObjectId_];
                if (go == null) {
                    break;
                }
                e = new RingEffect(go, showEffect.pos1_.x_, showEffect.color_);
                map.addObj(e, go.x_, go.y_);
                break;
            case ShowEffect.LIGHTNING_EFFECT_TYPE:
                go = map.goDict_[showEffect.targetObjectId_];
                if (go == null) {
                    break;
                }
                e = new LightningEffect(go, showEffect.pos1_, showEffect.color_, showEffect.pos2_.x_);
                map.addObj(e, go.x_, go.y_);
                break;
            case ShowEffect.COLLAPSE_EFFECT_TYPE:
                go = map.goDict_[showEffect.targetObjectId_];
                if (go == null) {
                    break;
                }
                e = new CollapseEffect(go, showEffect.pos1_, showEffect.pos2_, showEffect.color_);
                map.addObj(e, showEffect.pos1_.x_, showEffect.pos1_.y_);
                break;
            case ShowEffect.CONEBLAST_EFFECT_TYPE:
                go = map.goDict_[showEffect.targetObjectId_];
                if (go == null) {
                    break;
                }
                e = new ConeBlastEffect(go, showEffect.pos1_, showEffect.pos2_.x_, showEffect.color_);
                map.addObj(e, go.x_, go.y_);
                break;
            case ShowEffect.JITTER_EFFECT_TYPE:
                this.gs_.camera_.startJitter();
                break;
            case ShowEffect.FLASH_EFFECT_TYPE:
                go = map.goDict_[showEffect.targetObjectId_];
                if (go == null) {
                    break;
                }
                go.flash_ = new FlashDescription(getTimer(), showEffect.color_, showEffect.pos1_.x_, showEffect.pos1_.y_);
                break;
            case ShowEffect.THROW_PROJECTILE_EFFECT_TYPE:
                start = showEffect.pos1_.toPoint();
                e = new ThrowProjectileEffect(showEffect.color_, showEffect.pos2_.toPoint(), showEffect.pos1_.toPoint());
                map.addObj(e, start.x, start.y);
                break;
            case ShowEffect.SHEATHE_SLASH_EFFECT_TYPE:
                go = map.goDict_[showEffect.targetObjectId_];
                if (go == null) {
                    break;
                }
                e = new SheatheSlashEffect(0xFFFFFF, 100, 20);
                map.addObj(e, go.x_, go.y_);
                break;
            default:
                trace("ERROR: Unknown Effect type: " + showEffect.effectType_);
        }
    }

    private function updateGameObject(go:GameObject, stats:Vector.<StatData>, isMyObject:Boolean):void {
        var stat:StatData = null;
        var value:int = 0;
        var floatValue:Number = 0;
        var index:int = 0;
        var player:Player = go as Player;
        var merchant:Merchant = go as Merchant;
        for each(stat in stats) {
            if (stat.statType_ == 255) {
                continue;
            }
            value = stat.statValue_;
            floatValue = stat.floatValue_;
            switch (stat.statType_) {
                case StatData.MAX_HP_STAT:
                    go.maxHP = value;
                    continue;
                case StatData.HP_STAT:
                    go.hp = value;
                    continue;
                case StatData.SIZE_STAT:
                    go.size_ = value;
                    continue;
                case StatData.MAX_MP_STAT:
                    player.maxMP = value;
                    continue;
                case StatData.MP_STAT:
                    player.mp = value;
                    continue;
                case StatData.NEXT_LEVEL_EXP_STAT:
                    player.nextLevelXp_ = value;
                    continue;
                case StatData.EXP_STAT:
                    player.experience_ = value;
                    continue;
                case StatData.LEVEL_STAT:
                    go.level_ = value;
                    continue;
                case StatData.STAT_POINTS_STAT:
                    go.statPoints_ = value;
                    continue;
                case StatData.ATTACK_STAT:
                    player.attack = value;
                    continue;
                case StatData.ARMOR_STAT:
                    go.armor = value;
                    continue;
                case StatData.MOVEMENT_SPEED_STAT:
                    player.movementSpeed = roundToPrecision(floatValue, 1);
                    continue;
                case StatData.DEXTERITY_STAT:
                    player.dexterity = value;
                    continue;
                case StatData.LIFE_REGENERATION_STAT:
                    player.lifeRegeneration = value;
                    continue;
                case StatData.WISDOM_STAT:
                    player.wisdom = value;
                    continue;
                case StatData.CONDITION1_STAT:
                    go.condition_[ConditionEffect.CE_FIRST_BATCH] = value;
                    continue;
                case StatData.INVENTORY_0_STAT:
                case StatData.INVENTORY_1_STAT:
                case StatData.INVENTORY_2_STAT:
                case StatData.INVENTORY_3_STAT:
                case StatData.INVENTORY_4_STAT:
                case StatData.INVENTORY_5_STAT:
                case StatData.INVENTORY_6_STAT:
                case StatData.INVENTORY_7_STAT:
                case StatData.INVENTORY_8_STAT:
                case StatData.INVENTORY_9_STAT:
                case StatData.INVENTORY_10_STAT:
                case StatData.INVENTORY_11_STAT:
                    index = stat.statType_ - StatData.INVENTORY_0_STAT;
                    if (value == -1)
                        go.equipment_[index] = null;
                    else if (!go.equipment_[index] || (go.equipment_[index] && value != go.equipment_[index].ObjectType))
                        go.equipment_[index] = new ItemData(value);
                    go.itemTypes[index] = value;
                    continue;
                case StatData.NUM_STARS_STAT:
                    player.numStars_ = value;
                    continue;
                case StatData.NAME_STAT:
                    if (go.name_ != stat.strStatValue_) {
                        go.name_ = stat.strStatValue_;
                        go.nameBitmapData_ = null;
                    }
                    continue;
                case StatData.TEX1_STAT:
                    go.setTex1(value);
                    continue;
                case StatData.TEX2_STAT:
                    go.setTex2(value);
                    continue;
                case StatData.MERCHANDISE_TYPE_STAT:
                    merchant.setMerchandiseType(value);
                    continue;
                case StatData.CREDITS_STAT:
                    player.setCredits(value);
                    continue;
                case StatData.MERCHANDISE_PRICE_STAT:
                    (go as SellableObject).setPrice(value);
                    continue;
                case StatData.ACTIVE_STAT:
                    (go as Portal).active_ = value != 0;
                    continue;
                case StatData.ACCOUNT_ID_STAT:
                    player.accountId_ = value;
                    continue;
                case StatData.FAME_STAT:
                    player.fame_ = value;
                    continue;
                case StatData.MERCHANDISE_CURRENCY_STAT:
                    (go as SellableObject).setCurrency(value);
                    continue;
                case StatData.CONNECT_STAT:
                    go.connectType_ = value;
                    continue;
                case StatData.MERCHANDISE_COUNT_STAT:
                    merchant.count_ = value;
                    merchant.untilNextMessage_ = 0;
                    continue;
                case StatData.MERCHANDISE_MINS_LEFT_STAT:
                    merchant.minsLeft_ = value;
                    merchant.untilNextMessage_ = 0;
                    continue;
                case StatData.MERCHANDISE_DISCOUNT_STAT:
                    merchant.discount_ = value;
                    merchant.untilNextMessage_ = 0;
                    continue;
                case StatData.MERCHANDISE_RANK_REQ_STAT:
                    (go as SellableObject).setRankReq(value);
                    continue;
                case StatData.CHAR_FAME_STAT:
                    player.charFame_ = value;
                    continue;
                case StatData.NEXT_CLASS_QUEST_FAME_STAT:
                    player.nextClassQuestFame_ = value;
                    continue;
                case StatData.LEGENDARY_RANK_STAT:
                    player.legendaryRank_ = value;
                    continue;
                case StatData.SINK_LEVEL_STAT:
                    if (!isMyObject) {
                        player.sinkLevel_ = value;
                    }
                    continue;
                case StatData.ALT_TEXTURE_STAT:
                    go.setAltTexture(value);
                    continue;
                case StatData.GUILD_NAME_STAT:
                    player.setGuildName(stat.strStatValue_);
                    continue;
                case StatData.GUILD_RANK_STAT:
                    player.guildRank_ = value;
                    continue;
                case StatData.OXYGEN_STAT:
                    player.oxygen_ = value;
                    continue;
                case StatData.HEALTH_POTION_STACK_STAT:
                    player.healthPotionCount_ = value;
                    continue;
                case StatData.MAGIC_POTION_STACK_STAT:
                    player.magicPotionCount_ = value;
                    continue;
                case StatData.TEXTURE_STAT:
                    player.skinId != value && this.setPlayerSkinTemplate(player, value);
                    continue;
                case StatData.HASBACKPACK_STAT:
                    (go as Player).hasBackpack_ = Boolean(value);
                    if (isMyObject) {
                        this.updateBackpackTab.dispatch(Boolean(value));
                    }
                    continue;
                case StatData.BACKPACK_0_STAT:
                case StatData.BACKPACK_1_STAT:
                case StatData.BACKPACK_2_STAT:
                case StatData.BACKPACK_3_STAT:
                case StatData.BACKPACK_4_STAT:
                case StatData.BACKPACK_5_STAT:
                case StatData.BACKPACK_6_STAT:
                case StatData.BACKPACK_7_STAT:
                    index = stat.statType_ - StatData.BACKPACK_0_STAT + GeneralConstants.NUM_EQUIPMENT_SLOTS + GeneralConstants.NUM_INVENTORY_SLOTS;
                    if (value == -1)
                        go.equipment_[index] = null;
                    else if (!go.equipment_[index] || (go.equipment_[index] && value != go.equipment_[index].ObjectType))
                        go.equipment_[index] = new ItemData(value);
                    go.itemTypes[index] = value;
                    continue;
                case StatData.INVENTORYDATA_0_STAT:
                case StatData.INVENTORYDATA_1_STAT:
                case StatData.INVENTORYDATA_2_STAT:
                case StatData.INVENTORYDATA_3_STAT:
                case StatData.INVENTORYDATA_4_STAT:
                case StatData.INVENTORYDATA_5_STAT:
                case StatData.INVENTORYDATA_6_STAT:
                case StatData.INVENTORYDATA_7_STAT:
                case StatData.INVENTORYDATA_8_STAT:
                case StatData.INVENTORYDATA_9_STAT:
                case StatData.INVENTORYDATA_10_STAT:
                case StatData.INVENTORYDATA_11_STAT:
                case StatData.INVENTORYDATA_12_STAT:
                case StatData.INVENTORYDATA_13_STAT:
                case StatData.INVENTORYDATA_14_STAT:
                case StatData.INVENTORYDATA_15_STAT:
                case StatData.INVENTORYDATA_16_STAT:
                case StatData.INVENTORYDATA_17_STAT:
                case StatData.INVENTORYDATA_18_STAT:
                case StatData.INVENTORYDATA_19_STAT:
                    index = stat.statType_ - StatData.INVENTORYDATA_0_STAT;
                    if (go.equipment_[index] != null) {
//                        trace(stat.strStatValue_);
                        go.equipment_[index].ParseData(ItemData.GetStream(stat.strStatValue_));
                    }
                    continue;
                case StatData.PRIMARY_CONSTELLATION_STAT:
                    player.primaryConstellation = value;
                    continue;
                case StatData.SECONDARY_CONSTELLATION_STAT:
                    player.secondaryConstellation = value;
                    continue;
                case StatData.PRIMARY_NODE_DATA_STAT:
                    player.primaryNodeData = value;
                    player.primaryNodes = convertNodeData(value);
                    continue;
                case StatData.SECONDARY_NODE_DATA_STAT:
                    player.secondaryNodeData = value;
                    player.secondaryNodes = convertNodeData(value);
                    continue;
                case StatData.DODGE_CHANCE_STAT:
                    player.dodgeChance = roundToPrecision(floatValue, 1); //server sends 3.1, client reads 3.09999999999, so i round :cry:
                    continue;
                case StatData.DEFENSE_STAT:
                    player.defense = value;
                    continue;
                case StatData.CRITICAL_CHANCE_STAT:
                    player.criticalChance = roundToPrecision(floatValue, 1);
                    continue;
                case StatData.CRITICAL_DAMAGE_STAT:
                    player.criticalDamage = value;
                    continue;
                case StatData.MAX_MS_STAT:
                    go.maxMS = value;
                    continue;
                case StatData.MS_STAT:
                    go.ms = value;
                    continue;
                case StatData.MANA_REGENERATION_STAT:
                    player.manaRegeneration = value;
                    continue;
                case StatData.MS_REGEN_RATE_STAT:
                    player.msRegenRate = value;
                    continue;
                case StatData.DAMAGE_MULTIPLIER_STAT:
                    player.damageMultiplier = value;
                    continue;
                case StatData.TIME_IN_COMBAT_STAT:
                    player.timeInCombat = value;
                    continue;
                case StatData.ATTACK_SPEED_STAT:
                    player.attackSpeed = roundToPrecision(floatValue, 2);
                    continue;
                case StatData.CONDITION2_STAT:
                    go.condition_[ConditionEffect.CE_SECOND_BATCH] = value;
                    continue;
                case StatData.ACC_RANK_STAT:
                    go.accRank = value;
                    continue;
                case StatData.QUEST_ID:
                    this.gs_.map.quest_.setObject(value);
                    continue;
                case StatData.PARTY_ID:
                    player.partyId = value;
                    continue;
                case StatData.ABILITY_DATA_A:
                    player.abilityDataA = stat.strStatValue_;
                    continue;
                case StatData.ABILITY_DATA_B:
                    player.abilityDataB = stat.strStatValue_;
                    continue;
                case StatData.ABILITY_DATA_C:
                    player.abilityDataC = stat.strStatValue_;
                    continue;
                case StatData.ABILITY_DATA_D:
                    player.abilityDataD = stat.strStatValue_;
                    continue;
                default:
                    trace("unhandled stat: " + stat.statType_ + " Str: " + stat.strStatValue_ + " Value: " + stat.statValue_);
            }
        }
    }

    public static function roundToPrecision(value:Number, precision:int):Number {
        var multiplier:Number = Math.pow(10, precision);
        return Math.round(value * multiplier) / multiplier;
    }

    public function convertNodeData(data:int):Vector.<int> {
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

    private function setPlayerSkinTemplate(player:Player, skinId:int):void {
        var message:Reskin = this.messages.require(RESKIN) as Reskin;
        message.skinID = skinId;
        message.player = player;
        message.consume();
    }

    private function processObjectStatus(objectStatus:ObjectStatusData):void {
        var pLevel:int = -1;
        var pExp:int = -1;
        var pFame:int = -1;
        var type:CharacterClass = null;
        var map:Map = this.gs_.map;
        var go:GameObject = map.goDict_[objectStatus.objectId_];
        if (go == null) {
            trace("missing object: " + objectStatus.objectId_);
            return;
        }
        var isMyObject:Boolean = objectStatus.objectId_ == this.playerId_;
        if (!isMyObject) {
            go.onTickPos(objectStatus.pos_.x_, objectStatus.pos_.y_);
        }
        var player:Player = go as Player;
        if (player != null) {
            pLevel = player.level_;
            pExp = player.experience_;
            pFame = player.charFame_;
        }
        this.updateGameObject(go, objectStatus.stats_, isMyObject);
        if (player != null && pLevel != -1) {
            if (player.level_ > pLevel) {
                if (isMyObject) {
                    player.handleLevelUp();
                } else {
                    player.levelUpEffect("Level Up!");
                }
            } else if (player.experience_ > pExp) {
                player.handleExpUp(player.experience_ - pExp);
                if (player.charFame_ > pFame) {
                    player.handleFameUp(player.charFame_ - pFame)
                }
            }
        }
    }

    private function onText(text:Text):void {
        var go:GameObject = null;
        var colors:Vector.<uint> = null;
        var speechBalloonvo:AddSpeechBalloonVO = null;
        var textString:String = text.text_;
        if (text.objectId_ >= 0) {
            go = this.gs_.map.goDict_[text.objectId_];
            if (go != null) {
                colors = NORMAL_SPEECH_COLORS;
                if (go.props_.isEnemy_) {
                    colors = ENEMY_SPEECH_COLORS;
                } else if (text.recipient_ == Parameters.GUILD_CHAT_NAME) {
                    colors = GUILD_SPEECH_COLORS;
                } else if (text.recipient_ == Parameters.PARTY_CHAT_NAME) {
                    colors = PARTY_SPEECH_COLORS;
                } else if (text.recipient_ != "") {
                    colors = TELL_SPEECH_COLORS;
                }
                speechBalloonvo = new AddSpeechBalloonVO(go, textString, colors[0], 1, colors[1], 1, colors[2], text.bubbleTime_, false, true);
                this.addSpeechBalloon.dispatch(speechBalloonvo);
            }
        }
        this.addTextLine.dispatch(new AddTextLineVO(text.name_, textString, text.objectId_, text.numStars_, text.recipient_));
    }

    private function onInvResult(invResult:InvResult):void {
        if (invResult.result_ != 0) {
            this.handleInvFailure();
        }
    }

    private function handleInvFailure():void {
        SoundEffectLibrary.play("error");
        this.gs_.hudView.interactPanel.redraw();
    }

    private function onReconnect(reconnect:Reconnect):void {
        var gameID:int = reconnect.gameId_;
        var createChar:Boolean = this.gs_.createCharacter_;
        var charId:int = this.gs_.charId_;
        var reconnectEvent:ReconnectEvent = new ReconnectEvent(gameID, createChar, charId);
        this.gs_.dispatchEvent(reconnectEvent);
    }

    private function parseXML(xmlString:String):void {
        var extraXML:XML = XML(xmlString);
        GroundLibrary.parseFromXML(extraXML);
        ObjectLibrary.parseFromXML(extraXML);
        ObjectLibrary.parseFromXML(extraXML);
    }

    public function optionsChanged():void {
        var packet:OptionsChanged = this.messages.require(OPTIONS_CHANGED) as OptionsChanged;
        packet.allyShots = Parameters.data_.allyShotsList;
        packet.allyDamage = Parameters.data_.allyDamageList;
        packet.allyNotifs = Parameters.data_.allyNotifsList;
        packet.allyParticles = Parameters.data_.allyParticlesList;
        packet.allyEntities = Parameters.data_.allyEntitiesList;
        packet.damageCounter = Parameters.data_.damageCounter;
        this.serverConnection.sendMessage(packet);
    }

    private function onMapInfo(mapInfo:MapInfo):void {
        this.gs_.applyMapInfo(mapInfo);
        this.rand_ = new Random(mapInfo.fp_);
        if (this.gs_.createCharacter_) {
            this.create();
        } else {
            this.load();
        }

        // Send current options to server user
        optionsChanged();
    }

    private function onDeath(death:Death):void {
        this.death = death;
        var data:BitmapData = new BitmapData(this.gs_.width, this.gs_.height);
        Parameters.data_.GPURender = false;
        if (this.gs_.focus) {
            this.gs_.camera_.configureCamera(this.gs_.focus);
        }
        this.gs_.map.draw(this.gs_.camera_, this.gs_.lastUpdate_);
        data.draw(this.gs_);
        death.background = data;
        if (!this.gs_.isEditor) {
            this.handleDeath.dispatch(death);
        }
    }

    private function onBuyResult(buyResult:BuyResult):void {
        if (buyResult.result_ == BuyResult.SUCCESS_BRID) {
            if (this.outstandingBuy_) {
            }
        }
        this.outstandingBuy_ = null;
        switch (buyResult.result_) {
            case BuyResult.DIALOG_BRID:
                StaticInjectorContext.getInjector().getInstance(OpenDialogSignal).dispatch(new MessageCloseDialog("Purchase Error", buyResult.resultString_));
                break;
            default:
                this.addTextLine.dispatch(new AddTextLineVO(buyResult.result_ == BuyResult.SUCCESS_BRID ? Parameters.SERVER_CHAT_NAME : Parameters.ERROR_CHAT_NAME, buyResult.resultString_));
        }
    }

    private function onAccountList(accountList:AccountList):void {
        if (accountList.accountListId_ == 0) {
            this.gs_.map.party_.setStars(accountList);
        }
        if (accountList.accountListId_ == 1) {
            this.gs_.map.party_.setIgnores(accountList);
        }
    }

    private function onAoe(aoe:Aoe):void {
        var d:int = 0;
        var effects:Vector.<uint> = null;
        var e:AOEEffect = new AOEEffect(aoe.pos_.toPoint(), aoe.radius_, aoe.color_);
        this.gs_.map.addObj(e, aoe.pos_.x_, aoe.pos_.y_);
        if (this.player.isInvincible()) {
            this.aoeAck(this.gs_.lastUpdate_, this.player.x_, this.player.y_);
            return;
        }
        var hit:Boolean = this.player.distTo(aoe.pos_) < aoe.radius_;
        if (hit) {
            d = GameObject.damageWithDefense(aoe.damage_, this.player.armor, false, this.player.condition_);
            effects = null;
            if (aoe.effect_ != 0) {
                effects = new Vector.<uint>();
                effects.push(aoe.effect_);
            }
            this.player.damage(d, effects, null);
        }
        this.aoeAck(this.gs_.lastUpdate_, this.player.x_, this.player.y_);
    }

    private function onGuildResult(guildResult:GuildResult):void {
        this.addTextLine.dispatch(new AddTextLineVO(Parameters.ERROR_CHAT_NAME, guildResult.errorText_));
        this.gs_.dispatchEvent(new GuildResultEvent(guildResult.success_, guildResult.errorText_));
    }

    private function onInvitedToGuild(invitedToGuild:InvitedToGuild):void {
        if (Parameters.data_.showGuildInvitePopup) {
            this.gs_.hudView.interactPanel.setOverride(new GuildInvitePanel(this.gs_, invitedToGuild.name_, invitedToGuild.guildName_));
        }
        this.addTextLine.dispatch(new AddTextLineVO("", "You have been invited by " + invitedToGuild.name_ + " to join the guild " + invitedToGuild.guildName_ + ".\n  If you wish to join type \"/join " + invitedToGuild.guildName_ + "\""));
    }

    private function onPlaySound(playSound:PlaySound):void {
        SoundEffectLibrary.play(playSound.sound_);
    }

    private function onClosed():void {
        this.gs_.toHomeScreen = true;
        this.gs_.closed.dispatch();
    }

    private function onError(error:String):void {
        this.addTextLine.dispatch(new AddTextLineVO(Parameters.ERROR_CHAT_NAME, error));
    }

    private function onFailure(event:Failure):void {
        switch (event.errorId_) {
            case Failure.INCORRECT_VERSION:
                this.handleIncorrectVersionFailure(event);
                break;
            case Failure.FORCE_CLOSE_GAME:
                this.handleForceCloseGameFailure(event);
                break;
            case Failure.INVALID_TELEPORT_TARGET:
                this.handleInvalidTeleportTarget(event);
                break;
            default:
                this.handleDefaultFailure(event);
        }
    }

    private function handleInvalidTeleportTarget(event:Failure):void {
        this.addTextLine.dispatch(new AddTextLineVO(Parameters.ERROR_CHAT_NAME, event.errorDescription_));
        this.player.nextTeleportAt_ = 0;
    }

    private function handleForceCloseGameFailure(event:Failure):void {
        this.addTextLine.dispatch(new AddTextLineVO(Parameters.ERROR_CHAT_NAME, event.errorDescription_));
        this.gs_.toHomeScreen = true;
        this.gs_.closed.dispatch();
    }

    private function handleIncorrectVersionFailure(event:Failure):void {
        var dialog:Dialog = new Dialog("Client version: " + Parameters.BUILD_VERSION + "\nServer version: " + event.errorDescription_, "Client Update Needed", "Ok", null);
        dialog.addEventListener(Dialog.BUTTON1_EVENT, this.onDoClientUpdate);
        this.gs_.stage.addChild(dialog);
    }

    private function handleDefaultFailure(event:Failure):void {
        this.addTextLine.dispatch(new AddTextLineVO(Parameters.ERROR_CHAT_NAME, event.errorDescription_));
    }

    private function onDoClientUpdate(event:Event):void {
        var dialog:Dialog = event.currentTarget as Dialog;
        dialog.parent.removeChild(dialog);
        this.gs_.toHomeScreen = true;
        this.gs_.closed.dispatch();
    }

    public function gemstoneApply(itemSlot:int, gemSlot:int, invSlot:int):void {
        var gemstoneApply:GemstoneApply = this.messages.require(GEMSTONE_APPLY) as GemstoneApply;
        gemstoneApply.itemSlot = itemSlot;
        gemstoneApply.gemSlot = gemSlot;
        gemstoneApply.invSlot = invSlot;
        this.serverConnection.sendMessage(gemstoneApply);
    }

    public function gemstoneRemove(itemSlot:int, gemSlot:int, invSlot:int):void {
        var gemstoneRemove:GemstoneRemove = this.messages.require(GEMSTONE_REMOVE) as GemstoneRemove;
        gemstoneRemove.itemSlot = itemSlot;
        gemstoneRemove.gemSlot = gemSlot;
        gemstoneRemove.invSlot = invSlot;
        this.serverConnection.sendMessage(gemstoneRemove);
    }

    public function gemstoneSwap(itemSlot:int, gemSlot:int, gemSlot2:int):void {
        var gemstoneSwap:GemstoneSwap = this.messages.require(GEMSTONE_SWAP) as GemstoneSwap;
        gemstoneSwap.itemSlot = itemSlot;
        gemstoneSwap.gemSlot = gemSlot;
        gemstoneSwap.invSlot = gemSlot2;
        this.serverConnection.sendMessage(gemstoneSwap);
    }

    public function tradeRequest(name:String):void {
        var requestTrade:TradeRequest = this.messages.require(TRADEREQUEST) as TradeRequest;
        requestTrade.name = name;
        this.serverConnection.sendMessage(requestTrade);
    }

    public function changeTrade(offer:Vector.<Boolean>):void {
        var changeTrade:ChangeTrade = this.messages.require(CHANGETRADE) as ChangeTrade;
        changeTrade.offer_ = offer;
        this.serverConnection.sendMessage(changeTrade);
    }

    public function cancelTrade():void {
        this.serverConnection.sendMessage(this.messages.require(CANCELTRADE));
    }

    public function acceptTrade(myOffer:Vector.<Boolean>, yourOffer:Vector.<Boolean>):void {
        var acceptTrade:AcceptTrade = this.messages.require(ACCEPTTRADE) as AcceptTrade;
        acceptTrade.myOffer_ = myOffer;
        acceptTrade.yourOffer_ = yourOffer;
        this.serverConnection.sendMessage(acceptTrade);
    }

    private function onTradeRequested(tradeRequested:TradeRequested):void {
        if (Parameters.data_.showTradePopup) {
            this.gs_.hudView.interactPanel.setOverride(new TradeRequestPanel(this.gs_, tradeRequested.name));
        }
        this.addTextLine.dispatch(new AddTextLineVO("", tradeRequested.name + " wants to " + "trade with you.  Type \"/trade " + tradeRequested.name + "\" to trade."));
    }

    private function onTradeStart(tradeStart:TradeStart):void {
        if (Parameters.data_.showTradePopup)
            this.gs_.hudView.interactPanel.removeOverride();
        this.gs_.hudView.startTrade(this.gs_, tradeStart);
    }

    private function onTradeChanged(tradeChanged:TradeChanged):void {
        this.gs_.hudView.tradeChanged(tradeChanged);
    }

    private function onTradeDone(tradeDone:TradeDone):void {
        this.gs_.hudView.tradeDone();
        switch (tradeDone.code_) {
            case TradeDone.TRADE_SUCCESSFUL:
            case TradeDone.PLAYER_CANCELED:
                this.addTextLine.dispatch(new AddTextLineVO("", tradeDone.description_));
                break;
            case TradeDone.TRADE_ERROR:
                this.addTextLine.dispatch(new AddTextLineVO(Parameters.ERROR_CHAT_NAME, tradeDone.description_));
                break;
        }
    }

    private function onTradeAccepted(tradeAccepted:TradeAccepted):void {
        this.gs_.hudView.tradeAccepted(tradeAccepted);
    }
}
}
