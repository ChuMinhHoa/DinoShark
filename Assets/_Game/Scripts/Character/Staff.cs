using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UI;

public class Staff : Charater
{
    [SerializeField] float baseSpeed;
    public bool isFree = true;
    ServingPosition servingPosition;
    [SerializeField] Text foodPrice;
    string ONCOOK = "OnCook";
    string NONEPRICE = "";
    bool takeOrder;
    public float takeOrderTime = 1.6f;
    Machine machine;
    bool cook;
    public float cookTime = 2;
    bool doneCook;
    [SerializeField] Transform holdDishPos;
    public FreePosition freePosition;
    public EquipOutfit equipOutfit;
    
    public void Init()
    {
        isFree = true;
        takeOrder = false;
        cook = false;
        doneCook = false;
        foodPrice.text = NONEPRICE;
        timeIndicator.Init();
        if (this == GameManager.Instance.kitchenManager.invitedStaff)
            takeOrderTime = 1.2f;
    }

    public void UpdateSpeed(float speedBoost)
    {
        agent.speed = baseSpeed * speedBoost;
    }

    public void CallStaffToTakeOrder(ServingPosition serving)
    {
        isFree = false;
        takeOrder = true;
        cook = false;
        doneCook = false;
        servingPosition = serving;
        toMovePosition = serving.takeOrderPos;
        servingPosition.ordering = true;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        StateMachine.ChangeState(charaterMoveState);
        LeaveFreePos();
    }

    void DoneTakeOrder()
    {
        servingPosition.ordered = true;
        SoundManager.instance.PlaySoundEffect(SoundID.MAIN_ORDER);
        servingPosition.GetCustomer().CustomerDoneOrder();
        GetBackInFreeQueue();
        if (TutorialManager.Instance.levelHaveTutOrder && !TutorialManager.Instance.isDoneOrder)
            CameraController.Instance.CameraZoomOut(MoveBackCamera);
        GameManager.Instance.lobbyManager.CallFreeChefDoWork();
    }

    void MoveBackCamera() { CameraController.Instance.MoveBackToDefault(ChangeTutorialDoneOrder); }

    void ChangeTutorialDoneOrder() {
        TutorialManager.Instance.ChangeDoneOrder(true);
    }

    public void CallStaffToCook(ServingPosition serving, Machine machineIn)
    {
        machine = machineIn;
        if (machine != null)
        {
            servingPosition = serving;
            isFree = false;
            machine.UsingMachine();
            toMovePosition = machine.GetPointUsingMachine();
            servingPosition.orderProducts[0].orderingAmount--;
            StateMachine.ChangeState(charaterMoveState);
            takeOrder = false;
            cook = true;
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
            LeaveFreePos();
        }
        else
        {
            GetBackInFreeQueue();
        }
    }
    FoodObject holdingFood;
    FoodController foodController;
    BigNumber servingDishProfit;
    void DoneCook()
    {
        machine.DoneUseMachine();
        doneCook = true;
        SoundManager.instance.PlaySoundEffect(SoundID.MAIN_COOK);
        holdingFood = GameManager.Instance.pooling.GetFoodByFoodID(servingPosition.orderProducts[0].foodID);
        if (holdingFood != null)
        {
            holdingFood.gameObject.SetActive(true);
            holdingFood.transform.position = holdDishPos.position;
            holdingFood.transform.parent = holdDishPos;
        }
        machine.ResetMachine();
        if (servingPosition.floor == 0)
        {
            if (GameManager.Instance.lobbyManager.waiterIn0Floor != 0)
                toMovePosition = GameManager.Instance.lobbyManager.GetFoodElevetor(0);
            else
                toMovePosition = servingPosition.takeOrderPos;
        }
        else
        {
            toMovePosition = GameManager.Instance.lobbyManager.GetFoodElevetor(0);
        }
        StateMachine.ChangeState(charaterMoveState);
        servingDishProfit = foodController.GetFoodProfit();
        if (GameManager.Instance.kitchenManager.CheckIfMyChef(this))
        {
            servingDishProfit *= ProfileManager.Instance.playerData.outfitSave.GetMysticBoost();
            //holdingFood.InitFoodInfo(servingPosition, servingDishProfit, ProfileManager.Instance.playerData.outfitSave.MakeItPerfect());
        }
        //else
        //    holdingFood.InitFoodInfo(servingPosition, servingDishProfit);
        holdingFood.InitFoodInfo(servingPosition, servingDishProfit, ProfileManager.Instance.playerData.outfitSave.MakeItPerfect());
        GameManager.Instance.lobbyManager.CallFreeChefDoWork();
    }

