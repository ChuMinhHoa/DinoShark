using SDK;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShopOffer : MonoBehaviour
{
    [SerializeField] Button buyOfferBtn;
    [SerializeField] Text titleTxt;
    [SerializeField] Text desTxt;
    [SerializeField] Text priceTxt;
    [SerializeField] Image icon;
    [SerializeField] Image payIcon;
    [SerializeField] Sprite gemIc, adsIc, nKeyIc, rKeyIc;
    [SerializeField] Image bg;
    [SerializeField] Image buyBtnImg;
    [SerializeField] Sprite gemBuySpr;
    [SerializeField] Sprite iapSpr;
    [SerializeField] Sprite disableSpr;
    [SerializeField] RectTransform priceRect;
    PlayerData playerData;

    [SerializeField] OfferData offer;

    private void Start()
    {
        buyOfferBtn.onClick.AddListener(BuyOffer);
    }
    public void Init(OfferData offerData)
    {
        playerData = ProfileManager.Instance.playerData;
        offer = offerData;
        titleTxt.text = offerData.title;
        icon.sprite = offerData.itemRewards[0].icon;
        LoadValue();
        StartCoroutine(ResizeRect());
    }

    IEnumerator ResizeRect()
    {
        yield return new WaitForSeconds(0.2f);
        LayoutRebuilder.MarkLayoutForRebuild(priceRect);
        LayoutRebuilder.ForceRebuildLayoutImmediate(priceRect);
    }

    public void LoadValue()
    {
        // Setup price for product
        switch (offer.offerType)
        {
            case OfferType.GemBuy:
                SetGemPrice();
                break;
            case OfferType.IAP:
                //priceTxt.text = "$100";
                SetIAPPrice();
                // Get IAP price
                break;
            default:
                break;
        }

        // Setup description for product
        switch (offer.offerID)
        {
            case OfferID.PeasantSuitcase:
            case OfferID.NobleSuitcase:
            case OfferID.RoyalSuitcase:
                desTxt.text = offer.description;
                break;
            case OfferID.H4Revenue:
                // Canculate profit for 4h
                BigNumber profitAvg = playerData.GetMenuSave().dataFoodSave.GetAvgProfit();
                float serveTime = GameManager.Instance.GetFoodServedTime();
                BigNumber offlineProfit = 4 * 3600 * profitAvg / serveTime;
                if (min4HRevenue.IsBigger(offlineProfit))
                    offlineProfit = new BigNumber(min4HRevenue);
                desTxt.text = offlineProfit.ToString();
                break;
            case OfferID.X2Revenue:
            case OfferID.X5Revenue:
            case OfferID.Gems:
                desTxt.text = offer.description;
                break;
            default:
                break;
        }
    }

    BigNumber min4HRevenue = new BigNumber(2500);

    void SetGemPrice()
    {
        buyBtnImg.sprite = gemBuySpr;
        // Specific setup for suitcase because they can already be owned by player
        // So they can be opened when player has suitcase or buy them
        switch (offer.offerID)
        {
            case OfferID.PeasantSuitcase:
                pSuitcaseCoolDown = playerData.globalResourceSave.pSuitcaseCoolDown;
                if(pSuitcaseCoolDown <= 0)
                {
                    payIcon.gameObject.SetActive(!playerData.globalResourceSave.IsHaveFreePSuitcase());
                    payIcon.sprite = adsIc;
                    priceTxt.text = "Free";
                    buyOfferBtn.interactable = true;
                }
                else
                {
                    payIcon.gameObject.SetActive(false);
                    buyOfferBtn.interactable = false;
                    buyBtnImg.sprite = disableSpr;
                }
                return;
            case OfferID.NobleSuitcase:
                if (playerData.globalResourceSave.nobleSuitcase > 0)
                {
                    priceTxt.text = playerData.globalResourceSave.nobleSuitcase.ToString() + "/1";
                    payIcon.sprite = nKeyIc;
                    buyOfferBtn.interactable = true;
                    return;
                }
                break;
            case OfferID.RoyalSuitcase:
                if (playerData.globalResourceSave.royalSuitcase > 0)
                {
                    priceTxt.text = playerData.globalResourceSave.royalSuitcase.ToString() + "/1";
                    payIcon.sprite = rKeyIc;
                    buyOfferBtn.interactable = true;
                    return;
                }
                break;
            default:
                break;
        }
        // Other product can only be used when buy with gems
        payIcon.sprite = gemIc;
        priceTxt.text = offer.price.ToString("n0");
        if(ProfileManager.Instance.playerData.globalResourceSave.IsEnoughGem((int)(offer.price)))
        {
            buyOfferBtn.interactable = true;
        }
        else
        {
            buyOfferBtn.interactable = false;
            buyBtnImg.sprite = disableSpr;
        }
    }

    void SetIAPPrice()
    {
        buyBtnImg.sprite = iapSpr;
        payIcon.gameObject.SetActive(false);
        string priceLocal = MyIAPManager.instance.GetProductPriceFromStore(offer.productID);
        //priceTxt.text = priceLocal != "$0.01" ? priceLocal : "$" + offer.price.ToString();
        if (priceLocal != "$0.01" && priceLocal != "")
        {
            priceTxt.text = priceLocal;
            if (priceLocal.Length > 8)
            {
                priceTxt.fontSize = 55;
            }
            else
            {
                priceTxt.fontSize = 65;
            }
        }
        else
        {
            priceTxt.text = "$" + offer.price.ToString();
        }
    }

    /// <summary>
    /// Consume property and fet reward
    /// </summary>
    public void ConfirmBuy()
    {
        switch (offer.offerType)
        {
            case OfferType.GemBuy:
                switch (offer.offerID)
                {
                    case OfferID.PeasantSuitcase:
                        if (!playerData.globalResourceSave.buyOnTutorial)
                        {
                            playerData.globalResourceSave.BoughtTutorial();
                            UnlockSuitcase();
                            break;
                        }
                        if (playerData.globalResourceSave.IsHasSuitcase(offer.offerID))
                        {
                            if (ProfileManager.Instance.playerData.globalResourceSave.removeAds)
                            {
                                UnlockSuitcase();
                                return;
                            }
                            AdsManager.Instance.ShowRewardVideo(WatchVideoRewardType.FreeSuitcase.ToString(), UnlockSuitcase);
                        }
                        break;
                    case OfferID.NobleSuitcase:
                    case OfferID.RoyalSuitcase:
                        if(playerData.globalResourceSave.IsHasSuitcase(offer.offerID))
                        {
                            playerData.outfitSave.OpenBox(offer.offerID);
                        }
                        else if (playerData.globalResourceSave.IsEnoughGem((int)(offer.price)))
                        {
                            playerData.globalResourceSave.ConsumeGem((int)(offer.price));
                            playerData.outfitSave.OpenBox(offer.offerID);
                        }
                        break;
                    case OfferID.H4Revenue:
                    case OfferID.X2Revenue:
                    case OfferID.X5Revenue:
                        if (playerData.globalResourceSave.IsEnoughGem((int)(offer.price)))
                        {
                            playerData.globalResourceSave.ConsumeGem((int)(offer.price));
                            GameManager.Instance.ClaimItemReward(offer.itemRewards);
                        }
                        break;
                    default:
                        break;
                }
                UIManager.instance.GetPanel(UIPanelType.PanelShop).GetComponent<PanelShop>().ReloadValue();
                break;
            case OfferType.IAP:
                UIManager.instance.GetPanel(UIPanelType.PanelShop).GetComponent<PanelShop>().ReloadValue();
                // Already claim reward in MyIAPManager
                // This should call panel reward only
                break;
            default:
                break;
        }
    }
    void UnlockSuitcase()
    {
        playerData.outfitSave.OpenBox(offer.offerID); 
    }
    /// <summary>
    /// Show panel confirm when click buy product
    /// </summary>
    void BuyOffer()
    {
        SoundManager.instance.PlaySoundEffect(SoundID.UI_BUTTON);
        switch (offer.offerID)
        {
            case OfferID.PeasantSuitcase:
            case OfferID.NobleSuitcase:
            case OfferID.RoyalSuitcase:
            case OfferID.H4Revenue:
            case OfferID.X2Revenue:
            case OfferID.X5Revenue:
                UIManager.instance.ShowPanelConfirm();
                UIManager.instance.GetPanel(UIPanelType.PanelConfirm).GetComponent<PanelConfirm>().LoadConfirm(offer, ConfirmBuy);
                break;
            case OfferID.Gems:
                MyIAPManager.instance.Buy(offer.productID, ConfirmBuy);
                break;
            default:
                break;
        }
    }

    float pSuitcaseCoolDown;
    TimeSpan span;
    //string NEXTREWARD = "Next reward\n";
    private void Update()
    {
        if(offer.offerID == OfferID.PeasantSuitcase)
        {
            if(pSuitcaseCoolDown > 0)
            {
                pSuitcaseCoolDown -= Time.deltaTime;
                if(pSuitcaseCoolDown <= 0)
                {
                    LoadValue();
                }
                else
                {
                    span = TimeSpan.FromSeconds((double)pSuitcaseCoolDown);
                    //priceTxt.text = NEXTREWARD + span.ToString(@"hh\:mm"); ;
                    priceTxt.text = span.ToString(@"hh\:mm\:ss");
                    //priceTxt.text = ((int)span.TotalHours).ToString() + "h" + ((int)span.Minutes).ToString() + "m";
                }
            }
        }
    }
}
