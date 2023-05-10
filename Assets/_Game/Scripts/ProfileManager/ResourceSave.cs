using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class ResourceSave : SaveDataBase
{
    public BigNumber totalMoney;
    public string lastTime;
    bool getOfflineRev = false;
    float totalBoot = 1;
    public override void LoadData(int level)
    {
        SetStringSave("ResourceSave_");
        string jsonData = GetJsonData(level);
        if (!string.IsNullOrEmpty(jsonData))
        {
            ResourceSave dataSave = JsonUtility.FromJson<ResourceSave>(jsonData);
            totalMoney = dataSave.totalMoney;
            lastTime = dataSave.lastTime;
            getOfflineRev = false;
        }
        else
        {
            //totalMoney.Add((BigNumber)ProfileManager.Instance.dataConfig.menuDataConfig.GetFirstMoney(level));
            totalMoney.Add(ProfileManager.Instance.dataConfig.menuDataConfig.foodDefaultProperties[0].unlockPrice);
            IsMarkChangeData();
            SaveData(level);
        }
        EventManager.TriggerEvent(EventName.ChangeMoney.ToString());
    }

    public override void SaveData(int level)
    {
        base.SaveData(level);
    }

    public void SaveLastTime()
    {
        lastTime = DateTime.Now.ToString();
    }

    public BigNumber GetMoney() { return totalMoney; }

    public float GetTotalBoot() { return totalBoot; }
    
    public void AddMoney(BigNumber value) {
        totalMoney.Add(value);
        if(getOfflineRev)
            SaveLastTime();
        EventManager.TriggerEvent(EventName.UpdateTotalPanel.ToString());
        EventManager.TriggerEvent(EventName.ChangeMoney.ToString());
        SoundManager.instance.PlaySoundEffect(SoundID.MAIN_MONEY);
    }
    public void Consume(BigNumber value)
    {
        totalMoney.Substract(value);
        if (getOfflineRev)
            SaveLastTime();
        EventManager.TriggerEvent(EventName.UpdateTotalPanel.ToString());
        EventManager.TriggerEvent(EventName.ChangeMoney.ToString());
    }

    public void GetOfflineRev()
    {
        getOfflineRev = true;
        SaveOffline();
    }
    public void AddTotalBoot(float boot) { 
        totalBoot *= boot;
        EventManager.TriggerEvent(EventName.ChangeBoostProfit.ToString());
    }

    public bool IsHaveEnoughMoney(BigNumber price) {
        return totalMoney.IsBigger(price);
    }

    public void SaveOffline()
    {
        SaveLastTime();
        IsMarkChangeData();
        ProfileManager.Instance.playerData.SaveData();
    }

    public string GetLastOffline()
    {
        return lastTime;
    }
}
