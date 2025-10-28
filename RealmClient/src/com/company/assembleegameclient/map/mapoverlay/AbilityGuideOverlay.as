package com.company.assembleegameclient.map.mapoverlay {
import com.company.assembleegameclient.game.*;
import com.company.assembleegameclient.itemData.SheathDesc;
import com.company.assembleegameclient.map.Camera;
import com.company.assembleegameclient.objects.GameObject;
import com.company.assembleegameclient.objects.ObjectLibrary;
import com.company.assembleegameclient.objects.Player;
import com.company.util.KeyCodes;

import flash.display.GradientType;

import flash.display.Graphics;

import flash.display.Sprite;
import flash.geom.Point;

public class AbilityGuideOverlay extends Sprite implements IMapOverlayElement {

    private var go:GameObject;

    public function AbilityGuideOverlay(go:GameObject) {
        this.go = go;
    }

    public function draw(camera:Camera, time:int):Boolean {
        if (!MapUserInput.instance.isKeyDown[MapUserInput.SHIFT_KEY]){
            graphics.clear();
            return true;
        }

        var identifier:String = ObjectLibrary.getIdFromType(this.go.objectType_);
        switch (identifier){
            case "Rogue":
                break;
            case "Archer":
                break;
            case "Wizard":
                break;
            case "Priest":
                break;
            case "Warrior":
                break;
            case "Knight":
                break;
            case "Paladin":
                break;
            case "Assassin":
                break;
            case "Necromancer":
                break;
            case "Huntress":
                break;
            case "Trickster":
                break;
            case "Mystic":
                break;
            case "Sorcerer":
                break;
            case "Ninja":
                this.drawSheathGuide();
                break;
        }
        x = int(this.go.posS_[0]);
        y = int(this.go.posS_[1]);
        return true;
    }

    private function drawSheathGuide():void {
        if (this.go.equipment_[1] == null || this.go.equipment_[1].Sheath == null){
            return;
        }

        var g:Graphics = this.graphics;
        g.clear();
        g.lineStyle(2, 0);

        var sheath:SheathDesc = this.go.equipment_[1].Sheath;
        var r:Number = sheath.Radius * 50;
        g.drawCircle(0, 0, r);

        g.lineStyle();
    }

    public function dispose():void {
        parent.removeChild(this);
    }

    public function getGameObject():GameObject {
        return this.go;
    }
}
}
