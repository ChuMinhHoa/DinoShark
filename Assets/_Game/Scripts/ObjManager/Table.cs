using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[System.Serializable] 
public class OrderProduct
{
    //public ProductName productName;
    public FoodID foodID;
    public int orderingAmount;
    public int takenAmount;
}

[System.Serializable] 
public class ServingPosition
{
    public int floor;
    public bool useInPlace;
    public Transform position;
    public Transform takeOrderPos;
    public Transform pointSpawn;
    public bool taken;
    public bool ordered;
    public bool ordering;
    Customer customer;
    public List<OrderProduct> orderProducts;
    TipCoin tipCoin;

    public void SetCustomer(Customer customerIn)
    {
        customer = customerIn;
        taken = true;
    }
    public Customer GetCustomer()
    {
        return customer;
    }
    public void GetOrder()
    {
        ordered = true;
    }
    public void FreeTipCoin()
    {
        tipCoin = null;
    }
    public void GetTipCoin(BigNumber profit)
    {
        if(tipCoin == null)
        {
            if(pointSpawn.childCount > 0)
            {
                if(pointSpawn.GetChild(0).GetComponent<TipCoin>())
                {
                    tipCoin = pointSpawn.GetChild(0).GetComponent<TipCoin>();
                }
            }
            else
            {
                tipCoin = GameManager.Instance.pooling.GetCoin(pointSpawn.position);
                tipCoin.transform.parent = pointSpawn;
            }
            tipCoin.Init(FreeTipCoin);
            tipCoin.SetTip(profit);
        }
        else
        {
            tipCoin.SetTip(profit);
        }
    }
    public void FreeSit()
    {
        //taken = false;
        //ordered = false;
        //ordering = false;
        if(customer != null)
        {
            customer.CustomerGetAllServe();
        }
        //GameManager.Instance.lobbyManager.orderingPositions.Remove(this);
    }

    public void FreeTakenSit()
    {
        taken = false;
        ordered = false;
        ordering = false;
        //if (customer != null)
        //{
        //    customer.CustomerOutRestaurant();
        //}
        GameManager.Instance.lobbyManager.orderingPositions.Remove(this);
    }
}

public class Table : MonoBehaviour
{
    public List<ServingPosition> servingPositions;
    [SerializeField] GameObject deliveryBox;
    [SerializeField] GameObject unlockSmoke;
    [SerializeField] GameObject tableModel;
    [SerializeField] Collider myCollider;

    Vector3 pointTarget;
    public void InitTable()
    {
        //Do move
        if (myCollider != null)
            myCollider.enabled = false;
        List<ServingPosition> lobbyPos = GameManager.Instance.lobbyManager.servingPositions;
        for (int i = 0; i < servingPositions.Count; i++)
        {
            lobbyPos.Add(servingPositions[i]);
        }
    }
    public void DisableTable()
    {
        gameObject.SetActive(false);
    }

    public void TableOnDelivery()
    {
        gameObject.SetActive(true);
        deliveryBox.SetActive(true);
        unlockSmoke.SetActive(false);
        tableModel.SetActive(false);
    }

    public void OnUnbox()
    {
        deliveryBox.SetActive(false);
        unlockSmoke.SetActive(true);
        tableModel.SetActive(true);
        
        InitTable();
        GameManager.Instance.lobbyManager.CallCustomer(servingPositions.Count);
    }

    public void MoveObject() {
        pointTarget = transform.position;
        pointTarget.y += 10f;
        transform.position = pointTarget;
        pointTarget.y -= 10f;
        myCollider.enabled = false;
        transform.DOMove(pointTarget, 1f).OnComplete(()=> {
            myCollider.enabled = true;
            TutorialManager.Instance.TableUnlockMoveDone();
        });
    }

    void OnMouseDown()
    {
        OnUnbox();
    }
    //void OnTouchDown()
    //{
    //    OnUnbox();
    //}
}
