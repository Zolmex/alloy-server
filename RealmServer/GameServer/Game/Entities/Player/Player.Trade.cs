#region

using Common;
using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using GameServer.Game.Network.Messaging.Outgoing;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace GameServer.Game.Entities;

public partial class Player
{
    public enum TradeResult : byte
    {
        Successful,
        Canceled,
        Error
    }

    private const int TradeCooldown = 3000;

    public Player TradePartner;
    public Player PotentialPartner;
    public HashSet<int> TradedWith;
    public HashSet<int> PendingTrades;
    public bool[] Trade;
    public bool IsTradeAccepted;
    public long NextTradeTime;

    public void TradeRequest(string name)
    {
        if (name == Name)
        {
            SendError("Can not trade with yourself");
            return;
        }

        if (TradePartner != null)
        {
            SendError("Already trading");
            return;
        }

        var partner = World.Players
            .Where(x => x.Value.Name == name)
            .Select(x => x.Value).FirstOrDefault();
        if (partner == null)
        {
            SendError(name + " not found");
            return;
        }

        //if (partner.Client.Account.IgnoredIds.Contains(AccountId))
        //    return;

        if (partner.TradePartner != null)
        {
            SendError(name + " is already trading");
            return;
        }

        if (PotentialPartner == null || !PotentialPartner.Equals(partner))
        {
            partner.PotentialPartner = this;
            if (!PendingTrades.Contains(partner.AccountId))
            {
                RealmManager.AddTimedAction(20000, () => { TradeTimeout(partner); });
                PendingTrades.Add(partner.AccountId);
            }

            partner.User.SendPacket(new TradeRequested(Name));
            SendInfo($"Trade request sent to {partner.Name}");
        }
        else
        {
            TradePartner = partner;
            Trade = new bool[12];
            IsTradeAccepted = false;
            PotentialPartner = null;
            TradedWith.Add(partner.AccountId);
            partner.TradePartner = this;
            partner.Trade = new bool[12];
            partner.IsTradeAccepted = false;
            partner.PotentialPartner = null;
            partner.TradedWith.Add(AccountId);

            var myItems = new TradeItem[12];
            var theirItems = new TradeItem[12];
            var myPlayerDesc = XmlLibrary.Id2Player(Desc.ObjectId);
            var theirPlayerDesc = XmlLibrary.Id2Player(partner.Desc.ObjectId);
            for (var i = 0; i < 12; i++)
            {
                var item = Inventory[i];
                myItems[i] = new TradeItem
                {
                    Item = item == null ? -1 : item.ObjectType,
                    SlotType = (ItemType)myPlayerDesc.SlotTypes[i],
                    Included = false,
                    Tradeable = item != null && i >= 4 &&
                                !XmlLibrary.ItemDescs[item.ObjectType].Soulbound /*&&
                                this.User.Account.Rank < 60 && partner.User.Account.Rank < 60*/
                };

                var theirItem = partner.Inventory[i];
                theirItems[i] = new TradeItem
                {
                    Item = theirItem == null ? -1 : theirItem.ObjectType,
                    SlotType = (ItemType)theirPlayerDesc.SlotTypes[i],
                    Included = false,
                    Tradeable = theirItem != null && i >= 4 &&
                                !XmlLibrary.ItemDescs[theirItem.ObjectType].Soulbound &&
                                User.Account.Rank < 50 && TradePartner.User.Account.Rank < 50
                };
            }

            User.SendPacket(new TradeStart(myItems, theirItems, partner.Name));
            partner.User.SendPacket(new TradeStart(theirItems, myItems, Name));
        }
    }

