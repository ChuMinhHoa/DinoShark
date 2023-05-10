using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
#endif

[System.Serializable]
public class UpgradeData
{
    public string upgradeName;
    public UpgradeType upgradeType;
    public FoodID foodID;
    public float refAmount;
    public BigNumber priceUpgrade;
    public Sprite sprIcon;
    public Sprite sprIconBootType;
    public int refID;
    public int upgradeID;
}
[CreateAssetMenu(fileName = "UpgradeDataByLevel", menuName = "ScriptAbleObjects/UpgradeData/New UpgradeDataByLevel")]
public class UpgradeDataByLevel : ScriptableObject
{
    public SpriteDataConfig spriteDataConfig;
    public TextAsset upgradeCSVBylevel;
    public List<UpgradeData> upgradeDatas;

    public UpgradeData GetUpgradeDataByType(UpgradeType upgradeType) {
        for (int i = 0; i < upgradeDatas.Count; i++)
        {
            if (upgradeDatas[i].upgradeType == upgradeType)
                return upgradeDatas[i];
        }
        return null;
    }
    public UpgradeData GetUpgradeDataByID(int upgradeID)
    {
        for (int i = 0; i < upgradeDatas.Count; i++)
        {
            if (upgradeDatas[i].upgradeID == upgradeID)
                return upgradeDatas[i];
        }
        return null;
    }

#if UNITY_EDITOR
    private void OnEnable()
    {
        ImportUpgradelevel();
    }

    private void ImportUpgradelevel()
    {
        upgradeDatas.Clear();
        List<Dictionary<string, string>> datas = CSVReader.Read(upgradeCSVBylevel);
        for (int i = 0; i < datas.Count; i++)
        {
            UpgradeData upgradeData = new UpgradeData();
            upgradeData.upgradeName = datas[i]["Upgrade Type"];
            upgradeData.upgradeType = (UpgradeType)System.Enum.Parse(typeof(UpgradeType), datas[i]["Upgrade Type"]);
            upgradeData.foodID = (FoodID)System.Enum.Parse(typeof(FoodID), datas[i]["Food ID"]);
            upgradeData.refAmount = float.Parse(datas[i]["Amount"]);
            upgradeData.refID = int.Parse(datas[i]["Ref ID"]);
            upgradeData.upgradeID = int.Parse(datas[i]["Upgrade ID"]);
            upgradeData.priceUpgrade = new BigNumber();
            upgradeData.priceUpgrade.value = double.Parse(datas[i]["PriceValue"]);
            upgradeData.priceUpgrade.exp = int.Parse(datas[i]["PriceEXP"]);
            Sprite newSprite = spriteDataConfig.GetSpriteUpgradeDataByLevelByName(datas[i]["Spire Icon"]);
            if (newSprite != null)
                upgradeData.sprIcon = newSprite;
            Sprite newBootSprite = spriteDataConfig.GetSpriteUpgradeDataByLevelByName(datas[i]["SpireBootType"]);
            if (newBootSprite != null)
                upgradeData.sprIconBootType = newBootSprite;
            upgradeDatas.Add(upgradeData);
        }
    }
#endif
}

[System.Serializable]
public class UpgradeFoodCount
{
    public FoodID foodID;
    public int upgradeCount;
}

