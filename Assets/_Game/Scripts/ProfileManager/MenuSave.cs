using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class MenuSave : SaveDataBase
{
    public DataFoodSave dataFoodSave = new DataFoodSave();
    [Header("Marketing Food")]
    public FoodID marketedFood;
    float baseMarketingTime = 5 * 60;
    float baseMarketingCoolDown = 10 * 60;
    public float foodMarketingTimeRemain;
    public float foodMarketingCoolDown;
    public string foodMarketingEnd;
    public string foodMarketingCoolDownEnd;
    public bool onWaitPromotion;
    public bool onPromotion;
    public override void LoadData(int level) {
        SetStringSave("MenuSave_");
        string jsonData = GetJsonData(level);
        if (!string.IsNullOrEmpty(jsonData))
        {
            MenuSave dataSave = JsonUtility.FromJson<MenuSave>(jsonData);
            dataFoodSave = dataSave.dataFoodSave;
            foodMarketingEnd = dataSave.foodMarketingEnd;
            foodMarketingCoolDownEnd = dataSave.foodMarketingCoolDownEnd;
            marketedFood = dataSave.marketedFood;
        }
        else {
            foodMarketingEnd = DateTime.Now.ToString();
            IsMarkChangeData();
            SaveData(level);
        }
        FoodMarketingRemainCalculate();
        dataFoodSave.CanculateFoodRate();
    }

    public DataFoodSave GetDataFoodSave()
    {
        return dataFoodSave;
    }

    public DataFood GetDataFood(FoodID foodID) {
        return dataFoodSave.GetDataFood(foodID);
    }

    int currentLevel;
    int maxLevelOfMap;
    public bool CanExpandNewMap() {
        currentLevel = ProfileManager.Instance.playerData.currentLevel;
        if (currentLevel == ProfileManager.Instance.dataConfig.levelDataConfig.levelDatas.Count)
            return false;
        maxLevelOfMap = ProfileManager.Instance.dataConfig.menuDataConfig.GetMenuDataByLevel(currentLevel).GetLevelMax();
        for (int i = 0; i < dataFoodSave.foods.Count; i++)
        {
            if (dataFoodSave.foods[i].level < maxLevelOfMap)
                return false;
        }
        return true;
    }
    void FoodMarketingRemainCalculate()
    {
        DateTime momment = DateTime.Now;
        if (!String.IsNullOrEmpty(foodMarketingEnd))
        {
            DateTime boostEndTime = DateTime.Parse(foodMarketingEnd);
            TimeSpan span = boostEndTime.Subtract(momment);
            if (span.TotalSeconds > 0)
            {
                foodMarketingTimeRemain = (float)(span.TotalSeconds);
                onPromotion = true;
            }
            else
            {
                foodMarketingTimeRemain = 0f;
                onPromotion = false;
            }
        }

        if (!String.IsNullOrEmpty(foodMarketingCoolDownEnd))
        {
            DateTime boostEndTime = DateTime.Parse(foodMarketingCoolDownEnd);
            TimeSpan span = boostEndTime.Subtract(momment);
            if (span.TotalSeconds > 0)
            {
                foodMarketingCoolDown = (float)(span.TotalSeconds);
                onWaitPromotion = true;
            }
            else
            {
                foodMarketingCoolDown = 0f;
                onWaitPromotion = false;
            }
        }

    }
    public void OnMarketingFood(FoodID foodID)
    {
        marketedFood = foodID;

        foodMarketingTimeRemain = baseMarketingTime;
        foodMarketingEnd = ((DateTime.Now).AddSeconds(baseMarketingTime)).ToString();

        foodMarketingCoolDown = baseMarketingCoolDown; 
        foodMarketingCoolDownEnd = ((DateTime.Now).AddSeconds(baseMarketingCoolDown)).ToString();

        IsMarkChangeData();
        dataFoodSave.BoostFoodRate(foodID);
        onWaitPromotion = true;
        ProfileManager.Instance.playerData.SaveData();
    }
    public bool CheckFoodMarketing()
    {
        return onPromotion;
    }
    public FoodID GetMarketedFood() { return marketedFood; }

    public void Update()
    {
        if (foodMarketingTimeRemain > 0)
        {
            foodMarketingTimeRemain -= Time.deltaTime;
            if (foodMarketingTimeRemain <= 0f)
            {
                dataFoodSave.CalculateNormalFoodRate();
            }
        }

        if (foodMarketingCoolDown>0)
        {
            foodMarketingCoolDown -= Time.deltaTime;
            if (foodMarketingCoolDown <= 0f)
                onWaitPromotion = false;
        }
    }
}
[System.Serializable]
public class DataFoodSave
{
    public List<DataFood> foods = new List<DataFood>();
    
