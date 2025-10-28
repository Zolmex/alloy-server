package kabam.rotmg.ui.view {
import com.company.assembleegameclient.objects.Player;
import com.company.assembleegameclient.ui.IconButton;
import com.company.assembleegameclient.util.MaskedImage;
import com.company.assembleegameclient.util.TextureRedrawer;
import com.company.ui.SimpleText;
import com.company.util.AssetLibrary;

import flash.display.Bitmap;
import flash.display.BitmapData;
import flash.display.Sprite;
import flash.events.MouseEvent;
import flash.filters.DropShadowFilter;
import flash.text.TextFieldAutoSize;

import io.decagames.rotmg.ui.texture.TextureParser;

import org.osflash.signals.Signal;
import org.osflash.signals.natives.NativeSignal;

public class CharacterDetailsView extends Sprite {

    public static const NEXUS_BUTTON:String = "NEXUS_BUTTON";
    public static const OPTIONS_BUTTON:String = "OPTIONS_BUTTON";
    private static const WIDTH:int = 200;
    private static const HEIGHT:int = 30;

    private var portrait_:Bitmap;
    private var button:IconButton;
    private var nameText_:SimpleText;
    private var nexusClicked:NativeSignal;
    private var optionsClicked:NativeSignal;
    public var gotoNexus:Signal;
    public var gotoOptions:Signal;
    private var xpBar:Bitmap;
    private var xpText:SimpleText;

    public function CharacterDetailsView() {
        this.portrait_ = new Bitmap(null);
        this.nameText_ = new SimpleText(20, 0xFFFFFF, false, 0, 0);
        this.nexusClicked = new NativeSignal(this.button, MouseEvent.CLICK);
        this.optionsClicked = new NativeSignal(this.button, MouseEvent.CLICK);
        this.gotoNexus = new Signal();
        this.gotoOptions = new Signal();
        super();
    }

    public function init(playerName:String, buttonType:String):void {
        this.xpBar = TextureParser.instance.getTexture("UI", "xp_background"); // Stretch this the closer we are to xp goal
        addChild(this.xpBar);

        this.createPortrait();
        this.createNameText(playerName);
        this.createButton(buttonType);
        this.createXPStuff();
    }

    private function createXPStuff():void {

        this.xpText = new SimpleText(12, 0xf2f288, false, 130);
        this.xpText.setAutoSize(TextFieldAutoSize.LEFT);
        this.xpText.setBold(true);
        this.xpText.setText("Lv. 0 - 0/0");
        this.xpText.filters = [new DropShadowFilter(1, 0, 0)];
        this.xpText.updateMetrics();
        this.xpText.x = this.nameText_.x;
        this.xpText.y = this.nameText_.y + 18 + 3;
        addChild(this.xpText);
    }

    private function createButton(buttonType:String):void {
        if (buttonType == NEXUS_BUTTON) {
            this.button = new IconButton(AssetLibrary.getImageFromSet("lofiInterfaceBig", 6), "Nexus", "escapeToNexus");
            this.nexusClicked = new NativeSignal(this.button, MouseEvent.CLICK, MouseEvent);
            this.nexusClicked.add(this.onNexusClick);
        } else if (buttonType == OPTIONS_BUTTON) {
            this.button = new IconButton(AssetLibrary.getImageFromSet("lofiInterfaceBig", 5), "Options", "options");
            this.optionsClicked = new NativeSignal(this.button, MouseEvent.CLICK, MouseEvent);
            this.optionsClicked.add(this.onOptionsClick);
        }
        this.button.x = WIDTH - (this.button.width / 2) - 10;
        this.button.y = (HEIGHT - (this.button.height / 2)) / 2 + 2;
        addChild(this.button);
    }

    private function createPortrait():void {
        this.portrait_.x = -7;
        this.portrait_.y = -9;
        addChild(this.portrait_);
    }

    private function createNameText(name:String):void {
        this.nameText_.setBold(true);
        this.nameText_.filters = [new DropShadowFilter(0, 0, 0)];
        this.nameText_.text = name;
        this.nameText_.updateMetrics();
        this.nameText_.x = 37;
        this.nameText_.y = this.portrait_.y + 2;
        addChild(this.nameText_);
    }

    public function update(player:Player):void {
        var skin:MaskedImage = player.getPortraitImage();
        this.portrait_.bitmapData = TextureRedrawer.redraw(skin.image_, 100 * (8 / skin.image_.width), skin.mask_, 0, true, 3);
    }

    public function draw(player:Player):void {
    }

    private function onNexusClick(event:MouseEvent):void {
        this.gotoNexus.dispatch();
    }

    private function onOptionsClick(event:MouseEvent):void {
        this.gotoOptions.dispatch();
    }

    public function setName(name:String):void {
        this.nameText_.text = name;
    }

    public function updateXP(level:int, xp:int, xpGoal:int):void {
        var perc:Number = xp / xpGoal;
        this.xpBar.scaleX = Math.min(1, perc);
        this.xpText.setText("Lv. " + level + " - " + xp + "/" + xpGoal);
    }
}
}
