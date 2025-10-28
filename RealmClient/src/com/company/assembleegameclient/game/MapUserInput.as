package com.company.assembleegameclient.game {
import com.company.assembleegameclient.itemData.ItemData;
import com.company.assembleegameclient.objects.ObjectLibrary;
import com.company.assembleegameclient.objects.Player;
import com.company.assembleegameclient.parameters.Parameters;
import com.company.assembleegameclient.ui.options.Options;
import com.company.assembleegameclient.ui.panels.GuildHallPortalPanel;
import com.company.assembleegameclient.ui.panels.PortalPanel;
import com.company.assembleegameclient.ui.panels.itemgrids.itemtiles.InteractiveItemTile;
import com.company.assembleegameclient.ui.panels.itemgrids.itemtiles.ItemTile;
import com.company.util.KeyCodes;

import flash.display.DisplayObject;
import flash.display.Stage;
import flash.display.StageDisplayState;
import flash.events.Event;
import flash.events.KeyboardEvent;
import flash.events.MouseEvent;
import flash.utils.getTimer;

import kabam.rotmg.constants.GeneralConstants;
import kabam.rotmg.core.StaticInjectorContext;
import kabam.rotmg.core.view.Layers;
import kabam.rotmg.game.model.PotionInventoryModel;
import kabam.rotmg.game.model.UsePotionVO;
import kabam.rotmg.game.signals.AddTextLineSignal;
import kabam.rotmg.game.signals.SetTextBoxVisibilitySignal;
import kabam.rotmg.game.signals.UsePotionSignal;
import kabam.rotmg.game.view.components.statsview.left.AllStatsView;
import kabam.rotmg.messaging.impl.GameServerConnection;
import kabam.rotmg.minimap.control.MiniMapZoomSignal;
import kabam.rotmg.ui.model.TabStripModel;
import kabam.rotmg.ui.signals.StatsTabHotKeyInputSignal;

import net.hires.debug.Stats;

import org.swiftsuspenders.Injector;

public class MapUserInput {
    public static const NO_ACTION:String = "none";
    public static const MOVE_LEFT:String = "moveLeft";
    public static const MOVE_RIGHT:String = "moveRight";
    public static const MOVE_UP:String = "moveUp";
    public static const MOVE_DOWN:String = "moveDown";
    public static const ROTATE_LEFT:String = "rotateLeft";
    public static const ROTATE_RIGHT:String = "rotateRight";
    public static const RESET_CAMERA_ANGLE:String = "resetCameraAngle";
    public static const USE_SPECIAL:String = "useSpecial";
    public static const AUTOFIRE:String = "autofireToggle";
    public static const USE_EQUIP_INV_SLOT_1:String = "useEquipInvSlot1";
    public static const USE_EQUIP_INV_SLOT_2:String = "useEquipInvSlot2";
    public static const USE_EQUIP_INV_SLOT_3:String = "useEquipInvSlot3";
    public static const USE_EQUIP_INV_SLOT_4:String = "useEquipInvSlot4";
    public static const USE_EQUIP_INV_SLOT_5:String = "useEquipInvSlot5";
    public static const USE_EQUIP_INV_SLOT_6:String = "useEquipInvSlot6";
    public static const USE_EQUIP_INV_SLOT_7:String = "useEquipInvSlot7";
    public static const USE_EQUIP_INV_SLOT_8:String = "useEquipInvSlot8";
    public static const USE_HEALTH_POTION:String = "useHealthPotion";
    public static const USE_MAGIC_POTION:String = "useMagicPotion";
    public static const MINIMAP_ZOOM_OUT:String = "minimapZoomOut";
    public static const MINIMAP_ZOOM_IN:String = "minimapZoomIn";
    public static const TOGGLE_PERFORMANCE_STATS:String = "togglePerformanceStats";
    public static const ESCAPE_TO_NEXUS:String = "escapeToNexus";
    public static const OPTIONS:String = "options";
    public static const STATS_VIEW:String = "statsView";
    public static const TOGGLE_CENTERING:String = "toggleCentering";
    public static const SWITCH_TABS:String = "switchTabs";
    public static const TOGGLE_FULLSCREEN_MODE:String = "toggleFullscreenMode";
    public static const CROUCH:String = "crouch";
    public static const SHIFT_KEY:String = "shiftKey";
    public static const SCROLL_CHAT_UP:String = "scrollChatUp";
    public static const SCROLL_CHAT_DOWN:String = "scrollChatDown";
    public static const CHAT:String = "chat";
    public static const CHAT_COMMAND:String = "chatCommand";
    public static const TELL:String = "tell";
    public static const GUILD_CHAT:String = "guildChat";
    public static const PARTY_CHAT:String = "partyChat";
    public static const INTERACT:String = "interact";
    private var actionHandlers:Object = {
        interact: {
            down: function ():void {
                if (currentPortalPanel != null) currentPortalPanel.onInteract();
                if (currentGuildPortalPanel != null) currentGuildPortalPanel.onInteract();
            },
            up: function ():void {
                endKey(INTERACT)
            }
        },
        guildChat: {
            endEvent: true,
            tabbedCondition: function ():Boolean {
                return gs_.textBox_ != null && !gs_.textBox_.checkInputTabbed();
            },
            down: function ():void {
                gs_.textBox_ != null && gs_.textBox_.guildChat();
            }
        },
        partyChat: {
            endEvent: true,
            tabbedCondition: function ():Boolean {
                return gs_.textBox_ != null && !gs_.textBox_.checkInputTabbed();
            },
            down: function ():void {
                gs_.textBox_ != null && gs_.textBox_.partyChat();
            }
        },
        tell: {
            endEvent: true,
            tabbedCondition: function ():Boolean {
                return gs_.textBox_ != null && !gs_.textBox_.checkInputTabbed();
            },
            down: function ():void {
                gs_.textBox_ != null && gs_.textBox_.tell();
            }
        },
        chatCommand: {
            endEvent: true,
            tabbedCondition: function ():Boolean {
                return gs_.textBox_ != null && !gs_.textBox_.checkInputTabbed();
            },
            down: function ():void {
                gs_.textBox_ != null && gs_.textBox_.chatCommand();
            }
        },
        chat: {
            endEvent: true,
            tabbedCondition: function ():Boolean {
                return gs_.textBox_ != null
            },
            down: function ():void {
                if (gs_.textBox_ != null) {
                    if (gs_.textBox_.checkInputTabbed()) {
                        gs_.textBox_.onEnterUp();
                    } else {
                        gs_.textBox_.chat();
                    }
                }
            }
        },
        scrollChatDown: {
            tabbedCondition: function ():Boolean {
                return gs_.textBox_ != null
            },
            down: function ():void {
                gs_.textBox_ != null && gs_.textBox_.scrollChat(false);
            }
        },
        scrollChatUp: {
            tabbedCondition: function ():Boolean {
                return gs_.textBox_ != null
            },
            down: function ():void {
                gs_.textBox_ != null && gs_.textBox_.scrollChat(true);
            }
        },
        shiftKey: {
            down: function ():void {
                toggleShiftKey(true)
            },
            up: function ():void {
                toggleShiftKey(false)
            }
        },
        moveLeft: {
            down: function ():void {
                toggleMove(MOVE_LEFT, true);
            },
            up: function ():void {
                toggleMove(MOVE_LEFT, false);
            }
        },
        moveRight: {
            down: function ():void {
                toggleMove(MOVE_RIGHT, true);
            },
            up: function ():void {
                toggleMove(MOVE_RIGHT, false);
            }
        },
        moveUp: {
            down: function ():void {
                toggleMove(MOVE_UP, true);
            },
            up: function ():void {
                toggleMove(MOVE_UP, false);
            }
        },
        moveDown: {
            down: function ():void {
                toggleMove(MOVE_DOWN, true);
            },
            up: function ():void {
                toggleMove(MOVE_DOWN, false);
            }
        },
        rotateLeft: {
            down: function ():void {
                toggleRotate(ROTATE_LEFT, true)
            },
            up: function ():void {
                toggleRotate(ROTATE_LEFT, false)
            }
        },
        rotateRight: {
            down: function ():void {
                toggleRotate(ROTATE_RIGHT, true)
            },
            up: function ():void {
                toggleRotate(ROTATE_RIGHT, false)
            }
        },
        resetCameraAngle: {
            down: function ():void {
                resetCameraAngle()
            },
            up: function ():void {
                endKey(RESET_CAMERA_ANGLE)
            }
        },
        useSpecial: {
            down: function ():void {
                if (gs_.map.player_ != null && !canSpecial)
                    canSpecial = gs_.map.player_.useAltWeapon(gs_.map.mouseX, gs_.map.mouseY, false);

                isKeyDown[USE_SPECIAL] = true;
            },
            up: function ():void {
                if (gs_.map.player_ != null && canSpecial)
                    gs_.map.player_.useAltWeapon(gs_.map.mouseX, gs_.map.mouseY, true);

                endKey(USE_SPECIAL)
            }
        },
        autofireToggle: {
            down: function ():void {
                autofire = !autofire;
                isKeyDown[AUTOFIRE] = true;
            },
            up: function ():void {
                endKey(AUTOFIRE)
            }
        },
        useEquipInvSlot1: {
            down: function ():void {
                useEquipItem(4, USE_EQUIP_INV_SLOT_1)
            },
            up: function ():void {
                endKey(USE_EQUIP_INV_SLOT_1)
            }
        },
        useEquipInvSlot2: {
            down: function ():void {
                useEquipItem(5, USE_EQUIP_INV_SLOT_2)
            },
            up: function ():void {
                endKey(USE_EQUIP_INV_SLOT_2)
            }
        },
        useEquipInvSlot3: {
            down: function ():void {
                useEquipItem(6, USE_EQUIP_INV_SLOT_3)
            },
            up: function ():void {
                endKey(USE_EQUIP_INV_SLOT_3)
            }
        },
        useEquipInvSlot4: {
            down: function ():void {
                useEquipItem(7, USE_EQUIP_INV_SLOT_4)
            },
            up: function ():void {
                endKey(USE_EQUIP_INV_SLOT_4)
            }
        },
        useEquipInvSlot5: {
            down: function ():void {
                useEquipItem(8, USE_EQUIP_INV_SLOT_5)
            },
            up: function ():void {
                endKey(USE_EQUIP_INV_SLOT_5)
            }
        },
        useEquipInvSlot6: {
            down: function ():void {
                useEquipItem(9, USE_EQUIP_INV_SLOT_6)
            },
            up: function ():void {
                endKey(USE_EQUIP_INV_SLOT_6)
            }
        },
        useEquipInvSlot7: {
            down: function ():void {
                useEquipItem(10, USE_EQUIP_INV_SLOT_7)
            },
            up: function ():void {
                endKey(USE_EQUIP_INV_SLOT_7)
            }
        },
        useEquipInvSlot8: {
            down: function ():void {
                useEquipItem(11, USE_EQUIP_INV_SLOT_8)
            },
            up: function ():void {
                endKey(USE_EQUIP_INV_SLOT_8)
            }
        },
        useHealthPotion: { //doesnt exist rn anyways so will leave like this until flasks are added
        },
        useMagicPotion: {},
        minimapZoomOut: {
            down: function ():void {
                miniMapZoom.dispatch(MiniMapZoomSignal.OUT)
            },
            up: null
        },
        minimapZoomIn: {
            down: function ():void {
                miniMapZoom.dispatch(MiniMapZoomSignal.IN)
            },
            up: null
        },
        togglePerformanceStats: {
            down: function ():void {
                togglePerformanceStats()
            },
            up: function ():void {
                endKey(TOGGLE_PERFORMANCE_STATS)
            }
        },
        escapeToNexus: {
            down: function ():void {
                escapeToNexus()
            },
            up: function ():void {
                endKey(ESCAPE_TO_NEXUS)
            }
        },
        options: {
            down: function ():void {
                toggleOptions()
            },
            up: function ():void {
                endKey(OPTIONS)
            }
        },
        statsView: {
            down: function ():void {
                toggleStatsView()
            },
            up: function ():void {
                endKey(STATS_VIEW)
            }
        },
        toggleCentering: {
            down: function ():void {
                toggleCentering()
            },
            up: function ():void {
                endKey(TOGGLE_CENTERING)
            }
        },
        switchTabs: {
            down: function ():void {
                switchTabs()
            },
            up: function ():void {
                endKey(SWITCH_TABS)
            }
        },
        toggleFullscreenMode: {
            down: function ():void {
                toggleFullscreenMode()
            },
            up: function ():void {
                endKey(TOGGLE_FULLSCREEN_MODE)
            }
        },
        crouch: {
            down: function ():void {
                toggleCrouch(true)
            },
            up: function ():void {
                toggleCrouch(false)
            }
        },
        none: {}
    };

    public static var instance:MapUserInput;
    private static var stats_:Stats = new Stats();
    public var activeKeys:Object = {};
    private var keyActionMap:Object = {};
    public var isKeyDown:Object = {};
    public var inputWhitelist:Vector.<String> = new Vector.<String>();

    public var gs_:GameSprite;

    public var currentPortalPanel:PortalPanel;
    public var currentGuildPortalPanel:GuildHallPortalPanel;
    private var canSpecial:Boolean = false;
    private var autofire:Boolean = false;
    private var mouseDown_:Boolean = false;
    private var enablePlayerInput_:Boolean = true;

    private var addTextLine:AddTextLineSignal;
    private var setTextBoxVisibility:SetTextBoxVisibilitySignal;
    private var statsTabHotKeyInputSignal:StatsTabHotKeyInputSignal;
    private var miniMapZoom:MiniMapZoomSignal;
    private var usePotionSignal:UsePotionSignal;

    private var potionInventoryModel:PotionInventoryModel;
    private var tabStripModel:TabStripModel;

    public var layers:Layers;

    public function MapUserInput(gs:GameSprite) {
        super();
        instance = this;
        this.gs_ = gs;
        this.gs_.addEventListener(Event.ADDED_TO_STAGE, this.onAddedToStage);
        this.gs_.addEventListener(Event.REMOVED_FROM_STAGE, this.onRemovedFromStage);
        var injector:Injector = StaticInjectorContext.getInjector();

        this.addTextLine = injector.getInstance(AddTextLineSignal);
        this.setTextBoxVisibility = injector.getInstance(SetTextBoxVisibilitySignal);
        this.statsTabHotKeyInputSignal = injector.getInstance(StatsTabHotKeyInputSignal);
        this.miniMapZoom = injector.getInstance(MiniMapZoomSignal);
        this.usePotionSignal = injector.getInstance(UsePotionSignal);

        this.potionInventoryModel = injector.getInstance(PotionInventoryModel);
        this.tabStripModel = injector.getInstance(TabStripModel);

        this.layers = injector.getInstance(Layers);
        this.gs_.map.signalRenderSwitch.add(this.onRenderSwitch);

        updateKeyActionMappings();
    }

    public function updateKeyActionMappings():void {
        this.keyActionMap = {};
        addKeyMapping(Parameters.data_.moveLeft, MOVE_LEFT);
        addKeyMapping(Parameters.data_.moveRight, MOVE_RIGHT);
        addKeyMapping(Parameters.data_.moveUp, MOVE_UP);
        addKeyMapping(Parameters.data_.moveDown, MOVE_DOWN);
        addKeyMapping(Parameters.data_.rotateLeft, ROTATE_LEFT);
        addKeyMapping(Parameters.data_.rotateRight, ROTATE_RIGHT);
        addKeyMapping(Parameters.data_.useSpecial, USE_SPECIAL);
        addKeyMapping(Parameters.data_.interact, INTERACT);
        addKeyMapping(Parameters.data_.useEquipInvSlot1, USE_EQUIP_INV_SLOT_1);
        addKeyMapping(Parameters.data_.useEquipInvSlot2, USE_EQUIP_INV_SLOT_2);
        addKeyMapping(Parameters.data_.useEquipInvSlot3, USE_EQUIP_INV_SLOT_3);
        addKeyMapping(Parameters.data_.useEquipInvSlot4, USE_EQUIP_INV_SLOT_4);
        addKeyMapping(Parameters.data_.useEquipInvSlot5, USE_EQUIP_INV_SLOT_5);
        addKeyMapping(Parameters.data_.useEquipInvSlot6, USE_EQUIP_INV_SLOT_6);
        addKeyMapping(Parameters.data_.useEquipInvSlot7, USE_EQUIP_INV_SLOT_7);
        addKeyMapping(Parameters.data_.useEquipInvSlot8, USE_EQUIP_INV_SLOT_8);
        addKeyMapping(Parameters.data_.escapeToNexus, ESCAPE_TO_NEXUS);
        addKeyMapping(Parameters.data_.escapeToNexus2, ESCAPE_TO_NEXUS);
        addKeyMapping(Parameters.data_.autofireToggle, AUTOFIRE);
        addKeyMapping(Parameters.data_.scrollChatUp, SCROLL_CHAT_UP);
        addKeyMapping(Parameters.data_.scrollChatDown, SCROLL_CHAT_DOWN);
        addKeyMapping(Parameters.data_.miniMapZoomOut, MINIMAP_ZOOM_OUT);
        addKeyMapping(Parameters.data_.miniMapZoomIn, MINIMAP_ZOOM_IN);
        addKeyMapping(Parameters.data_.resetToDefaultCameraAngle, RESET_CAMERA_ANGLE);
        addKeyMapping(Parameters.data_.togglePerformanceStats, TOGGLE_PERFORMANCE_STATS);
        addKeyMapping(Parameters.data_.options, OPTIONS);
        addKeyMapping(Parameters.data_.statsView, STATS_VIEW);
        addKeyMapping(Parameters.data_.toggleCentering, TOGGLE_CENTERING);
        addKeyMapping(Parameters.data_.chat, CHAT);
        addKeyMapping(Parameters.data_.chatCommand, CHAT_COMMAND);
        addKeyMapping(Parameters.data_.tell, TELL);
        addKeyMapping(Parameters.data_.guildChat, GUILD_CHAT);
        addKeyMapping(Parameters.data_.partyChat, PARTY_CHAT);
        addKeyMapping(Parameters.data_.useHealthPotion, USE_HEALTH_POTION);
        addKeyMapping(Parameters.data_.useMagicPotion, USE_MAGIC_POTION);
        addKeyMapping(Parameters.data_.switchTabs, SWITCH_TABS);
        addKeyMapping(Parameters.data_.toggleFullscreenMode, TOGGLE_FULLSCREEN_MODE);
        addKeyMapping(Parameters.data_.crouchKey, CROUCH);
        addKeyMapping(KeyCodes.SHIFT, SHIFT_KEY);
    }

    public function addKeyMapping(keyCode:uint, actionId:String):void {
        if (this.keyActionMap[keyCode] == undefined) {
            this.keyActionMap[keyCode] = new Vector.<String>();
        }
        var actions:Vector.<String> = this.keyActionMap[keyCode];

        if (actions.indexOf(actionId) == -1)
            actions.push(actionId);
    }


    private function onMouseDown(event:MouseEvent):void {
        var player:Player = this.gs_.map.player_;
        if (player == null)
            return;
        if (!this.enablePlayerInput_)
            return;
        if (event.shiftKey) {
            return;
        }
        var angle:Number = NaN;

        if (Parameters.GPURenderFrame) {
            if (event.currentTarget == event.target || event.target == this.gs_.map || event.target == this.gs_)
                angle = Math.atan2(this.gs_.map.mouseY, this.gs_.map.mouseX);
            else
                return;
        } else
            angle = Math.atan2(this.gs_.map.mouseY, this.gs_.map.mouseX);

        player.attemptAttackAngle(angle);
        this.mouseDown_ = true;
    }

    private function onMouseWheel(event:MouseEvent):void {
        var stage:DisplayObject = WebMain.STAGE;
        if (event.ctrlKey) {
            if (event.delta > 0) {
                Parameters.data_.mscale = Math.min(Parameters.data_.mscale + 0.1, 5);
                Parameters.save(this);
                stage.dispatchEvent(new Event(Event.RESIZE));
            } else {
                Parameters.data_.mscale = Math.max(Parameters.data_.mscale - 0.1, 0.5);
                Parameters.save(this);
                stage.dispatchEvent(new Event(Event.RESIZE));
            }
            return;
        }
        if (event.delta > 0)
            this.miniMapZoom.dispatch(MiniMapZoomSignal.IN);
        else
            this.miniMapZoom.dispatch(MiniMapZoomSignal.OUT);
    }

    private function onEnterFrame(event:Event):void {
        if (this.enablePlayerInput_ && (this.mouseDown_ || this.autofire)) {
            var angle:Number = Math.atan2(this.gs_.map.mouseY, this.gs_.map.mouseX);
            var player:Player = this.gs_.map.player_;
            if (player != null) {
                player.attemptAttackAngle(angle);
            }
        }
    }

    private function onKeyDown(event:KeyboardEvent):void {
        if (inputWhitelist.length > 0 && !checkInputWhitelist(event.keyCode))
            return;

        var endEvent:Boolean = processInput(event.keyCode, true);
        if (endEvent) {
            event.preventDefault();
            event.stopPropagation();
        }
    }

    private function onKeyUp(event:KeyboardEvent):void {
        if (inputWhitelist.length > 0 && !checkInputWhitelist(event.keyCode))
            return;

        var endEvent:Boolean = processInput(event.keyCode, false);
        if (endEvent) {
            event.preventDefault();
            event.stopPropagation();
        }
    }

    private function checkInputWhitelist(keyCode:uint):Boolean {
        if (keyCode in keyActionMap) {
            for each (var id:String in keyActionMap[keyCode])
                if (this.inputWhitelist.indexOf(id) != -1)
                    return true;
        }
        return false;
    }

    private function processInput(keyCode:uint, isKeyDown:Boolean):Boolean {
        var endEvent:Boolean = false;
        var skipped:Boolean = false;
        var actions:Vector.<String> = this.keyActionMap[keyCode];
        for each (var actionId:String in actions)
            if (actionId) {
                if (!checkKeyChange(actionId, isKeyDown)) {
                    skipped = true;
                    continue;
                }
                var action:Object = this.actionHandlers[actionId];
                if (action) {
                    if (action.endEvent) endEvent = true;

                    if (!(action.tabbedCondition != null ? action.tabbedCondition() : (!isKeyDown ? true : this.gs_.stage.focus == null))) {
                        //trace("Returned tabbedCondition " + actionId + " on " + (isKeyDown ? "Active" : "Inactive"));
                        skipped = true;
                        continue;
                    }
                    if (action.condition != null && !action.condition()) {
                        skipped = true;
                        continue;
                    }
                    if (isKeyDown) {
                        if (action.down != null) {
                            action.down();
                            trace("Input Processed: " + actionId + " : Active");
                        }
                    } else {
                        if (action.up != null) {
                            action.up();
                            trace("Input Processed: " + actionId + " : Inactive");
                        }
                    }
                    if (isMovementKey(keyCode))
                        setPlayerMovement();

                    this.activeKeys[keyCode] = isKeyDown;
                } else
                    trace("Null Action: " + actionId);
            }
        return skipped ? false : endEvent;
    }

    private function checkKeyChange(id:String, after:Boolean):Boolean {
        var keyExists:Boolean = id in this.isKeyDown;
        return keyExists ? this.isKeyDown[id] != after : true;
        //we are checking if pressing this key will actually change anything, if it should change something.
        //for example, pressing W sets moveUp to true, letting go sets it to false
        //we dont want it to repeatedly fire the function that sets it to true if we are holding it,
        //so we check if there is actually a change first.
    }

    private static function isMovementKey(keyCode:uint):Boolean {
        return keyCode == Parameters.data_.moveLeft
                || keyCode == Parameters.data_.moveRight
                || keyCode == Parameters.data_.moveUp
                || keyCode == Parameters.data_.moveDown
                || keyCode == Parameters.data_.rotateLeft
                || keyCode == Parameters.data_.rotateRight;
    }

    private function togglePerformanceStats():void {
        this.isKeyDown[TOGGLE_PERFORMANCE_STATS] = true;
        if (this.gs_.contains(stats_)) {
            this.gs_.removeChild(stats_);
            this.gs_.removeChild(this.gs_.gsc_.jitterWatcher_);
            this.gs_.gsc_.disableJitterWatcher();
        } else {
            this.gs_.addChild(stats_);
            this.gs_.gsc_.enableJitterWatcher();
            this.gs_.gsc_.jitterWatcher_.y = stats_.height;
            this.gs_.addChild(this.gs_.gsc_.jitterWatcher_);
        }
    }

    public function endKey(id:String):void {
        this.isKeyDown[id] = false;
        if (id != OPTIONS)
            this.inputWhitelist = new Vector.<String>();

        if (id == USE_SPECIAL) this.canSpecial = false;
    }

    private function useEquipItem(slotId:int, id:String):void {
        this.isKeyDown[id] = true;
        if (this.tabStripModel.currentSelection == TabStripModel.BACKPACK) {
            slotId = slotId + GeneralConstants.NUM_INVENTORY_SLOTS;
        }

        var slotIndex:int = ObjectLibrary.getMatchingSlotIndex(this.gs_.map.player_.equipment_[slotId], this.gs_.map.player_);
        var staticTile:ItemTile = this.gs_.hudView.tabStrip.inventoryGrid.staticToolTipTile || this.gs_.hudView.equippedGrid.staticToolTipTile;
        if (staticTile != null && (staticTile.tileId == slotId || staticTile.tileId == slotIndex)) {
            return;
        }

        if (slotIndex != -1) {
            GameServerConnection.instance.invSwap(
                    this.gs_.map.player_,
                    this.gs_.map.player_, slotId,
                    this.gs_.map.player_, slotIndex);
        } else {
            GameServerConnection.instance.useItem_new(this.gs_.map.player_, slotId, getTimer());
        }
    }

    private function setPlayerMovement():void {
        var player:Player = this.gs_.map.player_;
        if (player != null) {
            if (this.enablePlayerInput_) {
                player.setRelativeMovement(
                        (isKeyDown[ROTATE_RIGHT] ? 1 : 0) - (isKeyDown[ROTATE_LEFT] ? 1 : 0),
                        (isKeyDown[MOVE_RIGHT] ? 1 : 0) - (isKeyDown[MOVE_LEFT] ? 1 : 0),
                        (isKeyDown[MOVE_DOWN] ? 1 : 0) - (isKeyDown[MOVE_UP] ? 1 : 0));
            } else {
                player.setRelativeMovement(0, 0, 0);
            }
        }
    }

    public function toggleStatsView(isKeyDown:Boolean = true):void {
        this.isKeyDown[STATS_VIEW] = isKeyDown;
        if (this.gs_.allStatsView != null) {
            this.gs_.allStatsView.stage.focus = null;
            this.gs_.allStatsView.parent.removeChild(this.gs_.allStatsView);
            this.gs_.textBox_.statsOpen = false;
            return;
        }
        this.layers.overlay.addChild(new AllStatsView(this.gs_));
    }

    public function switchTabs():void {
        this.isKeyDown[SWITCH_TABS] = true;
        this.statsTabHotKeyInputSignal.dispatch();
    }

    public function toggleFullscreenMode():void {
        this.isKeyDown[TOGGLE_FULLSCREEN_MODE] = true;

        var newState:String = this.gs_.stage.displayState == StageDisplayState.NORMAL ? StageDisplayState.FULL_SCREEN_INTERACTIVE : StageDisplayState.NORMAL;
        this.gs_.stage.displayState = newState;
    }

    public function toggleCrouch(active:Boolean):void {
        if (this.gs_.map.player_ == null)
            return;

        if (this.enablePlayerInput_)
            this.isKeyDown[CROUCH] = active;
    }

    public function toggleShiftKey(active:Boolean):void {
        if (this.gs_.statsView == null)
            return;

        this.isKeyDown[SHIFT_KEY] = active;
        this.gs_.statsView.reloadIncButtons();
    }

    public function toggleCentering():void {
        this.isKeyDown[TOGGLE_CENTERING] = true;

        Parameters.data_.centerOnPlayer = !Parameters.data_.centerOnPlayer;
        Parameters.save(this);
    }

    public function toggleOptions():void {
        this.clearInput();
        this.isKeyDown[OPTIONS] = true;
        if (this.gs_.optionsView) {
            this.gs_.optionsView.close();
            this.inputWhitelist = new Vector.<String>();
            return;
        }
        this.inputWhitelist.push(OPTIONS);
        this.layers.overlay.addChild(new Options(this.gs_));
    }

    public function resetCameraAngle():void {
        Parameters.data_.cameraAngle = Parameters.data_.defaultCameraAngle;
        Parameters.save(this);
        isKeyDown[RESET_CAMERA_ANGLE] = true;
    }

    public function escapeToNexus():void {
        this.gs_.gsc_.escape();
        this.isKeyDown[ESCAPE_TO_NEXUS] = true;
    }

    public function toggleRotate(dir:String, active:Boolean):void {
        if (!Parameters.data_.allowRotation)
            return;
        this.isKeyDown[dir] = active
    }

    public function toggleMove(dir:String, active:Boolean):void {
        this.isKeyDown[dir] = active
    }

    public function clearInput():void {
        this.isKeyDown[MOVE_LEFT] = false;
        this.isKeyDown[MOVE_RIGHT] = false;
        this.isKeyDown[MOVE_UP] = false;
        this.isKeyDown[MOVE_DOWN] = false;
        this.isKeyDown[ROTATE_LEFT] = false;
        this.isKeyDown[ROTATE_RIGHT] = false;
        this.isKeyDown[CROUCH] = false;
        this.autofire = false;
        this.mouseDown_ = false;
        this.setPlayerMovement();
    }

    public function onRenderSwitch(wasLastGpu:Boolean):void {
        if (wasLastGpu) {
            this.gs_.stage.removeEventListener(MouseEvent.MOUSE_DOWN, this.onMouseDown);
            this.gs_.stage.removeEventListener(MouseEvent.MOUSE_UP, this.onMouseUp);
            this.gs_.map.addEventListener(MouseEvent.MOUSE_DOWN, this.onMouseDown);
            this.gs_.map.addEventListener(MouseEvent.MOUSE_UP, this.onMouseUp);
        } else {
            this.gs_.map.removeEventListener(MouseEvent.MOUSE_DOWN, this.onMouseDown);
            this.gs_.map.removeEventListener(MouseEvent.MOUSE_UP, this.onMouseUp);
            this.gs_.stage.addEventListener(MouseEvent.MOUSE_DOWN, this.onMouseDown);
            this.gs_.stage.addEventListener(MouseEvent.MOUSE_UP, this.onMouseUp);
        }
    }

    public function setEnablePlayerInput(enable:Boolean):void {
        if (this.enablePlayerInput_ != enable) {
            this.enablePlayerInput_ = enable;
            this.clearInput();
        }
    }

    private function onAddedToStage(event:Event):void {
        var stage:Stage = this.gs_.stage;
        stage.addEventListener(Event.ACTIVATE, this.onActivate);
        stage.addEventListener(Event.DEACTIVATE, this.onDeactivate);
        stage.addEventListener(KeyboardEvent.KEY_DOWN, this.onKeyDown);
        stage.addEventListener(KeyboardEvent.KEY_UP, this.onKeyUp);
        stage.addEventListener(MouseEvent.MOUSE_WHEEL, this.onMouseWheel);
        if (Parameters.GPURenderFrame) {
            stage.addEventListener(MouseEvent.MOUSE_DOWN, this.onMouseDown);
            stage.addEventListener(MouseEvent.MOUSE_UP, this.onMouseUp);
        } else {
            this.gs_.map.addEventListener(MouseEvent.MOUSE_DOWN, this.onMouseDown);
            this.gs_.map.addEventListener(MouseEvent.MOUSE_UP, this.onMouseUp);
        }
        stage.addEventListener(Event.ENTER_FRAME, this.onEnterFrame);
    }

    private function onRemovedFromStage(event:Event):void {
        var stage:Stage = this.gs_.stage;
        stage.removeEventListener(Event.ACTIVATE, this.onActivate);
        stage.removeEventListener(Event.DEACTIVATE, this.onDeactivate);
        stage.removeEventListener(KeyboardEvent.KEY_DOWN, this.onKeyDown);
        stage.removeEventListener(KeyboardEvent.KEY_UP, this.onKeyUp);
        stage.removeEventListener(MouseEvent.MOUSE_WHEEL, this.onMouseWheel);
        if (Parameters.GPURenderFrame) {
            stage.removeEventListener(MouseEvent.MOUSE_DOWN, this.onMouseDown);
            stage.removeEventListener(MouseEvent.MOUSE_UP, this.onMouseUp);
        } else {
            this.gs_.map.removeEventListener(MouseEvent.MOUSE_DOWN, this.onMouseDown);
            this.gs_.map.removeEventListener(MouseEvent.MOUSE_UP, this.onMouseUp);
        }
        stage.removeEventListener(Event.ENTER_FRAME, this.onEnterFrame);

        while (this.layers.overlay.numChildren > 0) this.layers.overlay.removeChildAt(0);
    }

    private function onActivate(event:Event):void {
    }

    private function onDeactivate(event:Event):void {
        this.clearInput();
    }

    private function onMouseUp(event:MouseEvent):void {
        this.mouseDown_ = false;
    }
}
}
