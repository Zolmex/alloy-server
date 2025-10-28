package com.company.assembleegameclient.ui.constellations
{
import com.company.assembleegameclient.ui.panels.*;
import com.company.assembleegameclient.account.ui.Frame;
import com.company.assembleegameclient.game.GameSprite;
   import com.company.assembleegameclient.objects.Player;
   import com.company.assembleegameclient.ui.board.GuildBoardWindow;
import com.company.assembleegameclient.ui.reusablebg.BGWithOverlay;
import com.company.assembleegameclient.ui.constellations.ConstellationsScreen;
import com.company.assembleegameclient.ui.reusablebg.ReusableBG;
import com.company.assembleegameclient.util.GuildUtil;

import flash.display.Sprite;
import flash.events.MouseEvent;
   
   public class ConstellationsPanel extends ButtonPanel
   {
      public function ConstellationsPanel(gs:GameSprite)
      {
         super(gs,"Constellations","Open");
      }
      
      override protected function onButtonClick(event:MouseEvent) : void
      {
         var p:Player = gs_.map.player_;
         if(p == null)
         {
            return;
         }
         if (gs_.map.player_.level_ >= 1) //change to 30 or whatever number when constellations are finished, idk if we need to check on server or what probably not gg
         {
            if (gs_.allStatsView != null)
               gs_.mui_.toggleStatsView(false);

            if (gs_.map.player_.primaryConstellation == -1) //doesnt have a constellation
            {
               gs_.addChild(new ConstellationsScreen(gs_, false));
            } else { //player has at least a primary constellation
               gs_.addChild(new ConstellationsScreen(gs_, true));
            }
         }
         else
         {
            var frame:BGWithOverlay = new BGWithOverlay("Error", "You must be level 30 or above!", "Close", 300, 200, false, gs_);
            gs_.addChild(frame.overlayT);
            gs_.addChild(frame);
         }
      }
   }
}
