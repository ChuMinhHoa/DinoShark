using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OutfitSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image icon;
    [SerializeField] Image iconType;
    [SerializeField] Image slotWrap;
    [SerializeField] Image subFrame;
    [SerializeField] Outline subFrameLine;
    [SerializeField] Image itemTypeWrap;
    [SerializeField] Text txtLevel;
    [SerializeField] List<GameObject> starsPlus;
    public OutfitData myOutfitData;
    public RarityInfo myRarityInfor;
    OwnedOutfit ownedOutfit;
    public OutfitType outfitType;
    public int ownedOutfitID = -1;
    public virtual void InitData(OwnedOutfit ownedOutfit) {
        if(ownedOutfit != null)
        {
            this.ownedOutfit = ownedOutfit;
            ownedOutfitID = ownedOutfit.saveID;
            myOutfitData = ProfileManager.Instance.dataConfig.outfitDataConfig.GetOutfitData(ownedOutfit.outfitID, ownedOutfit.outfitType);
            myRarityInfor = ProfileManager.Instance.dataConfig.outfitDataConfig.GetRarityInfo(ownedOutfit.rarity);
            icon.sprite = myOutfitData.outfitIcon;
            outfitType = ownedOutfit.outfitType;
            iconType.sprite = ProfileManager.Instance.dataConfig.outfitDataConfig.GetSpriteOfType(outfitType);
            txtLevel.text = ownedOutfit.level + "/" + myRarityInfor.maxLevel;
            slotWrap.sprite = myRarityInfor.sprSlot;
            SetupStarPlus(myRarityInfor.rarity);
            if (myRarityInfor.subFrame != null)
            {
                subFrame.sprite = myRarityInfor.subFrame;
                subFrameLine.effectColor = myRarityInfor.colorRarity;
                subFrame.gameObject.SetActive(true);
            }
            else
            {
                subFrame.gameObject.SetActive(false);
            }
            //itemTypeWrap.sprite = myRarityInfor.sprSlot;
            itemTypeWrap.color = myRarityInfor.colorRarity;
        }
    }

    public void ReloadLevel()
    {
        if(ownedOutfit != null)
        {
            txtLevel.text = ownedOutfit.level + "/" + myRarityInfor.maxLevel;
        }
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
            if(i < starCount)
                starsPlus[i].SetActive(true);
            else
                starsPlus[i].SetActive(false);
        }
    }

    public virtual void UpdateData() {
        ownedOutfit = ProfileManager.Instance.playerData.outfitSave.GetOwnedOutfit(ownedOutfitID);
        InitData(ownedOutfit);
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (ownedOutfitID == -1)
            return;
        PanelOutfit.Instance.OutfitOnClick(ownedOutfitID);
    }

    public virtual void DeSelectOutfit()
    {
        ownedOutfitID = -1;
    }
}
