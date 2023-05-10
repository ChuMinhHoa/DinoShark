using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelShop : UIPanel
{
    public RectTransform topRect;
    public RectTransform productRect;
    [SerializeField] RectTransform content;
    bool shopBuilt = false;
    [SerializeField] ProductCargo suitcasesCargo;
    [SerializeField] ProductCargo boosterCargo;
    [SerializeField] ProductCargo gemsCargo;
    float gemCargoPos = 3600f;
    bool scrollToGemMarker;
    [SerializeField] SpecialCargo specialCargo;
    [SerializeField] DepositCargo depositCargo;
    [SerializeField] Button exitBtn;
    
    public override void Awake()
    {
        panelType = UIPanelType.PanelShop;
        base.Awake();
        exitBtn.onClick.AddListener(ClosePanel);
        CheckScreenObstacle();
    }

    void CheckScreenObstacle()
    {
        float screenRatio = (float)Screen.height / (float)Screen.width;
        if (screenRatio > 2.1f) // Now we got problem 
        {
            topRect.sizeDelta = new Vector2(0, -100);
            topRect.anchoredPosition = new Vector2(0, -50);
            productRect.sizeDelta = new Vector2(0, -300);
            productRect.anchoredPosition = new Vector2(0, -150);
        }
        else
        {
            topRect.sizeDelta = new Vector2(0, 0);
            topRect.anchoredPosition = new Vector2(0, 0);
        }
    }

    public void InnitShop(bool scroll)
    {
        scrollToGemMarker = scroll;
        StartCoroutine(BuildShop());
    }

    IEnumerator BuildShop()
    {
        if(!shopBuilt)
        {
            LayoutRebuilder.MarkLayoutForRebuild(content);
            yield return 0;
            suitcasesCargo.GenerateProduct(ProfileManager.Instance.dataConfig.shopDataConfig.suitcaseOfferDatas);
            yield return 0;
            specialCargo.InitOffer(ProfileManager.Instance.dataConfig.shopDataConfig.specialOfferDatas, ProfileManager.Instance.dataConfig.shopDataConfig.revenuePack);
            yield return 0;
            depositCargo.InitOffer(ProfileManager.Instance.dataConfig.shopDataConfig.depositPack);
            yield return 0;
            boosterCargo.GenerateProduct(ProfileManager.Instance.dataConfig.shopDataConfig.boosterOfferDatas);
            yield return 0;
            gemsCargo.GenerateProduct(ProfileManager.Instance.dataConfig.shopDataConfig.gemsOfferDatas);
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);
            shopBuilt = true;
        }
        if (scrollToGemMarker)
        {
            yield return 0;
            ScrollToGem();
        }
    }

    private void OnEnable()
    {
        if(shopBuilt)
        {
            ReloadValue();
        }
    }

    public void ReloadValue()
    {
        suitcasesCargo.ReloadValue();
        specialCargo.InitOffer(ProfileManager.Instance.dataConfig.shopDataConfig.specialOfferDatas, ProfileManager.Instance.dataConfig.shopDataConfig.revenuePack);
        boosterCargo.ReloadValue();
        depositCargo.Reload();
    }

    void ClosePanel()
    {
        SoundManager.instance.PlaySoundEffect(SoundID.UI_BUTTON);
        content.anchoredPosition = new Vector2(0f, 0f);
        UIManager.instance.ClosePanelShop();
    }

    public void ScrollToGem()
    {
        content.anchoredPosition = new Vector2(0f, gemCargoPos);
    }
}
