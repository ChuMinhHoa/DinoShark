using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OwnedOutfit
{
    public OutfitType outfitType;
    public int saveID;
    public int outfitID;
    public int level;
    public Rarity rarity;
    public bool onUsing;
}

[System.Serializable]
public class OutfitSave : GlobalSaveDataBase
{
    public List<OwnedOutfit> ownedOutfitLists;
    public bool cheated;
    float speedBoost;
    float chefProductTime;
    float allProductTime;
    float allProductionTime;

    public override void LoadData()
    {
        SetStringSave("OutfitSave_");
        string jsonData = GetJsonData();
        if (!string.IsNullOrEmpty(jsonData))
        {
            OutfitSave dataSave = JsonUtility.FromJson<OutfitSave>(jsonData);
            ownedOutfitLists = dataSave.ownedOutfitLists;
            cheated = dataSave.cheated;
        }
        else
        {

            IsMarkChangeData();
            SaveData();
        }
        CanculateBoost();
    }

    public void CanculateBoost()
    {
        CanculateOufitsSpeedBoost();
        CanculateOufitsProductionTimeRemain();
        CanculateOufitsAllProductionTimeRemain();
    }

    public void AddOutfit(OutfitType type, int id, Rarity rarity, bool showUI = true)
    {
        OwnedOutfit outfit = new OwnedOutfit();
        outfit.level = 1;
        outfit.outfitID = id;
        outfit.rarity = rarity;
        outfit.outfitType = type;
        outfit.saveID = ownedOutfitLists.Count;
        ownedOutfitLists.Add(outfit);
        IsMarkChangeData();
        SaveData();
        if(showUI)
        {
            UIManager.instance.ShowPanelOpenOutfit(offerID, id, type, rarity);
        }
        if(PanelOutfit.Instance)
            PanelOutfit.Instance.ChangeData();
    }

    public List<OwnedOutfit> GetOwnedOutfits() {
        ownedOutfitLists.Sort((a, b) => (b.rarity - a.rarity));
        return ownedOutfitLists;
    }

    public OwnedOutfit GetOwnedOutfit(int ownedID) {
        for (int i = 0; i < ownedOutfitLists.Count; i++)
        {
            if (ownedOutfitLists[i].saveID == ownedID)
                return ownedOutfitLists[i];
        }
        return null;
    }

    public void RemoveOwnedOutfit(int ownedID)
    {
        for (int i = 0; i < ownedOutfitLists.Count; i++)
        {
            if (ownedOutfitLists[i].saveID == ownedID)
            {
                OnRevertOutfit(ownedOutfitLists[i].outfitType, ownedOutfitLists[i].saveID);
                ownedOutfitLists.Remove(ownedOutfitLists[i]);
                break;
            }   
        }
    }
    OfferID offerID;
    public bool OpenBox(OfferID offerID)
    {
        //bool hasSuitcase = ProfileManager.Instance.playerData.globalResourceSave.ConsumeSuitcase(offerID);
        this.offerID = offerID;
        OutfitBox outfitBox = ProfileManager.Instance.dataConfig.outfitBoxDataConfig.GetBoxByOffer(offerID);
        Rarity rarity = GetRarityByRate(outfitBox.rateByRarities);
        if(offerID == OfferID.RoyalSuitcase)
        {
            if(rarity == Rarity.Epic)
            {
                ProfileManager.Instance.playerData.globalResourceSave.ResetRSuitcaseCount();
            }
            if(ProfileManager.Instance.playerData.globalResourceSave.CheckRSuitcaseCount())
            {
                rarity = Rarity.Epic;
            }
        }
        TakeNewOutfit(rarity);
        bool hasSuitcase = ProfileManager.Instance.playerData.globalResourceSave.ConsumeSuitcase(offerID);
        return hasSuitcase;
    }

