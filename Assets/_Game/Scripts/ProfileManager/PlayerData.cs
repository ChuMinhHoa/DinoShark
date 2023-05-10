using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SDK;

[System.Serializable]
public class DataSaveByLevel {
    public MenuSave menuSave = new MenuSave();
    public ResourceSave resourceSave = new ResourceSave();
    public UpgradeSave upgradeDataSave = new UpgradeSave();
}

[System.Serializable]
public class PlayerData
{
    string SETUPDONE = "setupdone";
    public string setupVersion;
    [Header("=====Global Save=====")]
    public GlobalResourceSave globalResourceSave = new GlobalResourceSave();
    public MerchandiseSave merchandiseSave = new MerchandiseSave();
    public BoostSave boostSave = new BoostSave();
    public OutfitSave outfitSave = new OutfitSave();
    public TutorialSave tutorialSave = new TutorialSave();
    public InviteSave inviteSave = new InviteSave();
    public DailyRewardManager dailyRewardManager = new DailyRewardManager();

    [Header("=====Level Save=====")]
    public DataSaveByLevel dataLevel1;
    public DataSaveByLevel dataLevel2;
    public DataSaveByLevel dataLevel3;
    public DataSaveByLevel dataLevel4;
    public DataSaveByLevel dataLevel5;
    public DataSaveByLevel dataLevel6;
    public DataSaveByLevel dataLevel7;

    public int currentLevel = 0;
    bool doneInit = false;

    public MenuSave GetMenuSave() {
        switch (currentLevel)
        {
            case 0:
                return dataLevel1.menuSave;
            case 1:
                return dataLevel2.menuSave;
            case 2:
                return dataLevel3.menuSave;
            case 3:
                return dataLevel4.menuSave;
            case 4:
                return dataLevel5.menuSave;
            case 5:
                return dataLevel6.menuSave;
            case 6:
                return dataLevel7.menuSave;
            default:
                return dataLevel1.menuSave;
        }
    }

    public ResourceSave GetResource()
    {
        switch (currentLevel)
        {
            case 0:
                return dataLevel1.resourceSave;
            case 1:
                return dataLevel2.resourceSave;
            case 2:
                return dataLevel3.resourceSave;
            case 3:
                return dataLevel4.resourceSave;
            case 4:
                return dataLevel5.resourceSave;
            case 5:
                return dataLevel6.resourceSave;
            case 6:
                return dataLevel7.resourceSave;
            default:
                return dataLevel1.resourceSave;
        }
    }

    public UpgradeSave GetUpgradeSave()
    {
        switch (currentLevel)
        {
            case 0:
                return dataLevel1.upgradeDataSave;
            case 1:
                return dataLevel2.upgradeDataSave;
            case 2:
                return dataLevel3.upgradeDataSave;
            case 3:
                return dataLevel4.upgradeDataSave;
            case 4:
                return dataLevel5.upgradeDataSave;
            case 5:
                return dataLevel6.upgradeDataSave;
            case 6:
                return dataLevel7.upgradeDataSave;
            default:
                return dataLevel1.upgradeDataSave;
        }
    }

    public TutorialSave GetTutorialSave() {
        return tutorialSave;
    }

    public InviteSave GetInviteSave()
    {
        return inviteSave;
    }

    public void LoadData() {
        //PlayerPrefs.DeleteAll();
        Debug.Log("Load data");
        merchandiseSave.LoadData();
        globalResourceSave.LoadData();
        boostSave.LoadData();
        outfitSave.LoadData();
        dailyRewardManager.LoadData();
        GetTutorialSave().LoadData();
        GetInviteSave().LoadData();
        TutorialManager.Instance.InitData();
        LoadSceneManager.Instance.AddTotalProgress(40);
        SoundManager.instance.PlayAudio();
        
    }

