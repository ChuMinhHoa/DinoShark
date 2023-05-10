using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelPopupIAP : UIPanel
{
    [SerializeField] Button btnExit;
    [SerializeField] List<IAPPack> popupPacks;

    public override void Awake()
    {
        panelType = UIPanelType.PanelPopupIAP;
        base.Awake();
        btnExit.onClick.AddListener(ClosePanel);
    }

    public void OpenPackInfo(OfferID offerID)
    {
        for (int i = 0; i < popupPacks.Count; i++)
        {
            if(popupPacks[i].offerId == offerID)
            {
                popupPacks[i].gameObject.SetActive(true);
            }
            else
            {
                popupPacks[i].gameObject.SetActive(false);
            }
        }
    }

    void ClosePanel()
    {
        SoundManager.instance.PlaySoundEffect(SoundID.UI_BUTTON);
        if (openAndCloseAnim != null)
            openAndCloseAnim.OnClose(UIManager.instance.ClosePanelPopupIAP);
        else UIManager.instance.ClosePanelPopupIAP();
    }
}
