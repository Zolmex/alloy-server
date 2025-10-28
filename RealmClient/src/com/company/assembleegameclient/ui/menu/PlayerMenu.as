package com.company.assembleegameclient.ui.menu
{
   import com.company.assembleegameclient.game.GameSprite;
   import com.company.assembleegameclient.objects.Player;
   import com.company.assembleegameclient.ui.GameObjectListItem;
   import com.company.assembleegameclient.util.GuildUtil;
   import com.company.util.AssetLibrary;
   import flash.events.Event;
   import flash.events.MouseEvent;
   
   public class PlayerMenu extends Menu
   {
      public var gameSprite:GameSprite;
      public var playerName:String;
      public var player:Player;
      public var playerPanel:GameObjectListItem;
      
      public function PlayerMenu(gs:GameSprite, targetPlayer:Player)
      {
         var option:MenuOption = null;
         super(3552822,16777215);
         gameSprite = gs;
         playerName = targetPlayer.name_;
         player = targetPlayer;
         playerPanel = new GameObjectListItem(11776947,true,targetPlayer);
         addChild(playerPanel);

         if(gameSprite.map.allowPlayerTeleport_ && targetPlayer.isTeleportEligible(targetPlayer))
         {
            option = new TeleportMenuOption(gameSprite.map.player_);
            option.addEventListener(MouseEvent.CLICK,onTeleport);
            addOption(option);
         }

         if(gameSprite.map.player_.guildRank_ >= GuildUtil.OFFICER && (targetPlayer.guildName_ == null || targetPlayer.guildName_.length == 0))
         {
            option = new MenuOption(AssetLibrary.getImageFromSet("lofiInterfaceBig",10),16777215,"Invite to Guild");
            option.addEventListener(MouseEvent.CLICK,onGuildInvite);
            addOption(option);
         }

         if (gameSprite.map.player_.partyId != -1 && targetPlayer.partyId == -1)
         {
            option = new MenuOption(AssetLibrary.getImageFromSet("lofiInterfaceBig",10),16777215,"Invite to Party");
            option.addEventListener(MouseEvent.CLICK,onPartyInvite);
            addOption(option);
         }

         if(!targetPlayer.starred_)
         {
            option = new MenuOption(AssetLibrary.getImageFromSet("lofiInterface2",5),16777215,"Lock");
            option.addEventListener(MouseEvent.CLICK,onLock);
            addOption(option);
         }
         else
         {
            option = new MenuOption(AssetLibrary.getImageFromSet("lofiInterface2",6),16777215,"Unlock");
            option.addEventListener(MouseEvent.CLICK,onUnlock);
            addOption(option);
         }

         if(!targetPlayer.ignored_)
         {
            option = new MenuOption(AssetLibrary.getImageFromSet("lofiInterfaceBig",8),16777215,"Ignore");
            option.addEventListener(MouseEvent.CLICK,onIgnore);
            addOption(option);
         }
         else
         {
            option = new MenuOption(AssetLibrary.getImageFromSet("lofiInterfaceBig",8),16777215,"Unignore");
            option.addEventListener(MouseEvent.CLICK,onUnignore);
            addOption(option);
         }

         option = new MenuOption(AssetLibrary.getImageFromSet("lofiInterfaceBig", 7),16777215, "Trade" );
         option.addEventListener(MouseEvent.CLICK, onTrade);
         addOption(option);
      }
      
      private function onTeleport(event:Event) : void
      {
         gameSprite.map.player_.teleportTo(player);
         remove();
      }
      
      private function onGuildInvite(event:Event) : void
      {
         gameSprite.gsc_.guildInvite(playerName);
         remove();
      }

      private function onPartyInvite(event:Event) : void
      {
         gameSprite.gsc_.partyInvite(player.objectId_);
         remove();
      }
      
      private function onLock(event:Event) : void
      {
         gameSprite.map.party_.lockPlayer(player);
         remove();
      }
      
      private function onUnlock(event:Event) : void
      {
         gameSprite.map.party_.unlockPlayer(player);
         remove();
      }
      
      private function onIgnore(event:Event) : void
      {
         gameSprite.map.party_.ignorePlayer(player);
         remove();
      }
      
      private function onUnignore(event:Event) : void
      {
         gameSprite.map.party_.unignorePlayer(player);
         remove();
      }

      private function onTrade(event:Event) : void
      {
         gameSprite.gsc_.tradeRequest(player.name_);
         remove();
      }
   }
}