    public void CheckSetup()
    {
        string s = PlayerPrefs.GetString(SETUPDONE);
        if (!string.IsNullOrEmpty(s))
        {
            ABIFirebaseManager.Instance.SetUserProperty("Install_Version", Application.version);
            setupVersion = Application.version;
            Debug.Log(setupVersion);
            PlayerPrefs.SetString(SETUPDONE, setupVersion.ToString());
        }
        
    }

    public void LoadDataLevel() {
        doneInit = false;
        Debug.Log("Load data level: " + currentLevel);
        GetMenuSave().LoadData(currentLevel);
        GetResource().LoadData(currentLevel);
        GetUpgradeSave().LoadData(currentLevel);
        LoadSceneManager.Instance.AddTotalProgress(30);
        GameManager.Instance.InitDataToAsset();
        LoadSceneManager.Instance.ILoadDataDone();
        InviteHelperManager.Instance.InitData();
        doneInit = true;
        EventManager.TriggerEvent(EventName.UpdateTotalPanel.ToString());
    }

    public void SetCurrentLevel() {
        currentLevel = GetCurrentLevel();
    }

    public int GetCurrentLevel() {
        return PlayerPrefs.GetInt("PLayerLevel");
    }

    public void SaveCurrentLevel(int level) 
    {
        ABIAnalyticsManager.Instance.TrackPassMap(currentLevel);
        currentLevel = level;
        PlayerPrefs.SetInt("PLayerLevel", level);
    }

    public void SaveData()
    {
        GetMenuSave().SaveData(currentLevel);
        GetResource().SaveData(currentLevel);
        GetUpgradeSave().SaveData(currentLevel);
        GetTutorialSave().SaveData();
        GetInviteSave().SaveData();
        merchandiseSave.SaveData();
        globalResourceSave.SaveData();
        boostSave.SaveData();
        outfitSave.SaveData();
        dailyRewardManager.SaveData();
    }

    public void SaveMenuData(FoodBase foodBase) {
        GetMenuSave().GetDataFoodSave().SaveFoodData(foodBase);
        GetMenuSave().IsMarkChangeData();
        SaveData();
    }

    public DataFoodSave GetDataFoodSave() {
        return GetMenuSave().GetDataFoodSave();
    }

    public DataFood GetDataFood(FoodID foodID) {
        return GetMenuSave().GetDataFood(foodID);
    }

    public void AddMoney(BigNumber value) {
        if (!LoadSceneManager.Instance.isLoadDataDone)
            return;
        GetResource().AddMoney(value);
        GetResource().IsMarkChangeData();
        GetResource().SaveData(currentLevel);
    }

    public void ConsumeMoney(BigNumber value) {
        GetResource().Consume(value);
        GetResource().IsMarkChangeData();
        GetResource().SaveData(currentLevel);
    }

    public bool IsUpgraded(int upgradeID) {
        return GetUpgradeSave().IsUpgraded(upgradeID);
    }

