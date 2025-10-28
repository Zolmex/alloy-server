package com.company.assembleegameclient.ui.constellations
{
import com.company.assembleegameclient.parameters.Parameters;
import com.company.assembleegameclient.ui.constellations.misc.ConstellationNodeButton;
import com.company.assembleegameclient.ui.constellations.misc.ConstellationsTutorial;
import com.company.assembleegameclient.ui.reusablebg.BGWithOverlay;
import com.company.assembleegameclient.ui.reusablebg.TwoButtonUI;
import com.company.assembleegameclient.ui.constellations.misc.ConstellationBanner;
import com.company.assembleegameclient.ui.constellations.misc.ConstellationNode;
import com.company.assembleegameclient.ui.reusablebg.ReusableBG;
import com.company.assembleegameclient.ui.constellations.misc.images.TempConstellationBanner;
import com.company.assembleegameclient.ui.constellations.xml.ConstellationsDataStore;
import com.company.assembleegameclient.ui.guild.*;
   import com.company.assembleegameclient.game.GameSprite;
   import com.company.assembleegameclient.game.events.GuildResultEvent;
   import com.company.assembleegameclient.objects.Player;
   import com.company.assembleegameclient.screens.TitleMenuOption;
   import com.company.assembleegameclient.ui.dialogs.Dialog;
import com.company.assembleegameclient.util.TextureRedrawer;
import com.company.rotmg.graphics.ScreenGraphic;
   import flash.display.Sprite;
import flash.display.StageScaleMode;
import flash.events.Event;
   import flash.events.MouseEvent;
   
   public class ConstellationsScreen extends Sprite
   {
      private var gs_:GameSprite;

      public var background:BGWithOverlay;
      private var continueButton:TitleMenuOption;
      private var backToNodeSelection:TitleMenuOption;
      private var chooseConstellation:TitleMenuOption;
      private var editButton:TitleMenuOption;
      private var confirmUI:TwoButtonUI;
      private var tutorialUI:ConstellationsTutorial;

      private var nodes:Vector.<ConstellationNode>;
      private var constellationBanners:Vector.<ConstellationBanner>;
      public var constContainer1:Sprite;
      public var constContainer2:Sprite;
      public var constButtons1:Vector.<ConstellationNodeButton>;
      public var constButtons2:Vector.<ConstellationNodeButton>;

      public var confirmationOpen:Boolean;
      public var currentMode:String = "normal"; //normal or selecting
      public var hasSecondary:Boolean;

      public var chosenPrimary:Vector.<int> = new Vector.<int>(4, true);
      public var chosenSecondary:Vector.<int> = new Vector.<int>(4, true);

      public function ConstellationsScreen(gs:GameSprite, nodeSelection:Boolean)
      {
         super();
         this.gs_ = gs;
         this.gs_.mui_.clearInput();
         this.nodes = ConstellationsDataStore.getInstance().constellationNodes;
         var player:Player = gs_.map.player_;
         this.gs_.constellationsView = this;
         this.chosenPrimary = player.primaryNodes;
         this.chosenSecondary = player.secondaryNodes;
         this.hasSecondary = player.secondaryConstellation != -1;

         if (player.primaryConstellation == -1 || player.secondaryConstellation == -1) {
            this.constellationBanners = new Vector.<ConstellationBanner>();
            this.constellationBanners.push(new ConstellationBanner(gs_, "Indus", 0, nodes.filter(function(item:ConstellationNode, index:int, vector:Vector.<ConstellationNode>):Boolean {
               return item.constellation == 0;
            }), onSelectClick));
            this.constellationBanners.push(new ConstellationBanner(gs_, "Apus", 1, nodes.filter(function(item:ConstellationNode, index:int, vector:Vector.<ConstellationNode>):Boolean {
               return item.constellation == 1;
            }), onSelectClick));
            this.constellationBanners.push(new ConstellationBanner(gs_, "Cygnus", 2, nodes.filter(function(item:ConstellationNode, index:int, vector:Vector.<ConstellationNode>):Boolean {
               return item.constellation == 2;
            }), onSelectClick));
            this.constellationBanners.push(new ConstellationBanner(gs_, "Grus", 3, nodes.filter(function(item:ConstellationNode, index:int, vector:Vector.<ConstellationNode>):Boolean {
               return item.constellation == 3;
            }), onSelectClick));
            this.constellationBanners.push(new ConstellationBanner(gs_, "Pyxis", 4, nodes.filter(function(item:ConstellationNode, index:int, vector:Vector.<ConstellationNode>):Boolean {
               return item.constellation == 4;
            }), onSelectClick));
         }
         this.background = new BGWithOverlay("", "", "", 525, 320, true, gs);
         addChild(background.overlayT);
         addChild(background);

         if (player.primaryConstellation != -1) {
            this.editButton = new TitleMenuOption("edit",24,false);
            this.editButton.addEventListener(MouseEvent.CLICK,this.onEditClick);
            background.addChild(editButton);
         }

         this.continueButton = new TitleMenuOption("back",24,false);
         this.continueButton.addEventListener(MouseEvent.CLICK,this.onContinueClick);
         background.addChild(continueButton);

         if (gs_.map.player_.primaryConstellation == -1
         && Parameters.data_.showConstellationsTutorial) {
            this.tutorialUI = new ConstellationsTutorial("Tutorial!",
                    "Hello! It seems this might be your first time using constellations, so I'll give you\n" +
                    "the rundown." +
                    "\n\n" +
                    "Constellations are a power source that you attune to by completing trials of the stars,\n" +
                    "upon completing your first trial you will unlock your primary constellation, from\n" +
                    "there you can complete one more to unlock your secondary constellation." +
                    "\n\n" +
                    "Once a constellation is unlocked, this trial picker will be replaced with a menu that\n" +
                    "lets you select your chosen constellation nodes. Each constellation has four large\n" +
                    "nodes and nine smaller nodes, you are able to pick one large node and one smaller\n" +
                    "node in each row, meaning one large node and three small nodes in total.\n" +
                    "The same goes for your secondary constellation, allowing for in-depth set-ups." +
                    "\n\n" +
                    "That's about it for constellations, check the box below to never see this tutorial again!", "Got it!", 650, 400, false, gs);
            addChild(tutorialUI.overlayT);
            addChild(tutorialUI);
         }

         addEventListener(Event.ADDED_TO_STAGE,this.onAddedToStage);
         addEventListener(Event.REMOVED_FROM_STAGE,this.onRemovedFromStage);
      }

      private function onEditClick(event:MouseEvent) : void {
         this.currentMode = "selecting";
         this.editButton.setText("save");
         this.editButton.removeEventListener(MouseEvent.CLICK,this.onEditClick);
         this.editButton.addEventListener(MouseEvent.CLICK,this.onSaveClick);

         var totalButtons:Vector.<ConstellationNodeButton> = constButtons2 != null ? constButtons1.concat(constButtons2) : constButtons1;
         for each (var node:ConstellationNodeButton in totalButtons)
            node.activate();
      }

      private function checkNodesActive(nodes:Vector.<int>) : Boolean
      {
         for each (var chosenNode:int in nodes)
            if (chosenNode == -1)
               return true;
         return false;
      }

      private function onSaveClick(event:MouseEvent) : void {
         if (checkNodesActive(chosenPrimary) || (hasSecondary && checkNodesActive(chosenSecondary)))
         {
            var frame:BGWithOverlay = new BGWithOverlay("Error", "You must choose one node per row!", "Close", 300, 200, false, gs_);
            gs_.addChild(frame.overlayT);
            gs_.addChild(frame);
            return;
         }

         this.currentMode = "normal";
         this.editButton.setText("edit");
         this.editButton.addEventListener(MouseEvent.CLICK,this.onEditClick);
         this.editButton.removeEventListener(MouseEvent.CLICK,this.onSaveClick);
         this.gs_.gsc_.constellationsSave(toData(chosenPrimary), toData(chosenSecondary));

         var totalButtons:Vector.<ConstellationNodeButton> = constButtons2 != null ? constButtons1.concat(constButtons2) : constButtons1;
         for each (var node:ConstellationNodeButton in totalButtons)
            node.deactivate();
      }

      private static function toData(array:Vector.<int>):int {
         var number:int = 0;
         for (var i:int = 0; i < array.length; i++) {
            number += array[i] * Math.pow(10, array.length - i - 1);
         }
         return number == -1111 ? -1 : number;
      }

      private function onConfirm(e:MouseEvent) : void {
         this.gs_.gsc_.constellationsTrial();
         this.confirmUI.close();
         var tpUI:BGWithOverlay = new BGWithOverlay("Teleporting!", "Teleporting in three seconds...", "Ok", 250, 100, false, gs_);
         addChild(tpUI.overlayT);
         addChild(tpUI);
         this.confirmationOpen = true;
      }

      private function onCancel(e:MouseEvent) : void {
         this.confirmUI.close();
         this.confirmationOpen = false;
      }

      private function onSelectClick(e:MouseEvent) : void {
         if (confirmationOpen)
            return;
         this.confirmUI = new TwoButtonUI("Confirmation", "Are you sure you wish to\nenter this constellation's trial?", "Cancel", "Go!", onCancel, onConfirm, 280, 150, gs_);
         addChild(confirmUI.overlayT);
         addChild(confirmUI);
         this.confirmationOpen = true;
      }

      private function toggleBannerChoosing(on:Boolean):void {
         if (on) {
            this.background.removeChild(constContainer1);
            this.background.removeChild(chooseConstellation);
            this.background.removeChild(editButton);
            this.background.removeChild(continueButton);

            for each (var banner:ConstellationBanner in constellationBanners) {
               this.background.addChild(banner);
            }

            this.backToNodeSelection = new TitleMenuOption("back",24,false);
            this.backToNodeSelection.addEventListener(MouseEvent.CLICK,backToSelecting);
            this.background.addChild(backToNodeSelection);
            this.backToNodeSelection.x = background.width_ / 2 - this.backToNodeSelection.width / 2 - 6;
            this.backToNodeSelection.y = this.background.height_ - this.backToNodeSelection.height - 15;
         } else {
            this.background.addChild(constContainer1);
            this.background.addChild(chooseConstellation);
            this.background.addChild(editButton);
            this.background.addChild(continueButton);

            for each (var banner1:ConstellationBanner in constellationBanners) {
               this.background.removeChild(banner1);
            }

            this.background.removeChild(backToNodeSelection);
         }
      }
      
      private function onAddedToStage(event:Event) : void {
         if (gs_.map.player_.secondaryConstellation == -1)
            for each (var banner:ConstellationBanner in constellationBanners) {
               banner.x = x + ((100 + (banner.position >= 1 ? 4 : 0)) * banner.position) - 2;
               if (gs_.map.player_.primaryConstellation == -1)
                  this.background.addChild(banner);
            }
         this.continueButton.x = (gs_.map.player_.primaryConstellation != -1 ? ((background.width_ * 2) / 3) : background.width_ / 2) - this.continueButton.width / 2 - 6;
         this.continueButton.y = this.background.height_ - this.continueButton.height - 15;
         this.gs_.mui_.inputWhitelist.push("ConstellationScreenBlacklist");
         drawNodes();
      }

      private function drawNodes():void {
         var player:Player = gs_.map.player_;
         if (player.primaryConstellation != -1) {
            this.editButton.x = background.width_ / 3 - this.editButton.width / 2 - 6;
            this.editButton.y = this.background.height_ - this.editButton.height - 15;

            this.constContainer1 = createAndAddContainer(19, 19, background);
            this.constButtons1 = drawConstellationNodes(player.primaryConstellation, this.constContainer1);

            if (player.secondaryConstellation != -1) {
               this.constContainer2 = createAndAddContainer(constContainer1.x + 245, 19, background);
               this.constButtons2 = drawConstellationNodes(player.secondaryConstellation, this.constContainer2);
            } else {
               this.chooseConstellation = new TitleMenuOption("choose secondary\nconstellation",24,true);
               this.chooseConstellation.addEventListener(MouseEvent.CLICK,this.onConstellationSelect);
               background.addChild(chooseConstellation);
               this.chooseConstellation.x = background.width_ / 4 + background.width_ / 2 - this.chooseConstellation.width / 2 - 6;
               this.chooseConstellation.y = this.background.height_ / 2 - this.chooseConstellation.height / 2;
            }
         }
      }

      private static function createAndAddContainer(xPos:int, yPos:int, parentContainer:Sprite):Sprite {
         var container:Sprite = new Sprite();
         container.graphics.beginFill(0x252525, 1);
         container.graphics.drawRect(0, 0, 230, 230);
         container.graphics.endFill();
         container.x = xPos;
         container.y = yPos;
         parentContainer.addChild(container);
         return container;
      }

      private function drawConstellationNodes(constellation:int, container:Sprite):Vector.<ConstellationNodeButton> {
         var nodes:Vector.<ConstellationNode> = getNodesOfConstellation(constellation);
         var buttons:Vector.<ConstellationNodeButton> = new Vector.<ConstellationNodeButton>();

         for each (var node:ConstellationNode in nodes) {
            var newButton:ConstellationNodeButton = new ConstellationNodeButton(node, gs_);
            positionNode(node, newButton, 19);
            buttons.push(newButton);
            container.addChild(newButton);
         }
         var player:Player = gs_.map.player_;
         for each (var button:ConstellationNodeButton in buttons) {
            button.setRow(getNodesOfRow(buttons, button.nodeData));
            if (button.nodeData.constellation == player.primaryConstellation && player.primaryNodes[button.index()] == button.nodeData.id) {
               chosenPrimary[button.index()] = button.nodeData.id;
               button.selected = true;
               button.reloadSprite();
            } else if (button.nodeData.constellation == player.secondaryConstellation && player.secondaryNodes[button.index()] == button.nodeData.id) {
               chosenSecondary[button.index()] = button.nodeData.id;
               button.selected = true;
               button.reloadSprite();
            }
         }
         return buttons;
      }

      private static function positionNode(node:ConstellationNode, button:ConstellationNodeButton, containerX:int):void {
         button.y = node.large ? 30 : (80 + (node.row * 50));
         button.x = containerX + (node.large ? (20 + ((node.id - 1) * 40)) : (30 + ((node.id - 1) * 50)));
      }
      
      private function onRemovedFromStage(event:Event) : void
      {
         this.gs_.constellationsView = null;
         this.gs_.mui_.inputWhitelist.pop();
      }
      
      private function close() : void
      {
         stage.focus = null;
         parent.removeChild(this);
      }

      private function onContinueClick(event:MouseEvent) : void {
         this.close();
      }
      private function onConstellationSelect(event:MouseEvent) : void {
         toggleBannerChoosing(true);
      }
      private function backToSelecting(event:MouseEvent) : void {
         toggleBannerChoosing(false);
      }

      private static function getNodesOfRow(nodes:Vector.<ConstellationNodeButton>, targetNode:ConstellationNode):Vector.<ConstellationNodeButton> {
         var row:Vector.<ConstellationNodeButton> = new Vector.<ConstellationNodeButton>();
         for each (var node:ConstellationNodeButton in nodes) {
            if (node.nodeData.row == targetNode.row && node.nodeData.large == targetNode.large) {
               row.push(node);
            }
         }
         return row;
      }

      private function getNodesOfConstellation(constellation:int):Vector.<ConstellationNode> {
         var nodes:Vector.<ConstellationNode> = new Vector.<ConstellationNode>();
         for each (var node:ConstellationNode in this.nodes) {
            if (node.constellation == constellation)
               nodes.push(node);
         }
         return nodes;
      }
   }
}
