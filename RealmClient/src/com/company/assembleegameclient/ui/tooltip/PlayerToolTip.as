package com.company.assembleegameclient.ui.tooltip
{
   import com.company.assembleegameclient.objects.Player;
   import com.company.assembleegameclient.ui.GameObjectListItem;
   import com.company.assembleegameclient.ui.GuildText;
   import com.company.assembleegameclient.ui.RankText;
   import com.company.assembleegameclient.ui.StatusBar;
   import com.company.assembleegameclient.ui.panels.itemgrids.EquippedGrid;
   import com.company.ui.SimpleText;
   import flash.filters.DropShadowFilter;
   
   public class PlayerToolTip extends ToolTip
   {
       
      
      public var player_:Player;
      
      private var playerPanel_:GameObjectListItem;
      
      private var rankText_:RankText;
      
      private var guildText_:GuildText;
      
      private var hpBar_:StatusBar;

      private var msBar_:StatusBar;

      private var mpBar_:StatusBar;
      
      private var clickMessage_:SimpleText;
      
      private var eGrid:EquippedGrid;
      
      public function PlayerToolTip(player:Player)
      {
         var yOffset:int = 0;
         super(3552822,0.5,16777215,1);
         this.player_ = player;
         this.playerPanel_ = new GameObjectListItem(11776947,true,this.player_);
         addChild(this.playerPanel_);
         yOffset = 34;
         this.rankText_ = new RankText(this.player_.numStars_,this.player_.accRank, false,true);
         this.rankText_.x = 6;
         this.rankText_.y = yOffset;
         addChild(this.rankText_);
         yOffset = yOffset + 30;
         if(player.guildName_ != null && player.guildName_ != "")
         {
            this.guildText_ = new GuildText(this.player_.guildName_,this.player_.guildRank_,136);
            this.guildText_.x = 6;
            this.guildText_.y = yOffset - 2;
            addChild(this.guildText_);
            yOffset = yOffset + 30;
         }
         this.hpBar_ = new StatusBar("hp_bar_background", "hp_bar_fill", "HP");
         this.hpBar_.x = 7;
         this.hpBar_.y = yOffset + 1;
         yOffset = yOffset + 24;
         this.msBar_ = new StatusBar("ms_bar_background", "ms_bar_fill", null);
         this.msBar_.x = this.hpBar_.x - 2;
         this.msBar_.y = this.hpBar_.y - 2;
         addChild(this.msBar_);
         addChild(this.hpBar_);
         this.mpBar_ = new StatusBar("mp_bar_background", "mp_bar_fill", "MP");
         this.mpBar_.x = 6;
         this.mpBar_.y = yOffset;
         addChild(this.mpBar_);
         yOffset = yOffset + 24;
         this.eGrid = new EquippedGrid(null,this.player_.slotTypes_,this.player_);
         this.eGrid.x = 8;
         this.eGrid.y = yOffset;
         addChild(this.eGrid);
         yOffset = yOffset + 52;
         this.clickMessage_ = new SimpleText(12,11776947,false,0,0);
         this.clickMessage_.text = "(Click to open menu)";
         this.clickMessage_.updateMetrics();
         this.clickMessage_.filters = [new DropShadowFilter(0,0,0)];
         this.clickMessage_.x = width / 2 - this.clickMessage_.width / 2;
         this.clickMessage_.y = yOffset;
         addChild(this.clickMessage_);
      }
      
      override public function draw() : void
      {
         this.hpBar_.draw(this.player_.hp,this.player_.maxHP);
         this.mpBar_.draw(this.player_.mp,this.player_.maxMP);
         this.msBar_.draw(this.player_.ms,this.player_.maxMS);
         this.eGrid.setItems(this.player_.equipment_);
         this.rankText_.draw(this.player_.numStars_, this.player_.accRank);
         super.draw();
      }
   }
}
