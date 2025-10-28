package com.company.assembleegameclient.sound {
import com.company.assembleegameclient.parameters.Parameters;

import flash.events.IOErrorEvent;
import flash.media.Sound;
import flash.media.SoundTransform;
import flash.net.URLRequest;
import flash.utils.Dictionary;

public class SoundEffectLibrary {

    private static const URL_PATTERN:String = "{URLBASE}/sfx/{NAME}.mp3";
    private static const MUSIC_URL_PATTERN:String = "{URLBASE}/sfx/music/{NAME}.mp3";
    public static var nameMap_:Dictionary = new Dictionary();

    public function SoundEffectLibrary() {
        super();
    }

    public static function load(name:String, isSong:Boolean = false):Sound {
        return nameMap_[name] = nameMap_[name] || makeSound(name, isSong);
    }

    public static function makeSound(name:String, isSong:Boolean):Sound {
        var sound:Sound = new Sound();
        sound.addEventListener(IOErrorEvent.IO_ERROR, onIOError);
        sound.load(makeSoundRequest(name, isSong));
        return sound;
    }

    private static function makeSoundRequest(name:String, isSong:Boolean):URLRequest {
        var url:String = URL_PATTERN.replace("{URLBASE}", Parameters.getSoundsAddress()).replace("{NAME}", name);
        if (isSong){
            url = MUSIC_URL_PATTERN.replace("{URLBASE}", Parameters.getSongsAddress()).replace("{NAME}", name);
        }
        return new URLRequest(url);
    }

    public static function play(name:String, isFX:Boolean = true):void {
        var playFX:Boolean = Parameters.data_.playSFX && isFX || !isFX && Parameters.data_.playPewPew;
        if (!playFX) {
            return;
        }

        var actualVolume:Number = isFX ? Parameters.data_.sfxVolume : Parameters.data_.pewPewVolume;
        var trans:SoundTransform = null;
        var sound:Sound = load(name);
        try {
            trans = new SoundTransform(actualVolume);
            sound.play(0, 0, trans);
        } catch (error:Error) {
            trace("ERROR playing " + name + ": " + error.message);
        }
    }

    public static function onIOError(event:IOErrorEvent):void {
        trace("ERROR loading sound: " + event.text);
    }
}
}
