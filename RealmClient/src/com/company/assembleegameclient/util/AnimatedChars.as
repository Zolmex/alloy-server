package com.company.assembleegameclient.util {
import flash.display.BitmapData;
import flash.utils.Dictionary;
import flash.utils.getQualifiedClassName;

public class AnimatedChars {

    public static var nameMap_:Dictionary = new Dictionary();


    public function AnimatedChars() {
        super();
    }

    public static function getAnimatedChar(name:String, id:int):AnimatedChar {
        var chars:Vector.<AnimatedChar> = nameMap_[name];
        if (chars == null || id >= chars.length) {
            return null;
        }
        return chars[id];
    }

    public static function add(name:String, dataClass:Class, maskClass:Class, charWidth:int, charHeight:int, sheetWidth:int, sheetHeight:int, firstDir:int):void {
        var images:BitmapData;
        var masks:BitmapData = null
        if (dataClass != null) {
            images = new dataClass().bitmapData as BitmapData;
        } else {
            trace("Failed to create animated sprite sheet", name);
            return;
        }
        if (maskClass != null) {
            masks = new maskClass().bitmapData as BitmapData;
        }

        var image:MaskedImage = null;
        var chars:Vector.<AnimatedChar> = new Vector.<AnimatedChar>();
        var charImages:MaskedImageSet = new MaskedImageSet();
//        trace("Loading", name, "sheet")
        charImages.addFromBitmapData(images, masks, sheetWidth, sheetHeight);
        for each(image in charImages.images_) {
            chars.push(new AnimatedChar(image, charWidth, charHeight, firstDir));
        }
        nameMap_[name] = chars;
    }
}
}
