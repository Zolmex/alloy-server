package kabam.rotmg.ui.view {
import com.company.assembleegameclient.game.GameSprite;
import com.company.assembleegameclient.map.Map;
import com.company.assembleegameclient.objects.Player;
import com.company.assembleegameclient.ui.TradePanel;
import com.company.assembleegameclient.ui.panels.InteractPanel;
import com.company.assembleegameclient.ui.panels.itemgrids.EquippedGrid;
import com.company.assembleegameclient.util.TextureRedrawer;
import com.company.ui.SimpleText;
import com.company.util.BitmapUtil;
import com.company.util.GraphicsUtil;
import com.company.util.SpriteUtil;

import flash.display.Bitmap;
import flash.display.BitmapData;

import flash.display.GraphicsPath;
import flash.display.GraphicsSolidFill;
import flash.display.IGraphicsData;
import flash.display.Sprite;
import flash.events.Event;
import flash.geom.Point;
import flash.text.TextFieldAutoSize;

import io.decagames.rotmg.ui.texture.TextureParser;

import kabam.rotmg.game.view.components.TabStripView;
import kabam.rotmg.messaging.impl.incoming.TradeAccepted;
import kabam.rotmg.messaging.impl.incoming.TradeChanged;
import kabam.rotmg.messaging.impl.incoming.TradeStart;
import kabam.rotmg.minimap.view.MiniMap;

public class HUDView extends Sprite {

    private const WIDTH:int = 200;
    private const HEIGHT:int = 600;
    private const BG_POSITION:Point = new Point(0, 0);
    private const MAP_POSITION:Point = new Point(1, 27);
    private const COMBAT_ICON_POSITION:Point = new Point(1, 180);
    private const CHARACTER_DETAIL_PANEL_POSITION:Point = new Point(3, 222);
    private const STAT_METERS_POSITION:Point = new Point(3, 262);
    private const EQUIPMENT_INVENTORY_POSITION:Point = new Point(11, 302);
    private const TAB_STRIP_POSITION:Point = new Point(8, 354);
    private const INTERACT_PANEL_POSITION:Point = new Point(0, 500);

    private var worldTitle:SimpleText;
    private var worldDifficulty:Sprite;
    private var background:CharacterWindowBackground;
    private var miniMap:MiniMap;
    private var miniMapForeground:Bitmap;
    public var equippedGrid:EquippedGrid;
    public var tabStrip:TabStripView;
    private var statMeters:StatMetersView;
    public var characterDetails:CharacterDetailsView;
    public var interactPanel:InteractPanel;
    public var tradePanel:TradePanel;
    private var equippedGridBG:Bitmap;
    public var inCombatIcon:InCombatIcon;

    public function HUDView() {
        super();
        this.createAssets();
        this.addAssets();
        this.positionAssets();
    }

    private function createAssets():void {
        this.worldTitle = new SimpleText(15, 0xFFFFFF, false, 175);
        this.worldTitle.setAutoSize(TextFieldAutoSize.LEFT);
        this.worldTitle.setBold(true);
        this.worldDifficulty = new Sprite();
        this.background = new CharacterWindowBackground();
        this.miniMap = new MiniMap(190, 187);
        this.miniMapForeground = TextureParser.instance.getTexture("UI", "map_foreground");
        this.tabStrip = new TabStripView();
        this.characterDetails = new CharacterDetailsView();
        this.statMeters = new StatMetersView();
    }

    private function addAssets():void {
        addChild(this.background);
        addChild(this.worldTitle);
        addChild(this.worldDifficulty);
        addChild(this.miniMap);
        addChild(this.miniMapForeground);
        addChild(this.characterDetails);
        addChild(this.statMeters);
        addChild(this.tabStrip);
    }

    private function positionAssets():void {
        this.background.x = this.BG_POSITION.x;
        this.background.y = this.BG_POSITION.y;
        this.miniMap.x = this.MAP_POSITION.x + 4;
        this.miniMap.y = this.MAP_POSITION.y + 4;
        this.miniMapForeground.y = this.MAP_POSITION.y;
        this.miniMapForeground.y = this.MAP_POSITION.y;
        this.tabStrip.x = this.TAB_STRIP_POSITION.x;
        this.tabStrip.y = this.TAB_STRIP_POSITION.y;
        this.characterDetails.x = this.CHARACTER_DETAIL_PANEL_POSITION.x;
        this.characterDetails.y = this.CHARACTER_DETAIL_PANEL_POSITION.y;
        this.statMeters.x = (WIDTH - this.statMeters.width) / 2;
        this.statMeters.y = this.STAT_METERS_POSITION.y;
    }

    public function setPlayerDependentAssets(gs:GameSprite):void {
        var player:Player = gs.map.player_;
        this.worldTitle.setText(player.map_.name_);
        this.worldTitle.updateMetrics();
        this.worldTitle.x = (WIDTH - this.worldTitle.actualWidth_) / 2;
        this.worldTitle.y = -2;
        this.buildWorldDifficulty(gs.map);

        this.equippedGridBG = TextureParser.instance.getTexture("UI", "equipment_background");
        this.equippedGridBG.x = this.EQUIPMENT_INVENTORY_POSITION.x;
        this.equippedGridBG.y = this.EQUIPMENT_INVENTORY_POSITION.y;
        addChild(this.equippedGridBG);
        this.equippedGrid = new EquippedGrid(player, player.slotTypes_, player);
        this.equippedGrid.x = this.EQUIPMENT_INVENTORY_POSITION.x + 3;
        this.equippedGrid.y = this.EQUIPMENT_INVENTORY_POSITION.y + 3;
        addChild(this.equippedGrid);
        this.interactPanel = new InteractPanel(gs, player, 200, 100);
        this.interactPanel.x = this.INTERACT_PANEL_POSITION.x;
        this.interactPanel.y = this.INTERACT_PANEL_POSITION.y;
        addChild(this.interactPanel);
        this.inCombatIcon = new InCombatIcon(player);
        this.inCombatIcon.x = this.COMBAT_ICON_POSITION.x;
        this.inCombatIcon.y = this.COMBAT_ICON_POSITION.y;
        addChild(inCombatIcon)
    }

    private function buildWorldDifficulty(map:Map):void {
        if (map.difficulty <= 0) {
            var text:SimpleText = new SimpleText(13, 0x657673);
            text.setText("Safe zone");
            text.setBold(true);
            text.updateMetrics();
            text.y = -6;
            this.worldDifficulty.addChild(text);
        } else {
            var scale:Number = 1.4;
            for (var i:int = 0; i < map.difficulty; i++) {
                var grave:Bitmap = TextureParser.instance.getTexture("UI", "difficulty_icon");
                grave.scaleX = scale;
                grave.scaleY = scale;
                grave.x = 17 + i * (grave.width + 4);
                grave.y = (10 - grave.height) / 2;
                this.worldDifficulty.addChild(grave);

                if (i == 0 || i == map.difficulty - 1) {
                    var decorData:BitmapData = TextureParser.instance.getTextureData("UI", "difficulty_decor");
                    var decor:Bitmap = new Bitmap(i == 0 ? decorData : BitmapUtil.mirror(decorData));
                    decor.scaleX = scale;
                    decor.scaleY = scale;
                    if (i == 0) {
                        decor.x = 0;
                    } else {
                        decor.x = grave.x + grave.width + 4;
                    }
                    decor.y = (10 - decor.height) / 2;
                    this.worldDifficulty.addChild(decor);
                }
            }
        }
        this.worldDifficulty.x = (WIDTH - this.worldDifficulty.width) / 2;
        this.worldDifficulty.y = 17;
    }

    public function draw():void {
        if (this.equippedGrid)
            this.equippedGrid.draw();

        if (this.interactPanel)
            this.interactPanel.draw();
    }

    public function startTrade(gs:GameSprite, tradeStart:TradeStart):void {
        if (this.tradePanel != null) {
            return;
        }
        this.tradePanel = new TradePanel(gs, tradeStart);
        this.tradePanel.y = 200;
        this.tradePanel.addEventListener(Event.CANCEL, this.onTradeCancel);
        addChild(this.tradePanel);
        this.characterDetails.visible = false;
        this.statMeters.visible = false;
        this.tabStrip.visible = false;
        this.equippedGrid.visible = false;
        this.equippedGridBG.visible = false;
        this.interactPanel.visible = false;
    }

    public function tradeChanged(tradeChanged:TradeChanged):void {
        if (this.tradePanel == null) {
            return;
        }
        this.tradePanel.setYourOffer(tradeChanged.offer_);
    }

    public function tradeAccepted(tradeAccepted:TradeAccepted):void {
        if (this.tradePanel == null) {
            return;
        }
        this.tradePanel.youAccepted(tradeAccepted.myOffer_, tradeAccepted.yourOffer_);
    }

    private function onTradeCancel(event:Event):void {
        this.removeTradePanel();
    }

    public function tradeDone():void {
        this.removeTradePanel();
    }

    private function removeTradePanel():void {
        if (this.tradePanel != null) {
            this.tradePanel.removeEventListener(Event.CANCEL, this.onTradeCancel);
            SpriteUtil.safeRemoveChild(this, this.tradePanel);
            this.tradePanel = null;
            this.characterDetails.visible = true;
            this.statMeters.visible = true;
            this.tabStrip.visible = true;
            this.equippedGrid.visible = true;
            this.equippedGridBG.visible = true;
            this.interactPanel.visible = true;
        }
    }
}
}
