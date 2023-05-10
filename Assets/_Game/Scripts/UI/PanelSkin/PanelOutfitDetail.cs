using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class PanelOutfitDetail : UIPanel
{

    public override void Awake()
    {
        panelType = UIPanelType.PanelDetailOutfit;
        base.Awake();
    }

    [Header("Rarity boot")]
    [SerializeField] GroupShowRarityBoot bootRare;
    [SerializeField] GroupShowRarityBoot bootEpic;
    [SerializeField] GroupShowRarityBoot bootLegend;
    [SerializeField] GroupShowRarityBoot bootMystic;

    [Header("Boot of outfit")]
    [SerializeField] Text txtPriceUpgrade;
    [SerializeField] Text txtCurrentBoot;
    [SerializeField] Text txtNextBootAdd;

    [Header("Other")]
    [SerializeField] Text txtLevel;
    [SerializeField] Text txtRarity;
    [SerializeField] Text txtName;

    [SerializeField] Button btnEquip;
    [SerializeField] Button btnRevert;
    [SerializeField] Button btnExit;
    [SerializeField] Button btnUpgrade;

    [SerializeField] Image imgOutfitIcon;
    [SerializeField] Image imgType;
    [SerializeField] Image imgTypeWrap;
    [SerializeField] GameObject imgGemPrice;
    [SerializeField] GameObject imgNextBoot;
    [SerializeField] GameObject textNextBoot;

    [Header("ColorFix")]
    [SerializeField] Image imgOutfitWrap;
    [SerializeField] Image imgOutfitSubFrame;
    [SerializeField] List<GameObject> starsPlus;
    [SerializeField] Image titleNameWrap;
    [SerializeField] RectTransform rectPrice;
    [SerializeField] RectTransform rectBootWrap;
    [SerializeField] Image buttonImg;
    [SerializeField] Text buttonTxt;
    [SerializeField] Sprite btnOn, btnOff;

    int currentUpgradeGem;
    int currentRevertGem;
    int level;
    int currentOutfitSaveID;
    int currentOutfitID;
    int rarityMaxLevel;
    bool onUsing;
    Rarity currentRarity;
    OutfitType currentOutfitType;
    OwnedOutfit ownedOutfit;

    private void Start()
    {
        btnExit.onClick.AddListener(ExitPanel);
        btnEquip.onClick.AddListener(ButtonActionOnClick);
        btnRevert.onClick.AddListener(RevertUpgrade);
        btnUpgrade.onClick.AddListener(UpgradeOutfit);
    }

    public void InitData(int ownedOutfitID, UnityAction onUpgrade = null) {
        ownedOutfit = ProfileManager.Instance.playerData.outfitSave.GetOwnedOutfit(ownedOutfitID);
        level = ownedOutfit.level;
        currentOutfitSaveID = ownedOutfit.saveID;
        currentRarity = ownedOutfit.rarity;
        currentOutfitType = ownedOutfit.outfitType;
        currentOutfitID = ownedOutfit.outfitID;
        SetupStarPlus(currentRarity);
        onUsing = ownedOutfit.onUsing;
        if (onUsing)
        {
            buttonTxt.text = "Unequip";
            buttonImg.sprite = btnOff;
        }
        else
        {
            buttonTxt.text = "Equip";
            buttonImg.sprite = btnOn;
        }
        LoadData();
        SetUpOnPanelOutfit();
    }

    void SetupStarPlus(Rarity rarity)
    {
        int starCount = 0;
        switch (rarity)
        {
            case Rarity.Epic_1:
            case Rarity.Legend_1:
                starCount = 1;
                break;
            case Rarity.Epic_2:
            case Rarity.Legend_2:
                starCount = 2;
                break;
            case Rarity.Legend_3:
                starCount = 3;
                break;
            default:
                starCount = 0;
                break;
        }
        for (int i = 0; i < starsPlus.Count; i++)
        {
            if (i < starCount)
                starsPlus[i].SetActive(true);
            else
                starsPlus[i].SetActive(false);
        }
    }

    public void InitDataOnInvitePanel(InviteOutfitRandom inviteOutfitRandom) {
        currentOutfitID = inviteOutfitRandom.outfitID;
        level = inviteOutfitRandom.level;
        currentRarity = inviteOutfitRandom.rarity;
        currentOutfitType = inviteOutfitRandom.outfitType;
        onUsing = false;
        LoadData();
        SetupOnInvite();
    }

    public void InitDataOnDailyRewardPanel(ItemEquip itemEquip) {
        currentOutfitID = itemEquip.itemId;
        level = 1;
        currentRarity = itemEquip.rarity;
        currentOutfitType = itemEquip.outfitType;
        onUsing = false;
        LoadData();
        SetupOnInvite();
    }

    void LoadData() {
        OutfitData outfitData = ProfileManager.Instance.dataConfig.outfitDataConfig.GetOutfitData(currentOutfitID, currentOutfitType);
        RarityInfo rarityInfo = ProfileManager.Instance.dataConfig.outfitDataConfig.GetRarityInfo(currentRarity);

        ChangeColorFollowRarity(rarityInfo);

        currentUpgradeGem = rarityInfo.priceUpgrade;

        imgOutfitIcon.sprite = outfitData.outfitIcon;
        imgType.sprite = ProfileManager.Instance.dataConfig.outfitDataConfig.GetSpriteOfType(currentOutfitType);

        txtRarity.text = currentRarity.ToString();
        txtName.text = outfitData.outfitName;
        txtLevel.text = level.ToString() + "/" + rarityInfo.maxLevel.ToString();

        LayoutRebuilder.MarkLayoutForRebuild(rectPrice);
        LayoutRebuilder.MarkLayoutForRebuild(rectBootWrap);

        imgGemPrice.SetActive(!(level == rarityInfo.maxLevel));
        if (level < rarityInfo.maxLevel)
        {
            txtPriceUpgrade.text = (rarityInfo.priceUpgrade * level).ToString();
            textNextBoot.SetActive(true);
            imgNextBoot.SetActive(true);
        }
        else
        {
            textNextBoot.SetActive(false);
            imgNextBoot.SetActive(false);
            txtPriceUpgrade.text = "MAX";
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(rectPrice);
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectBootWrap);

        for (int i = 0; i < outfitData.bootOutfits.Count; i++)
        {
            switch (outfitData.bootOutfits[i].rarity)
            {
                case Rarity.Rare:
                    bootRare.InitData(outfitData.bootOutfits[i], "", 0, currentRarity >= Rarity.Rare);
                    break;
                case Rarity.Epic:
                    bootEpic.InitData(outfitData.bootOutfits[i], "", 0, currentRarity >= Rarity.Epic);
                    break;
                case Rarity.Legend:
                    bootLegend.InitData(outfitData.bootOutfits[i], "", 0, currentRarity >= Rarity.Legend);
                    break;
                case Rarity.Mystic:
                    bootMystic.InitData(outfitData.bootOutfits[i], outfitData.outfitName, level, currentRarity >= Rarity.Mystic);
                    break;
            }
        }
        txtCurrentBoot.text = ProfileManager.Instance.dataConfig.outfitDataConfig.GetBootOfItem(currentRarity, level).ToString() + "%";
        txtNextBootAdd.text = ProfileManager.Instance.dataConfig.outfitDataConfig.GetBootOfItem(currentRarity, level + 1).ToString() + "%";
        rarityMaxLevel = rarityInfo.maxLevel;
        btnRevert.interactable = !(level == 1);
        btnUpgrade.interactable = GameManager.Instance.IsHaveEnoughGem(currentUpgradeGem) && level < rarityInfo.maxLevel;
        //btnEquip.interactable = !onUsing;
    }

    void SetupOnInvite()
    {
        btnEquip.gameObject.SetActive(false);
        btnRevert.gameObject.SetActive(false);
        btnUpgrade.gameObject.SetActive(false);
    }

    void SetUpOnPanelOutfit() {
        btnEquip.gameObject.SetActive(true);
        btnRevert.gameObject.SetActive(true);
        btnUpgrade.gameObject.SetActive(true);
    }

    void ChangeColorFollowRarity(RarityInfo rarityInfo) {
        imgOutfitWrap.sprite = rarityInfo.sprSlot;
        titleNameWrap.color = rarityInfo.colorRarity;
        imgTypeWrap.color = rarityInfo.colorRarity;
        if(rarityInfo.subFrame != null)
        {
            imgOutfitSubFrame.gameObject.SetActive(true);
            imgOutfitSubFrame.sprite = rarityInfo.subFrame;
        }
        else
        {
            imgOutfitSubFrame.gameObject.SetActive(false);
        }
    }

    void ExitPanel() {
        SoundManager.instance.PlaySoundEffect(SoundID.UI_BUTTON);
        UIManager.instance.ClosePanelOufitDetail();
    }

    void ButtonActionOnClick()
    {
        if(ownedOutfit.onUsing)
        {
            TakeOffOutfit();
        }
        else
        {
            EquipOutfit();
        }
    }

    void EquipOutfit() {
        SoundManager.instance.PlaySoundEffect(SoundID.UI_BUTTON);
        ProfileManager.Instance.playerData.outfitSave.OnEquipOutfit(currentOutfitType, currentOutfitSaveID);
        ProfileManager.Instance.playerData.SaveData();
        InitData(currentOutfitSaveID);
        PanelOutfit.Instance.ChangeData();
        PanelOutfit.Instance.InitData();
        GameManager.Instance.kitchenManager.ChangeOutfitMyCheff(currentOutfitSaveID);
        //Debug.Log("Equip outfit");
    }

    void TakeOffOutfit()
    {
        SoundManager.instance.PlaySoundEffect(SoundID.UI_BUTTON);
        ProfileManager.Instance.playerData.outfitSave.OnTakeOffOutfit(currentOutfitType);
        ProfileManager.Instance.playerData.SaveData();
        InitData(currentOutfitSaveID);
        PanelOutfit.Instance.ChangeData();
        PanelOutfit.Instance.InitData();
        GameManager.Instance.kitchenManager.TakeoffOutfitMyChef(ownedOutfit.outfitType);
    }

    void RevertUpgrade()
    {
        SoundManager.instance.PlaySoundEffect(SoundID.UI_BUTTON);
        ProfileManager.Instance.playerData.outfitSave.OnRevertOutfit(currentOutfitType, currentOutfitSaveID);
        ProfileManager.Instance.playerData.SaveData();
        InitData(currentOutfitSaveID);
        PanelOutfit.Instance.UpdateDataOnSlot(currentOutfitSaveID);
        PanelOutfit.Instance.UpdateTotalBoot();
        Debug.Log("Revert outfit ID: " + currentOutfitSaveID);
    }

    void UpgradeOutfit()
    {
        SoundManager.instance.PlaySoundEffect(SoundID.UI_BUTTON);
        currentUpgradeGem = ProfileManager.Instance.dataConfig.outfitDataConfig.GetGemUpgradePrice(currentRarity, level);
        ProfileManager.Instance.playerData.outfitSave.OnUpgradeOutfit(currentOutfitType, currentOutfitSaveID);
        ProfileManager.Instance.playerData.globalResourceSave.ConsumeGem(currentUpgradeGem);
        ProfileManager.Instance.playerData.SaveData();
        InitData(currentOutfitSaveID);
        PanelOutfit.Instance.UpdateDataOnSlot(currentOutfitSaveID);
        PanelOutfit.Instance.UpdateTotalBoot();
        btnUpgrade.interactable = GameManager.Instance.IsHaveEnoughGem(currentUpgradeGem) && level < rarityMaxLevel;
    }

}
