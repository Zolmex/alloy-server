package com.company.assembleegameclient.itemData {

public class CustomToolTipData {
    
    public var Name:String;
    public var Description:String;
    
    public function CustomToolTipData(obj:*) {
        this.Name = ItemData.GetValue(obj, "@name", "");
        this.Description = ItemData.GetValue(obj, "@description", null);
    }
}
}