    void DoneServe()
    {
        GetBackInFreeQueue();
        //foodPrice.text = NONEPRICE;
        if (servingPosition.floor == 0)
        {
            if (GameManager.Instance.lobbyManager.waiterIn0Floor != 0)
                holdingFood.OnDelivery(servingPosition.floor, GameManager.Instance.lobbyManager.GetFoodElevetor(0));
            else
                holdingFood.OnServe();
        }
        else
        {
            holdingFood.OnDelivery(servingPosition.floor, GameManager.Instance.lobbyManager.GetFoodElevetor(0));
        }
        GameManager.Instance.lobbyManager.CallFreeChefDoWork();
    }

    void GetBackInFreeQueue()
    {
        takeOrder = false;
        cook = false;
        doneCook = false;
        isFree = true;
        if (GameManager.Instance.kitchenManager.invitedStaff == this)
        {
            if (!GameManager.Instance.kitchenManager.staffs.Contains(this))
            {
                InviteTimeout();
                return;
            }
        }
        LeaveFreePos();
        GoToFreePlace();
    }

    void GoToFreePlace()
    {
        freePosition = GameManager.Instance.kitchenManager.GetFreePlace();
        toMovePosition = freePosition.position;
        StateMachine.ChangeState(charaterMoveState);
    }
    void LeaveFreePos()
    {
        if (freePosition != null)
        {
            freePosition.isTaken = false;
            freePosition = null;
        }
        ResetAct();
    }

    void StaffActing()
    {
        var values = Enum.GetValues(typeof(CharIdleAct));
        CharIdleAct act = (CharIdleAct)UnityEngine.Random.Range(0, values.Length);
        if (m_Animator) m_Animator.SetBool(act.ToString(), true);
    }

    void ResetAct()
    {
        if (m_Animator)
        {
            var values = Enum.GetValues(typeof(CharIdleAct));
            for (int i = 0; i < values.Length; i++)
            {
                m_Animator.SetBool(((CharIdleAct)i).ToString(), false);
            }
        }

    }

    public override void OnMoveExecute()
    {
        base.OnMoveExecute();
        if (IsFinishMoveOnNavemesh())
        {
            if (!isFree)
            {
                if (!doneCook)
                {
                    StateMachine.ChangeState(charaterDoWorkState);
                }
                else
                {
                    GetBackInFreeQueue();
                    DoneServe();
                }
            }
            else
            {
                StateMachine.ChangeState(charaterIdleState);
            }
        }
    }

    Vector3 lookAtCamera = new Vector3(0, 180, 0);
    public override void OnMoveExit()
    {
        base.OnMoveExit();
        if (!isFree)
        {
            transform.localEulerAngles = toMovePosition.eulerAngles;
        }
        else
        {
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            transform.localEulerAngles = lookAtCamera;
        }
    }

    public override void OnDoWorkStart()
    {
        base.OnDoWorkStart();
        if (takeOrder && !cook)
        {
            timeIndicator.InitTime(takeOrderTime);
            if (TutorialManager.Instance.levelHaveTutOrder && !TutorialManager.Instance.isDoneOrder)
            {
                CameraController.Instance.CameraMoveTarget(transform, new Vector3(0, 1.5f, 0), ZoomInCameraOntutorial);
            }
        }
        else if (!takeOrder && cook)
        {
            foodController = GameManager.Instance.kitchenManager.GetFoodController(servingPosition.orderProducts[0].foodID);
            cookTime = foodController.GetTimeMakeFood();
            if (this == GameManager.Instance.kitchenManager.invitedStaff)
                cookTime = cookTime * 0.8f;

            if (GameManager.Instance.kitchenManager.CheckIfMyChef(this)) {
                cookTime *= ProfileManager.Instance.playerData.outfitSave.GetOufitsProductionTimeRemain(); 
            }

            cookTime *= ProfileManager.Instance.playerData.outfitSave.GetOufitsAllProductionTimeRemain();
            if (ProfileManager.Instance.playerData.outfitSave.MakeItInstant())
                cookTime = 0;
            if (m_Animator) m_Animator.SetBool(ONCOOK, true);
            timeIndicator.InitTime(cookTime);
            machine.UseMachine();
        }
    }

