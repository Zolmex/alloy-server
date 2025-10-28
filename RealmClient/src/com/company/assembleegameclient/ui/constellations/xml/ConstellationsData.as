package com.company.assembleegameclient.ui.constellations.xml {
    import flash.utils.ByteArray;

public class ConstellationsData {
    [Embed(source = "constellationsData.xml", mimeType = "application/octet-stream")]
    private static const constellationsData:Class;

    public static function get XMLData():XML {
        var byteArray:ByteArray = new constellationsData();
        return new XML(byteArray.readUTFBytes(byteArray.length));
    }
}

}