    public Rarity GetRarityByRate(List<RateByRarity> rateByRarities)
    {
        float rand = Random.Range(0f, 100f);
        float top = 0f;
        for (int i = 0; i < rateByRarities.Count; i++)
        {
            top += rateByRarities[i].rate;
            if (rand < top)
            {
                return rateByRarities[i].rarity;
            }
        }
        return Rarity.Common;
    }

    public void TakeNewOutfit(Rarity rarity)
    {
        int outfitType = Random.Range(0, ProfileManager.Instance.dataConfig.outfitDataConfig.outfitDatas.Count);
        int outfitId = Random.Range(0, ProfileManager.Instance.dataConfig.outfitDataConfig.outfitDatas[outfitType].outfitDatas.Count);
        OutfitType type = ProfileManager.Instance.dataConfig.outfitDataConfig.outfitDatas[outfitType].outfitType;
        if (ProfileManager.Instance.playerData.globalResourceSave.IsHaveFreePSuitcase())
        {
            type = OutfitType.Hat;
            outfitId = 1;
        }
        AddOutfit(type, outfitId, rarity);
    }

    public void OnUpgradeOutfit(OutfitType outfitType, int outfitUpgradeID) {
        for (int i = 0; i < ownedOutfitLists.Count; i++)
        {
            if (ownedOutfitLists[i].saveID == outfitUpgradeID && ownedOutfitLists[i].outfitType == outfitType)
            {
                ownedOutfitLists[i].level++;
                if(ownedOutfitLists[i].onUsing)
                {
                    CanculateBoost();
                }
                IsMarkChangeData();
                ProfileManager.Instance.playerData.SaveData();
                EventManager.TriggerEvent(EventName.ChangeBoostProfit.ToString());
                return;
            }
        }
    }

    public void OnRevertOutfit(OutfitType outfitType, int outfitRevertID) {
        for (int i = 0; i < ownedOutfitLists.Count; i++)
        {
            if (ownedOutfitLists[i].saveID == outfitRevertID && outfitType == ownedOutfitLists[i].outfitType)
            {
                int currentRevertGem = ProfileManager.Instance.dataConfig.outfitDataConfig.GetGemRevertPrice(ownedOutfitLists[i].rarity, ownedOutfitLists[i].level);
                ProfileManager.Instance.playerData.globalResourceSave.AddGem(currentRevertGem);
                ownedOutfitLists[i].level = 1;
                if (ownedOutfitLists[i].onUsing)
                {
                    CanculateBoost();
                }
                IsMarkChangeData();
                ProfileManager.Instance.playerData.SaveData();
                EventManager.TriggerEvent(EventName.ChangeBoostProfit.ToString());
                return;
            }
        }
    }

    public void OnEquipOutfit(OutfitType outfitType, int outfitRevertID) {
        for (int i = 0; i < ownedOutfitLists.Count; i++)
        {
            if (ownedOutfitLists[i].outfitType == outfitType && ownedOutfitLists[i].onUsing)
                ownedOutfitLists[i].onUsing = false;
            if (ownedOutfitLists[i].outfitType == outfitType && ownedOutfitLists[i].saveID == outfitRevertID)
                ownedOutfitLists[i].onUsing = true;
        }
        CanculateBoost();
        EventManager.TriggerEvent(EventName.ChangeBoostProfit.ToString());
        GameManager.Instance.kitchenManager.UpdateStaffSpeed();
        IsMarkChangeData();
    }

    public void OnTakeOffOutfit(OutfitType outfitType)
    {
        for (int i = 0; i < ownedOutfitLists.Count; i++)
        {
            if (ownedOutfitLists[i].outfitType == outfitType && ownedOutfitLists[i].onUsing)
                ownedOutfitLists[i].onUsing = false;
        }
        CanculateBoost();
        EventManager.TriggerEvent(EventName.ChangeBoostProfit.ToString());
        GameManager.Instance.kitchenManager.UpdateStaffSpeed();
        IsMarkChangeData();
    }

