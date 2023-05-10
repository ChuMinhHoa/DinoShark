using SDK;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;

public class UIManager : MonoBehaviour {
    public static UIManager instance;
    [SerializeField] Transform mainCanvas;
    public  PanelTotal panelTotal;
    public PanelTutorial panelTutorial;
    Dictionary<UIPanelType, GameObject> listPanel = new Dictionary<UIPanelType, GameObject>();
    public bool isHasPopupOnScene = false;
    public bool isHasTutorial = false;
    public bool isHasPriorityPanel = false;
    // Start is called before the first frame update
    void Awake() {
        instance = this;
    }
    void CloseAllPopUp() {
        isHasPopupOnScene = false;
        foreach (KeyValuePair<UIPanelType, GameObject> panel in listPanel)
        {
            if (panel.Key != UIPanelType.PanelOffline)
                panel.Value.gameObject.SetActive(false);
        }      
    }
    public bool IsHavePopUpOnScene() { return isHasPopupOnScene; }
    public bool IsHaveTutorial() { return isHasTutorial; }
    public void RegisterPanel(UIPanelType type, GameObject obj)
    {
        GameObject go = null;
        if (!listPanel.TryGetValue(type, out go))
        {
            Debug.Log("RegisterPanel " + type.ToString());
            listPanel.Add(type, obj);
        }
        obj.SetActive(false);
    }
    public GameObject GetPanel(UIPanelType type) {
        GameObject panel = null;
        if (!listPanel.TryGetValue(type, out panel)) {
            switch (type) {
                case UIPanelType.PanelMachineInfor:
                    panel = Instantiate(Resources.Load("UI/PanelMachineInfor") as GameObject, mainCanvas);
                    break;
                case UIPanelType.PanelUpgrade:
                    panel = Instantiate(Resources.Load("UI/PanelUpgrade") as GameObject, mainCanvas);
                    break;
                case UIPanelType.PanelMerchandise:
                    panel = Instantiate(Resources.Load("UI/PanelMerchandise") as GameObject, mainCanvas);
                    break;
                case UIPanelType.PanelOffline:
                    panel = Instantiate(Resources.Load("UI/PanelOffline") as GameObject, mainCanvas);
                    break;
                case UIPanelType.PanelShop:
                    panel = Instantiate(Resources.Load("UI/PanelShop") as GameObject, mainCanvas);
                    break;
                case UIPanelType.PanelBusinessGrowth:
                    panel = Instantiate(Resources.Load("UI/PanelBusinessGrowth") as GameObject, mainCanvas);
                    break;
                case UIPanelType.PanelConfirm:
                    panel = Instantiate(Resources.Load("UI/PanelConfirm") as GameObject, mainCanvas);
                    break;
                case UIPanelType.PanelSkin:
                    panel = Instantiate(Resources.Load("UI/PanelSkin") as GameObject, mainCanvas);
                    break;
                case UIPanelType.PanelLoadScene:
                    panel = Instantiate(Resources.Load("UI/PanelLoadScene") as GameObject, mainCanvas);
                    break;
                case UIPanelType.PanelMarketing:
                    panel = Instantiate(Resources.Load("UI/PanelMarketing") as GameObject, mainCanvas);
                    break;
                case UIPanelType.PanelInvite:
                    panel = Instantiate(Resources.Load("UI/PanelInvite") as GameObject, mainCanvas);
                    break;
                case UIPanelType.PanelSetting:
                    panel = Instantiate(Resources.Load("UI/PanelSetting") as GameObject, mainCanvas);
                    break;
                case UIPanelType.PanelOpenOutfit:
                    panel = Instantiate(Resources.Load("UI/PanelOpenOutfit") as GameObject, mainCanvas);
                    break;
                case UIPanelType.PanelDetailOutfit:
                    panel = Instantiate(Resources.Load("UI/PanelDetailOutfit") as GameObject, mainCanvas);
                    break;
                case UIPanelType.PanelPopupIAP:
                    panel = Instantiate(Resources.Load("UI/PanelPopupPack") as GameObject, mainCanvas);
                    break;
                case UIPanelType.PanelCheat:
                    panel = Instantiate(Resources.Load("UI/PanelCheat") as GameObject, mainCanvas);
                    break;
                case UIPanelType.PanelDailyReward:
                    panel = Instantiate(Resources.Load("UI/PanelDailyReward") as GameObject, mainCanvas);
                    break;
            }
            if (panel) panel.SetActive(true);
            return panel;
        }
        return listPanel[type];
    }
    public void ShowPanelMachineInfor(IMachineController selectedRoom) {
        CloseAllPopUp();
        SoundManager.instance.PlaySoundEffect(SoundID.UI_POPUP);
        isHasPopupOnScene = true;
        GameObject go = GetPanel(UIPanelType.PanelMachineInfor);
        PanelMachineInfor panelMachine = go.GetComponent<PanelMachineInfor>();
        panelMachine.SetUpToShow(selectedRoom);
        panelMachine.OnOpen();
    }
    public void ClosePanelMachineInfor() {
        isHasPopupOnScene = false;
        GameObject go = GetPanel(UIPanelType.PanelMachineInfor);
        PanelMachineInfor panelMachine = go.GetComponent<PanelMachineInfor>();
        panelMachine.Close();
    }
    public void ShowPanelUpgrade() {
        CloseAllPopUp();
        SoundManager.instance.PlaySoundEffect(SoundID.UI_POPUP);
        isHasPopupOnScene = true;
        GameObject go = GetPanel(UIPanelType.PanelUpgrade);
        go.SetActive(true);
        go.GetComponent<PanelUpgrade>().OnOpen();
    }
    public void ClosePanelUpgrade() {
        isHasPopupOnScene = false;
        GameObject go = GetPanel(UIPanelType.PanelUpgrade);
        go.SetActive(false);
        if (!ProfileManager.Instance.playerData.globalResourceSave.removeAds)
            AdsManager.Instance.ShowInterstitial(null, null);
    }