    public void OnUpgrade(UpgradeData upgradeData) {
        GetUpgradeSave().OnUpgrade(upgradeData.upgradeID);
        GetUpgradeSave().IsMarkChangeData();
        GetRewardOnUpgradeData(upgradeData);
        globalResourceSave.AddDeposit();
        SaveData();
    }
    public void GetRewardOnUpgradeData(UpgradeData upgradeData, bool firstLoad = false) {
        switch (upgradeData.upgradeType)
        {
            case UpgradeType.AddCheff:
                GetUpgradeSave().AddStaff(upgradeData.refAmount);
                if (firstLoad)
                {
                    if (!tutorialSave.IsDoneTutorial(TutorialType.UnlockNewStaff))
                    { 
                        tutorialSave.AddTutorialDone(TutorialType.UnlockNewStaff);
                        SaveData();
                    }
                }
                if (GameManager.Instance && doneInit)
                {
                    GameManager.Instance.kitchenManager.SpawnNewStaff(false);
                    GameManager.Instance.kitchenManager.UpdateStaffSpeed();
                }
                break;
            case UpgradeType.AddCustomer:
                GetUpgradeSave().AddCustomer(upgradeData.refAmount);
                if (GameManager.Instance && doneInit)
                {
                    GameManager.Instance.lobbyManager.CallCustomer((int)(upgradeData.refAmount));
                }
                break;
            case UpgradeType.ReduceTime:
                FoodController foodController;
                if (GameManager.Instance && doneInit)
                {
                    //Debug.Log(upgradeData.foodID);
                    foodController = GameManager.Instance.kitchenManager.GetFoodController(upgradeData.foodID);
                    foodController.ReduceTime(upgradeData.refAmount);
                }
                GetMenuSave().dataFoodSave.ReduceTime(upgradeData.foodID, upgradeData.refAmount);
                break;
            case UpgradeType.IncreasePrice:
                if (GameManager.Instance && doneInit)
                {
                    //Debug.Log(upgradeData.foodID);
                    foodController = GameManager.Instance.kitchenManager.GetFoodController(upgradeData.foodID);
                    foodController.BootProfit(upgradeData.refAmount);
                }
                GetMenuSave().dataFoodSave.BootProfit(upgradeData.foodID, upgradeData.refAmount);
                break;
            case UpgradeType.IncreaseSpeedCheff:
                GetUpgradeSave().IncreaseStaffSpeed(upgradeData.refAmount);
                if (GameManager.Instance && doneInit)
                {
                    GameManager.Instance.kitchenManager.UpdateStaffSpeed();
                }
                break;
            case UpgradeType.AddTable:
                GetUpgradeSave().UnlockTable(upgradeData.refID);
                GetUpgradeSave().AddCustomer(upgradeData.refAmount);
                if (GameManager.Instance && doneInit)
                {
                    GameManager.Instance.lobbyManager.BuildTable(upgradeData.refID);
                }
                break;
            case UpgradeType.AddTotalBoot:
                GetResource().AddTotalBoot(upgradeData.refAmount);
                break;
            case UpgradeType.AddFloor:
                GetUpgradeSave().UnlockFloor((int)upgradeData.refAmount);
                GetUpgradeSave().UnlockTable(upgradeData.refID);
                
                if (firstLoad)
                {
                    GameManager.Instance.floorManager.AddCustomerToFirstTable((int)upgradeData.refAmount);
                    GetUpgradeSave().AddWaiter(1);
                    break;
                }
                GameManager.Instance.floorManager.UnlockFloor((int)upgradeData.refAmount, upgradeData.refID);
                break;
            case UpgradeType.AddWaiter:
                GetUpgradeSave().AddWaiter(upgradeData.refAmount);
                if (GameManager.Instance && doneInit)
                {
                    GameManager.Instance.lobbyManager.SpawnNewWaitor();
                }
                break;
            case UpgradeType.IncreaseSpeedWaiter:
                GetUpgradeSave().IncreaseWaiterSpeed(upgradeData.refAmount);
                if (GameManager.Instance && doneInit)
                {
                    GameManager.Instance.lobbyManager.UpdateWaiterSpeed();
                }
                break;
            default:
                break;
        }
        GetUpgradeSave().DataOnUpgrade(upgradeData);
    }

    public void Update()
    {
        boostSave.Update();
        globalResourceSave.Update();
        GetMenuSave().Update();
        GetInviteSave().Update();
    }
    float totalUpgrade;
    float currentProcess;
    public float GetUpgradeFoodProcess() {
        totalUpgrade = 0;
        currentProcess = GetDataFoodSave().GetTotalLevelUpgraded();
        //foreach (FoodID foodID in Enum.GetValues(typeof(FoodID)))
        //    totalUpgrade += ProfileManager.Instance.dataConfig.GetMenuDataByLevel().GetFoodData(foodID).GetLevelMax();
        totalUpgrade += ProfileManager.Instance.dataConfig.GetMenuDataByLevel().GetAllFoodMaxLevel();
        return currentProcess / totalUpgrade;
    }
}
