package com.company.assembleegameclient.parameters {
import com.company.assembleegameclient.game.MapUserInput;
import com.company.assembleegameclient.ui.damagecounter.DamageCounterType;
import com.company.util.KeyCodes;
import flash.display.DisplayObject;
import flash.events.Event;
import flash.net.SharedObject;
import flash.utils.Dictionary;

public class Parameters {

    public static var root:DisplayObject;

    public static const SPIEGEL:String = "Spiegel";
    public static const BEAUFORT:String = "BeaufortPro";

    public static const BUILD_VERSION:String = "0.3.3";
    public static const SERVER_ADDRESS:String = "127.0.0.1";
    public static const GAME_PORT:int = 2050;
    public static const APP_PORT:int = 80;

    public static const SERVER_CHAT_NAME:String = "";
    public static const CLIENT_CHAT_NAME:String = "*Client*";
    public static const ERROR_CHAT_NAME:String = "*Error*";
    public static const HELP_CHAT_NAME:String = "*Help*";
    public static const GUILD_CHAT_NAME:String = "*Guild*";
    public static const PARTY_CHAT_NAME:String = "*Party*";

    public static const FELLOW_GUILD_COLOR:uint = 10944349;
    public static const NAME_COLOUR:uint = 16572160;

    public static const GUILD_CREATION_PRICE:int = 1000;
    public static const CHARACTER_SKIN_PRICE:int = 1000;

    public static const MAX_SINK_LEVEL:Number = 18;
    public static const PLAYER_ROTATE_SPEED:Number = 0.003;
    public static const BREATH_THRESH:int = 20;

    public static const NEXUS_GAMEID:int = -1;
    public static const TEST_GAMEID:int = -2;

    public static var data_:Object = null;
    public static var GPURenderError:Boolean = false;
    public static var GPURenderFrame:Boolean = false;

    private static var savedOptions_:SharedObject = null;
    private static var keyNames_:Dictionary = new Dictionary();

    public function Parameters() {
        super();
    }

    public static function appServerAddress():String {
        return "http://" + SERVER_ADDRESS + ":" + APP_PORT;
    }

    public static function getSoundsAddress():String { // SFX only
        return "http://zolmex.github.io/astrum-core";
    }

    public static function getSongsAddress():String { // Music
        return "http://zolmex.github.io/astrum";
    }

    public static function load():void {
        try {
            savedOptions_ = SharedObject.getLocal("OWSettings", "/");
            data_ = savedOptions_.data;
        }
        catch (error:Error) {
            trace("WARNING: unable to save settings");
            data_ = new Object();
        }
        setDefaults();
        save();
    }

    public static function save(mui:MapUserInput = null):void {
        try {
            if (savedOptions_) {
                savedOptions_.flush();
                if (mui)
                    mui.updateKeyActionMappings();
            }
        }
        catch (error:Error) {
        }
    }

    private static function setDefaultKey(keyName:String, key:uint):void {
        if (!data_.hasOwnProperty(keyName)) {
            data_[keyName] = key;
        }
        keyNames_[keyName] = true;
    }

    public static function setKey(keyName:String, key:uint):void {
        var otherKeyName:* = null;
        for (otherKeyName in keyNames_) {
            if (data_[otherKeyName] == key) {
                data_[otherKeyName] = KeyCodes.UNSET;
            }
        }
        data_[keyName] = key;
    }

    private static function setDefault(keyName:String, value:*):void {
        if (!data_.hasOwnProperty(keyName)) {
            data_[keyName] = value;
        }
    }

    public static function isGpuRender():Boolean {
        return !GPURenderError && data_.GPURender;
    }

    public static function clearGpuRenderEvent(event:Event):void {
        clearGpuRender();
    }

    public static function clearGpuRender():void {
        GPURenderError = true;
    }

    public static function setDefaults():void {
        setDefaultKey("moveLeft", KeyCodes.A);
        setDefaultKey("moveRight", KeyCodes.D);
        setDefaultKey("moveUp", KeyCodes.W);
        setDefaultKey("moveDown", KeyCodes.S);
        setDefaultKey("rotateLeft", KeyCodes.Q);
        setDefaultKey("rotateRight", KeyCodes.E);
        setDefaultKey("useSpecial", KeyCodes.SPACE);
        setDefaultKey("interact", KeyCodes.NUMBER_0);
        setDefaultKey("useEquipInvSlot1", KeyCodes.NUMBER_1);
        setDefaultKey("useEquipInvSlot2", KeyCodes.NUMBER_2);
        setDefaultKey("useEquipInvSlot3", KeyCodes.NUMBER_3);
        setDefaultKey("useEquipInvSlot4", KeyCodes.NUMBER_4);
        setDefaultKey("useEquipInvSlot5", KeyCodes.NUMBER_5);
        setDefaultKey("useEquipInvSlot6", KeyCodes.NUMBER_6);
        setDefaultKey("useEquipInvSlot7", KeyCodes.NUMBER_7);
        setDefaultKey("useEquipInvSlot8", KeyCodes.NUMBER_8);
        setDefaultKey("escapeToNexus", KeyCodes.INSERT);
        setDefaultKey("escapeToNexus2", KeyCodes.F5);
        setDefaultKey("autofireToggle", KeyCodes.I);
        setDefaultKey("scrollChatUp", KeyCodes.PAGE_UP);
        setDefaultKey("scrollChatDown", KeyCodes.PAGE_DOWN);
        setDefaultKey("miniMapZoomOut", KeyCodes.MINUS);
        setDefaultKey("miniMapZoomIn", KeyCodes.EQUAL);
        setDefaultKey("resetToDefaultCameraAngle", KeyCodes.R);
        setDefaultKey("togglePerformanceStats", KeyCodes.UNSET);
        setDefaultKey("options", KeyCodes.O);
        setDefaultKey("statsView", KeyCodes.SEMICOLON);
        setDefaultKey("toggleCentering", KeyCodes.UNSET);
        setDefaultKey("chat", KeyCodes.ENTER);
        setDefaultKey("chatCommand", KeyCodes.SLASH);
        setDefaultKey("tell", KeyCodes.TAB);
        setDefaultKey("guildChat", KeyCodes.G);
        setDefaultKey("partyChat", KeyCodes.P);
        setDefaultKey("useHealthPotion", KeyCodes.F);
        setDefaultKey("useMagicPotion", KeyCodes.V);
        setDefaultKey("switchTabs", KeyCodes.B);
        setDefaultKey("toggleFullscreenMode", KeyCodes.UNSET);
        setDefaultKey("crouchKey", KeyCodes.SHIFT);
        setDefault("playerObjectType", 782);
        setDefault("playSFX", true);
        setDefault("playPewPew", true);
        setDefault("centerOnPlayer", true);
        setDefault("preferredServer", null);
        setDefault("cameraAngle", 7 * Math.PI / 4);
        setDefault("defaultCameraAngle", 7 * Math.PI / 4);
        setDefault("showQuestPortraits", true);
        setDefault("allowRotation", false);
        setDefault("charIdUseMap", {});
        setDefault("drawShadows", true);
        setDefault("textBubbles", true);
        setDefault("showTradePopup",true);
        setDefault("showGuildInvitePopup", true);
        setDefault("GPURender", false);
        setDefault("particles", true);
        setDefault("hpBars", true)
        setDefault("quality", true);
        setDefault("cursor", "4");
        setDefault("mscale", 1);
        setDefault("chatScaling", 1.0);
        setDefault("hideList", 0);
        setDefault("allyShotsList", 0);
        setDefault("allyDamageList", 0);
        setDefault("allyNotifsList", 0);
        setDefault("allyParticlesList", 0);
        setDefault("allyEntitiesList", 0);
        setDefault("damageCounter", DamageCounterType.FULL);
        setDefault("statsViewOpen", false);
        setDefault("statsViewIndex", 0);
        setDefault("showConstellationsTutorial", true);
        setDefault("showTierTag", true);
        setDefault("playMusic", true);
        setDefault("musicVolume", 0.65);
        setDefault("sfxVolume", 0.75);
        setDefault("pewPewVolume", 0.75);
        setDefault("projOutline", true);
    }
}
}
