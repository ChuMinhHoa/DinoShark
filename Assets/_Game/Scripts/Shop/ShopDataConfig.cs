using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopDataConfig", menuName = "ScriptAbleObjects/ShopDataConfig")]
public class ShopDataConfig : ScriptableObject
{
    public List<Sprite> sprConfig;
    public List<OfferData> suitcaseOfferDatas = new List<OfferData>();
    public List<OfferData> boosterOfferDatas = new List<OfferData>();
    public List<OfferData> gemsOfferDatas = new List<OfferData>();
    public List<OfferData> specialOfferDatas = new List<OfferData>();
    public OfferData revenuePack;
    public OfferData depositPack;

    public void OnEnable()
    {
        LoadSuitcaseData();
        LoadBoosterData();
        LoadGemsData();
        LoadSpecialData();
        LoadOtherPackData();
    }

    public Sprite GetSpriteByName(string name)
    {
        foreach (Sprite spr in sprConfig)
        {
            if (spr.name == name) return spr;
        }
        return null;
    }

    void LoadSuitcaseData()
    {
        suitcaseOfferDatas.Clear();
        OfferData PSuitcase = new OfferData();
        PSuitcase.title = "Free Suitcase";
        PSuitcase.offerID = OfferID.PeasantSuitcase;
        PSuitcase.price = 0;
        PSuitcase.description = "Get Free Outfit";
        PSuitcase.offerType = OfferType.GemBuy;
        suitcaseOfferDatas.Add(PSuitcase);
        ItemReward reward1 = new ItemReward(OfferID.PeasantSuitcase);
        reward1.icon = GetSpriteByName("PSuitcase");
        PSuitcase.itemRewards = new List<ItemReward>();
        PSuitcase.itemRewards.Add(reward1);

        OfferData NSuitcase = new OfferData();
        NSuitcase.title = "Nobal Suitcase"; 
        NSuitcase.offerID = OfferID.NobleSuitcase;
        NSuitcase.price = 80;
        NSuitcase.description = "75% Uncommon Outfit opened";
        NSuitcase.offerType = OfferType.GemBuy;
        suitcaseOfferDatas.Add(NSuitcase);
        ItemReward reward2 = new ItemReward(OfferID.NobleSuitcase);
        reward2.icon = GetSpriteByName("NSuitcase");
        NSuitcase.itemRewards = new List<ItemReward>();
        NSuitcase.itemRewards.Add(reward2);

        OfferData RSuitcase = new OfferData();
        RSuitcase.title = "Royal Suitcase";
        RSuitcase.offerID = OfferID.RoyalSuitcase;
        RSuitcase.price = 300;
        RSuitcase.description = "10% Epic Outfit opened";
        RSuitcase.offerType = OfferType.GemBuy;
        suitcaseOfferDatas.Add(RSuitcase);
        ItemReward reward3 = new ItemReward(OfferID.RoyalSuitcase);
        reward3.icon = GetSpriteByName("RSuitcase");
        RSuitcase.itemRewards = new List<ItemReward>();
        RSuitcase.itemRewards.Add(reward3);
    }

    void LoadBoosterData()
    {
        boosterOfferDatas.Clear();
        OfferData Revenue = new OfferData();
        Revenue.title = "4H Revenue";
        Revenue.offerID = OfferID.H4Revenue;
        Revenue.price = 30;
        Revenue.description = "";
        Revenue.offerType = OfferType.GemBuy;
        boosterOfferDatas.Add(Revenue);
        ItemReward reward1 = new ItemReward(OfferID.H4Revenue);
        reward1.icon = GetSpriteByName("icon4h");
        Revenue.itemRewards = new List<ItemReward>();
        Revenue.itemRewards.Add(reward1);

        OfferData X2Revenue = new OfferData();
        X2Revenue.title = "X2 Revenue";
        X2Revenue.offerID = OfferID.X2Revenue;
        X2Revenue.price = 50;
        X2Revenue.description = "20 Minutes";
        X2Revenue.offerType = OfferType.GemBuy;
        boosterOfferDatas.Add(X2Revenue);
        ItemReward reward2 = new ItemReward(OfferID.X2Revenue, 20);
        reward2.icon = GetSpriteByName("iconx2");
        X2Revenue.itemRewards = new List<ItemReward>();
        X2Revenue.itemRewards.Add(reward2);

        OfferData X5Revenue = new OfferData();
        X5Revenue.title = "X5 Revenue";
        X5Revenue.offerID = OfferID.X5Revenue;
        X5Revenue.price = 100;
        X5Revenue.description = "5 Minutes";
        X5Revenue.offerType = OfferType.GemBuy;
        boosterOfferDatas.Add(X5Revenue);
        ItemReward reward3 = new ItemReward(OfferID.X5Revenue, 5);
        reward3.icon = GetSpriteByName("iconx5");
        X5Revenue.itemRewards = new List<ItemReward>();
        X5Revenue.itemRewards.Add(reward3);
    }

