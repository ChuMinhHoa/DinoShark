using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[System.Serializable]
public class DailyRewardManager : SaveDataBase
{
    public int dayReceived;
    DailyDataconfig dailyDataconfig;
    public bool endDaily = false;
    DailyReward dailyReward;
    public string lastDay;
    public int dayPassed;
    PanelDailyReward panelDailyReward;
    public override void LoadData()
    {
        SetStringSave("DailyReward");
        string jsonData = GetJsonData();
        if (!string.IsNullOrEmpty(jsonData))
        {
            DailyRewardManager dataSave = JsonUtility.FromJson<DailyRewardManager>(jsonData);
            dayReceived = dataSave.dayReceived;
            lastDay = dataSave.lastDay;
            dayPassed = dataSave.dayPassed;
            CanculateDateTime();
        }
        else
        {
            dayPassed = 1;
            dayReceived = 0;
            lastDay = DateTime.Now.ToString();
            UIManager.instance.panelTotal.ActiveBtnDaily(true);
            IsMarkChangeData();
            SaveData();
        }
        UIManager.instance.panelTotal.SetActiveNoticeDailyReward(CanReceiveDailyReward());
        dailyDataconfig = ProfileManager.Instance.dataConfig.dailyDataconfig;
    }

    void CanculateDateTime() {
        if (string.IsNullOrEmpty(lastDay))
        {
            dayPassed = 1;
            dayReceived = 0;
            lastDay = DateTime.Now.ToString();
            UIManager.instance.panelTotal.ActiveBtnDaily(true);
            IsMarkChangeData();
            SaveData();
        }
        else {
            int totalDays = (int)DateTime.Now.Subtract(DateTime.Parse(lastDay)).TotalDays;
            if (totalDays >= 1)
            {
                dayPassed += 1;
                lastDay = DateTime.Parse(lastDay).AddDays(totalDays).ToString();
            }
            endDaily = dayReceived >= ProfileManager.Instance.dataConfig.dailyDataconfig.dailyRewards.Count;
            UIManager.instance.panelTotal.ActiveBtnDaily(!endDaily);
            IsMarkChangeData();
            SaveData();
        }
    }


    public void GetReward(int day) {
        IsMarkChangeData();
        dailyReward = dailyDataconfig.GetDailyRewardByDay(day);
        dayReceived++;
        if (dailyReward == null)
        {
            Debug.Log("Daily data null");
            return;
        }

        if (dailyReward.itemRewards.Count > 0)
            GameManager.Instance.ClaimItemReward(dailyReward.itemRewards);

        if (dailyReward.itemEquips.Count > 0)
            GameManager.Instance.ClaimItemEquip(dailyReward.itemEquips);

        ProfileManager.Instance.playerData.SaveData();
        endDaily = dayReceived >= ProfileManager.Instance.dataConfig.dailyDataconfig.dailyRewards.Count;
        UIManager.instance.panelTotal.ActiveBtnDaily(!endDaily);
        if (dayReceived % 7 == 0 && !endDaily)
        {
            if (panelDailyReward == null)
                panelDailyReward = UIManager.instance.GetPanel(UIPanelType.PanelDailyReward).GetComponent<PanelDailyReward>();
            panelDailyReward.InitData();
        }
        UIManager.instance.panelTotal.SetActiveNoticeDailyReward(CanReceiveDailyReward());
    }
    DateTime theNextDay;
    public string GetTimeRemain() {
        theNextDay = DateTime.Parse(lastDay);
        theNextDay = theNextDay.AddDays(1);
        return TimeUtil.RemainTimeToString3((float)theNextDay.Subtract(DateTime.Now).TotalSeconds);
    }

    public bool CanReceiveDailyReward() {
        return dayReceived < dayPassed && !endDaily;
    }
}
