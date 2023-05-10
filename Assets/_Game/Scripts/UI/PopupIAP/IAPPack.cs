using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IAPPack : MonoBehaviour
{
    public OfferID offerId;
    OfferData offer;
    [SerializeField] string productId;
    [SerializeField] Text priceTxt;
    [SerializeField] Button buyBtn;
    void Start()
    {
        buyBtn.onClick.AddListener(BuyPack);
        offer = ProfileManager.Instance.dataConfig.shopDataConfig.GetOfferDataByOfferID(offerId);
        string priceLocal = MyIAPManager.instance.GetProductPriceFromStore(offer.productID);
        //priceTxt.text = priceLocal != "$0.01" ? priceLocal : "$" + offer.price.ToString();
        if (priceLocal != "$0.01" && priceLocal != "")
        {
            priceTxt.text = priceLocal;
        }
        else
        {
            priceTxt.text = "$" + offer.price.ToString();
        }
    }

    // Update is called once per frame
    void BuyPack()
    {
        if(offer == null)
        {
            return;
        }
        if (offer.offerType == OfferType.IAP)
        {
            MyIAPManager.instance.Buy(offer.productID, BuySuccess);
            //UIManager.instance.GetPanel(UIPanelType.PanelShop).GetComponent<PanelShop>().ReloadValue();
        }
    }

    public void BuySuccess()
    {
        UIManager.instance.GetTotalPanel().CheckQuickIAP();
        UIManager.instance.ClosePanelPopupIAP();
        //ProfileManager.Instance.playerData.globalResourceSave.OnSaveBoughtIAPPackage(offer.offerID);
        //GameManager.Instance.ClaimItemReward(offer.itemRewards);
        //GameManager.Instance.ClaimItemEquip(offer.itemEquips);
    }
}
