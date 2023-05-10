using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MoneyShowOnText : MonoBehaviour
{
    [SerializeField] Text txtMoney;
    string lastString;
    string currentString;
    bool onChangeText = false;
    [SerializeField] Vector3 vectorScale;
    [SerializeField] Vector3 vectorDefault = new Vector3(1, 1, 1);
    private void Start()
    {
        lastString = txtMoney.text;
        currentString = txtMoney.text;
        EventManager.AddListener(EventName.ChangeMoney.ToString(), OnChangeMoneyCount);
        OnChangeMoneyCount();
    }
    void OnChangeMoneyCount()
    {
        currentString = ProfileManager.Instance.playerData.GetResource().totalMoney.ToString();
        if (currentString != lastString && !onChangeText)
        {
            onChangeText = true;
            txtMoney.transform.DOScale(vectorScale, .1f).OnComplete(() => {
                txtMoney.transform.DOScale(new Vector3(1.1f, 0.9f, 1f), .1f).OnComplete(() => {
                    txtMoney.transform.DOScale(vectorDefault, .1f).OnComplete(() =>
                    {
                        lastString = currentString;
                        txtMoney.text = currentString;
                        onChangeText = false;
                    });
                });
            });
        }
        
    }
}
