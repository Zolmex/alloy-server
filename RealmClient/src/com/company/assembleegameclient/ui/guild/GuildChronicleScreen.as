package com.company.assembleegameclient.ui.guild {
import com.company.assembleegameclient.game.GameSprite;
import com.company.assembleegameclient.game.events.GuildResultEvent;
import com.company.assembleegameclient.objects.Player;
import com.company.assembleegameclient.screens.TitleMenuOption;
import com.company.assembleegameclient.ui.dialogs.Dialog;
import com.company.rotmg.graphics.ScreenGraphic;

import flash.display.Sprite;
import flash.events.Event;
import flash.events.MouseEvent;

public class GuildChronicleScreen extends Sprite {


    private var gs_:GameSprite;

    private var guildPlayerList_:GuildPlayerList;

    private var continueButton_:TitleMenuOption;

    public function GuildChronicleScreen(gs:GameSprite) {
        super();
        this.gs_ = gs;
        graphics.clear();
        graphics.beginFill(2829099, 0.8);
        graphics.drawRect(0, 0, 800, 600);
        graphics.endFill();
        this.addList();
        addChild(new ScreenGraphic());
        this.continueButton_ = new TitleMenuOption("continue", 36, false);
        this.continueButton_.addEventListener(MouseEvent.CLICK, this.onContinueClick);
        addChild(this.continueButton_);
        addEventListener(Event.ADDED_TO_STAGE, this.onAddedToStage);
        addEventListener(Event.REMOVED_FROM_STAGE, this.onRemovedFromStage);
    }

    private function addList():void {
        var player:Player = this.gs_.map.player_;
        this.guildPlayerList_ = new GuildPlayerList(50, 0, player == null ? "" : player.name_, player.guildRank_);
        this.guildPlayerList_.addEventListener(GuildPlayerListEvent.SET_RANK, this.onSetRank);
        this.guildPlayerList_.addEventListener(GuildPlayerListEvent.REMOVE_MEMBER, this.onRemoveMember);
        addChild(this.guildPlayerList_);
    }

    private function removeList():void {
        this.guildPlayerList_.removeEventListener(GuildPlayerListEvent.SET_RANK, this.onSetRank);
        this.guildPlayerList_.removeEventListener(GuildPlayerListEvent.REMOVE_MEMBER, this.onRemoveMember);
        removeChild(this.guildPlayerList_);
    }

    private function onSetRank(event:GuildPlayerListEvent):void {
        this.removeList();
        this.gs_.addEventListener(GuildResultEvent.EVENT, this.onSetRankResult);
        this.gs_.gsc_.changeGuildRank(event.name_, event.rank_);
    }

    private function onSetRankResult(event:GuildResultEvent):void {
        this.gs_.removeEventListener(GuildResultEvent.EVENT, this.onSetRankResult);
        if (!event.success_) {
            this.showError(event.errorText_);
        } else {
            this.addList();
        }
    }

    private function onRemoveMember(event:GuildPlayerListEvent):void {
        this.removeList();
        this.gs_.addEventListener(GuildResultEvent.EVENT, this.onRemoveResult);
        this.gs_.gsc_.guildRemove(event.name_);
    }

    private function onRemoveResult(event:GuildResultEvent):void {
        this.gs_.removeEventListener(GuildResultEvent.EVENT, this.onRemoveResult);
        if (!event.success_) {
            this.showError(event.errorText_);
        } else {
            this.addList();
        }
    }

    private function showError(errorText:String):void {
        var dialog:Dialog = new Dialog(errorText, "Error", "Ok", null);
        dialog.addEventListener(Dialog.BUTTON1_EVENT, this.onErrorTextDone);
        stage.addChild(dialog);
    }

    private function onErrorTextDone(event:Event):void {
        var dialog:Dialog = event.currentTarget as Dialog;
        stage.removeChild(dialog);
        this.addList();
    }

    private function onContinueClick(event:MouseEvent):void {
        this.close();
    }

    private function onAddedToStage(event:Event):void {
        this.continueButton_.x = 800 / 2 - this.continueButton_.width / 2;
        this.continueButton_.y = 520;
        this.gs_.mui_.inputWhitelist.push("GChronicleBlacklist");
        stage.addEventListener(Event.RESIZE, this.onStageResize);
    }

    private function onStageResize(e:Event):void {
        var scaleW:Number = WebMain.sWidth / 800;
        var scaleH:Number = WebMain.sHeight / 600;
        this.scaleX = scaleW;
        this.scaleY = scaleH;
    }

    private function onRemovedFromStage(event:Event):void {
        this.gs_.mui_.inputWhitelist.pop();
    }

    private function close():void {
        stage.focus = null;
        parent.removeChild(this);
    }
}
}
