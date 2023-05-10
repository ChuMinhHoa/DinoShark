using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OutfitType
{
    Hat,
    Clothes,
    Tool
}
public enum Rarity { 
    Common,
    Uncommon,
    Rare,
    Epic,
    Epic_1,
    Epic_2,
    Legend,
    Legend_1,
    Legend_2,
    Legend_3,
    Mystic
}
public enum OutfitBootType { 
    PerfectDishRate,
    MoveSpeed,
    ExtremelyGenerousTipRate,
    ProductionSpeed,
    AllEmployeesProductionSpeed,
    InstantDish,
    x2Revenue,
    AllPerfectDishRate,
    UpgradeDiscount
}

[System.Serializable]
public class OutfitData
{
    public string outfitName;
    public Sprite outfitIcon;
    public Mesh mesh;
    public GameObject outfitObj;
    public int outfitID;
    public List<BootOutfit> bootOutfits;
}
[System.Serializable]
public class OutfitDataByType
{
    public OutfitType outfitType;
    public Sprite outfitTypeIcon;
    public List<OutfitData> outfitDatas;
    public OutfitData GetOutfitData(int outfitID) {
        for (int i = 0; i < outfitDatas.Count; i++)
        {
            if (outfitDatas[i].outfitID == outfitID)
                return outfitDatas[i];
        }
        Debug.Log("Can't find");
        Debug.Log(outfitID);
        return null;
    }

    public int GetRamdomOutfitID()
    {
        return Random.Range(0, outfitDatas.Count);
    }
}
[System.Serializable]
public class BootOutfit
{
    public OutfitBootType bootType;
    [HideInInspector] public float bootValue;
    public Rarity rarity;
}
[System.Serializable]
public class RarityInfo
{
    public Rarity rarity;
    public float increaseBoot;
    public int priceUpgrade;
    public int maxLevel;
    public Color colorRarity;
    public Sprite sprSlot;
    public Sprite subFrame;
}

[System.Serializable]
public class CombineCondition
{
    public Rarity rarity;
    public int amount;
    public bool sameId;
}

[System.Serializable]
public class DefaultOutfit
{
    public OutfitType type;
    public GameObject outfit;
}


[CreateAssetMenu(fileName = "NewOutfitDataConfig", menuName = "ScriptAbleObjects/NewOutfitDataConfig")]
public class OutfitDataConfig : ScriptableObject
{
    public List<RarityInfo> rarityInfos;
    public List<OutfitDataByType> outfitDatas;
    public List<CombineCondition> combineConditions;
    public List<DefaultOutfit> defaultOutfits;

    public OutfitData GetOutfitData(int outfitID, OutfitType outfitType) {
        for (int i = 0; i < outfitDatas.Count; i++)
        {
            if (outfitDatas[i].outfitType == outfitType)
            return outfitDatas[i].GetOutfitData(outfitID);
        }
        return null;
    }

    public RarityInfo GetRarityInfo(Rarity rarity) {
        for (int i = 0; i < rarityInfos.Count; i++)
        {
            if (rarityInfos[i].rarity == rarity)
                return rarityInfos[i];
        }
        return null;
    }

    public int GetBootOfItem(Rarity rarity, int level) {
        int indexRarity = 0;
        float totalBoot = 0;
        for (int i = 0; i < rarityInfos.Count; i++)
        {
            if (rarity == rarityInfos[i].rarity)
            {
                indexRarity = i;
                totalBoot = rarityInfos[i].increaseBoot;
                break;
            }
        }
        totalBoot += Mathf.Pow(3, indexRarity) * (level - 1);
        return (int)totalBoot;
    }

    public Sprite GetSpriteOfType(OutfitType outfitType) {
        for (int i = 0; i < outfitDatas.Count; i++)
        {
            if (outfitDatas[i].outfitType == outfitType)
                return outfitDatas[i].outfitTypeIcon;
        }
        return null;
    }

    public int GetGemUpgradePrice(Rarity rarity, int level) {
        for (int i = 0; i < rarityInfos.Count; i++)
        {
            if (rarity == rarityInfos[i].rarity)
                return rarityInfos[i].priceUpgrade * level;
        }
        return -1;
    }

    int totalGemRevert;
    public int GetGemRevertPrice(Rarity rarity, int level) {
        totalGemRevert = 0;
        for (int i = 0; i < level - 1; i++)
            totalGemRevert += GetGemUpgradePrice(rarity, i + 1);
        return totalGemRevert;
    }

    public int GetRamdomOutfitID(OutfitType outfitType) {
        for (int i = 0; i < outfitDatas.Count; i++)
        {
            if (outfitDatas[i].outfitType == outfitType)
                return outfitDatas[i].GetRamdomOutfitID();
        }
        return -1;
    }

    private void OnEnable()
    {
        for (int i = 0; i < outfitDatas.Count; i++)
        {
            List<OutfitData> outfits = outfitDatas[i].outfitDatas;
            for (int j = 0; j < outfits.Count; j++)
            {
                for (int k = 0; k < outfits[j].bootOutfits.Count; k++)
                {
                    switch (outfits[j].bootOutfits[k].bootType)
                    {
                        case OutfitBootType.PerfectDishRate:
                            outfits[j].bootOutfits[k].bootValue = 10;
                            break;
                        case OutfitBootType.MoveSpeed:
                            outfits[j].bootOutfits[k].bootValue = 35;
                            break;
                        case OutfitBootType.ExtremelyGenerousTipRate:
                            outfits[j].bootOutfits[k].bootValue = 10;
                            break;
                        case OutfitBootType.ProductionSpeed:
                            outfits[j].bootOutfits[k].bootValue = 100;
                            break;
                        case OutfitBootType.AllEmployeesProductionSpeed:
                            outfits[j].bootOutfits[k].bootValue = 40;
                            break;
                        case OutfitBootType.InstantDish:
                            outfits[j].bootOutfits[k].bootValue = 25;
                            break;
                        case OutfitBootType.x2Revenue:
                            outfits[j].bootOutfits[k].bootValue = 2;
                            break;
                        case OutfitBootType.AllPerfectDishRate:
                            outfits[j].bootOutfits[k].bootValue = 5;
                            break;
                        case OutfitBootType.UpgradeDiscount:
                            outfits[j].bootOutfits[k].bootValue = 10;
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }

    public CombineCondition GetCombineConditionByRarity(Rarity rarity)
    {
        for (int i = 0; i < combineConditions.Count; i++)
        {
            if(combineConditions[i].rarity == rarity)
            {
                return combineConditions[i];
            }
        }
        return null;
    }

    public GameObject GetDefaultOutfitByType(OutfitType type)
    {
        for (int i = 0; i < defaultOutfits.Count; i++)
        {
            if (defaultOutfits[i].type == type)
                return defaultOutfits[i].outfit;
        }
        return null;
    }

    public string GetOutfitIncreaseByRarity(Rarity rarity)
    {
        string toReturn = "";
        for (int i = 0; i < rarityInfos.Count; i++)
        {
            if(rarityInfos[i].rarity == rarity)
            {
                toReturn += Mathf.Pow(3, i) + "% >" + Mathf.Pow(3, i + 1) + "%";
            }
        }
        return toReturn;
    }
}

