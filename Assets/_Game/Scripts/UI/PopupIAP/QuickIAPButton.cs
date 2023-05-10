using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickIAPButton : MonoBehaviour
{
    public OfferID offerId;
    [SerializeField] Button btn;
    private void Start()
    {
        btn.onClick.AddListener(IAPOnClick);
    }
    public void Check()
    {
        if (ProfileManager.Instance.playerData.globalResourceSave.CheckBoughtPack(offerId)
            || ProfileManager.Instance.playerData.GetCurrentLevel() < 2)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

    public void IAPOnClick()
    {
        UIManager.instance.ShowPanelPopupIAP(offerId);
    }
}
