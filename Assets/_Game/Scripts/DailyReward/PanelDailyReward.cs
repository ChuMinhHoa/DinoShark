using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelDailyReward : UIPanel
{
    [SerializeField] Button btnExit;
    public override void Awake()
    {
        panelType = UIPanelType.PanelDailyReward;
        base.Awake();
    }

    private void Start()
    {
        dailyRewardManager = ProfileManager.Instance.playerData.dailyRewardManager;
        InitData();
        btnExit.onClick.AddListener(OnExit);
    }

    public List<DailyRewardSlot> dailyRewardSlots = new List<DailyRewardSlot>();
    DailyDataconfig dailyDataconfig;
    int dayPassed;
    int dayReceived;
    DailyRewardManager dailyRewardManager;
    int indexStart;
    public void InitData()
    {
        dailyDataconfig = ProfileManager.Instance.dataConfig.dailyDataconfig;
        dayPassed = dailyRewardManager.dayPassed;
        dayReceived = dailyRewardManager.dayReceived;

        indexStart = (int)(dayReceived / 7);

        for (int i = indexStart * 7; i < indexStart * 7 + 7; i++)
        {
            dailyRewardSlots[i % 7].InitData(dailyDataconfig.GetDailyRewardByDay(i+1));
            if (i < dayReceived)
                dailyRewardSlots[i % 7].ReceivedMode();
            else if (i >= dayReceived && i < dayPassed)
                dailyRewardSlots[i % 7].CanReceiveMode();
            else if (i > dayPassed)
                dailyRewardSlots[i % 7].LockMode(false);
            else if (i == dayPassed)
                dailyRewardSlots[i % 7].LockMode(true);
        }
    }

    void OnExit() {
        UIManager.instance.ClosePanelDailyReward();
    }
}