    void LoadGemsData()
    {
        gemsOfferDatas.Clear();
        OfferData Gem1 = new OfferData();
        Gem1.title = "Few Gems";
        Gem1.offerID = OfferID.Gems;
        Gem1.price = 1.99f;
        Gem1.description = "200";
        Gem1.offerType = OfferType.IAP;
        Gem1.productID = MyIAPManager.product_Gem1;
        gemsOfferDatas.Add(Gem1);
        ItemReward G1 = new ItemReward(OfferID.Gems, 200);
        G1.icon = GetSpriteByName("Gem1");
        Gem1.itemRewards = new List<ItemReward>();
        Gem1.itemRewards.Add(G1);

        OfferData Gem2 = new OfferData();
        Gem2.title = "Dozen of Gems";
        Gem2.offerID = OfferID.Gems;
        Gem2.price = 4.99f;
        Gem2.description = "600";
        Gem2.offerType = OfferType.IAP;
        Gem2.productID = MyIAPManager.product_Gem2;
        gemsOfferDatas.Add(Gem2);
        ItemReward G2 = new ItemReward(OfferID.Gems, 600);
        G2.icon = GetSpriteByName("Gem2");
        Gem2.itemRewards = new List<ItemReward>();
        Gem2.itemRewards.Add(G2);

        OfferData Gem3 = new OfferData();
        Gem3.title = "Packs of Gems"; 
        Gem3.offerID = OfferID.Gems;
        Gem3.price = 9.99f;
        Gem3.description = "1400";
        Gem3.offerType = OfferType.IAP;
        Gem3.productID = MyIAPManager.product_Gem3;
        gemsOfferDatas.Add(Gem3);
        ItemReward G3 = new ItemReward(OfferID.Gems, 1400);
        G3.icon = GetSpriteByName("Gem3");
        Gem3.itemRewards = new List<ItemReward>();
        Gem3.itemRewards.Add(G3);

        OfferData Gem4 = new OfferData();
        Gem4.title = "Bowl of Gems ";
        Gem4.offerID = OfferID.Gems;
        Gem4.price = 19.99f;
        Gem4.description = "2900";
        Gem4.offerType = OfferType.IAP;
        Gem4.productID = MyIAPManager.product_Gem4;
        gemsOfferDatas.Add(Gem4);
        ItemReward G4 = new ItemReward(OfferID.Gems, 2900);
        G4.icon = GetSpriteByName("Gem4");
        Gem4.itemRewards = new List<ItemReward>();
        Gem4.itemRewards.Add(G4);

        OfferData Gem5 = new OfferData();
        Gem5.title = "Box of Gems";
        Gem5.offerID = OfferID.Gems;
        Gem5.price = 49.99f;
        Gem5.description = "7500";
        Gem5.offerType = OfferType.IAP;
        Gem5.productID = MyIAPManager.product_Gem5;
        gemsOfferDatas.Add(Gem5);
        ItemReward G5 = new ItemReward(OfferID.Gems, 7500);
        G5.icon = GetSpriteByName("Gem5");
        Gem5.itemRewards = new List<ItemReward>();
        Gem5.itemRewards.Add(G5);

        OfferData Gem6 = new OfferData();
        Gem6.title = "Treasure of Gems";
        Gem6.offerID = OfferID.Gems;
        Gem6.price = 99.99f;
        Gem6.description = "16000";
        Gem6.offerType = OfferType.IAP;
        Gem6.productID = MyIAPManager.product_Gem6;
        gemsOfferDatas.Add(Gem6);
        ItemReward G6 = new ItemReward(OfferID.Gems, 16000);
        G6.icon = GetSpriteByName("Gem6");
        Gem6.itemRewards = new List<ItemReward>();
        Gem6.itemRewards.Add(G6);
    }