    void ZoomInCameraOntutorial() { CameraController.Instance.CameraZoomIn(5f); }

    public override void OnDoWorkExecute()
    {
        base.OnDoWorkExecute();
        if (takeOrder && !cook)
        {
            // Staff come and take order from customer
            if (timeIndicator.IsFinish())
            {
                DoneTakeOrder();
            }
        }
        else if (!takeOrder && cook)
        {
            // Staff cooking food in the kitchen
            if (timeIndicator.IsFinish())
            {
                if (m_Animator) m_Animator.SetBool(ONCOOK, false);
                DoneCook();
            }
        }
    }

    public override void OnIdleStart()
    {
        base.OnIdleStart();
        StaffActing();
    }
    public override void OnIdleExecute()
    {
        base.OnIdleExecute();
        transform.localEulerAngles = lookAtCamera;
    }

    public void ChangeOutfit()
    {
        List<OwnedOutfit> ownedOutfits = ProfileManager.Instance.playerData.outfitSave.GetOwnedOutfits();
        equipOutfit.InitOutfit();
        for (int i = 0; i < ownedOutfits.Count; i++)
        {
            if (ownedOutfits[i].onUsing)
                equipOutfit.Equip(ownedOutfits[i].outfitID, ownedOutfits[i].outfitType);
        }
    }

    public void ChangeOutfitInvite() {
        equipOutfit.InitOutfit();
        InviteOutfitData inviteOutfitData = ProfileManager.Instance.playerData.GetInviteSave().GetInviteOutfitData(OutfitType.Hat);
        equipOutfit.Equip(inviteOutfitData.outfitID, inviteOutfitData.outfitType);

        inviteOutfitData = ProfileManager.Instance.playerData.GetInviteSave().GetInviteOutfitData(OutfitType.Clothes);
        equipOutfit.Equip(inviteOutfitData.outfitID, inviteOutfitData.outfitType);

        inviteOutfitData = ProfileManager.Instance.playerData.GetInviteSave().GetInviteOutfitData(OutfitType.Tool);
        equipOutfit.Equip(inviteOutfitData.outfitID, inviteOutfitData.outfitType);
    }

    public void ChangeOutfit(OwnedOutfit ownedOutfit) {
        equipOutfit.Equip(ownedOutfit.outfitID, ownedOutfit.outfitType);
    }

    public void TakeoffOutfit(OutfitType type)
    {
        equipOutfit.EquipDefault(type);
    }

    UnityAction onInvite;
    public void StaffOnInvite(UnityAction a)
    {
        onInvite = a;
        StartCoroutine(Dancin());
    }

    IEnumerator Dancin()
    {
        StaffActing();
        yield return new WaitForSeconds(5);
        onInvite();
    }

    public void InviteTimeout()
    {
        gameObject.SetActive(false);
        StateMachine.ChangeState(charaterIdleState);
    }

    public override void EggMode()
    {
        Egg eggPreb = ProfileManager.Instance.dataConfig.eggMeshDataConfig.GetEggType();
        Egg myNewEgg = Instantiate(eggPreb, eggPoint);
        myNewEgg.transform.localPosition = Vector3.zero;
        myNewEgg.SetMyCharactor(this);
        myEgg.gameObject.SetActive(true);
        smokeEffect.gameObject.SetActive(false);
        myBody.gameObject.SetActive(false);
        eggMode = true;
    }

    public override void EggBroke()
    {
        myEgg.gameObject.SetActive(false);
        smokeEffect.gameObject.SetActive(true);
        myBody.gameObject.SetActive(true);
        m_Animator.SetBool("OnHire", true);
        StartCoroutine(IE_WaitAnim());
        eggMode = false;
    }

    public override IEnumerator IE_WaitAnim() {
        yield return new WaitForSeconds(1f);
        GameManager.Instance.kitchenManager.AddStaff(this);
        m_Animator.SetBool("OnHire", false);
        GameManager.Instance.lobbyManager.CallFreeChefDoWork();
    }

}
