using SDK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class OutfitInviteSlot {
    public Button btnActive;
    [SerializeField] Image imgWrap;
    [SerializeField] Image subFrame;
    [SerializeField] Image imgIcon;
    [SerializeField] Image imgWrapType;
    [SerializeField] Image imgType;
    [SerializeField] Text txtLevel;
    [SerializeField] List<GameObject> starsPlus;
    OutfitData outfitData;
    public void InitData(InviteOutfitRandom inviteOutfitRandom, RarityInfo rarityInfo) {
        outfitData = ProfileManager.Instance.dataConfig.outfitDataConfig.GetOutfitData(inviteOutfitRandom.outfitID, inviteOutfitRandom.outfitType);
        imgWrap.sprite = rarityInfo.sprSlot;
        if(rarityInfo.subFrame != null)
        {
            subFrame.gameObject.SetActive(true);
            subFrame.sprite = rarityInfo.subFrame;
        }
        else
        {
            subFrame.gameObject.SetActive(false);
        }
        SetupStarPlus(rarityInfo.rarity);
        imgIcon.sprite = outfitData.outfitIcon;
        imgWrapType.sprite = rarityInfo.sprSlot;
        imgType.sprite = ProfileManager.Instance.dataConfig.outfitDataConfig.GetSpriteOfType(inviteOutfitRandom.outfitType);
        txtLevel.text = "Lv." + inviteOutfitRandom.level.ToString();
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
}

public class UIInviteSlot : MonoBehaviour
{
    [SerializeField] OutfitInviteSlot hatSlot;
    [SerializeField] OutfitInviteSlot clothesSlot;
    [SerializeField] OutfitInviteSlot toolSlot;
    [SerializeField] Button btnInvite;
    [SerializeField] RawImage rawImage;
    [SerializeField] Button adsBtn;

    InviteOutfitRandom hatInvitedata;
    InviteOutfitRandom clothesInvitedata;
    InviteOutfitRandom toolInvitedata;

    RarityInfo rarityInfo;
    InviteHelperManager inviteHelperManager;
    MyRaw myRaw;
    PlayerData playerData;

    private void Awake()
    {
        hatSlot.btnActive.onClick.AddListener(() => {
            OnShowDetail(OutfitType.Hat);
        });

        clothesSlot.btnActive.onClick.AddListener(() => {
            OnShowDetail(OutfitType.Clothes);
        });

        toolSlot.btnActive.onClick.AddListener(() => {
            OnShowDetail(OutfitType.Tool);
        });

        btnInvite.onClick.AddListener(OnInvite);
        adsBtn.onClick.AddListener(WatchAdsToInvite);   
    }

    public void InitData() {
        myRaw = RawImageManager.Instance.GetMyRaw(RawType.RawInvite, transform.GetSiblingIndex());
        inviteHelperManager = InviteHelperManager.Instance;
        playerData = ProfileManager.Instance.playerData;
        rawImage.texture = myRaw.rawSkin;

        hatInvitedata = inviteHelperManager.hatOutfitRandom[transform.GetSiblingIndex()];
        InitSlotOutfitData(hatInvitedata, OutfitType.Hat);

        clothesInvitedata = inviteHelperManager.clothesOutfitRandom[transform.GetSiblingIndex()];
        InitSlotOutfitData(clothesInvitedata, OutfitType.Clothes);

        toolInvitedata = inviteHelperManager.toolOutfitRandom[transform.GetSiblingIndex()];
        InitSlotOutfitData(toolInvitedata, OutfitType.Tool);
        btnInvite.gameObject.SetActive(playerData.inviteSave.CheckFreeInvite());
    }

    void InitSlotOutfitData(InviteOutfitRandom inviteOutfitRandom, OutfitType outfitType) {
        rarityInfo = ProfileManager.Instance.dataConfig.outfitDataConfig.GetRarityInfo(inviteOutfitRandom.rarity);
        switch (outfitType)
        {
            case OutfitType.Hat:
                hatSlot.InitData(inviteOutfitRandom, rarityInfo);
                break;
            case OutfitType.Clothes:
                clothesSlot.InitData(inviteOutfitRandom, rarityInfo);
                break;
            case OutfitType.Tool:
                toolSlot.InitData(inviteOutfitRandom, rarityInfo);
                break;
        }
        myRaw.equipOutfit.Equip(inviteOutfitRandom.outfitID, inviteOutfitRandom.outfitType);
    }

    void OnShowDetail(OutfitType outfitType) {
        switch (outfitType)
        {
            case OutfitType.Hat:
                UIManager.instance.ShowPanelOutfitDetail(hatInvitedata);
                break;
            case OutfitType.Clothes:
                UIManager.instance.ShowPanelOutfitDetail(clothesInvitedata);
                break;
            case OutfitType.Tool:
                UIManager.instance.ShowPanelOutfitDetail(toolInvitedata);
                break;
            default:
                break;
        }
    }

    void OnInvite() {
        SoundManager.instance.PlaySoundEffect(SoundID.UI_BUTTON);
        playerData.GetInviteSave().SaveInviteOutfitData(hatInvitedata, OutfitType.Hat);
        playerData.GetInviteSave().SaveInviteOutfitData(clothesInvitedata, OutfitType.Clothes);
        playerData.GetInviteSave().SaveInviteOutfitData(toolInvitedata, OutfitType.Tool);
        playerData.GetInviteSave().OnInvite();
        playerData.SaveData();
        inviteHelperManager.OnInvited();
        UIManager.instance.GetPanel(UIPanelType.PanelInvite).GetComponent<PanelInvite>().UnInit();
        UIManager.instance.ClosePanelInvite();
    }

    void WatchAdsToInvite()
    {
        if (ProfileManager.Instance.playerData.globalResourceSave.removeAds)
        {
            OnInvite();
            return;
        }  
        AdsManager.Instance.ShowRewardVideo(WatchVideoRewardType.InviteAds.ToString(), OnInvite);
    }
}
