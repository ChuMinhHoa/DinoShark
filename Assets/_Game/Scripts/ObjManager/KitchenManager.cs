using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenManager : MonoBehaviour
{
    public List<Transform> staffPos;
    public List<Staff> staffs;
    public List<FoodController> foodControllers;
    public Transform invitePos;

    [SerializeField] List<FreePosition> freePos;
    PlayerData playerData;
    Staff myCheff;
    public Staff invitedStaff;

    public void UpdateStaffSpeed()
    {
        float merchandiseSpeed = ProfileManager.Instance.playerData.merchandiseSave.GetStaffSpeed();
        float upgradeSpeed = ProfileManager.Instance.playerData.GetUpgradeSave().GetUpgradeSpeed();
        if(staffs.Count > 1)
        {
            for (int i = 1; i < staffs.Count; i++)
            {
                if(staffs[i] != invitedStaff)
                    staffs[i].UpdateSpeed(merchandiseSpeed * upgradeSpeed);
            }
        }
        float outfitSpeed = ProfileManager.Instance.playerData.outfitSave.GetOufitsSpeedBoost();
        if(myCheff) 
            myCheff.UpdateSpeed(merchandiseSpeed * upgradeSpeed * outfitSpeed);
    }

    public void InitStaff()
    {
        if (playerData == null)
            playerData = ProfileManager.Instance.playerData;
        int staffAmount = playerData.GetUpgradeSave().GetTotalStaff();
        for (int i = 0; i < staffAmount; i++)
        {
            if (i == 0)
            {
                myCheff = SpawnNewStaff();
                myCheff.ChangeOutfit();
                staffs.Add(myCheff);
            }
            else staffs.Add(SpawnNewStaff());
        }
        UpdateStaffSpeed();
        if(ProfileManager.Instance.playerData.inviteSave.timeRemain > 0)
        {
            SpawnInvitedStaff();
        }
    }
    int indexSpawn = 0;
    public Staff SpawnNewStaff(bool firstLoad = true)
    {
        float merchandiseSpeed = ProfileManager.Instance.playerData.merchandiseSave.GetStaffSpeed();
        float upgradeSpeed = ProfileManager.Instance.playerData.GetUpgradeSave().GetUpgradeSpeed();
        Staff staff = GameManager.Instance.pooling.GetStaff();
        staff.UpdateSpeed(merchandiseSpeed * upgradeSpeed);
        if (staff != null)
        {
            staff.transform.position = staffPos[indexSpawn].position;
            staff.gameObject.SetActive(true);
            staff.Init();
            if (!firstLoad)
                staff.EggMode();
            indexSpawn++;
        }
        GameManager.Instance.lobbyManager.CallFreeChefDoWork();
        return staff;
    }

    public void AddStaff(Staff staff) { staffs.Add(staff); }

    public Staff SpawnInvitedStaff()
    {
        if(!invitePos)
        {
            return null;
        }
        if(!invitedStaff)
        {
            invitedStaff = GameManager.Instance.pooling.GetStaff();
        }
        invitedStaff.gameObject.SetActive(true);
        invitedStaff.transform.position = invitePos.position;
        invitedStaff.Init();
        invitedStaff.StaffOnInvite(() => { staffs.Add(invitedStaff); });
        invitedStaff.ChangeOutfitInvite();
        float merchandiseSpeed = ProfileManager.Instance.playerData.merchandiseSave.GetStaffSpeed();
        float upgradeSpeed = ProfileManager.Instance.playerData.GetUpgradeSave().GetUpgradeSpeed();
        invitedStaff.UpdateSpeed(merchandiseSpeed * upgradeSpeed * 1.5f);

        //GameManager.Instance.lobbyManager.CallFreeChefDoWork();
        return invitedStaff;
    }
    public void RemoveInvitedStaff()
    {
        staffs.Remove(invitedStaff);
        if(invitedStaff.isFree)
        {
            invitedStaff.InviteTimeout();
        }
    }

    public bool CheckIfMyChef(Staff checkStaff)
    {
        if(checkStaff == myCheff)
        {
            return true;
        }
        return false;
    }

    public void ChangeOutfitMyCheff(int outfitSaveID = -1) {
        OwnedOutfit ownedOutfit = playerData.outfitSave.GetOwnedOutfit(outfitSaveID);
        myCheff.ChangeOutfit(ownedOutfit);
    }

    public void TakeoffOutfitMyChef(OutfitType type)
    {
        myCheff.TakeoffOutfit(type);
    }

    public void CallStaffToTakeOrder(ServingPosition serving = null)
    {
        for(int i = 0; i < staffs.Count; i++)
        {
            if(staffs[i].isFree)
            {
                if(serving != null)
                {
                    staffs[i].CallStaffToTakeOrder(serving);
                }
                else
                {

                }
                return;
            }
        }
    }
    public void CallStaffToCook(ServingPosition serving = null)
    {
        for (int i = 0; i < staffs.Count; i++)
        {
            if (staffs[i].isFree)
            {
                if (serving != null)
                {
                    Machine machine = GetFreeMachine(serving.orderProducts[0].foodID);
                    if (machine != null)
                    {
                        staffs[i].CallStaffToCook(serving, machine);
                    }
                }
                return;
            }
        }
    }

    public Machine GetFreeMachine(FoodID foodID)
    {
        for (int i = 0; i < foodControllers.Count; i++)
        {
            if(foodControllers[i].foodBase.foodID == foodID)
            {
                return foodControllers[i].GetMachineAble();
            }
        }
        return null;
    }

    public FreePosition GetFreePlace()
    {
        for (int i = 0; i < freePos.Count; i++)
        {
            if(!freePos[i].isTaken)
            {
                freePos[i].isTaken = true;
                return freePos[i];
            }
        }
        return freePos[0];
    }
    public FoodController GetFoodController(FoodID foodID) {
        for (int i = 0; i < foodControllers.Count; i++)
        {
            if (foodControllers[i].foodBase.foodID == foodID)
                return foodControllers[i];
        }
        return null;
    }

    public void InitData()
    {
        EventManager.AddListener(EventName.ChangeMoney.ToString(), CheckFoodController);
        for (int i = 0; i < foodControllers.Count; i++)
        {
            foodControllers[i].SetDefaultProperty(ProfileManager.Instance.dataConfig.menuDataConfig.foodDefaultProperties[i]);
            foodControllers[i].InitData(this);
        }
        CheckFoodController();
    }

    public void CheckFoodController()
    {
        for (int i = 0; i < foodControllers.Count; i++)
        {
            foodControllers[i].CheckIsEnoughMoney();
        }
    }

}

[System.Serializable]
public class FreePosition
{
    public bool isTaken;
    public Transform position;
}
