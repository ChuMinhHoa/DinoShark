using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using DG.Tweening;

public class PanelOpenOutfit : UIPanel
{

    [SerializeField] Image outfitFrame;
    [SerializeField] Image rarityTxtFrame;
    [SerializeField] Text rarityTxt;
    [SerializeField] Image outfitIcon;
    [SerializeField] Image outfitTypeIcon;
    [SerializeField] Image outfitTypeWrap;
    [SerializeField] Text outfitName;

    // Button
    [SerializeField] Button confirmBtn;
    [SerializeField] Button openBtn;
    [SerializeField] Text conditionTxt;
    [SerializeField] Image conditionImg;
    [SerializeField] Sprite gemSpr, nKeySpr, rKeySpr;
    [SerializeField] GameObject objButtonWrap;
    [SerializeField] RectTransform priceRt;

    // Data
    PlayerData playerData;
    OfferID offerID;
    OfferData currentOffer;
    RarityInfo currentRarityInfor;
    Sprite currentSpriteOfType;

    [Header("=========SLOT MOVE===========")]
    [SerializeField] Transform outfitSlotTrs;
    [SerializeField] Transform pointStart;
    [SerializeField] Transform pointMoveTarget;
    [SerializeField] Vector3 vectorZero = Vector3.zero;
    [SerializeField] Vector3 vectorRotage = new Vector3(0, 0, -5);

    [SerializeField] Image glassEffect;
    [SerializeField] Image lightEffect;
    [SerializeField] SkeletonGraphic valiAnimUI;

    [SerializeField] readonly string ANIM_CLOSE = "idle1";
    [SerializeField] readonly string ANIM_OPEN_IDLE = "idle2";
    [SerializeField] readonly string ANIM_OPEN = "open";

    [SerializeField] readonly string COMMON_SKIN = "common";
    [SerializeField] readonly string RARE_SKIN = "rare";
    [SerializeField] readonly string EPIC_SKIN = "epic";
    public override void Awake()
    {
        panelType = UIPanelType.PanelOpenOutfit;
        base.Awake();
        confirmBtn.onClick.AddListener(ClosePanel);
        openBtn.onClick.AddListener(OpenAnother);
        playerData = ProfileManager.Instance.playerData;
    }

    bool rewardOnPack;
    public void Setup(OfferID offer, int outfitID, OutfitType outfitType, Rarity rarity, bool rewardPack = false)
    {
        //SetAnim(ANIM_CLOSE, true);
        outfitSlotTrs.localScale = vectorZero;
        offerID = offer;
        rewardOnPack = rewardPack;
        OutfitData outfitData = ProfileManager.Instance.dataConfig.outfitDataConfig.GetOutfitData(outfitID, outfitType);
        currentRarityInfor = ProfileManager.Instance.dataConfig.outfitDataConfig.GetRarityInfo(rarity);
        currentSpriteOfType = ProfileManager.Instance.dataConfig.outfitDataConfig.GetSpriteOfType(outfitType);
        outfitFrame.sprite = currentRarityInfor.sprSlot;
        rarityTxtFrame.sprite = currentRarityInfor.sprSlot;
        rarityTxt.text = rarity.ToString();
        outfitIcon.sprite = outfitData.outfitIcon;
        outfitTypeIcon.sprite = currentSpriteOfType;
        outfitTypeWrap.color = currentRarityInfor.colorRarity;
        outfitName.text = outfitData.outfitName;
        lightEffect.color = currentRarityInfor.colorRarity;
        objButtonWrap.SetActive(false);

        switch (offer)
        {
            case OfferID.PeasantSuitcase:
                SetChestSkin(COMMON_SKIN);
                break;
            case OfferID.NobleSuitcase:
                SetChestSkin(RARE_SKIN);
                break;
            case OfferID.RoyalSuitcase:
                SetChestSkin(EPIC_SKIN);
                break;
            default:
                break;
        }

        SetAnim(ANIM_OPEN, false);
        StartCoroutine(IE_OpenDone());
    }

