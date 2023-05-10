using DG.Tweening;
using SDK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AdsReward
{
    Gem, 
    Money
}

public class AdsRewardButton : MonoBehaviour
{
    [SerializeField] Button btn;
    [SerializeField] Text valueTxt;
    [SerializeField] Image adsIcon, icon;
    [SerializeField] Sprite gemRewardIcon, gemIcon;
    [SerializeField] Sprite moneyRewardIcon, moneyIcon;
    [SerializeField] RectTransform valueRect;

    [SerializeField] Transform iconTr;
    [SerializeField] Transform defaultPos;
    [SerializeField] Transform hidenPos;
    bool OnScene = false;
    float OnSceneTime = 25;
    float OnSceneRemain;
    AdsReward adsReward;
    int gemReward;
    BigNumber moneyReward;
    private void Start()
    {
        btn.onClick.AddListener(ButtonOnClick);
        iconTr.position = hidenPos.position;
    }

    void ButtonOnClick()
    {
        btn.interactable = false;
        OnScene = false;
        iconTr.DOMove(hidenPos.position, 0.5f);
        switch (adsReward)
        {
            case AdsReward.Gem:
                if (ProfileManager.Instance.playerData.globalResourceSave.removeAds)
                {
                    GetReward();
                    return;
                }
                AdsManager.Instance.ShowRewardVideo(WatchVideoRewardType.GemRewardAds.ToString(), GetReward);
                break;
            case AdsReward.Money:
                if (ProfileManager.Instance.playerData.globalResourceSave.removeAds)
                {
                    GetReward();
                    return;
                }
                AdsManager.Instance.ShowRewardVideo(WatchVideoRewardType.MoneyRewardAds.ToString(), GetReward);
                break;
            default:
                break;
        }
    }
    
    void GetReward()
    {
        UIManager.instance.GetTotalPanel().SetAdsRewardShowing(false);
        switch (adsReward)
        {
            case AdsReward.Gem:
                ProfileManager.Instance.playerData.globalResourceSave.AddGem(gemReward);
                break;
            case AdsReward.Money:
                ProfileManager.Instance.playerData.AddMoney(moneyReward);
                break;
            default:
                break;
        }
    }

    public void InitReward()
    {
        if (ProfileManager.Instance.playerData.GetCurrentLevel() < 1)
        {
            OnSceneRemain = 0;
            btn.interactable = false;
            UIManager.instance.GetTotalPanel().SetAdsRewardShowing(false);
            OnScene = false;
            return;
        }
        iconTr.DOMove(defaultPos.position, 0.5f).OnComplete(() => { btn.interactable = true; });
        OnScene = true;
        if (ProfileManager.Instance.playerData.merchandiseSave.AbleToGetGemAds())
        {
            adsReward = AdsReward.Gem;
            gemReward = 5;
            adsIcon.sprite = gemRewardIcon;
            icon.sprite = gemIcon;
            LayoutRebuilder.MarkLayoutForRebuild(valueRect);
            valueTxt.text = gemReward.ToString();
            LayoutRebuilder.ForceRebuildLayoutImmediate(valueRect);
        }
        else
        {
            adsReward = AdsReward.Money;
            adsIcon.sprite = moneyRewardIcon;
            icon.sprite = moneyIcon;
            moneyReward = ProfileManager.Instance.playerData.GetMenuSave().dataFoodSave.GetAdsMoneyReward() * 40 * ProfileManager.Instance.playerData.merchandiseSave.GetAdsRewardBoost();
            if(moneyReward < 160)
            {
                moneyReward = 160;
            }
            LayoutRebuilder.MarkLayoutForRebuild(valueRect);
            valueTxt.text = moneyReward.ToString();
            LayoutRebuilder.ForceRebuildLayoutImmediate(valueRect);
        }      
    }

    private void Update()
    {
        if(OnScene)
        {
            OnSceneRemain += Time.deltaTime;
            if(OnSceneRemain >= OnSceneTime)
            {
                OnSceneRemain = 0;
                btn.interactable = false;
                iconTr.DOMove(hidenPos.position, 0.5f);
                UIManager.instance.GetTotalPanel().SetAdsRewardShowing(false);
                OnScene = false;
            }
        }
    }

    public void Reset()
    {
        OnSceneRemain = 0;
        btn.interactable = false;
        iconTr.position = hidenPos.position;
        UIManager.instance.GetTotalPanel().SetAdsRewardShowing(false);
        OnScene = false;
    }
}
