using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum MerchantType
{
    TipChance,          //DONE
    GemChance,
    AdsBoostEffect,     
    CustomerSpeed,      //DONE
    TipValue,           //DONE
    OfflineRevenue,
    AdsBoostDuration,   //DONE
    StaffSpeed,         //DONE
    OfflineTime,
    AdsReward,       //DONE
}

[System.Serializable]
public class Merchandise
{
    public MerchantType merchantType;
    public Sprite sprOn;
    public Sprite sprOff;
    public string description;
    public List<float> value;
    public List<int> upgrade;
}

[CreateAssetMenu(fileName = "MerchantDataConfig", menuName = "ScriptAbleObjects/MerchantDataConfig")]
public class MerchantDataConfig : ScriptableObject
{
    public List<Merchandise> merchandises;

    public int GetPriceByLevel(MerchantType type,int level = 1)
    {
        for (int i = 0; i < merchandises.Count; i++)
        {
            if (merchandises[i].merchantType == type)
            {
                return merchandises[i].upgrade[level];
            }
        }
        return 10;
    }

    public float GetValueByLevel(MerchantType type, int level = 1)
    {
        for (int i = 0; i < merchandises.Count; i++)
        {
            if (merchandises[i].merchantType == type)
            {
                return merchandises[i].value[level];
            }
        }
        return 0f;
    }
    string UNLOCK = "Unlock";
    string INCREASE = " > ";
    string TEMP = "";
    string PERCENT = "%";
    string MULTIPLY = "x";
    string HOURS = "h+";
    string MINUTES = "m";
    public string GetValueIncreaseFromLevel(MerchantType type, int level = 1)
    {
        if(level == 0)
        {
            return UNLOCK;
        }
        switch (type)
        {
            case MerchantType.TipChance:
            case MerchantType.GemChance:
                TEMP = PERCENT;
                break;
            case MerchantType.AdsBoostEffect:
            case MerchantType.CustomerSpeed:
            case MerchantType.TipValue:
            case MerchantType.OfflineRevenue:
            case MerchantType.StaffSpeed:
            case MerchantType.AdsReward:
                TEMP = MULTIPLY;
                break;
            case MerchantType.AdsBoostDuration:
                TEMP = MINUTES;
                break;
            case MerchantType.OfflineTime:
                TEMP = HOURS;
                break;
            default:
                break;
        }
        for (int i = 0; i < merchandises.Count; i++)
        {
            if (merchandises[i].merchantType == type)
            {
                if(level < GetMaxLevelByType(type))
                {
                    return merchandises[i].value[level].ToString() + TEMP + INCREASE + merchandises[i].value[level + 1].ToString() + TEMP;
                }
                
            }
        }
        return UNLOCK;
    }

    public int GetMaxLevelByType(MerchantType type)
    {
        for (int i = 0; i < merchandises.Count; i++)
        {
            if (merchandises[i].merchantType == type)
            {
                return merchandises[i].upgrade.Count - 1;
            }
        }
        return 0;
    }

    public int GetMerchandiseIndex(MerchantType type)
    {
        for (int i = 0; i < merchandises.Count; i++)
        {
            if (merchandises[i].merchantType == type)
            {
                return i;
            }
        }
        return 0;
    }
    public MerchantType GetPreviousMerchandise(MerchantType type)
    {
        for (int i = 0; i < merchandises.Count; i++)
        {
            if (merchandises[i].merchantType == type)
            {
                return merchandises[i-1].merchantType;
            }
        }
        return MerchantType.TipChance;
    }
}
