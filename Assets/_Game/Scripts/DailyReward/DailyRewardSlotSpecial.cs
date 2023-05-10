using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardSlotSpecial : DailyRewardSlot
{
    [SerializeField] Button btnShowOutfitInfor;
    [SerializeField] Image imgBGIcon;
    [SerializeField] Image imgBGType;
    [SerializeField] Image imgType;
    [SerializeField] RawImage rawImage;
    public override void Start()
    {
        base.Start();
        btnShowOutfitInfor.onClick.AddListener(ShowOutfitInfor);
    }
    DailyReward dailyReward;
    RarityInfo rarityInfor;
    MyRaw myRaw;
    public override void InitData(DailyReward data)
    {
        base.InitData(data);
        rarityInfor = ProfileManager.Instance.dataConfig.outfitDataConfig.GetRarityInfo(data.itemEquips[0].rarity);
        imgType.sprite = ProfileManager.Instance.dataConfig.outfitDataConfig.GetSpriteOfType(data.itemEquips[0].outfitType);
        imgBGType.color = rarityInfor.colorRarity;
        imgBGIcon.sprite = rarityInfor.sprSlot;
        myRaw = RawImageManager.Instance.GetMyRaw(RawType.RawDaily, 0);
        rawImage.texture = myRaw.rawSkin;
        txtAmout.text = "+" + rarityInfor.increaseBoot + "%";
        myRaw.equipOutfit.Equip(data.itemEquips[0].itemId, data.itemEquips[0].outfitType);
    }
    void ShowOutfitInfor() {
        dailyReward = ProfileManager.Instance.dataConfig.dailyDataconfig.GetDailyRewardByDay(day);
        UIManager.instance.ShowPanelOutfitDetail(dailyReward.itemEquips[0]);
    }
    public override void LockMode(bool timeShow)
    {
        base.LockMode(timeShow);
    }
}
