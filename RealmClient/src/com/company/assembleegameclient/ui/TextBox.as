package com.company.assembleegameclient.ui {
import com.company.assembleegameclient.game.GameSprite;
import com.company.assembleegameclient.parameters.Parameters;
import com.company.assembleegameclient.util.TextureRedrawer;
import com.company.util.AssetLibrary;
import com.company.util.KeyCodes;

import flash.display.Bitmap;
import flash.display.BitmapData;
import flash.display.Sprite;
import flash.events.Event;
import flash.events.MouseEvent;
import flash.events.TimerEvent;
import flash.filters.GlowFilter;
import flash.text.TextField;
import flash.text.TextFieldType;
import flash.text.TextFormat;
import flash.text.engine.TextBlock;
import flash.text.engine.TextLine;
import flash.utils.Timer;
import flash.utils.getTimer;

import org.osflash.signals.natives.NativeSignal;

public class TextBox extends Sprite {

    private static const MAX_LINES:int = 10;

    private static const LINE_SCROLL:int = 3;

    private static const LINE_TIME:int = 20000;

    private static const INDENT:int = 20;

    private static const MIN_LINE_HEIGHT:int = 16;

    private static const LINE_SPACING:int = 0;

    private static const BLOCK_SPACING:int = 4;

    private static var lines_:Vector.<TextBoxLine> = new Vector.<TextBoxLine>(0);

    public static var isInputtingText:Boolean = false;


    private var gs_:GameSprite;

    private var w_:int;

    private var h_:int;

    public var textSprite_:Sprite;

    private var textSpriteYPos_:Number;

    private var inputField_:TextField;

    private var showMax_:Boolean = false;

    private var lc_:int = 0;

    private var tellers_:Array;

    private var currentTeller_:int = 0;

    private var timer_:Timer;

    private var isInputAllowed:Boolean = true;

    private var speechBubbleIcon_:Bitmap;

    private var speechBubbleContainer:Sprite;

    private var inputNotAllowedMessage:String;

    public var inputTextClicked:NativeSignal;

    public var speechBubbleClicked:NativeSignal;

    public var statsOpen:Boolean;

    public function TextBox(gs:GameSprite, w:int, h:int) {
        this.tellers_ = [];
        this.timer_ = new Timer(1000);
        super();
        this.gs_ = gs;
        this.w_ = w;
        this.h_ = h;
        this.textSprite_ = new Sprite();
        this.textSprite_.x = 2;
        this.textSprite_.filters = [new GlowFilter(0, 1, 3, 3, 2, 1)];
        this.textSprite_.mouseEnabled = false;
        this.textSprite_.mouseChildren = false;
        addChild(this.textSprite_);
        var format:TextFormat = new TextFormat();
        format.font = "Spiegel";
        format.size = 14;
        format.color = 16777215;
        format.bold = true;
        this.inputField_ = new TextField();
        this.inputField_.embedFonts = true;
        this.inputField_.defaultTextFormat = format;
        this.inputField_.type = TextFieldType.INPUT;
        this.inputField_.border = true;
        this.inputField_.borderColor = 16777215;
        this.inputField_.maxChars = 128;
        this.inputField_.filters = [new GlowFilter(0, 1, 3, 3, 2, 1)];
        this.inputField_.width = this.w_ - 2;
        this.inputField_.height = 18;
        this.inputNotAllowedMessage = "Please REGISTER to see in-game chat.";
        this.speechBubbleContainer = new Sprite();
        addChild(this.speechBubbleContainer);
        var bubbleBD:BitmapData = AssetLibrary.getImageFromSet("lofiInterfaceBig", 21);
        bubbleBD = TextureRedrawer.redraw(bubbleBD, 20, true, 0, false);
        this.speechBubbleIcon_ = new Bitmap(bubbleBD);
        this.speechBubbleIcon_.x = this.speechBubbleIcon_.x - 5;
        addEventListener(Event.ADDED_TO_STAGE, this.onAddedToStage);
        addEventListener(Event.REMOVED_FROM_STAGE, this.onRemovedFromStage);
        this.inputTextClicked = new NativeSignal(this.inputField_, MouseEvent.CLICK, MouseEvent);
        this.speechBubbleClicked = new NativeSignal(this.speechBubbleContainer, MouseEvent.CLICK, MouseEvent);
    }

    public function checkInputTabbed():Boolean {
        return stage.focus == inputField_;
    }

    public function setInputNotAllowedMessage(message:String):void {
        this.inputNotAllowedMessage = message;
    }

    public function setInputTextAllowed(isInputAllowed:Boolean):void {
        if (this.isInputAllowed != isInputAllowed) {
            this.isInputAllowed = isInputAllowed;
            if (isInputAllowed) {
                this.inputField_.border = true;
                this.inputField_.borderColor = 16777215;
                this.inputField_.type = TextFieldType.INPUT;
                this.inputField_.selectable = true;
                if (this.speechBubbleContainer.contains(this.speechBubbleIcon_)) {
                    this.speechBubbleContainer.removeChild(this.speechBubbleIcon_);
                }
                this.inputField_.x = this.inputField_.x - 29;
                this.inputField_.text = "";
                removeChild(this.inputField_);
            } else {
                this.inputField_.border = false;
                this.inputField_.type = TextFieldType.DYNAMIC;
                this.inputField_.text = this.inputNotAllowedMessage;
                this.inputField_.x = this.inputField_.x + 29;
                this.inputField_.selectable = false;
                addChild(this.inputField_);
                this.speechBubbleContainer.addChild(this.speechBubbleIcon_);
                this.placeTextField();
            }
        }
    }

    public function addText(name:String, text:String):void {
        var textBoxLine:TextBoxLine = new TextBoxLine(getTimer(), name, -1, -1, "", false, text);
        lines_.push(textBoxLine);
        this.refreshStatusBox();
    }

    public function addTextFull(name:String, objectId:int, numStars:int, recipient:String, text:String):void {
        var fromMe:Boolean = recipient != "" && name == this.gs_.model.getName();
        var toMe:Boolean = recipient == this.gs_.model.getName();
        var textBoxLine:TextBoxLine = new TextBoxLine(getTimer(), name, objectId, numStars, recipient, toMe, text);
        lines_.push(textBoxLine);
        this.refreshStatusBox();
        if (toMe) {
            if (this.tellers_.length == 0 || this.tellers_[this.tellers_.length - 1] != name) {
                this.tellers_.push(name);
            }
        } else if (recipient != Parameters.GUILD_CHAT_NAME) {
            if (fromMe) {
                if (this.tellers_.length == 0 || this.tellers_[this.tellers_.length - 1] != recipient) {
                    this.tellers_.push(recipient);
                }
            }
        }
    }

    private function onTimer(event:TimerEvent):void {
        this.refreshStatusBox();
    }

    private function clearTextSprite():void {
        while (this.textSprite_.numChildren > 0) {
            this.textSprite_.removeChildAt(0);
        }
        this.textSprite_.graphics.clear();
        this.textSpriteYPos_ = 0;
    }

    private function addTextBlock(textBlock:TextBlock):void {
        var blockSprite:Sprite = null;
        blockSprite = new Sprite();
        var textLine:TextLine = null;
        var yPos:Number = 0;

        textLine = textBlock.createTextLine(textLine, yPos == 0 ? Number(this.w_ - 4) : Number(this.w_ - 4 - INDENT));
        textLine.x = yPos == 0 ? Number(0) : Number(INDENT);
        textLine.y = textLine.ascent + yPos;
        yPos = yPos + Math.max(MIN_LINE_HEIGHT, textLine.height);
        blockSprite.addChild(textLine);

        /*while(textLine = textBlock.createTextLine(textLine,yPos == 0?Number(this.w_ - 4):Number(this.w_ - 4 - INDENT)))
        {
           textLine.x = yPos == 0?Number(0):Number(INDENT);
           textLine.y = textLine.ascent + yPos;
           yPos = yPos + Math.max(MIN_LINE_HEIGHT,textLine.height);
           blockSprite.addChild(textLine);
        }*/
        if (this.textSpriteYPos_ != 0) {
            this.textSpriteYPos_ = this.textSpriteYPos_ + BLOCK_SPACING;
        }
        blockSprite.y = this.textSpriteYPos_;
        this.textSprite_.addChild(blockSprite);
        this.textSpriteYPos_ = this.textSpriteYPos_ + yPos;
    }

    private function refreshStatusBox():void {
        var line:TextBoxLine = null;
        var textBlock:TextBlock = null;
        this.clearTextSprite();
        var now:int = getTimer();
        var startL:int = Math.max(0, lines_.length - 1 - this.lc_ - MAX_LINES);
        var endL:int = Math.min(lines_.length, startL + MAX_LINES + 1);
        for (var l:int = startL; l < endL; l++) {
            line = lines_[l];
            if (!(!this.showMax_ && now > line.time_ + 20000)) {
                textBlock = line.getTextBlock();
                this.addTextBlock(textBlock);
            }
        }
        this.placeTextField();
    }

    private function onAddedToStage(event:Event):void {
        var yMod:uint = 0;
        if (this.isInputAllowed)
            yMod = 20;
        else
            yMod = 25;

        this.inputField_.y = this.h_ - yMod;
        this.speechBubbleIcon_.y = this.inputField_.y - 8;
        this.placeTextField();
        this.timer_.addEventListener(TimerEvent.TIMER, this.onTimer);
        this.timer_.start();
    }

    private function onRemovedFromStage(event:Event):void {
        this.timer_.stop();
        this.timer_.removeEventListener(TimerEvent.TIMER, this.onTimer);
    }

    public function chatCommand():void {
        if (stage.focus == null) {
            this.selectInput();
            this.inputField_.text = "/";
            this.inputField_.setSelection(this.inputField_.length, this.inputField_.length);
        }
    }

    public function chat():void {
        if (stage.focus == null) {
            this.selectInput();
        }
    }

    public function tell():void {
        if (stage.focus == null) {
            this.selectInput();
        }
        this.insertTellPrefix(gs_.mui_.activeKeys[KeyCodes.SHIFT]);
    }

    public function partyChat() : void {
        if (stage.focus == null) {
            inputField_.text = "/p ";
            inputField_.setSelection(inputField_.length, inputField_.length);
            selectInput();
        }
    }

    public function guildChat():void {
        if (stage.focus == null) {
            this.inputField_.text = "/g ";
            this.inputField_.setSelection(this.inputField_.length, this.inputField_.length);
            this.selectInput();
        }
    }

    public function scrollChat(up:Boolean):void {
        if (!this.showMax_)
            this.showMax_ = true;
        else {
            this.lc_ = up ? Math.max(0, Math.min(lines_.length - LINE_SCROLL, this.lc_ + LINE_SCROLL))
                    : Math.max(0, this.lc_ - LINE_SCROLL);
        }
        this.refreshStatusBox();
    }

    private function insertTellPrefix(backward:Boolean):void {
        this.inputField_.text = "/tell ";
        if (this.tellers_.length != 0) {
            if (backward)
                this.currentTeller_++;
            else
                this.currentTeller_--;

            if (this.currentTeller_ <= 0)
                this.currentTeller_ = 0;

            if (this.currentTeller_ >= this.tellers_.length)
                this.currentTeller_ = this.tellers_.length - 1;

            this.inputField_.appendText(this.tellers_[this.currentTeller_] + " ");
        }
        this.inputField_.setSelection(this.inputField_.length, this.inputField_.length);
    }

    public function onEnterUp():void {
        if (stage.focus != inputField_)
            return;

        if (this.inputField_.text.length != 0) {
            if (this.gs_.map.player_ != null) {
                if (!this.handleClientCommands()) {
                    this.gs_.gsc_.playerText(this.inputField_.text);
                }
            }
            this.inputField_.text = "";
        }
        this.unselectInput();
    }

    private function handleClientCommands():Boolean { // Returns true if a client command was executed
        var text:String = this.inputField_.text;
        var words:Array = text.split(' ');
        var command:String = words[0];
        switch (command) {
            case "/showclientconstellations":
                this.addText("", "primary: " + this.gs_.map.player_.primaryConstellation);
                this.addText("", "secondary: " + this.gs_.map.player_.secondaryConstellation);
                return true;
            case "/mscale":
                var value:Number = Number(words[1]);
                if (value > 5 || value < 0.5) {
                    this.addText("", "Map scale must be bigger than 0.5 and smaller than 5.")
                } else {
                    Parameters.data_.mscale = value;
                    Parameters.save();
                    WebMain.STAGE.dispatchEvent(new Event(Event.RESIZE));
                }
                return true;
        }
        return false;
    }

    private function selectInput():void {
        this.inputField_.type = TextFieldType.INPUT;
        this.inputField_.border = true;
        this.inputField_.borderColor = 16777215;
        TextBox.isInputtingText = true;
        addChild(this.inputField_);
        if (stage != null) {
            if (gs_.allStatsView != null) {
                gs_.mui_.toggleStatsView(false);
                statsOpen = true;
            }
            stage.focus = this.inputField_;
        }
        this.placeTextField();
        this.currentTeller_ = this.tellers_.length;
    }

    private function unselectInput():void {
        if (stage != null) {
            stage.focus = null;
        }
        if (contains(this.inputField_)) {
            TextBox.isInputtingText = false;
            removeChild(this.inputField_);
            if (statsOpen) {
                gs_.mui_.toggleStatsView(false);
            }
        }
        this.placeTextField();
    }

    private function placeTextField():void {
        if (contains(this.inputField_))
            this.textSprite_.y = this.h_ - 8 - this.inputField_.height - this.textSpriteYPos_;
        else
            this.textSprite_.y = this.h_ - 4 - this.textSpriteYPos_;
    }
}
}
