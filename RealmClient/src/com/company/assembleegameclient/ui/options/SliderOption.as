package com.company.assembleegameclient.ui.options {
import com.company.assembleegameclient.parameters.Parameters;

import flash.events.Event;

public class SliderOption extends Option{

    public function SliderOption(paramName:String, callback:Function = null) {
        super(paramName, "", "");
        this.sliderBar = new VolumeSliderBar(Parameters.data_[paramName_]);
        this.sliderBar.addEventListener(Event.CHANGE, this.onChange);
        this.callbackFunc = callback;
        addChild(this.sliderBar);
        this.sliderBar.y = this.sliderBar.height / 2;
    }

    private var sliderBar:VolumeSliderBar;
    private var callbackFunc:Function;

    override public function refresh():void {
        this.sliderBar.currentVolume = Parameters.data_[paramName_];
    }

    private function onChange(e:Event):void {
        Parameters.data_[paramName_] = this.sliderBar.currentVolume;
        if (this.callbackFunc != null) {
            this.callbackFunc(this.sliderBar.currentVolume);
        }
        Parameters.save();
    }
}
}
