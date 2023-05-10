using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PanelConfirm : UIPanel
{
    [SerializeField] Text offerTitle;
    [SerializeField] Text price;
    [SerializeField] Text value;
    [SerializeField] Image icon;
    [SerializeField] Button confirmBtn, cancelBtn, bgBtn;
    [SerializeField] RectTransform content;
    [SerializeField] Image payIcon;
    [SerializeField] Sprite gemIc, adsIc, nKeyIc, rKeyIc;
    [SerializeField] Image buyBtnImg;
    [SerializeField] Sprite activeSpr, unActiveSpr;

    string H4REVENUE = " from profit in 4 hours";
    PlayerData playerData;

    public override void Awake()
    {
        panelType = UIPanelType.PanelConfirm;
        base.Awake();
        cancelBtn.onClick.AddListener(ClosePanel);
        //bgBtn.onClick.AddListener(ClosePanel);
        confirmBtn.onClick.AddListener(OnConfirm);
        playerData = ProfileManager.Instance.playerData;
    }
    BigNumber min4HRevenue = new BigNumber(2500);
    public void LoadConfirm(OfferData offerData, UnityAction action)
    {
        LayoutRebuilder.MarkLayoutForRebuild(content);
        confirmBuy = action;
        offerTitle.text = offerData.title;
        price.text = (offerData.price).ToString("n0");
        icon.sprite = offerData.itemRewards[0].icon;
        switch (offerData.offerID)
        {
            case OfferID.H4Revenue:
                BigNumber profitAvg = ProfileManager.Instance.playerData.GetMenuSave().dataFoodSave.GetAvgProfit();
                float serveTime = GameManager.Instance.GetFoodServedTime();
                BigNumber offlineProfit = 4 * 3600 * profitAvg / serveTime;
                if (min4HRevenue.IsBigger(offlineProfit))
                    offlineProfit = new BigNumber(min4HRevenue);
                value.text = offlineProfit.ToString() + H4REVENUE;
                break;
            case OfferID.X2Revenue:
            case OfferID.X5Revenue:
                value.text = offerData.description;
                break;
            default:
                value.text = "";
                break;
        }
        CheckAbleToBuy(offerData);
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
    }

    void CheckAbleToBuy(OfferData offer)
    {
        payIcon.gameObject.SetActive(true);
        switch (offer.offerID)
        {
            case OfferID.PeasantSuitcase:
            case OfferID.NobleSuitcase:
            case OfferID.RoyalSuitcase:
                value.text = offer.description;
                if (playerData.globalResourceSave.IsHasSuitcase(offer.offerID))
                {
                    SetAbleToBuy(true);
                    if (offer.offerID == OfferID.PeasantSuitcase)
                    {
                        payIcon.gameObject.SetActive(!playerData.globalResourceSave.IsHaveFreePSuitcase());
                        payIcon.sprite = adsIc;
                        price.text = "Free";
                    }
                    else 
                    { 
                        price.text = playerData.globalResourceSave.GetSuitcaseRemain(offer.offerID).ToString();
                        if(offer.offerID == OfferID.NobleSuitcase)
                        {
                            payIcon.sprite = nKeyIc;
                        }
                        else if(offer.offerID == OfferID.RoyalSuitcase)
                        {
                            payIcon.sprite = rKeyIc;
                        }
                    }
                    return;
                }
                else if(playerData.globalResourceSave.IsEnoughGem((int)(offer.price)) && offer.offerID != OfferID.PeasantSuitcase)
                {
                    SetAbleToBuy(true);
                    if (offer.offerID == OfferID.PeasantSuitcase)
                    {
                        payIcon.sprite = adsIc;
                        price.text = "Free";
                    }
                    else
                    {
                        payIcon.sprite = gemIc;
                        price.text = (offer.price).ToString("n0");
                    }
                    return;
                }
                break;
            case OfferID.H4Revenue:
            case OfferID.X2Revenue:
            case OfferID.X5Revenue:
                if (playerData.globalResourceSave.IsEnoughGem((int)(offer.price)))
                {
                    SetAbleToBuy(true);
                    payIcon.sprite = gemIc;
                    price.text = (offer.price).ToString("n0");
                    return;
                }
                break;
            default:
                SetAbleToBuy(false);
                price.text = "";
                break;
        }
        SetAbleToBuy(false);
        payIcon.sprite = gemIc;
        price.text = (offer.price).ToString("n0");
    }

    void SetAbleToBuy(bool buyAble)
    {
        confirmBtn.interactable = buyAble;
        if(buyAble)
        {
            buyBtnImg.sprite = activeSpr;
        }
        else
        {
            buyBtnImg.sprite = unActiveSpr;
        }
    }

    UnityAction confirmBuy;
    void OnConfirm()
    {
        SoundManager.instance.PlaySoundEffect(SoundID.UI_BUTTON);
        confirmBuy();
        ClosePanel();
    }

    void ClosePanel()
    {
        SoundManager.instance.PlaySoundEffect(SoundID.UI_BUTTON);
        if (openAndCloseAnim != null)
            openAndCloseAnim.OnClose(UIManager.instance.ClosePanelConfirm);
        else UIManager.instance.ClosePanelConfirm();
    }
}
