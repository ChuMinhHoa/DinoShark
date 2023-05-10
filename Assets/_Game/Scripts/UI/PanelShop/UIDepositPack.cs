using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDepositPack : MonoBehaviour
{
    [SerializeField] OfferData offer;
    [SerializeField] Button buyOfferBtn;
    [SerializeField] Text titleTxt;
    [SerializeField] Text priceTxt;
    [SerializeField] Slider slider;
    [SerializeField] RectTransform handle;
    [SerializeField] RectTransform wdValue;
    float wdAmount;
    [SerializeField] Text wdValueTxt;
    float maxAmount;
    [SerializeField] Text maxValueTxt;
    [SerializeField] Text currentValueTxt;
    private void Start()
    {
        buyOfferBtn.onClick.AddListener(BuyOffer);
    }
    public bool Init(OfferData offerData)
    {
        titleTxt.text = offerData.description;
        wdAmount = offerData.itemRewards[0].amount;
        wdValueTxt.text = offerData.itemRewards[0].amount.ToString();
        maxAmount = offerData.itemRewards[1].amount;
        maxValueTxt.text = offerData.itemRewards[1].amount.ToString();
        LoadOffer(offerData);
        //SetWithDrawPos();
        string priceLocal = MyIAPManager.instance.GetProductPriceFromStore(offer.productID);
        //priceTxt.text = priceLocal != "$0.01" ? priceLocal : priceTxt.text;
        if (priceLocal != "$0.01" && priceLocal != "")
        {
            priceTxt.text = priceLocal;
        }
        else
        {
            priceTxt.text = "$" + offer.price.ToString();
        }
        Reload();
        return true;
    }

    void LoadOffer(OfferData offerData)
    {
        offer = offerData;
        ItemReward reward = new ItemReward(OfferID.DepositPack);
        offer.itemRewards = new List<ItemReward>();
        offer.itemRewards.Add(reward);
    }

    void SetWithDrawPos()
    {
        slider.value = wdAmount / maxAmount;
        wdValue.anchoredPosition = new Vector2(handle.anchoredPosition.x, wdValue.anchoredPosition.y);
    }

    public void Reload()
    {
        int currentGem = ProfileManager.Instance.playerData.globalResourceSave.depositGems;
        currentValueTxt.text = currentGem.ToString();
        slider.value = currentGem / maxAmount;
        if(currentGem >= wdAmount)
        {
            buyOfferBtn.gameObject.SetActive(true);
        }
        else
        {
            buyOfferBtn.gameObject.SetActive(false);
        }
    }

    void BuyOffer()
    {
        SoundManager.instance.PlaySoundEffect(SoundID.UI_BUTTON);
        if (offer.offerType == OfferType.IAP)
        {
            MyIAPManager.instance.Buy(offer.productID, BuySuccess);
        }
    }
    /// <summary>
    /// Already claim reward in MyIAPManager
    /// This should call panel reward only
    /// </summary>
    public void BuySuccess()
    {
        //UIManager.instance.GetPanel(UIPanelType.PanelShop).GetComponent<PanelShop>().ReloadValue();
        //ProfileManager.Instance.playerData.globalResourceSave.OnSaveBoughtIAPPackage(offer.offerID);
        //GameManager.Instance.ClaimItemReward(offer.itemRewards);
        //GameManager.Instance.ClaimItemEquip(offer.itemEquips);
        Reload();
    }
}