    public DataFood GetDataFood(FoodID foodID) {
        for (int i = 0; i < foods.Count; i++)
        {
            if (foods[i].foodID == foodID)
                return foods[i];
        }
        return null;
    }
    public void SaveFoodData(FoodBase foodBase) {
        bool haveData = false;
        
        for (int i = 0; i < foods.Count; i++)
        {
            if (foods[i].foodID == foodBase.foodID)
            {
                foods[i].level = foodBase.level;
                foods[i].profit = foodBase.profit;
                foods[i].able = foodBase.able;
                haveData = true;
                break;
            }
        }

        if (!haveData)
        {
            DataFood menuSave = new DataFood();
            menuSave.level = foodBase.level;
            menuSave.foodID = foodBase.foodID;
            menuSave.profit = foodBase.profit;
            menuSave.able = false;
            foods.Add(menuSave);
        }
    }
    public void CanculateFoodRate()
    {
        if(ProfileManager.Instance.playerData.GetMenuSave().CheckFoodMarketing())
        {
            BoostFoodRate(ProfileManager.Instance.playerData.GetMenuSave().GetMarketedFood());
        }
        else
        {
            CalculateNormalFoodRate();
        }
    }
    public void CalculateNormalFoodRate()
    {
        // More expensive more order
        float totalRate = 0;
        for (int i = 0; i < foods.Count; i++)
        {
            totalRate += (int)(i / 3) + 1;
        }
        for (int i = 0; i < foods.Count; i++)
        {
            foods[i].SetFoodRate((100 * ((int)(i / 3) + 1)) / (foods.Count * totalRate));
        }
    }
    float boostRate = 2f;
    public void BoostFoodRate(FoodID foodID)
    {
        if(CheckHasFood(foodID))
        {
            float totalRate = 0;
            for (int i = 0; i < foods.Count; i++)
            {
                totalRate += (int)(i / 3) + 1;
            }
            totalRate += boostRate;
            for (int i = 0; i < foods.Count; i++)
            {
                if (foods[i].foodID == foodID)
                {
                    foods[i].SetFoodRate((100 * ((int)(i / 3) + 1 + boostRate)) / (foods.Count * totalRate));
                }
                else
                {
                    foods[i].SetFoodRate((100 * ((int)(i / 3) + 1)) / (foods.Count * totalRate));
                }
            }
        }
        else
        {
            CalculateNormalFoodRate();
        }
    }
    public bool CheckHasFood(FoodID foodID)
    {
        for (int i = 0; i < foods.Count; i++)
        {
            if (foods[i].foodID == foodID)
            {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// At the level start, customer call the first in menu (whether it's unlocked or not)
    /// Then just call the unlocked food;
    /// Expensive food called more offen
    /// </summary>
    /// <returns>FoodID</returns>
    public FoodID GetRandomFood()
    {
        float totalRate = 0;
        for (int i = 0; i < foods.Count; i++)
        {
            if(foods[i].able)
            {
                totalRate += foods[i].GetFoodRate();
            }
        }
        float rand = UnityEngine.Random.Range(0f, totalRate);
        float top = 0;
        for (int i = 0; i < foods.Count; i++)
        {
            if (foods[i].able)
            {
                top += foods[i].GetFoodRate();
                if (rand < top)
                {
                    return foods[i].foodID;
                }
            }
        }
        if(foods.Count > 0)
        {
            return foods[0].foodID;
        }
        return FoodID.None;
    }
    int totalUpgrade = 0;
    public int GetTotalLevelUpgraded() {
        totalUpgrade = 0;
        for (int i = 0; i < foods.Count; i++)
            totalUpgrade += foods[i].level;
        return totalUpgrade; 
    }
    public BigNumber GetAvgProfit()
    {
        int countMachine = 0;
        BigNumber avg = 0;
        for (int i = 0; i < foods.Count; i++)
        {           
            if(foods[i].profit > 0)
            {
                countMachine++;
                avg.Add(foods[i].profit * foods[i].GetTotalBoost());
            }  
        }
        // Average must multiply with boost 
        avg.Multiply((double)(ProfileManager.Instance.playerData.boostSave.GetBoostEffect()));
        avg.Divide((double)countMachine);
        return avg * 0.8f; 
    }

    public BigNumber GetAdsMoneyReward()
    {
        int countMachine = 0;
        BigNumber rewardMoney = GetAvgProfit();
        for (int i = 0; i < foods.Count; i++)
        {
            if (foods[i].profit > 0)
            {
                countMachine++;
            }
        }
        return rewardMoney * countMachine;
    }

    public void BootProfit(FoodID foodID, float boot = 1)
    {
        DataFood dataF = GetDataFood(foodID);
        dataF.AddTotalBoost(boot);
    }

    public void ReduceTime(FoodID foodID, float value)
    {
        DataFood dataF = GetDataFood(foodID);
        //dataF.GetFoodBase().timeReduce = Mathf.Clamp01(value);
        dataF.ReduceTime(value);
    }

    public bool CheckHaveMachineUnlock()
    {
        for (int i = 0; i < foods.Count; i++)
        {
            if (foods[i].able)
                return true;
        }
        return false;
    }
    public int GetToralFoodAble() {
        int count = 0;
        for (int i = 0; i < foods.Count; i++)
        {
            if (foods[i].able)
                count++;
        }
        return count;
    }
}
[System.Serializable]
public class DataFood
{
    public int level;
    public FoodID foodID;
    public BigNumber profit;
    public bool able;
    float totalBoot = 1;
    float timeReduce = 1;
    float foodRate = 0;

    public void AddTotalBoost(float value = 1)
    {
        totalBoot *= value;
        EventManager.TriggerEvent(EventName.ChangeBoostProfit.ToString());
    }
    public void ReduceTime(float value = 1)
    {
        timeReduce *= 2;
    }

    public float GetTotalBoost() { return totalBoot; }
    public float GetReduceTime() { return timeReduce; }

    public void SetFoodRate(float value) { foodRate = value; }
    public float GetFoodRate() { return foodRate; }
}

