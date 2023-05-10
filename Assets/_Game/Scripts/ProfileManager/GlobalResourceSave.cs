using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class GlobalResourceSave : GlobalSaveDataBase
{
    public int gem;
    public int nobleSuitcase, royalSuitcase;
    public int rSuitcaseCounter;
    public float pSuitcaseCoolDown;
    public string pSuitcaseFlag;
    bool pSuitcaseFree;
    public List<OfferID> boughtPack = new List<OfferID>();
    public bool buyOnTutorial;
    public bool removeAds;
    public bool x2Revenue;
    public bool soundOn;
    public bool musicOn;
    public int depositGems;
    int maxDeposit;


    public override void LoadData()
    {
        SetStringSave("GlobalResourceSave_");
        string jsonData = GetJsonData();
        if (!string.IsNullOrEmpty(jsonData))
        {
            GlobalResourceSave dataSave = JsonUtility.FromJson<GlobalResourceSave>(jsonData);
            gem = dataSave.gem;
            nobleSuitcase = dataSave.nobleSuitcase;
            royalSuitcase = dataSave.royalSuitcase;
            rSuitcaseCounter = dataSave.rSuitcaseCounter;
            pSuitcaseFlag = dataSave.pSuitcaseFlag;
            boughtPack = dataSave.boughtPack;
            removeAds = dataSave.removeAds;
            x2Revenue = dataSave.x2Revenue;
            soundOn = dataSave.soundOn;
            musicOn = dataSave.musicOn;
            buyOnTutorial = dataSave.buyOnTutorial;
            depositGems = dataSave.depositGems;
            allRev = dataSave.allRev;
            noAds = dataSave.noAds;
            vip1 = dataSave.vip1;
            vip2 = dataSave.vip2;
            vip3 = dataSave.vip3;
             EventManager.TriggerEvent(EventName.ChangeGem.ToString());
        }
        else
        {

            soundOn = true;
            musicOn = true;
            IsMarkChangeData();
            SaveData();
        }
        SetMaxDeposit();
        CalculatePSuitcaseCoolDown();
    }

    void CalculatePSuitcaseCoolDown()
    {
        if (!String.IsNullOrEmpty(pSuitcaseFlag))
        {
            DateTime momment = DateTime.Now;
            DateTime coolDownEndTime = DateTime.Parse(pSuitcaseFlag);
            if(momment.Date != coolDownEndTime.Date)
            {
                //pSuitcaseFree = true;
                pSuitcaseCoolDown = 0f;
            }
            else
            {
                TimeSpan span = coolDownEndTime.Subtract(momment);
                if (span.TotalSeconds > 0)
                {
                    pSuitcaseCoolDown = (float)(span.TotalSeconds);
                }
                else
                {
                    pSuitcaseCoolDown = 0f;
                }
            }
            pSuitcaseFree = false;
        }
        else
        {
            pSuitcaseFree = true;
            pSuitcaseCoolDown = 0f;
        }
    }
    public bool IsHaveFreePSuitcase()
    {
        return pSuitcaseFree;
    }

    public void AddGem(int amount)
    {
        IsMarkChangeData();
        gem += amount;
        ProfileManager.Instance.playerData.SaveData();
        EventManager.TriggerEvent(EventName.ChangeGem.ToString());
    }
    public void ConsumeGem(int amount)
    {
        if(IsEnoughGem(amount))
        {
            IsMarkChangeData();
            gem -= amount;
        }
        ProfileManager.Instance.playerData.SaveData();
        EventManager.TriggerEvent(EventName.ChangeGem.ToString());
    }
    public bool IsEnoughGem(int amount)
    {
        if(gem >= amount)
        {
            return true;
        }
        return false;
    }

    public int GetGem()
    {
        return gem;
    }

    public void AddSuitcase(OfferID type, int amount)
    {
        switch (type)
        {
            case OfferID.PeasantSuitcase:
                break;
            case OfferID.NobleSuitcase:
                nobleSuitcase += amount;
                break;
            case OfferID.RoyalSuitcase:
                royalSuitcase += amount;
                break;
            default:
                break;
        }
        EventManager.TriggerEvent(EventName.ChangeSuiteCase.ToString());
        IsMarkChangeData();
        SaveData();
    }

    float peasantCoolDown = 10; // Reopen in 10 minutes
    public bool ConsumeSuitcase(OfferID offerID)
    {
        bool hasSuitcase = false;
        switch (offerID)
        {
            case OfferID.PeasantSuitcase:
                pSuitcaseCoolDown = peasantCoolDown * 60;
                DateTime momment = DateTime.Now;
                pSuitcaseFlag = (momment.AddMinutes(peasantCoolDown)).ToString();
                pSuitcaseFree = false;
                break;
            case OfferID.NobleSuitcase:
                if (nobleSuitcase > 0)
                {
                    nobleSuitcase--;
                    hasSuitcase = true;
                }
                break;
            case OfferID.RoyalSuitcase:
                rSuitcaseCounter++;
                if (royalSuitcase > 0)
                {
                    royalSuitcase--;
                    hasSuitcase = true;
                }
                break;
            default:
                break;
        }
        IsMarkChangeData();
        SaveData();
        EventManager.TriggerEvent(EventName.ChangeSuiteCase.ToString());
        return hasSuitcase;
    }

    public void ResetRSuitcaseCount()
    {
        rSuitcaseCounter = 0;
    }

    public bool CheckRSuitcaseCount()
    {
        if(rSuitcaseCounter >= 5)
        {
            rSuitcaseCounter = 0;
            return true;
        }
        return false;
    }

    public bool IsHasSuitcase(OfferID offerID)
    {
        switch (offerID)
        {
            case OfferID.PeasantSuitcase:
                if (pSuitcaseCoolDown <= 0) return true;
                else return false;
            case OfferID.NobleSuitcase:
                if (nobleSuitcase > 0) return true;
                break;
            case OfferID.RoyalSuitcase:
                if (royalSuitcase > 0) return true;
                break;
            default:
                return false;
        }
        return false;
    }

    public int GetSuitcaseRemain(OfferID offerID)
    {
        switch (offerID)
        {
            case OfferID.PeasantSuitcase:
                return 0;
            case OfferID.NobleSuitcase:
                return nobleSuitcase;
            case OfferID.RoyalSuitcase:
                return royalSuitcase;
            default:
                return 0;
        }
    }

    public bool CheckBoughtPack(OfferID offerID)
    {
        if (boughtPack.Contains(offerID))
            return true;
        bool returnVal;
        switch (offerID)
        {
            case OfferID.NoAds:
                return noAds;
                //break;
            case OfferID.Vip1Pack:
                return vip1;
                //break;
            case OfferID.Vip2Pack:
                return vip2;
                //break;
            case OfferID.Vip3Pack:
                return vip3;
                //break;
            case OfferID.AllRevenue:
                return allRev;
                //break;
            default:
                return false;
        }
        return returnVal;
    }
    public bool allRev;
    public bool noAds;
    public bool vip1;
    public bool vip2;
    public bool vip3;
    public void OnSaveBoughtIAPPackage(OfferID offerID)
    {
        boughtPack.Add(offerID);
        switch (offerID)
        {
            case OfferID.NoAds:
                noAds = true;
                break;
            case OfferID.Vip1Pack:
                vip1 = true;
                break;
            case OfferID.Vip2Pack:
                vip2 = true;
                break;
            case OfferID.Vip3Pack:
                vip3 = true;
                break;
            case OfferID.AllRevenue:
                allRev = true;
                break;
            default:
                break;
        }
        IsMarkChangeData();
        SaveData();
    }

    public void SetRemoveAds(bool setValue)
    {
        removeAds = setValue;
        IsMarkChangeData();
    }

    public void SetX2Revenue(bool setValue)
    {
        x2Revenue = setValue;
        EventManager.TriggerEvent(EventName.ChangeBoostProfit.ToString());
        IsMarkChangeData();
    }
    public float GetX2Revenue()
    {
        if (x2Revenue) return 2f;
        return 1f;
    }

    public void SetMaxDeposit()
    {
        maxDeposit = (int)(ProfileManager.Instance.dataConfig.shopDataConfig.depositPack.itemRewards[1].amount);
    }
    public int GetMaxDeposit()
    {
        return maxDeposit;
    }

    public void AddDeposit(int value = 10)
    {
        if(depositGems < maxDeposit)
        {
            depositGems += value;
            if (depositGems >= maxDeposit)
            {
                depositGems = maxDeposit;
            }
        }
        IsMarkChangeData();
    }

    public void CollectDeposit()
    {
        AddGem(depositGems);
        depositGems = 0;
        IsMarkChangeData();
    }

    public void Update()
    {
        if(pSuitcaseCoolDown > 0)
        {
            pSuitcaseCoolDown -= Time.deltaTime;
            if (pSuitcaseCoolDown <= 0f)
                EventManager.TriggerEvent(EventName.ChangeSuiteCase.ToString());
        }
    }

    public void ChangeSoundState()
    {
        IsMarkChangeData();
        soundOn = !soundOn;
        SoundManager.instance.ChangeSoundState(soundOn);
    }
    public void ChangeMusicState()
    {
        IsMarkChangeData();
        musicOn = !musicOn;
        SoundManager.instance.ChangeMusicState(musicOn);
    }

    public void BoughtTutorial() {
        buyOnTutorial = true;
        IsMarkChangeData();
        ProfileManager.Instance.playerData.SaveData();
    }
}