    public void ShowPanelMerchandise()
    {
        CloseAllPopUp();
        SoundManager.instance.PlaySoundEffect(SoundID.UI_POPUP);
        isHasPopupOnScene = true;
        GameObject go = GetPanel(UIPanelType.PanelMerchandise);
        go.SetActive(true);
    }
    public void ClosePanelMerchandise()
    {
        isHasPopupOnScene = false;
        GameObject go = GetPanel(UIPanelType.PanelMerchandise);
        go.SetActive(false);
        if (!ProfileManager.Instance.playerData.globalResourceSave.removeAds)
            AdsManager.Instance.ShowInterstitial(null, null);
    }

    public void ShowPanelOffline(TimeSpan span)
    {
        CloseAllPopUp();
        SoundManager.instance.PlaySoundEffect(SoundID.UI_POPUP);
        isHasPopupOnScene = true;
        isHasPriorityPanel = true;
        GameObject go = UIManager.instance.GetPanel(UIPanelType.PanelOffline);
        go.SetActive(true);
        go.GetComponent<PanelOffline>().SetUp(span);
    }

    public void ClosePanelOffline()
    {
        isHasPopupOnScene = false;
        isHasPriorityPanel = false;
        GameObject go = GetPanel(UIPanelType.PanelOffline);
        go.SetActive(false);
    }


    public void ShowPanelShop(bool scroll = false)
    {
        CloseAllPopUp();
        SoundManager.instance.PlaySoundEffect(SoundID.UI_POPUP);
        isHasPopupOnScene = true;
        GameObject go = GetPanel(UIPanelType.PanelShop);
        go.SetActive(true);
        go.GetComponent<PanelShop>().InnitShop(scroll);
    }
    public void ClosePanelShop()
    {
        isHasPopupOnScene = false;
        GameObject go = GetPanel(UIPanelType.PanelShop);
        go.SetActive(false);
        if (!ProfileManager.Instance.playerData.globalResourceSave.removeAds)
            AdsManager.Instance.ShowInterstitial(null, null);
    }
    public void ShowPanelBusinessGrowth()
    {
        CloseAllPopUp();
        SoundManager.instance.PlaySoundEffect(SoundID.UI_POPUP);
        isHasPopupOnScene = true;
        GameObject go = GetPanel(UIPanelType.PanelBusinessGrowth);
        go.SetActive(true);
        PanelBusinessGrowth panelBusinessGrowth = go.GetComponent<PanelBusinessGrowth>();
        panelBusinessGrowth.OnOpen();
    }

