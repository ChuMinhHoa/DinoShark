using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class UpgradeSave : SaveDataBase
{
    int staffCount;
    int customerCount;
    int waiterCount;
    float staffSpeed;
    float waiterSpeed;
    public List<int> tableUnlockID;
    public List<int> floorUnlockID;
    public List<int> upgradeIDs;
    List<UpgradeData> upgradeDatasRemain = new List<UpgradeData>();

    public bool IsUpgraded(int upgradeID)
    {
        for (int i = 0; i < upgradeIDs.Count; i++)
        {
            if (upgradeIDs[i] == upgradeID)
                return true;
        }
        return false;
    }
    public void OnUpgrade(int upgradeID) { 
        upgradeIDs.Add(upgradeID);
    }
    public override void LoadData(int level)
    {
        SetStringSave("UpgradeSave_");
        string jsonData = GetJsonData(level);
        if (!string.IsNullOrEmpty(jsonData))
        {
            UpgradeSave dataSave = JsonUtility.FromJson<UpgradeSave>(jsonData);
            upgradeIDs = dataSave.upgradeIDs;
            floorUnlockID = dataSave.floorUnlockID;
            staffCount = 1;
            customerCount = 1;
            staffSpeed = 1;
            waiterSpeed = 1;
        }
        else
        {
            staffCount = 1;
            waiterCount = 0;
            customerCount = 1;
            staffSpeed = 1;
            waiterSpeed = 1;
            IsMarkChangeData();
            SaveData(level);
        }
        LoadProperty();
    }
    void LoadProperty() {
        int level = ProfileManager.Instance.playerData.currentLevel;
        UpgradeDataByLevel upgradeDataLevel = ProfileManager.Instance.dataConfig.upgradeDataConfig.GetUpgradeDataByLevel(level);

        for (int i = 0; i < upgradeDataLevel.upgradeDatas.Count; i++)
        {
            //UpgradeData upgradeData = upgradeDataLevel.GetUpgradeDataByID(upgradeIDs[i]);
            //ProfileManager.Instance.playerData.GetRewardOnUpgradeData(upgradeData, true);
            if(upgradeIDs.Contains(upgradeDataLevel.upgradeDatas[i].upgradeID))
            {
                ProfileManager.Instance.playerData.GetRewardOnUpgradeData(upgradeDataLevel.upgradeDatas[i], true);
            }
            else
            {
                upgradeDatasRemain.Add(upgradeDataLevel.upgradeDatas[i]);
            }
        }
        EventManager.TriggerEvent(EventName.UpdateTotalPanel.ToString());
    }
    public void DataOnUpgrade(UpgradeData upgradeData)
    {
        upgradeDatasRemain.Remove(upgradeData);
    }
    public void AddStaff(float value) { 
        staffCount += (int)value;
        SaveData(ProfileManager.Instance.playerData.currentLevel);
    }
    public void AddCustomer(float value) { 
        customerCount += (int)value;
        SaveData(ProfileManager.Instance.playerData.currentLevel);
    }
    public void AddWaiter(float value) {
        waiterCount += (int)value;
        SaveData(ProfileManager.Instance.playerData.currentLevel);
    }
    public void UnlockTable(int tableID) {
        tableUnlockID.Add(tableID);
        SaveData(ProfileManager.Instance.playerData.currentLevel);
    }

    public void UnlockFloor(int floorID)
    {
        floorUnlockID.Add(floorID);
        SaveData(ProfileManager.Instance.playerData.currentLevel);
    }

    public void IncreaseStaffSpeed(float value)
    {
        staffSpeed *= 1.25f;
        SaveData(ProfileManager.Instance.playerData.currentLevel);
    } 
    
    public void IncreaseWaiterSpeed(float value)
    {
        waiterSpeed *= 1.25f;
        SaveData(ProfileManager.Instance.playerData.currentLevel);
    }
    public int GetTotalStaff() { return staffCount; }
    public int GetTotalWaiter() { return waiterCount; }
    public int GetTotalCustomer() { return customerCount; }
    public float GetUpgradeSpeed() { return staffSpeed;  }
    public float GetWaiterUpgradeSpeed() { return waiterSpeed;  }
    public bool IsTableUnlocked(int tableId = 0)
    {
        if (tableId == 0) return true;  // id 0 ==> Default table
        if (tableUnlockID.Contains(tableId)) return true;
        return false;
    }
    public bool IsFloorUnlocked(int areaID)
    {
        if (floorUnlockID.Contains(areaID)) return true;
        return false;
    }

    public bool CheckUpgradeAvailable()
    {
        if (upgradeDatasRemain.Count == 0)
            return false;
        return GameManager.Instance.IsHaveEnoughMoney(upgradeDatasRemain[0].priceUpgrade);
    }

    public UpgradeData GetUpgradeAvailable()
    {
        if (upgradeDatasRemain.Count == 0)
            return null;
        if (GameManager.Instance.IsHaveEnoughMoney(upgradeDatasRemain[0].priceUpgrade))
            return upgradeDatasRemain[0];
        return null;
    }

    public bool IsOutOfUpgrade(int total = 0)
    {
        if(total == 0)
        {
            total = ProfileManager.Instance.dataConfig.upgradeDataConfig.GetUpgradeDataByLevel(ProfileManager.Instance.playerData.currentLevel).upgradeDatas.Count;
        }
        if(total == upgradeIDs.Count)
        {
            return true;
        }
        return false;
    }

    public List<UpgradeData> GetRemainUpgrades()
    {
        return upgradeDatasRemain;
    }
}
