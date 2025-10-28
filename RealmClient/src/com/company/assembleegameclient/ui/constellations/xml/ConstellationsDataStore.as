package com.company.assembleegameclient.ui.constellations.xml {
import com.company.assembleegameclient.ui.constellations.misc.ConstellationNode;

public class ConstellationsDataStore {
    private static var instance:ConstellationsDataStore;

    public var constellationNodes:Vector.<ConstellationNode> = new Vector.<ConstellationNode>();

    public static function getInstance():ConstellationsDataStore {
        if (instance == null) {
            instance = new ConstellationsDataStore();
        }
        return instance;
    }

    public function loadConstellationsDataFromXML(xmlData:XML):void {
        for each (var constellationXML:XML in xmlData.Node) {
            var name:String = constellationXML.@name;
            var constellation:int = int(constellationXML.@constellation);
            var description:String = constellationXML.Description;
            var large:Boolean = constellationXML.@large == "true";
            var row:int = int(constellationXML.@row);
            var id:int = int(constellationXML.@id);

            var constellationNode:ConstellationNode = new ConstellationNode(name, constellation, description, large, row, id);
            this.constellationNodes.push(constellationNode);
        }
    }
}
}
