package kabam.rotmg.game.view.components.statsview.assets {

import flash.display.Bitmap;
import flash.display.BitmapData;
import flash.geom.Point;
import flash.geom.Rectangle;
import flash.utils.Dictionary;

public class StatAssetsManager {

    private static var instance:StatAssetsManager;
    public var spriteSheet:BitmapData;
    public var assetsDict:Dictionary;

    public static function getInstance():StatAssetsManager {
        if (instance == null) {
            instance = new StatAssetsManager();
        }
        return instance;
    }

    public function getAssetByName(name:String):Bitmap {
        var bitmapData:BitmapData = assetsDict[name];
        if (bitmapData) {
            return new Bitmap(bitmapData);
        }
        return null;
    }

    public function parse(sheet:BitmapData, xmlData:XML):void {
        this.spriteSheet = sheet;
        this.assetsDict = new Dictionary();

        for each (var statAsset:XML in xmlData.statAsset) {
            var name:String = statAsset.@name;
            var rect:Rectangle = new Rectangle(statAsset.@x, statAsset.@y, statAsset.@width, statAsset.@height);

            var assetBitmapData:BitmapData = new BitmapData(rect.width, rect.height);
            assetBitmapData.copyPixels(spriteSheet, rect, new Point(0, 0));
            assetsDict[name] = assetBitmapData;
        }
    }
}
}


