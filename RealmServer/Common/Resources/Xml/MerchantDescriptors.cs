#region

using System;
using System.Linq;
using System.Xml.Linq;
using Common.Utilities;

#endregion

namespace Common.Resources.Xml;

public class MerchantDesc {
    public readonly MerchandiseEntry[] Entries;
    public readonly string Region;

    public MerchantDesc(XElement e, string region) {
        Region = region;
        Entries = e.Elements("Entry").Select(x => new MerchandiseEntry(x)).ToArray();
    }
}

public class MerchandiseEntry {
    public readonly CurrencyType Currency;
    public readonly MerchandiseDiscount[] Discounts;
    public readonly int MaxDuration;
    public readonly int MaxStock;
    public readonly int MinDuration;

    public readonly int MinStock;
    public readonly string ObjectId;
    public readonly int Price;
    public readonly int RankReq;

    public MerchandiseEntry(XElement e) {
        ObjectId = e.GetAttribute<string>("id");
        Price = e.GetAttribute<int>("price");
        RankReq = e.GetAttribute<int>("rankReq");
        var currency = e.GetAttribute<string>("currency");
        Currency = currency == null ? CurrencyType.Fame : (CurrencyType)Enum.Parse(typeof(CurrencyType), currency);

        var stock = e.Element("Stock");
        MinStock = stock.GetAttribute<int>("min");
        MaxStock = stock.GetAttribute<int>("max");

        var duration = e.Element("Duration");
        MinDuration = duration.GetAttribute<int>("min");
        MaxDuration = duration.GetAttribute<int>("max");

        Discounts = e.Elements("Discount").Select(x => new MerchandiseDiscount(x)).ToArray();
    }
}

public class MerchandiseDiscount {
    public readonly float Probability;
    public readonly int Value;

    public MerchandiseDiscount(XElement e) {
        Value = int.Parse(e.Value);
        Probability = e.GetAttribute<float>("prob");
    }
}