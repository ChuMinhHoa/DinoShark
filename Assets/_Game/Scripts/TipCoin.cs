using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TipCoin : MonoBehaviour
{
    [SerializeField] Transform coinPos;
    [SerializeField] Text tipAmountUI;
    BigNumber tipAmount = new BigNumber();

    public void Init(UnityAction action)
    {
        GetTip += action;
        tipAmount = new BigNumber();
    }
    public void SetTip(BigNumber amount)
    {
        tipAmount.Add(amount);
        tipAmount = tipAmount * ProfileManager.Instance.playerData.merchandiseSave.GetTipValue();
        tipAmountUI.text = tipAmount.ToString();
    }
    void OnMouseDown()
    {
        AddTip();
    }

    UnityAction GetTip;

    void AddTip()
    {
        GetTip();
        ProfileManager.Instance.playerData.AddMoney(tipAmount);
        GameManager.Instance.pooling.GetCoinEffect(coinPos.position);
        GameManager.Instance.pooling.FreeCoin(this);
    }
}
