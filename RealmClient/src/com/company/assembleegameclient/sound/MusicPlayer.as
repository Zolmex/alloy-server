package com.company.assembleegameclient.sound {
import com.company.assembleegameclient.parameters.Parameters;
import com.company.assembleegameclient.parameters.Parameters;
import com.gskinner.motion.GTween;

import flash.events.Event;

import flash.media.Sound;
import flash.media.SoundChannel;
import flash.media.SoundTransform;
import flash.utils.Dictionary;

public class MusicPlayer {

    private static const VOLUME_MULT:Number = 0.5; // Multiply by half the volume cus music is so loud

    private static var currentSong:Song;
    private static var selectedSong:String;
    private static var loadedSound:Sound;
    private static var volume:Number = Parameters.data_.musicVolume * VOLUME_MULT;

    public static function changeVolume(vol:Number):void {
        if (!Parameters.data_.playMusic){
            return;
        }

        volume = vol * VOLUME_MULT;
        if (volume == 0){
            stop();
        }
        else {
            resume();
            currentSong.changeVolume(volume);
        }
    }

    public static function switchOnOff():void {
        if (Parameters.data_.playMusic){
            resume();
        }
        else {
            stop();
        }
    }

    public static function playSong(name:String, resume:Boolean = false):void {
        if (name == "" || name == null || (!resume && name == selectedSong)){
            return;
        }

        selectedSong = name;
        loadedSound = SoundEffectLibrary.load(name, true);
        if (loadedSound.bytesLoaded == 0) {
            loadedSound.addEventListener(Event.COMPLETE, onSongLoaded);
        }
        else {
            onSongLoaded(null);
        }
    }

    private static function onSongLoaded(e:Event):void {
        if (!Parameters.data_.playMusic){
            return;
        }

        if (currentSong != null){
            currentSong.fadeOut();
        }

        currentSong = new Song(loadedSound, volume);
        currentSong.play();
        currentSong.fadeIn();
    }

    public static function resume():void {
        playSong(selectedSong, true);
    }

    private static function stop():void {
        if (currentSong == null){
            return;
        }

        currentSong.stop();
    }
}
}
