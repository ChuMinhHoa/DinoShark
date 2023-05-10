using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ItemRewardOnPack
{
    public GameObject slot;
    public Image icon;
    public Text des;
}

public class UISpecialOffer : MonoBehaviour
{
    [SerializeField] RectTransform rect;
    [SerializeField] OfferData offer;
    [SerializeField] List<ItemRewardOnPack> itemRewards;
    [SerializeField] List<UIItemEquip> uIItemEquips;
    [SerializeField] Button buyBtn;
    [SerializeField] Text titleTxt;
    [SerializeField] Text priceTxt;
    private void Start()
    {
        buyBtn.onClick.AddListener(BuyOffer);
    }
    public bool Init(OfferData offerData, float width)
    {
        offer = offerData;
        if (ProfileManager.Instance.playerData.globalResourceSave.CheckBoughtPack(offer.offerID))
        {
            gameObject.SetActive(false);
            return false;
        }
        rect.sizeDelta = new Vector2(width - 80f, 0f);
        //rect.sizeDelta = new Vector2(Screen.width - 80f, 0f);
        titleTxt.text = offerData.title;
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
        ShowItemOnPack();
        return true;
    }

    void ShowItemOnPack()
    {
        for (int i = 0; i < offer.itemRewards.Count; i++)
        {
            itemRewards[i].slot.SetActive(true);
            itemRewards[i].des.text = offer.itemRewards[i].amount.ToString();
            if(offer.itemRewards[i].itemType == OfferID.NoAds)
            {
                itemRewards[i].des.text = "No Ads";
            }
            itemRewards[i].icon.sprite = offer.itemRewards[i].icon;
        }
        for (int i = 0; i < offer.itemEquips.Count; i++)
        {
            uIItemEquips[i].Init(offer.itemEquips[i]);
        }
    }

    public void BuyOffer()
    {
        SoundManager.instance.PlaySoundEffect(SoundID.UI_BUTTON);
        if (offer.offerType == OfferType.IAP)
        {
            Debug.Log("BuyOffer");
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