    public void ClosePanelBusinessGrowth()
    {
        isHasPopupOnScene = false;
        GameObject go = GetPanel(UIPanelType.PanelBusinessGrowth);
        go.SetActive(false);
    }

    public void ShowPanelConfirm()
    {
        SoundManager.instance.PlaySoundEffect(SoundID.UI_POPUP);
        isHasPopupOnScene = true;
        GameObject go = GetPanel(UIPanelType.PanelConfirm);
        go.SetActive(true);
    }

    public void ClosePanelConfirm()
    {
        isHasPopupOnScene = false;
        GameObject go = GetPanel(UIPanelType.PanelConfirm);
        go.SetActive(false);
    }
    public void ShowPanelSkin() {
        CloseAllPopUp();
        SoundManager.instance.PlaySoundEffect(SoundID.UI_POPUP);
        isHasPopupOnScene = true;
        isHasPriorityPanel = true;
        GameObject go = GetPanel(UIPanelType.PanelSkin);
        go.SetActive(true);
        PanelOutfit.Instance.InitData();
    }
    public void ClosePanelSkin()
    {
        isHasPopupOnScene = false;
        isHasPriorityPanel = false;
        GameObject go = GetPanel(UIPanelType.PanelSkin);
        go.SetActive(false);
        if (!ProfileManager.Instance.playerData.globalResourceSave.removeAds)
            AdsManager.Instance.ShowInterstitial(null, null);
    }

    public void ShowLoadScene() {
        isHasPopupOnScene = true;
        GameObject go = GetPanel(UIPanelType.PanelLoadScene);
        go.SetActive(true);
    }

    public void CloseLoadScene()
    {
        isHasPopupOnScene = false;
        GameObject go = GetPanel(UIPanelType.PanelLoadScene);
        go.SetActive(false);
    }

    public PanelTotal GetTotalPanel()
    {
        return panelTotal;
    }
    public void ShowPanelTotal()
    {
        panelTotal.OnOpen();
    }

    public void ClosePanelTotal()
    {
        panelTotal.OnClose();
    }

    public void ShowPanelMarketing()
    {
        SoundManager.instance.PlaySoundEffect(SoundID.UI_POPUP);
        isHasPopupOnScene = true;
        GameObject go = GetPanel(UIPanelType.PanelMarketing);
        go.SetActive(true);
        go.GetComponent<PanelMarketing>().InitData();
    }
    public void ClosePanelMarketing()
    {
        isHasPopupOnScene = false;
        GameObject go = GetPanel(UIPanelType.PanelMarketing);
        go.SetActive(false);
        if (!ProfileManager.Instance.playerData.globalResourceSave.removeAds)
            AdsManager.Instance.ShowInterstitial(null, null);
    }
    public void ShowPanelTutorial()
    {
        isHasTutorial = true;
        CloseAllPopUp();
        panelTutorial.gameObject.SetActive(true);
    }

    public void ClosePanelTutorial()
    {
        isHasTutorial = false;
        panelTutorial.gameObject.SetActive(false);
    }

    public void ShowPanelIntive() {
        SoundManager.instance.PlaySoundEffect(SoundID.UI_POPUP);
        if (InviteHelperManager.Instance.GetTimeRemain() > 0) {
            //On Invited
            return;
        }
        isHasPopupOnScene = true;
        GameObject go = GetPanel(UIPanelType.PanelInvite);
        go.SetActive(true);
    }

    public void ClosePanelInvite() {
        isHasPopupOnScene = false;
        GetPanel(UIPanelType.PanelInvite).SetActive(false);
        if (!ProfileManager.Instance.playerData.globalResourceSave.removeAds)
            AdsManager.Instance.ShowInterstitial(null, null);
    }

    public void ShowPanelSetting()
    {
        SoundManager.instance.PlaySoundEffect(SoundID.UI_POPUP);
        isHasPopupOnScene = true;
        GameObject go = GetPanel(UIPanelType.PanelSetting);
        go.SetActive(true);
    }
    public void ClosePanelSetting()
    {
        isHasPopupOnScene = false;
        GameObject go = GetPanel(UIPanelType.PanelSetting);
        go.SetActive(false);
        if (!ProfileManager.Instance.playerData.globalResourceSave.removeAds)
            AdsManager.Instance.ShowInterstitial(null, null);
    }

