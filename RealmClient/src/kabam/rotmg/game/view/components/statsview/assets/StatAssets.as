package kabam.rotmg.game.view.components.statsview.assets {
import flash.utils.ByteArray;

public class StatAssets {
    [Embed(source = "statAssets.xml", mimeType = "application/octet-stream")]
    private static const statAssets:Class;

    public static function get XMLData():XML {
        var byteArray:ByteArray = new statAssets();
        return new XML(byteArray.readUTFBytes(byteArray.length));
    }
}

}