    public OwnedOutfit OnCombineOutfit(int outfitUpgradeID) {
        OwnedOutfit ownedOutfit = GetOwnedOutfit(outfitUpgradeID);
        OnRevertOutfit(ownedOutfit.outfitType, ownedOutfit.saveID);
        ownedOutfit.rarity++;
        IsMarkChangeData();
        return ownedOutfit;
    }
    float totalOutfit;
    public float GetTotalOutfitBoot()
    {
        totalOutfit = 0;
        for (int i = 0; i < ownedOutfitLists.Count; i++)
        {
            if (ownedOutfitLists[i].onUsing)
                totalOutfit+= ProfileManager.Instance.dataConfig.outfitDataConfig.GetBootOfItem(ownedOutfitLists[i].rarity, ownedOutfitLists[i].level);
        }
        return totalOutfit;
    }
    #region Move Speed
    public float GetOufitsSpeedBoost()
    {
        return speedBoost;
    }

    void CanculateOufitsSpeedBoost()
    {
        float boost = 1;
        for (int i = 0; i < ownedOutfitLists.Count; i++)
        {
            if (ownedOutfitLists[i].onUsing)
            {
                boost *= GetOufitSpeedBoostById(ownedOutfitLists[i].outfitID, ownedOutfitLists[i].outfitType, ownedOutfitLists[i].rarity);
            }
        }
        speedBoost = boost;
    }

    public float GetOufitSpeedBoostById(int id, OutfitType outfitType, Rarity rarity)
    {
        float boost = 1;
        OutfitData selected = ProfileManager.Instance.dataConfig.outfitDataConfig.GetOutfitData(id, outfitType);
        for (int i = 0; i < selected.bootOutfits.Count; i++)
        {
            if(selected.bootOutfits[i].bootType == OutfitBootType.MoveSpeed && (int)selected.bootOutfits[i].rarity <= (int)rarity)
            {
                boost *= (1 + (selected.bootOutfits[i].bootValue / 100));
            }
        }
        return boost;
    }
    #endregion

    #region Chef Production Speed
    public float GetOufitsProductionTimeRemain()
    {
        return chefProductTime;
    }

    void CanculateOufitsProductionTimeRemain()
    {
        float rate = 100;
        for (int i = 0; i < ownedOutfitLists.Count; i++)
        {
            if (ownedOutfitLists[i].onUsing)
            {
                rate += GetOufitProductionTimeReduceById(ownedOutfitLists[i].outfitID, ownedOutfitLists[i].outfitType, ownedOutfitLists[i].rarity);
            }
        }
        chefProductTime = (100 / rate);
    }

    public float GetOufitProductionTimeReduceById(int id, OutfitType outfitType, Rarity rarity)
    {
        float rate = 0;
        OutfitData selected = ProfileManager.Instance.dataConfig.outfitDataConfig.GetOutfitData(id, outfitType);
        for (int i = 0; i < selected.bootOutfits.Count; i++)
        {
            if (selected.bootOutfits[i].bootType == OutfitBootType.ProductionSpeed && (int)selected.bootOutfits[i].rarity <= (int)rarity)
            {
                rate += selected.bootOutfits[i].bootValue;
            }
        }
        return rate;
    }
    #endregion

    #region All Production Speed
    public float GetOufitsAllProductionTimeRemain()
    {
        return allProductionTime;
    }

    void CanculateOufitsAllProductionTimeRemain()
    {
        float rate = 100;
        for (int i = 0; i < ownedOutfitLists.Count; i++)
        {
            if (ownedOutfitLists[i].onUsing)
            {
                rate += GetOufitAllProductionTimeReduceById(ownedOutfitLists[i].outfitID, ownedOutfitLists[i].outfitType, ownedOutfitLists[i].rarity);
            }
        }
        allProductionTime = (100 / rate);
    }
    public float GetOufitAllProductionTimeReduceById(int id, OutfitType outfitType, Rarity rarity)
    {
        float rate = 0;
        OutfitData selected = ProfileManager.Instance.dataConfig.outfitDataConfig.GetOutfitData(id, outfitType);
        for (int i = 0; i < selected.bootOutfits.Count; i++)
        {
            if (selected.bootOutfits[i].bootType == OutfitBootType.AllEmployeesProductionSpeed && (int)selected.bootOutfits[i].rarity <= (int)rarity)
            {
                rate += selected.bootOutfits[i].bootValue;
            }
        }
        return rate;
    }
    #endregion

