using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GemShowOnText : MonoBehaviour
{
    [SerializeField] Text txtGem;
    string lastString;
    string currentString;
    bool onChangeText = false;
    [SerializeField] Vector3 vectorScale;
    [SerializeField] Vector3 vectorDefault = new Vector3(1, 1, 1);
    [SerializeField] Button buyGemBtn;
    private void Start()
    {
        lastString = txtGem.text;
        currentString = txtGem.text;
        EventManager.AddListener(EventName.ChangeGem.ToString(), OnChangeGemCount);
        buyGemBtn.onClick.AddListener(BuyGem);
        OnChangeGemCount();
    }
    void OnChangeGemCount() {
        currentString = ProfileManager.Instance.playerData.globalResourceSave.GetGem().ToString();
        if (currentString != lastString && !onChangeText)
        {
            onChangeText = true;
            txtGem.transform.DOScale(vectorScale, .1f).OnComplete(() => {
                txtGem.transform.DOScale(new Vector3(1.1f, 0.9f, 1f), .1f).OnComplete(() => {
                    txtGem.transform.DOScale(vectorDefault, .1f).OnComplete(() =>
                    {
                        lastString = currentString;
                        txtGem.text = currentString;
                        onChangeText = false;
                    });
                });
            });
        }
    }

    void BuyGem()
    {
        UIManager.instance.ShowPanelShop(true);
    }

    public void CheckCondition()
    {
        buyGemBtn.gameObject.SetActive(ProfileManager.Instance.playerData.currentLevel >= 2);
    } 
}