    public void AcceptTrade(bool[] myOffer, bool[] theirOffer)
    {
        if (TradePartner == null)
        {
            OnTradeDone(TradeResult.Canceled);
            return;
        }

        if (RealmManager.WorldTime.TotalElapsedMs < NextTradeTime)
        {
            SendError("Too early to accept trade");
            return;
        }

        if (IsTradeAccepted)
        {
            return;
        }

        if (VerifyTrade(myOffer, this))
        { }

        if (VerifyTrade(theirOffer, TradePartner))
        { }

        Trade = myOffer;
        if (TradePartner.Trade.SequenceEqual(theirOffer))
        {
            var mySelectedTotal = 0;
            var theirSelectedTotal = 0;
            for (var i = 4; i < 12; i++)
            {
                if (myOffer[i])
                    mySelectedTotal++;

                if (theirOffer[i])
                    theirSelectedTotal++;
            }

            if (mySelectedTotal > TradePartner.Inventory.GetTotalFreeInventorySlots() + theirSelectedTotal ||
                theirSelectedTotal > Inventory.GetTotalFreeInventorySlots() + mySelectedTotal)
                return;

            IsTradeAccepted = true;
            TradePartner.User.SendPacket(new TradeAccepted(theirOffer, myOffer));

            if (IsTradeAccepted && TradePartner.IsTradeAccepted)
            {
                if (User.Account.Rank < 50 && TradePartner.User.Account.Rank < 50)
                {
                    DoTrade();
                }
                else
                {
                    TradePartner.OnTradeDone(TradeResult.Canceled);
                    OnTradeDone(TradeResult.Canceled);
                }
            }
        }
    }

    private void DoTrade()
    {
        var myItems = new List<Item>();
        var theirItems = new List<Item>();

        if (TradePartner == null || !TradePartner.World.Equals(World))
        {
            OnTradeDone(TradeResult.Canceled);
            return;
        }

        if (!IsTradeAccepted || !TradePartner.IsTradeAccepted)
        {
            return;
        }

        for (var i = 4; i < Trade.Length; i++)
        {
            if (Trade[i])
            {
                myItems.Add(Inventory[i]);
                Inventory[i] = null;
            }

            if (TradePartner.Trade[i])
            {
                theirItems.Add(TradePartner.Inventory[i]);
                TradePartner.Inventory[i] = null;
            }
        }

        foreach (var item in myItems)
        {
            for (var i = 4; i < 12; i++)
            {
                if (TradePartner.Inventory[i] == null ||
                    TradePartner.Trade[i])
                {
                    TradePartner.Inventory[i] = item;
                    TradePartner.Trade[i] = false;
                    break;
                }
            }
        }

        foreach (var item in theirItems)
        {
            for (var i = 4; i < 12; i++)
            {
                if (Inventory[i] == null ||
                    Trade[i])
                {
                    Inventory[i] = item;
                    Trade[i] = false;
                    break;
                }
            }
        }
        //UpdateInventory();
        //TradePartner.UpdateInventory();

        //SaveToCharacter();
        //TradePartner.SaveToCharacter();

        OnTradeDone(TradeResult.Successful);
    }

    public void ChangeTrade(bool[] newOffer)
    {
        if (TradePartner == null)
            return;

        var triedSoulbound = VerifyTrade(newOffer, this);

        IsTradeAccepted = false;
        TradePartner.IsTradeAccepted = false;
        Trade = newOffer;
        NextTradeTime = RealmManager.WorldTime.TotalElapsedMs + TradeCooldown;

        TradePartner.User.SendPacket(new TradeChanged(Trade));
        if (triedSoulbound)
        {
            SendError("Can not trade Soulbound items");
        }
    }

    public void OnTradeDone(TradeResult result)
    {
        User.SendPacket(new TradeDone(result));
        TradePartner?.User.SendPacket(new TradeDone(result));
        ResetTrade();
    }

    private bool VerifyTrade(bool[] trade, Player player)
    {
        var hadSoulbound = false;
        for (var i = 0; i < trade.Length; i++)
        {
            if (trade[i] && XmlLibrary.ItemDescs[player.Inventory[i].ObjectType].Soulbound)
            {
                hadSoulbound = true;
                trade[i] = false;
            }
        }

        return hadSoulbound;
    }

    private void ResetTrade()
    {
        if (TradePartner != null)
        {
            TradePartner.TradePartner = null;
            TradePartner.Trade = null;
            TradePartner.IsTradeAccepted = false;
            PendingTrades.Remove(TradePartner.AccountId);
        }

        TradePartner = null;
        Trade = null;
        IsTradeAccepted = false;
        NextTradeTime = 0;
    }

    private void TradeTimeout(Player partner)
    {
        if (TradedWith.Contains(partner.AccountId))
        {
            TradedWith.Remove(partner.AccountId);
            partner.TradedWith.Remove(AccountId);
            return;
        }

        SendInfo($"Trade to {partner.Name} has timed out");
        PendingTrades.Remove(partner.AccountId);
    }
}