    public void ShowPanelOpenOutfit(OfferID offer, int outfitID, OutfitType outfitType, Rarity rarity, bool rewardPack = false)
    {
        SoundManager.instance.PlaySoundEffect(SoundID.UI_POPUP);
        isHasPopupOnScene = true;
        GameObject go = GetPanel(UIPanelType.PanelOpenOutfit);
        go.SetActive(true);
        go.GetComponent<PanelOpenOutfit>().Setup(offer, outfitID, outfitType, rarity, rewardPack);
    }

    public void ClosePanelOpenOutfit()
    {
        isHasPopupOnScene = false;
        GameObject go = GetPanel(UIPanelType.PanelOpenOutfit);
        go.SetActive(false);
    }

    public void ShowPanelOutfitDetail(int outfitId) {
        SoundManager.instance.PlaySoundEffect(SoundID.UI_POPUP);
        isHasPopupOnScene = true;
        GameObject go = GetPanel(UIPanelType.PanelDetailOutfit);
        go.SetActive(true);
        go.GetComponent<PanelOutfitDetail>().InitData(outfitId);
        go.transform.SetAsLastSibling();
    }

    public void ShowPanelOutfitDetail(InviteOutfitRandom inviteOutfitRandom) {
        SoundManager.instance.PlaySoundEffect(SoundID.UI_POPUP);
        isHasPopupOnScene = true;
        GameObject go = GetPanel(UIPanelType.PanelDetailOutfit);
        go.SetActive(true);
        go.GetComponent<PanelOutfitDetail>().InitDataOnInvitePanel(inviteOutfitRandom); 
        go.transform.SetAsLastSibling();
    }

    public void ShowPanelOutfitDetail(ItemEquip itemEquip) {
        SoundManager.instance.PlaySoundEffect(SoundID.UI_POPUP);
        isHasPopupOnScene = true;
        GameObject go = GetPanel(UIPanelType.PanelDetailOutfit);
        go.SetActive(true);
        go.GetComponent<PanelOutfitDetail>().InitDataOnDailyRewardPanel(itemEquip);
        go.transform.SetAsLastSibling();
    }

    public void ClosePanelOufitDetail()
    {
        GameObject go = GetPanel(UIPanelType.PanelDetailOutfit);
        go.SetActive(false);
        if (!ProfileManager.Instance.playerData.globalResourceSave.removeAds)
            AdsManager.Instance.ShowInterstitial(null, null);
    }

    public void ShowPanelPopupIAP(OfferID offerID)
    {
        SoundManager.instance.PlaySoundEffect(SoundID.UI_POPUP);
        isHasPopupOnScene = true;
        GameObject go = GetPanel(UIPanelType.PanelPopupIAP);
        go.SetActive(true);
        go.GetComponent<PanelPopupIAP>().OpenPackInfo(offerID);
    }
    public void ClosePanelPopupIAP()
    {
        isHasPopupOnScene = false;
        GameObject go = GetPanel(UIPanelType.PanelPopupIAP);
        go.SetActive(false);
    }

    public void ShowPanelCheat()
    {
        CloseAllPopUp();
        SoundManager.instance.PlaySoundEffect(SoundID.UI_POPUP);
        isHasPopupOnScene = true;
        GameObject go = GetPanel(UIPanelType.PanelCheat);
        go.SetActive(true);
    }
    public void ClosePanelCheat()
    {
        isHasPopupOnScene = false;
        GameObject go = GetPanel(UIPanelType.PanelCheat);
        go.SetActive(false);
        //if (!ProfileManager.Instance.playerData.globalResourceSave.removeAds)
        //    AdsManager.Instance.ShowInterstitial(null, null);
    }

    public void ShowPanelDailyReward() {
        SoundManager.instance.PlaySoundEffect(SoundID.UI_POPUP);
        isHasPopupOnScene = true;
        GameObject go = GetPanel(UIPanelType.PanelDailyReward);
        go.SetActive(true);
    }
    public void ClosePanelDailyReward()
    {
        isHasPopupOnScene = false;
        GameObject go = GetPanel(UIPanelType.PanelDailyReward);
        go.SetActive(false);
        if (!ProfileManager.Instance.playerData.globalResourceSave.removeAds)
            AdsManager.Instance.ShowInterstitial(null, null);
    }

}
