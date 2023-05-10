using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BoostSave : GlobalSaveDataBase
{
    [Header("Merchandise boost")]
    public float adsBoostTimeRemain;            
    public string boostEnd;
    [Header("Buy X2 boost with gems")]
    public float x2BoostTimeRemain;
    public string x2BoostEnd;
    [Header("Buy X5 boost with gems")]
    public float x5BoostTimeRemain;
    public string x5BoostEnd;

    public override void LoadData()
    {
        SetStringSave("BoostSave_");
        string jsonData = GetJsonData();
        if (!string.IsNullOrEmpty(jsonData))
        {
            BoostSave dataSave = JsonUtility.FromJson<BoostSave>(jsonData);
            boostEnd = dataSave.boostEnd;
            x2BoostEnd = dataSave.x2BoostEnd;
            x5BoostEnd = dataSave.x5BoostEnd;
        }
        else
        {
            IsMarkChangeData();
            boostEnd = DateTime.Now.ToString();
            x2BoostEnd = DateTime.Now.ToString();
            x5BoostEnd = DateTime.Now.ToString();
            SaveData();
        }
        AdsBoostRemainCalculate();
        GemBoostRemainCalculate();
        UIManager.instance.GetTotalPanel().adsBoostBtn.CheckCurrentBoost();
    }

    void AdsBoostRemainCalculate()
    {
        if (!String.IsNullOrEmpty(boostEnd))
        {
            DateTime momment = DateTime.Now;
            DateTime boostEndTime = DateTime.Parse(boostEnd);
            TimeSpan span = boostEndTime.Subtract(momment);
            if (span.TotalSeconds > 0)
            {
                adsBoostTimeRemain = (float)(span.TotalSeconds);
            }
            else
            {
                adsBoostTimeRemain = 0f;
            }
        }
    }

    public float AddAdsBoost()
    {
        if(adsBoostTimeRemain <= 0)
        {
            adsBoostTimeRemain = 0;
        }
        adsBoostTimeRemain += ProfileManager.Instance.playerData.merchandiseSave.GetAdsBoostDuration() * 60f;
        if(adsBoostTimeRemain >= 60 * 60)
        {
            adsBoostTimeRemain = 60 * 60; 
        }
        DateTime momment = DateTime.Now;
        boostEnd = (momment.AddSeconds(adsBoostTimeRemain)).ToString();
        IsMarkChangeData();
        SaveData();
        EventManager.TriggerEvent(EventName.ChangeBoostProfit.ToString());
        return adsBoostTimeRemain;
    }

    public float GetBoostEffect()
    {
        float boost = 1f;
        if (adsBoostTimeRemain > 0f)
        {
            boost *= ProfileManager.Instance.playerData.merchandiseSave.GetAdsBoostEffect();
        }
        if (x2BoostTimeRemain > 0)
        {
            boost *= 2f;
        }
        if (x5BoostTimeRemain > 0)
        {
            boost *= 5f;
        }
        return boost;
    }

    
    public void Update()
    {
        if(adsBoostTimeRemain > 0)
        {
            adsBoostTimeRemain -= Time.deltaTime;
            if (adsBoostTimeRemain <= 0)
            {
                EventManager.TriggerEvent(EventName.ChangeBoostProfit.ToString());
            }
        }
        if (x2BoostTimeRemain > 0)
        {
            x2BoostTimeRemain -= Time.deltaTime;
            if (x2BoostTimeRemain <= 0)
            {
                EventManager.TriggerEvent(EventName.ChangeBoostProfit.ToString());
            }
        }
        if (x5BoostTimeRemain > 0)
        {
            x5BoostTimeRemain -= Time.deltaTime;
            if(x5BoostTimeRemain <= 0)
            {
                EventManager.TriggerEvent(EventName.ChangeBoostProfit.ToString());
            } 
        }
    }

    public float GetAdsBoostRemain()
    {
        return adsBoostTimeRemain;
    }

    public void AddGemBuyBoost(OfferID type, float duration)
    {
        DateTime momment = DateTime.Now;
        switch (type)
        {
            case OfferID.X2Revenue:
                if (x2BoostTimeRemain <= 0)
                {
                    x2BoostTimeRemain = 0;
                }
                x2BoostTimeRemain += duration;
                x2BoostEnd = (momment.AddSeconds(x2BoostTimeRemain)).ToString();
                break;
            case OfferID.X5Revenue:
                if (x5BoostTimeRemain <= 0)
                {
                    x5BoostTimeRemain = 0;
                }
                x5BoostTimeRemain += duration;
                x5BoostEnd = (momment.AddSeconds(x5BoostTimeRemain)).ToString();
                break;
            default:
                break;
        }
        IsMarkChangeData();
        ProfileManager.Instance.playerData.SaveData();
        EventManager.TriggerEvent(EventName.ChangeBoostProfit.ToString());
        UIManager.instance.GetTotalPanel().adsBoostBtn.CheckCurrentBoost();
    }

    void GemBoostRemainCalculate()
    {
        DateTime momment = DateTime.Now;
        if (!String.IsNullOrEmpty(x2BoostEnd))
        {
            DateTime boostEndTime = DateTime.Parse(x2BoostEnd);
            TimeSpan spanX2 = boostEndTime.Subtract(momment);
            if (spanX2.TotalSeconds > 0)
            {
                x2BoostTimeRemain = (float)(spanX2.TotalSeconds);
            }
            else
            {
                x2BoostTimeRemain = 0f;
            }
        }
        if (!String.IsNullOrEmpty(x5BoostEnd))
        {
            DateTime boostEndTime = DateTime.Parse(x5BoostEnd);
            TimeSpan spanX2 = boostEndTime.Subtract(momment);
            if (spanX2.TotalSeconds > 0)
            {
                x5BoostTimeRemain = (float)(spanX2.TotalSeconds);
            }
            else
            {
                x5BoostTimeRemain = 0f;
            }
        }
    }
}
