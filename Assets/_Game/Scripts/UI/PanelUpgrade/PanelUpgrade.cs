using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using DG.Tweening;

public class PanelUpgrade : UIPanel
{
    [Header("PanelUpgrade")]
    [SerializeField] UIUpgradeSlot uIUpgradeSlot;
    [SerializeField] Transform mainContent;
    [SerializeField] Button btnExit;
    [SerializeField] List<UIUpgradeSlot> listUpgradeSlots = new List<UIUpgradeSlot>();
    [SerializeField] SkeletonGraphic sharkBinh;
    [SerializeField] Transform pointAddGem;
    [SerializeField] Slider slider;
    int maxDeposit;
    [SerializeField] Text txtGemCount;
    int totalUpgradeCount = 0;
    List<UpgradeData> upgradeDatasRemain;
    PlayerData playerData;
    UIManager uIManager;
    UpgradeDataConfig upgradeData;
    [SerializeField] GameObject upgradeListObject;
    [SerializeField] GameObject blankObject;
    int level = -1;
    public override void Awake()
    {
        panelType = UIPanelType.PanelUpgrade;
        base.Awake();
        playerData = ProfileManager.Instance.playerData;
        upgradeData = ProfileManager.Instance.dataConfig.upgradeDataConfig;
        uIManager = UIManager.instance;
        txtGemCount.text = ProfileManager.Instance.playerData.globalResourceSave.depositGems.ToString();
        maxDeposit = playerData.globalResourceSave.GetMaxDeposit();
        btnExit.onClick.AddListener(ClosePanel);
    }
    public void OnOpen() {
        IdleSkarkAnim();
        slider.value = (float)ProfileManager.Instance.playerData.globalResourceSave.depositGems / (float)maxDeposit;

        if (level != playerData.currentLevel)
        {
            level = playerData.currentLevel;
            upgradeDatasRemain = playerData.GetUpgradeSave().GetRemainUpgrades();
            totalUpgradeCount = upgradeData.GetUpgradeDataByLevel(level).upgradeDatas.Count;
            OnInitData();
        }
        else
        {
            for (int i = 0; i < listUpgradeSlots.Count; i++)
            {
                listUpgradeSlots[i].CheckUpgraded();
            }
        }
        CheckOutOfUpgade();
    }
    void OnInitData() {
        float floor = -1;
        for (int i = 0; i < upgradeDatasRemain.Count; i++)
        {
            if (i < listUpgradeSlots.Count)
            {
                listUpgradeSlots[i].InitData(upgradeDatasRemain[i], pointAddGem, floor);
            }
            else
            {
                UIUpgradeSlot newUIUpgradeSlot = Instantiate(uIUpgradeSlot, mainContent);
                newUIUpgradeSlot.InitData(upgradeDatasRemain[i], pointAddGem, floor);
                listUpgradeSlots.Add(newUIUpgradeSlot);
            }
            if (upgradeDatasRemain[i].upgradeType == UpgradeType.AddFloor)
            {
                floor = upgradeDatasRemain[i].refAmount;
            }
        }
    }

    public void ClosePanel() {
        SoundManager.instance.PlaySoundEffect(SoundID.UI_BUTTON);
        StopCoroutine("IE_WaitToBack");
        if (openAndCloseAnim != null)
            openAndCloseAnim.OnClose(uIManager.ClosePanelUpgrade);
        else uIManager.ClosePanelUpgrade();
    }

    public void CheckOutOfUpgade()
    {
        upgradeListObject.SetActive(!ProfileManager.Instance.playerData.GetUpgradeSave().IsOutOfUpgrade(totalUpgradeCount));
        blankObject.SetActive(ProfileManager.Instance.playerData.GetUpgradeSave().IsOutOfUpgrade(totalUpgradeCount));
    }

    public void IdleSkarkAnim() {
        txtGemCount.text = ProfileManager.Instance.playerData.globalResourceSave.depositGems.ToString();
        sharkBinh.AnimationState.SetAnimation(0, "idle", loop: true);
    }

    public void SupriseSkarkAnim()
    {
        sharkBinh.AnimationState.SetAnimation(0, "suprise", loop: false);
        slider.value = (float)ProfileManager.Instance.playerData.globalResourceSave.depositGems / (float)maxDeposit;
        txtGemCount.text = ProfileManager.Instance.playerData.globalResourceSave.depositGems.ToString();
        //sharkBinh.AnimationState.End += delegate {
        //    IdleSkarkAnim();
        //};
        //StartCoroutine(IE_WaitToBack());
    }

    IEnumerator IE_WaitToBack() {
        yield return new WaitForSeconds(.7f);
        IdleSkarkAnim();
    }
}
