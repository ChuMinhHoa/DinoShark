using SDK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelTotal : MonoBehaviour
{
    [SerializeField] Button btnUpgrade;
    [SerializeField] Button btnSkin;
    [SerializeField] Button btnMerchandise;
    [SerializeField] Button btnExpand;
    [SerializeField] Button btnShop;
    [SerializeField] Button btnMarketing;
    [SerializeField] Button btnInvite;
    [SerializeField] Button btnSetting;
    [SerializeField] Button btnDailyReward;

    [SerializeField] RectTransform rightGroup;

    [SerializeField] Slider process;

    [SerializeField] Text txtTimeRemainInvite;

    [SerializeField] GameObject objTextTimeInvite;
    [SerializeField] GameObject imgNoticeExpand;
    [SerializeField] GameObject upgradeAlert;
    [SerializeField] GameObject objDailyNotice;
    [SerializeField] UIUpgradeSlot upgradeSlot;
    [SerializeField] List<QuickIAPButton> quickIAPButtons;

    public Transform pointGemAdd;

    public AdsBoostBtn adsBoostBtn;
    public AdsRewardButton adsRewardButton;
    public RectTransform m_PanelContent;
    public RectTransform m_PanelBannerAds;

    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Button showUIBtn;
    [SerializeField] ParticleSystem ps_Process;

    [SerializeField] UIBtnShop uIBtnShop;

    //[Header("Hack Button")]
    //[SerializeField] Button btnHackLenLevel;

    [SerializeField] GemShowOnText gemShow;

    private void Start()
    {
        btnUpgrade.onClick.AddListener(OpenUpgradePanel);
        btnSkin.onClick.AddListener(OpenSkinPanel);
        btnMerchandise.onClick.AddListener(OpenMerchandisePanel);
        btnShop.onClick.AddListener(OpenShopPanel);
        btnExpand.onClick.AddListener(OpenExpandPanle);
        btnMarketing.onClick.AddListener(OpenPanelMarketing);
        btnInvite.onClick.AddListener(OpenPanelInvite);
        btnSetting.onClick.AddListener(OpenPanelSetting);
        btnDailyReward.onClick.AddListener(OpenPanelDailyReward);
        //EventManager.AddListener("UpdateRemoteConfigs", UpdateBannerAdsStatus);
        //ActiveBannerAds(false);
        CheckScreenObstacle();
        //UpdateBannerAdsStatus();
        UpdateTimeOnButtonInvite();
        EventManager.AddListener(EventName.UpdateTotalPanel.ToString(), UpdateStatus);
        UpdateStatus();
        showUIBtn.onClick.AddListener(ShowUI);
        showUIBtn.gameObject.SetActive(false);
        //btnHackLenLevel.onClick.AddListener(ToNextLevel);
    }

    void CheckScreenObstacle()
    {
        float screenRatio = (float)Screen.height / (float)Screen.width;
        if(screenRatio > 2.1f) // Now we got problem 
        {
            m_PanelContent.sizeDelta = new Vector2(0, -100);
            m_PanelContent.anchoredPosition = new Vector2(0, -50);
        }
        else
        {
            m_PanelContent.sizeDelta = new Vector2(0, 0);
            m_PanelContent.anchoredPosition = new Vector2(0, 0);
        }
    }

    public void CheckUICondition()
    {
        CheckAdsBoost();
        CheckQuickIAP();
        gemShow.CheckCondition();
    }

    public void ActiveBtnDaily(bool active) {
        btnDailyReward.gameObject.SetActive(active);
    }

    public void SetActiveNoticeDailyReward(bool active) {
        objDailyNotice.SetActive(active);
    }

    void CheckAdsBoost()
    {
        adsBoostBtn.gameObject.SetActive((ProfileManager.Instance.playerData.GetCurrentLevel() >= 1));
    }

    public void CheckQuickIAP()
    {
        for (int i = 0; i < quickIAPButtons.Count; i++)
        {
            quickIAPButtons[i].Check();
        }
    }

    //void ToNextLevel() {
    //    for (int i = 0; i < System.Enum.GetNames(typeof(TutorialType)).Length; i++)
    //    {
    //        ProfileManager.Instance.playerData.tutorialSave.AddTutorialDone((TutorialType)i);
    //    }
    //    ProfileManager.Instance.playerData.SaveCurrentLevel((ProfileManager.Instance.playerData.currentLevel + 1));
    //    ProfileManager.Instance.playerData.SaveData();
    //    LoadSceneManager.Instance.ChangeLevel();
    //}

    void UpdateTimeOnButtonInvite() {
        if (!InviteHelperManager.Instance.isOnInvited) {
            CloseTimeInvited();
        }
        else { 
            ShowTimeInvited();
        }
    }
    public void UpdateBannerAdsStatus()
    {
        //if (AdsManager.Instance.IsGoodToShowBannerAds())
        //{
        //    AdsManager.Instance.ShowBannerAds();
        //    ActiveBannerAds(true);
        //}
        //else
        //{
        //    ActiveBannerAds(false);
        //}
    }
    public void UpdateProcess() {
        process.value = ProfileManager.Instance.playerData.GetUpgradeFoodProcess();
    }
    private void UpdateStatus()
    {
        if (ProfileManager.Instance.playerData.GetUpgradeSave().CheckUpgradeAvailable())
        {
            upgradeAlert.SetActive(true);
            if(ProfileManager.Instance.playerData.currentLevel < 2)
            {
                if(ProfileManager.Instance.playerData.tutorialSave.IsDoneTutorial(TutorialType.Upgrade))
                {
                    upgradeSlot.gameObject.SetActive(true);
                    upgradeSlot.InitData(ProfileManager.Instance.playerData.GetUpgradeSave().GetUpgradeAvailable());
                }
                else
                {
                    upgradeSlot.gameObject.SetActive(false);
                }
            }
            else
            {
                upgradeSlot.gameObject.SetActive(false);
            }
        }
        else
        {
            upgradeAlert.SetActive(false);
            upgradeSlot.gameObject.SetActive(false);
        }
        imgNoticeExpand.SetActive(ProfileManager.Instance.playerData.GetMenuSave().CanExpandNewMap());
    }
    void OpenSkinPanel() {
        UIManager.instance.ShowPanelSkin();
    }
    void OpenUpgradePanel() {
        UIManager.instance.ShowPanelUpgrade();
    }
    void OpenMerchandisePanel() {
        UIManager.instance.ShowPanelMerchandise();
    }
    void OpenShopPanel()
    {
        UIManager.instance.ShowPanelShop();
    }
    void OpenExpandPanle()
    {
        UIManager.instance.ShowPanelBusinessGrowth();
    }

    void OpenPanelMarketing()
    {
        UIManager.instance.ShowPanelMarketing();
    }

    void OpenPanelInvite() {
        UIManager.instance.ShowPanelIntive();
    }

    void OpenPanelSetting()
    {
        UIManager.instance.ShowPanelSetting();
    }
    
    void OpenPanelDailyReward()
    {
        UIManager.instance.ShowPanelDailyReward();
    }

    public void OnOpen() {
        gameObject.SetActive(true);
        process.value = ProfileManager.Instance.playerData.GetUpgradeFoodProcess();
        uIBtnShop.LoadData();
    }
    public void OnClose() { gameObject.SetActive(false); }

    public void ActiveBannerAds(bool isActive)
    {
        //m_PanelBannerAds.gameObject.SetActive(isActive);
        //if (isActive)
        //{
        //    m_PanelContent.sizeDelta = new Vector2(0, -200);
        //    m_PanelContent.anchoredPosition = new Vector2(0, 100);
        //}
        //else
        //{
        //    m_PanelContent.sizeDelta = new Vector2(0, 0);
        //    m_PanelContent.anchoredPosition = new Vector2(0, 0);
        //}
    }

    public void ShowTimeInvited() {
        LayoutRebuilder.MarkLayoutForRebuild(rightGroup);
        objTextTimeInvite.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(rightGroup);
    }

    public void CloseTimeInvited() {
        LayoutRebuilder.MarkLayoutForRebuild(rightGroup);
        objTextTimeInvite.SetActive(false);
        LayoutRebuilder.ForceRebuildLayoutImmediate(rightGroup);
    }

    public void ChangeTimeInvitedText(string timeChange) {
        txtTimeRemainInvite.text = timeChange;
    }

    public void ShowAdsReward()
    {
        adsRewardButton.InitReward();
    }

    #region Ads Reward
    bool adsRewardShowing = false;
    float adsRewardCoolDown = 60;
    float adsRewardCounter;
    private void Update()
    {
        if(!adsRewardShowing)
        {
            adsRewardCounter += Time.deltaTime;
            if (adsRewardCounter >= adsRewardCoolDown)
            {
                adsRewardShowing = true;
                ShowAdsReward();
                adsRewardCounter = 0;
            }
        }
    }
    public void SetAdsRewardShowing(bool value)
    {
        adsRewardShowing = value;
    }
    #endregion

    bool showing = true;
    public void ShowUI()
    {
        if(showing)
        {
            canvasGroup.alpha = 0;
            showUIBtn.gameObject.SetActive(true);
        }
        else
        {
            canvasGroup.alpha = 1;
            showUIBtn.gameObject.SetActive(false);
        }
        showing = !showing;
    }
}
