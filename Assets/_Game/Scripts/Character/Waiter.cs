using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Waiter : Charater
{
    public int floor;
    [SerializeField] float baseSpeed;
    public bool isFree = true;
    ServingPosition servingPosition;
    bool takeOrder;
    bool foodTaken;
    public float takeOrderTime = 2;
    [SerializeField] Transform holdDishPos;
    public FreePosition freePosition;
    public EquipOutfit equipOutfit;
    FoodObject bringingFood;

    public void Init()
    {
        isFree = true;
        takeOrder = false;
        timeIndicator.Init();
    }

    public void CallWaiterToTakeOrder(ServingPosition serving)
    {
        isFree = false;
        takeOrder = true;
        foodTaken = false;
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
        GameManager.Instance.lobbyManager.CallFreeChefDoWork();
        GameManager.Instance.lobbyManager.CallWaiterToBringFood();
    }

    public void CallWaiterToBringFood(FoodObject food)
    {
        isFree = false;
        takeOrder = false;
        foodTaken = false;
        bringingFood = food;
        //toMovePosition = GameManager.Instance.lobbyManager.GetFoodElevetor(0);
        toMovePosition = GameManager.Instance.lobbyManager.GetFoodElevetor(floor);
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        StateMachine.ChangeState(charaterMoveState);
        LeaveFreePos();
    }
    void ReachFoodElevetor()
    {
        if (bringingFood != null)
        {
            bringingFood.transform.position = holdDishPos.position;
            bringingFood.transform.parent = holdDishPos;
            toMovePosition = bringingFood.servingPosition.takeOrderPos;
            bringingFood.ActivePriceText(true);
        }
        StateMachine.ChangeState(charaterMoveState);
        foodTaken = true;
    }
    void DoneServeFood()
    {
        isFree = true;
        takeOrder = false;
        foodTaken = false;
        bringingFood.OnServe();
        GetBackInFreeQueue();
        GameManager.Instance.lobbyManager.CallFreeChefDoWork();
        GameManager.Instance.lobbyManager.CallWaiterToBringFood();
    }


    void GetBackInFreeQueue()
    {
        takeOrder = false;
        foodTaken = false;
        isFree = true;
        LeaveFreePos();
        GoToFreePlace();
    }

    void GoToFreePlace()
    {
        freePosition = GameManager.Instance.lobbyManager.GetFreePlace();
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
                if(takeOrder)
                {
                    StateMachine.ChangeState(charaterDoWorkState);
                }
                else
                {
                    if(foodTaken)
                    {
                        DoneServeFood();
                    }
                    else
                    {
                        ReachFoodElevetor();
                    }
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
        if (takeOrder)
        {
            timeIndicator.InitTime(takeOrderTime);
        }
    }

    void ZoomInCameraOntutorial() { CameraController.Instance.CameraZoomIn(5f); }

    public override void OnDoWorkExecute()
    {
        base.OnDoWorkExecute();
        if (takeOrder)
        {
            // Staff come and take order from customer
            if (timeIndicator.IsFinish())
            {
                DoneTakeOrder();
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

    public override IEnumerator IE_WaitAnim()
    {
        yield return new WaitForSeconds(1f);
        GameManager.Instance.lobbyManager.AddWaiter(this);
        m_Animator.SetBool("OnHire", false);
        GameManager.Instance.lobbyManager.CallFreeChefDoWork();
        GameManager.Instance.lobbyManager.CallWaiterToBringFood();
    }

    public void UpdateSpeed(float speedBoost)
    {
        agent.speed = baseSpeed * speedBoost;
    }
}