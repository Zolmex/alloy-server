package com.company.assembleegameclient.sound {
import com.gskinner.motion.GTween;

import flash.media.Sound;
import flash.media.SoundChannel;
import flash.media.SoundTransform;

public class Song {

    private static const FADE_IN_TIME:Number = 0.5;
    private static const FADE_OUT_TIME:Number = 0.5;

    private var sound:Sound;
    private var channel:SoundChannel;
    private var soundTrans:SoundTransform;
    private var volume:Number;

    public function Song(sound:Sound, vol:Number) {
        this.sound = sound;
        this.volume = vol;
        this.soundTrans = new SoundTransform();
    }

    public function changeVolume(vol:Number):void {
        this.volume = vol;
        this.soundTrans.volume = vol;
        this.channel.soundTransform = this.soundTrans;
    }

    public function fadeOut():void {
        var tween:GTween = new GTween(this.soundTrans, FADE_OUT_TIME, {"volume": 0});
        tween.onChange = onChange;
        tween.onComplete = onFadeOutComplete;
    }

    public function fadeIn():void {
        this.soundTrans.volume = 0;
        this.channel.soundTransform = this.soundTrans;

        var tween:GTween = new GTween(this.soundTrans, FADE_IN_TIME, {"volume": this.volume});
        tween.onChange = onChange;
    }

    private function onChange(tween:GTween):void {
        if (this.channel == null){
            return;
        }
        this.channel.soundTransform = this.soundTrans;
    }

    private function onFadeOutComplete(tween:GTween):void {
        if (this.channel == null){
            return;
        }

        this.channel.stop();
        this.channel = null;
        this.soundTrans = null;
    }

    public function play():void {
        if (this.channel != null){ // Already playing
            return;
        }

        this.channel = this.sound.play(0, 999, this.soundTrans);
    }

    public function stop():void {
        if (this.channel == null){ // Not playing
            return;
        }

        this.channel.stop();
        this.channel = null;
    }
}
}
