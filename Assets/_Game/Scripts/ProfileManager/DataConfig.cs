using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class DataConfig
{
    public float baseOfflineTime = 3;
    public UpgradeDataConfig upgradeDataConfig;
    public MenuDataConfig menuDataConfig;
    public MerchantDataConfig merchantDataConfig;
    public SkinDataConfig skinDataConfig;
    public OutfitDataConfig outfitDataConfig;
    public OutfitBoxDataConfig outfitBoxDataConfig;
    public ShopDataConfig shopDataConfig;
    public LevelDataConfig levelDataConfig;
    public TutorialDataConfig tutorialDataConfig;
    public SpriteDataConfig spriteDatatConfig;
    public EggDataconfig eggMeshDataConfig;
    public DailyDataconfig dailyDataconfig;
    PlayerData playerData;
    public void Start(PlayerData playerData) {
        this.playerData = playerData;
    }
    public MenuDataByLevel GetMenuDataByLevel()
    {
        return menuDataConfig.GetMenuDataByLevel(playerData.currentLevel);
    }
    public LevelData GetLevelData(int level)
    {
        return levelDataConfig.GetLevelData(level);
    }

}
