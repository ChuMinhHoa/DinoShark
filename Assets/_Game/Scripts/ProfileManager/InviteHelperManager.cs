using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
[System.Serializable]
public class InviteOutfitRandom {
    public int outfitID;
    public Rarity rarity;
    public int level;
    public int totalBoot;
    public OutfitType outfitType;
}

public class InviteHelperManager : MonoBehaviour
{
    public static InviteHelperManager Instance;
    OutfitDataConfig outfitDataConfig;
    List<OwnedOutfit> ownedOutfits;

    public List<InviteOutfitRandom> hatOutfitRandom = new List<InviteOutfitRandom>();
    public List<InviteOutfitRandom> clothesOutfitRandom = new List<InviteOutfitRandom>();
    public List<InviteOutfitRandom> toolOutfitRandom = new List<InviteOutfitRandom>();

    public bool isOnInvited;
    DateTime timeDoneInvite;
    PlayerData playerData;
    float timeRemain;

    PanelTotal panelTotal;
    private void Awake()
    {
        Instance = this;
        playerData = ProfileManager.Instance.playerData;
        panelTotal = UIManager.instance.GetTotalPanel();
    }

    public void InitData()
    {
        outfitDataConfig = ProfileManager.Instance.dataConfig.outfitDataConfig;
        ownedOutfits = playerData.outfitSave.ownedOutfitLists.Where(e => e.onUsing).ToList();
        CreateRandomOutfit();
    }

    OwnedOutfit ownedOutfit;
    void CreateRandomOutfit() {
        for (int i = 0; i < 4; i++)
        {
            InviteOutfitRandom newHatOutfit = new InviteOutfitRandom();
            newHatOutfit.outfitID = outfitDataConfig.GetRamdomOutfitID(OutfitType.Hat);
            ownedOutfit = GetOwnedOutfitOnUsing(OutfitType.Hat);
            newHatOutfit.rarity = RandomRarity();
            newHatOutfit.level = RandomLevel();
            newHatOutfit.outfitType = OutfitType.Hat;
            if (i >= hatOutfitRandom.Count)
                hatOutfitRandom.Add(newHatOutfit);
            else hatOutfitRandom[i] = newHatOutfit;

            InviteOutfitRandom newClothesOutfit = new InviteOutfitRandom();
            newClothesOutfit.outfitID = outfitDataConfig.GetRamdomOutfitID(OutfitType.Clothes);
            ownedOutfit = GetOwnedOutfitOnUsing(OutfitType.Clothes);
            newClothesOutfit.rarity = RandomRarity();
            newClothesOutfit.level = RandomLevel();
            newClothesOutfit.outfitType = OutfitType.Clothes;
            if (i >= clothesOutfitRandom.Count)
                clothesOutfitRandom.Add(newClothesOutfit);
            else clothesOutfitRandom[i] = newClothesOutfit;

            InviteOutfitRandom newToolOutfit = new InviteOutfitRandom();
            newToolOutfit.outfitID = outfitDataConfig.GetRamdomOutfitID(OutfitType.Tool);
            ownedOutfit = GetOwnedOutfitOnUsing(OutfitType.Tool);
            newToolOutfit.rarity = RandomRarity();
            newToolOutfit.level = RandomLevel();
            newToolOutfit.outfitType = OutfitType.Tool;
            if (i >= toolOutfitRandom.Count)
                toolOutfitRandom.Add(newToolOutfit);
            else toolOutfitRandom[i] = newToolOutfit;
        }
    }

    Rarity RandomRarity() {
        int rarityIndex = 0;
        if (ownedOutfit != null)
            rarityIndex = (int)ownedOutfit.rarity;
        rarityIndex += UnityEngine.Random.Range(-1, 2);
        if (rarityIndex > (int)Rarity.Mystic)
            return Rarity.Mystic;
        else if(rarityIndex < (int)Rarity.Common)
            return Rarity.Common;
        return (Rarity)rarityIndex;
    }

    int RandomLevel() {
        int levelReturn;
        if (ownedOutfit == null)
            return 1;
        levelReturn = ownedOutfit.level + UnityEngine.Random.Range(-1, 3);
        if (levelReturn < 1)
            return 1;
        return levelReturn;
    }

    OwnedOutfit GetOwnedOutfitOnUsing(OutfitType outfitType) {
        for (int i = 0; i < ownedOutfits.Count; i++)
        {
            if (ownedOutfits[i].outfitType == outfitType && ownedOutfits[i].onUsing)
                return ownedOutfits[i];
        }
        return null;
    }

    private void Update()
    {
        if (isOnInvited)
        {
            timeRemain = (float)timeDoneInvite.Subtract(DateTime.Now).TotalSeconds;
            if (timeRemain <= 0)
            { 
                OnNormalMode();
                return;
            }
            panelTotal.ChangeTimeInvitedText(GetTimeRemainToString());
        }
    }

    public void OnInvited() { 
        isOnInvited = true;
        timeDoneInvite = DateTime.Parse(playerData.GetInviteSave().timeDoneCooldown);
        if (panelTotal != null)
            panelTotal.ShowTimeInvited();
    }

    public void OnNormalMode() { 
        isOnInvited = false;
        if (panelTotal != null)
            panelTotal.CloseTimeInvited();
        //Turn off staff invite
    }

    public string GetTimeRemainToString() {
        return TimeUtil.TimeToString(timeRemain);
    }

    public float GetTimeRemain() { return timeRemain; }
}
