using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class DailyReward {
    public int day;
    public List<ItemReward> itemRewards;
    public List<ItemEquip> itemEquips;
    public bool received;
    public bool canReceive;

    public void DeclareNewReward() {
        itemRewards = new List<ItemReward>();
        itemEquips = new List<ItemEquip>();
        itemRewards.Clear();
        itemEquips.Clear();
    }
}

[CreateAssetMenu(fileName = "DailyDataconfig", menuName = "ScriptAbleObjects/New DailyDataconfig")]
public class DailyDataconfig : ScriptableObject
{
    public SpriteDataConfig spriteDataConfig;
    public TextAsset dailyRewardCSV;
    public List<DailyReward> dailyRewards = new List<DailyReward>();

    public void OnReceive(int index, bool received) {
        dailyRewards[index].received = received;
    }

    public void CanReceive(int index, bool canReceive) {
        dailyRewards[index].canReceive = canReceive;
    }

    private void OnEnable()
    {
        ImportData();
    }
    int indexInData;
    int day;
    Sprite sprIcon;
    OfferID offerID;
    Rarity rarity = Rarity.Common;
    OutfitType outfitType = OutfitType.Hat;
    DailyReward currentDailyReward;
    void ImportData() {
        dailyRewards.Clear();
        indexInData = 0;
        List<Dictionary<string, string>> datas = CSVReader.Read(dailyRewardCSV);
        while (indexInData < datas.Count)
        {
            day = int.Parse(datas[indexInData]["Day"]);
            offerID = (OfferID)System.Enum.Parse(typeof(OfferID), datas[indexInData]["RewardType"]);
            currentDailyReward = IsExistDay(day);

            if (datas[indexInData]["OutfitType"]!="None")
                outfitType = (OutfitType)System.Enum.Parse(typeof(OutfitType), datas[indexInData]["OutfitType"]);

            if (datas[indexInData]["OutfitRarity"] != "None")
                rarity = (Rarity)System.Enum.Parse(typeof(Rarity), datas[indexInData]["OutfitRarity"]);

            if (currentDailyReward == null)
            {
                DailyReward newDailyReward = new DailyReward();
                newDailyReward.day = day;
                newDailyReward.received = false;
                newDailyReward.DeclareNewReward();
                dailyRewards.Add(newDailyReward);
                currentDailyReward = newDailyReward;
            }

            AddReward(int.Parse(datas[indexInData]["Amount"]), datas[indexInData]["IconSprite"], outfitType, int.Parse(datas[indexInData]["OutfitID"]), rarity);
            indexInData++;
        }
    }

    void AddReward(int amount, string spriteName, OutfitType outfitType = OutfitType.Hat, int outfitID = 0, Rarity outfitRarity = Rarity.Common)
    {
        if (offerID != OfferID.Outfit)
        {
            ItemReward newItemReward = new ItemReward(offerID, amount);
            sprIcon = spriteDataConfig.GetSpriteDailyReward(spriteName);
            newItemReward.icon = sprIcon;
            AddItemRewardForDay(newItemReward);
        }
        else
        {
            sprIcon = spriteDataConfig.GetSpriteDailyReward(spriteName);
            ItemEquip newItemEquip = new ItemEquip(offerID, outfitType, outfitRarity, sprIcon, outfitID);
            AddOutfitRewardForDay(newItemEquip);
        }
    }

    void AddItemRewardForDay(ItemReward itemReward) {
        currentDailyReward.itemRewards.Add(itemReward);
    }

    void AddOutfitRewardForDay(ItemEquip itemEquip) {
        currentDailyReward.itemEquips.Add(itemEquip);
    }

    DailyReward IsExistDay(int day) {
        for (int i = 0; i < dailyRewards.Count; i++)
        {
            if (dailyRewards[i].day == day)
                return dailyRewards[i];
        }
        return null;
    }

    public DailyReward GetDailyRewardByDay(int day) 
    {
        for (int i = 0; i < dailyRewards.Count; i++)
        {
            if (dailyRewards[i].day == day)
                return dailyRewards[i];
        }
        return null;
    }
}