    #region Perfect Dish
    public bool MakeItPerfect()
    {
        float rand = Random.Range(0, 100);
        if (rand < GetOutfitPerfectChance())
            return true;
        return false;
    }

    public float GetOutfitPerfectChance()
    {
        float rate = 0;
        for (int i = 0; i < ownedOutfitLists.Count; i++)
        {
            if (ownedOutfitLists[i].onUsing)
            {
                rate += GetOufitPerfectChanceById(ownedOutfitLists[i].outfitID, ownedOutfitLists[i].outfitType, ownedOutfitLists[i].rarity);
            }
        }
        return rate;
    }

    public float GetOufitPerfectChanceById(int id, OutfitType outfitType, Rarity rarity)
    {
        float rate = 0;
        OutfitData selected = ProfileManager.Instance.dataConfig.outfitDataConfig.GetOutfitData(id, outfitType);
        for (int i = 0; i < selected.bootOutfits.Count; i++)
        {
            if (selected.bootOutfits[i].bootType == OutfitBootType.PerfectDishRate && (int)selected.bootOutfits[i].rarity <= (int)rarity)
            {
                rate += selected.bootOutfits[i].bootValue;
            }
        }
        return rate;
    }
    #endregion

    #region Instant Dish
    public bool MakeItInstant()
    {
        float rand = Random.Range(0, 100);
        if (rand < GetOutfitPerfectChance())
            return true;
        return false;
    }

    public float GetOutfitInstantChance()
    {
        float rate = 0;
        for (int i = 0; i < ownedOutfitLists.Count; i++)
        {
            if (ownedOutfitLists[i].onUsing)
            {
                rate += GetOufitInstantChanceById(ownedOutfitLists[i].outfitID, ownedOutfitLists[i].outfitType, ownedOutfitLists[i].rarity);
            }
        }
        return rate;
    }

    public float GetOufitInstantChanceById(int id, OutfitType outfitType, Rarity rarity)
    {
        float rate = 0;
        OutfitData selected = ProfileManager.Instance.dataConfig.outfitDataConfig.GetOutfitData(id, outfitType);
        for (int i = 0; i < selected.bootOutfits.Count; i++)
        {
            if (selected.bootOutfits[i].bootType == OutfitBootType.InstantDish && (int)selected.bootOutfits[i].rarity <= (int)rarity)
            {
                rate += selected.bootOutfits[i].bootValue;
            }
        }
        return rate;
    }
    #endregion

    #region X2 Mystic
    public float GetMysticBoost()
    {
        float boost = 1;
        for (int i = 0; i < ownedOutfitLists.Count; i++)
        {
            if (ownedOutfitLists[i].onUsing && ownedOutfitLists[i].rarity == Rarity.Mystic)
            {
                boost *= 2;
            }
        }
        return boost;
    }

    #endregion


    public void UnlockAllItem()
    {
        if(!cheated)
        {
            int outfitCount = ProfileManager.Instance.dataConfig.outfitDataConfig.outfitDatas[0].outfitDatas.Count;
            for (int i = 0; i < outfitCount; i++)
            {
                AddOutfit(OutfitType.Hat, i, Rarity.Epic, false);
                AddOutfit(OutfitType.Clothes, i, Rarity.Epic, false);
                AddOutfit(OutfitType.Tool, i, Rarity.Epic, false);
            }
            cheated = true;
        }
    }
}