    void LoadSpecialData()
    {
        specialOfferDatas.Clear();
        OfferData RemoveAdsPack = new OfferData();
        RemoveAdsPack.title = "Remove Ads Pack";
        RemoveAdsPack.offerID = OfferID.NoAds;
        RemoveAdsPack.offerType = OfferType.IAP;
        RemoveAdsPack.price = 19.99f;
        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
            RemoveAdsPack.productID = MyIAPManager.product_noads_ios;
        else RemoveAdsPack.productID = MyIAPManager.product_noads;
        specialOfferDatas.Add(RemoveAdsPack);
        RemoveAdsPack.itemRewards = new List<ItemReward>();
        ItemReward reward10 = new ItemReward(OfferID.NoAds);
        reward10.icon = GetSpriteByName("iconnoads");
        ItemReward reward11 = new ItemReward(OfferID.Gems, 600);
        reward11.icon = GetSpriteByName("Gem2");
        ItemReward reward12 = new ItemReward(OfferID.RoyalSuitcase);
        reward12.icon = GetSpriteByName("RSuitcase");
        RemoveAdsPack.itemRewards.Add(reward10);
        RemoveAdsPack.itemRewards.Add(reward11);
        RemoveAdsPack.itemRewards.Add(reward12);
        RemoveAdsPack.itemEquips = new List<ItemEquip>();

        OfferData StarterPack = new OfferData();
        StarterPack.title = "Starter Pack";
        StarterPack.offerID = OfferID.Vip1Pack;
        StarterPack.offerType = OfferType.IAP;
        StarterPack.price = 0.99f;
        StarterPack.productID = MyIAPManager.product_vip1pack;
        specialOfferDatas.Add(StarterPack);
        StarterPack.itemRewards = new List<ItemReward>();
        ItemReward reward21 = new ItemReward(OfferID.Gems, 300);
        reward21.icon = GetSpriteByName("Gem2");
        ItemReward reward22 = new ItemReward(OfferID.NobleSuitcase, 5);
        reward22.icon = GetSpriteByName("NSuitcase");
        StarterPack.itemRewards.Add(reward21);
        StarterPack.itemRewards.Add(reward22);
        StarterPack.itemEquips = new List<ItemEquip>();
        ItemEquip item21 = new ItemEquip(OfferID.Outfit, OutfitType.Hat, Rarity.Epic, GetSpriteByName("Hat_Ocean"), 1);
        StarterPack.itemEquips.Add(item21);

        OfferData SharpPack = new OfferData();
        SharpPack.title = "Sharp Teeth Pack";
        SharpPack.offerID = OfferID.Vip2Pack;
        SharpPack.offerType = OfferType.IAP;
        SharpPack.price = 2.99f;
        SharpPack.productID = MyIAPManager.product_vip2pack;
        specialOfferDatas.Add(SharpPack);
        SharpPack.itemRewards = new List<ItemReward>();
        ItemReward reward31 = new ItemReward(OfferID.Gems, 600);
        reward31.icon = GetSpriteByName("Gem2");
        ItemReward reward32 = new ItemReward(OfferID.NobleSuitcase, 2);
        reward32.icon = GetSpriteByName("NSuitcase");
        SharpPack.itemRewards.Add(reward31);
        SharpPack.itemRewards.Add(reward32);
        SharpPack.itemEquips = new List<ItemEquip>();
        ItemEquip item31 = new ItemEquip(OfferID.Outfit, OutfitType.Clothes, Rarity.Epic, GetSpriteByName("Cloth_Ocean"), 1);
        SharpPack.itemEquips.Add(item31);

        OfferData GreatPack = new OfferData();
        GreatPack.title = "Great Shark Pack";
        GreatPack.offerID = OfferID.Vip3Pack;
        GreatPack.offerType = OfferType.IAP;
        GreatPack.price = 19.99f;
        GreatPack.productID = MyIAPManager.product_vip3pack;
        specialOfferDatas.Add(GreatPack);
        GreatPack.itemRewards = new List<ItemReward>();
        ItemReward reward41 = new ItemReward(OfferID.Gems, 2000);
        reward41.icon = GetSpriteByName("Gem3");
        ItemReward reward42 = new ItemReward(OfferID.RoyalSuitcase, 10);
        reward42.icon = GetSpriteByName("RSuitcase");
        GreatPack.itemRewards.Add(reward41);
        GreatPack.itemRewards.Add(reward42);
        GreatPack.itemEquips = new List<ItemEquip>();
        ItemEquip item41 = new ItemEquip(OfferID.Outfit, OutfitType.Tool, Rarity.Epic, GetSpriteByName("Arm_Ocean"), 1);
        GreatPack.itemEquips.Add(item41);
    }

