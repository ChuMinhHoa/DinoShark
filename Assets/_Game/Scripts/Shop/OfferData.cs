using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OfferType
{
    GemBuy,
    IAP
}

public enum OfferID
{
    PeasantSuitcase,
    NobleSuitcase,
    RoyalSuitcase,
    H4Revenue,
    X2Revenue,
    X5Revenue,
    Gems,
    NoAds,
    Vip1Pack,
    Vip2Pack,
    Vip3Pack,
    Outfit,
    AllRevenue,
    DepositPack
}


[System.Serializable]
public class OfferData
{
    public string title;
    public OfferID offerID;
    public float price;
    public string description;
    public OfferType offerType;
    public string productID;
    public List<ItemReward> itemRewards;
    public List<ItemEquip> itemEquips;
}

[System.Serializable]
public class ItemReward
{
    public Sprite icon;
    public OfferID itemType;
    public float amount;

    public ItemReward(OfferID id, float amt = 1)
    {
        itemType = id;
        amount = amt;
    }
}

[System.Serializable]
public class ItemEquip
{
    public Sprite icon;
    public OfferID itemType;
    public int itemId;
    public OutfitType outfitType;
    public Rarity rarity;

    public ItemEquip(OfferID id, OutfitType type, Rarity rare, Sprite iconIn, int eId = 1)
    {
        itemType = id;
        itemId = eId;
        outfitType = type;
        rarity = rare;
        icon = iconIn;
    }
}
