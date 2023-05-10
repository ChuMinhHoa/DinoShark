using SDK;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdsBoostBtn : MonoBehaviour
{
    [SerializeField] Button btnAdsBoost;
    [SerializeField] Text AdsBoostValue;
    string X = "X";
    [SerializeField] Text AdsBoostDuration;
    string ADD = "+";
    string MINUTE = "M";
    float adsBoostTimeRemain;
    TimeSpan spanAds;
    [SerializeField] Text AdsBoostRemain;
    [SerializeField] GameObject timeAdsObj;
    float x2BoostTimeRemain;
    TimeSpan spanX2;
    [SerializeField] Text x2BoostRemain;
    [SerializeField] GameObject timeX2Obj;
    float x5BoostTimeRemain;
    TimeSpan spanX5;
    [SerializeField] Text x5BoostRemain;
    [SerializeField] GameObject timeX5Obj;

    void Start()
    {
        btnAdsBoost.onClick.AddListener(AdsBoostClick);
    }

    void AdsBoostClick()
    {
        if(ProfileManager.Instance.playerData.globalResourceSave.removeAds)
        {
            X2ProfitSuccess();
            return;
        }
        AdsManager.Instance.ShowRewardVideo(WatchVideoRewardType.X2_Profit.ToString(), X2ProfitSuccess);
    }
    void X2ProfitSuccess()
    {
        adsBoostTimeRemain = ProfileManager.Instance.playerData.boostSave.AddAdsBoost();
        timeAdsObj.SetActive(true);
    }

    public void CheckAdsBoostStatus()
    {
        AdsBoostValue.text = X + ProfileManager.Instance.playerData.merchandiseSave.GetAdsBoostEffect().ToString();
        AdsBoostDuration.text = ADD + ProfileManager.Instance.playerData.merchandiseSave.GetAdsBoostDuration().ToString() + MINUTE;
    }

    public void CheckCurrentBoost()
    {
        adsBoostTimeRemain = ProfileManager.Instance.playerData.boostSave.GetAdsBoostRemain();
        if (adsBoostTimeRemain > 0)
        {
            timeAdsObj.SetActive(true);
        }
        else
        {
            timeAdsObj.SetActive(false);
        }
        x2BoostTimeRemain = ProfileManager.Instance.playerData.boostSave.x2BoostTimeRemain;
        if (x2BoostTimeRemain > 0)
        {
            timeX2Obj.SetActive(true);
        }
        else
        {
            timeX2Obj.SetActive(false);
        }
        x5BoostTimeRemain = ProfileManager.Instance.playerData.boostSave.x5BoostTimeRemain;
        if (x5BoostTimeRemain > 0)
        {
            timeX5Obj.SetActive(true);
        }
        else
        {
            timeX5Obj.SetActive(false);
        }
    }

    private void Update()
    {
        if (adsBoostTimeRemain > 0)
        {
            spanAds = TimeSpan.FromSeconds((double)adsBoostTimeRemain);
            AdsBoostRemain.text = spanAds.ToString(@"mm\:ss");
            adsBoostTimeRemain -= Time.deltaTime;
            if (adsBoostTimeRemain <= 0)
            {
                timeAdsObj.SetActive(false);
            }
        }
        if (x2BoostTimeRemain > 0)
        {
            spanX2 = TimeSpan.FromSeconds((double)x2BoostTimeRemain);
            x2BoostRemain.text = spanX2.ToString(@"mm\:ss");
            x2BoostTimeRemain -= Time.deltaTime;
            if (x2BoostTimeRemain <= 0)
            {
                timeX2Obj.SetActive(false);
            }
        }
        if (x5BoostTimeRemain > 0)
        {
            spanX5 = TimeSpan.FromSeconds((double)x5BoostTimeRemain);
            x5BoostRemain.text = spanX5.ToString(@"mm\:ss");
            x5BoostTimeRemain -= Time.deltaTime;
            if (x5BoostTimeRemain <= 0)
            {
                timeX5Obj.SetActive(false);
            }
        }
    }
}
