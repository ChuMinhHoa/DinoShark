using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public enum UIPanelType {
    PanelMachineInfor,
    PanelUpgrade,
    PanelMerchandise,
    PanelOffline,
    PanelShop,
    PanelBusinessGrowth,
    PanelConfirm,
    PanelSkin,
    PanelLoadScene,
    PanelMarketing,
    PanelTutorial,
    PanelInvite,
    PanelSetting,
    PanelOpenOutfit,
    PanelDetailOutfit,
    PanelPopupIAP,
    PanelCheat,
    PanelDailyReward
}
public class UIPanel : MonoBehaviour {
    public bool isRegisterInUI = true;
    protected UIPanelType panelType;
    public UIPanelAnimOpenAndClose openAndCloseAnim;

    // Start is called before the first frame update
    public virtual void Awake() {
        if (isRegisterInUI) UIManager.instance.RegisterPanel(panelType, gameObject);
    }
}
