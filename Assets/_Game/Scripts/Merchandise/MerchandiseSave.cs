using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDK;

[System.Serializable] 
public class OwnedMerchandise
{
    public MerchantType merchantType;
    public int level = 0;
}
[System.Serializable]
public class MerchandiseSave : GlobalSaveDataBase
{
    public List<OwnedMerchandise> ownedMerchandises = new List<OwnedMerchandise>();
    float tipChance;
    float gemChance;
    float adsBoostEffect;
    float customerSpeed;
    float tipValue;
    float offlineRevenue;
    float adsBoostDuration;
    float staffSpeed;
    float offlineTime;
    float adsRewardBoost;

    public override void LoadData()
    {
        SetStringSave("MerchandiseSave_");
        string jsonData = GetJsonData();
        if (!string.IsNullOrEmpty(jsonData))
        {
            MerchandiseSave dataSave = JsonUtility.FromJson<MerchandiseSave>(jsonData);
            ownedMerchandises = dataSave.ownedMerchandises;
        }
        else
        {
            IsMarkChangeData();
            SaveData();
        }
        LoadMerchandiseValue();
    }
    void LoadMerchandiseValue()
    {
        tipChance = GetMerchandiseValue(MerchantType.TipChance);
        gemChance = GetMerchandiseValue(MerchantType.GemChance);
        adsBoostEffect = GetMerchandiseValue(MerchantType.AdsBoostEffect);
        customerSpeed = GetMerchandiseValue(MerchantType.CustomerSpeed);
        tipValue = GetMerchandiseValue(MerchantType.TipValue);
        offlineRevenue = GetMerchandiseValue(MerchantType.OfflineRevenue);
        adsBoostDuration = GetMerchandiseValue(MerchantType.AdsBoostDuration);
        staffSpeed = GetMerchandiseValue(MerchantType.StaffSpeed);
        offlineTime = GetMerchandiseValue(MerchantType.OfflineTime);
        adsRewardBoost = GetMerchandiseValue(MerchantType.AdsReward);

        // Load Ads boost
        UIManager.instance.GetTotalPanel().adsBoostBtn.CheckAdsBoostStatus();
    }
    public int GetMerchandiseLevel(MerchantType type)
    {
        for (int i = 0; i < ownedMerchandises.Count; i++)
        {
            if (ownedMerchandises[i].merchantType == type)
            {
                return ownedMerchandises[i].level;
            }
        }
        return 0;
    }

    public bool CheckPreviousMerchandiseBought(MerchantType type)
    {
        if(ProfileManager.Instance.dataConfig.merchantDataConfig.GetMerchandiseIndex(type) == 0)
        {
            return true;
        }
        MerchantType type1 = ProfileManager.Instance.dataConfig.merchantDataConfig.GetPreviousMerchandise(type);
        return CheckMerchandiseBought(type1);
    }

    bool CheckMerchandiseBought(MerchantType type)
    {
        for (int i = 0; i < ownedMerchandises.Count; i++)
        {
            if (ownedMerchandises[i].merchantType == type)
            {
                return true;
            }
        }
        return false;
    }

    public bool BuyMerchandise(MerchantType type)
    {
        OwnedMerchandise selectedMerchandise = new OwnedMerchandise();
        bool hasOwned = false;
        for (int i = 0; i < ownedMerchandises.Count; i++)
        {
            if (ownedMerchandises[i].merchantType == type)
            {
                hasOwned = true;
                selectedMerchandise = ownedMerchandises[i];
            }
        }
        if(!hasOwned)
        {
            selectedMerchandise.merchantType = type;
            selectedMerchandise.level = 0;
            ownedMerchandises.Add(selectedMerchandise);
        }
        int price = ProfileManager.Instance.dataConfig.merchantDataConfig.GetPriceByLevel(type, selectedMerchandise.level + 1);
        if (ProfileManager.Instance.playerData.globalResourceSave.IsEnoughGem(price))
        {
            ProfileManager.Instance.playerData.globalResourceSave.ConsumeGem(price);
            selectedMerchandise.level++;
            IsMarkChangeData();
            ProfileManager.Instance.playerData.SaveData();
            UpdateStatus(type);
            ABIAnalyticsManager.Instance.TrackUpgradeTalent(type, selectedMerchandise.level);
            return true;
        }
        return false;
    }

    void UpdateStatus(MerchantType type)
    {
        switch (type)
        {
            case MerchantType.TipChance:
                tipChance = GetMerchandiseValue(MerchantType.TipChance);
                break;
            case MerchantType.GemChance:
                gemChance = GetMerchandiseValue(MerchantType.GemChance);
                break;
            case MerchantType.AdsBoostEffect:
                adsBoostEffect = GetMerchandiseValue(MerchantType.AdsBoostEffect);
                break;
            case MerchantType.CustomerSpeed:
                customerSpeed = GetMerchandiseValue(MerchantType.CustomerSpeed);
                GameManager.Instance.lobbyManager.UpdateCustomerSpeed();
                break;
            case MerchantType.TipValue:
                tipValue = GetMerchandiseValue(MerchantType.TipValue);
                break;
            case MerchantType.OfflineRevenue:
                offlineRevenue = GetMerchandiseValue(MerchantType.OfflineRevenue);
                break;
            case MerchantType.AdsBoostDuration:
                adsBoostDuration = GetMerchandiseValue(MerchantType.AdsBoostDuration);
                break;
            case MerchantType.StaffSpeed:
                staffSpeed = GetMerchandiseValue(MerchantType.StaffSpeed);
                GameManager.Instance.kitchenManager.UpdateStaffSpeed();
                break;
            case MerchantType.OfflineTime:
                offlineTime = GetMerchandiseValue(MerchantType.OfflineTime);
                break;
            case MerchantType.AdsReward:
                adsRewardBoost = GetMerchandiseValue(MerchantType.AdsReward);
                EventManager.TriggerEvent(EventName.ChangeBoostProfit.ToString());
                break;
            default:
                break;
        }
    }

    public float GetMerchandiseValue(MerchantType type)
    {
        for (int i = 0; i < ownedMerchandises.Count; i++)
        {
            if (ownedMerchandises[i].merchantType == type)
            {
                return ProfileManager.Instance.dataConfig.merchantDataConfig.GetValueByLevel(type, ownedMerchandises[i].level);
            }
        }
        return ProfileManager.Instance.dataConfig.merchantDataConfig.GetValueByLevel(type, 0);
    }

    public float GetTipChance()
    {
        return tipChance;
    }
    public float GetGemChance()
    {
        return gemChance;
    }
    public float GetAdsBoostEffect()
    {
        return adsBoostEffect;
    }
    public float GetCustomerSpeed()
    {
        return customerSpeed;
    }
    public float GetTipValue()
    {
        return tipValue;
    }
    public float GetOfflineRevenue()
    {
        return offlineRevenue;
    }
    public float GetAdsBoostDuration()
    {
        return adsBoostDuration;
    }
    public float GetStaffSpeed()
    {
        return staffSpeed;
    }
    public float GetOfflineTime()
    {
        return offlineTime;
    }
    public float GetAdsRewardBoost()
    {
        return adsRewardBoost;
    }

    public bool AbleToGetTip()
    {
        float chance = GetTipChance();
        if (chance == 0f)
        {
            return false;
        }
        float drop = Random.Range(0f, 100f);
        if(drop < chance)
        {
            return true;
        }
        return false;
    }

    public bool AbleToGetGemAds()
    {
        float top = Random.Range(0f, 100f);
        if (top < gemChance)
        {
            return true;
        }
        return false;
    }

}
