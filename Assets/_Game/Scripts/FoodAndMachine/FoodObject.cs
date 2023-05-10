using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodObject : MonoBehaviour
{
    public FoodID foodID;
    [SerializeField] Text priceTxt;
    [SerializeField] MeshFilter mesh;
    BigNumber price=new BigNumber();
    [SerializeField] GameObject glowEffect;
    public ServingPosition servingPosition;

    public void InitFood(FoodID id, Mesh FoodModel)
    {
        mesh.mesh = FoodModel;
    }

    public void InitFoodInfo(ServingPosition serving, BigNumber inPrice, bool perfect = false)
    {
        ActivePriceText(true);
        servingPosition = serving;
        price = new BigNumber(inPrice);
        if (perfect)
        {
            price = price * 2;
            glowEffect.SetActive(true);
        }
        else
            glowEffect.SetActive(false);

        priceTxt.text = price.ToString();
        
    }
    public void OnDelivery(int floor, Transform startFloor)
    {
        transform.parent = startFloor;
        transform.position = startFloor.position;
        ActivePriceText(false);
        transform.DOMove(GameManager.Instance.lobbyManager.GetFoodElevetor(floor).position, floor * 1.5f).OnComplete(() => {
            GameManager.Instance.lobbyManager.EnqueueFood(floor, this);
        });
    }

    BigNumber tipPrice;
    public void OnServe()
    {
        tipPrice = new BigNumber(price);
        ProfileManager.Instance.playerData.AddMoney(price);
        GameManager.Instance.pooling.GetCoinEffect(servingPosition.pointSpawn.position);
        GameManager.Instance.pooling.ReturnFood(this);
        servingPosition.orderProducts[0].takenAmount--;
        servingPosition.GetCustomer().UpdateRemainAmount();
        if (servingPosition.orderProducts[0].takenAmount <= 0)
        {
            servingPosition.FreeSit();
        }
        if (ProfileManager.Instance.playerData.merchandiseSave.AbleToGetTip())
        {
            servingPosition.GetTipCoin(tipPrice);
        }
    }

    public void ActivePriceText(bool a)
    {
        priceTxt.gameObject.SetActive(a);
    }
}