    void LoadOtherPackData()
    {
        revenuePack = new OfferData();
        revenuePack.title = "X2 All Revenue";
        revenuePack.offerID = OfferID.AllRevenue;
        revenuePack.description = "";
        revenuePack.offerType = OfferType.IAP;
        revenuePack.price = 4.99f;
        revenuePack.productID = MyIAPManager.revenue_pack;
        ItemReward reward1 = new ItemReward(OfferID.AllRevenue);
        reward1.icon = GetSpriteByName("revenuePack");
        revenuePack.itemRewards = new List<ItemReward>();
        revenuePack.itemRewards.Add(reward1);
        revenuePack.itemEquips = new List<ItemEquip>();

        depositPack = new OfferData();
        depositPack.title = "Sharko Bank";
        depositPack.offerID = OfferID.DepositPack;
        depositPack.description = "Upgrade to fill up gems in the Sharko Bank";
        depositPack.offerType = OfferType.IAP;
        depositPack.price = 2.99f;
        depositPack.productID = MyIAPManager.deposit_pack;
        ItemReward reward21 = new ItemReward(OfferID.Gems);
        reward21.icon = GetSpriteByName("Gem3");
        reward21.amount = 350;
        ItemReward reward22 = new ItemReward(OfferID.Gems);
        reward22.icon = GetSpriteByName("Gem3");
        reward22.amount = 700;
        depositPack.itemRewards = new List<ItemReward>();
        depositPack.itemRewards.Add(reward21);
        depositPack.itemRewards.Add(reward22);
        depositPack.itemEquips = new List<ItemEquip>();
    }
    public OfferData GetOfferDataByProductID(string productID)
    {
        foreach (var offer in gemsOfferDatas)
        {
            if (offer.productID == productID) return offer;
        }
        foreach (var offer in specialOfferDatas)
        {
            if (offer.productID == productID) return offer;
        }
        if (revenuePack.productID == productID) return revenuePack;
        if (depositPack.productID == productID) return depositPack;
        return null;
    }

    public OfferData GetOfferDataByOfferID(OfferID offerID)
    {
        foreach (var offer in gemsOfferDatas)
        {
            if (offer.offerID == offerID) return offer;
        }
        foreach (var offer in specialOfferDatas)
        {
            if (offer.offerID == offerID) return offer;
        }
        if (revenuePack.offerID == offerID) return revenuePack;
        if (depositPack.offerID == offerID) return depositPack;
        return null;
    }

    public OfferData GetSuitcaseByOfferID(OfferID iD)
    {
        foreach (var offer in suitcaseOfferDatas)
        {
            if (offer.offerID == iD) return offer;
        }
        return null;
    }
}