    public void SetAnim(string animName, bool bLoop) {
        valiAnimUI.AnimationState.SetAnimation(0, animName, loop: bLoop);
    }

    IEnumerator IE_OpenDone() {
        yield return new WaitForSeconds(.5f);
        SetAnim(ANIM_OPEN_IDLE, true);
        outfitSlotTrs.position = pointStart.position;

        outfitSlotTrs.DORotate(vectorRotage, .1f).OnComplete(() => {
            outfitSlotTrs.DORotate(-vectorRotage, .1f).OnComplete(() =>
            {
                outfitSlotTrs.DORotate(vectorZero, .1f);
            });
        });

        outfitSlotTrs.DOScale(0.5f, .2f).OnComplete(()=> {
            StartCoroutine(IE_WaitToShowButton());
        });
    }

    IEnumerator IE_WaitToShowButton() {
        yield return new WaitForSeconds(.5f);
        glassEffect.fillOrigin = (int)Image.OriginHorizontal.Left;
        glassEffect.DOFillAmount(1, 0.1f).OnComplete(()=> {
            glassEffect.fillOrigin = (int)Image.OriginHorizontal.Right;
            glassEffect.DOFillAmount(0, 0.1f).OnComplete(() => {
                outfitSlotTrs.DOMove(pointMoveTarget.position, 0.5f);
                outfitSlotTrs.DOScale(1f, 0.5f).OnComplete(() => {
                    objButtonWrap.SetActive(true);
                    SetupButton();
                });
            });
        });
        
    }

    void SetChestSkin(string chestSkin)
    {
        if (valiAnimUI != null)
        {
            valiAnimUI.Skeleton.SetSkin(chestSkin);
        }
    }
    int gemPrice = 0;
    void SetupButton()
    {
        switch (offerID)
        {
            case OfferID.PeasantSuitcase:
                openBtn.gameObject.SetActive(false);
                break;
            case OfferID.NobleSuitcase:
            case OfferID.RoyalSuitcase:
                openBtn.gameObject.SetActive(true);
                currentOffer = ProfileManager.Instance.dataConfig.shopDataConfig.GetSuitcaseByOfferID(offerID);
                gemPrice = (int)(currentOffer.price);
                if (playerData.globalResourceSave.IsHasSuitcase(offerID))
                {
                    openBtn.interactable = true;
                    conditionTxt.text = (playerData.globalResourceSave.GetSuitcaseRemain(offerID)).ToString();
                    if(offerID == OfferID.NobleSuitcase)
                    {
                        conditionImg.sprite = nKeySpr;
                    }
                    else if(offerID == OfferID.RoyalSuitcase)
                    {
                        conditionImg.sprite = rKeySpr;
                    }
                }
                else
                {
                    conditionImg.sprite = gemSpr;
                    conditionTxt.text = currentOffer.price.ToString();
                    if (playerData.globalResourceSave.IsEnoughGem((int)(currentOffer.price)))
                    {
                        openBtn.interactable = true;
                    }
                    else
                    {
                        openBtn.interactable = false;
                    }
                }
                break;
            default:
                openBtn.gameObject.SetActive(false);
                break;
        }
        if(rewardOnPack)
        {
            openBtn.gameObject.SetActive(false);
        }
        StartCoroutine(PriceCoroutine());
    }

    IEnumerator PriceCoroutine()
    {
        yield return new WaitForSeconds(0.25f);
        LayoutRebuilder.MarkLayoutForRebuild(priceRt);
        LayoutRebuilder.ForceRebuildLayoutImmediate(priceRt);
    }

    void OpenAnother()
    {
        if(!playerData.outfitSave.OpenBox(offerID))
        {
            playerData.globalResourceSave.ConsumeGem(gemPrice);
        }
    }

    void ClosePanel()
    {
        SetAnim(ANIM_CLOSE, true);
        UIManager.instance.GetPanel(UIPanelType.PanelShop).GetComponent<PanelShop>().ReloadValue();
        openAndCloseAnim.OnClose(UIManager.instance.ClosePanelOpenOutfit);
    }
}
