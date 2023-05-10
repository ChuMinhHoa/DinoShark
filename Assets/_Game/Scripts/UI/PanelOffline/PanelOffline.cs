using SDK;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelOffline : UIPanel
{
    [SerializeField] Text profitTxt;
    [SerializeField] Text offlineBuffTxt;
    [SerializeField] Text offlineMaxTimeTxt;
    [SerializeField] Text offlinedurationTxt;
    [SerializeField] Button X2Btn;
    [SerializeField] Button exitBtn;
    [SerializeField] Button Bg;
    [SerializeField] Slider slider;

    BigNumber offlineProfit;
    public override void Awake()
    {
        panelType = UIPanelType.PanelOffline;
        base.Awake();
        X2Btn.onClick.AddListener(WatchX2Ads);
        exitBtn.onClick.AddListener(ClosePanel);
        //Bg.onClick.AddListener(ClosePanel);
    }

    public void SetUp(TimeSpan span)
    {
        offlineProfit = ProfileManager.Instance.playerData.GetMenuSave().dataFoodSave.GetAvgProfit();
        //Debug.Log(offlineProfit.ToString());
        float serveTime = GameManager.Instance.GetFoodServedTime();
        offlineProfit.Multiply((double)(span.TotalSeconds));
        offlineProfit.Divide((double)serveTime);
        profitTxt.text = offlineProfit.ToString();
        offlineBuffTxt.text = "x" + ProfileManager.Instance.playerData.merchandiseSave.GetOfflineRevenue().ToString();

        TimeSpan maxSpan = TimeSpan.FromSeconds((double)(GetMaxOfflineTimeToSec()));
        offlineMaxTimeTxt.text = "";
        if (maxSpan.Hours > 1)
        {
            offlineMaxTimeTxt.text += (maxSpan.Hours).ToString() + "h";
        }
        offlineMaxTimeTxt.text += (maxSpan.Minutes).ToString() + "m";

        offlinedurationTxt.text = "";
        if(span.TotalSeconds <= GetMaxOfflineTimeToSec())
        {
            if (span.Hours > 1)
            {
                offlinedurationTxt.text += (span.Hours).ToString() + "h";
            }
            offlinedurationTxt.text += (span.Minutes).ToString() + "m";
            offlinedurationTxt.text += (span.Seconds).ToString() + "s";
            slider.value = (float)(span.TotalSeconds) / GetMaxOfflineTimeToSec();
        }
        else
        {
            offlinedurationTxt.text = offlineMaxTimeTxt.text;
            slider.value = 1;
        }
    }

    float GetMaxOfflineTimeToSec()
    {
        return GetMaxOfflineTimeToHour() * 3600f;
    }

    float GetMaxOfflineTimeToHour() 
    {
        return ProfileManager.Instance.dataConfig.baseOfflineTime + ProfileManager.Instance.playerData.merchandiseSave.GetOfflineTime();
    }

    void WatchX2Ads()
    {
        SoundManager.instance.PlaySoundEffect(SoundID.UI_BUTTON);
        if (ProfileManager.Instance.playerData.globalResourceSave.removeAds)
        {
            GetX2Profit();
            return;
        }
        AdsManager.Instance.ShowRewardVideo(WatchVideoRewardType.X2_Offline.ToString(), GetX2Profit);
    }

    void GetX2Profit()
    {
        ProfileManager.Instance.playerData.AddMoney(offlineProfit);
        UIManager.instance.ClosePanelOffline();
    }
    void ClosePanel()
    {
        ProfileManager.Instance.playerData.GetResource().GetOfflineRev();
        SoundManager.instance.PlaySoundEffect(SoundID.UI_BUTTON);
        ProfileManager.Instance.playerData.AddMoney(offlineProfit);
        if (openAndCloseAnim != null)
            openAndCloseAnim.OnClose(UIManager.instance.ClosePanelOffline);
        else UIManager.instance.ClosePanelOffline();
    }
}
