using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SDK;

[System.Serializable]
public class InviteOutfitData {
    public int outfitID;
    public int level;
    public Rarity rarity;
    public OutfitType outfitType;
}
[System.Serializable]
public class InviteSave : GlobalSaveDataBase
{
    public InviteOutfitData hatInvite;
    public InviteOutfitData bodyInvite;
    public InviteOutfitData toolInvite;
    public string timeDoneCooldown;
    public float timeRemain;
    public string timeEndInvite;
    public string lastInvite;

    public override void LoadData()
    {
        SetStringSave("Invite_Save");
        string jsonData = GetJsonData();
        if (!string.IsNullOrEmpty(jsonData))
        {
            InviteSave dataSave = JsonUtility.FromJson<InviteSave>(jsonData);
            timeDoneCooldown = dataSave.timeDoneCooldown;
            timeEndInvite = dataSave.timeEndInvite;
            lastInvite = dataSave.lastInvite;

            if (!HaveInviteRemain())
            {
                hatInvite = new InviteOutfitData();
                bodyInvite = new InviteOutfitData();
                toolInvite = new InviteOutfitData();
            }
            else
            {
                hatInvite = dataSave.hatInvite;
                bodyInvite = dataSave.bodyInvite;
                toolInvite = dataSave.toolInvite;
            }

            if(HaveCoolDownRemain())
            {
                InviteHelperManager.Instance.OnInvited();
            }
        }
        else
        {
            IsMarkChangeData();
            ProfileManager.Instance.playerData.SaveData();
        }
    }

    public float GetInviteRemain()
    {
        return timeRemain;
    }

    public InviteOutfitData GetInviteOutfitData(OutfitType outfitType) {
        return outfitType switch
        {
            OutfitType.Hat => hatInvite,
            OutfitType.Clothes => bodyInvite,
            OutfitType.Tool => toolInvite,
            _ => null,
        };
    }
    float coolDownTime = 5;
    float inviteTime = 5;
    
    public void OnInvite()
    {
        timeDoneCooldown = DateTime.Now.AddMinutes(coolDownTime).ToString();
        timeRemain = inviteTime * 60;
        timeEndInvite = DateTime.Now.AddMinutes(inviteTime).ToString();
        GameManager.Instance.kitchenManager.SpawnInvitedStaff();
        lastInvite = DateTime.Now.ToString();
        IsMarkChangeData();
        ABIAnalyticsManager.Instance.TrackRentShark();
    }

    public void SaveInviteOutfitData(InviteOutfitRandom inviteOutfitRandom, OutfitType outfitType) {
        switch (outfitType)
        {
            case OutfitType.Hat:
                hatInvite.outfitID = inviteOutfitRandom.outfitID;
                hatInvite.rarity = inviteOutfitRandom.rarity;
                hatInvite.level = inviteOutfitRandom.level;
                hatInvite.outfitType = outfitType;
                break;
            case OutfitType.Clothes:
                bodyInvite.outfitID = inviteOutfitRandom.outfitID;
                bodyInvite.rarity = inviteOutfitRandom.rarity;
                bodyInvite.level = inviteOutfitRandom.level;
                bodyInvite.outfitType = outfitType;
                break;
            case OutfitType.Tool:
                toolInvite.outfitID = inviteOutfitRandom.outfitID;
                toolInvite.rarity = inviteOutfitRandom.rarity;
                toolInvite.level = inviteOutfitRandom.level;
                toolInvite.outfitType = outfitType;
                break;
            default:
                break;
        }
    }

    bool HaveInviteRemain() {
        if (!String.IsNullOrEmpty(timeEndInvite))
        {
            DateTime momment = DateTime.Now;
            DateTime inviteEnd = DateTime.Parse(timeEndInvite);
            TimeSpan span = inviteEnd.Subtract(momment);
            if (span.TotalSeconds > 0)
            {
                timeRemain = (float)(span.TotalSeconds);
                return true;
            }
            else
            {
                timeRemain = 0f;
                return false;
            }
        }
        return false;
    }
    bool HaveCoolDownRemain()
    {
        if (!String.IsNullOrEmpty(timeEndInvite))
        {
            DateTime momment = DateTime.Now;
            DateTime cooldownEnd = DateTime.Parse(timeDoneCooldown);
            TimeSpan span = cooldownEnd.Subtract(momment);
            if (span.TotalSeconds > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    public void Update()
    {
        if(timeRemain > 0)
        {
            timeRemain -= Time.deltaTime;
            if(timeRemain <= 0)
            {
                // Remove invited friend
                GameManager.Instance.kitchenManager.RemoveInvitedStaff();
            }
        }
    }

    public bool CheckFreeInvite()
    {
        if (!String.IsNullOrEmpty(lastInvite))
        {
            DateTime momment = DateTime.Now;
            DateTime inviteLast = DateTime.Parse(lastInvite);
            if(momment.Date != momment.Date)
            {
                return true;
            }
            return false;
        }
        return true;
    }
}
