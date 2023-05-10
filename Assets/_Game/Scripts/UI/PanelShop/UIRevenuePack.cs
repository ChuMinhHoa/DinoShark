using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRevenuePack : MonoBehaviour
{
    [SerializeField] RectTransform rect;
    [SerializeField] OfferData offer;
    [SerializeField] Button buyOfferBtn;
    [SerializeField] Text titleTxt;
    [SerializeField] Text priceTxt;

    private void Start()
    {
        buyOfferBtn.onClick.AddListener(BuyOffer);
    }
    public bool Init(OfferData offerData, float width)
    {
        offer = offerData;
        if (ProfileManager.Instance.playerData.globalResourceSave.CheckBoughtPack(offer.offerID))
        {
            gameObject.SetActive(false);
            return false;
        }
        rect.sizeDelta = new Vector2(width - 80f, rect.sizeDelta.y);
        titleTxt.text = offerData.title;
        string priceLocal = MyIAPManager.instance.GetProductPriceFromStore(offer.productID);
        //priceTxt.text = priceLocal != "$0.01" ? priceLocal : "$" + offer.price.ToString();
        if(priceLocal != "$0.01" && priceLocal != "")
        {
            priceTxt.text = priceLocal;
        }
        else
        {
            priceTxt.text = "$" + offer.price.ToString();
        }
        return true;
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
        UIManager.instance.GetPanel(UIPanelType.PanelShop).GetComponent<PanelShop>().ReloadValue();
        //ProfileManager.Instance.playerData.globalResourceSave.OnSaveBoughtIAPPackage(offer.offerID);
        //GameManager.Instance.ClaimItemReward(offer.itemRewards);
        //GameManager.Instance.ClaimItemEquip(offer.itemEquips);
    }
}
