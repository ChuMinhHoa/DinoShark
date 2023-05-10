using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SDK;
public interface IMachineController
{
    public FoodID GetFoodId();
    public string GetFoodName();
    public int GetFoodLevelMax();
    public int GetFoodLevel();
    public float GetTimeMakeFood();
    public BigNumber GetFoodPriceUpgrade();
    public void OpenControlerInfo();
    public void OnUpgradeFood(UnityAction actionAddGem = null);

    public BigNumber GetFoodProfit();
    public BigNumber GetFoodBaseProfit();
    public Vector3 GetPointStay(int index);
    public Machine GetMachineAble();

    public int GetLastLevelStar();
    public int GetNextLevelStar();
    public Sprite GetIconSprite();

    public void BootProfit(float boot = 1);
    public void ReduceTime(float value);
}
public class FoodController : MonoBehaviour, IMachineController
{
    public FoodBase foodBase;
    BigNumber currentProfit = new BigNumber();
    public FoodDataConfig foodDataAsset;
    public FoodDefaultProperty defaultProperty;
    int countMachineActive;
    ResourceSave resourceSave;
    [SerializeField] GameObject upgradeAlert;
    [SerializeField] AlertClicker alertClicker;
    [SerializeField] List<int> levelMachineUnlock;
    [SerializeField] GameObject foodObj;
    [SerializeField] Animator animator;
    KitchenManager kitchenManager;
    
    public virtual void InitData(KitchenManager manager) {
        kitchenManager = manager;
        foodDataAsset = ProfileManager.Instance.dataConfig.GetMenuDataByLevel().GetFoodData(foodBase.foodID);
        LoadDataMachineUnlock();
        LoadMachineUnlock();
        if (!GameManager.Instance.IsUnLockFood(foodBase.foodID))
        {
            if(foodObj)
                foodObj.SetActive(false);
            OffMachine();
            //if(ProfileManager.Instance.dataConfig.GetMenuDataByLevel().IsFirstFood(foodBase.foodID) && ProfileManager.Instance.playerData.currentLevel >= 2)
            //{
            //    OnUpgradeFood();
            //}
            ProfileManager.Instance.playerData.SaveMenuData(foodBase);
        }
        OnLoadMachine();
        resourceSave = ProfileManager.Instance.playerData.GetResource();
        CalculateNewProfit();
        EventManager.AddListener(EventName.ChangeBoostProfit.ToString(), CalculateNewProfit);
        if (alertClicker)
            alertClicker.SetControler(this);
    }

    public void SetDefaultProperty(FoodDefaultProperty inProperty)
    {
        defaultProperty = inProperty;
    }

    void LoadDataMachineUnlock() {
        levelMachineUnlock = ProfileManager.Instance.dataConfig.menuDataConfig.machineUnlock;
    }

    void LoadMachineUnlock()
    {
        for (int i = 0; i < foodBase.machines.Count; i++)
        {
            foodBase.machines[i].SetLevelActive(levelMachineUnlock[i]);
            foodBase.machines[i].SetMachineController(this);
        }              
    }
    public void OnLoadMachine()
    {
        LoadData(ProfileManager.Instance.playerData.GetDataFood(foodBase.foodID));
        foodBase.totalBoot = ProfileManager.Instance.playerData.GetMenuSave().GetDataFood(foodBase.foodID).GetTotalBoost();
        foodBase.timeBoost = ProfileManager.Instance.playerData.GetMenuSave().GetDataFood(foodBase.foodID).GetReduceTime();
    }
    public void OffMachine() { foodBase.OffMachines(); }
    void LoadData(DataFood dataFood)
    {
        foodBase.level = dataFood.level;
        foodBase.profit = dataFood.profit;
        foodBase.able = dataFood.able;
        OffMachine();
        countMachineActive = GetCountMachine();
        for (int i = 0; i < foodBase.machines.Count; i++)
        {
            if (i < countMachineActive)
                foodBase.machines[i].ChangeMachineStatus(MachineStatus.CanUsing);
            else foodBase.machines[i].ChangeMachineStatus(MachineStatus.Lock);
        }
    }
    int GetCountMachine() {
        int countReturn = 0;
        for (int i = 0; i < foodBase.machines.Count; i++)
        {
            if (foodBase.level >= foodBase.machines[i].GetLevelActive())
                countReturn++;
        }
        return countReturn;
    }

