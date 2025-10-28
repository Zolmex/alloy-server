package kabam.rotmg.ui.view {
import com.company.assembleegameclient.parameters.Parameters;
import com.company.assembleegameclient.screens.AccountScreen;
import com.company.assembleegameclient.screens.TitleMenuOption;
import com.company.assembleegameclient.sound.MusicPlayer;
import com.company.rotmg.graphics.ScreenGraphic;
import com.company.ui.SimpleText;

import flash.display.Sprite;
import flash.filters.DropShadowFilter;

import kabam.rotmg.ui.view.components.ScreenBase;

import org.osflash.signals.Signal;

public class TitleView extends Sprite {

    public static const TITLE_SONG:String = "sorc";
    private static const COPYRIGHT:String = "© 2010, 2011 by Wild Shadow Studios, Inc.";

    public var playClicked:Signal;
    public var serversClicked:Signal;
    public var accountClicked:Signal;
    public var legendsClicked:Signal;
    public var editorClicked:Signal;
    public var minigamesClicked:Signal;
    public var optionsClicked:Signal;

    private var container:Sprite;

    private var playButton:TitleMenuOption;
    private var serversButton:TitleMenuOption;
    private var accountButton:TitleMenuOption;
    private var legendsButton:TitleMenuOption;
    private var editorButton:TitleMenuOption;
    private var minigamesButton:TitleMenuOption;
    private var optionsButton:TitleMenuOption;

    private var versionText:SimpleText;
    private var copyrightText:SimpleText;

    public function TitleView() {
        super();
        addChild(new ScreenBase());
        addChild(new AccountScreen());
        addChild(new ScreenGraphic());
        this.makeChildren();
        MusicPlayer.playSong(TITLE_SONG);
    }

    private function makeChildren():void {
        this.container = new Sprite();
        this.playButton = new TitleMenuOption("play", 36, true);
        this.playClicked = this.playButton.clicked;
        this.container.addChild(this.playButton);
        this.serversButton = new TitleMenuOption("servers", 22, false);
        this.serversClicked = this.serversButton.clicked;
        this.container.addChild(this.serversButton);
        this.accountButton = new TitleMenuOption("account", 22, false);
        this.accountClicked = this.accountButton.clicked;
        this.container.addChild(this.accountButton);
        this.legendsButton = new TitleMenuOption("legends", 22, false);
        this.legendsClicked = this.legendsButton.clicked;
        this.container.addChild(this.legendsButton);
        this.editorButton = new TitleMenuOption("editor", 22, false);
        this.editorClicked = this.editorButton.clicked;
        this.container.addChild(editorButton);
        this.minigamesButton = new TitleMenuOption("minigames", 22, true);
        this.minigamesClicked = this.minigamesButton.clicked;
        this.container.addChild(minigamesButton);
        this.optionsButton = new TitleMenuOption("options", 22, false);
        this.optionsClicked = this.optionsButton.clicked;
        this.container.addChild(optionsButton);
        this.versionText = new SimpleText(12, 8355711, false, 0, 0);
        this.versionText.filters = [new DropShadowFilter(0, 0, 0)];
        this.container.addChild(this.versionText);
        this.copyrightText = new SimpleText(12, 8355711, false, 0, 0);
        this.copyrightText.text = COPYRIGHT;
        this.copyrightText.updateMetrics();
        this.copyrightText.filters = [new DropShadowFilter(0, 0, 0)];
        this.container.addChild(this.copyrightText);
    }

    public function initialize():void {
        this.updateVersionText();
        this.positionButtons();
        this.addChildren();
    }

    private function updateVersionText():void {
        this.versionText.htmlText = "RotMG " + Parameters.BUILD_VERSION;
        this.versionText.updateMetrics();
    }

    private function addChildren():void {
        addChild(this.container);
    }

    private function positionButtons():void {
        this.playButton.x = 800 / 2 - this.playButton.width / 2;
        this.playButton.y = 525;
        this.serversButton.x = 800 / 2 - this.serversButton.width / 2 - 94;
        this.serversButton.y = 532;
        this.accountButton.x = 800 / 2 - this.accountButton.width / 2 + 96;
        this.accountButton.y = 532;
        this.legendsButton.x = 550;
        this.legendsButton.y = 532;
        this.editorButton.x = 180;
        this.editorButton.y = 532;
        this.minigamesButton.x = 770 - this.minigamesButton.width;
        this.minigamesButton.y = 532;
        this.optionsButton.x = this.editorButton.x - this.optionsButton.width - 20;
        this.optionsButton.y = 532;
        this.versionText.y = 600 - this.versionText.height;
        this.copyrightText.x = 800 - this.copyrightText.width;
        this.copyrightText.y = 600 - this.copyrightText.height;
    }
}
}
