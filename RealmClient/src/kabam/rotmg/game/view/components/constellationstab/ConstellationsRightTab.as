package kabam.rotmg.game.view.components.constellationstab
{
import com.company.assembleegameclient.objects.Player;
import com.company.assembleegameclient.ui.constellations.misc.ConstellationNode;
import com.company.assembleegameclient.ui.constellations.misc.ConstellationUINode;
import com.company.assembleegameclient.ui.constellations.xml.ConstellationsDataStore;
import com.company.ui.SimpleText;


import flash.display.Bitmap;

   import flash.display.Sprite;
import flash.text.TextFieldAutoSize;

import kabam.rotmg.game.view.components.statsview.assets.StatAssetsManager;
import kabam.rotmg.game.view.components.statsview.left.StatHeader;

   public class ConstellationsRightTab extends Sprite
   {
      private static const WIDTH:int = 185;
      private static const HEIGHT:int = 130;

      public var nodes:Vector.<ConstellationNode>;
      public var player:Player;

      public var nothingEquippedText:SimpleText;
      public var unlockText:SimpleText;

      public var primaryText:SimpleText;
      public var secondaryText:SimpleText;
      
      public var noPrimaryText:SimpleText;
      public var noSecondaryText:SimpleText;

      public var primaryConstellation:int = 999;
      public var secondaryConstellation:int = 999;
      public var primaryNodes:Vector.<int>;
      public var secondaryNodes:Vector.<int>;

      public var primaryNodesUI:Vector.<ConstellationUINode>;
      public var secondaryNodesUI:Vector.<ConstellationUINode>;

      private static var assetManager:StatAssetsManager = StatAssetsManager.getInstance();

      public function ConstellationsRightTab(player:Player)
      {
         super();
         this.player = player;
         this.nodes = ConstellationsDataStore.getInstance().constellationNodes;
         initialize();
      }

      public function initialize():void
      {
         this.nothingEquippedText = new SimpleText(14, 0xb3b3b3);
         this.nothingEquippedText.setAutoSize(TextFieldAutoSize.CENTER);
         this.nothingEquippedText.setBold(true);
         this.nothingEquippedText.setText("No Active Constellations");
         this.nothingEquippedText.x = WIDTH / 2 - this.nothingEquippedText.width / 2;
         this.nothingEquippedText.y = 5;

         this.unlockText = new SimpleText(14, 0xb3b3b3);
         this.unlockText.setAutoSize(TextFieldAutoSize.CENTER);
         this.unlockText.setBold(true);
         this.unlockText.setText("Unlock at Lvl 10");
         this.unlockText.x = WIDTH / 2 - this.unlockText.width / 2;
         this.unlockText.y = HEIGHT / 2 - this.unlockText.height / 2;

         this.primaryText = new SimpleText(14, 0xb3b3b3);
         this.primaryText.setAutoSize(TextFieldAutoSize.CENTER);
         this.primaryText.setBold(true);
         this.primaryText.x = WIDTH / 4 - this.primaryText.width / 2;
         this.primaryText.y = 5;

         this.secondaryText = new SimpleText(14, 0xb3b3b3);
         this.secondaryText.setAutoSize(TextFieldAutoSize.CENTER);
         this.secondaryText.setBold(true);
         this.secondaryText.setText("None");
         this.secondaryText.x = (WIDTH - WIDTH / 4) - this.secondaryText.width / 2;
         this.secondaryText.y = 5;

         this.noSecondaryText = new SimpleText(14, 0xb3b3b3);
         this.noSecondaryText.setAutoSize(TextFieldAutoSize.CENTER);
         this.noSecondaryText.setBold(true);
         this.noSecondaryText.setText("No Nodes\nActive");
         this.noSecondaryText.x = (WIDTH - WIDTH / 4) - this.noSecondaryText.width / 2;
         this.noSecondaryText.y = (50 - this.noSecondaryText.height / 2) + 27;

         this.noPrimaryText = new SimpleText(14, 0xb3b3b3);
         this.noPrimaryText.setAutoSize(TextFieldAutoSize.CENTER);
         this.noPrimaryText.setBold(true);
         this.noPrimaryText.setText("No Nodes\nActive");
         this.noPrimaryText.x = WIDTH / 4 - this.noPrimaryText.width / 2;
         this.noPrimaryText.y = (50 - this.noPrimaryText.height / 2) + 27;

         update(player);
      }
      public function update(pla:Player):void
      {
         this.player = pla;

         if (this.primaryConstellation != player.primaryConstellation)
         {
            trace("primary");

            this.primaryConstellation = player.primaryConstellation;

            if (primaryConstellation == -1)
            {
               this.secondaryConstellation = player.secondaryConstellation;
               removeChildren();
               addChild(nothingEquippedText);

               if (this.player.level_ >= 10){
                  this.unlockText.setText("Choose a constellation!");
               }

               addChild(unlockText);
               trace("no constellation");
               return;
            }
            else
            {
               if (nothingEquippedText.parent != null) removeChild(nothingEquippedText);
               if (unlockText.parent != null) removeChild(unlockText);
               trace("constellation");
            }

            this.primaryText.setText(constIndexToName(primaryConstellation));
            this.primaryText.updateMetrics();
            if (this.primaryText.parent == null) addChild(this.primaryText);
            if (this.secondaryText.parent == null) addChild(this.secondaryText);
            this.secondaryText.setText("None");
         }
         if (this.primaryConstellation != -1 && this.primaryNodes != player.primaryNodes)
         {
            trace("primarynodes");
            this.primaryNodes = player.primaryNodes;

            drawNodes(nodes.filter(function(item:ConstellationNode, index:int, vector:Vector.<ConstellationNode>):Boolean {
               return item.constellation == primaryConstellation;
            }).filter(function(item:ConstellationNode, index:int, vector:Vector.<ConstellationNode>):Boolean {
               return primaryNodes[item.row + (item.large ? 0 : 1)] == item.id;
            }), false);
         }
         if (this.secondaryConstellation != player.secondaryConstellation)
         {
            trace("secondary");
            this.secondaryConstellation = player.secondaryConstellation;
            if (secondaryConstellation != -1)
               this.secondaryText.setText(constIndexToName(secondaryConstellation));
            else
               this.secondaryText.setText("None");
         }
         if (this.secondaryConstellation != -1 && this.secondaryNodes != player.secondaryNodes)
         {
            trace("secondarynodes");
            this.secondaryNodes = player.secondaryNodes;

            drawNodes(nodes.filter(function(item:ConstellationNode, index:int, vector:Vector.<ConstellationNode>):Boolean {
               return item.constellation == secondaryConstellation;
            }).filter(function(item:ConstellationNode, index:int, vector:Vector.<ConstellationNode>):Boolean {
               return secondaryNodes[item.row + (item.large ? 0 : 1)] == item.id;
            }), true);
         }
      }
      public function drawNodes(nodes:Vector.<ConstellationNode>, secondary:Boolean):void
      {
         if (nodes.length <= 0)
         {
            if (secondary) {
               if (this.noSecondaryText.parent == null) addChild(this.noSecondaryText);
            }
            else
            {
               if (this.noPrimaryText.parent == null) addChild(this.noPrimaryText);
            }
            return;
         } else
         {
            if (this.noSecondaryText.parent != null) removeChild(this.noSecondaryText);
            if (this.noPrimaryText.parent != null) removeChild(this.noPrimaryText);
         }
         var node:ConstellationUINode = null;

         if (secondary)
         {
            for each (node in secondaryNodesUI)
            {
               if (node.parent != null) removeChild(node);
               node = null;
            }
            this.secondaryNodesUI = new Vector.<ConstellationUINode>();
         } else
         {
            for each (node in primaryNodesUI)
            {
               if (node.parent != null) removeChild(node);
               node = null;
            }
            this.primaryNodesUI = new Vector.<ConstellationUINode>();
         }
         for (var i:int = 0; i < 4; i++)
         {
            var constNode:ConstellationUINode = new ConstellationUINode(nodes[i], constIndexToName(secondary ? secondaryConstellation : primaryConstellation), player.map_.gs_, false)
            constNode.x = (secondary ? 104 : 11) + ((i % 2) * 41);
            constNode.y = 41 + ((int)(i / 2) * 41);
            if (secondary) secondaryNodesUI.push(constNode);
            else primaryNodesUI.push(constNode);
            addChild(constNode);
         }
      }

      public function constIndexToName(index:int):String
      {
         switch (index)
         {
            case 0:
               return "Indus";
            case 1:
               return "Apus";
            case 2:
               return "Cygnus";
            case 3:
               return "Grus";
            case 4:
               return "Pyxis";
            default:
               return "Indus";
         }
      }
   }
}