    public void CheckIsEnoughMoney()
    {
        if (resourceSave.GetMoney().IsBigger(GetFoodPriceUpgrade()) && foodBase.level < GetFoodLevelMax())
        {
            upgradeAlert.SetActive(true);
        }
        else
        {
            upgradeAlert.SetActive(false);
        }
    }
    public void OpenControlerInfo()
    {
        //Debug.Log("ShowPanel");
        animator.SetTrigger("OnClick");
        UIManager.instance.ShowPanelMachineInfor(this);
    }
    public void OnUpgradeFood(UnityAction actionAddGem = null)
    {
        int nextStarLevel = GetNextLevelStar();
        foodBase.level++; 
        foodBase.level = Mathf.Clamp(foodBase.level, 0, ProfileManager.Instance.dataConfig.GetMenuDataByLevel().GetLevelMax());
        if (countMachineActive < GetCountMachine())
        {
            countMachineActive = GetCountMachine();
            foodBase.machines[countMachineActive - 1].ChangeMachineStatus(MachineStatus.Box);
        }
        UpdateProperty();
        foodBase.able = true;
        CalculateNewProfit();
        if (foodBase.level == nextStarLevel)
        {
            ProfileManager.Instance.playerData.globalResourceSave.AddGem(2);
            actionAddGem();
            ABIAnalyticsManager.Instance.TrackUpgradeItem(foodBase.foodID, GetMilestone());
        }
        ProfileManager.Instance.playerData.SaveMenuData(foodBase);
        if (foodBase.level == 1)
        {
            ABIAnalyticsManager.Instance.TrackUpgradeItem(foodBase.foodID, GetMilestone());
            if (foodObj)
                foodObj.SetActive(true);
            ProfileManager.Instance.playerData.GetMenuSave().dataFoodSave.CanculateFoodRate();
            if (ProfileManager.Instance.playerData.currentLevel > 1)
            {
                GameManager.Instance.lobbyManager.InitCustomer();
            }
            GameManager.Instance.lobbyManager.CustomersShowOrder();
        }
        EventManager.TriggerEvent(EventName.UpdateTotalPanel.ToString());
        kitchenManager.CheckFoodController();
    }

    void UpdateProperty() {
        if (foodBase.profit.exp==0 && foodBase.profit.value == 0)
        {
            //foodBase.profit = new BigNumber(defaultProperty.firstProfit);
            foodBase.profit.exp = defaultProperty.firstProfit.exp;
            foodBase.profit.value = defaultProperty.firstProfit.value;
            return;
        }
        if(ProfileManager.Instance.dataConfig.menuDataConfig.IsUnlockStar(foodBase.level))
        {
            // If meet new star unlock then double profit
            foodBase.profit.Multiply(2f);
        }
        BigNumber curProfitAdd = foodBase.profit + 1;
        BigNumber curProfitMul = foodBase.profit * 1.08f;
        if (curProfitMul.IsBigger(curProfitAdd))
        {
            foodBase.profit.Multiply(1.08f);
        }
        else
        {

            foodBase.profit.Add(1);
        }
    }

    public int GetFoodLevelMax()
    {
        return ProfileManager.Instance.dataConfig.GetMenuDataByLevel().GetLevelMax();
    }

    public int GetFoodLevel()
    {
        return foodBase.level;
    }

    public Vector3 GetPointStay(int index)
    {
        return foodBase.machines[index].transform.position;
    }

    public Machine GetMachineAble()
    {
        return foodBase.GetMachineFree(countMachineActive);
    }

    public BigNumber GetFoodPriceUpgrade()
    {
        return defaultProperty.GetPriceUpgrade(foodBase.level);
    }

    public string GetFoodName()
    {
        return ProfileManager.Instance.dataConfig.menuDataConfig.GetFoodName(foodBase.foodID);
    }

    public float GetTimeMakeFood()
    {
        //float timeMake = defaultProperty.timeMake;
        //timeMake *= (1 - foodBase.timeReduce);
        return defaultProperty.timeMake / foodBase.timeBoost;
    }


    public int GetLastLevelStar()
    {
        //return foodDataAsset.GetLastLevelStar(foodBase.level);
        return ProfileManager.Instance.dataConfig.menuDataConfig.GetLastLevelStar(foodBase.level);
    }

    public int GetNextLevelStar()
    {
        //return foodDataAsset.GetNextLevelStar(foodBase.level);
        return ProfileManager.Instance.dataConfig.menuDataConfig.GetNextLevelStar(foodBase.level);
    }

    public int GetMilestone()
    {
        return ProfileManager.Instance.dataConfig.menuDataConfig.GetMilestone(foodBase.level);
    }

    public void CalculateNewProfit()
    {
        float globalBoot = resourceSave.GetTotalBoot();
        float additionalProfit = ProfileManager.Instance.playerData.boostSave.GetBoostEffect();
        float outfitBoot = (100 + ProfileManager.Instance.playerData.outfitSave.GetTotalOutfitBoot()) / 100;
        float x2Revenue = ProfileManager.Instance.playerData.globalResourceSave.GetX2Revenue();
        
        BigNumber currentBoot = globalBoot * foodBase.totalBoot * additionalProfit * outfitBoot * x2Revenue;
        currentProfit = foodBase.profit * currentBoot;
    }

    public BigNumber GetFoodProfit() {
        return currentProfit * 0.8f;
    }
    public BigNumber GetFoodBaseProfit()
    {
        return foodBase.profit * foodBase.totalBoot * 0.8f;
    }

    public void BootProfit(float boot = 1) {
        foodBase.totalBoot *= boot;
    }

    public void ReduceTime(float value) {
        foodBase.timeBoost *= 2;
        //foodBase.timeReduce = Mathf.Clamp01(value);
    }

    public Sprite GetIconSprite()
    {
        return foodDataAsset.GetIcon();
    }

    public FoodID GetFoodId()
    {
        return foodBase.foodID;
    }
}
