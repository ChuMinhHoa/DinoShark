using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

public class Customer : Charater
{
    [SerializeField] float baseSpeed;
    bool ordered;
    bool served;
    bool doneMeal;
    public OrderProduct orderProduct;
    [SerializeField] GameObject orderUI;
    [SerializeField] Image productUI;
    [SerializeField] Text amountUI;
    [SerializeField] NavMeshObstacle obstacle;
    ServingPosition servingPosition;
    bool floorReached;
    public void OnInit()
    {
        Material sharkMat = ProfileManager.Instance.dataConfig.skinDataConfig.GetRandomMaterial();
        Material faceMaterial = ProfileManager.Instance.dataConfig.skinDataConfig.GetRandomFaceMaterial();
        ordered = false;
        orderUI.SetActive(false);
        if (sharkMat != null)
        {
            Material[] mats = meshs[0].materials;
            mats[0] = sharkMat;
            for (int i = 0; i < meshs.Count; i++)
            {
                //meshs[i].materials[0] = sharkMat;
                meshs[i].materials = mats;
            }
        }
        if (faceMaterial != null)
        {
            Material[] mats = new Material[1];
            mats[0] = faceMaterial;
            face.materials = mats;
        }
        UpdateSpeed();
    }

    public void UpdateSpeed()
    {
        float merchandiseSpeed = ProfileManager.Instance.playerData.merchandiseSave.GetCustomerSpeed();

        agent.speed = baseSpeed * merchandiseSpeed;
    }

    public void CallCustomerMove(ServingPosition position, Transform elevetor = null)
    {
        served = false;
        doneMeal = false;
        ordered = false;
        servingPosition = position;
        if(elevetor != null)
        {
            toMovePosition = elevetor;
            floorReached = false;
        }
        else
        {
            toMovePosition = servingPosition.position;
            floorReached = true;
        }
        StateMachine.ChangeState(charaterMoveState);
    }

    UnityAction customerToTable;
    public void CustomerStartOrder(UnityAction order)
    {
        customerToTable = order;
        int amount = Random.Range(2, 4);
        orderProduct.orderingAmount = amount;
        orderProduct.takenAmount = amount;
        amountUI.text = orderProduct.takenAmount.ToString();
        // Customer random order
        FoodID curFoodID = ProfileManager.Instance.playerData.GetMenuSave().dataFoodSave.GetRandomFood();
        orderProduct.foodID = curFoodID;
        productUI.sprite = ProfileManager.Instance.dataConfig.menuDataConfig.GetFoodSprByID(ProfileManager.Instance.playerData.currentLevel ,curFoodID);
    }

    public void CustomerDoneOrder()
    {
        if (!ProfileManager.Instance.playerData.GetMenuSave().dataFoodSave.CheckHaveMachineUnlock() || !ordered)
        {
            return;
        }
        orderUI.SetActive(true);
    }

    public void UpdateRemainAmount()
    {
        amountUI.text = orderProduct.takenAmount.ToString();
    }

    public void CustomerGetAllServe()
    {
        orderUI.SetActive(false);
        if (servingPosition.floor == 0 && !servingPosition.useInPlace)  // Customers  take away food
        {
            CustomerDoneActive();
        }
        else // Customers have meal at the restaurant
        {
            if (m_Animator) m_Animator.SetBool(ONEAT, true);
            StateMachine.ChangeState(charaterDoWorkState);
        }
    }

    public void CustomerDoneActive()
    {
        servingPosition.FreeTakenSit();
        orderUI.SetActive(false);
        if(servingPosition.floor == 0)
        {
            toMovePosition = GameManager.Instance.GetOutPosition();
            floorReached = true;
        }
        else
        {
            toMovePosition = GameManager.Instance.lobbyManager.GetElevetor(servingPosition.floor);
            floorReached = false;
        }
        served = true;
        doneMeal = true;
        GameManager.Instance.lobbyManager.CallCustomer();
        StateMachine.ChangeState(charaterMoveState);
    }

    public override void OnMoveExecute()
    {
        base.OnMoveExecute();
        if (IsFinishMoveOnNavemesh())
        {
            if(!doneMeal)
            {
                if (floorReached)
                {
                    if (!served)
                    {
                        ordered = true;
                        customerToTable();
                    }
                    else
                    {
                        gameObject.SetActive(false);
                    }
                }  
            }
            else
            {
                if (floorReached)
                {
                    if (served)
                    {
                        gameObject.SetActive(false);
                    }
                }
            }
            StateMachine.ChangeState(charaterIdleState);
        }
    }
    public override void OnMoveExit()
    {
        base.OnMoveExit(); 
    }

    public override void OnIdleStart()
    {
        base.OnIdleStart();
        transform.localEulerAngles = toMovePosition.eulerAngles;
        agent.enabled = false;
        if(floorReached && !doneMeal)
        {
            transform.position = toMovePosition.position;
            obstacle.enabled = true;
        }
        else if(!floorReached && !doneMeal)
        {
            //transform.position = GameManager.Instance.lobbyManager.GetElevetor(servingPosition.floor).position;
            transform.DOMove(GameManager.Instance.lobbyManager.GetElevetor(servingPosition.floor).position, 2f).OnComplete(() => {
                CallCustomerMove(servingPosition);
                floorReached = true;
            });  
        }
        else if (!floorReached && doneMeal)
        {
            //transform.position = GameManager.Instance.lobbyManager.GetElevetor(0).position; 
            transform.DOMove(GameManager.Instance.lobbyManager.GetElevetor(0).position, 2f).OnComplete(() => {
                toMovePosition = GameManager.Instance.GetOutPosition();
                StateMachine.ChangeState(charaterMoveState);
                floorReached = true;
            });
        }
    }

    public override void OnIdleExit()
    {
        base.OnIdleExit();
        obstacle.enabled = false;
        //agent.enabled = true;
    }



    float mealTime = 5;
    public override void OnDoWorkStart()
    {
        base.OnDoWorkStart();
        obstacle.enabled = true;
        timeIndicator.InitTime(mealTime);
    }
    public override void OnDoWorkExecute()
    {
        base.OnDoWorkExecute();
        if (timeIndicator.IsFinish())
        {
            CustomerDoneActive();
        }
    }
    public override void OnDoWorkExit()
    {
        base.OnDoWorkExit();
        obstacle.enabled = false;
    }
}
